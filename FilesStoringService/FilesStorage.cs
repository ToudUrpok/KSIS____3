using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace FilesStoringService
{
    public class FilesStorage
    {
        public Dictionary<int, FileInformation> FilesDictionary;
        static public int NonexistingFileID = -1;
        static private string SavingRepository = "C:/FilesStorage";

        public FilesStorage()
        {
            FilesDictionary = new Dictionary<int, FileInformation>();
        }

        string GetMD5HashFromFile(byte[] content)
        {
            byte[] hash = null;
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                hash = md5.ComputeHash(content);
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                stringBuilder.Append(hash[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        void CreateFile(string fileName, byte[] content)
        {
            FileInfo fileInf = new FileInfo(SavingRepository + "/" + fileName);
            using (var fileStream = fileInf.Create())
            {
                fileInf.Attributes = FileAttributes.Normal;
                fileStream.Write(content, 0, content.Length);
            }
        }

        public int SaveFile(string fileName, byte[] content)
        {
            int fileID = NonexistingFileID;
            if (!IsFileExists(fileName))
            {
                CreateFile(fileName, content);
                fileID = GetMD5HashFromFile(content).GetHashCode();
                FilesDictionary.Add(fileID, new FileInformation(fileName, content.Length.ToString()));
            }
            return fileID;
        }

        private bool IsFileExists(string fileName)
        {
            foreach(FileInformation fileInformation in FilesDictionary.Values)
            {
                if (fileInformation.Name == fileName)
                    return true;
            }
            return false;
        }

        public void DeleteFile(int fileID)
        {
            if (FilesDictionary.ContainsKey(fileID))
            {
                FileInfo fileInf = new FileInfo(SavingRepository + "/" + FilesDictionary[fileID].Name);
                Console.WriteLine("Delete request: " + FilesDictionary[fileID].Name);
                fileInf.Delete();
                FilesDictionary.Remove(fileID);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public byte[] ReadFile(int fileID)
        {
            if (FilesDictionary.ContainsKey(fileID))
            {
                using (var fileStream = new FileStream(SavingRepository + "/" + FilesDictionary[fileID].Name, FileMode.Open)) // 
                {
                    byte[] buffer = new byte[fileStream.Length];
                    try
                    {
                        fileStream.Read(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        buffer = null;
                    }
                    return buffer;
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
