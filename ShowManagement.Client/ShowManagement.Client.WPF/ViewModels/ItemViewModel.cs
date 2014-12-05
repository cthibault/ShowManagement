using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    class ItemViewModel<TItem, TKey>
    {
        public ItemViewModel(TItem item, TKey key, string displayValue)
        {
            this._item = item;
            this._key = key;
            this._displayValue = displayValue;
        }

        public TItem Item { get { return this._item; } }
        private readonly TItem _item;

        public TKey Key { get { return this._key; } }
        private readonly TKey _key;

        public string DisplayValue { get { return this._displayValue; } }
        private readonly string _displayValue;
    }
}
