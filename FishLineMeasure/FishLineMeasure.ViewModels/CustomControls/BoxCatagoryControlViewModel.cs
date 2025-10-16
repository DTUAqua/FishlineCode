using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using FishLineMeasure.ViewModels.Infrastructure;
using FishLineMeasure.ViewModels.Lenghts;
using FishLineMeasure.ViewModels.Lookups;

namespace FishLineMeasure.ViewModels.CustomControls
{
    public class BoxCatagoryControlViewModel : AViewModel
    {
        private string _displayName;
        private string _search;
        private bool _isMandatory = false;
        private CheckBoxControlViewModel<Lookups.LookupItemViewModel> _selectedItem;

        private List<CheckBoxControlViewModel<Lookups.LookupItemViewModel>> _lookups;

        private List<CheckBoxControlViewModel<Lookups.LookupItemViewModel>> _allLookups;

        private Type _lookupType;

        private AddRowViewModel _vmParent;


        #region Properties


        public List<CheckBoxControlViewModel<Lookups.LookupItemViewModel>> Lookups
        {
            get { return _lookups; }
            set
            {
                _lookups = value;
                RaisePropertyChanged(nameof(Lookups));
               // SetEventsUP(_options);
            }
        }


        public Lookups.LookupItemViewModel SelectedLookup
        {
            get
            {
                if (_lookups == null)
                    return null;

                var li = _lookups.Where(x => x.IsChecked).FirstOrDefault();

                if (li == null)
                    return null;

                return li.Entity;
            }
        }


        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                RaisePropertyChanged(nameof(Search));
                if (!string.IsNullOrEmpty(_search))
                {
                    Lookups = _allLookups.Where(L => L.UIDisplay.Contains(_search, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    Lookups = _allLookups.ToList();
                }
                
            }
        }


        public bool HasSelectedLookup
        {
            get { return SelectedLookup != null; }
        }


        public CheckBoxControlViewModel<Lookups.LookupItemViewModel> SelectedItem
        {
            get { return _selectedItem; }
            private set
            {
                if(_selectedItem != value)
                {
                    _selectedItem = value;

                    if(_vmParent != null)
                        _vmParent.SelectedBoxCategoryItemChanged(this);
                }
                RaisePropertyChanged(() => SelectedItem);
                RaisePropertyChanged(() => HasSelectedItem);
            }
        }


        public bool HasSelectedItem
        {
            get { return _selectedItem != null; }
        }


        public bool IsMandatory
        {
            get
            {
                return _isMandatory;
            }
        }


        public Type LookupType
        {
            get { return _lookupType; }
        }


        #endregion


        public BoxCatagoryControlViewModel(AddRowViewModel parent, string displayName, List<Lookups.LookupItemViewModel> lookups, Type tLookup)
        {
            _vmParent = parent;
            _lookupType = tLookup;
            DisplayName = displayName;
            _allLookups = new List<CheckBoxControlViewModel<Lookups.LookupItemViewModel>>();
            _isMandatory = LookupsViewModel.IsLookupMandatory(tLookup);

            try
            {
                foreach (var l in lookups)
                {
                    var c = new CheckBoxControlViewModel<Lookups.LookupItemViewModel>(l, u => u.UIDisplay, u => u.Type);
                    c.OnCheckedChanged += Lookup_OnCheckedChanged;
                    _allLookups.Add(c);
                }
                Lookups = _allLookups.ToList();
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }

        public void Lookup_OnCheckedChanged(CheckBoxControlViewModel<Lookups.LookupItemViewModel> item, bool oldValue, bool newValue)
        {
            try
            {
                //Uncheck any old value.
                if (SelectedItem != null && SelectedItem != item && SelectedItem.IsChecked)
                    SelectedItem.IsChecked = false;

                if (newValue)
                {
                    SelectedItem = item;
                    if(!SelectedItem.IsChecked)
                        SelectedItem.IsChecked = true;
                }
                else
                    SelectedItem = null;
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }

        public void UnCheckAll()
        {
            try
            {
                if (_lookups == null)
                    return;

                _lookups.ForEach(l =>
                {
                    if (l.IsChecked)
                        l.IsChecked = false;
                });

                SelectedItem = null;
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }


        public override void Dispose()
        {
            base.Dispose();

            if (_allLookups != null)
            {
                _allLookups.ForEach(x => x.OnCheckedChanged -= Lookup_OnCheckedChanged);
                _allLookups.Clear();
            }
        }

    }
}
