using FDK.BassMixExtension;
using NAudio.Wave;

namespace FDK;

// CSound は、サウンドデバイスが変更されたときも、インスタンスを再作成することなく、新しいデバイスで作り直せる必要がある。
// そのため、デバイスごとに別のクラスに分割するのではなく、１つのクラスに集約するものとする。

public class CSound : IDisposable
{
    public const int MinimumSongVol = 0;
    public const int MaximumSongVol = 200; // support an approximate doubling in volume.
    public const int DefaultSongVol = 100;

    // 2018-08-19 twopointzero: Note the present absence of a MinimumAutomationLevel.
    // We will revisit this if/when song select BGM fade-in/fade-out needs
    // updating due to changing the type or range of AutomationLevel
    public const int MaximumAutomationLevel = 100;
    public const int DefaultAutomationLevel = 100;

    public const int MinimumGroupLevel = 0;
    public const int MaximumGroupLevel = 100;
    public const int DefaultGroupLevel = 100;
    public const int DefaultSoundEffectLevel = 80;
    public const int DefaultVoiceLevel = 90;
    public const int DefaultSongPreviewLevel = 75;
    public const int DefaultSongPlaybackLevel = 90;

    public static readonly Lufs MinimumLufs = new Lufs(-100.0);
    public static readonly Lufs MaximumLufs = new Lufs(10.0); // support an approximate doubling in volume.

    private static readonly Lufs DefaultGain = new Lufs(0.0);

    public readonly ESoundGroup SoundGroup;

    #region [ DTXMania用拡張 ]

    public int nDurationms => SoundImpl?.nDurationms ?? 0;
    public double dbPlaySpeed
    {
        get => SoundImpl?.dbPlaySpeed ?? 1.0f;
        set
        {
            if (SoundImpl is not null)
            {
                SoundImpl.dbPlaySpeed = value;
            }
        }
    }
    #endregion
    public bool b演奏終了後も再生が続くチップである = false;	// これがtrueなら、本サウンドの再生終了のコールバック時に自動でミキサーから削除する

    /// <summary>
    /// Gain is applied "first" to the audio data, much as in a physical or
    /// software mixer. Later steps in the flow of audio apply "channel" level
    /// (e.g. AutomationLevel) and mixing group level (e.g. GroupLevel) before
    /// the audio is output.
    ///
    /// This method, taking an integer representing a percent value, is used
    /// for mixing in the SONGVOL value, when available. It is also used for
    /// DTXViewer preview mode.
    /// </summary>
    public void SetGain(int songVol)
    {
        SetGain(LinearIntegerPercentToLufs(songVol), null);
    }

    private static Lufs LinearIntegerPercentToLufs(int percent)
    {
        // 2018-08-27 twopointzero: We'll use the standard conversion until an appropriate curve can be selected
        return new Lufs(20.0 * Math.Log10(percent / 100.0));
    }

    /// <summary>
    /// Gain is applied "first" to the audio data, much as in a physical or
    /// software mixer. Later steps in the flow of audio apply "channel" level
    /// (e.g. AutomationLevel) and mixing group level (e.g. GroupLevel) before
    /// the audio is output.
    ///
    /// This method, taking a LUFS gain value and a LUFS true audio peak value,
    /// is used for mixing in the loudness-metadata-base gain value, when available.
    /// </summary>
    public void SetGain(Lufs gain, Lufs? truePeak)
    {
        if (Equals(_gain, gain))
        {
            return;
        }

        _gain = gain;
        _truePeak = truePeak;

        if (SoundGroup == ESoundGroup.SongPlayback)
        {
            Trace.TraceInformation($"{nameof(CSound)}.{nameof(SetGain)}: Gain: {_gain}. True Peak: {_truePeak}");
        }

        SetVolume();
    }

    /// <summary>
    /// AutomationLevel is applied "second" to the audio data, much as in a
    /// physical or sofware mixer and its channel level. Before this Gain is
    /// applied, and after this the mixing group level is applied.
    ///
    /// This is currently used only for automated fade in and out as is the
    /// case right now for the song selection screen background music fade
    /// in and fade out.
    /// </summary>
    public int AutomationLevel
    {
        get => _automationLevel;
        set
        {
            if (_automationLevel == value)
            {
                return;
            }

            _automationLevel = value;

            if (SoundGroup == ESoundGroup.SongPlayback)
            {
                Trace.TraceInformation($"{nameof(CSound)}.{nameof(AutomationLevel)} set: {AutomationLevel}");
            }

            SetVolume();
        }
    }

