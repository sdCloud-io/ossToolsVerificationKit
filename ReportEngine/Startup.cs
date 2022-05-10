using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.filesystem;
using ReportEngine.filesystem.adapters;
using ReportEngine.filesystem.interfaces;
using ReportEngine.Instruments;
using ReportEngine.Instruments.Interfaces;
using ReportEngine.models;
using ReportEngine.Services;
using ReportEngine.Services.Implementation;
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
            var reportService = serviceProvider.GetService<IReportBuilder>();
            var report = reportService?.GenerateExecutionReport();
            reportService?.GenerateComparisonReport(report);

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
                .AddTransient<IReportBuilder, ReportBuilder>()
                .AddTransient<IScriptComparator, ScriptComparator>()
                .AddTransient<IScriptExecution, ScriptExecution>()
                .AddTransient<IFileSystemHelper, FileSystemHelper>()
                .AddTransient<IInstrumentProvider, InstrumentsProvider>()
                .AddTransient<IInstrumentExecutor, SdeExecutor>()
                .AddTransient<IInstrumentExecutor, PySdExecutor>()
                .AddSingleton<SdeFileAdapter>()
                .AddSingleton<PySDFileAdapter>()
                .AddSingleton<PySdExecutor>()
                .AddSingleton(configuration)
                .Configure<Configuration>(configuration);
        }
    }
}