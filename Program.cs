using System;
using System.Linq;
using TagLib;


class Program
{
    public record SongInfo(string Title, string Artist, string Album, string path, int id);
    static public int currentId = 0;
    static public List<SongInfo> SongDatabase = new List<SongInfo>();
    static void Main()
    {
        indexFiles();
        searchBy();
    }

    static void indexFiles()
    {
        Console.WriteLine("Please put the path to the directory you want to search:");
        string pathIn = Console.ReadLine();
        if (!Directory.Exists(pathIn))
        {
            Console.WriteLine("Error: Not a valid path. Please try again.");
            Environment.Exit(1);
        }
        string[] allowedExtentions = {".ogg", ".mp3", ".wav", ".flac"};
        string disallowedBegining = "._";


        
        var foundFiles = Directory.EnumerateFiles(pathIn, "*.*", SearchOption.AllDirectories) .Where(file => allowedExtentions.Contains(Path.GetExtension(file).ToLower()))
         .Where(file => !Path.GetFileName(file).StartsWith(disallowedBegining));

        foreach (var file in foundFiles)
        {
            Console.WriteLine($"File Found: {file}");
            try
            {
               getTag(file); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There's an error with this file. {ex} Continue? (y or n, defualt is y)");
                var erInpt = Console.ReadLine();
                if (erInpt == null || erInpt.Contains("y") && !erInpt.Contains("n"))
                {
                    continue;
                }
                if (erInpt.Contains("n") && !erInpt.Contains("y"))
                {
                    Environment.Exit(2);
                }
            }
            
        }
        return;
    }

    static void getTag(string path)
    {
        var tagF = TagLib.File.Create(path);

     
        var songEnter = new SongInfo(
            Title:tagF.Tag.Title ?? "Unknown Title",
            Artist:tagF.Tag.FirstPerformer ?? "Unknown Artist",
            Album:tagF.Tag.Album ?? "Unknown Album",
            path:path,
            id:currentId++
        );
        SongDatabase.Add(songEnter);
        return;
    }

    static void searchBy()
    {
        Console.WriteLine("Search By: (Ar for Artist, T for Title, Al for Album)");
        string input = Console.ReadLine();
        
        if (input == null)
        {
            Console.WriteLine("Please Input Something");
            Environment.Exit(1);
        }
        if (input.Contains("Al"))
        {
            Console.WriteLine("What Album?");
            string alInpt = Console.ReadLine();
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
                Console.WriteLine($"Title: {result.Title}. Artist: {result.Artist}. Album: {result.Album}.");
            }
            
        }
    }

}