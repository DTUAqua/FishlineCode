using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.Entities.Sprattus
{
    public partial class Report : INodeItem
    {
        private bool _blnIsExpanded;
        private bool _blnIsSelected;
        private bool _blnIsVisible = true;
        private bool? _blnIsChecked = false;

        private string _strReportDetails;

        private List<ReportFile> _lstFiles;


        /// <summary>
        /// Gets/sets whether the treeviewitem associated with this object is expanded.
        /// </summary>
        [DataMember]
        public bool IsExpanded
        {
            get { return _blnIsExpanded; }
            set
            {
                _blnIsExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        /// <summary>
        /// A boolean value indicating whether the treeviewitem is checked/unchecked (when tree is rendered with a checkbox)
        /// </summary>
        [DataMember]
        public bool? IsChecked
        {
            get { return _blnIsChecked; }
            set
            {
                _blnIsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }


        /// <summary>
        /// Get/Set whether the treeviewitem associated with this item is selected.
        /// </summary>
        [DataMember]
        public bool IsSelected
        {
            get { return _blnIsSelected; }
            set
            {
                _blnIsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }


        /// <summary>
        /// Get/Set whether the treeviewitem associated with this item is visible.
        /// </summary>
        [DataMember]
        public bool IsVisible
        {
            get { return _blnIsVisible; }
            set
            {
                _blnIsVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

         [DataMember]
        public string Details
        {
            get { return _strReportDetails; }
            set
            {
                _strReportDetails = value;
                OnPropertyChanged("Details");
            }
        }


          [DataMember]
         public List<ReportFile> Files
         {
             get { return _lstFiles; }
             set
             {
                 _lstFiles = value;
                 OnPropertyChanged("Files");
             }
         }

          private HashSet<SecurityTask>  _lstPermissions = null;

          public HashSet<SecurityTask> Permissions
          {
              get
              {
                  try
                  {
                      if (_lstPermissions == null)
                      {
                          _lstPermissions = new HashSet<SecurityTask>();

                          if (!string.IsNullOrWhiteSpace(permissionTasks))
                          {
                              var lstTasks = permissionTasks.Split(';');

                              foreach (var t in lstTasks)
                              {
                                  SecurityTask st;

                                  if (Enum.TryParse<SecurityTask>(t, out st) && !_lstPermissions.Contains(st))
                                      _lstPermissions.Add(st);
                              }
                          }
                      }
                  }
                  catch (Exception e)
                  {
                      Anchor.Core.Loggers.Logger.LogError(e);
                  }

                  return _lstPermissions;
              }
              set
              {
                  _lstPermissions = value;

                  if (_lstPermissions == null || _lstPermissions.Count == 0)
                      permissionTasks = null;
                  else
                      permissionTasks = string.Join(";", _lstPermissions.Select(x => x.ToString()));
              }
          }


          public bool IsDocumentType
          {
              get { return type == "Document"; }
          }
    }
}
