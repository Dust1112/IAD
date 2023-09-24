using System;
using System.Threading.Tasks;
using IADEditor.Common;

namespace IADEditor.GameDev;

public class Settings : ViewModelBase
{
    private const string Editor = "IAD_EDITOR";
    private const string Compiler = "IAD_COMPILER";
    
    private string _editorText = Environment.GetEnvironmentVariable(Editor, EnvironmentVariableTarget.User);
    public string EditorText
    {
        get => _editorText;
        set
        {
            if (_editorText != value)
            {
                _editorText = value;
                OnPropertyChanged(nameof(EditorText));
            }
        }
    }

    private string _compilerText = Environment.GetEnvironmentVariable(Compiler, EnvironmentVariableTarget.User);
    public string CompilerText
    {
        get => _compilerText;
        set
        {
            if (_compilerText != value)
            {
                _compilerText = value;
                OnPropertyChanged(nameof(CompilerText));
            }
        }
    }

    public async Task SetEnvironmentVariables()
    {
        await Task.Run(() =>
        {
            Environment.SetEnvironmentVariable(Compiler, CompilerText, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(Editor, EditorText, EnvironmentVariableTarget.User);
        });
    }
}