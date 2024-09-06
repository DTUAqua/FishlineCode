using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Export
{
    public class TreeViewItemViewModel : AViewModel
    {
        public event Action<TreeViewItemViewModel> OnCheckedChanged;

        private TreeViewItemViewModel _parent;


        public bool? _IsChecked { get; set; }

        public virtual bool? IsChecked
        {
            get { return _IsChecked; }
            set
            {
                var old = _IsChecked;
                _IsChecked = value;

                RaisePropertyChanged(nameof(IsChecked));

                if (value != old && OnCheckedChanged != null)
                    OnCheckedChanged(this);
            }
        }

        private bool _isExpanded { get; set; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(nameof(IsExpanded));
            }
        }

        private string _name;

        public virtual string Name
        {
            get { return _name; }
           
        }


        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                RaisePropertyChanged(nameof(Parent));
            }
        }

    }
}
