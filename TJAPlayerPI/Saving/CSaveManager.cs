using FDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Saving
{
    internal class CSaveManager
    {
        public static readonly string SavesPath = Path.Combine(TJAPlayerPI.strEXEのあるフォルダ, "Saves");

        public static string GetPath(string fileName) => Path.Combine(SavesPath, fileName);


        public CSaveData[] SaveDatas { get; init; } = new CSaveData[2];

        public string GetSaveFilePath(int nPlayer) => GetPath($"{TJAPlayerPI.app.ConfigToml.General.SaveFile[nPlayer]}.json");

        public CSaveManager()
        {
            if (!Directory.Exists(SavesPath))
            {
                Directory.CreateDirectory(SavesPath);
            }

            for (int nPlayer = 0; nPlayer < 2; nPlayer++)
            {
                SaveDatas[nPlayer] = new CSaveData()
                {
                    Name = $"{nPlayer + 1}P"
                };
            }
        }

        public void Read(string fileName, int nPlayer)
        {
            TJAPlayerPI.app.ConfigToml.General.SaveFile[nPlayer] = fileName;

            string path = GetSaveFilePath(nPlayer);
            if (!File.Exists(path))
            {
                Save(nPlayer);
                return;
                //throw new FileNotFoundException();
            }

            SaveDatas[nPlayer] = JsonSerializer.Deserialize<CSaveData>(CJudgeTextEncoding.ReadTextFile(path) ?? "") ?? SaveDatas[nPlayer];
        }
        public void Read(int nPlayer) => Read(TJAPlayerPI.app.ConfigToml.General.SaveFile[nPlayer], nPlayer);

        public void Save(string fileName, int nPlayer)
        {
            TJAPlayerPI.app.ConfigToml.General.SaveFile[nPlayer] = fileName;

            string path = GetSaveFilePath(nPlayer);
            string text = JsonSerializer.Serialize(SaveDatas[nPlayer]);

            using StreamWriter streamWriter = new StreamWriter(path, false);
            streamWriter.Write(text);
        }
        public void Save(int nPlayer) => Save(TJAPlayerPI.app.ConfigToml.General.SaveFile[nPlayer], nPlayer);
    }
}
