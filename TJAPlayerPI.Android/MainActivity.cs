using Org.Libsdl.App;

namespace TJAPlayerPI.Android
{
    [Activity(Label = "TJAPlayerPI", MainLauncher = true)]
    public class MainActivity : SDLActivity
    {
        protected override string[] GetLibraries() => ["SDL3", "bass", "bass_fx", "bassmix"];

        protected override void Main()
        {
            using TJAPlayerPI app = new TJAPlayerPI();
            app.Run();
        }
    }
}
