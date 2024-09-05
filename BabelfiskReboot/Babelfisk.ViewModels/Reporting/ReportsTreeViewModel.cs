using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.Entities;
using System.Collections.ObjectModel;

namespace Babelfisk.ViewModels.Reporting
{
    public class ReportsTreeViewModel : AViewModel
    {
        private DelegateCommand<object> _cmdNewFolder;
        private DelegateCommand<object> _cmdEditFolder;
        private DelegateCommand<object> _cmdDeleteFolder;
        private DelegateCommand<object> _cmdNewReport;
        private DelegateCommand<object> _cmdEditReport;
        private DelegateCommand<object> _cmdDeleteReport;

        private DelegateCommand _cmdRefresh;

        private ObservableCollection<IObjectWithChangeTracker> _lstTreeNodes;

        private AViewModel _vmSelectedItem;


        #region Properties


        public ObservableCollection<IObjectWithChangeTracker> TreeNodes
        {
            get { return _lstTreeNodes; }
            set
            {
                _lstTreeNodes = value;
                RaisePropertyChanged(() => TreeNodes);
            }
        }

       
        public AViewModel SelectedItem
        {
            get { return _vmSelectedItem; }
            set
            {
                if (_vmSelectedItem != null)
                    _vmSelectedItem.Dispose();

                _vmSelectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
                RaisePropertyChanged(() => HasSelectedItem);
            }
        }


        public bool HasSelectedItem
        {
            get { return _vmSelectedItem != null; }
        }


        public bool CanEditReports
        {
            get { return User.IsAdmin; }
        }


        #endregion



        public ReportsTreeViewModel()
        {
        }


        public Task InitializeAsync()
        {
            IsLoading = true;
            return Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        public void Initialize()
        {
            var lstReportNodes = GetReportTreeNodes();

            new Action(() =>
            {
                try
                {
                    TreeNodes = lstReportNodes.ToObservableCollection();
                }
                catch (Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                }
            }).Dispatch();
        }


        private List<IObjectWithChangeTracker> GetReportTreeNodes()
        {
            List<IObjectWithChangeTracker> lst = null;
            try
            {
                var repMan = new BusinessLogic.Reporting.ReportingManager();

                lst = repMan.GetReportTreeNodes();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }

            return lst;
        }


        #region New Folder Command


        public DelegateCommand<object> NewFolderCommand
        {
            get { return _cmdNewFolder ?? (_cmdNewFolder = new DelegateCommand<object>(obj => NewFolder(obj))); }
        }


        public void NewFolder(object obj)
        {
            ReportingTreeNode repNodeParent = obj as ReportingTreeNode;

            var vm = new AddEditModels.AddEditReportingTreeNodeViewModel(null, repNodeParent);
            vm.WindowWidth = 500;
            vm.WindowTitle = string.Format("Opret ny mappe {0}", repNodeParent == null ? "i roden af træet" : ("under '" + repNodeParent.name + "'"));
            vm.OnSaved += vmAddEditReportingTreeNode_OnSaved;
            vm.Closed += vmAddEditReportingTreeNode_Closed;
            _appRegionManager.LoadWindowViewFromViewModel(vm, false, "WindowWithBorderAutoHeightStyle");
        }

        void vmAddEditReportingTreeNode_OnSaved(AddEditModels.AddEditReportingTreeNodeViewModel obj)
        {
            obj.OnSaved -= vmAddEditReportingTreeNode_OnSaved;

            RefreshTreeAsync(obj.Entity);
        }


        #endregion


        #region Edit Folder Command


        public DelegateCommand<object> EditFolderCommand
        {
            get { return _cmdEditFolder ?? (_cmdEditFolder = new DelegateCommand<object>(obj => EditFolder(obj))); }
        }


        public void EditFolder(object obj)
        {
            ReportingTreeNode repNode = obj as ReportingTreeNode;

            if (repNode == null)
                return;

            var vm = new AddEditModels.AddEditReportingTreeNodeViewModel(repNode, repNode.ParentTreeNode);
            vm.WindowWidth = 500;
            vm.WindowTitle = string.Format("Omdøb mappe '{0}'", repNode.name);
            vm.OnSaved += vmAddEditReportingTreeNode_OnSaved;
            vm.Closed += vmAddEditReportingTreeNode_Closed;
            _appRegionManager.LoadWindowViewFromViewModel(vm, false, "WindowWithBorderAutoHeightStyle");
        }

        void vmAddEditReportingTreeNode_Closed(object arg1, AViewModel arg2)
        {
            arg2.Closed -= vmAddEditReportingTreeNode_Closed;
            if (arg2 is AddEditModels.AddEditReportingTreeNodeViewModel)
                (arg2 as AddEditModels.AddEditReportingTreeNodeViewModel).OnSaved -= vmAddEditReportingTreeNode_OnSaved;
        }



        #endregion


        #region Delete Folder Command


        public DelegateCommand<object> DeleteFolderCommand
        {
            get { return _cmdDeleteFolder ?? (_cmdDeleteFolder = new DelegateCommand<object>(obj => DeleteFolderAsync(obj as ReportingTreeNode))); }
        }


        public void DeleteFolderAsync(ReportingTreeNode repNode)
        {
            if (repNode == null)
                return;

            var lst = repNode.ChildTreeNodes.GetReportNodeHierarchy().ToList();
            lst.AddRange(repNode.Reports);

            if (lst.Count > 0)
            {
                DispatchMessageBox(string.Format("Der er {0} rapport(er) tilgængelig(e) i mappen (eller dens undermapper). Flyt eller slet venligst alle rapporter i mappen (og undermapper) før du sletter den.", lst.Count));
                return;
            }


            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil slette mappen '{0}'?", repNode.name), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            IsLoading = true;
            Task.Factory.StartNew(() => DeleteFolder(repNode)).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void DeleteFolder(ReportingTreeNode repNode)
        {
            try
            {
                var man = new BusinessLogic.Reporting.ReportingManager();
                repNode.MarkAsDeleted();
                DatabaseOperationResult res = man.SaveReportingTreeNode(ref repNode);

                //If saving cruise failed, show error message.
                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    DispatchMessageBox("En uventet fejl opstod. " + res.Message);

                    return;
                }

                RefreshTree();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("En uventet fejl opstod. {0}", e.Message));
            }
        }


