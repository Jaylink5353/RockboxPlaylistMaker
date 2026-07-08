using System;
using System.IO;


namespace m3u8Manager
{
    internal class Program
    {
        public static List<string> SongDatabase = new List<string>();
        static void Main(string[] args)
        {
            SongDatabase.Add("/Music/Music/Music/song!");
            SongDatabase.Add("musicPath2?");
            convertPath(@"C:\Song");
            // checkFile(@"C:\Users\Jaymes\Desktop\testPlaylist.m3u8");
        }

        static public void checkFile(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write("#EXTMCU");

                foreach (var result in SongDatabase)
                {
                    sw.Write("\n");

                    sw.WriteLine(result);
                }
            }
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

            Console.WriteLine(rockboxPath);
            return rockboxPath;
        }
    }
}
