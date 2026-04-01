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
            checkFile(@"C:\Users\Jaymes\Desktop\testPlaylist.m3u8");
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
    }
}
