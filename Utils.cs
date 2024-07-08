using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Crypt
{
    public class Utils
    {
        protected static string Password = "";
        private static readonly int KeySize = 256;
        private static readonly int BlockSize = 128;
        private static readonly int Iterations = 100000;

        // Variáveis para controle de progresso
        protected static long totalFiles;
        protected static long filesProcessed;

        protected static byte[] GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }
        
        protected async static Task ProcessDirectory(
                string directory, BinaryWriter binaryWriter, string originPath, byte[] key, byte[] iv)
        {
            string[] files = Directory.GetFiles(directory);

            for (int i = 0; i < files.Length; i += 100)
            {
                var batch = files.Skip(i).Take(100);

                foreach (var file in batch)
                {
                    byte[] content = await File.ReadAllBytesAsync(file);
                    byte[] compressedContent = await Compress(content);
                    byte[] encryptedContent = await Encrypt(compressedContent, key, iv);

                    binaryWriter.Write(Path.GetRelativePath(originPath, file));
                    binaryWriter.Write(encryptedContent.Length);
                    binaryWriter.Write(encryptedContent);

                    filesProcessed++;
                    DisplayProgress();
                }
            }

            var subDirectories = Directory.GetDirectories(directory);

            foreach (var subDirectory in subDirectories)
            {
                await ProcessDirectory(subDirectory, binaryWriter, originPath, key, iv);
            }
        }

        protected static byte[] GenerateKey(string password, byte[] salt)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return deriveBytes.GetBytes(KeySize / 8);
        }

        protected static byte[] GenerateIv()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] iv = new byte[BlockSize / 8];
            rng.GetBytes(iv);
            return iv;
        }

        protected async static Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            return await PerformCryptography(data, encryptor);
        }

        protected async static Task<byte[]> PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using var ms = new MemoryStream();
            using var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);
            await cryptoStream.WriteAsync(data, 0, data.Length);
            await cryptoStream.FlushFinalBlockAsync();
            return ms.ToArray();
        }

        protected async static Task<byte[]> Decompress(byte[] data)
        {
            using var compressedStream = new MemoryStream(data);
            using var decompressedStream = new MemoryStream();
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                await gzipStream.CopyToAsync(decompressedStream);
            }
            return decompressedStream.ToArray();
        }

        protected static List<List<T>> SplitListMatrix<T>(List<T> list, int size)
        {
            int numOfSublists = (int)Math.Ceiling((double)list.Count / size);
            List<List<T>> matrix = new List<List<T>>(numOfSublists);

            for (int i = 0; i < numOfSublists; i++)
            {
                int start = i * size;
                int count = Math.Min(size, list.Count - start);
                matrix.Add(list.GetRange(start, count));
            }

            return matrix;
        }

        protected async static Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            return await PerformCryptography(data, decryptor);
        }
        protected async static Task<byte[]> Compress(byte[] data)
        {
            using var compressedStream = new MemoryStream();
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                await gzipStream.WriteAsync(data, 0, data.Length);
            }
            return compressedStream.ToArray();
        }
        protected static void DisplayProgress()
        {
            double progressPercentage = (double)filesProcessed / totalFiles * 100;

            Console.Write($"\rProgresso: {progressPercentage:F4} %");
        }

        protected static void CheckCreatFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }
    }
}