    /// <summary>
    /// GroupLevel is applied "third" to the audio data, much as in the sub
    /// mixer groups of a physical or software mixer. Before this both the
    /// Gain and AutomationLevel are applied, and after this the audio
    /// flows into the audio subsystem for mixing and output based on the
    /// master volume.
    ///
    /// This is currently automatically managed for each sound based on the
    /// configured and dynamically adjustable sound group levels for each of
    /// sound effects, voice, song preview, and song playback.
    ///
    /// See the SoundGroupLevelController and related classes for more.
    /// </summary>
    public int GroupLevel
    {
        private get => _groupLevel;
        set
        {
            if (_groupLevel == value)
            {
                return;
            }

            _groupLevel = value;

            if (SoundGroup == ESoundGroup.SongPlayback)
            {
                Trace.TraceInformation($"{nameof(CSound)}.{nameof(GroupLevel)} set: {GroupLevel}");
            }

            SetVolume();
        }
    }

    private void SetVolume()
    {
        var automationLevel = LinearIntegerPercentToLufs(AutomationLevel);
        var groupLevel = LinearIntegerPercentToLufs(GroupLevel);

        var gain =
            _gain +
            automationLevel +
            groupLevel;

        var safeTruePeakGain = _truePeak?.Negate() ?? new Lufs(0);
        var finalGain = gain.Min(safeTruePeakGain);

        if (SoundGroup == ESoundGroup.SongPlayback)
        {
            Trace.TraceInformation(
                $"{nameof(CSound)}.{nameof(SetVolume)}: Gain:{_gain}. Automation Level: {automationLevel}. Group Level: {groupLevel}. Summed Gain: {gain}. Safe True Peak Gain: {safeTruePeakGain}. Final Gain: {finalGain}.");
        }

        lufsVolume = finalGain;
    }

    protected Lufs lufsVolume
    {
        set
        {
            if (SoundImpl is not null)
            {
                SoundImpl.lufsVolume = value;
            }
        }
    }

    /// <summary>
    /// <para>左:-100～中央:0～100:右。set のみ。</para>
    /// </summary>
    public int nPanning
    {
        get => SoundImpl?.nPanning ?? 0;
        set
        {
            if (SoundImpl is not null)
            {
                SoundImpl.nPanning = value;
            }
        }
    }

    internal CSoundImpl? SoundImpl;

    /// <summary>
    /// <para>全インスタンスリスト。</para>
    /// <para>～を作成する() で追加され、t解放する() or Dispose() で解放される。</para>
    /// </summary>
    public static readonly ObservableCollection<CSound> listインスタンス = new ObservableCollection<CSound>();

    public CSound(ESoundGroup soundGroup)
    {
        SoundGroup = soundGroup;

        listインスタンス.Add(this);
    }

    #region [ DTXMania用の変換 ]

    public void t再生を開始する()
    {
        t再生位置を先頭に戻す();
        tサウンドを再生する();
    }
    public void t再生を開始する(bool bループする) => SoundImpl?.t再生を開始する(bループする);
    public void t再生を停止する()
    {
        tサウンドを停止する();
        t再生位置を先頭に戻す();
    }
    public void t再生を一時停止する()
    {
        tサウンドを停止する();
    }
    public void t再生を再開する(long t)
    {
        Debug.WriteLine("t再生を再開する(long " + t + ")");
        t再生位置を変更する(t);
        tサウンドを再生する();
    }
    public bool b一時停止中 => SoundImpl?.b一時停止中 ?? false;
    public bool bPlaying => SoundImpl?.bPlaying ?? false;
    #endregion


    public void t解放する()
    {
        t解放する(false);
    }

    public virtual void t解放する(bool _bインスタンス削除)
    {
        CSoundManager.nStreams--;

        this.Dispose(true, _bインスタンス削除);   // CSoundの再初期化時は、インスタンスは存続する。
    }
    public void tサウンドを再生する()
    {
        tサウンドを再生する(false);
    }
    protected void tサウンドを再生する(bool bループする) => SoundImpl?.tサウンドを再生する(bループする);

