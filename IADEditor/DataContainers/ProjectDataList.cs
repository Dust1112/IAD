using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IADEditor.GameProject
{
    [DataContract]
    public class ProjectDataList
    {
        [DataMember] public List<ProjectData> Projects { get; set; }
    }
}
