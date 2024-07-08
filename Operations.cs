namespace Crypt
{
    public class Operations : Utils
    {
        //private async static void EncryptDirectoryToTxt(string src, string destFile)
        //{
        //    filesProcessed = 0;

        //    using (var fileStream = new FileStream(destFile, FileMode.Create))
        //    using (var binaryWriter = new BinaryWriter(fileStream))
        //    {

        //        byte[] salt = GenerateSalt();
        //        var key = GenerateKey(Password, salt);
        //        byte[] iv = GenerateIv();

        //        binaryWriter.Write(iv.Length);
        //        binaryWriter.Write(iv);
        //        binaryWriter.Write(salt.Length);
        //        binaryWriter.Write(salt);

        //        totalFiles = Directory.GetFiles(src, "*.*", SearchOption.AllDirectories).Length;

        //        async void ProcessDirectory(string directory)
        //        {
        //            string[] list = Directory.GetFiles(directory);

        //            foreach (var file in list)
        //            {
        //                byte[] content = File.ReadAllBytes(file);
        //                byte[] encryptedContent = Encrypt(content, key, iv);

        //                binaryWriter.Write(Path.GetRelativePath(src, file));
        //                binaryWriter.Write(encryptedContent.Length);
        //                binaryWriter.Write(encryptedContent);

        //                //byte[] content = File.ReadAllBytes(file);

        //                //binaryWriter.Write(Path.GetRelativePath(src, file));
        //                //binaryWriter.Write(content.Length);
        //                //binaryWriter.Write(content);

        //                // Atualiza e exibe o progresso
        //                filesProcessed++;
        //                DisplayProgress();

        //            }

        //            foreach (var subDirectory in Directory.GetDirectories(directory))
        //            {
        //                ProcessDirectory(subDirectory);
        //            }
        //        }

        //        ProcessDirectory(src);
        //    }
        //}

        protected async static Task EncryptDirectoryToTxt(string src, string destFile)
        {
            filesProcessed = 0;

            CheckCreatFile(destFile);

            using (var fileStream = new FileStream(destFile, FileMode.Create))
            using (var binaryWriter = new BinaryWriter(fileStream))
            {
                byte[] salt = GenerateSalt();
                var key = GenerateKey(Password, salt);
                byte[] iv = GenerateIv();

                binaryWriter.Write(iv.Length);
                binaryWriter.Write(iv);
                binaryWriter.Write(salt.Length);
                binaryWriter.Write(salt);

                totalFiles = Directory.GetFiles(src, "*.*", SearchOption.AllDirectories).Length;

                await ProcessDirectory(src, binaryWriter, src, key, iv);
            }
        }


        //private static void DecryptTxtToDirectory(string srcFile, string dest)
        //{
        //    filesProcessed = 0;

        //    totalFiles = File.ReadLines(srcFile).Count() - 1;

        //    using (var fileStream = new FileStream(srcFile, FileMode.Open))
        //    using (var binaryReader = new BinaryReader(fileStream))
        //    {
        //        int ivLength = binaryReader.ReadInt32();
        //        byte[] iv = binaryReader.ReadBytes(ivLength);
        //        int saltLength = binaryReader.ReadInt32();
        //        byte[] salt = binaryReader.ReadBytes(saltLength);

        //        var key = GenerateKey(Password, salt);

        //        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        //        {
        //            string relativePath = binaryReader.ReadString();
        //            int encryptedContentLength = binaryReader.ReadInt32();
        //            byte[] encryptedContent = binaryReader.ReadBytes(encryptedContentLength);

        //            byte[] decryptedContent = Decrypt(encryptedContent, key, iv);

        //            string filePath = Path.Combine(dest, relativePath);
        //            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        //            File.WriteAllBytes(filePath, decryptedContent);
        //            //File.WriteAllBytes(filePath, encryptedContent);

        //            filesProcessed++;
        //            DisplayProgress();
        //        }
        //    }

        //}
        protected async static Task DecryptTxtToDirectory(string srcFile, string dest)
        {
            filesProcessed = 0;
            
            CheckCreatFile(srcFile);

            using (var fileStream = new FileStream(srcFile, FileMode.Open))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                int ivLength = binaryReader.ReadInt32();
                byte[] iv = binaryReader.ReadBytes(ivLength);
                int saltLength = binaryReader.ReadInt32();
                byte[] salt = binaryReader.ReadBytes(saltLength);

                var key = GenerateKey(Password, salt);

                totalFiles = binaryReader.BaseStream.Length;
                
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    string relativePath = binaryReader.ReadString();
                    int encryptedContentLength = binaryReader.ReadInt32();
                    byte[] encryptedContent = binaryReader.ReadBytes(encryptedContentLength);

                    byte[] decryptedContent = await Decrypt(encryptedContent, key, iv);
                    byte[] decompressedContent = await Decompress(decryptedContent);

                    string filePath = Path.Combine(dest, relativePath);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    await File.WriteAllBytesAsync(filePath, decompressedContent);

                    filesProcessed = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
                    DisplayProgress();
                }
            }
        }

        public static void ConfigureApplicationPath()
        {
            string environmentVariableName = "Crypt";
            string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Crypt.exe");
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

            if (!currentPath.Contains(applicationPath))
            {
                string updatedPath = currentPath + ";" + applicationPath;

                Environment.SetEnvironmentVariable("PATH", updatedPath, EnvironmentVariableTarget.Machine);
            }

            if (!string.IsNullOrEmpty(applicationPath))
            {
                Environment.SetEnvironmentVariable(environmentVariableName, applicationPath, EnvironmentVariableTarget.User);
            }
        }
    }
}
