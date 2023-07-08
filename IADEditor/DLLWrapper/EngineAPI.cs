﻿using IADEditor.Components;
using IADEditor.DLLWrapper.Structs;
using System.Runtime.InteropServices;

namespace IADEditor.DLLWrapper
{
    public static class EngineAPI
    {
        private const string _engineDll = "EngineDll.dll";

        [DllImport(_engineDll, CharSet = CharSet.Ansi)]
        public static extern int LoadGameCodeDll(string dllPath);
        
        [DllImport(_engineDll)]
        public static extern int UnloadGameCodeDll();
        
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
