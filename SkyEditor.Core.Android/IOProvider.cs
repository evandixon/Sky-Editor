using System;
using System.IO;

namespace SkyEditor.Core.Android
{
    public class IOProvider : SkyEditor.Core.IO.IOProvider
    {
        public override bool CanLoadFileInMemory(long fileSize)
        {
            //Todo: Figure out how to determine how much RAM is available.
            //In the mean time, we'll only allow in-memory file caching if it's 5 MB or less.
            return fileSize <= (5 * 1024 * 1024);
        }

        public override void CopyFile(string sourceFilename, string destinationFilename)
        {
            File.Copy(sourceFilename, destinationFilename, true);
        }

        public override void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public override void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        public override void DeleteFile(string filename)
        {
            File.Delete(filename);
        }

        public override bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public override bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public override string[] GetDirectories(string path, bool topDirectoryOnly)
        {
            if (topDirectoryOnly)
            {
                return Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            }
            else
            {
                return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            }
        }

        public override long GetFileLength(string filename)
        {
            return (new FileInfo(filename).Length);
        }

        public override string[] GetFiles(string path, string searchPattern, bool topDirectoryOnly)
        {
            if (topDirectoryOnly)
            {
                return Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
            }
            else
            {
                return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
            }
        }

        public override string GetTempDirectory()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "SkyEditor", Guid.NewGuid().ToString());
            if (!DirectoryExists(tempDir))
            {
                CreateDirectory(tempDir);
            }
            return tempDir;
        }

        public override string GetTempFilename()
        {
            return Path.GetTempFileName();
        }

        public override Stream OpenFile(string filename)
        {
            return File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        public override Stream OpenFileReadOnly(string filename)
        {
            return File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
        }

        public override Stream OpenFileWriteOnly(string filename)
        {
            return File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public override byte[] ReadAllBytes(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public override string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }

        public override void WriteAllBytes(string filename, byte[] data)
        {
            File.WriteAllBytes(filename, data);
        }

        public override void WriteAllText(string filename, string data)
        {
            File.WriteAllText(filename, data);
        }
    }
}
