using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.TreeView
{
    public class AnimalTreeItemViewModel : TreeItemViewModel
    {
        private string _strHeader = "";

        public override string Header
        {
            get { return _strHeader; }
        }

        public AnimalTreeItemViewModel(TreeItemViewModel parent, string strHeader)
            : base(parent, false)
        {
            _strHeader = strHeader;
        }
    }
}
