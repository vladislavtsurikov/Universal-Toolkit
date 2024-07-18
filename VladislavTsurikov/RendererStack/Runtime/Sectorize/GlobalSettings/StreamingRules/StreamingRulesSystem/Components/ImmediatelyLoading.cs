﻿using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    [PersistentComponent]
    [Name("Immediately Loading")]
    public class ImmediatelyLoading : StreamingRule
    {
        public float MaxDistance = 2000;
        
        public override bool ShowActiveToggle()
        {
            return false;
        }
    }
}