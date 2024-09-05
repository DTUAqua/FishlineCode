using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class R_TargetSpecies : OfflineEntity
    {


        public override bool Equals(object obj)
        {
            R_TargetSpecies other = obj as R_TargetSpecies;

            if (other == null)
                return false;

            return this.TargetSpeciesId == other.TargetSpeciesId &&
                this.speciesCode == null ? other.speciesCode == null : this.speciesCode.Equals(other.speciesCode) &&
                this._sampleId == other.sampleId
                ;
        }
    }
}
