using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;

namespace Babelfisk.ViewModels.TreeView
{
    public class SpeciesListTreeItemViewModel : TreeItemViewModel
    {
        public override string Header
        {
            get { return "Artsliste"; }
        }


        public Sample SampleEntity
        {
            get { return _item as Sample; }
            set
            {
                _item = value;
                RaisePropertyChanged(() => SampleEntity);
            }
        }


        public SpeciesListTreeItemViewModel(TreeItemViewModel parent, Sample sample) 
            : base(parent, false/*sample != null*/) //Not sure about them sample != null is the right thing here.
        {
            SampleEntity = sample;
        }


        protected override void LoadChildren()
        {
           /* var datRes = new BusinessLogic.DataRetrievalManager();

            var lstSubSamples = datRes.GetTreeViewSubSample(_sample.sampleId);
        //    var lstSpeciesLists = datRes.GetTreeViewSpeciesList(_sample.sampleId);

            var vChildren = ParseLawSingleFish(lstSubSamples);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                base.Children = vChildren.ToObservableCollection();
            }));*/
        }


        internal override void RefreshTree(object entity)
        {
            Sample s = entity as Sample;

            if (s != null)
            {
                SampleEntity = s;
            }
        }


        /// <summary>
        /// Load station
        /// </summary>
        public bool LoadSpeciesListView()
        {
            var vm = new ViewModels.Input.SpeciesListViewModel(SampleEntity.sampleId);
            return AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);
        }

        private List<TreeItemViewModel> ParseLawSingleFish(List<SubSample> lstSubSample)
        {
            int intLawRepSom = 0, intLawNotRepSom = 0;
            double dblLawRepSow = 0, dblLawNotRepSow = 0;
            string strLAWRepLengthUnit = "", strLAWNotRepLengthUnit = "";
            bool blnLAWRep = false, blnLAWNotRep = false;

            int intSingleFishRepSom = 0, intSingleFishNotRepSom = 0;
            double dblSinglefishRepSow = 0, dblSinglefishNotRepSow = 0;
            string strSFRepLengthUnit = "", strSFNotRepLengthUnit = "";
            bool blnSingleFishRep = false, blnSingleFishNotRep = false;


            //This is basically created the same way as Niels has done it in clsSpeciesList.SetSpeciesListSommeryGroupBox (with a few optimizations).
            //It should therefore be revised and probably rewritten a bit (optimizations).
            //Also the code below calculates the different things using all subsamples and species lists, Niels' code only does
            //the calculation for a selected species list record (from a grid). This does that the treeview changes every time
            //a the selection of species list record changes (in the grid).

            foreach (var subsample in lstSubSample)
            {
                bool blnHasIndividNum = subsample.Animal.Where(x => x.individNum.HasValue && x.individNum.Value != 0).Count() > 0;
                bool blnRepresentatives = !string.IsNullOrEmpty(subsample.representative) && subsample.representative.Equals("ja", StringComparison.InvariantCultureIgnoreCase);

                //Checking for LAW, generate LAW & SON, SOW and length unit values for rep and not-rep.
                if (!blnHasIndividNum)
                {
                    int intLawSom = subsample.Animal.Sum(x => x.number);
                    double dblLawSow = 0;
                    string strLAWLengthUnit = "";

                    if (intLawSom > 0)
                    {
                        dblLawSow = subsample.Animal.Where(x => x.weight.HasValue).Sum(x => Convert.ToDouble(x.weight.Value));

                        var ani = subsample.Animal.FirstOrDefault();
                        if (ani != null)
                            strLAWLengthUnit = ani.lengthMeasureUnit;
                    }

                    if (blnRepresentatives)
                    {
                        intLawRepSom += intLawSom;
                        dblLawRepSow += dblLawSow;
                        strLAWRepLengthUnit = strLAWLengthUnit;
                        blnLAWRep = true;
                    }
                    else
                    {
                        intLawNotRepSom += intLawSom;
                        dblLawNotRepSow += dblLawSow;
                        strLAWNotRepLengthUnit = strLAWLengthUnit;
                        blnLAWNotRep = true;
                    }
                }


                if (blnRepresentatives)
                {
                    bool blnIsSingleFishRecord = true;

                    if (subsample.Animal.Count > 0)
                    {
                        //Only a signle fish record if all animal records do not have individNum or individNum is equal to 0.
                        blnIsSingleFishRecord = subsample.Animal.Where(x => x.individNum.HasValue && x.individNum.Value != 0).Count() == subsample.Animal.Count;

                        if (blnIsSingleFishRecord)
                        {
                            intSingleFishRepSom += subsample.Animal.Count;

                            if (intSingleFishRepSom > 0)
                            {
                                dblSinglefishRepSow += subsample.Animal.Where(x => x.weight.HasValue).Sum(x => Convert.ToDouble(x.weight.Value));

                                var ani = subsample.Animal.FirstOrDefault();
                                if (ani != null)
                                    strSFRepLengthUnit = ani.lengthMeasureUnit;
                            }
                        }
                    }

                    //if law-rep have values, deactivate sf-rep
                    if (intLawRepSom > 0)
                        blnIsSingleFishRecord = false;

                    if (blnIsSingleFishRecord)
                    {
                        blnSingleFishRep = true;

                        if (intSingleFishRepSom > 0)
                            blnLAWRep = false;
                    }
                }
                else
                {
                    if (!blnLAWNotRep)
                    {
                        if (subsample.Animal.Count > 0)
                        {
                            intSingleFishNotRepSom += subsample.Animal.Where(x => x.individNum.HasValue && x.individNum.Value != 0).Count();

                            if (intSingleFishNotRepSom > 0)
                            {
                                dblSinglefishNotRepSow += subsample.Animal.Where(x => x.weight.HasValue).Sum(x => Convert.ToDouble(x.weight.Value));

                                var ani = subsample.Animal.FirstOrDefault();
                                if (ani != null)
                                    strSFNotRepLengthUnit = ani.lengthMeasureUnit;
                            }

                            if (intSingleFishNotRepSom > 0 && dblSinglefishNotRepSow > 0)
                                blnLAWNotRep = false;
                        }

                        blnSingleFishNotRep = true;
                    }
                    else
                    {
                        if (intLawNotRepSom == 0 && dblLawNotRepSow == 0.0)
                            blnSingleFishNotRep = true;
                    }
                }
            }


            if (dblLawRepSow > 0) { dblLawRepSow = Math.Round((dblLawRepSow), 3); }
            if (dblLawNotRepSow > 0) { dblLawNotRepSow = Math.Round((dblLawNotRepSow), 3); }
            if (dblSinglefishRepSow > 0) { dblSinglefishRepSow = Math.Round((dblSinglefishRepSow), 3); }
            if (dblSinglefishNotRepSow > 0) { dblSinglefishNotRepSow = Math.Round((dblSinglefishNotRepSow), 3); }


            List<TreeItemViewModel> lst = new List<TreeItemViewModel>();

            if (blnLAWRep)
                lst.Add(new AnimalTreeItemViewModel(this, "LAV, Rep"));

            if (blnLAWNotRep)
                lst.Add(new AnimalTreeItemViewModel(this, "LAV, Not Rep"));

            if (blnSingleFishRep)
                lst.Add(new AnimalTreeItemViewModel(this, "EF, Rep"));

            if (blnSingleFishNotRep)
                lst.Add(new AnimalTreeItemViewModel(this, "EF, Not Rep"));

            return lst;
        }
    }
}
