using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SelectDirectoryPathViewModel : AViewModel
    {
        private DelegateCommand _cmdSaveSamples;

        private List<string> _lstDirectories;
        private string _searchStringDirectory;
        private bool _isDefaultPath;

        private List<DirectoryPathItem> _lstDirectoryPathItem;
        private List<DirectoryPathItem> _lstFilteredDirectoryPathItem;

        private string _fileName;

        public bool Saved = false;
        

        #region Properties
        
        public List<DirectoryPathItem> DirectoryPathItems
        {
            get { return _lstDirectoryPathItem; }
            set
            {
                _lstDirectoryPathItem = value;
                RaisePropertyChanged(() => DirectoryPathItems);
            }
        }

        public List<DirectoryPathItem> FilteredDirectoryPathItems
        {
            get { return _lstFilteredDirectoryPathItem; }
            set
            {
                _lstFilteredDirectoryPathItem = value;
                RaisePropertyChanged(() => FilteredDirectoryPathItems);
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set 
            { 
                _fileName = value;
                RaisePropertyChanged(()=>FileName);
            }
        }

        public bool IsDefaultPath
        {
            get { return _isDefaultPath; }
            set 
            { 
                _isDefaultPath = value;
                RaisePropertyChanged(()=>IsDefaultPath);
            }
        }

        public string SearchStringDirectory
        {
            get { return _searchStringDirectory; }
            set
            {
                _searchStringDirectory = value;
                FilterPathItems();
                RaisePropertyChanged(() => SearchStringDirectory);
                RaisePropertyChanged(() => SearchStringDirectoryHasValue);
            }
        }

        public bool SearchStringDirectoryHasValue
        {
            get { return !string.IsNullOrEmpty(SearchStringDirectory); }
        }

        public bool HasSelectedPath
        {
            get { return _lstDirectoryPathItem != null && _lstDirectoryPathItem.Count() > 0 ? _lstDirectoryPathItem.Any(x => x.IsSelected) : false; }
        }

        #endregion
        public SelectDirectoryPathViewModel(List<string> directories, string fileName)
        {
            WindowWidth = 700;
            WindowHeight = 500;

            _lstDirectories = directories;
            _fileName = fileName;
            _lstDirectoryPathItem = new List<DirectoryPathItem>();
            _isDefaultPath = false;

        }

        public void InitializeView()
        {
            if(_lstDirectories != null && _lstDirectories.Count() > 0)
                foreach (var item in _lstDirectories)
                {

                    _lstDirectoryPathItem.Add(new DirectoryPathItem(this, item));
                }

            RaisePropertyChanged(() => DirectoryPathItems);
            FilterPathItems();
        }

        private void FilterPathItems()
        {
            try
            {
                if (_lstFilteredDirectoryPathItem == null)
                {
                    _lstFilteredDirectoryPathItem = new List<DirectoryPathItem>();
                    return;
                }

                IEnumerable<DirectoryPathItem> lst = _lstDirectoryPathItem;

                if (!string.IsNullOrWhiteSpace(SearchStringDirectory))
                {
                    lst = lst.Where(x => x.PathString.Contains(SearchStringDirectory, StringComparison.InvariantCultureIgnoreCase));
                }

               

                FilteredDirectoryPathItems = lst.ToList();
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }

        }



        //Window closed button 
        public override void FireClosing(object sender, CancelEventArgs e)
        {
            if (Saved)
                return;

            if (DirectoryPathItems != null && DirectoryPathItems.Count > 0 && !Saved)
            {
                var res = AppRegionManager.ShowMessageBox(Translate("SelectDirectoryPathView", "CantLeaveTheWindow"), System.Windows.MessageBoxButton.OK);
                e.Cancel = true;
            }

        }

        #region Save Samples Command
        public DelegateCommand SaveCommand
        {
            get
            {
                if (_cmdSaveSamples == null)
                    _cmdSaveSamples = new DelegateCommand(() => SaveSamples());
                return _cmdSaveSamples;
            }
        }

        public void SaveSamples()
        {
            try
            {
                //If all list are empty, return.
                if (FilteredDirectoryPathItems == null || FilteredDirectoryPathItems.Count <= 0  || !FilteredDirectoryPathItems.Any(x => x.IsSelected))
                {
                    var res = AppRegionManager.ShowMessageBox(Translate("SelectDirectoryPathView", "CantLeaveTheWindow"), System.Windows.MessageBoxButton.OK);
                    return;
                }

                Saved = true;
                Close();
                
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }
        #endregion
    }
}
