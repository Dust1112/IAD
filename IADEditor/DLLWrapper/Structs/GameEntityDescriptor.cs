using System.Runtime.InteropServices;

namespace IADEditor.DLLWrapper.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public class GameEntityDescriptor
    {
        public TransformComponent Transform = new TransformComponent();
    }
}
