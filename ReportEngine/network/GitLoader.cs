using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using ReportEngine.filesystem.interfaces;
using ReportEngine.network.interfaces;

namespace ReportEngine.network
{
    public class GitLoader : IGitLoader
    {
        private readonly ILogger<GitLoader> _logger;
        private readonly IFileSystemWorker _fileSystemWorker;

        public GitLoader(ILogger<GitLoader> logger, IFileSystemWorker fileSystemWorker)
        {
            _logger = logger;
            _fileSystemWorker = fileSystemWorker;
        }

        public void DownloadRepository(string name, string sourceUrl, string path)
        {
            _logger.LogInformation($"Checking out {name} repository from {sourceUrl}");
            using var webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.UserAgent, "sdCloud");
            var repo = sourceUrl.Replace("https://github.com/", "");
            var contentsUrl = $"https://api.github.com/repos/{repo}/zipball";
            var zipFileName = name + ".zip";
            webClient.DownloadFile(contentsUrl, zipFileName);

            _fileSystemWorker.ExtractFile(zipFileName, path);
        }
    }
}