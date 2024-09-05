using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.Settings;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.ViewModels.Input
{
    public class ColumnVisibilityViewModel : AViewModel
    {
        private DelegateCommand _cmdSave;
        private DelegateCommand _cmdResetToDefault;


        private string _strVariableType;

        private string _strGroup;

        private DataGridColumnSettingsContainer _container;

        private Action<List<DataGridColumnSettings>> _columnVisibilityChanged;

        private List<DataGridColumnSettings> _lstColumns;

        private bool _blnIsSettingsOpen;


        #region Properties


        public bool HasColumns
        {
            get { return _lstColumns != null && _lstColumns.Count > 0; }
        }


        public List<DataGridColumnSettings> Columns
        {
            get { return _lstColumns; }
            set
            {
                _lstColumns = value;
                RaisePropertyChanged(() => Columns);
                RaisePropertyChanged(() => HasColumns);
            }
        }


        public bool IsSettingsOpen
        {
            get { return _blnIsSettingsOpen; }
            set
            {
                _blnIsSettingsOpen = value;
                RaisePropertyChanged(() => IsSettingsOpen);
            }
        }


        #endregion



        public ColumnVisibilityViewModel(string strVariableType, string strGroup, Action<List<DataGridColumnSettings>> columnVisibilityChanged)
        {
            _strVariableType = strVariableType;
            _strGroup = strGroup;

            _container = BusinessLogic.Settings.Settings.Instance.DataGridColumnSettings;
            _columnVisibilityChanged = columnVisibilityChanged;
           // _container.RefreshColumnsVisibility = columnVisibilityChanged;

            var dic = _container.GetColumns(_strVariableType, _strGroup);

            if (dic != null)
            {
                Columns = dic.Values.OrderBy(x => x.Sorting).ToList();
                foreach(var c in Columns)
                {
                    c.IsVisibleChanged = IsVisibleChanged;
                }
            }

            IsDirty = false;
        }

        private void IsVisibleChanged(DataGridColumnSettings c)
        {
            RaiseColumnsChanged();
        }

        private void ColumnVisibilityChanged(List<DataGridColumnSettings> cs)
        {
            RaiseColumnsChanged();

            IsDirty = true;
        }


        private void RaiseColumnsChanged()
        {
            if (_columnVisibilityChanged != null)
                _columnVisibilityChanged(Columns);
        }



        #region Save Command


        public DelegateCommand SaveCommand
        {
            get { return _cmdSave ?? (_cmdSave = new DelegateCommand(Save)); }
        }


        private void Save()
        {
            IsDirty = false;
        }


        #endregion


        #region Reset To Default Command


        public DelegateCommand ResetToDefaultCommand
        {
            get { return _cmdResetToDefault ?? (_cmdResetToDefault = new DelegateCommand(Reset)); }
        }


        private void Reset()
        {
            if (_container != null && AppRegionManager.ShowMessageBox("Er du sikker på du vil nulstille visningen af kolonner til standardindstillinger?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                _container.ResetAllColumnVisibilitiesToDefault(_strVariableType, _strGroup);

            RaiseColumnsChanged();
        }


        #endregion
    }
}
