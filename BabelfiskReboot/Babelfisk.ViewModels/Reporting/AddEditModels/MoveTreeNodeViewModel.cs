using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using Babelfisk.Entities;

namespace Babelfisk.ViewModels.Reporting.AddEditModels
{
    public class MoveTreeNodeViewModel : AViewModel
    {
        public event Action<MoveTreeNodeViewModel> OnSaved;

        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdOK;

        private INodeItem _source;

        private ReportingTreeNode _target;


        #region Properties


        public INodeItem Source
        {
            get { return _source; }
            set
            {
                _source = value;
                RaisePropertyChanged(() => Source);
            }
        }


        public string SourceQuestion
        {
            get { return string.Format("Er du sikker på du vil flytte '{0}'?", Source == null ? "" : ((dynamic)Source).name); }
        }


        public ReportingTreeNode Target
        {
            get { return _target; }
            set
            {
                _target = value;
                RaisePropertyChanged(() => Target);
            }
        }


        public string SourceString
        {
            get
            {
                return _source is ReportingTreeNode ? GetTargetString((_source as ReportingTreeNode).ParentTreeNode) : GetTargetString((_source as Report).ReportingTreeNodes.FirstOrDefault());
            }
        }


        public string TargetString
        {
            get
            {
                return GetTargetString(_target);
            }
        }


        #endregion


        public MoveTreeNodeViewModel(INodeItem niSource, ReportingTreeNode tnTarget)
        {
            _source = niSource;
            _target = tnTarget;
        }


        public static void ShowDialog(MoveTreeNodeViewModel mtn)
        {
            mtn.WindowWidth = 450;
            mtn.WindowTitle = string.Format("Flyt {0}", mtn.Source is Report ? "rapport" : "mappe");
            _appRegionManager.LoadWindowViewFromViewModel(mtn, true, "WindowWithBorderAutoHeightStyle");
        }


        public static string GetTargetString(ReportingTreeNode tn)
        {
            if(tn == null)
                return @"\";

            if (tn.ParentTreeNode == null)
                return @"\" + tn.name;

            return GetTargetString(tn.ParentTreeNode) + @"\" + tn.name;

        }


        #region OK Command


        public DelegateCommand OKCommand
        {
            get { return _cmdOK ?? (_cmdOK = new DelegateCommand(OK)); }
        }


        public void OK()
        {
            //Turn on UI validation (this will also show fire an error on the UI controls causing errors).
            _blnValidate = true;

            //Validate all properties
            ValidateAllProperties();

            //Turn off UI validation
            _blnValidate = false;

            if (HasErrors)
                return;

            IsLoading = true;
            Task.Factory.StartNew(Save).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());

        }

        /// <summary>
        /// Save changes to database. This method does not validate any fields (this should be done prior to calling the method).
        /// </summary>
      
        private void Save()
        {
            try
            {
                var man = new BusinessLogic.Reporting.ReportingManager();

                if (_source is ReportingTreeNode)
                {
                    var node = _source as ReportingTreeNode;
                    node.parentTreeNodeId = _target == null ? null : new Nullable<int>(_target.reportingTreeNodeId);
                    man.SaveReportingTreeNode(ref node);
                    node.AcceptChanges();
                }
                else
                {
                    var node = _source as Report;
                    node.ReportingTreeNodes.Clear();
                    if(_target != null)
                        node.ReportingTreeNodes.Add(_target);
                    man.SaveReport(ref node);
                    node.AcceptChanges();
                }

                DatabaseOperationResult res = DatabaseOperationResult.CreateSuccessResult(); // man.SaveReportingTreeNode(ref _repTreeNode);

                //If saving cruise failed, show error message.
                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    DispatchMessageBox("En uventet fejl opstod. " + res.Message);

                    return;
                }

                new Action(() =>
                {
                    if (OnSaved != null)
                        OnSaved(this);

                    Close();
                }).Dispatch();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("En uventet fejl opstod. {0}", e.Message));
                Anchor.Core.Loggers.Logger.LogError(e);
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
            Close();
        }


        #endregion
    }
}