        #endregion


        #region Update Command


        public DelegateCommand RefreshCommand
        {
            get { return _cmdRefresh ?? (_cmdRefresh = new DelegateCommand(() => RefreshTreeAsync(null))); }
        }


        public void RefreshTreeAsync(INodeItem repSelectedNode = null)
        {
            IsLoading = true;

            Task.Factory.StartNew(() => RefreshTree(repSelectedNode)).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        /// <summary>
        /// Reloads tree while preserving selected/expanded node states.
        /// </summary>
        public void RefreshTree(INodeItem repSelectedNode = null)
        {
            var lst = GetReportTreeNodes();

            int intSelectedId = -1;
            try
            {
                if (lst != null && _lstTreeNodes != null && _lstTreeNodes.Count > 0)
                {
                    //Synchronize ReportingTreeNode items
                    var dicNew = lst.OfType<ReportingTreeNode>().GetTreeNodeHierarchy().Distinct().ToDictionary(x => x.reportingTreeNodeId);
                    var dicOld = _lstTreeNodes.OfType<ReportingTreeNode>().GetTreeNodeHierarchy().Distinct().ToDictionary(x => x.reportingTreeNodeId);

                    foreach (var old in dicOld)
                    {
                        if (dicNew.ContainsKey(old.Key))
                        {
                            var dNew = dicNew[old.Key];

                            if (old.Value.IsSelected != dNew.IsSelected && repSelectedNode == null)
                                dNew.IsSelected = old.Value.IsSelected;

                            if (old.Value.IsExpanded != dNew.IsExpanded)
                                dNew.IsExpanded = old.Value.IsExpanded;
                        }
                    }

                    //Synchronize Report items
                    var dicRepNew = lst.OfType<INodeItem>().GetReportNodeHierarchy().Distinct().ToDictionary(x => x.reportId);
                    var dicRepOld = _lstTreeNodes.OfType<INodeItem>().GetReportNodeHierarchy().Distinct().ToDictionary(x => x.reportId);

                    foreach (var old in dicRepOld)
                    {
                        if (dicRepNew.ContainsKey(old.Key))
                        {
                            var dNew = dicRepNew[old.Key];

                            if (old.Value.IsSelected != dNew.IsSelected && repSelectedNode == null)
                                dNew.IsSelected = old.Value.IsSelected;

                            if (old.Value.IsExpanded != dNew.IsExpanded)
                                dNew.IsExpanded = old.Value.IsExpanded;
                        }
                    }

                    //Select repSelectedNode (ReportingTreeNode) if it is not null.
                    if (repSelectedNode != null && repSelectedNode is ReportingTreeNode)
                    {
                        intSelectedId = (repSelectedNode as ReportingTreeNode).reportingTreeNodeId;
                        if (dicNew.ContainsKey(intSelectedId))
                        {
                            dicNew[intSelectedId].ExpandAllParents();
                            dicNew[intSelectedId].IsSelected = true;
                        }
                    }

                    //Select repSelectedNode (Report) if it is not null.
                    if (repSelectedNode != null && repSelectedNode is Report)
                    {
                        intSelectedId = (repSelectedNode as Report).reportId;
                        if (dicRepNew.ContainsKey(intSelectedId))
                        {
                            dicRepNew[intSelectedId].ExpandAllParents();
                            dicRepNew[intSelectedId].IsSelected = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            SelectedItem = null;

            new Action(() =>
            {
                TreeNodes = lst.ToObservableCollection();
            }).Dispatch();
        }


        #endregion


        #region Edit Report Command


        public DelegateCommand<object> EditReportCommand
        {
            get { return _cmdEditReport ?? (_cmdEditReport = new DelegateCommand<object>(obj => EditReport(obj))); }
        }


        public void EditReport(object obj)
        {
            Report repNode = obj as Report;

            if (repNode == null)
                return;

            var vm = new AddEditModels.AddEditReportViewModel(repNode, repNode.ReportingTreeNodes.FirstOrDefault());
            vm.WindowWidth = 650;
            vm.WindowTitle = string.Format("Rediger rapport '{0}'", repNode.name);
           // vm.OnSaved += vmAddEditReportingTreeNode_OnSaved;
            vm.Closed += vmAddEditReport_Closed;
            _appRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderAutoHeightStyle");
        }

        protected void vmAddEditReport_Closed(object arg1, AViewModel arg2)
        {
            arg2.Closed -= vmAddEditReport_Closed;

            var vm = arg2 as AddEditModels.AddEditReportViewModel;
            if (vm != null)
            {
                if(vm.IsSaved)
                    RefreshTreeAsync(vm.ReportEntity);
            }
           // if (arg2 is AddEditModels.AddEditReportingTreeNodeViewModel)
           //     (arg2 as AddEditModels.AddEditReportingTreeNodeViewModel).OnSaved -= vmAddEditReportingTreeNode_OnSaved;
        }



        #endregion


        #region New Report Command


        public DelegateCommand<object> NewReportCommand
        {
            get { return _cmdNewReport ?? (_cmdNewReport = new DelegateCommand<object>(obj => NewReport(obj))); }
        }


        public void NewReport(object obj)
        {
            ReportingTreeNode repNode = obj as ReportingTreeNode;

            var vm = new AddEditModels.AddEditReportViewModel(null, repNode);
            vm.WindowWidth = 650;
            vm.WindowTitle = string.Format("Ny rapport");
            // vm.OnSaved += vmAddEditReportingTreeNode_OnSaved;
            vm.Closed += vmAddEditReport_Closed;
            _appRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderAutoHeightStyle");
        }



        #endregion


        #region Delete Report Command


        public DelegateCommand<object> DeleteReportCommand
        {
            get { return _cmdDeleteReport ?? (_cmdDeleteReport = new DelegateCommand<object>(obj => DeleteReportAsync(obj as Report))); }
        }


        public void DeleteReportAsync(Report report)
        {
            if (report == null)
                return;

            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil slette rapporten '{0}'?", report.name), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            IsLoading = true;
            Task.Factory.StartNew(() => DeleteReport(report)).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void DeleteReport(Report rep)
        {
            try
            {
                var man = new BusinessLogic.Reporting.ReportingManager();
                rep.MarkAsDeleted();
                DatabaseOperationResult res = man.SaveReport(ref rep);

                //If saving cruise failed, show error message.
                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    DispatchMessageBox("En uventet fejl opstod. " + res.Message);

                    return;
                }

                RefreshTree();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("En uventet fejl opstod. {0}", e.Message));
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        #endregion
    
    }
}
