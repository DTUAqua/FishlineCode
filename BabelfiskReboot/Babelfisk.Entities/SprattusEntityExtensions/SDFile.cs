using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class SDFile
    {
        public string UIDisplayForAgeTransfer
        {
            get
            {
                return string.Format("{0}", fileName);
            }
        }


        public IEnumerable<SDAnnotation> SDAnnotationsApproved
        {
            get
            {
                return SDAnnotation.Where(a => a.age.HasValue && a.isApproved.HasValue && a.isApproved.Value);
            }
        }


        private SDAnnotation _selectedSDAnnotationForAgeTransfer = null;

        public SDAnnotation SelectedSDAnnotationForAgeTransfer
        {
            get
            {
                return _selectedSDAnnotationForAgeTransfer;
            }
            set
            {
                _selectedSDAnnotationForAgeTransfer = value;
                OnNavigationPropertyChanged("SelectedSDAnnotationForAgeTransfer");
                OnNavigationPropertyChanged("HasSelectedSDAnnotationForAgeTransfer");
            }
        }

        public bool HasSelectedSDAnnotationForAgeTransfer
        {
            get { return _selectedSDAnnotationForAgeTransfer != null; }
        }


        public void AssignRelativePath(string inPath)
        {
            try
            {
                //Remove the leading '\' character if it is there.
                if (inPath != null && inPath.StartsWith("\\"))
                    inPath = path.Substring(1);
            }
            catch { }

            this.path = inPath;
        }
    }
}
