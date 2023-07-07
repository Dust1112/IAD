using System.Numerics;
using System.Runtime.InteropServices;

namespace IADEditor.DLLWrapper.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public class TransformComponent
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = new Vector3(1, 1, 1);
    }
}
