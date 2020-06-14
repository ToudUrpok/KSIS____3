using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace ChatClient
{
    public class FilesService
    {
        static private long MaxFileSize = 20971520;
        static private long MaxTotalFilesSize = 52428800;
        public static List<string> InvalidFilesExtensions = new List<string>
        {
            ".exe"
        };

        public bool IsValidFileExtension(string fileName, List<string> invalidFilesExtensions)
        {
            string fileExtesnion = GetFileExtension(fileName);
            return !invalidFilesExtensions.Contains(fileExtesnion);
        }

        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public bool AreValidArchivedFilesExtensions(FileStream zipToOpen, List<string> invalidFilesExtensions)
        {
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (!IsValidFileExtension(entry.Name, invalidFilesExtensions))
                        return false;
                }
            }
            return true;
        }

        static public void WriteFile(byte[] content, string name)
        {
            FileInfo fileInf = new FileInfo(name);
            using (var fileStream = fileInf.Create())
            {
                fileInf.Attributes = FileAttributes.Normal;
                fileStream.Write(content, 0, content.Length);
            }
        }

        async public Task<byte[]> ReadFile(string fileName, long totalFilesSize)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            if (totalFilesSize + fileInfo.Length > MaxTotalFilesSize)
            {
                throw new TotalFilesSizeException("File can't be loaded because the total files size should be less then"
                  + MaxTotalFilesSize.ToString());
            }

            if (fileInfo.Length > MaxFileSize)
            {
                throw new FileSizeException("File can't be loaded because the files size should be less then"
                  + MaxFileSize.ToString());
            }

            byte[] buffer = new byte[fileInfo.Length];
            using (var fileStream = fileInfo.OpenRead())
            {
                if (GetFileExtension(fileName).Equals(".zip")
                      && !AreValidArchivedFilesExtensions(fileStream, InvalidFilesExtensions))
                {
                    throw new InvalidZippedFileExtentionException("Archive contains invalid files extensions");
                }
                
                fileStream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }
    }
}