    public void tサウンドを停止する() => SoundImpl?.tサウンドを停止する();

    public void t再生位置を先頭に戻す()
    {
        t再生位置を変更する(0);
    }
    public void t再生位置を変更する(long n位置ms) => SoundImpl?.t再生位置を変更する(n位置ms);

    /// <summary>
    /// デバッグ用
    /// </summary>
    /// <param name="n位置byte"></param>
    /// <param name="db位置ms"></param>
    public void t再生位置を取得する(out long n位置byte, out double db位置ms)
    {
        if (SoundImpl is not null)
        {
            SoundImpl.t再生位置を取得する(out n位置byte, out db位置ms);
            return;
        }

        n位置byte = 0;
        db位置ms = 0;
    }


    public static void tすべてのサウンドを初期状態に戻す()
    {
        foreach (var sound in CSound.listインスタンス)
        {
            sound.t解放する(false);
        }
    }
    internal static void tすべてのサウンドを再構築する(ISoundDevice device)
    {
        if (CSound.listインスタンス.Count == 0)
            return;


        // サウンドを再生する際にインスタンスリストも更新されるので、配列にコピーを取っておき、リストはクリアする。

        var sounds = CSound.listインスタンス.ToArray();
        CSound.listインスタンス.Clear();


        // 配列に基づいて個々のサウンドを作成する。

        for (int i = 0; i < sounds.Length; i++)
        {
            switch (sounds[i].eMakeType)
            {
                #region [ ファイルから ]
                case EMakeType.File:
                    string? strFilename = sounds[i].strFilename;
                    sounds[i].Dispose(true, false);
                    if (strFilename is not null)
                    {
                        device.tCreateSound(strFilename, sounds[i]);
                    }
                    break;
                #endregion
                #region [ WAVファイルイメージから ]
                case EMakeType.WAVFileImage:
                    byte[]? waveStream = sounds[i].byArrWAVファイルイメージ;
                    sounds[i].Dispose(true, false);
                    if (waveStream is not null)
                    {
                        device.tCreateSound(waveStream, sounds[i]);
                    }
                    break;
                    #endregion
            }
        }
    }

    #region [ Dispose-Finalizeパターン実装 ]
    //-----------------
    public void Dispose()
    {
        this.Dispose(true, true);
        GC.SuppressFinalize(this);
    }
    protected void Dispose(bool bManagedも解放する, bool bインスタンス削除)
    {
        SoundImpl?.Dispose(bManagedも解放する);

        if (bManagedも解放する)
        {
            this.eSoundDeviceType = ESoundDeviceType.Unknown;

            if (bインスタンス削除)
            {
                //try
                //{
                //    CSound.listインスタンス.RemoveAt( freeIndex );
                //}
                //catch
                //{
                //    Debug.WriteLine( "FAILED to remove CSound.listインスタンス: Count=" + CSound.listインスタンス.Count + ", filename=" + Path.GetFileName( this.strFilename ) );
                //}
                bool b = CSound.listインスタンス.Remove(this);    // これだと、Clone()したサウンドのremoveに失敗する
                if (!b)
                {
                    Debug.WriteLine("FAILED to remove CSound.listインスタンス: Count=" + CSound.listインスタンス.Count + ", filename=" + Path.GetFileName(this.strFilename));
                }

            }
        }
    }
    ~CSound()
    {
        this.Dispose(false, true);
    }
    //-----------------
    #endregion

    #region [ protected ]
    //-----------------
    public enum EMakeType { File, WAVFileImage, Unknown }
    public EMakeType eMakeType => SoundImpl?.eMakeType ?? EMakeType.Unknown;
    protected ESoundDeviceType eSoundDeviceType = ESoundDeviceType.Unknown;
    public string? strFilename => SoundImpl?.strFilename;
    protected byte[]? byArrWAVファイルイメージ => SoundImpl?.byArrWAVファイルイメージ;  // WAVファイルイメージ、もしくはchunkのDATA部のみ
    //-----------------
    #endregion

    #region [ private ]
    //-----------------
    private Lufs _gain = DefaultGain;
    private Lufs? _truePeak = null;
    private int _automationLevel = DefaultAutomationLevel;
    private int _groupLevel = DefaultGroupLevel;

    #endregion
}
