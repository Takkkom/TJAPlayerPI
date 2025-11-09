using NAudio.Wave;

namespace FDK;

internal interface ISoundDevice : IDisposable
{
    ESoundDeviceType eOutputDevice { get; }
    int nMasterVolume { get; set; }
    long nElapsedTimems { get; }
    long SystemTimemsWhenUpdatingElapsedTime { get; }
    CTimer tmSystemTimer { get; }

    CSound tCreateSound(string strFilename, ESoundGroup soundGroup);
    void tCreateSound(string strFilename, CSound sound);
    void tCreateSound(byte[] byArrWAVファイルイメージ, CSound sound);
}
