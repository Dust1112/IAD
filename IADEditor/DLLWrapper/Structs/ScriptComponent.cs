using System;
using System.Runtime.InteropServices;

namespace IADEditor.DLLWrapper.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public class ScriptComponent
    {
        public IntPtr ScriptCreator;
    }   
}