using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SDSampleItem : AViewModel
    {
        private ImportFromCSVViewModel _importFromCSVVM;

        private SDSample _sdsample;
        
        private List<SDMessageItem> _warningErrorMessages;
        
        private int _csvRowNumber;
        private bool _isSelected;
        private bool _isAdded;


        #region Properties

        public SDSample Sample
        {
            get { return _sdsample; }
            set
            {
                _sdsample = value;
                RaisePropertyChanged(() => Sample);
            }
        }


        public List<SDMessageItem> WarningErrorMessages
        {
            get { return _warningErrorMessages; }
            set
            {
                _warningErrorMessages = value;
                RaisePropertyChanged(() => WarningErrorMessages);
                RaisePropertyChanged(() => HasErrors);
                RaisePropertyChanged(() => HasWarnings);
            }
        }


        public int CSVRowNumber
        {
            get { return _csvRowNumber; }
        }


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if(!HasErrors)
                    _isSelected = value;
                if (_importFromCSVVM != null)
                    _importFromCSVVM.RaiseIsAllSelected();
                RaisePropertyChanged(() => IsSelected);
            }
        }


        public bool IsAdded
        {
            get { return _isAdded; }
            set
            {
                _isAdded = value;
                RaisePropertyChanged(() => IsAdded);
            }

        }


        public bool HasErrors
        {
            get { return WarningErrorMessages != null && WarningErrorMessages.Count > 0 ? WarningErrorMessages.Any(x => x.Type == MessageType.Error) : false; }
        }


        public bool HasWarnings
        {
            get { return !HasErrors && WarningErrorMessages != null && WarningErrorMessages.Count > 0 ? WarningErrorMessages.Any(x => x.Type == MessageType.Warning) : false; }
        }


        public bool HasDuplicates
        {
            get { return !HasWarnings && WarningErrorMessages != null && WarningErrorMessages.Count > 0 ? WarningErrorMessages.Any(x => x.Type == MessageType.Duplicate) : false; }
        }


        public bool HasErrorsOrWarnings
        {
            get { return HasErrors || HasWarnings || HasDuplicates; }
        }


        #endregion

        public SDSampleItem(ImportFromCSVViewModel ifcsvvm, SDSample sdSample, int rowNumber)
        {
            _importFromCSVVM = ifcsvvm;
            _sdsample = sdSample;
            _isSelected = false;
            _isAdded = false;
            _csvRowNumber = rowNumber;

            WarningErrorMessages = new List<SDMessageItem>();
        }

        public void SetIsSelected(bool val)
        {
            IsSelected = val;
        }

        public void RaiseHasErrors()
        {
            RaisePropertyChanged(() => HasErrors);
            RaisePropertyChanged(() => HasWarnings);
            RaisePropertyChanged(() => HasDuplicates);

            if (HasErrors && IsSelected)
                _isSelected = false;
        }
    }
}
