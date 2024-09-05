using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SelectImportFileAndImageFoldersViewModel : AViewModel
    {
        private DelegateCommand _cmdImport;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdEditImageFolders;
        private DelegateCommand _cmdBrowse;

        private string _filePath;

        private string[] _imageFolderPaths;

        private bool _cancelled = true;


        #region Properties


        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                RaisePropertyChanged(() => FilePath);
            }
        }


        public string[] ImageFolderPaths
        {
            get { return _imageFolderPaths; }
            set
            {
                _imageFolderPaths = value;
                RaisePropertyChanged(() => ImageFolderPaths);
                RaisePropertyChanged(() => HasImageFolders);
            }
        }


        public bool HasImageFolders
        {
            get { return _imageFolderPaths != null && _imageFolderPaths.Length > 0; }
        }


        public bool IsCancelled
        {
            get { return _cancelled; }
            set
            {
                _cancelled = value;
                RaisePropertyChanged(() => IsCancelled);
            }
        }


        #endregion


        public SelectImportFileAndImageFoldersViewModel(string[] defaultImageFolderPaths)
        {
            _imageFolderPaths = defaultImageFolderPaths;
        }


        public bool Initialize()
        {
            return Browse();
        }


        #region Import Command


        public DelegateCommand ImportCommand
        {
            get { return _cmdImport ?? (_cmdImport = new DelegateCommand(Import)); }
        }

        public void Import()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FilePath))
                {
                    AppRegionManager.ShowMessageBox(Translate("SelectImportFileAndImageFoldersView", "SelectFileToImportMessage"));
                    return;
                }

                if (!File.Exists(FilePath))
                {
                    AppRegionManager.ShowMessageBox(Translate("SelectImportFileAndImageFoldersView", "FileDoesNotExistMessage"));
                    return;
                }

                if (SDSamplesViewModel.IsFileinUse(new FileInfo(FilePath)))
                {
                    AppRegionManager.ShowMessageBox(Translate("SDSampleView", "WarningFileIsOpen"), System.Windows.MessageBoxButton.OK);
                    return;
                }

                IsCancelled = false;

                Close();
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }

        public void Cancel()
        {
            try
            {
                IsCancelled = true;

                Close();
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }

        #endregion


        #region Edit Image Folders Command


        public DelegateCommand EditImageFoldersCommand
        {
            get { return _cmdEditImageFolders ?? (_cmdEditImageFolders = new DelegateCommand(EditImageFolders)); }
        }

        public void EditImageFolders()
        {
            try
            {
                var vm = new SelectFoldersOrFilesViewModel(true, false, false, ImageFolderPaths);
                vm.WindowWidth = 600;
                vm.WindowHeight = 400;
                vm.WindowTitle = Translate("AddEditSDEventView", "SelectFoldersHeader"); //"Select folders";

                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "StandardWindowWithBorderStyle");

                //Assign selection, unless Cancel was clicked.
                if (!vm.IsCancelled)
                    ImageFolderPaths = vm.GetSelectedFolders();
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion


        #region Browse Command


        public DelegateCommand BrowseCommand
        {
            get { return _cmdBrowse ?? (_cmdBrowse = new DelegateCommand(() => Browse())); }
        }


        private bool Browse()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Import CSV File";
                dialog.Filter = "CSV Files (*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK)
                    FilePath = dialog.FileName;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogAndDispatchUnexpectedErrorMessage(ex);
            }

            return true;
        }

        #endregion

    }
}
