using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;

namespace Rimball
{
    class RimballMod : Mod
    {
        public static RimballSettings settings;
        private Vector2 scrollPosition = Vector2.zero;

        public RimballMod(ModContentPack content) : base(content)
        {
            LongEventHandler.ExecuteWhenFinished(GetSettings);
            settings = GetSettings<RimballSettings>();
        }

        public void GetSettings()
        {
            GetSettings<RimballSettings>();
        }

        public static IEnumerable<ThingDef> PossibleThingDefs()
        {
            return from d in DefDatabase<ThingDef>.AllDefs
                   where (d.category == ThingCategory.Item && d.scatterableOnMapGen && !d.destroyOnDrop && !d.MadeFromStuff)
                   select d;
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            settings.DoWindowContents(rect);
        }

        public override string SettingsCategory()
        {
            return "Rim Ball";
        }
    }
}
