namespace DemoBlazorApp.Test;

using System;
using System.Collections.Generic;
using System.Globalization;

internal static class UserCodeDirectory
{

    private static Dictionary<string, string> Parameters = new()
    {
        // build output of the DemoBlazorApp project (BaseOutputPath = ..\artifacts)
        { "DemoBlazerAppFilesRoot", @"..\..\..\..\artifacts\Debug\net10.0\" } ,
        { "ECU300SimulationFilesRoot", @"..\..\..\ECU300\plc\artifacts\Simulation\" } ,
        { "ECU300FilesRoot", @"..\..\..\ECU300\net\artifacts\Release\" } ,
        { "ECU300TempLogFilesRoot", @".\TestReports\Logs\" } ,
        { "ECU300DeployedFilesRoot", @"C:\work\ECU300\artifacts" }
    };

    public static readonly Lazy<string> FullDemoBlazorAppPath =
        new Lazy<string>(() => RetrieveDirectoryPathFromGlobalVariable("DemoBlazerAppFilesRoot"));
    public static readonly Lazy<string> FullSimulationPath =
        new Lazy<string>(() => RetrieveDirectoryPathFromGlobalVariable("ECU300SimulationFilesRoot"));
    public static readonly Lazy<string> FullECU300Path =
        new Lazy<string>(() => RetrieveDirectoryPathFromGlobalVariable("ECU300FilesRoot"));
    public static readonly Lazy<string> FullECU300TempLogFilesPath =
        new Lazy<string>(() => RetrieveDirectoryPathFromGlobalVariable("ECU300TempLogFilesRoot"));
    public static readonly Lazy<string> FullECU300DeployedFilesPath =
        new Lazy<string>(() => RetrieveDirectoryPathFromGlobalVariable("ECU300DeployedFilesRoot"));

    private static string RetrieveDirectoryPathFromGlobalVariable(string variableName)
    {
        string variableContent = Parameters[variableName];
        string path = Path.GetFullPath(variableContent);
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException(string.Format(
                CultureInfo.InvariantCulture,
                "The variable {0} contains value '{1}' which is resolved to directory '{2}' which does not exist.",
                variableName,
                variableContent,
                path));
        }
        return path;
    }
}
