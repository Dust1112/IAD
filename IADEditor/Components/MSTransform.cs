using IADEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IADEditor.Components
{
    sealed class MSTransform : MSComponent<Transform>
    {
        private float? posX;

        public float? PosX
        {
            get => posX;
            set
            {
                if (!posX.IsTheSameAs(value))
                {
                    posX = value;
                    OnPropertyChanged(nameof(PosX));
                }
            }
        }

        private float? posY;
        public float? PosY
        {
            get => posY;
            set
            {
                if (!posY.IsTheSameAs(value))
                {
                    posY = value;
                    OnPropertyChanged(nameof(PosY));
                }
            }
        }

        private float? posZ;
        public float? PosZ
        {
            get => posZ;
            set
            {
                if (!posZ.IsTheSameAs(value))
                {
                    posZ = value;
                    OnPropertyChanged(nameof(PosZ));
                }
            }
        }

        private float? rotX;
        public float? RotX
        {
            get => rotX;
            set
            {
                if (!rotX.IsTheSameAs(value))
                {
                    rotX = value;
                    OnPropertyChanged(nameof(RotX));
                }
            }
        }

        private float? rotY;
        public float? RotY
        {
            get => rotY;
            set
            {
                if (!rotY.IsTheSameAs(value))
                {
                    rotY = value;
                    OnPropertyChanged(nameof(RotY));
                }
            }
        }

        private float? rotZ;
        public float? RotZ
        {
            get => rotZ;
            set
            {
                if (!rotZ.IsTheSameAs(value))
                {
                    rotZ = value;
                    OnPropertyChanged(nameof(RotZ));
                }
            }
        }

        private float? scaleX;
        public float? ScaleX
        {
            get => scaleX;
            set
            {
                if (!scaleX.IsTheSameAs(value))
                {
                    scaleX = value;
                    OnPropertyChanged(nameof(ScaleX));
                }
            }
        }

        private float? scaleY;
        public float? ScaleY
        {
            get => scaleY;
            set
            {
                if (!scaleY.IsTheSameAs(value))
                {
                    scaleY = value;
                    OnPropertyChanged(nameof(ScaleY));
                }
            }
        }

        private float? scaleZ;

        public float? ScaleZ
        {
            get => scaleZ;
            set
            {
                if (!scaleZ.IsTheSameAs(value))
                {
                    scaleZ = value;
                    OnPropertyChanged(nameof(ScaleZ));
                }
            }
        }

        public MSTransform(MSEntity msEntity) : base(msEntity)
        {
            Refresh();
        }

        protected override bool UpdateComponents(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(PosX):
                case nameof(PosY):
                case nameof(PosZ):
                    SelectedComponents.ForEach(c => c.Position = new Vector3(posX ?? c.Position.X, posY ?? c.Position.Y, posZ ?? c.Position.Z));
                    return true;

                case nameof(RotX):
                case nameof(RotY):
                case nameof(RotZ):
                    SelectedComponents.ForEach(c => c.Rotation = new Vector3(rotX ?? c.Rotation.X, rotY ?? c.Rotation.Y, rotZ ?? c.Rotation.Z));
                    return true;

                case nameof(ScaleX):
                case nameof(ScaleY):
                case nameof(ScaleZ):
                    SelectedComponents.ForEach(c => c.Scale = new Vector3(scaleX ?? c.Scale.X, scaleY ?? c.Scale.Y, scaleZ ?? c.Scale.Z));
                    return true;
            }

            return false;
        }

        protected override bool UpdateMSComponent()
        {
            PosX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.X));
            PosY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.Y));
            PosZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.Z));

            RotX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.X));
            RotY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.Y));
            RotZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.Z));

            ScaleX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.X));
            ScaleY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.Y));
            ScaleZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.Z));

            return true;
        }
    }
}
