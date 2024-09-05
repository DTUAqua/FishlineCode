using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Anchor.Core.Comparers;
using System.Windows.Input;

namespace Babelfisk.ViewModels.Input
{
    public class SFViewModel : ALavSFViewModel
    {
        private string _strMaturityIndexMethod;

        private int? _intHatchMonthId;

        private int? _intMaturityReaderId;

        private List<Maturity> _lstMaturity;


        #region Properties


        public override string LavSFType
        {
            get { return "SF"; }
        }


        public string MaturityIndexMethod
        {
            get { return _strMaturityIndexMethod; }
            set
            {
                if (_parent != null && !_parent.IsAssigningLookups)
                    AssignMaturityIndexMethod(value);

                RaisePropertyChanged(() => MaturityIndexMethod);
                RaisePropertyChanged(() => HasUnsavedData);
                RaisePropertyChanged(() => IsMaturityIndexMethodSet);

            }
        }


        public bool IsMaturityIndexMethodSet
        {
            get { return !string.IsNullOrEmpty(MaturityIndexMethod); }
        }


        public List<Maturity> MaturityList
        {
            get { return _lstMaturity == null ? null : _lstMaturity.ToList(); }
            private set
            {
                _lstMaturity = value;
                RaisePropertyChanged(() => MaturityList);
            }
        }


        public int? HatchMonthReaderId
        {
            get { return _sl == null ? null : _sl.hatchMonthReaderId; }
            set
            {
                if (_parent != null && !_parent.IsAssigningLookups)
                {
                    _sl.hatchMonthReaderId = value;
                    IsDirty = true;
                }


                RaisePropertyChanged(() => HatchMonthReaderId);
                RaisePropertyChanged(() => IsHatchMonthReaderSet);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public bool IsHatchMonthReaderSet
        {
            get { return HatchMonthReaderId.HasValue; }
        }


        public int? MaturityReaderId
        {
            get { return _sl == null ? null : _sl.maturityReaderId; }
            set
            {
                if (_parent != null && !_parent.IsAssigningLookups)
                {
                    _sl.maturityReaderId = value;
                    IsDirty = true;
                }

                RaisePropertyChanged(() => MaturityReaderId);
                RaisePropertyChanged(() => IsMaturityReaderIdSet);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        public bool IsMaturityReaderIdSet
        {
            get { return MaturityReaderId.HasValue; }
        }


        public string WeightGuttedSumString
        {
            get
            {
                return GetSumString<AnimalItem>(x => x.WeightGutted);
            }
        }


        public string WeightGonadsSumString
        {
            get
            {
                return GetSumString<AnimalItem>(x => x.WeightGonads);
            }
        }


        public string WeightLiverSumString
        {
            get
            {
                return GetSumString<AnimalItem>(x => x.WeightLiver);
            }
        }


        public SubSampleType SubSampleType
        {
            get { return _enmSubSampleType; }
        }

        #endregion




        public SFViewModel(SubSampleViewModel parent, SubSampleType enmSubSampleType)
            : base(parent)
        {
            _enmSubSampleType = enmSubSampleType;
            RegisterToKeyDown();
        }


        protected override void Initializing(List<AnimalItem> lst)
        {
            var lstAnimalInfos = lst.Where(x => x.AnimalInfoEntity != null);

            var animalInfoMaturity = lstAnimalInfos.Where(x => x.AnimalInfoEntity.maturityId.HasValue).Select(x => x.AnimalInfoEntity).FirstOrDefault();

            //Load maturity index method from a "random" maturityId (if any exists). All maturityIds should be the same.
            if (animalInfoMaturity != null && _parent.MaturityList != null)
            {
                Maturity m = null;
                if ((m = _parent.MaturityList.Where(x => x.maturityId == animalInfoMaturity.maturityId).FirstOrDefault()) != null)
                {
                    _strMaturityIndexMethod = m.maturityIndexMethod;
                    RefreshMaturityItemsFromIndexMethod();
                }
            }
        }


        private bool AssignMaturityIndexMethod(string strValue)
        {
            if (Items == null)
                return false;

            var maturityItems = Items.Where(x => x.MaturityId.HasValue);

            System.Windows.MessageBoxResult res = System.Windows.MessageBoxResult.OK;

            if (_strMaturityIndexMethod != strValue && maturityItems.Count() > 0)
                res = AppRegionManager.ShowMessageBox("En eller flere rækker i tabellen har en modenhed angivet, som er tilknyttet det tidligere valgte modenhedsindeks. Hvis du derfor vælger at fortsætte (klikker OK), vil de valgte modenheder i tabellen nulstilles og det nyvalgte modenhedsindeks benyttes fremover.", System.Windows.MessageBoxButton.OKCancel);

            if (res == System.Windows.MessageBoxResult.Cancel)
                return false;

            _strMaturityIndexMethod = strValue;
            IsDirty = true;

            //Reset currently selected maturities, since a new method has been selected.
            maturityItems.ToList().ForEach(x => x.MaturityId = null);

            RefreshMaturityItemsFromIndexMethod();
            RaisePropertyChanged(() => IsMaturityIndexMethodSet);

            return true;
        }


        private void RefreshMaturityItemsFromIndexMethod()
        {
            if(_parent != null && _parent.MaturityList != null)
            {
                new Action(() =>
                {
                    MaturityList = _parent.MaturityList.Where(x => x.maturityIndexMethod.Equals(_strMaturityIndexMethod, StringComparison.InvariantCultureIgnoreCase)).OrderBy(x => x.UIDisplay,  new StringNumberComparer()).ToList();
                }).Dispatch();
            }
        }


        protected override AnimalItem GetNewAnimalItem()
        {
            var ai = base.GetNewAnimalItem();

            //Single fish always only have 1 in number.
            ai.Number = 1;
            ai.IsDirty = false;
            return ai;
        }


        protected override void InternalRefreshSums()
        {
            RaisePropertyChanged(() => WeightGuttedSumString);
            RaisePropertyChanged(() => WeightGonadsSumString);
            RaisePropertyChanged(() => WeightLiverSumString);
        }


        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key == System.Windows.Input.Key.N && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.N || e.Key == Key.N) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
            {
                e.Handled = true;
                ScrollTo("NewItem");
            }
        }


        protected override string ValidateInheritedObject(string strPropertyName)
        {
            return base.ValidateInheritedObject(strPropertyName);
        }

    }
}
