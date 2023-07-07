using IADEditor.Common;
using IADEditor.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IADEditor.Components
{
    abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
    {
        public List<T> SelectedComponents { get; set; }

        private bool enableUpdates = true;

        public MSComponent(MSEntity msEntity) 
        {
            Debug.Assert(msEntity.SelectedEntities.Any() == true);

            SelectedComponents = msEntity.SelectedEntities.Select(entity => entity.GetComponent<T>()).ToList();
            PropertyChanged += (s, e) => { if (enableUpdates) UpdateConponents(e.PropertyName); };
        }

        protected abstract bool UpdateConponents(string propertyName);
        protected abstract bool UpdateMSComponent();

        public void Refresh()
        {
            enableUpdates = false;
            UpdateMSComponent();
            enableUpdates = true;
        }
    }
}
