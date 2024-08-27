using System.Collections.Generic;
using Verse;

namespace Rimball
{
    class CompProperties_SportPitch : CompProperties
    {

        public List<IntVec2> sideACoords = new List<IntVec2>();
        public List<IntVec2> sideBCoords = new List<IntVec2>();
        public string soundDefName;
        public List<TraitEntry> potentialTraits = new List<TraitEntry>();

        public CompProperties_SportPitch()
            {            
                this.compClass = typeof(Comp_SportPitch);
            }
    }
}
