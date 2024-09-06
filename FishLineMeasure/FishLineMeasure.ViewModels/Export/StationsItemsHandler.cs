using FishLineMeasure.ViewModels.Overview;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Export
{
    public class StationsItemsHandler : TreeViewItemViewModel
    {
        public event Action<StationsItemsHandler> ReactOnChanges;
        private  StationViewModel _vmStation;
        public StationViewModel VmStation
        {
            get { return _vmStation; }
            set
            {
                _vmStation = value;
                RaisePropertyChanged(nameof(VmStation));
            }
        }
        public override string Name => _vmStation.StationNumber;
        public StationsItemsHandler()
        {
            IsChecked = false;
            OnCheckedChanged += OnCheckMarkValueChanged;
        }

        private void OnCheckMarkValueChanged(TreeViewItemViewModel obj)
        {
            if (ReactOnChanges != null)
            {
                ReactOnChanges(this);
            }
        }
    }
}
