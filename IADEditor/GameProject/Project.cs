using System;
using IADEditor.Common;
using IADEditor.Utilities;
using IADEditor.Utilities.Enums;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IADEditor.Components;
using IADEditor.DLLWrapper;
using IADEditor.GameDev;

namespace IADEditor.GameProject
{
    public enum BuildConfiguration
    {
        DebugEditor,
        ReleaseEditor,
        Release,
        Debug,
    }
    
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension => ".iad";
        public string Solution => $@"{Path}{Name}.sln";
        [DataMember] public string Name { get; private set; } = "New Project";
        [DataMember] public string Path { get; private set; }

        public string FullPath => $@"{Path}{Name}{Extension}";
        public string ContentPath => $@"{Path}Content\";

        [DataMember(Name = "Scenes")]
        private readonly ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        private Scene _activeScene;
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene != value) 
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }
        
        private static readonly string[] _buildConfigurationNames = new[] { "DebugEditor", "ReleaseEditor" };

        private int _buildConfig;
        public int BuildConfig
        {
            get => _buildConfig;
            set
            {
                if (_buildConfig != value)
                {
                    _buildConfig = value;
                    OnPropertyChanged(nameof(BuildConfig));
                }
            }
        }

        private string[] _availableScripts;

        public string[] AvailableScripts
        {
            get => _availableScripts;
            set
            {
                if (_availableScripts != value)
                {
                    _availableScripts = value;
                    OnPropertyChanged(nameof(AvailableScripts));
                }
            }
        }

        public BuildConfiguration StandAloneBuildConfiguration =>
            BuildConfig == 0 ? BuildConfiguration.Debug : BuildConfiguration.Release;
        
        public BuildConfiguration DllBuildConfiguration =>
            BuildConfig == 0 ? BuildConfiguration.DebugEditor : BuildConfiguration.ReleaseEditor;

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CompileCommand { get; private set; }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            Debug.Assert(File.Exists((Path + Name + Extension).ToLower()));
            OnDeserialized(new StreamingContext());
        }

        public static Project Load(string file) 
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullPath);
            Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
        }

        public void Unload()
        {
            VisualStudio.CloseVisualStudio();
            UndoRedo.Reset();
        }

        private void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveScene(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }

        [OnDeserialized]
        private async void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }

            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive)!;
            Debug.Assert(ActiveScene != null);
            
            await BuildGameCodeDll(false);

            SetCommands();
        }

        private async Task BuildGameCodeDll(bool showWindow = true)
        {
            try
            {
                UnloadGameCodeDll();
                await Task.Run(() =>
                    VisualStudio.BuildSolution(this, GetConfigurationName(DllBuildConfiguration), showWindow));
                await Task.Delay(1000);
                
                if (VisualStudio.BuildSucceeded)
                {
                    LoadGameCodeDll();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        private void LoadGameCodeDll()
        {
            string configName = GetConfigurationName(DllBuildConfiguration);
            string dll = $@"{Path}x64\{configName}\{Name}.dll";

            AvailableScripts = null;
            if (File.Exists(dll) && EngineAPI.LoadGameCodeDll(dll) != 0)
            {
                AvailableScripts = EngineAPI.GetScriptNames();
                ActiveScene.GameEntities.Where(x => x.GetComponent<Script>() != null)
                    .ToList()
                    .ForEach(x => x.IsActive = true);
                Logger.Log(MessageType.Info, "Game code DLL loaded successfully.");
            }
            else
            {
                Logger.Log(MessageType.Warning, "Failed to load game code DLL file. Try to build the project first.");
            }
        }

        private void UnloadGameCodeDll()
        {
            ActiveScene.GameEntities.Where(x => x.GetComponent<Script>() != null)
                .ToList()
                .ForEach(x => x.IsActive = false);
            if (EngineAPI.UnloadGameCodeDll() != 0)
            {
                AvailableScripts = null;
                Logger.Log(MessageType.Info, "Game code DLL unloaded.");
            }
        }

        private static string GetConfigurationName(BuildConfiguration configuration) =>
            _buildConfigurationNames[(int)configuration];

        private void SetCommands()
        {
            AddSceneCommand = new RelayCommand<object>(x =>
            {
                AddScene($"New Scene {_scenes?.Count}");
                
                Scene newScene = _scenes!.Last();
                int sceneIndex = _scenes!.Count - 1;

                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"));
            });

            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                int sceneIndex = _scenes!.IndexOf(x);
                RemoveScene(x);

                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveScene(x),
                    $"Remove {x.Name}"));
            }, x => !x.IsActive);

            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo(), x => UndoRedo.UndoList.Any());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo(), x => UndoRedo.RedoList.Any());
            SaveCommand = new RelayCommand<object>(x => Save(this));
            CompileCommand = new RelayCommand<bool>( async x => await BuildGameCodeDll(x),
                x => !VisualStudio.IsDebugging() && VisualStudio.BuildDone);
            
            OnPropertyChanged(nameof(AddSceneCommand));
            OnPropertyChanged(nameof(RemoveSceneCommand));
            OnPropertyChanged(nameof(UndoCommand));
            OnPropertyChanged(nameof(RedoCommand));
            OnPropertyChanged(nameof(SaveCommand));
            OnPropertyChanged(nameof(CompileCommand));
        }
    }
}
