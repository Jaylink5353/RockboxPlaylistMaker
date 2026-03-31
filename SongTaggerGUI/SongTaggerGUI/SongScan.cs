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
        public static List<SongInfo> SearchResults = new List<SongInfo>();

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
        static public bool searchDispatch(bool artist, bool album, bool title, string? artistString, string? albumString, string? titleString)
        {
            if (!artist && !album && !title)
                return false;

            IEnumerable<SongInfo> query = SongDatabase;

            if (artist && !string.IsNullOrWhiteSpace(artistString))
                query = query.Where(s => s.Artist.Contains(artistString, StringComparison.OrdinalIgnoreCase));

            if (album && !string.IsNullOrWhiteSpace(albumString))
                query = query.Where(s => s.Album.Contains(albumString, StringComparison.OrdinalIgnoreCase));

            if (title && !string.IsNullOrWhiteSpace(titleString))
                query = query.Where(s => s.Title.Contains(titleString, StringComparison.OrdinalIgnoreCase));

            SearchResults = query.DistinctBy(s => s.id).ToList();
            return true;
        }
        static private void searchArtist(string? artist)
        {
            if (artist.IsWhiteSpace()| artist == null)
            {
                return;
            }
            else
            {
                var search = (from SongInfo in SongDatabase where SongInfo.Artist.Contains(artist) select SongInfo).ToList();
                foreach (var result in search)
                {
                    SearchResults.Add(result);
                }
            }
        }
        static private void searchAlbum(string album)
        {
            if (album.IsWhiteSpace() || album == null)
            {
                return;
            }
            else
            {
                var search = (from SongInfo in SongDatabase where SongInfo.Album.Contains(album) select SongInfo).ToList();
                foreach (var result in search)
                {
                    SearchResults.Add(result);
                }
            }
        }
        static private void searchTitle(string title)
        {
            if (title.IsWhiteSpace() || title == null)
            {
                return;
            }
            else
            {
                var search = (from SongInfo in SongDatabase where SongInfo.Title.Contains(title) select SongInfo).ToList();
                foreach (var result in search)
                {
                    SearchResults.Add(result);
                }
            }
        }
        
    }
}