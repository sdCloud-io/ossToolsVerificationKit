using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.filesystem;
using ReportEngine.filesystem.adapters;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;
using ReportEngine.services;
using ReportEngine.services.interfaces;
using ReportEngine.strategies;
using ReportEngine.strategies.interfaces;
using Serilog;

namespace ReportEngine
{
    class Startup
    {
        public static IConfiguration configuration;

        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var reportService = serviceProvider.GetService<IReportService>();
            reportService?.GenerateReport();
            Console.WriteLine("Ok");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();


            services.AddLogging(opt => opt.AddSerilog(
                    new LoggerConfiguration().WriteTo.Console().CreateLogger()
                ))
                .AddTransient<IReportService, ReportBuilder>()
                .AddTransient<IFileSystemHelper, FileSystemHelper>()
                .AddTransient<IStrategyProvider, StrategyProvider>()
                .AddTransient<IInstrumentStrategy, SdeStrategy>()
                .AddTransient<IInstrumentStrategy, PySDStrategy>()
                .AddSingleton<SdeFileAdapter>()
                .AddSingleton<PySDFileAdapter>()
                .AddSingleton<PySDStrategy>()
                .AddSingleton(configuration)
                .Configure<Configuration>(configuration);
        }
    }
}