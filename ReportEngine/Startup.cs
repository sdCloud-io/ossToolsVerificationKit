using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.filesystem;
using ReportEngine.FileSystem;
using ReportEngine.GitHelper;
using ReportEngine.InstrumentHelper;
using ReportEngine.Instruments.Pysd;
using ReportEngine.Instruments.Sde;
using ReportEngine.Services;
using ReportEngine.Services.Implementation;
using Serilog;

namespace ReportEngine
{
    class Startup
    {
        private static IConfiguration _configuration;

        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var reportService = serviceProvider.GetService<IReportBuilder>();
            reportService?.GenerateExecutionReport();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();


            services.AddLogging(opt => opt.AddSerilog(
                    new LoggerConfiguration().WriteTo.Console().CreateLogger()
                ))
                .AddSingleton<IGitHelper, GitHelper.GitHelper>()
                .AddTransient<IReportBuilder, ReportBuilder>()
                .AddSingleton<IFileSystemHelper, FileSystemHelper>()
                .AddTransient<IInstrumentProvider, InstrumentsProvider>()
                .AddSingleton<Instruments.IInstrumentExecutor, SdeExecutor>()
                .AddSingleton<Instruments.IInstrumentExecutor, PySdExecutor>()
                .AddTransient<SdeFileValueReader>()
                .AddTransient<PySdFileValueReader>()
                .AddSingleton(_configuration)
                .Configure<Configuration.Configuration>(_configuration);
        }
    }
}