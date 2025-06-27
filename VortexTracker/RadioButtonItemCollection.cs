using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexTracker.Controls;

namespace VortexTracker
{
    public class RadioButtonItemCollection : Collection<string>
    {
        private readonly RadioGroup _owner;

        public RadioButtonItemCollection(RadioGroup owner)
        {
            _owner = owner;
        }

        protected override void InsertItem(int index, string item)
        {
            base.InsertItem(index, item);
            _owner.InsertRadioButton(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            _owner.RemoveRadioButtonAt(index);
        }

        protected override void SetItem(int index, string item)
        {
            base.SetItem(index, item);
            _owner.UpdateRadioButtonText(index, item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _owner.ClearRadioButtons();
        }

        public void AddRange(IEnumerable<string> items)
        {
            if (items == null)
                return;

            foreach (var item in items)
                this.Add(item);
        }
    }
}
