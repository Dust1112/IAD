using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using IADEditor.GameProject;
using IADEditor.Utilities;
using IADEditor.Utilities.Enums;

namespace IADEditor.GameDev
{
    public static class VisualStudio
    {
        private static EnvDTE80.DTE2 _vsInstance = null;
        private static readonly string _programId = "VisualStudio.DTE.17.0";
        public static bool BuildSucceeded { get; private set; } = true;
        public static bool BuildDone { get; private set; } = true;

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        public static void OpenVisualStudio(string solutionPath)
        {
            IRunningObjectTable rot = null;
            IEnumMoniker monikerTable = null;
            IBindCtx bindCtx = null;
            try
            {
                if (_vsInstance == null)
                {
                    // Find and open Visual Studio
                    int hResult = GetRunningObjectTable(0, out rot);
                    if (hResult < 0 || rot == null)
                    {
                        throw new COMException($"GetRunningObjectTable returned HRESULT: {hResult:x8}");
                    }

                    rot.EnumRunning(out monikerTable);
                    monikerTable.Reset();

                    hResult = CreateBindCtx(0, out bindCtx);
                    if (hResult < 0 || bindCtx == null)
                    {
                        throw new COMException($"CreateBindCtx returned HRESULT: {hResult:x8}");
                    }

                    IMoniker[] currentMoniker = new IMoniker[1];
                    while (monikerTable.Next(1, currentMoniker, IntPtr.Zero) == 0)
                    {
                        string name = string.Empty;
                        currentMoniker[0].GetDisplayName(bindCtx, null, out name);

                        if (name.Contains(_programId))
                        {
                            hResult = rot.GetObject(currentMoniker[0], out object obj);
                            if (hResult < 0 || obj == null)
                            {
                                throw new COMException(
                                    $"Running object table's GetObject returned HRESULT: {hResult:x8}");
                            }

                            EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2;
                            string solutionName = dte.Solution.FullName;

                            if (solutionName == solutionPath)
                            {
                                _vsInstance = dte;
                                break;
                            }
                        }
                    }

                    // Visual Studio is not running
                    // Open manually
                    if (_vsInstance == null)
                    {
                        Type visualStudioType = Type.GetTypeFromProgID(_programId, true);
                        _vsInstance = Activator.CreateInstance(visualStudioType) as EnvDTE80.DTE2;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.Log(MessageType.Error, "Failed to open Visual Studio");
            }
            finally
            {
                if (monikerTable != null)
                {
                    Marshal.ReleaseComObject(monikerTable);
                }

                if (rot != null)
                {
                    Marshal.ReleaseComObject(rot);
                }

                if (bindCtx != null)
                {
                    Marshal.ReleaseComObject(bindCtx);
                }
            }
        }

        public static void CloseVisualStudio()
        {
            if (_vsInstance?.Solution.IsOpen == true)
            {
                _vsInstance.ExecuteCommand("File.SaveAll");
                _vsInstance.Solution.Close(true);
            }

            _vsInstance?.Quit();
        }

        public static bool AddFilesToSolution(string solution, string projectName, string[] files)
        {
            Debug.Assert(files?.Length > 0);
            OpenVisualStudio(solution);

            try
            {
                if (_vsInstance != null)
                {
                    if (!_vsInstance.Solution.IsOpen)
                    {
                        _vsInstance.Solution.Open(solution);
                    }
                    else
                    {
                        _vsInstance.ExecuteCommand("File.SaveAll");
                    }

                    foreach (EnvDTE.Project project in _vsInstance.Solution.Projects)
                    {
                        if (project.UniqueName.Contains(projectName))
                        {
                            foreach (string file in files)
                            {
                                project.ProjectItems.AddFromFile(file);
                            }
                        }
                    }

                    string? cpp = files.FirstOrDefault(x => Path.GetExtension(x) == ".cpp");
                    if (!string.IsNullOrEmpty(cpp))
                    {
                        _vsInstance.ItemOperations.OpenFile(cpp, "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}").Visible =
                            true;
                    }

                    _vsInstance.MainWindow.Activate();
                    _vsInstance.MainWindow.Visible = true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to add files to Visual Studio project");
                return false;
            }

            return true;
        }

        public static bool IsDebugging()
        {
            bool result = false;
            bool tryAgain = true;

            for (int i = 0; i < 3 && tryAgain; i++)
            {
                try
                {
                    result = _vsInstance != null &&
                             (_vsInstance.Debugger.CurrentProgram != null ||
                              _vsInstance.Debugger.CurrentMode == EnvDTE.dbgDebugMode.dbgRunMode);
                    tryAgain = false;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    System.Threading.Thread.Sleep(1000);
                }
            }

            return result;
        }

        public static void BuildSolution(Project project, string buildConfig, bool showWindow = true)
        {
            if (IsDebugging())
            {
                Logger.Log(MessageType.Error, "Visual Studio is currently running a process");
                return;
            }

            OpenVisualStudio(project.Solution);
            BuildDone = BuildSucceeded = false;
            
            // TODO This had a for loop for retrying, but was removed as it was deemed unnecessary.
            // Put the for loop back in the future if needed: ( for(int i = 0; i < 3 && !BuildDone; i++) )
            try
            {
                if (!_vsInstance.Solution.IsOpen)
                {
                    _vsInstance.Solution.Open(project.Solution);
                }

                _vsInstance.MainWindow.Visible = showWindow;
                
                _vsInstance.Events.BuildEvents.OnBuildProjConfigBegin += OnBuildSolutionBegin;
                _vsInstance.Events.BuildEvents.OnBuildProjConfigDone += OnBuildSolutionDone;

                try
                {
                    foreach (string pdbFile in Directory.GetFiles(Path.Combine($"{project.Path}", $@"x64\{buildConfig}"), "*.pdb"))
                    {
                        File.Delete(pdbFile);
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                _vsInstance.Solution.SolutionBuild.SolutionConfigurations.Item(buildConfig).Activate();
                _vsInstance.ExecuteCommand("Build.BuildSolution");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine($"Attempt {1}: failed to build {project.Name}");
            }
        }

        private static void OnBuildSolutionDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (BuildDone) { return; }

            if (success)
            {
                Logger.Log(MessageType.Info, $"Building {projectConfig} configuration succeeded");
            }
            else
            {
                Logger.Log(MessageType.Error, $"Building {projectConfig} configuration failed");
            }

            BuildDone = true;
            BuildSucceeded = success;
        }

        private static void OnBuildSolutionBegin(string project, string projectConfig, string platform, string solutionConfig)
        {
            Logger.Log(MessageType.Info, $"Building {project}, {projectConfig}, {platform}, {solutionConfig}");
        }

        public static void Run(Project project, string configName, bool debug)
        {
            if (_vsInstance != null && !IsDebugging() && BuildDone && BuildSucceeded)
            {
                _vsInstance.ExecuteCommand(debug ? "Debug.Start" : "Debug.StartWithoutDebugging");
            }
        }

        public static void Stop()
        {
            if (_vsInstance != null && IsDebugging())
            {
                _vsInstance.ExecuteCommand("Debug.StopDebugging");
            }
        }
    }
}