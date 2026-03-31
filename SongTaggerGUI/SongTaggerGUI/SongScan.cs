using System;
using System.Linq;
using System.IO;
using TagLib;
using System.Windows;
using SongTaggerGUI;

namespace tagIndex { 
    public class songDatabaseManager
    {
        public record SongInfo(string Title, string Artist, string Album, string path, int id);
        static public int currentId = 0;
        static public List<SongInfo> SongDatabase = new List<SongInfo>();

        public static void indexFiles(string pathIn, IProgress<string>? progress = null)
        {;
            if (!Directory.Exists(pathIn))
            {
                Console.WriteLine("Error: Not a valid path. Please try again.");
                Environment.Exit(1);
            }
            string[] allowedExtentions = { ".ogg", ".mp3", ".wav", ".flac" };
            string disallowedBegining = "._";



            var foundFiles = Directory.EnumerateFiles(pathIn, "*.*", SearchOption.AllDirectories).Where(file => allowedExtentions.Contains(Path.GetExtension(file).ToLower()))
             .Where(file => !Path.GetFileName(file).StartsWith(disallowedBegining));

            foreach (var file in foundFiles)
            {
                try
                {
                    progress?.Report(Path.GetFileName(file));
                    getTag(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Detected with this file: {ex}");
                    continue;
                }

            }
            return;
        }

        private static void getTag(string path)
        {
            var tagF = TagLib.File.Create(path);


            var songEnter = new SongInfo(
                Title: tagF.Tag.Title ?? "Unknown Title",
                Artist: tagF.Tag.FirstPerformer ?? "Unknown Artist",
                Album: tagF.Tag.Album ?? "Unknown Album",
                path: path,
                id: currentId++
            );
            SongDatabase.Add(songEnter);
            return;
        }

        static void searchBy()
        {
            Console.WriteLine("Search By: (Ar for Artist, T for Title, Al for Album)");
            string? input = Console.ReadLine();

            if (input == null)
            {
                Console.WriteLine("Please Input Something");
                Environment.Exit(1);
            }
            if (input.Contains("Al") && !input.Contains("Ar"))
            {
                Console.WriteLine("What Album?");
                string? alInpt = Console.ReadLine();
                if (alInpt == null)
                {
                    Console.WriteLine("Please Input Something");
                    Environment.Exit(1);
                }
                var search = (from SongInfo in SongDatabase where SongInfo.Album.Contains(alInpt) select SongInfo).ToList();
                if (search == null)
                {
                    Console.WriteLine("No Results found");
                    return;
                }

                foreach (var result in search)
                {
                    Console.WriteLine($"Title: {result.Title} | Artist: {result.Artist} | Album: {result.Album} |");
                }

            }
            if (input.Contains("Ar") && !input.Contains("Al"))
            {
                Console.WriteLine("What Artist?");
                string? arInpt = Console.ReadLine();
                if (arInpt == null)
                {
                    Console.WriteLine("Please Input Something");
                    Environment.Exit(1);
                }
                var search = (from SongInfo in SongDatabase where SongInfo.Artist.Contains(arInpt) select SongInfo).ToList();
                if (search == null)
                {
                    Console.WriteLine("No Results found");
                    return;
                }

                foreach (var result in search)
                {
                    Console.WriteLine($"Title: {result.Title} | Artist: {result.Artist} | Album: {result.Album} |");
                }
            }
            if (input.Contains("T") && !input.Contains("A"))
            {
                Console.WriteLine("What's the title of the song?");
                string? tInpt = Console.ReadLine();
                if (tInpt == null)
                {
                    Console.WriteLine("Please Input Something");
                    Environment.Exit(1);
                }
                var search = (from SongInfo in SongDatabase where SongInfo.Title.Contains(tInpt) select SongInfo).ToList();
                if (search == null)
                {
                    Console.WriteLine("No Results found");
                    return;
                }
                foreach (var result in search)
                {
                    Console.WriteLine($"Title: {result.Title} | Artist: {result.Artist} | Album: {result.Album}");
                }
            }
        }

    }
}