using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities
{
    public interface INodeItem
    {
        bool IsExpanded
        {
            get;
            set;
        }

        bool? IsChecked
        {
            get;
            set;
        }

        bool IsSelected
        {
            get;
            set;
        }

        bool IsVisible
        {
            get;
            set;
        }
    }
}
