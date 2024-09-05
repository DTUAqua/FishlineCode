using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anchor.Core;

namespace Babelfisk.Entities.Sprattus
{
    public partial class SDSample
    {
        private bool _isselected;
        public bool IsSelected
        {
            get { return _isselected; }
            set
            {
                _isselected = value;
                OnNavigationPropertyChanged("IsSelected");
            }
        }

        public DateTime? CreatedTimeLocal
        {
            get { return createdTime == null ? null : new Nullable<DateTime>(createdTime.Value.ToLocalTime()); }
            set
            {
                if (value == null)
                    createdTime = null;
                else
                    createdTime = value.Value.ToUniversalTime();
                OnPropertyChanged("CreatedTimeLocal");
            }
        }


        public bool HasSDFiles
        {
            get { return SDFile.Count > 0; }
        }


        public DateTime? ModifiedTimeLocal
        {
            get { return modifiedTime == null ? null : new Nullable<DateTime>(modifiedTime.Value.ToLocalTime()); }
            set
            {
                if (value == null)
                    modifiedTime = null;
                else
                    modifiedTime = value.Value.ToUniversalTime();

                OnPropertyChanged("ModifiedTimeLocal");
            }
        }

        public Decimal? WeightGrams
        {
            get { return _fishWeightG; }
        }


        public string ScalesString
        {
            get
            {
                try
                {
                    if (SDFile.Count == 0)
                        return "";

                    return string.Join(", ", SDFile.Where(x => x.scale.HasValue).Select(x => x.scale.Value.DoubleToString(null)));
                }
                catch { }
                return "";
            }
        }

        public Dictionary<string, DFUPerson> TemporaryPrimaryReader
        {
            get;
            set;
        }

        public IEnumerable<SDFile> SDFilesWithApprovedAnnotations
        {
            get
            {
                return SDFile.Where(f => f.SDAnnotationsApproved.Any());
            }
        }

        private SDFile _selectedSDFileForAgeTransfer = null;
        public SDFile SelectedSDFileForAgeTransfer
        {
            get
            {
                return _selectedSDFileForAgeTransfer;
            }
            set
            {
                try
                {
                    _selectedSDFileForAgeTransfer = value;

                    if (value != null)
                    {
                        var anno = value.SDAnnotationsApproved;
                        IEnumerable<SDAnnotation> primaryReaderAnnotation = null;

                        //If only one approved annotation or if all ages are the same, assign the first annotation.
                        if (anno.Count() == 1)
                        {
                            value.SelectedSDAnnotationForAgeTransfer = anno.First();
                        }
                        //Get the annotation done by the primary reader, if there.
                        else if(TemporaryPrimaryReader != null && (primaryReaderAnnotation = anno.Where(x => x.createdByUserName != null && TemporaryPrimaryReader.ContainsKey(x.createdByUserName.ToLower()))).Any())
                        {
                            value.SelectedSDAnnotationForAgeTransfer = primaryReaderAnnotation.FirstOrDefault();
                        }
                        else
                        {
                            value.SelectedSDAnnotationForAgeTransfer = null;
                        }
                    }
                }
                catch { }
                OnNavigationPropertyChanged("SelectedSDFileForAgeTransfer");
                OnNavigationPropertyChanged("HasSelectedSDFileForAgeTransfer");
            }
        }


        public bool HasSelectedSDFileForAgeTransfer
        {
            get { return _selectedSDFileForAgeTransfer != null; }
        }


        public SDSampleImportStatus? ImportStatusEnum
        {
            get
            {
                SDSampleImportStatus impStatus;
                if (string.IsNullOrEmpty(importStatus) || !Enum.TryParse<SDSampleImportStatus>(importStatus, out impStatus))
                    return null;
                return impStatus;
            }
            set
            {
                importStatus = value.HasValue ? value.Value.ToString() : null;
            }

        }

        public void SetIsSelected(bool val)
        {
            _isselected = val;
        }


        public bool HasValueTypeChanges(SDSample sdCompare, params string[] omittedColumns)
        {
            List<string> oColumns = new List<string>()
            {
                "ScalesString",
                "sdSampleId",
                "sampleId",
                "sdEventId",
                "readOnly",
                "createdTime",
                "CreatedTimeLocal",
                "ModifiedTimeLocal",
                "createdById",
                "createdByUserName",
                "IsSelected",
                "HasSDFiles",
                "sdSampleGuid",
                "modifiedTime",
                "TemporaryPrimaryReader",
                "SelectedSDFileForAgeTransfer",
                "HasSelectedSDFileForAgeTransfer",
                "SDFilesWithApprovedAnnotations"
            };

            if (omittedColumns != null && omittedColumns.Length > 0)
                oColumns.AddRange(omittedColumns);

            if (sdCompare != null && this.HasEntityValueTypeChanges(sdCompare, oColumns.ToArray()))
                return true;

            return false;

        }
    }
}
