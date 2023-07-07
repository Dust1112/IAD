using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IADEditor.Utilities;
using IADEditor.Utilities.Enums;
using Project = IADEditor.GameProject.Project;
using Window = System.Windows.Window;

namespace IADEditor.GameDev;

public partial class NewScriptDialog : Window
{
    private static readonly string _namespace = GetNamespaceFromProjectName();

    public NewScriptDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
        ScriptPath.Text = @"Game Code\";
    }

    private void OnScriptName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!Validate()) return;
        string name = ScriptName.Text.Trim();
        Project project = Project.Current;
        MessageTextBlock.Text = $"{name}.h and {name}.cpp will be added to {project.Name}";
    }

    private void OnScriptPath_TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        Validate();
    }

    private async void OnCreate_Button_Click(object sender, RoutedEventArgs e)
    {
        if (!Validate()) return;
        IsEnabled = false;

        try
        {
            string name = ScriptName.Text;
            string path = Path.GetFullPath(Path.Combine(Project.Current.Path, ScriptPath.Text.Trim()));
            string solution = Project.Current.Solution;
            string projectName = Project.Current.Name;
            await Task.Run(() => CreateScript(name, path, solution, projectName));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            Logger.Log(MessageType.Error, $"Failed to create {ScriptName.Text}");
        }
        
        Close();
    }

    private void CreateScript(string name, string path, string solution, string projectName)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string cpp = Path.GetFullPath(Path.Combine(path, $"{name}.cpp"));
        string h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

        using (var sw = File.CreateText(cpp))
        {
            sw.Write(string.Format(Global.cppCode, name, _namespace));
        }
        
        using (var sw = File.CreateText(h))
        {
            sw.Write(string.Format(Global.hCode, name, _namespace));
        }

        string[] files = new string[] { cpp, h };

        for (int i = 0; i < 3; i++)
        {
            if (!VisualStudio.AddFilesToSolution(solution, projectName, files))
            {
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                break;
            }
        }
    }

    private bool Validate()
    {
        bool isValid = false;
        string name = ScriptName.Text.Trim();
        string path = ScriptPath.Text.Trim();

        string errorMsg = string.Empty;
        if (string.IsNullOrEmpty(name))
        {
            errorMsg = "Type in a script name.";
        }
        else if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || name.Any(x => char.IsWhiteSpace(x)))
        {
            errorMsg = "Invalid character(s) used in script name.";
        }
        else if (string.IsNullOrEmpty(path))
        {
            errorMsg = "Select a valid script folder.";
        }
        else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        {
            errorMsg = "Invalid character(s) used in script path";
        }
        else if (!Path.GetFullPath(Path.Combine(Project.Current.Path, path)).Contains(Path.Combine(Project.Current.Path, @"Game Code\")))
        {
            errorMsg = "Script must be added to (a sub-folder of) Game Code";
        }
        else if (File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.cpp"))) ||
                 File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.h"))))
        {
            errorMsg = $"Script {name} already exists in this folder";
        }
        else
        {
            isValid = true;
        }

        if (!isValid)
        {
            MessageTextBlock.Foreground = FindResource("Editor.RedBrush") as Brush;
        }
        else
        {
            MessageTextBlock.Foreground = FindResource("Editor.FontBrush") as Brush;
        }

        MessageTextBlock.Text = errorMsg;

        return isValid;
    }
    
    private static string GetNamespaceFromProjectName()
    {
        string projectName = Project.Current.Name;
        projectName = projectName.Replace(' ', '_');
        return projectName;
    }
}