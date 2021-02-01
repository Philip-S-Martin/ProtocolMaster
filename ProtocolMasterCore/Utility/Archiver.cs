using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace ProtocolMasterCore.Utility
{
    public static class Archiver
    {
        public static void ArchiveOldestInDirectory(string path, string archivePath, string extension, int unarchivedMax, int unarchivedMin)
        {
            // Get all files of type
            IEnumerable<FileSystemInfo> zipTargets =
                new DirectoryInfo(path)
                .GetFileSystemInfos()
                .Where(fi => fi.Name.Contains(extension));
            // Exit if bounds not met
            if (zipTargets.Count() < unarchivedMax)
                return;
            // Refine target list to oldest
            zipTargets = 
                zipTargets.OrderBy(fi => fi.CreationTime)
                .Take(zipTargets.Count() - unarchivedMin);
            // final archive name (I use date / time)
            string archiveName = $"{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}[{zipTargets.Count()}].zip";
            // zip loop
            using (MemoryStream zipMS = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(zipMS, ZipArchiveMode.Create, true))
                {
                    // loop through files to add
                    foreach (FileInfo zipTarget in zipTargets)
                    {
                        // read the file bytes
                        byte[] fileToZipBytes = File.ReadAllBytes(zipTarget.FullName);
                        // create the entry - this is the zipped filename
                        // change slashes - now it's VALID
                        ZipArchiveEntry zipFileEntry = zipArchive.CreateEntry(zipTarget.Name.Replace(path, "").Replace('\\', '/'));
                        // add the file contents
                        using Stream zipEntryStream = zipFileEntry.Open();
                        using BinaryWriter zipFileBinary = new BinaryWriter(zipEntryStream);
                        zipFileBinary.Write(fileToZipBytes);
                    }
                }
                using (FileStream finalZipFileStream = new FileStream(Path.Combine(archivePath, archiveName), FileMode.Create))
                {
                    zipMS.Seek(0, SeekOrigin.Begin);
                    zipMS.CopyTo(finalZipFileStream);
                }
                foreach (FileInfo target in zipTargets)
                {
                    File.Delete(target.FullName);
                }
            }
        }
    }
}
