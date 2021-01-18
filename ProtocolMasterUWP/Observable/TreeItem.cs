using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterUWP.Observable
{
    class TreeItem
    {
        public string Name { get; set; }
        public ObservableCollection<TreeItem> Children { get; set; } = new ObservableCollection<TreeItem>();

        public override string ToString()
        {
            return Name;
        }
    }
}
