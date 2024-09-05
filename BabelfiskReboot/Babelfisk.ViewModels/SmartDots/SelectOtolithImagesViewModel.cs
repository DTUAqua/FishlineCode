using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SelectOtolithImagesViewModel: AViewModel
    {
        private List<OtolithFileItem> _otolithItemList;
        private List<OtolithFileItem> _filteredOtolithItemList;
        private List<OtolithFileItem> _selectedOtolithItemList;

        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdSave;

        private string _searchStringDirectory;
        private string _searchStringName;

        public bool IsCanceled = true;

        private string _description = null;

        private bool _isSingleSelection = false;

        private bool _showMessageWhenNoFilesAreSelected = true;

        private string _cancelButtonText = Translate("Common", "Cancel");

        private string[] _serverImageFolders = null;


        #region Properties


        public List<OtolithFileItem> OtolithItemList
        {
            get { return _otolithItemList; }
            set
            {
                _otolithItemList = value;
                RaisePropertyChanged(() => OtolithItemList);
            }
        }


        public List<OtolithFileItem> FilteredOtolithItemList
        {
            get { return _filteredOtolithItemList; }
            set
            {
                _filteredOtolithItemList = value;
                RaisePropertyChanged(() => FilteredOtolithItemList);
            }
        }


        public List<OtolithFileItem> SelectedOtolithItemList
        {
            get { return _selectedOtolithItemList; }
            set
            {
                _selectedOtolithItemList = value;
                RaisePropertyChanged(() => SelectedOtolithItemList);
            }
        }


        public string SearchStringDirectory
        {
            get { return _searchStringDirectory; }
            set 
            {
                _searchStringDirectory = value;
                FilterOtolithFileItems();
                RaisePropertyChanged(()=>SearchStringDirectory);
                RaisePropertyChanged(() => SearchStringDirectoryHasValue);
            }
        }


        public string SearchStringName
        {
            get { return _searchStringName; }
            set
            {
                _searchStringName = value;
                FilterOtolithFileItems();
                RaisePropertyChanged(() => SearchStringName);
                RaisePropertyChanged(() => SearchStringNameHasValue);
            }
        }


        public bool SearchStringDirectoryHasValue
        {
            get { return !string.IsNullOrEmpty(SearchStringDirectory); }
        }


        public bool SearchStringNameHasValue
        {
            get { return !string.IsNullOrEmpty(SearchStringName); }
        }


        public bool HasOtolithFiles
        {
            get { return OtolithItemList != null && OtolithItemList.Count > 0; }
        }


        public bool HasSelectedFiles
        {
            get { return OtolithItemList != null && OtolithItemList.Count > 0 ? OtolithItemList.Any(x => x.IsSelected) : false; }
        }


        public int SelectedFilesCount
        {
            get { return _filteredOtolithItemList != null ? _filteredOtolithItemList.Where(x => x.IsSelected).Count() : 0; }
        }


        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }


        public bool HasDescription
        {
            get { return !string.IsNullOrWhiteSpace(_description); }
        }

        
        public bool IsSingleSelection
        {
            get { return _isSingleSelection; }
            set
            {
                _isSingleSelection = value;
                RaisePropertyChanged(() => IsSingleSelection);
            }
        }


        public bool ShowMessageWhenNoFilesAreSelected
        {
            get { return _showMessageWhenNoFilesAreSelected; }
            set
            {
                _showMessageWhenNoFilesAreSelected = value;
                RaisePropertyChanged(() => ShowMessageWhenNoFilesAreSelected);
            }
        }


        public string CancelButtonText
        {
            get { return _cancelButtonText; }
            set
            {
                _cancelButtonText = value;
                RaisePropertyChanged(() => CancelButtonText);
            }
        }


        #endregion


        public SelectOtolithImagesViewModel(string[] serverImageFolders = null)
        {
            WindowWidth = 950;
            WindowHeight = 535;

            _serverImageFolders = serverImageFolders;

            FilteredOtolithItemList = new List<OtolithFileItem>();
            OtolithItemList = new List<OtolithFileItem>();
            SelectedOtolithItemList = new List<OtolithFileItem>();
        }

        public void InitializeAsync(IEnumerable<string> filePaths = null)
        {
            IsLoading = true;
            Task.Factory.StartNew(() => Initialize(filePaths)).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        private void Initialize(IEnumerable<string> suppliedFilePaths = null)
        {
            try
            {
                string[] filePaths = null;
                if (suppliedFilePaths == null)
                {
                    var man = new BusinessLogic.SmartDots.SmartDotsManager();
                    filePaths = man.GetAllFilePaths(_serverImageFolders);
                }
                else if(suppliedFilePaths.Any())
                {
                    filePaths = suppliedFilePaths.ToArray();
                }

                if (filePaths == null)
                    return;

                var tempOtolithItemList = new List<OtolithFileItem>();

                foreach (var path in filePaths)
                {
                    tempOtolithItemList.Add(new OtolithFileItem(this, path));
                }
                new Action(() =>
                {
                    OtolithItemList = tempOtolithItemList;
                    FilterOtolithFileItems();

                }).Dispatch();

            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }

        }
        public void FilterOtolithFileItems()
        {
            try
            {
                if (_otolithItemList == null)
                {
                    FilteredOtolithItemList = new List<OtolithFileItem>();
                    return;
                }

                IEnumerable<OtolithFileItem> lst = _otolithItemList;

                if (!string.IsNullOrWhiteSpace(SearchStringName))
                {
                    var search = SearchStringName ?? "";
                    lst = lst.Where(x => (x.ImageName.Contains(search, StringComparison.InvariantCultureIgnoreCase)));
                }

                if (!string.IsNullOrWhiteSpace(SearchStringDirectory))
                {
                    var search = SearchStringDirectory ?? "";
                    lst = lst.Where(x => (x.ImageDirectory.Contains(search, StringComparison.InvariantCultureIgnoreCase)));
                }

                FilteredOtolithItemList = lst.ToList();
                RaisePropertyChanged(() => HasOtolithFiles);
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
           

        }

        public void RaiseIsAllSelected()
        {
            RaisePropertyChanged(() => HasSelectedFiles);
            RaisePropertyChanged(() => SelectedFilesCount);
        }

        #region Check box
        private bool IsControlDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }
        public void CheckBox_Initialized(object sender, EventArgs e, Key? keyLastPressed)
        {
            CheckBox tb = (sender as CheckBox);
            bool blnIsCtrlDown = IsControlDown();

            new Action(() =>
            {
                if (keyLastPressed.HasValue && keyLastPressed.Value != Key.None && !blnIsCtrlDown && Keyboard.IsKeyDown(keyLastPressed.Value) && keyLastPressed.Value == Key.Space)
                {
                    if (tb.IsThreeState)
                        tb.IsChecked = (tb.IsChecked == null ? false : (tb.IsChecked.Value ? null : new Nullable<bool>(true)));
                    else
                        tb.IsChecked = !tb.IsChecked;
                }
            }).Dispatch();
        }
        #endregion

        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get
            {
                if (_cmdCancel == null)
                    _cmdCancel = new DelegateCommand(() => Return());

                return _cmdCancel;
            }
        }


        /// <summary>
        /// Cancel any alterations or the new sample.
        /// </summary>
        private void Return()
        {
            IsCanceled = true;
            Close();
        }


        //Window closed button same logic as cancel button
        public override void FireClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (!IsCanceled)
                    return;

                //check for changes
                if (HasSelectedFiles && ShowMessageWhenNoFilesAreSelected)
                {
                    if (AppRegionManager.ShowMessageBox(Translate("SelectOtolithImagesView", "WarningHasSelectedFiles"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        #endregion

        #region Add Files Command


        public DelegateCommand AddFilesCommand
        {
            get
            {
                if (_cmdSave == null)
                    _cmdSave = new DelegateCommand(() => AddFiles());

                return _cmdSave;
            }
        }

        private void AddFiles()
        {
            try
            {
                if (SelectedFilesCount == 0)
                {
                    if (ShowMessageWhenNoFilesAreSelected)
                        AppRegionManager.ShowMessageBox(Translate("SelectOtolithImagesView", "WarningNothingSelected"), System.Windows.MessageBoxButton.OK);
                      
                    return;
                }
                else
                {
                    if (OtolithItemList != null && OtolithItemList.Count > 0 && OtolithItemList.Any(x => x.IsSelected))
                        SelectedOtolithItemList = OtolithItemList.Where(x => x.IsSelected).ToList();

                    if (SelectedOtolithItemList != null && SelectedOtolithItemList.Count > 10)
                    {
                        if (AppRegionManager.ShowMessageBox(string.Format(Translate("SelectOtolithImagesView", "WarningTenOrMoreSelected"), SelectedOtolithItemList.Count), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    IsCanceled = false;
                    Close();
                }
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
          
        }
        #endregion
    }
}
