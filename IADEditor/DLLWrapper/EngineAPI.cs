using System;
using System.Linq;
using IADEditor.Components;
using IADEditor.DLLWrapper.Structs;
using System.Runtime.InteropServices;
using IADEditor.GameProject;
using IADEditor.Utilities;
using IADEditor.Utilities.Enums;

namespace IADEditor.DLLWrapper
{
    public static class EngineAPI
    {
        private const string _engineDll = "EngineDll.dll";

        [DllImport(_engineDll, CharSet = CharSet.Ansi)]
        public static extern int LoadGameCodeDll(string dllPath);
        
        [DllImport(_engineDll)]
        public static extern int UnloadGameCodeDll();

        [DllImport(_engineDll)]
        public static extern IntPtr GetScriptCreator(string name);

        [DllImport(_engineDll)]
        [return: MarshalAs(UnmanagedType.SafeArray)]
        public static extern string[] GetScriptNames();
        
        internal static class EntityAPI
        {
            [DllImport(_engineDll)]
            private static extern int CreateGameEntity(GameEntityDescriptor descriptor);

            public static int CreateGameEntity(GameEntity gameEntity) 
            {
                GameEntityDescriptor descriptor = new GameEntityDescriptor();

                // Transform component
                {
                    Transform component = gameEntity.GetComponent<Transform>();

                    descriptor.Transform.position = component.Position;
                    descriptor.Transform.rotation = component.Rotation;
                    descriptor.Transform.scale = component.Scale;
                }
                // Script component
                {
                    Script component = gameEntity.GetComponent<Script>();
                    if (component != null && Project.Current != null)
                    {
                        if (Project.Current.AvailableScripts.Contains(component.Name))
                        {
                            descriptor.Script.ScriptCreator = GetScriptCreator(component.Name);
                        }
                        else
                        {
                            Logger.Log(MessageType.Error, $"Unable to find script with name {component.Name}. Game entity will be created without script component!");
                        }
                    }
                }

                return CreateGameEntity(descriptor);
            }

            [DllImport(_engineDll)]
            private static extern void RemoveGameEntity(int id);
            public static void RemoveGameEntity(GameEntity gameEntity) 
            {
                RemoveGameEntity(gameEntity.EntityId);
            }
        }
    }
}
