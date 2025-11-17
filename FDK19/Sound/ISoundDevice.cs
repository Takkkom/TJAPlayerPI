using NAudio.Wave;

namespace FDK;

public interface ISoundDevice : IDisposable
{
    int nMasterVolume { get; set; }
    long nElapsedTimems { get; }
    long SystemTimemsWhenUpdatingElapsedTime { get; }
    CTimer tmSystemTimer { get; }
    bool bValid { get; }

    CSound tCreateSound(string strFilename, ESoundGroup soundGroup);
    CSound tCreateSound(byte[] byArrWAVファイルイメージ, ESoundGroup soundGroup);
    void tCreateSound(string strFilename, CSound sound);
    void tCreateSound(byte[] byArrWAVファイルイメージ, CSound sound);
}
