using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSoft
{
    public class CryptoSoft
    {
        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: CryptoSoft.exe <sourceDirectory> <destinationDirectory> <key>");
                return;
            }

            string sourceDirectory = args[0];
            string destinationDirectory = args[1];
            string key = args[2];

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            EncryptFiles(sourceDirectory, destinationDirectory, keyBytes);
        }

        // Parcourt les fichiers et les crypte
        public static void EncryptFiles(string sourceDirectory, string destinationDirectory, byte[] key)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                Console.WriteLine("Source directory not found.");
                return;
            }

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.ForEach(Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories), filePath =>
            {
                EncryptFile(filePath, sourceDirectory, destinationDirectory, key);
            });
            stopwatch.Stop();
            long timeElapsed = stopwatch.ElapsedMilliseconds;

        }

        // Permet de chiffrer chaque fichier selon le chiffrement XOR
        private static void EncryptFile(string sourceFilePath, string sourceDirectory, string destinationDirectory, byte[] key)
        {
            string relativePath = Path.GetRelativePath(sourceDirectory, sourceFilePath);
            string destinationFilePath = Path.Combine(destinationDirectory, relativePath + ".encrypted");

            using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        buffer[i] ^= key[i % key.Length]; // Applique le chiffrement XOR
                    }

                    destinationStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
