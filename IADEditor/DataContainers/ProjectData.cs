using System;
using System.Runtime.Serialization;

namespace IADEditor.GameProject
{
    [DataContract]
    public class ProjectData
    {
        [DataMember] public string ProjectName { get; set; }
        [DataMember] public string ProjectPath { get; set; }
        [DataMember] public DateTime Date { get; set; }

        public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}"; }
        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
    }
}
