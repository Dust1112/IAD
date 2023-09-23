using IADEditor.GameDev.Enums;
using IADEditor.GameProject;

namespace IADEditor.GameDev;

public static class EditorWrapper
{
    public static bool BuildSucceeded { get; private set; } = true;
    public static bool BuildDone { get; private set; } = true;
    
    public static void BuildSolution(Project project, string buildConfig, bool showWindow = true,
        Editor editor = Editor.VisualStudio)
    {
        switch (editor)
        {
            case Editor.VisualStudio:
                VisualStudio.BuildSolution(project, buildConfig, showWindow);
                BuildSucceeded = VisualStudio.BuildSucceeded;
                BuildDone = VisualStudio.BuildDone;
                break;
            case Editor.VsCode:
                VsCode.BuildSolution(project, buildConfig, showWindow);
                BuildSucceeded = VsCode.BuildSucceeded;
                BuildDone = VsCode.BuildDone;
                break;
        }
    }

    public static void Run(Project project, string configName, bool debug, Editor editor = Editor.VisualStudio)
    {
        switch (editor)
        {
            case Editor.VisualStudio:
                VisualStudio.Run(project, configName, debug);
                break;
            case Editor.VsCode:
                VsCode.Run(project, configName, debug);
                break;
        }
    }

    public static void Stop(Editor editor = Editor.VisualStudio)
    {
        switch (editor)
        {
            case Editor.VisualStudio:
                VisualStudio.Stop();
                break;
            case Editor.VsCode:
                VsCode.Stop();
                break;
        }
    }
}