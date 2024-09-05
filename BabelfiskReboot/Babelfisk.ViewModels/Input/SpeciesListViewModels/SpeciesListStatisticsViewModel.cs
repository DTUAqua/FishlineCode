using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;

namespace Babelfisk.ViewModels.Input
{
    public class SpeciesListStatisticsViewModel : AViewModel
    {
        private SpeciesListItem _speciesListItem;

        private string _dRepLAVWeight;
        private int? _iRepLAVNum;
        private string _dRepSFWeight;
        private int?_iRepSFNum;
        private string _dNotRepSFWeight;
        private int? _iNotRepSFNum;

        private int _intRepLAVCount, _intRepSFCount, _intNotRepSFCount;

        private string _strRepLAVUnit, _strRepSFUnit, _strNotRepSFUnit;

        #region Properties


        public string RepLAVWeight
        {
            get { return _dRepLAVWeight; }
            set
            {
                _dRepLAVWeight = value;
                RaisePropertyChanged(() => RepLAVWeight);
            }
        }

        public int? RepLAVNum
        {
            get { return _iRepLAVNum; }
            set
            {
                _iRepLAVNum = value;
                RaisePropertyChanged(() => RepLAVNum);
            }
        }


        public string RepSFWeight
        {
            get { return _dRepSFWeight; }
            set
            {
                _dRepSFWeight = value;
                RaisePropertyChanged(() => RepSFWeight);
            }
        }

        public int? RepSFNum
        {
            get { return _iRepSFNum; }
            set
            {
                _iRepSFNum = value;
                RaisePropertyChanged(() => RepSFNum);
            }
        }


        public string NotRepSFWeight
        {
            get { return _dNotRepSFWeight; }
            set
            {
                _dNotRepSFWeight = value;
                RaisePropertyChanged(() => NotRepSFWeight);
            }
        }

        public int? NotRepSFNum
        {
            get { return _iNotRepSFNum; }
            set
            {
                _iNotRepSFNum = value;
                RaisePropertyChanged(() => NotRepSFNum);
            }
        }



        public string RepLAVUnit
        {
            get { return _strRepLAVUnit; }
            set
            {
                _strRepLAVUnit = value;
                RaisePropertyChanged(() => RepLAVUnit);
            }
        }


        public string RepSFUnit
        {
            get { return _strRepSFUnit; }
            set
            {
                _strRepSFUnit = value;
                RaisePropertyChanged(() => RepSFUnit);
            }
        }


        public string NotRepSFUnit
        {
            get { return _strNotRepSFUnit; }
            set
            {
                _strNotRepSFUnit = value;
                RaisePropertyChanged(() => NotRepSFUnit);
            }
        }


        public int RepLavCount
        {
            get { return _intRepLAVCount; }
            set
            {
                _intRepLAVCount = value;
                RaisePropertyChanged(() => RepLavCount);
                RaisePropertyChanged(() => IsLavRepEnabled);
                RaisePropertyChanged(() => IsSFRepEnabled);
                RaisePropertyChanged(() => IsSFNotRepEnabled);
            }
        }


        public int RepSFCount
        {
            get { return _intRepSFCount; }
            set
            {
                _intRepSFCount = value;
                RaisePropertyChanged(() => RepSFCount);
                RaisePropertyChanged(() => IsLavRepEnabled);
                RaisePropertyChanged(() => IsSFRepEnabled);
                RaisePropertyChanged(() => IsSFNotRepEnabled);
            }
        }


        public int NotRepSFCount
        {
            get { return _intNotRepSFCount; }
            set
            {
                _intNotRepSFCount = value;
                RaisePropertyChanged(() => NotRepSFCount);
                RaisePropertyChanged(() => IsLavRepEnabled);
                RaisePropertyChanged(() => IsSFRepEnabled);
                RaisePropertyChanged(() => IsSFNotRepEnabled);
            }
        }


        public bool IsLavRepEnabled
        {
            get { return RepSFCount == 0 || RepLavCount > 0; }
        }

        public bool IsSFRepEnabled
        {
            get { return (RepLavCount == 0 && NotRepSFCount == 0) || RepSFCount > 0; }
        }

        public bool IsSFNotRepEnabled
        {
            get { return (RepSFCount == 0) || NotRepSFCount > 0; }
        }


        #endregion



        public SpeciesListStatisticsViewModel(SpeciesListItem sli)
        {
            _speciesListItem = sli;

            InitializeAsync();
        }


