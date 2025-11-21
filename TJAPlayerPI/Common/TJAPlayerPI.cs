using FDK;
using SkiaSharp;
using System.Security.Principal;
using TJAPlayerPI.Common;
using TJAPlayerPI.Fade;
using TJAPlayerPI.Helper;
using TJAPlayerPI.Saving;
using TJAPlayerPI.Stages.SongSelect;
using TJAPlayerPI.Stages.SongSelect.Legacy;
using static TJAPlayerPI.Cスコア;

namespace TJAPlayerPI;

public class TJAPlayerPI : Game
{
    // プロパティ
    #region [ properties ]
    public static string VERSION
    {
        get
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is null)
                return "null";

            return version.ToString();
        }
    }

    public static TJAPlayerPI app
    {
        get;
        private set;
    }
    internal C文字コンソール act文字コンソール
    {
        get;
        private set;
    }
    internal CConfigIni ConfigIni
    {
        get;
        private set;
    }
    public CConfigToml ConfigToml
    {
        get;
        private set;
    }
    internal static CDTX[] DTX
    {
        get
        {
            return dtx;
        }
        set
        {
            for (int nPlayer = 0; nPlayer < 2; nPlayer++)
            {
                if ((dtx[nPlayer] is not null) && (app is not null))
                {
                    dtx[nPlayer].On非活性化();
                    app.listトップレベルActivities.Remove(dtx[nPlayer]);
                }
            }
            dtx = value;
            for (int nPlayer = 0; nPlayer < 2; nPlayer++)
            {
                if ((dtx[nPlayer] is not null) && (app is not null))
                {
                    app.listトップレベルActivities.Add(dtx[nPlayer]);
                }
            }
        }
    }

    public static bool IsPerformingCalibration;

    public CFPS FPS
    {
        get;
        private set;
    }

    public CInputManager InputManager
    {
        get;
        private set;
    }
    public CPad Pad
    {
        get;
        private set;
    }
    internal CSkin Skin
    {
        get;
        private set;
    }
    internal static CSongsManager SongsManager
    {
        get;
        set;    // 2012.1.26 yyagi private解除 CStage起動でのdesirialize読み込みのため
    }
    internal static CEnumSongs EnumSongs
    {
        get;
        private set;
    }
    internal static CActEnumSongs actEnumSongs
    {
        get;
        private set;
    }
    internal static CActScanningLoudness actScanningLoudness
    {
        get;
        private set;
    }

    public static CSoundManager SoundManager
    {
        get;
        private set;
    }

    public static SongGainController SongGainController
    {
        get;
        private set;
    }

    public static SoundGroupLevelController SoundGroupLevelController
    {
        get;
        private set;
    }

    private static CStageStartUp stageStartUp
    {
        get;
        set;
    }
    private static CStageTitle stageTitle
    {
        get;
        set;
    }
    private static CStageConfig stageConfig
    {
        get;
        set;
    }
    [Obsolete]
    private static CStage選曲Legacy stage選曲Legacy
    {
        get;
        set;
    }
    /*
    private static CStageSongLoading stageSongLoading
    {
        get;
        set;
    }
    */
    private static CStage演奏画面共通 stage演奏ドラム画面
    {
        get;
        set;
    }
    private static CStageResult stageResult
    {
        get;
        set;
    }
    private static CStageEnding stageEnding
    {
        get;
        set;
    }
    private static CStageMaintenance stageMaintenance
    {
        get;
        set;
    }
    internal static CNamePlate actNamePlate
    {
        get;
        private set;
    }
    internal static FadeManager FadeManager
    {
        get;
        private set;
    }
    public static CStage r現在のステージ = null;
    public static CStage r直前のステージ = null;
    public static string strEXEのあるフォルダ => AppContext.BaseDirectory;
    public CTimer Timer
    {
        get;
        private set;
    }
    internal DiscordRichPresence Discord
    {
        get;
        private set;
    }
    internal CSaveManager SaveManager
    {
        get;
        private set;
    }

    public bool bApplicationActive
    {
        get
        {
            return this.Focused;
        }
    }
    public bool b次のタイミングで垂直帰線同期切り替えを行う
    {
        get;
        set;
    }
    public bool b次のタイミングで全画面_ウィンドウ切り替えを行う
    {
        get;
        set;
    }
    private static Size currentClientSize       // #23510 2010.10.27 add yyagi to keep current window size
    {
        get;
        set;
    }

    internal int[] n確定された曲の難易度
    {
        get;
        private set;
    } = new int[4];
    internal C曲リストノード? r確定された曲
    {
        get;
        private set;
    }
    internal Cスコア? r確定されたスコア => r確定された曲?.arスコア;

    public static string SkinName = "Unknown";
    public static string SkinVersion = "Unknown";
    public static string SkinCreator = "Unknown";
    public static string Renderer = "Unknown";

    #endregion

    // コンストラクタ

    public TJAPlayerPI()
        : base("TJAPlayerPI", 1280, 720)
    {
        TJAPlayerPI.app = this;

        RemoveDefaultSkin();
        ExportEmbeddedFiles();
        Renderer = this.RendererName;
        #region [ Config.toml の読み込み ]
        string tomlpath = Path.Combine(strEXEのあるフォルダ, "Config.toml");
        ConfigToml = CConfigToml.Load(tomlpath);
        #endregion
        #region [ Config.ini の読込み ]
        //---------------------
        ConfigIni = new CConfigIni();
        string path = Path.Combine(strEXEのあるフォルダ, "Config.ini");
        if (File.Exists(path))
        {
            try
            {
                ConfigIni.tファイルから読み込み(path);
            }
            catch (Exception e)
            {
                //ConfigIni = new CConfigIni();	// 存在してなければ新規生成
                Trace.TraceError(e.ToString());
                Trace.TraceError("An exception has occurred, but processing continues.");
            }
        }
        //---------------------
        #endregion
        #region [ ログ出力開始 ]
        //---------------------
        Trace.AutoFlush = true;
        Trace.Listeners.Add(new CTraceLogListener(new StreamWriter(Path.Combine(strEXEのあるフォルダ, "TJAPlayer3-f.log"), false, new UTF8Encoding(false))));

        Trace.WriteLine("");
        Trace.WriteLine("DTXMania powered by YAMAHA Silent Session Drums");
        Trace.WriteLine(string.Format("Release: {0}", VERSION));
        Trace.WriteLine("");
        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ アプリケーションの初期化");
        Trace.TraceInformation("OS Version: " + Environment.OSVersion);
        Trace.TraceInformation("ProcessorCount: " + Environment.ProcessorCount.ToString());
        Trace.TraceInformation("CLR Version: " + Environment.Version.ToString());
        //---------------------
        #endregion

        #region [ FFmpegのパス設定 ]
        if (!string.IsNullOrEmpty(ConfigToml.General.FFmpegPath))
            FFmpeg.AutoGen.ffmpeg.RootPath = ConfigToml.General.FFmpegPath;
        #endregion

        #region [ ウィンドウ初期化 ]
        //---------------------
        base.Location = new Point(ConfigToml.Window.X, ConfigToml.Window.Y);   // #30675 2013.02.04 ikanick add


        base.Title = "";

        base.ClientSize = new Size(ConfigToml.Window.Width, ConfigToml.Window.Height);   // #34510 yyagi 2010.10.31 to change window size got from Config.ini

        if (ConfigToml.Window.FullScreen)                       // #23510 2010.11.02 yyagi: add; to recover window size in case bootup with fullscreen mode
        {                                                       // #30666 2013.02.02 yyagi: currentClientSize should be always made
            currentClientSize = new Size(ConfigToml.Window.Width, ConfigToml.Window.Height);
        }

        var icon_stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TJAPlayer3.TJAPlayer3-f.ico");
        if (icon_stream is not null)
            base.Icon = icon_stream;
        base.MouseWheel += this.Window_MouseWheel;
        base.Resize += this.Window_ResizeOrMove;                       // #23510 2010.11.20 yyagi: to set resized window size in Config.ini
        base.Move += this.Window_ResizeOrMove;
        //---------------------
        #endregion
        #region [ Direct3D9 デバイスの生成 ]
        //---------------------
        this.FullScreen = ConfigToml.Window.FullScreen;
        this.VSync = ConfigToml.Window.VSyncWait;
        base.ClientSize = new Size(ConfigToml.Window.Width, ConfigToml.Window.Height);   // #23510 2010.10.31 yyagi: to recover window size. width and height are able to get from Config.ini.
                                                                                         //---------------------
        #endregion

        DTX[0] = null;
        DTX[1] = null;

        #region [ Skin の初期化 ]
        //---------------------
        Trace.TraceInformation("スキンの初期化を行います。");
        Trace.Indent();
        try
        {
            Skin = new CSkin(TJAPlayerPI.app.ConfigToml.General._AbsSkinPath);
            TJAPlayerPI.app.ConfigToml.General.SkinPath = TJAPlayerPI.app.Skin.GetCurrentSkinSubfolderFullName(true);    // 旧指定のSkinフォルダが消滅していた場合に備える
            this.LogicalSize = new Size(Skin.SkinConfig.General.Width, Skin.SkinConfig.General.Height);
            Trace.TraceInformation("スキンの初期化を完了しました。");
        }
        catch
        {
            Trace.TraceInformation("スキンの初期化に失敗しました。");
            throw;
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        //-----------
        #region [ Timer の初期化 ]
        //---------------------
        Trace.TraceInformation("タイマの初期化を行います。");
        Trace.Indent();
        try
        {
            Timer = new CTimer();
            Trace.TraceInformation("タイマの初期化を完了しました。");
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        //-----------


        #region [ セーブ管理の初期化 ]
        //---------------------
        Trace.TraceInformation("セーブ管理の初期化を行います。");
        Trace.Indent();
        try
        {
            SaveManager = new CSaveManager();
            for (int nPlayer = 0; nPlayer < 2; nPlayer++)
            {
                SaveManager.Read(nPlayer);
            }
            Trace.TraceInformation("セーブ管理を生成しました。");
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        #region [ FPS カウンタの初期化 ]
        //---------------------
        Trace.TraceInformation("FPSカウンタの初期化を行います。");
        Trace.Indent();
        try
        {
            FPS = new CFPS();
            Trace.TraceInformation("FPSカウンタを生成しました。");
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        #region [ act文字コンソールの初期化 ]
        //---------------------
        Trace.TraceInformation("文字コンソールの初期化を行います。");
        Trace.Indent();
        try
        {
            act文字コンソール = new C文字コンソール();
            Trace.TraceInformation("文字コンソールを生成しました。");
            act文字コンソール.On活性化();
            Trace.TraceInformation("文字コンソールを活性化しました。");
            Trace.TraceInformation("文字コンソールの初期化を完了しました。");
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
            Trace.TraceError("文字コンソールの初期化に失敗しました。");
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        #region [ InputManager の初期化 ]
        //---------------------
        Trace.TraceInformation("InputManagerの初期化を行います。");
        Trace.Indent();
        try
        {
            InputManager = new CInputManager();
            foreach (IInputDevice device in InputManager.listInputDevices)
            {
                if ((device.eInputDeviceType == EInputDeviceType.Joystick) && !ConfigToml.JoystickGUID.ContainsValue(device.GUID))
                {
                    int key = 0;
                    while (ConfigToml.JoystickGUID.ContainsKey(key))
                    {
                        key++;
                    }
                    ConfigToml.JoystickGUID.Add(key, device.GUID);
                }
            }
            InputCTS = new CancellationTokenSource();
            Task.Factory.StartNew(() => InputLoop());
            Trace.TraceInformation("InputManagerの初期化を完了しました。");
        }
        catch
        {
            Trace.TraceError("InputManagerの初期化に失敗しました。");
            throw;
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        #region [ Pad の初期化 ]
        //---------------------
        Trace.TraceInformation("パッドの初期化を行います。");
        Trace.Indent();
        try
        {
            Pad = new CPad(ConfigIni, ConfigToml, InputManager);
            Trace.TraceInformation("パッドの初期化を完了しました。");
        }
        catch (Exception exception3)
        {
            Trace.TraceError(exception3.ToString());
            Trace.TraceError("パッドの初期化に失敗しました。");
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        #region [ SoundManager の初期化 ]
        //---------------------
        Trace.TraceInformation("サウンドデバイスの初期化を行います。");
        Trace.Indent();
        try
        {
            SoundManager = new CSoundManager(TJAPlayerPI.app.ConfigToml.SoundDevice.DeviceType,
                                        TJAPlayerPI.app.ConfigToml.SoundDevice.WASAPIBufferSizeMs,
                                        0,
                                        TJAPlayerPI.app.ConfigToml.SoundDevice.ASIODevice,
                                        TJAPlayerPI.app.ConfigToml.SoundDevice.BASSBufferSizeMs,
                                        TJAPlayerPI.app.ConfigToml.SoundDevice.UseOSTimer
            );


            Trace.TraceInformation("Initializing loudness scanning, song gain control, and sound group level control...");
            Trace.Indent();
            try
            {
                actScanningLoudness = new CActScanningLoudness();
                actScanningLoudness.On活性化();
                LoudnessMetadataScanner.ScanningStateChanged +=
                    (_, args) => actScanningLoudness.bIsActivelyScanning = args.IsActivelyScanning;
                LoudnessMetadataScanner.StartBackgroundScanning();

                SongGainController = new SongGainController();
                ConfigIniToSongGainControllerBinder.Bind(ConfigToml, SongGainController);

                SoundGroupLevelController = new SoundGroupLevelController(CSound.listインスタンス);
                ConfigIniToSoundGroupLevelControllerBinder.Bind(ConfigToml, SoundGroupLevelController);
            }
            finally
            {
                Trace.Unindent();
                Trace.TraceInformation("Initialized loudness scanning, song gain control, and sound group level control.");
            }

            ShowWindowTitleWithSoundType();
            CSoundManager.bIsTimeStretch = TJAPlayerPI.app.ConfigToml.PlayOption.TimeStretch;
            SoundManager.nMasterVolume = TJAPlayerPI.app.ConfigToml.MasterVolume;
            //FDK.CSoundManager.bIsMP3DecodeByWindowsCodec = CDTXMania.ConfigIni.bNoMP3Streaming;
            Trace.TraceInformation("サウンドデバイスの初期化を完了しました。");
        }
        catch (Exception e)
        {
            throw new NullReferenceException("サウンドデバイスがひとつも有効になっていないため、サウンドデバイスの初期化ができませんでした。", e);
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        #region [ SongsManager の初期化 ]
        //---------------------
        Trace.TraceInformation("曲リストの初期化を行います。");
        Trace.Indent();
        try
        {
            SongsManager = new CSongsManager();
            //				SongsManager_裏読 = new CSongsManager();
            EnumSongs = new CEnumSongs();
            actEnumSongs = new CActEnumSongs();
            Trace.TraceInformation("曲リストの初期化を完了しました。");
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
            Trace.TraceError("曲リストの初期化に失敗しました。");
        }
        finally
        {
            Trace.Unindent();
        }
        //---------------------
        #endregion
        #region [ ステージの初期化 ]
        //---------------------
        r現在のステージ = null;
        r直前のステージ = null;
        actNamePlate = new CNamePlate();
        FadeManager = new FadeManager();

        stageStartUp = new CStageStartUp();
        stageStartUp.Finished += OnStartupFinished;

        stageTitle = new CStageTitle();
        stageTitle.PressedGameStart += OnTitlePressedGameStart;
        stageTitle.PressedConfig += OnTitlePressedConfig;
        stageTitle.PressedExit += OnTitlePressedExit;
        stageTitle.PressedMaintenance += OnTitlePressedMaintenance;
        //			stageオプション = new CStageオプション();
        stageConfig = new CStageConfig();
        stageConfig.PressedExit += OnConfigPressedExit;

        stage選曲Legacy = new CStage選曲Legacy();
        stage選曲Legacy.GoToTitle += OnSongSelectLegacyGoToTitle;
        stage選曲Legacy.GoToConfig += OnSongSelectLegacyGoToConfig;
        stage選曲Legacy.GoToGame += OnSongSelectLegacyGoToGame;

        //stageSongLoading = new CStageSongLoading();
        stage演奏ドラム画面 = new CStage演奏画面共通();
        stage演奏ドラム画面.RestartAndReloadChart += OnGameRestartAndReloadChart;
        stage演奏ドラム画面.ExitGameAndGoToSongSelect += OnGameExitGameAndGoToSongSelect;
        stage演奏ドラム画面.ExitGameAndGoToResult += OnGameExitGameAndGoToResult;

        stageResult = new CStageResult();
        stageResult.ExitResult += OnResultExitResult;

        stageEnding = new CStageEnding();
        stageEnding.ExitGame += OnEndingExitGame;

        stageMaintenance = new CStageMaintenance();
        stageMaintenance.ExitMaintenance += OnMaintenanceExitMaintenance;

        this.listトップレベルActivities = new List<CActivity>();
        this.listトップレベルActivities.Add(actEnumSongs);
        this.listトップレベルActivities.Add(act文字コンソール);
        this.listトップレベルActivities.Add(actNamePlate);
        this.listトップレベルActivities.Add(stageStartUp);
        this.listトップレベルActivities.Add(stageTitle);
        //			this.listトップレベルActivities.Add( stageオプション );
        this.listトップレベルActivities.Add(stageConfig);
        this.listトップレベルActivities.Add(stage選曲Legacy);
        //this.listトップレベルActivities.Add(stageSongLoading);
        this.listトップレベルActivities.Add(stage演奏ドラム画面);
        this.listトップレベルActivities.Add(stageResult);
        this.listトップレベルActivities.Add(stageEnding);
        this.listトップレベルActivities.Add(stageMaintenance);

        this.listトップレベルActivities.Add(FadeManager);
        //---------------------
        #endregion
        #region Discordの処理
        this.Discord = new DiscordRichPresence("692578108997632051");
        this.Discord.Update("Startup");
        #endregion

        Trace.TraceInformation("アプリケーションの初期化を完了しました。");


        #region [ 最初のステージの起動 ]
        //---------------------
        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ 起動");

        actNamePlate.On活性化();
        FadeManager.On活性化();

        r現在のステージ = stageStartUp;

        r現在のステージ.On活性化();

        //---------------------
        #endregion
    }

    // メソッド

    public void t全画面_ウィンドウモード切り替え()
    {
        if ((ConfigToml is not null) && (ConfigToml.Window.FullScreen != this.FullScreen))
        {
            if (ConfigToml.Window.FullScreen)   // #23510 2010.10.27 yyagi: backup current window size before going fullscreen mode
            {
                currentClientSize = this.ClientSize;
                ConfigToml.Window.Width = this.ClientSize.Width;
                ConfigToml.Window.Height = this.ClientSize.Height;
            }
            this.FullScreen = ConfigToml.Window.FullScreen;
            if (!ConfigToml.Window.FullScreen)    // #23510 2010.10.27 yyagi: to resume window size from backuped value
            {
                base.ClientSize =
                    new Size(currentClientSize.Width, currentClientSize.Height);
            }
        }
    }

    // Game 実装

    protected override void OnClosing(CancelEventArgs e)
    {
        if ((EEndingAnime)ConfigToml.Ending.EndingAnime == EEndingAnime.Force && (r現在のステージ is CStageEnding))
        {
            e.Cancel = true;
            r現在のステージ.On非活性化();
            Trace.TraceInformation("----------------------");
            Trace.TraceInformation("■ 終了");
            stageEnding.On活性化();
            r直前のステージ = r現在のステージ;
            r現在のステージ = stageEnding;
            this.tガベージコレクションを実行する();
        }
        base.OnClosing(e);
    }
    protected override void OnClosed(EventArgs e)
    {
        this.t終了処理();
    }

    protected override void OnRenderFrame(EventArgs e)
    {
        Timer?.t更新();
        CSoundManager.rc演奏用タイマ?.t更新();
        InputManager?.tSwapEventList();
        FPS.tUpdateCounter();

        FadeManager.OnUpdate();

        if (this.Device is null)
            return;

        // #xxxxx 2013.4.8 yyagi; sleepの挿入位置を、EndScnene～Present間から、BeginScene前に移動。描画遅延を小さくするため。
        #region [ スリープ ]
        if (ConfigToml.Window.SleepTimePerFrame > 0)            // #xxxxx 2011.11.27 yyagi
        {
            Thread.Sleep(ConfigToml.Window.SleepTimePerFrame);
        }
        if (ConfigToml.Window.BackSleep > 0 && !this.Focused)
        {
            Thread.Sleep(ConfigToml.Window.BackSleep);
        }
        #endregion

        actNamePlate?.On進行描画();

        if (r現在のステージ is not null)
        {
            this.n進行描画の戻り値 = (r現在のステージ is not null) ? r現在のステージ.On進行描画() : 0;

            #region [ 曲検索スレッドの起動/終了 ]					// ここに"Enumerating Songs..."表示を集約
            actEnumSongs.On進行描画();                          // "Enumerating Songs..."アイコンの描画
            if (r現在のステージ is CStageTitle or CStageConfig or CStage選曲Legacy && EnumSongs is not null)
            {
                #region [ 曲検索が完了したら、実際の曲リストに反映する ]
                // CStage選曲.On活性化() に回した方がいいかな？
                if (EnumSongs.IsSongListEnumerated)
                {
                    actEnumSongs.On非活性化();
                    TJAPlayerPI.stage選曲Legacy.act曲リスト.bIsEnumeratingSongs = false;

                    bool bRemakeSongTitleBar = (r現在のステージ is CStage選曲Legacy) ? true : false;
                    TJAPlayerPI.stage選曲Legacy.Refresh(EnumSongs.SongsManager, bRemakeSongTitleBar);
                    EnumSongs.SongListEnumCompletelyDone();
                }
                #endregion
            }
            #endregion

            FadeManager.On進行描画();

            actScanningLoudness.On進行描画();

            if (r現在のステージ is not null && Tx.IsLoaded && Tx.Network_Connection is not null)
            {
                if (Math.Abs(Timer.nシステム時刻ms - this.前回のシステム時刻ms) > 10000)
                {
                    this.前回のシステム時刻ms = CSoundManager.rc演奏用タイマ.nシステム時刻ms;
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            //IPv4 8.8.8.8にPingを送信する(timeout 5000ms)
                            PingReply reply = new Ping().Send("8.8.8.8", 5000);
                            this.bネットワークに接続中 = reply.Status == IPStatus.Success;
                        }
                        catch
                        {
                            this.bネットワークに接続中 = false;
                        }
                    });
                }
                int width = TJAPlayerPI.app.Skin.SkinConfig.Overlay.NetworkConnectionSize[0];
                int height = TJAPlayerPI.app.Skin.SkinConfig.Overlay.NetworkConnectionSize[1];
                int shift = this.bネットワークに接続中 ? 2 : 0;

                TJAPlayerPI.app.Tx.Network_Connection.t2D描画(app.Device, TJAPlayerPI.app.Skin.SkinConfig.Overlay.NetworkConnectionX, TJAPlayerPI.app.Skin.SkinConfig.Overlay.NetworkConnectionY,
                    new Rectangle(width * shift, 0, width, height));
            }
            // オーバレイを描画する(テクスチャの生成されていない起動ステージは例外
            if (r現在のステージ is not null && Tx.IsLoaded)
            {
                TJAPlayerPI.app.Tx.Overlay?.t2D描画(app.Device, 0, 0);
            }
        }


        for (int i = 0; i < 0x10; i++)
        {
            if (ConfigIni.KeyAssign.Capture[i].Code > 0)
                if (InputManager.Keyboard.bIsKeyPressed((int)ConfigIni.KeyAssign.Capture[i].Code))
                {
                    // Debug.WriteLine( "capture: " + string.Format( "{0:2x}", (int) e.KeyCode ) + " " + (int) e.KeyCode );
                    string strFullPath =
                        Path.Combine(TJAPlayerPI.strEXEのあるフォルダ, "Capture_img");
                    strFullPath = Path.Combine(strFullPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ConfigToml.General.ScreenShotExt);
                    this.SaveScreen(strFullPath);
                }
            if (ConfigIni.KeyAssign.FullScreen[i].Code > 0)
                if (InputManager.Keyboard.bIsKeyPressed((int)ConfigIni.KeyAssign.FullScreen[i].Code))
                {
                    if (ConfigToml is not null)
                    {
                        ConfigToml.Window.FullScreen = !ConfigToml.Window.FullScreen;
                        this.t全画面_ウィンドウモード切り替え();
                    }
                }
        }
        if ((InputManager.Keyboard.bIsKeyDown((int)SlimDXKeys.Key.LeftAlt) || InputManager.Keyboard.bIsKeyDown((int)SlimDXKeys.Key.RightAlt)) && InputManager.Keyboard.bIsKeyPressed((int)SlimDXKeys.Key.Return))
        {
            if (ConfigToml is not null)
            {
                ConfigToml.Window.FullScreen = !ConfigToml.Window.FullScreen;
                this.t全画面_ウィンドウモード切り替え();
            }
        }

        this.Render();


        #region [ 全画面_ウインドウ切り替え ]
        if (this.b次のタイミングで全画面_ウィンドウ切り替えを行う)
        {
            ConfigToml.Window.FullScreen = !ConfigToml.Window.FullScreen;
            app.t全画面_ウィンドウモード切り替え();
            this.b次のタイミングで全画面_ウィンドウ切り替えを行う = false;
        }
        #endregion

        #region [ 垂直基線同期切り替え ]
        if (this.b次のタイミングで垂直帰線同期切り替えを行う)
        {
            currentClientSize = this.ClientSize;                                             // #23510 2010.11.3 yyagi: to backup current window size before changing VSyncWait

            this.VSync = ConfigToml.Window.VSyncWait;
            this.b次のタイミングで垂直帰線同期切り替えを行う = false;
            base.ClientSize = new Size(currentClientSize.Width, currentClientSize.Height);   // #23510 2010.11.3 yyagi: to resume window size after changing VSyncWait
        }
        #endregion
    }

    internal void tSetSongInfo(C曲リストノード song, int diffP1, int diffP2)
    {
        r確定された曲 = song;
        n確定された曲の難易度[0] = diffP1;
        n確定された曲の難易度[1] = diffP2;

        if (r確定されたスコア?.譜面情報 is ST譜面情報 info)
        {
            FadeManager.GetSongLoading().Title = info.Title;
            FadeManager.GetSongLoading().SubTitle = info.SubTitle;
        }
    }

    private void GoToTitle()
    {
        r現在のステージ?.On非活性化();
        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ Title");
        stageTitle.On活性化();
        r直前のステージ = r現在のステージ;
        r現在のステージ = stageTitle;

        this.tガベージコレクションを実行する();
        TJAPlayerPI.FadeManager.FadeIn();
    }

    private void GoToConfig()
    {
        r現在のステージ.On非活性化();
        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ Config");
        stageConfig.On活性化();
        r直前のステージ = r現在のステージ;
        r現在のステージ = stageConfig;
        TJAPlayerPI.FadeManager.FadeIn();
    }

    private void GoToSongSelectLegacy()
    {
        r現在のステージ.On非活性化();
        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ 選曲");
        stage選曲Legacy.On活性化();
        r直前のステージ = r現在のステージ;
        r現在のステージ = stage選曲Legacy;
        TJAPlayerPI.FadeManager.FadeIn();
    }

    private void GoToGame()
    {
        r現在のステージ.On非活性化();

        LoadingChart();

        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ 演奏（ドラム画面）");
        r直前のステージ = r現在のステージ;
        r現在のステージ = stage演奏ドラム画面;
        r現在のステージ.On活性化();

        this.tガベージコレクションを実行する();
        TJAPlayerPI.FadeManager.FadeIn();
    }

    private void GoToResult()
    {
        r現在のステージ.On非活性化();
        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ Result");

        stageResult.On活性化();
        r直前のステージ = r現在のステージ;
        r現在のステージ = stageResult;
        TJAPlayerPI.FadeManager.FadeIn();
    }

    private void GoToExit()
    {
        r現在のステージ.On非活性化();
        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ Ending");
        stageEnding.On活性化();
        r直前のステージ = r現在のステージ;
        r現在のステージ = stageEnding;
        TJAPlayerPI.FadeManager.FadeIn();
    }

    private void GoToMaintenance()
    {
        r現在のステージ.On非活性化();
        Trace.TraceInformation("----------------------");
        Trace.TraceInformation("■ Maintenance");
        stageMaintenance.On活性化();
        r直前のステージ = r現在のステージ;
        r現在のステージ = stageMaintenance;
        TJAPlayerPI.FadeManager.FadeIn();
    }

    private void LoadingChart()
    {
        DateTime timeBeginLoad = DateTime.Now;

        string str = TJAPlayerPI.app.r確定されたスコア?.FileInfo.FileAbsolutePath ?? "";

        CScoreJson json = CScoreJson.Load(str + ".score.json");

        if ((TJAPlayerPI.DTX[0] is not null) && TJAPlayerPI.DTX[0].b活性化してる)
            TJAPlayerPI.DTX[0].On非活性化();

        //if( CDTXMania.DTX is null )
        {
            bool bSession = TJAPlayerPI.app.ConfigToml.PlayOption.Session &&
                            TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount == 2 &&
                            TJAPlayerPI.app.n確定された曲の難易度[0] == TJAPlayerPI.app.n確定された曲の難易度[1];

            for (int i = 0; i < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; i++)
                TJAPlayerPI.DTX[i] = new CDTX(str, false, json.BGMAdjust, i, bSession);

            if (TJAPlayerPI.app.ConfigToml.OverrideScrollMode == false)
                TJAPlayerPI.app.ConfigToml.ScrollMode = TJAPlayerPI.DTX[0].eScrollMode;

            Trace.TraceInformation("----曲情報-----------------");
            Trace.TraceInformation("TITLE: {0}", TJAPlayerPI.DTX[0].TITLE);
            Trace.TraceInformation("FILE: {0}", TJAPlayerPI.DTX[0].strFilenameの絶対パス);
            Trace.TraceInformation("---------------------------");

            TimeSpan chartLoadingSpan = (TimeSpan)(DateTime.Now - timeBeginLoad);
            Trace.TraceInformation("DTX読込所要時間:           {0}", chartLoadingSpan.ToString());

            // 段位認定モード用。
            if (TJAPlayerPI.app.n確定された曲の難易度[0] == (int)Difficulty.Dan && TJAPlayerPI.DTX[0].List_DanSongs is not null)
            {
                for (int i = 0; i < TJAPlayerPI.DTX[0].List_DanSongs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(TJAPlayerPI.DTX[0].List_DanSongs[i].Title))
                    {
                        using (var pfTitle = HFontHelper.tCreateFont(32))
                        {
                            using (var bmpSongTitle = pfTitle.DrawText(TJAPlayerPI.DTX[0].List_DanSongs[i].Title, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC._TitleForeColor, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC._TitleBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                            {
                                TJAPlayerPI.DTX[0].List_DanSongs[i].TitleTex = TJAPlayerPI.app.tCreateTexture(bmpSongTitle);
                                TJAPlayerPI.DTX[0].List_DanSongs[i].TitleTex.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref TJAPlayerPI.DTX[0].List_DanSongs[i].TitleTex, 710);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitle))
                    {
                        using (var pfSubTitle = HFontHelper.tCreateFont(19))
                        {
                            using (var bmpSongSubTitle = pfSubTitle.DrawText(TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitle, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC._SubTitleForeColor, TJAPlayerPI.app.Skin.SkinConfig.Game.DanC._SubTitleBackColor, TJAPlayerPI.app.Skin.SkinConfig.Font.EdgeRatio))
                            {
                                TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitleTex = TJAPlayerPI.app.tCreateTexture(bmpSongSubTitle);
                                TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitleTex.vcScaling.X = TJAPlayerPI.GetSongNameXScaling(ref TJAPlayerPI.DTX[0].List_DanSongs[i].SubTitleTex, 710);
                            }
                        }
                    }

                }
            }
        }

        DateTime timeBeginLoadWAV = DateTime.Now;
        int nWAVcount = 1;
        int looptime = (TJAPlayerPI.app.ConfigToml.Window.VSyncWait) ? 3 : 1;   // VSyncWait=ON時は1frame(1/60s)あたり3つ読むようにする
        for (int i = 0; i < looptime && nWAVcount <= TJAPlayerPI.DTX[0].listWAV.Count; i++)
        {
            if (TJAPlayerPI.DTX[0].listWAV[nWAVcount].bUse) // #28674 2012.5.8 yyagi
            {
                TJAPlayerPI.DTX[0].tWAVの読み込み(TJAPlayerPI.DTX[0].listWAV[nWAVcount]);
            }
            nWAVcount++;
        }
        if (nWAVcount > TJAPlayerPI.DTX[0].listWAV.Count)
        {
            TimeSpan audioLoadingSpan = (TimeSpan)(DateTime.Now - timeBeginLoadWAV);
            Trace.TraceInformation("WAV読込所要時間({0,4}):     {1}", TJAPlayerPI.DTX[0].listWAV.Count, audioLoadingSpan.ToString());
            timeBeginLoadWAV = DateTime.Now;

            TJAPlayerPI.DTX[0].PlanToAddMixerChannel();

            for (int nPlayer = 0; nPlayer < TJAPlayerPI.app.ConfigToml.PlayOption.PlayerCount; nPlayer++)
            {
                TJAPlayerPI.DTX[nPlayer].t太鼓チップのランダム化(TJAPlayerPI.app.ConfigToml.PlayOption._Random[nPlayer]);
            }

            //TJAPlayerPI.stage演奏ドラム画面.On活性化();
        }

        if (TJAPlayerPI.app.ConfigToml.Game.Background.Movie)
            TJAPlayerPI.DTX[0].tAVIの読み込み();

        TimeSpan span = (TimeSpan)(DateTime.Now - timeBeginLoad);
        Trace.TraceInformation("総読込時間:                {0}", span.ToString());

        TJAPlayerPI.app.Timer.t更新();
    }

    private void OnStartupFinished(object? sender, EventArgs args)
    {
        #region [ (特定条件時) 曲検索スレッドの起動_開始 ]
        if (!EnumSongs.IsSongListEnumStarted)
        {
            actEnumSongs.On活性化();
            TJAPlayerPI.stage選曲Legacy.act曲リスト.bIsEnumeratingSongs = true;
            EnumSongs.StartEnumFromDisk();      // 曲検索スレッドの起動_開始
        }
        #endregion

        GoToTitle();
    }

    private void OnTitlePressedGameStart(object? sender, EventArgs args)
    {
        GoToSongSelectLegacy();
    }

    private void OnTitlePressedConfig(object? sender, EventArgs args)
    {
        GoToConfig();
    }

    private void OnTitlePressedExit(object? sender, EventArgs args)
    {
        GoToExit();
    }

    private void OnTitlePressedMaintenance(object? sender, EventArgs args)
    {
        GoToMaintenance();
    }

    private void OnConfigPressedExit(object? sender, EventArgs args)
    {
        if (r直前のステージ is CStageTitle)
        {
            GoToTitle();
        }
        else if (r直前のステージ is CStage選曲Legacy)
        {
            GoToSongSelectLegacy();
        }
    }

    private void OnSongSelectLegacyGoToTitle(object? sender, EventArgs args)
    {
        GoToTitle();
    }

    private void OnSongSelectLegacyGoToConfig(object? sender, EventArgs args)
    {
        GoToConfig();
    }

    private void OnSongSelectLegacyGoToGame(object? sender, EventArgs args)
    {
        LoadingChart();
        GoToGame();
    }

    private void OnGameRestartAndReloadChart(object? sender, EventArgs args)
    {
        //DTX[0].t全チップの再生停止();
        DTX[0].On非活性化();
        r現在のステージ.On非活性化();

        LoadingChart();
        r現在のステージ.On活性化();
    }

    private void OnGameExitGameAndGoToSongSelect(object? sender, EventArgs args)
    {
        this.tUpdateScoreJson();

        //DTX[0].t全チップの再生停止();
        DTX[0].On非活性化();

        CSoundManager.rc演奏用タイマ?.t再開();

        GoToSongSelectLegacy();
    }

    private void OnGameExitGameAndGoToResult(object? sender, EventArgs args)
    {
        CSoundManager.rc演奏用タイマ?.t再開();

        CScoreJson.CRecord[] cRecords = new CScoreJson.CRecord[4];
        for (int i = 0; i < ConfigToml.PlayOption.PlayerCount; i++)
            stage演奏ドラム画面.tSaveToCRecord(out cRecords[i], i);

        this.tUpdateScoreJson();

        for (int i = 0; i < ConfigToml.PlayOption.PlayerCount; i++)
            stageResult.cRecords[i] = cRecords[i];

        GoToResult();
    }

    private void OnResultExitResult(object? sender, EventArgs args)
    {
        //DTX.t全チップの再生一時停止();
        //DTX[0].t全チップの再生停止とミキサーからの削除();
        DTX[0].On非活性化();

        GoToSongSelectLegacy();
    }

    private void OnEndingExitGame(object? sender, EventArgs args)
    {
        base.Exit();
    }

    private void OnMaintenanceExitMaintenance(object? sender, EventArgs args)
    {
        GoToSongSelectLegacy();
    }

    // その他

    #region [ 汎用ヘルパー ]
    //-----------------
    public CTexture? tCreateTexture(string fileName)
    {
        try
        {
            return new CTexture(this.Device, fileName);
        }
        catch (CTextureCreateFailedException e)
        {
            Trace.TraceError(e.ToString());
            Trace.TraceError("テクスチャの生成に失敗しました。({0})", fileName);
            return null;
        }
        catch (FileNotFoundException)
        {
            Trace.TraceWarning("テクスチャファイルが見つかりませんでした。({0})", fileName);
            return null;
        }
    }
    public CTexture? tCreateTexture(SKBitmap image)
    {
        try
        {
            return new CTexture(this.Device, image);
        }
        catch (CTextureCreateFailedException e)
        {
            Trace.TraceError(e.ToString());
            Trace.TraceError("テクスチャの生成に失敗しました。(txData)");
            return null;
        }
    }

    public CTexture? ColorTexture(string htmlcolor, int width = 64, int height = 64)//2020.05.31 Mr-Ojii 単色塗りつぶしテクスチャの生成。必要かって？Tile_Black・Tile_Whiteがいらなくなるじゃん。あと、メンテモードの画像生成に便利かなって。
    {
        if (htmlcolor.Length == 7 && htmlcolor.StartsWith("#"))
            return ColorTexture(SKColor.Parse(htmlcolor.Remove(0, 1)), width, height);
        else
            return ColorTexture(SKColors.Black, width, height);
    }
    /// <summary>
    /// 単色塗りつぶしテクスチャの生成
    /// </summary>
    /// <param name="brush">ブラシの色とかの指定</param>
    /// <param name="width">幅</param>
    /// <param name="height">高さ</param>
    /// <returns></returns>
    public CTexture? ColorTexture(SKColor color, int width = 64, int height = 64)
    {
        using (var bitmap = new SKBitmap(width, height))
        {
            using(var canvas = new SKCanvas(bitmap))
            {
                canvas.DrawColor(color);
                return this.tCreateTexture(bitmap);
            }
        }
    }

    /// <summary>プロパティ、インデクサには ref は使用できないので注意。</summary>
    public static void t安全にDisposeする<T>(ref T? obj) where T : class, IDisposable //2020.06.06 Mr-Ojii twopointzero氏のソースコードをもとに改良
    {
        if (obj is null)
            return;

        obj.Dispose();
        obj = null;
    }

    public static void t安全にDisposeする<T>(ref T?[] array) where T : class, IDisposable //2020.08.01 Mr-Ojii twopointzero氏のソースコードをもとに追加
    {
        if (array is null)
        {
            return;
        }

        for (var i = 0; i < array.Length; i++)
        {
            array[i]?.Dispose();
            array[i] = null;
        }
    }

    /// <summary>
    /// そのフォルダの連番画像の最大値を返す。
    /// </summary>
    public static int t連番画像の枚数を数える(string ディレクトリ名, string プレフィックス = "", string 拡張子 = ".png")
    {
        int num = 0;
        while (File.Exists(ディレクトリ名 + プレフィックス + num + 拡張子))
        {
            num++;
        }
        return num;
    }

    /// <summary>
    /// そのフォルダの連番フォルダの最大値を返す。
    /// </summary>
    public static int t連番フォルダの個数を数える(string ディレクトリ名, string プレフィックス = "")
    {
        int num = 0;
        while (Directory.Exists(ディレクトリ名 + プレフィックス + num))
        {
            num++;
        }
        return num;
    }

    /// <summary>
    /// 曲名テクスチャの縮小倍率を返す。
    /// </summary>
    /// <param name="cTexture">曲名テクスチャ。</param>
    /// <param name="samePixel">等倍で表示するピクセル数の最大値(デフォルト値:645)</param>
    /// <returns>曲名テクスチャの縮小倍率。そのテクスチャがnullならば一倍(1f)を返す。</returns>
    public static float GetSongNameXScaling(ref CTexture cTexture, int samePixel = 660)
    {
        if (cTexture is null) return 1f;
        float scalingRate = (float)samePixel / (float)cTexture.szTextureSize.Width;
        if (cTexture.szTextureSize.Width <= samePixel)
            scalingRate = 1.0f;
        return scalingRate;
    }

    //-----------------
    #endregion

    #region [ private ]
    //-----------------
    private bool b終了処理完了済み;
    private bool bネットワークに接続中 = false;
    private long 前回のシステム時刻ms = long.MinValue;
    private static CDTX[] dtx = new CDTX[4];

    internal TextureLoader Tx = new TextureLoader();

    private List<CActivity> listトップレベルActivities;

    [Obsolete]
    private int n進行描画の戻り値;
    private CancellationTokenSource InputCTS = null;

    private void InputLoop()
    {
        while (!InputCTS.IsCancellationRequested)
        {
            InputManager?.tPolling(this.bApplicationActive);
            Thread.Sleep(1);
        }
    }

    public void ShowWindowTitleWithSoundType()
    {
        AssemblyName asmApp = Assembly.GetExecutingAssembly().GetName();
        base.Title = asmApp.Name + " Ver." + VERSION + " (" + SoundManager.GetCurrentSoundDeviceType() + ")";
    }

    private void t終了処理()
    {
        if (!this.b終了処理完了済み)
        {
            Trace.TraceInformation("----------------------");
            Trace.TraceInformation("■ アプリケーションの終了");
            #region [ 曲検索の終了処理 ]
            //---------------------
            if (actEnumSongs is not null)
            {
                Trace.TraceInformation("曲検索actの終了処理を行います。");
                Trace.Indent();
                try
                {
                    actEnumSongs.On非活性化();
                    actEnumSongs = null;
                    Trace.TraceInformation("曲検索actの終了処理を完了しました。");
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    Trace.TraceError("曲検索actの終了処理に失敗しました。");
                }
                finally
                {
                    Trace.Unindent();
                }
            }
            //---------------------
            #endregion
            #region [ 現在のステージの終了処理 ]
            //---------------------
            actNamePlate?.On非活性化();
            FadeManager?.On非活性化();

            if (TJAPlayerPI.r現在のステージ is not null && TJAPlayerPI.r現在のステージ.b活性化してる)		// #25398 2011.06.07 MODIFY FROM
            {
                Trace.TraceInformation("現在のステージを終了します。");
                Trace.Indent();
                try
                {
                    r現在のステージ.On非活性化();
                    Trace.TraceInformation("現在のステージの終了処理を完了しました。");
                }
                finally
                {
                    Trace.Unindent();
                }
            }
            //---------------------
            #endregion
            #region Discordの処理
            this.Discord.Dispose();
            #endregion
            #region [ 曲リストの終了処理 ]
            //---------------------
            if (SongsManager is not null)
            {
                Trace.TraceInformation("曲リストの終了処理を行います。");
                Trace.Indent();
                try
                {
                    SongsManager = null;
                    Trace.TraceInformation("曲リストの終了処理を完了しました。");
                }
                catch (Exception exception)
                {
                    Trace.TraceError(exception.ToString());
                    Trace.TraceError("曲リストの終了処理に失敗しました。");
                }
                finally
                {
                    Trace.Unindent();
                }
            }
            //---------------------
            #endregion
            #region TextureLoaderの処理
            Tx.DisposeTexture();
            #endregion
            #region [ スキンの終了処理 ]
            //---------------------
            if (Skin is not null)
            {
                Trace.TraceInformation("スキンの終了処理を行います。");
                Trace.Indent();
                try
                {
                    Skin.Dispose();
                    Skin = null;
                    Trace.TraceInformation("スキンの終了処理を完了しました。");
                }
                catch (Exception exception2)
                {
                    Trace.TraceError(exception2.ToString());
                    Trace.TraceError("スキンの終了処理に失敗しました。");
                }
                finally
                {
                    Trace.Unindent();
                }
            }
            //---------------------
            #endregion
            #region [ サウンドの終了処理 ]
            //---------------------
            if (SoundManager is not null)
            {
                Trace.TraceInformation("サウンド の終了処理を行います。");
                Trace.Indent();
                try
                {
                    SoundManager.Dispose();
                    SoundManager = null;
                    Trace.TraceInformation("サウンド の終了処理を完了しました。");
                }
                catch (Exception exception3)
                {
                    Trace.TraceError(exception3.ToString());
                    Trace.TraceError("サウンド の終了処理に失敗しました。");
                }
                finally
                {
                    Trace.Unindent();
                }
            }
            //---------------------
            #endregion
            #region [ パッドの終了処理 ]
            //---------------------
            if (Pad is not null)
            {
                Trace.TraceInformation("パッドの終了処理を行います。");
                Trace.Indent();
                try
                {
                    Pad = null;
                    Trace.TraceInformation("パッドの終了処理を完了しました。");
                }
                catch (Exception exception4)
                {
                    Trace.TraceError(exception4.ToString());
                    Trace.TraceError("パッドの終了処理に失敗しました。");
                }
                finally
                {
                    Trace.Unindent();
                }
            }
            //---------------------
            #endregion
            #region [ InputManagerの終了処理 ]
            //---------------------
            if (InputManager is not null)
            {
                Trace.TraceInformation("InputManagerの終了処理を行います。");
                Trace.Indent();
                try
                {
                    InputCTS.Cancel();
                    InputManager.Dispose();
                    InputManager = null;
                    Trace.TraceInformation("InputManagerの終了処理を完了しました。");
                }
                catch (Exception exception5)
                {
                    Trace.TraceError(exception5.ToString());
                    Trace.TraceError("InputManagerの終了処理に失敗しました。");
                }
                finally
                {
                    Trace.Unindent();
                }
            }
            //---------------------
            #endregion
            #region [ 文字コンソールの終了処理 ]
            //---------------------
            if (act文字コンソール is not null)
            {
                Trace.TraceInformation("文字コンソールの終了処理を行います。");
                Trace.Indent();
                try
                {
                    act文字コンソール.On非活性化();
                    act文字コンソール = null;
                    Trace.TraceInformation("文字コンソールの終了処理を完了しました。");
                }
                catch (Exception exception6)
                {
                    Trace.TraceError(exception6.ToString());
                    Trace.TraceError("文字コンソールの終了処理に失敗しました。");
                }
                finally
                {
                    Trace.Unindent();
                }
            }
            //---------------------
            #endregion
            #region [ FPSカウンタの終了処理 ]
            //---------------------
            Trace.TraceInformation("FPSカウンタの終了処理を行います。");
            Trace.Indent();
            try
            {
                if (FPS is not null)
                {
                    FPS = null;
                }
                Trace.TraceInformation("FPSカウンタの終了処理を完了しました。");
            }
            finally
            {
                Trace.Unindent();
            }
            //---------------------
            #endregion
            #region [ タイマの終了処理 ]
            //---------------------
            Trace.TraceInformation("タイマの終了処理を行います。");
            Trace.Indent();
            try
            {
                if (Timer is not null)
                {
                    Timer.Dispose();
                    Timer = null;
                    Trace.TraceInformation("タイマの終了処理を完了しました。");
                }
                else
                {
                    Trace.TraceInformation("タイマは使用されていません。");
                }
            }
            finally
            {
                Trace.Unindent();
            }
            //---------------------
            #endregion
            #region [ セーブデータの保存 ]
            for (int nPlayer = 0; nPlayer < 2; nPlayer++)
            {
                SaveManager.Save(nPlayer);
            }
            #endregion
            #region [ Config.iniの出力 ]
            //---------------------
            Trace.TraceInformation("Config.ini を出力します。");
            string str = Path.Combine(strEXEのあるフォルダ, "Config.ini");
            Trace.Indent();
            try
            {
                ConfigIni.t書き出し(str);
                Trace.TraceInformation("保存しました。({0})", str);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Trace.TraceError("Config.ini の出力に失敗しました。({0})", str);
            }
            finally
            {
                Trace.Unindent();
            }

            Trace.TraceInformation("Deinitializing loudness scanning, song gain control, and sound group level control...");
            Trace.Indent();
            try
            {
                SoundGroupLevelController = null;
                SongGainController = null;
                LoudnessMetadataScanner.StopBackgroundScanning(joinImmediately: true);
                actScanningLoudness.On非活性化();
                actScanningLoudness = null;
            }
            finally
            {
                Trace.Unindent();
                Trace.TraceInformation("Deinitialized loudness scanning, song gain control, and sound group level control.");
            }

            ConfigIni = null;

            //---------------------
            #endregion
            #region [ Config.toml の出力]
            string tomlpath = Path.Combine(strEXEのあるフォルダ, "Config.toml");
            ConfigToml.Save(tomlpath);
            #endregion
            Trace.TraceInformation("アプリケーションの終了処理を完了しました。");

            this.b終了処理完了済み = true;
        }
    }
    private void tUpdateScoreJson()
    {
        string strFilename = DTX[0].strFilenameの絶対パス + ".score.json";
        CScoreJson json = CScoreJson.Load(strFilename);
        if (!File.Exists(strFilename))
        {
            json.Title = DTX[0].TITLE;
            json.Name = DTX[0].strFilename;
        }
        json.BGMAdjust = DTX[0].nBGMAdjust;

        if (TJAPlayerPI.app.ConfigToml.PlayOption.AutoPlay[0] == false)
            json.Records[n確定された曲の難易度[0]].PlayCount++;

        json.Save(strFilename);
    }
    private void tガベージコレクションを実行する()
    {
        GC.Collect(GC.MaxGeneration);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration);
    }

    public void RefleshSkin()
    {
        Trace.TraceInformation("スキン変更:" + TJAPlayerPI.app.Skin.GetCurrentSkinSubfolderFullName(false));

        TJAPlayerPI.app.act文字コンソール.On非活性化();
        actNamePlate.On非活性化();
        FadeManager.On非活性化();

        TJAPlayerPI.app.Skin.Dispose();
        TJAPlayerPI.app.Skin = null;
        TJAPlayerPI.app.Skin = new CSkin(TJAPlayerPI.app.ConfigToml.General._AbsSkinPath);


        TJAPlayerPI.app.Tx.DisposeTexture();
        TJAPlayerPI.app.Tx.LoadTexture();

        TJAPlayerPI.app.act文字コンソール.On活性化();
        actNamePlate.On活性化();
        FadeManager.On活性化();
    }

    private void RemoveDefaultSkin()
    {
        string skinDir = Path.Combine(strEXEのあるフォルダ, "System/Default");

#if DEBUG
        if (Directory.Exists(skinDir))
        {
            Directory.Delete(skinDir, true);
        }
#endif
    }

    private void ExportEmbeddedFiles()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string[] exts = new string[] { "txt", "png", "ogg", "wav", "tja", "def", "lua", "mp3", "otf", "ttf", "so", "dll", "dylib", "md", "toml", "json", "ico" };
        foreach (string item in assembly.GetManifestResourceNames())
        {
            string fileName = item.Remove(0, "TJAPlayerPI.".Length);
            fileName = fileName.Replace('.', '/');

            foreach (var ext in exts)
            {
                fileName = fileName.Replace($"/{ext}", $".{ext}");
            }

            FileInfo fileInfo = new FileInfo(fileName);

            string oldDirPath = Path.Combine(strEXEのあるフォルダ, Path.GetDirectoryName(fileName) ?? "");
            string dirPath = "";
            foreach (var item1 in oldDirPath.Split('/', '\\'))
            {
                string line = item1;
                if (line.StartsWith("_"))
                {
                    line = line.Remove(0, 1);
                }
                dirPath += line + "/";
            }

            if (!Directory.Exists(dirPath))
            {
                Debug.Print($"create: {dirPath}");
                Directory.CreateDirectory(dirPath);
            }

            string nextFileName = dirPath + Path.GetFileName(fileName);
            if (File.Exists(nextFileName))
            {
                continue;
            }

            using Stream? stream = assembly.GetManifestResourceStream(item);
            if (stream is null)
            {
                continue;
            }
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer);

            using Stream newStream = File.OpenWrite(nextFileName);
            newStream.Write(buffer);
        }
    }

    #region [ Windowイベント処理 ]
    //-----------------
    private void Window_MouseWheel(object? sender, FDK.Windowing.MouseWheelEventArgs? e)
    {
        if (ConfigToml.SongSelect.EnableMouseWheel)
            TJAPlayerPI.stage選曲Legacy.MouseWheel(e.x - e.y);
    }

    private void Window_ResizeOrMove(object? sender, EventArgs? e)               // #23510 2010.11.20 yyagi: to get resized window size
    {
        if (!ConfigToml.Window.FullScreen)
        {
            ConfigToml.Window.X = this.Location.X;   // #30675 2013.02.04 ikanick add
            ConfigToml.Window.Y = this.Location.Y;   //
        }

        ConfigToml.Window.Width = (ConfigToml.Window.FullScreen) ? currentClientSize.Width : this.ClientSize.Width;    // #23510 2010.10.31 yyagi add
        ConfigToml.Window.Height = (ConfigToml.Window.FullScreen) ? currentClientSize.Height : this.ClientSize.Height;
    }

    #endregion
    #endregion
}
