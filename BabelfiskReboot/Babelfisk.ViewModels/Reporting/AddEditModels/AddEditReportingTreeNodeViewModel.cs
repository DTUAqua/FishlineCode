using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using System.Threading.Tasks;
using Babelfisk.Entities;

namespace Babelfisk.ViewModels.Reporting.AddEditModels
{
    public class AddEditReportingTreeNodeViewModel : AViewModel
    {
        public event Action<AddEditReportingTreeNodeViewModel> OnSaved;

        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdOK;

        private ReportingTreeNode _repTreeNode;

        private bool _blnIsEdit = false;



        #region Properties


        public bool IsEdit
        {
            get { return _blnIsEdit; }
            set
            {
                _blnIsEdit = value;
                RaisePropertyChanged(() => IsEdit);
            }
        }



        public ReportingTreeNode Entity
        {
            get { return _repTreeNode; }
            set
            {
                _repTreeNode = value;
                RaisePropertyChanged(() => Entity);
            }
        }

        #endregion




        public AddEditReportingTreeNodeViewModel(ReportingTreeNode repTreeNode, ReportingTreeNode repParent)
        {
            //If new tree node, create it and assign its parent (if any)
            if (repTreeNode == null)
            {
                _repTreeNode = new ReportingTreeNode();

                if (repParent != null)
                    _repTreeNode.parentTreeNodeId = repParent.reportingTreeNodeId;
            }
            else
            {
                _repTreeNode = repTreeNode.Clone();
                _blnIsEdit = true;
            }
        }



        /// <summary>
        /// Validates property with name strFieldName. This method is overriding a base method which is called
        /// whenever a property is changed.
        /// </summary>
        protected override string ValidateField(string strFieldName)
        {
            //Only perform validation when user clicks "Save".
            if (_blnValidate)
            {
                switch (strFieldName)
                {
                    case "Entity":
                        if (string.IsNullOrWhiteSpace(Entity.name))
                            return "Angiv venligst et navn til mappen.";
                        break;
                }
            }

            return null;
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
                DatabaseOperationResult res = man.SaveReportingTreeNode(ref _repTreeNode);

                //If saving cruise failed, show error message.
                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    DispatchMessageBox("En uventet fejl opstod. " + res.Message);

                    return;
                }

                new Action(() =>
                {
                    //Reset cruise changetracker.
                    _repTreeNode.AcceptChanges();

                    //Set report in edit mode.
                    IsEdit = true;

                    if (OnSaved != null)
                        OnSaved(this);

                    Close();
                }).Dispatch();

               // DispatchMessageBox("Mappen blev oprettet korrekt.", 2);
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