        public void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        public void Initialize()
        {
            var sl = _speciesListItem.SpeciesListEntity;

            if (sl == null)
                return;

            try
            {
                SubSample ssRep = sl.SubSample.Where(s => s.IsRepresentative && (s.subSampleWeight.HasValue || s.landingWeight.HasValue || s.sumAnimalWeights.HasValue)).OrderByDescending(x => x.stepNum).FirstOrDefault();

                //If the rep has no animals, but another representative subsample does, use the one that does. This is because only one subsample should have animal, and that's the one
                //with the highest stepNum. However, if the user is editing the weights, adding some to a stepNum or removing a weight from a stepNum, this is then first updated when saving
                //the specieslist and reloading it. Therefore the statistics can be out of sync until saving, which is why below case is needed.
                try
                {
                    if (ssRep != null && ssRep.Animal.Count <= 0)
                    {
                        var newssRep = sl.SubSample.Where(s => s.IsRepresentative && s.Animal.Count > 0).OrderByDescending(x => x.stepNum).FirstOrDefault();
                        if (newssRep != null)
                            ssRep = newssRep;
                    }
                }
                catch { }

                IEnumerable<Animal> aniRep = (ssRep == null || ssRep.Animal == null) ? (IEnumerable<Animal>)new List<Animal>() : ssRep.Animal;
                var aniNotRep = sl.SubSample.Where(s => !s.IsRepresentative && (s.subSampleWeight.HasValue || s.sumAnimalWeights.HasValue)).SelectMany(a => a.Animal);

                //SubSample.representative is "ja" and Animal.individNum is null or 0
                var aniRepLAV = aniRep.Where(a => !a.HasIndividNum);

                //SubSample.representative is "ja" and Animal.individNum > 0
                var aniRepSF = aniRep.Where(a => a.HasIndividNum);

                //SubSample.representative is "nej" and Animal.individNum > 0
                var aniNotRepSF = aniNotRep.Where(a => a.HasIndividNum);

                decimal? dRepLAVWeight = aniRepLAV.Where(a => a.weight.HasValue).Sum(a => a.weight.Value);
                int? iRepLAVNum = aniRepLAV.Sum(a => a.number);
                decimal? dRepSFWeight = aniRepSF.Where(a => a.weight.HasValue).Sum(a => a.weight.Value);
                int? iRepSFNum = aniRepSF.Sum(a => a.number);
                decimal? dNotRepSFWeight = aniNotRepSF.Where(a => a.weight.HasValue).Sum(a => a.weight.Value);
                int? iNotRepSFNum = aniNotRepSF.Sum(a => a.number);

                string strRepLAVUnit = aniRepLAV.Where(a => !string.IsNullOrEmpty(a.lengthMeasureUnit)).Select(a => a.lengthMeasureUnit).FirstOrDefault();
                string strRepSFUnit = aniRepSF.Where(a => !string.IsNullOrEmpty(a.lengthMeasureUnit)).Select(a => a.lengthMeasureUnit).FirstOrDefault();
                string strNotRepSFUnit = aniNotRepSF.Where(a => !string.IsNullOrEmpty(a.lengthMeasureUnit)).Select(a => a.lengthMeasureUnit).FirstOrDefault();

                int intRepLAVCount = aniRepLAV.Count();
                int intRepSFCount = aniRepSF.Count();
                int intNotRepSFCount = aniNotRepSF.Count();

                new Action(() =>
                {
                    RepLAVWeight = DecimalToString(dRepLAVWeight);
                    RepLAVNum = iRepLAVNum;
                    RepSFWeight = DecimalToString(dRepSFWeight);
                    RepSFNum = iRepSFNum;
                    NotRepSFWeight = DecimalToString(dNotRepSFWeight);
                    NotRepSFNum = iNotRepSFNum;

                    RepLAVUnit = strRepLAVUnit;
                    RepSFUnit = strRepSFUnit;
                    NotRepSFUnit = strNotRepSFUnit;

                    RepLavCount = intRepLAVCount;
                    RepSFCount = intRepSFCount;
                    NotRepSFCount = intNotRepSFCount;
                }).Dispatch();
            }
            catch (Exception e)
            {
            }
        }


        private string DecimalToString(decimal? dec)
        {
            if (!dec.HasValue)
                return "0";

            return dec.Value.ToString("0.###");
        }

    }
}
