// 
// This is part of Vortex Tracker II project
// 
// (c)2000-2009 S.V.Bulba
// Author: Sergey Bulba, vorobey@mail.khstu.ru
// Support page: http://bulba.untergrund.net/
// 
// Version 1.5 - 2.6
// (c)2017-2021 Ivan Pirog, ivan.pirog@gmail.com
// 
// Version 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker.Controls
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
                Add(item);
        }
    }
}
