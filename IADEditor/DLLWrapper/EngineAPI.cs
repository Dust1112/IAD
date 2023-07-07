using IADEditor.Components;
using IADEditor.DLLWrapper.Structs;
using System.Runtime.InteropServices;

namespace IADEditor.DLLWrapper
{
    public static class EngineAPI
    {
        private const string _dllName = "EngineDll.dll";

        [DllImport(_dllName)]
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

        [DllImport(_dllName)]
        private static extern void RemoveGameEntity(int id);
        public static void RemoveGameEntity(GameEntity gameEntity) 
        {
            RemoveGameEntity(gameEntity.EntityId);
        }
    }
}
