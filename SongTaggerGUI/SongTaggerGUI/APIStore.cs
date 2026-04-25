using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SpotiFechLib
{
    public class APIStore
    {
        static private byte[] key = Encoding.UTF8.GetBytes("ThisIsASecureKeyThatIAmLEakingBe");
        public static string? apiKey;
        public static string? decrypedApiKey;
        
        public static bool checkFile()
        {
            string configDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config"
            );
            string outputFile = Path.Combine(configDir, "spotifyMode.txt");

            if (File.Exists(outputFile))
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        static public void writeFile(string keyIn)
        {
            string configDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config"
            );
            Directory.CreateDirectory(configDir);

            string outputFile = Path.Combine(configDir, "spotifyMode.txt");

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                {
                    fsOutput.Write(iv, 0, iv.Length);

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] fileBytes = Encoding.UTF8.GetBytes(keyIn);
                        cs.Write(fileBytes, 0, fileBytes.Length);
                    }
                }
            }
        }
        static public string? readFile()
        {
            string configDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config"
            );
            Directory.CreateDirectory(configDir);

            string outputFile = Path.Combine(configDir, "spotifyMode.txt");
            if (File.Exists(outputFile))
            {
                using (FileStream fs = new FileStream(outputFile, FileMode.Open))
                {
                    byte[] iv = new byte[16];
                    fs.Read(iv, 0, iv.Length);

                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;

                        using (ICryptoTransform decryptor = aes.CreateDecryptor())
                        using (CryptoStream cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            decrypedApiKey = sr.ReadToEnd();
                        }
                    }
                }
                return decrypedApiKey;
            }
            else
            {
                //Console.WriteLine("Error Reading File!");
                //Environment.Exit(1);
                return null;
            }
        }
        static public string collectApiKey()
        {
            Console.WriteLine("Input API Key:");

            string apikey = Console.ReadLine();
           
            apiKey = apikey;
            writeFile(apikey);
            
            return apikey;
        }
    }
}
