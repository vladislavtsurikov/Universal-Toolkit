using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public abstract class Scatter : ComponentStack.Runtime.Component
    {
	    protected ScatterStack ScatterStack => (ScatterStack)Stack;
	    
	    public abstract IEnumerator Samples(BoxArea boxArea, List<Vector2> samples, Action<Vector2> onSpawn = null);
    }
}