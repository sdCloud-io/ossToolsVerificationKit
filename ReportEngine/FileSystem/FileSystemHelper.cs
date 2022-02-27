using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;

namespace ReportEngine.filesystem
{
    public class FileSystemHelper : IFileSystemHelper
    {
        private readonly Configuration _configuration;

        public FileSystemHelper(IOptions<Configuration> configuration)
        {
            _configuration = configuration.Value;
        }

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

        public void CreateSymbolicLinkDirectory(string source, string destination)
        {
            var sdeInitProcess = new Process
            {
                StartInfo = new ProcessStartInfo("ln")
                {
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    Arguments = $"-s {source} {destination}",
                    UseShellExecute = true,
                }
            };
            sdeInitProcess.Start();
            sdeInitProcess.WaitForExit();
        }

        public void CopyFile(string filePathSource, string filePathDestination)
        {
            File.Copy(filePathSource, filePathDestination);
        }

        public void SetPermissionExecute(string filePath)
        {
            Process.Start("chmod", $"+x {filePath}");
        }

        public void WriteJsonInFile<T>(T t, string filePath)
        {
            var json = JsonSerializer.Serialize(t);
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                var fileStream = File.Create("testReport.json");
                fileStream.Close();
                File.WriteAllText("testReport.json", json);
            }
        }

        public List<string> GetFilePathsByExtensions(string fileExts)
        {
            var result = new List<string>();
            foreach (var path in _configuration.Models.Select(model => model.Path))
            {
                result.AddRange(Directory.GetFiles(path, fileExts, SearchOption.AllDirectories));
            }

            return result;
        }
    }
}