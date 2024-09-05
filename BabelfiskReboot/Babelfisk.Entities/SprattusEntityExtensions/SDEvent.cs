using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Anchor.Core;

namespace Babelfisk.Entities.Sprattus
{
    public partial class SDEvent
    {
        private string _eventComments;



        public int AccessibleUsersCount
        {
            get
            {
                return SDReaders == null ? 0 : SDReaders.Count;
            }
        }

        public string DFUAreaString
        {
            get { return (L_DFUAreas != null && L_DFUAreas.Count > 0) ? string.Join(", ", L_DFUAreas.Select(x => x.DFUArea)) : ""; } 
        }


        public string EventComments
        {
            get { return _eventComments; }
            set
            {
                _eventComments = value;
                OnPropertyChanged("EventComment");
            }
        }


        public string EventIdAndNameString
        {
            get { return sdEventId != 0 && !string.IsNullOrEmpty(name) ? string.Format("{0} - {1}", sdEventId, name) : "Missing event id or name"; }
        }


        public bool HasEndDate
        {
            get { return endDate != null && endDate != default(DateTime); }
        }


        private int _samplesCount;

        [DataMember]
        public int SamplesCount
        {
            get { return _samplesCount; }
            set
            {
                _samplesCount = value;
                OnPropertyChanged("SamplesCount");
            }
        }



        public List<SDFilesExtraColumn> SDFileExtraColumns
        {
            get
            {
                var lst = new List<SDFilesExtraColumn>();

                try
                {
                    if (!string.IsNullOrWhiteSpace(uiSDFileExtraColumns))
                    {
                        var arr = uiSDFileExtraColumns.Split(';');

                        foreach (var s in arr)
                        {
                            SDFilesExtraColumn c;
                            if (Enum.TryParse<SDFilesExtraColumn>(s, out c))
                                lst.Add(c);
                        }
                    }
                }
                catch { }

                return lst;
            }
            set
            {
                try
                {
                    uiSDFileExtraColumns = (value == null || value.Count == 0) ? null : string.Join(";", value.Select(x => x.ToString()));
                }
                catch { }
                OnNavigationPropertyChanged("SDFileExtraColumns");
            }
        }


        public bool IsYearlyReadingEventType
        {
            get { return IsEventTypeYearlyReading(_sdEventTypeId); }
        }


        public bool IsTogetherReadingEventType
        {
            get { return IsEventTypeTogetherReading(_sdEventTypeId); }
        }


        public bool IsReferenceEventType
        {
            get { return IsEventTypeReference(_sdEventTypeId); }
        }


        public static bool IsEventTypeYearlyReading(int eventTypeId)
        {
            return eventTypeId == 1;
        }


        public static bool IsEventTypeTogetherReading(int eventTypeId)
        {
            return eventTypeId == 2;
        }


        public static bool IsEventTypeReference(int eventTypeId)
        {
            return eventTypeId == 3;
        }


        public string[] DefaultImageFoldersArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(defaultImageFolders))
                    return new string[] { };

                return defaultImageFolders.Split('|');
            }
            set
            {
                defaultImageFolders = (value == null || value.Length == 0) ? null : string.Join("|", value);
                OnPropertyChanged("DefaultImageFoldersArray");
            }
        }
    }
}
