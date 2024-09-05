using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;

namespace Babelfisk.ViewModels.Input
{
    public class SpeciesListREKItem : SpeciesListItem
    {

        #region Properties


        public string ApplicationCode
        {
            get { return _speciesListEntity != null && _speciesListEntity.applicationId.HasValue && _speciesListVM.ApplicationsDic.ContainsKey(_speciesListEntity.applicationId.Value) ? _speciesListVM.ApplicationsDic[_speciesListEntity.applicationId.Value].code : null; }
            set
            {
                L_Application tmp = null;
                if (value != null && (tmp = _speciesListVM.ApplicationsDic.Select(x => x.Value).Where(x => x.code.Equals(value)).FirstOrDefault()) != null)
                    _speciesListEntity.applicationId = tmp.L_applicationId;
                else
                    _speciesListEntity.applicationId = null;
                IsDirty = true;
                RaisePropertyChanged(() => ApplicationCode);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        #endregion


        public SpeciesListREKItem(SpeciesListViewModel slvm, SpeciesList sl = null)
            : base(slvm, sl)
        {

        }


        public override void BeforeSave()
        {
        }

    }
}
