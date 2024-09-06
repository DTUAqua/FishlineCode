using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Settings
{
    public class GeneralSettingsViewModel : AViewModel
    {
        private bool _updateLookupsAfterStartup;

        private bool _showScreenOffWarning;

        private SettingsViewModel _vmParent;

        private double _ValueForDeletingLastLenght;
        private double _ValueForGoingToNextStation;
        private double _ValueForGoingToNextOrder;


        #region Properties

        public bool ShowScreenOffWarning
        {
            get { return _showScreenOffWarning; }
            set
            {
                if (_showScreenOffWarning != value)
                {
                    _showScreenOffWarning = value;
                    IsDirty = true;
                }

                RaisePropertyChanged(nameof(ShowScreenOffWarning));
            }
        }

        public bool UpdateLookupsAfterStartup
        {
            get { return _updateLookupsAfterStartup; }
            set
            {
                if (_updateLookupsAfterStartup != value)
                {
                    _updateLookupsAfterStartup = value;
                    IsDirty = true;
                }

                RaisePropertyChanged(nameof(UpdateLookupsAfterStartup));
            }
        }

     
        public double ValueForDeletingLastLenght
        {
            get { return _ValueForDeletingLastLenght; }
            set
            {
               
               _ValueForDeletingLastLenght = value;
                RaisePropertyChanged(nameof(ValueForDeletingLastLenght));
                IsDirty = true;
            }
        }


        public double ValueForGoingToNextStation
        {
            get { return _ValueForGoingToNextStation; }
            set
            {
                _ValueForGoingToNextStation = value;
                RaisePropertyChanged(nameof(ValueForGoingToNextStation));
                IsDirty = true;
            }
        }


        public double ValueForGoingToNextOrder
        {
            get { return _ValueForGoingToNextOrder; }
            set
            {
                _ValueForGoingToNextOrder = value;
                RaisePropertyChanged(nameof(ValueForGoingToNextOrder));
                IsDirty = true;
            }
        }


        #endregion


        public GeneralSettingsViewModel(SettingsViewModel vmParent)
        {
            _vmParent = vmParent;
            UpdateLookupsAfterStartup = AppSettings.UpdateLookupsAfterStartup;
            ValueForDeletingLastLenght = AppSettings.ValueForDeletingLastEntry;
            ValueForGoingToNextStation = AppSettings.ValueForGoingToNextStation;
            ValueForGoingToNextOrder = AppSettings.ValueForGoingToNextOrder;
            ShowScreenOffWarning = AppSettings.ShowWarningOnScreenCloseCommand;
            IsDirty = false;
        }


        public void Persist()
        {
            AppSettings.UpdateLookupsAfterStartup = UpdateLookupsAfterStartup;
            AppSettings.ValueForDeletingLastEntry = _ValueForDeletingLastLenght;
            AppSettings.ValueForGoingToNextOrder = _ValueForGoingToNextOrder;
            AppSettings.ValueForGoingToNextStation = _ValueForGoingToNextStation;
            AppSettings.ShowWarningOnScreenCloseCommand = ShowScreenOffWarning;
            IsDirty = false;
        }
    }
}
