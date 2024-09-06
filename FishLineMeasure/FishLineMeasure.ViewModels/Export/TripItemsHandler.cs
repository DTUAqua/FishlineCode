using FishLineMeasure.ViewModels.Overview;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Export
{
    public class TripItemsHandler : TreeViewItemViewModel
    {
        private bool _assigningChildCheckedStatus = false;

        public override bool? IsChecked
        {
            get
            {
                if (tStations == null || tStations.Count == 0)
                    return false;

                var checkedCount = tStations.Where(x => x.IsChecked.HasValue && x.IsChecked.Value).Count();

                if (checkedCount == 0)
                    return false;

                return checkedCount == tStations.Count ? new Nullable<bool>(true) : null;
            }
            set
            {
                var status = IsChecked;

                _assigningChildCheckedStatus = true;
                {
                    try
                    {
                        if (!status.HasValue || !status.Value)
                        {
                            foreach (var s in tStations)
                                if (!s.IsChecked.HasValue || !s.IsChecked.Value)
                                    s.IsChecked = true;
                        }
                        else
                        {
                            foreach (var s in tStations)
                                if (!s.IsChecked.HasValue || s.IsChecked.Value)
                                    s.IsChecked = false;
                        }
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
                _assigningChildCheckedStatus = false;
            }
        }


        public TripViewModel Trip
        {
            get { return _vmTrip; }
        }


        TripViewModel _vmTrip;
        public ObservableCollection<StationsItemsHandler> tStations { get; set; } = new ObservableCollection<StationsItemsHandler>();
        public override string Name
        {
            get => _vmTrip.Key;
        }

        public TripItemsHandler(TripViewModel trip)
        {
            IsChecked = false;
            IsExpanded = false;

            _vmTrip = trip;
            if (trip.Stations != null)
            {
                foreach (var St in trip.Stations)
                {
                    var sth = new StationsItemsHandler { VmStation = St, Parent = this };
                    sth.OnCheckedChanged += Station_OnCheckedChanged;
                    tStations.Add(sth);
                }
            }
        }

        private void Station_OnCheckedChanged(TreeViewItemViewModel obj)
        {
            if (_assigningChildCheckedStatus)
                return;

            RaisePropertyChanged(nameof(IsChecked));
        }


        public override void Dispose()
        {
            try
            {
                if (tStations != null)
                {
                    foreach (var s in tStations)
                    {
                        s.OnCheckedChanged -= Station_OnCheckedChanged;
                        s.Dispose();
                    }
                }

                tStations.Clear();
            }
            catch { }

            base.Dispose();
        }
    }
}
