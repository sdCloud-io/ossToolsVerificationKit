using System.IO;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text.Json;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;

namespace ReportEngine.filesystem
{
    public class FileSystemWorker : IFileSystemWorker
    {
        public T ReadFromJsonFile<T>(string filePath)
        {
            using var jsonFile = new StreamReader(filePath);
            return JsonSerializer.Deserialize<T>(jsonFile.ReadToEnd());
        }

        public void DeleteDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return;

            Directory.Delete(dirPath, true);
        }

        public void CreateDirectory(string dirPath)
        {
            Directory.CreateDirectory(dirPath);
        }

        public void ChangeDirectory(string dirPath)
        {
            Directory.SetCurrentDirectory(dirPath);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public void ExtractFile(string zipFilePath, string path)
        {
            ZipFile.ExtractToDirectory(zipFilePath, "temp");
            DeleteFile(zipFilePath);
            var dirPath = Directory.GetDirectories("temp")[0];
            Directory.Move(dirPath, path);
            Directory.Delete("temp");
        }

        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public void MoveDirectory(string source, string destination)
        {
            Directory.Move(source, destination);
        }

        public void CopyFile(string filePathSource, string filePathDestination)
        {
            File.Copy(filePathSource, filePathDestination);
        }

        public void SetPermissionExecute(string filePath)
        {
        }

        public void WriteJsonInFile<T>(T t, string filePath)
        {
            var json = JsonSerializer.Serialize(t);
            File.WriteAllText(filePath, json);
        }

        public List<string> GetFilePathsByExtensions(List<string> paths, List<string> exts)
        {
            var result = new List<string>();
            foreach (var path in paths)
            {
                foreach (var ext in exts)
                {
                    result.AddRange(Directory.GetFiles(path, ext, SearchOption.AllDirectories));
                }
            }

            return result;
        }
    }
}