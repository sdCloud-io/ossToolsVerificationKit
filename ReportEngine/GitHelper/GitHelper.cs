using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using ReportEngine.Configuration;

namespace ReportEngine.GitHelper
{
    public class GitHelper : IGitHelper
    {
        private readonly ILogger<GitHelper> _logger;
        private readonly GitConfiguration _configuration;
        private readonly GitHubClient _gitHubClient;

        public GitHelper(ILogger<GitHelper> logger, IOptions<Configuration.Configuration> configuration)
        {
            _logger = logger;
            _configuration = configuration.Value.GitConfiguration;

            var token = Environment.GetEnvironmentVariable("TOKEN");
            _gitHubClient = new GitHubClient(new ProductHeaderValue("sd-cloud"))
            {
                Credentials = new Credentials(token)
            };
        }

        public string GetTag(string path)
        {
            string git = "git";
            var args = "describe --tags";
            var results = "";
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = git,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = path,
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                results += $"{proc.StandardOutput.ReadLine()}";
            }

            proc.WaitForExit();

            _logger.LogInformation($"Returned tag {results}");
            return results;
        }

        public async Task SendFileAsync(string fileName, string content)
        {
            var (owner, repo, filePath, branch) = (_configuration.GitName, _configuration.RepoName,
                fileName, _configuration.BranchName);

            try
            {
                // try to get the file (and with the file the last commit sha)
                var getAllContentsTask =
                    await _gitHubClient.Repository.Content.GetAllContentsByRef(owner, repo, filePath, branch);

                // update the file
                await _gitHubClient.Repository.Content.UpdateFile(owner, repo, filePath,
                    new UpdateFileRequest($"Update file {fileName}", content, getAllContentsTask.First().Sha,
                        branch));
            }
            catch (NotFoundException)
            {
                // if file is not found, create it
                await _gitHubClient.Repository.Content.CreateFile(owner, repo, filePath,
                    new CreateFileRequest($"Create file {fileName}", content, branch));
            }
        }
    }
}