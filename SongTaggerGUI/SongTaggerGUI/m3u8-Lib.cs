using System;
using System.ComponentModel;
using System.IO;
using SongTaggerGUI;
using tagIndex;
using dbMgmt;

namespace m3u8Manager
{
   
    public class Program
    {
        public record selectedSongType(string path,int id, bool isSelected, TimeSpan duration, string artist, string title);
        public static List<selectedSongType> idSongDatabase = new List<selectedSongType>();
        static private string tempPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\test.m3u8";
        public void reload()
        {
            foreach (var result in dbMgmt.Functions.localDbDisp)
            {
                var enter = new selectedSongType(
                    path: (convertPath(result.path)),
                    isSelected: false,
                    id: result.id,
                    duration: result.duration,
                    artist: result.Artist,
                    title: result.Title
                );

                idSongDatabase.Add(enter);
            }
        }


        static private void checkFile(string inPath)
        {
            using (StreamWriter sw = new StreamWriter(inPath))
            {
                int current = 0;
                sw.Write("#EXITMCU");
                sw.Write(Environment.NewLine);
                foreach (var result in idSongDatabase.Where(s => s.isSelected))
                {
                    current++;
                    if (current < 1)
                    {

                    }
                    sw.Write("\n");
                    sw.Write($"#EXTINF:{result.duration.TotalSeconds},{result.artist} - {result.title}");
                    sw.Write(Environment.NewLine);
                    sw.WriteLine(result.path);
                }
            }
        }
        static public void runFileMake(string path)
        {
            idSongDatabase.Clear();
            foreach (var result in dbMgmt.Functions.localDbDisp)
            {
                if (result.isSelected == true)
                {
                    var enter = new selectedSongType(
                        path: convertPath(result.path),
                        id: result.id,
                        isSelected: true,
                        duration: result.duration,
                        artist: result.Artist,
                        title: result.Title
                    );
                    idSongDatabase.Add(enter);
                }
            }

            checkFile(path);
            
        }
        static public string convertPath(string inputPath)
        {
            string pathRoot = Path.GetPathRoot(inputPath) ?? "";

            // Strip the drive root (e.g. "E:\")
            string stripped = pathRoot.Length > 0
                ? inputPath.Substring(pathRoot.Length)
                : inputPath;

            // Replace backslashes and prepend "/"
            string rockboxPath = "/" + stripped.Replace('\\', '/');
            return rockboxPath;
        }
    }
}
