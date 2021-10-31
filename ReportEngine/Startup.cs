using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.filesystem;
using ReportEngine.filesystem.interfaces;
using ReportEngine.network;
using ReportEngine.network.interfaces;
using ReportEngine.services;
using ReportEngine.services.interfaces;
using ReportEngine.strategies;
using ReportEngine.strategies.interfaces;
using Serilog;

namespace ReportEngine
{
    class Startup
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var reportService = serviceProvider.GetService<IReportService>();
            reportService.GenerateReport();
            Console.WriteLine("Ok");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(opt => opt.AddSerilog(
                    new LoggerConfiguration().WriteTo.Console().CreateLogger()
                ))
                .AddTransient<IReportService, ReportBuilder>()
                .AddTransient<IFileSystemWorker, FileSystemWorker>()
                .AddTransient<IStrategyProvider, StrategyProvider>()
                .AddTransient<IInstrumentStrategy, SdeStrategy>()
                .AddTransient<IInstrumentStrategy, PySDStrategy>()
                .AddTransient<IGitLoader, GitLoader>();
        }
    }
}