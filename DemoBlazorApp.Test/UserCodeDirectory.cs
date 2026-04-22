namespace DemoBlazorApp.Test;

using System;
using System.Collections.Generic;
using System.Globalization;

internal static class UserCodeDirectory
{

    private static Dictionary<string, string> Parameters = new()
    {
        { "DemoBlazerAppFilesRoot", @"..\..\..\..\artifacts\DemoBlazorApp\" } ,
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
        try
        {
            string variableContent = Parameters[variableName];
            string path = Path.GetFullPath(variableContent);
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Test", string.Format(
                        CultureInfo.InvariantCulture,
                        "The variable {0} contains value '{1}' which is resolved to directory '{2}' which does not exists",
                        variableName,
                        variableContent,
                        path));

                Directory.CreateDirectory(path);
            }
            return path;
        }
        catch (Exception ex)
        {
            Console.WriteLine("test", ex.Message);
            return string.Empty;
        }
    }
}
