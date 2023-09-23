using System.Diagnostics;
using IADEditor.GameProject;
using IADEditor.Utilities;
using IADEditor.Utilities.Enums;

namespace IADEditor.GameDev;

public class VsCode
{
    public static bool BuildSucceeded { get; private set; } = true;
    public static bool BuildDone { get; private set; } = true;
    public static Process VscodeInstance { get; private set; } 
    
    private static string MsBuildPath { get; set; } =
        "\"C:\\Program Files\\Microsoft Visual Studio\\2022\\Professional\\MSBuild\\Current\\Bin\\MSBuild.exe\"";
    private static string VsCodePath { get; set; } = "\"C:\\Program Files\\Microsoft VS Code\\Code.exe\"";
    
    public static void BuildSolution(Project project, string buildConfig, bool showWindow = true)
    {
        if (VscodeInstance == null && showWindow)
        {
            VscodeInstance = Process.Start(VsCodePath, $"\"{project.CodePath}\"");
        }
            
        var startInfo = new ProcessStartInfo
        {
            FileName = MsBuildPath,
            Arguments = $"\"{project.Solution}\" /t:Build /p:Configuration={buildConfig}",  // Additional arguments as required
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        // Capture output (if required)
        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Logger.Log(MessageType.Info, $"Build succeeded: {output}");
        }
        else
        {
            Logger.Log(MessageType.Error, $"Build failed: {output}");
        }

        BuildDone = true;
        BuildSucceeded = process.ExitCode == 0;
    }

    public static void Run(Project project, string configName, bool debug)
    {
        
    }

    public static void Stop()
    {
        if (VscodeInstance != null && !VscodeInstance.HasExited)
        {
            VscodeInstance.Kill();
            VscodeInstance.WaitForExit();  // Optionally wait for the process to finish exiting
        }
    }
}