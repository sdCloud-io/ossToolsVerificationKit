using System.Collections.Generic;
using ReportEngine.models;

namespace ReportEngine.Services
{
    public interface IScriptExecution
    {
        Report ExecuteScripts(IEnumerable<ScriptConfiguration> configurationScripts);
    }
}