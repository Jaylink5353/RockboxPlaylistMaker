using System;
using System.IO;

using TagLib;
namespace BasicTagReader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input File Path:");
            string? path = Console.ReadLine();

            if (path is null)
            {
                Environment.Exit(1);
            }

            var tagF = TagLib.File.Create(path);

            Console.WriteLine($"Title: {tagF.Tag.Title}");
            Console.WriteLine($"Artist: {tagF.Tag.FirstPerformer}");
            Console.WriteLine($"Album: {tagF.Tag.Album}");

        }
    }
}
