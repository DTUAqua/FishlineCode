using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Data;
using Babelfisk.BusinessLogic;
using Babelfisk.BusinessLogic.DataInput;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.Entities;
using System.Windows.Documents;

namespace Babelfisk.ViewModels.Input
{
    public abstract class ALavSFViewModel : AViewModel
    {
        protected DelegateCommand _cmdNewRow;
        protected DelegateCommand<AnimalItem> _cmdRemoveRow;
        protected DelegateCommand _cmdRefreshOtolithImages;

        protected SubSample _subSample;

        protected List<AnimalItem> _lstAllItems;

        protected ObservableCollection<AnimalItem> _lstItems;

        protected AnimalItem _selectedItem;

        protected SubSampleViewModel _parent;

        protected SubSampleType _enmSubSampleType;

        protected SpeciesList _sl;

        private string _strLastErrorProperty = null;
        private string _strLastErrorMsg = null;

        /// <summary>
        /// Current sorting state.
        /// </summary>
        protected string _strSortMemberPath = null;
        protected ListSortDirection? _sortDirection = null;

        protected string _strLengthMeasureUnit;
        protected int? _strLengthMeasureTypeId;

        private List<L_LengthMeasureType> _lstLengthMeasureType;
        private List<L_Species> _lstSpeciesLookups;

        /// <summary>
        /// Variable contains the length measure unit, last saved. So if the user changes the measuring unit, it can 
        /// be determined what it was changed from and to.
        /// </summary>
        protected string _strLastLengthMeasureUnit;
        protected int? _strLastLengthMeasureTypeId;


        public override bool HasUnsavedData
        {
            get
            {
                return (_sl != null &&
                       (_sl.ChangeTracker.State != ObjectState.Unchanged && !(_sl.ChangeTracker.State == ObjectState.Added && !IsDirty))) ||
                       (_lstItems != null && _lstItems.Where(x => x.HasUnsavedData).Count() > 0) ||
                       IsDirty;
            }
        }


        #region Properties



        public string LastLengthMeasureUnit
        {
            get { return _strLastLengthMeasureUnit; }
            set
            {
                _strLastLengthMeasureUnit = value;
                RaisePropertyChanged(() => LastLengthMeasureUnit);
            }
        }

        public int? LastLengthMeasureTypeId
        {
            get { return _strLastLengthMeasureTypeId; }
            set
            {
                _strLastLengthMeasureTypeId = value;
                RaisePropertyChanged(() => LastLengthMeasureTypeId);
            }
        }

        public abstract string LavSFType
        {
            get;
        }


        public bool IsSFType
        {
            get { return LavSFType != null && LavSFType.Equals("SF", StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool IsLAVType
        {
            get { return LavSFType != null && LavSFType.Equals("LAV", StringComparison.InvariantCultureIgnoreCase); }
        }


        public SubSampleViewModel Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                RaisePropertyChanged(() => Parent);
            }
        }


        public AnimalItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                var old = _selectedItem;
                _selectedItem = value;

                SelectedAnimalItemChanged(old, _selectedItem);

                RaisePropertyChanged(() => SelectedItem);
                RefreshSums();
            }
        }


        public ObservableCollection<AnimalItem> Items
        {
            get { return _lstItems; }
            set
            {
                _lstItems = value;
                RaisePropertyChanged(() => Items);
            }
        }

        public string WeightInGramsSumString
        {
            get
            {
                return GetSumString<AnimalItem>(x => x.WeightInGrams);
            }
        }


        /// <summary>
        /// Weight in kilograms as a string
        /// </summary>
        public string WeightSumString
        {
            get
            {
                return GetSumString<AnimalItem>(x => x.Weight);
            }
        }


        public decimal? WeightInGramsSum
        {
            get
            {
                return GetSum<AnimalItem>(x => x.WeightInGrams);
            }
        }



        /// <summary>
        /// Weight in kilograms
        /// </summary>
        public decimal? WeightSum
        {
            get
            {
                return GetSum<AnimalItem>(x => x.Weight);
            }
        }


        public int? NumberSum
        {
            get
            {
                return GetSum<AnimalItem>(x => x.Number);
            }
        }


