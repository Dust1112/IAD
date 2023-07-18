using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using IADEditor.Components.Interfaces;

namespace IADEditor.Components
{
    [DataContract]
    public class Script : Component
    {
        private string _name;
        [DataMember] 
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public Script(GameEntity owner) : base(owner)
        {
        }

        public override IMSComponent GetMultiselectionComponent(MSEntity msEntity) => new MSScript(msEntity);
        
        public override void WriteToBinary(BinaryWriter bw)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(Name);
            bw.Write(nameBytes.Length);
            bw.Write(nameBytes);
        }
    }

    sealed class MSScript : MSComponent<Script>
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        
        public MSScript(MSEntity msEntity) : base(msEntity)
        {
            Refresh();
        }

        protected override bool UpdateComponents(string propertyName)
        {
            if (propertyName == nameof(Name))
            {
                SelectedComponents.ForEach(c => c.Name = _name);
                return true;
            }

            return false;
        }

        protected override bool UpdateMSComponent()
        {
            Name = MSEntity.GetMixedValue(SelectedComponents, new Func<Script, string>(x => x.Name));
            return true;
        }
    }
}