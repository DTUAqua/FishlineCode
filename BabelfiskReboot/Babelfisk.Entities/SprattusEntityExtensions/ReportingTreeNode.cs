using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.Entities.Sprattus
{
    public partial class ReportingTreeNode : INodeItem
    {
        private bool _blnIsExpanded;
        private bool _blnIsSelected;
        private bool _blnIsVisible = true;
        private bool? _blnIsChecked = false;


        /// <summary>
        /// Gets/sets whether the treeviewitem associated with this object is expanded.
        /// </summary>
        [DataMember]
        public bool IsExpanded
        {
            get { return _blnIsExpanded; }
            set
            {
                _blnIsExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        /// <summary>
        /// A boolean value indicating whether the treeviewitem is checked/unchecked (when tree is rendered with a checkbox)
        /// </summary>
        [DataMember]
        public bool? IsChecked
        {
            get { return _blnIsChecked; }
            set
            {
                _blnIsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }


        /// <summary>
        /// Get/Set whether the treeviewitem associated with this item is selected.
        /// </summary>
        [DataMember]
        public bool IsSelected
        {
            get { return _blnIsSelected; }
            set
            {
                _blnIsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }


        /// <summary>
        /// Get/Set whether the treeviewitem associated with this item is visible.
        /// </summary>
        [DataMember]
        public bool IsVisible
        {
            get { return _blnIsVisible; }
            set
            {
                _blnIsVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

    }
}