        public int? AgeReaderId
        {
            get { return _sl == null ? null : _sl.ageReadId; }
            set
            {
                if (_parent != null && !_parent.IsAssigningLookups)
                {
                    _sl.ageReadId = value;
                    IsDirty = true;
                }

                RaisePropertyChanged(() => AgeReaderId);
                RaisePropertyChanged(() => IsAgeReaderIdSet);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public bool IsAgeReaderIdSet
        {
            get { return AgeReaderId.HasValue; }
        }



        public string LengthMeasureUnit
        {
            get { return _strLengthMeasureUnit; }
            set
            {
                if (_parent != null && !_parent.IsAssigningLookups)
                {
                    _strLengthMeasureUnit = value;
                    IsDirty = true;
                }

                RaisePropertyChanged(() => LengthMeasureUnit);
                RaisePropertyChanged(() => HasUnsavedData);
                RaisePropertyChanged(() => IsLengthMeasureUnitSet);
            }
        }

        public int? LengthMeasureTypeId
        {
            get { return _strLengthMeasureTypeId; }
            set
            {
                if (_parent != null && !_parent.IsAssigningLookups)
                {
                    _strLengthMeasureTypeId = value;
                    IsDirty = true;
                }

                RaisePropertyChanged(() => LengthMeasureTypeId);
                RaisePropertyChanged(() => HasUnsavedData);
                RaisePropertyChanged(() => IsLengthMeasureTypeSet);
            }
        }


        public bool IsLengthMeasureUnitSet
        {
            get { return !string.IsNullOrEmpty(_strLengthMeasureUnit); }
        }


        public bool IsLengthMeasureTypeSet
        {
            get { return _strLengthMeasureTypeId.HasValue; }
        }


        public int? AgePlusGroup
        {
            get { return _sl == null ? null : _sl.agePlusGroup; }
            set
            {
                _sl.agePlusGroup = value;
                IsDirty = true;

                RaisePropertyChanged(() => AgePlusGroup);
            }
        }


        public List<L_Species> SpeciesLookups
        {
            get { return _lstSpeciesLookups; }
            set
            {
                _lstSpeciesLookups = value;
                RaisePropertyChanged(() => SpeciesLookups);
            }

        }


        public List<L_LengthMeasureType> LengthMeasureTypes
        {
            get { return _lstLengthMeasureType == null ? null : _lstLengthMeasureType.ToList(); }

            private set
            {
                _lstLengthMeasureType = value;
                RaisePropertyChanged(() => LengthMeasureTypes);
            }
        }


        #endregion
        /*
        private void AssignLengthMeasureUnit(string strValue)
        {
            if (Items == null)
                return;

            var lengthItems = Items.Where(x => x.Length.HasValue);

            System.Windows.MessageBoxResult res = System.Windows.MessageBoxResult.OK;

           //  if(strValue == null && lengthItems.Count() > 0)
            //     res = AppRegionManager.ShowMessageBox("En eller flere rækker i tabellen har en længde angivet, som er tilknyttet den nuværende valgte længdeenhed. Hvis du derfor vælger at fjerne længdeenheden (klikker OK), vil længderne i tabellen nulstilles.", System.Windows.MessageBoxButton.OKCancel);

             if (res == System.Windows.MessageBoxResult.Cancel)
                 return;

             if (_strLengthMeasureUnit == null && strValue != null && lengthItems.Count() > 0)
             {
                 foreach (var li in lengthItems)
                     li.Length = li.Length;
             }
           //     res = AppRegionManager.ShowMessageBox("En eller flere rækker i tabellen har en modenhed angivet, som er tilknyttet den nuværende valgte modenhedsmetode. Hvis du derfor vælger at fortsætte (klikker OK), vil de valgte modenheder i tabellen nulstilles og den nyvalgte modenhedsmetode vælges.", System.Windows.MessageBoxButton.OKCancel);

           // if (res == System.Windows.MessageBoxResult.Cancel)
           //     return;

             _strLengthMeasureUnit = strValue;
            IsDirty = true;

            //Reset currently selected maturities, since a new method has been selected.
            maturityItems.ToList().ForEach(x => x.MaturityId = null);

            RefreshMaturityItemsFromIndexMethod();
            RaisePropertyChanged(() => IsMaturityIndexMethodSet);
        }*/






        public ALavSFViewModel(SubSampleViewModel parent)
        {
            _parent = parent;
        }


        public virtual void Initialize(SubSample ss)
        {
            _subSample = ss;
            int? intSelectedItemId = SelectedItem != null ? SelectedItem.AnimalEntity.animalId : new Nullable<int>();

            try
            {
                var man = new DataRetrievalManager();
                var datMan = new DataInputManager();
                var manLookup = new LookupManager();

                var lv = new LookupDataVersioning();

                //Retreive species list view model which is used for storing ageReadId and age plus group number.
                _sl = datMan.GetEntity<SpeciesList>(ss.speciesListId);

                List<Animal> lstAnimals;
                List<Age> lstAges;
                List<AnimalFile> lstAnimalFiles;
                List<AnimalInfo> lstAnimalInfos;
                List<R_AnimalInfoReference> lstAnimalInfoReferences;

                //10-11-2017: For optimization purposes and in offline mode, reuse above cloned SubSample to get the various list below.
                if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                {
                    lstAnimals = ss.Animal.ToList();
                    lstAges = ss.Animal.SelectMany(x => x.Age).ToList();
                    lstAnimalFiles = ss.Animal.SelectMany(x => x.AnimalFiles).ToList();
                    lstAnimalInfos = ss.Animal.SelectMany(x => x.AnimalInfo).ToList();
                    lstAnimalInfoReferences = ss.Animal.SelectMany(x => x.AnimalInfo).SelectMany(x => x.R_AnimalInfoReference).ToList();
                }
                else
                {
                    lstAnimals = man.GetAnimals(ss.subSampleId);
                    lstAges = man.GetAges(ss.subSampleId) ?? new List<Age>();
                    lstAnimalFiles = man.GetAnimalFiles(ss.subSampleId) ?? new List<AnimalFile>();
                    lstAnimalInfos = man.GetAnimalInfos(ss.subSampleId) ?? new List<AnimalInfo>();
                    lstAnimalInfoReferences = man.GetAnimalInfoReferences(ss.subSampleId) ?? new List<R_AnimalInfoReference>();
                }

                var lstSpecies = manLookup.GetLookups(typeof(L_Species), lv).OfType<L_Species>().OrderBy(x => x.speciesCode).ToList();
                var lstLengthMeasureType = manLookup.GetLookups(typeof(L_LengthMeasureType), lv).OfType<L_LengthMeasureType>().OrderBy(x => x.num).ThenBy(x => x.UIDisplay).ToList();


                //Default order by how they were entered.
                lstAnimals = lstAnimals.OrderBy(x => Math.Abs(x.animalId)).ToList();
                var lstItems = lstAnimals.Select((x, i) => new AnimalItem(this, x, lstAges.Where(a => a.animalId == x.animalId).ToList(), lstAnimalFiles.Where(af => af.animalId == x.animalId).ToList(), lstAnimalInfos.Where(a => a.animalId == x.animalId).ToList(), lstAnimalInfoReferences) { Index = i + 1 }).ToList();

                if (this is SFViewModel)
                    lstItems = lstItems.OrderBy(x => x.IndividNum.HasValue ? x.IndividNum.Value : int.MaxValue).ToList();

                string strLengthMeasureUnit = lstItems.Where(x => x.AnimalEntity != null && !string.IsNullOrEmpty(x.AnimalEntity.lengthMeasureUnit)).Select(x => x.AnimalEntity.lengthMeasureUnit).FirstOrDefault();

                int? intLengthMeasureTypeId = lstItems.Where(x => x.AnimalEntity != null && x.AnimalEntity.lengthMeasureTypeId.HasValue).Select(x => x.AnimalEntity.lengthMeasureTypeId).FirstOrDefault();

                // if (string.IsNullOrEmpty(strLengthMeasureUnit))
                //     strLengthMeasureUnit = "MM";

                _strLastLengthMeasureUnit = strLengthMeasureUnit;
                _strLastLengthMeasureTypeId = intLengthMeasureTypeId;

                Initializing(lstItems);



                new Action(() =>
                {
                    _lstAllItems = lstItems;
                    SpeciesLookups = lstSpecies;
                    LengthMeasureTypes = lstLengthMeasureType;

                    LengthMeasureUnit = strLengthMeasureUnit;
                    LengthMeasureTypeId = intLengthMeasureTypeId;

                    AssignItems(intSelectedItemId);

                    SyncNewRows();

                    SetDefaultLengthMeasureType();

                    IsDirty = false;
                    RefreshAllNotifiableProperties();
                }).Dispatch();
            }
            catch (Exception e)
            {
                if (e is TimeoutException || e is EndpointNotFoundException)
                    DispatchMessageBox("Det var ikke muligt at oprette forbindelse til databasen.");
                else
                    DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }

        public void SetDefaultLengthMeasureType()
        {
            if ((_lstItems == null || _lstItems.All(x => x.IsNew)) && !IsLengthMeasureTypeSet && _sl != null && SpeciesLookups != null & SpeciesLookups.Count > 0)
            {
                var resSpecies = SpeciesLookups.Where(x => x.speciesCode == _sl.speciesCode).FirstOrDefault();
                if (resSpecies != null && resSpecies.standardLengthMeasureTypeId != null && LengthMeasureTypes != null && LengthMeasureTypes.Count > 0)
                {
                    var resLengthMeasureType = LengthMeasureTypes.Where(x => x.L_lengthMeasureTypeId == resSpecies.standardLengthMeasureTypeId).FirstOrDefault();
                    if (resLengthMeasureType != null)
                    {
                        _strLengthMeasureTypeId = resLengthMeasureType.L_lengthMeasureTypeId;
                        RaisePropertyChanged(() => LengthMeasureTypeId);
                    }
                }
            }

        }

        public void SetLengthMeasurementUnit()
        {
            if(_strLastLengthMeasureUnit != null && _strLastLengthMeasureUnit != null)
            {
                var res = _parent.LengthMeasureUnits.FirstOrDefault(x => x.lengthMeasureUnit == _strLastLengthMeasureUnit);
                if(res != null)
                {
                    _strLengthMeasureUnit = res.lengthMeasureUnit;
                    RaisePropertyChanged(() => LengthMeasureUnit);
                }

            }
        }

        protected virtual void Initializing(List<AnimalItem> lst)
        {

        }

        protected virtual void SelectedAnimalItemChanged(AnimalItem oldItem, AnimalItem newItem)
        {

        }


        private void AssignItems(int? intSelectedItemId)
        {
            if (Items == null)
                Items = new ObservableCollection<AnimalItem>();

            IEnumerable<AnimalItem> vLst = _lstAllItems.Where(x => x.AnimalEntity.ChangeTracker.State != ObjectState.Deleted);

            //Do not reassign the list directly, since this would reset any sortings. Clear the collection instead
            //and add the new items. This ensures that a new ICollectionView will not be re-created on the bound DataGrid.
            Items.Clear();
            Items.AddRange(vLst);
            // Items = vLst.ToObservableCollection();

            if (_strSortMemberPath != null)
                Sort(_strSortMemberPath, _sortDirection);

            if (intSelectedItemId.HasValue)
                SelectedItem = vLst.Where(x => x.AnimalEntity != null && x.AnimalEntity.animalId == intSelectedItemId.Value).FirstOrDefault();
        }


        public void SyncNewRows()
        {
            //Add new rows to the bottom of the grid so there will always be some.
            int intWantedNewRows = 4;
            int intExistingNewRowsCount = 0;
            if (_lstAllItems.Count > 0)
            {
                for (int i = _lstAllItems.Count - 1; i >= 0; i--)
                {
                    if (!_lstAllItems[i].HasUnsavedData && _lstAllItems[i].IsNew)
                        intExistingNewRowsCount++;
                    else
                        break;
                }
            }

            for (int i = intExistingNewRowsCount; i < intWantedNewRows; i++)
            {
                var itm = GetNewAnimalItem();

                _lstAllItems.Add(itm);
                Items.Add(itm);
            }

            RecheckAndAssignIndividNum();
        }


        protected virtual AnimalItem GetNewAnimalItem()
        {
            int intNewIndex = (_lstAllItems == null || _lstAllItems.Count == 0) ? 1 : _lstAllItems.Select(x => x.Index).DefaultIfEmpty().Max() + 1;

            var ai = AnimalItem.New(this, _subSample.subSampleId, intNewIndex);

            return ai;
        }



        protected string GetSumString<T>(Func<T, decimal?> getWeight)
            where T : AnimalItem
        {
            var dec = GetSum<T>(getWeight);
            return dec.HasValue ? dec.Value.ToString("0.####") : null;
        }

        protected decimal? GetSum<T>(Func<T, decimal?> getWeight)
            where T : AnimalItem
        {
            if (_lstItems == null || _lstItems.Count == 0)
                return null;

            var q = _lstItems.OfType<T>().Where(x => !(x.IsNew && !x.IsDirty) && getWeight(x).HasValue);

            var dec = (q.Count() == 0) ? new Nullable<decimal>() : q.Sum(x => getWeight(x).Value);
            return dec;
        }


        protected int? GetSum<T>(Func<T, int?> getValue)
            where T : AnimalItem
        {
            if (_lstItems == null || _lstItems.Count == 0)
                return null;

            var q = _lstItems.OfType<T>().Where(x => !(x.IsNew && !x.IsDirty) && getValue(x).HasValue);

            var dec = (q.Count() == 0) ? new Nullable<int>() : q.Sum(x => getValue(x).Value);
            return dec;
        }


        public void RefreshSums()
        {
            RaisePropertyChanged(() => WeightInGramsSum);
            RaisePropertyChanged(() => WeightInGramsSumString);
            RaisePropertyChanged(() => WeightSum);
            RaisePropertyChanged(() => WeightSumString);
            RaisePropertyChanged(() => NumberSum);        
            InternalRefreshSums();
        }

        protected virtual void InternalRefreshSums()
        {

        }


        public string GetRowErrors()
        {
            string strError = null;

            var items = Items.ToList();

            ValidateAllProperties();

            if (HasErrors)
                strError = Error;

            if (strError == null)
            {
                foreach (var itm in items)
                {
                    if (!itm.HasUnsavedData && !itm.ShouldForceValidate())
                        continue;

                    itm.ValidateAllProperties();

                    if (itm.HasErrors && strError == null)
                        strError = itm.Error;
                }
            }

            return strError;
        }



        public override void ValidateAllProperties(bool blnRefreshProperties = false, bool raisePropertyChangedOnProperties = true)
        {
            _strLastErrorProperty = _strLastErrorMsg = null;

            base.ValidateAllProperties(true, raisePropertyChangedOnProperties);
        }


        /// <summary>
        /// Hide base method ValidateField for any inherited classes of this class (they should override ValidateListItem instead).
        /// </summary>
        protected override string ValidateField(string strFieldName)
        {
            string strError = null;

            if (_blnValidate && IsPropertyUsed(strFieldName))
            {
                switch (strFieldName)
                {
                    case "LengthMeasureTypeId":
                        if (Items != null && !LengthMeasureTypeId.HasValue && Items.Where(x => x.Length.HasValue).Count() > 0)
                            strError = "Du har angivet en eller flere længder i tabellen, men mangler Længdemålingstypen de er angivet under.";

                        break;

                    case "LengthMeasureUnit":
                        if (Items != null && string.IsNullOrEmpty(LengthMeasureUnit) && Items.Where(x => x.Length.HasValue).Count() > 0)
                            strError = "Du har angivet en eller flere længder i tabellen, men mangler længdeenheden de er angivet under.";

                        break;

                    case "AgePlusGroup":
                        if (AgePlusGroup < 0)
                            strError = "Aldersgruppe skal være et positivt heltal.";

                        break;

                    default:
                        strError = ValidateInheritedObject(strFieldName);
                        break;
                }

                //Save the property name of the property raising the error
                if (strError != null)
                {
                    _strLastErrorProperty = strFieldName;
                    _strLastErrorMsg = strError;
                }
            }
            //If this is a normal error check, return the same error as found during the manual error check if the
            //property being checked is the same as the property checked during the manual error checking.
            else if (_strLastErrorProperty == strFieldName)
            {
                return _strLastErrorMsg;
            }

            return strError;
        }


        protected virtual bool IsPropertyUsed(string strProperty)
        {
            return true;
        }


        protected virtual string ValidateInheritedObject(string strPropertyName)
        {
            return null;
        }




        public List<LavSFTransferItem> GetUnsavedData(ref SpeciesList sl)
        {
             sl = _sl;

            List<LavSFTransferItem> lst = new List<LavSFTransferItem>();

            //If lav,rep, delete animals where number = 0
            if (_enmSubSampleType == SubSampleType.LAVRep)
            {
                foreach (var ai in _lstAllItems.ToList())
                {
                    if (!(ai.IsNew && !ai.IsDirty) && ai.Number == 0)
                    {
                        base.Dispatcher.Invoke(new Action(() =>
                        {
                            RemoveItem(ai, false);
                        }));
                    }
                }
            }


            foreach (var ai in _lstAllItems.Where(x => x.Length.HasValue || x.Weight.HasValue).ToList())
            {
                if (ai.AnimalEntity.lengthMeasureUnit != LengthMeasureUnit && !(ai.IsNew && !ai.IsDirty))
                {
                    ai.AnimalEntity.lengthMeasureUnit = LengthMeasureUnit;
                    ai.IsDirty = true;
                }

                if (ai.AnimalEntity.lengthMeasureTypeId != LengthMeasureTypeId && !(ai.IsNew && !ai.IsDirty))
                {
                    ai.AnimalEntity.lengthMeasureTypeId = LengthMeasureTypeId;
                    ai.IsDirty = true;
                }


            }

            foreach (var ai in _lstAllItems)
            {
                var lavSFItem = ai.GetUnsavedData();

                if (lavSFItem != null)
                {
                    lavSFItem.SubSampleId = _subSample.subSampleId;
                    lst.Add(lavSFItem);
                }
            }

            return lst;
        }



        #region Sorting Methods

        public void Sort(string strSortMemberPath, ListSortDirection? direction)
        {
            _strSortMemberPath = strSortMemberPath;
            _sortDirection = direction;

            ObservableCollection<AnimalItem> source = _lstItems;

            if (_lstItems == null)
                return;

            for (int i = source.Count - 1; i >= 0; i--)
            {
                var row = source[i];
                //Ignore new empty rows
                if (row.IsNew && !row.HasUnsavedData)
                    continue;

                for (int j = 1; j <= i; j++)
                {
                    var row1 = source[j - 1];
                    var row2 = source[j];

                    var value1 = GetRowValue(strSortMemberPath, row1) as IComparable;
                    var value2 = GetRowValue(strSortMemberPath, row2) as IComparable;

                    int sortResult = 0;

                    if (value2 != null)
                        sortResult = ((value1 == null) ? 1 : value1.CompareTo(value2));

                    sortResult = direction == ListSortDirection.Ascending ? sortResult * 1 : sortResult * -1;

                    if (sortResult > 0)
                    {
                        // Position the item correctly
                        source.Move(j - 1, j);
                    }
                }
            }
        }

        private object GetRowValue(string strProperty, object obj)
        {
            var prop = obj.GetType().GetProperty(strProperty, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (prop != null)
                return prop.GetValue(obj, new object[] { });
            else
                return null;
        }

        #endregion


        #region New Row Command


        public DelegateCommand AddRowCommand
        {
            get { return _cmdNewRow ?? (_cmdNewRow = new DelegateCommand(() => NewRow())); }
        }


        protected void NewRow()
        {
            AnimalItem sli = GetNewAnimalItem();

            if (sli != null)
            {
                _lstAllItems.Add(sli);
                Items.Add(sli);

                // if(blnAssignItems)
                //     AssignItems();
            }

            RecheckAndAssignIndividNum();
        }


        #endregion


        #region Remove Command


        public DelegateCommand<AnimalItem> RemoveCommand
        {
            get
            {
                if (_cmdRemoveRow == null)
                    _cmdRemoveRow = new DelegateCommand<AnimalItem>(param => RemoveItem(param));

                return _cmdRemoveRow;
            }
        }


        private void RemoveItem(AnimalItem item, bool blnShowWarning = true)
        {
            if (item == null)
                return;

            SelectedItem = null;

            //Show confirmation message if lookup to remove is an existing db value.
            if (item.AnimalEntity.ChangeTracker.State != ObjectState.Added)
            {
                if (blnShowWarning)
                {
                    if(!User.HasDeleteAnimalsTask)
                    {
                        DispatchMessageBox("Du har ikke rettigheder til at slette enkeltfisk som er gemt i databasen. Kontakt venligst en administrator for at slette rækken.");
                        return;
                    }

                    var res = AppRegionManager.ShowMessageBox("Er du sikker på du vil fjerne den valgte række", System.Windows.MessageBoxButton.YesNo);

                    if (res == System.Windows.MessageBoxResult.No)
                        return;
                }

                item.MarkAsDeleted();
                Items.Remove(item);
                IsDirty = true;
            }
            else
            {
                _lstAllItems.Remove(item);
                Items.Remove(item);

                IsDirty = true;
            }

            RecheckAndAssignIndividNum();
            SelectedItem = null;
        }

        #endregion


        private void RecheckAndAssignIndividNum()
        {
            if (LavSFType != "SF")
                return;

            List<AnimalItem> lst = new List<AnimalItem>();
            foreach (var i in Items.ToList())
            {
                if(i.IsRowEmptySF)
                    lst.Add(i);
                else
                    lst.Clear();
            }
         //   var lst = Items.Where(x => x.IsRowEmptySF).ToList();

            int intIndividNum = (_lstAllItems == null || _lstAllItems.Count == 0) ? 1 : _lstAllItems.Where(x => x.AnimalEntity.ChangeTracker.State != ObjectState.Deleted && !x.IsRowEmptySF).Select(x => x.IndividNum.HasValue ? x.IndividNum.Value : 0).DefaultIfEmpty().Max();


            for (int i = 0; i < lst.Count; i++)
            {
                intIndividNum = intIndividNum + 1;

                if (!lst[i].IndividNum.Equals(intIndividNum))
                    lst[i].IndividNum = intIndividNum;
            }

            /*var lst = Items.ToList();

            for (int i = 0; i < Items.Count; i++)
            {
                int intIndividNum = i + 1;

                if (!Items[i].IndividNum.Equals(intIndividNum))
                    Items[i].IndividNum = intIndividNum;
            }*/
        }



        #region Refresh Otolith Images Command


        public DelegateCommand RefreshOtolithImagesCommand
        {
            get { return _cmdRefreshOtolithImages ?? (_cmdRefreshOtolithImages = new DelegateCommand(() => RefreshOtolithImages())); }
        }


        protected void RefreshOtolithImages()
        {
            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
            {
                AppRegionManager.ShowMessageBox("Programmet skal være online før det er muligt at søge efter otolith-billeder.");
                return;
            }

            if (RunFileSynchronizerViewModel.Instance.IsLoading)
            {
                AppRegionManager.ShowMessageBox("Der søges allerede efter billeder, vent venligst til den forrige søgning er færdig.");
                return;
            }

            if (HasUnsavedData)
            {
                AppRegionManager.ShowMessageBox("Gem venligst ændringer til tabellen inden du søger efter otolith-billeder.");
                return;
            }

            var t = RunFileSynchronizerViewModel.Instance.RunSynchronizerAsync();

            if (t != null)
            {
                AppRegionManager.ShowLoadingWindow("Søger efter billeder, vent venligst...");
                t.ContinueWith(th => new Action(() =>
                {
                    AppRegionManager.HideLoadingWindow();

                    _parent.ReloadSubSampleViewModel();
                }).Dispatch());
            }
        }


        #endregion

    }
}
