using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Settings
{
    public class SettingsViewModel : AViewModel
    {
        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdCancel;

        private GeneralSettingsViewModel _vmGeneralSettings;
        private BluetoothSettingsViewModel _vmBluetoothSettings;
        private FrequencySettingsViewModel _vmFrequencySettings;


        private int _selectedTabIndex;


        #region Properties


        public GeneralSettingsViewModel GeneralSettingsVM
        {
            get { return _vmGeneralSettings; }
            set
            {
                _vmGeneralSettings = value;
                RaisePropertyChanged(() => GeneralSettingsVM);
            }
        }

        public BluetoothSettingsViewModel BluetoothSettingsVM
        {
            get { return _vmBluetoothSettings; }
            set
            {
                _vmBluetoothSettings = value;
                RaisePropertyChanged(() => BluetoothSettingsVM);
            }
        }

        public FrequencySettingsViewModel FrequencySettingsVM
        {
            get { return _vmFrequencySettings; }
            set
            {
                _vmFrequencySettings = value;
                RaisePropertyChanged();
            }
        }


        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                RaisePropertyChanged(nameof(SelectedTabIndex));
            }
        }

        #endregion


        public SettingsViewModel()
        {
            WindowTitle = "Indstillinger";
            WindowWidth = 870;
            WindowHeight = 535;

            AdjustWindowWidthHeightToScreen();

            _vmGeneralSettings = new GeneralSettingsViewModel(this);
            _vmBluetoothSettings = new BluetoothSettingsViewModel(this);
            _vmFrequencySettings = new FrequencySettingsViewModel(this);
        }



        protected override string ValidateField(string strFieldName)
        {
            string error = null;

            if (!_blnValidate)
                return error;

            switch(strFieldName)
            {
                case "GeneralSettingsVM":
                    GeneralSettingsVM.ValidateAllProperties();

                    if (GeneralSettingsVM.HasErrors)
                    {
                        error = GeneralSettingsVM.Error;
                        SelectedTabIndex = 0;
                    }
                    break;

                case "BluetoothSettingsVM":
                    BluetoothSettingsVM.ValidateAllProperties();

                    if (BluetoothSettingsVM.HasErrors)
                    {
                        error = BluetoothSettingsVM.Error;
                        SelectedTabIndex = 1;
                    }
                    break;

                case "FrequencySettingsVM":
                    FrequencySettingsVM.ValidateAllProperties();

                    if (FrequencySettingsVM.HasErrors)
                    {
                        error = FrequencySettingsVM.Error;
                        SelectedTabIndex = 2;
                    }
                    break;
            }

            return error;
        }

        #region OK Command


        public DelegateCommand OKCommand
        {
            get
            {
                if (_cmdOK == null)
                    _cmdOK = new DelegateCommand(() => OK());

                return _cmdOK;
            }
        }

        private void OK()
        {
            try
            {
                ValidateAllProperties();

                if (HasErrors)
                    return;

                if(_vmGeneralSettings.IsDirty)
                    _vmGeneralSettings.Persist();

                if (_vmBluetoothSettings.IsDirty)
                    _vmBluetoothSettings.Persist();

                if (_vmFrequencySettings.IsDirty)
                    _vmFrequencySettings.Persist();

                AppSettings.Save();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                AppRegionManager.ShowMessageBox(string.Format("En uventet fejl opstod. {0}", e.Message));
                return;
            }

            Close();

        }


        #endregion


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get
            {
                if (_cmdCancel == null)
                    _cmdCancel = new DelegateCommand(() => Cancel());

                return _cmdCancel;
            }
        }

        private void Cancel()
        {
            Close();
        }


        #endregion
    }
}
