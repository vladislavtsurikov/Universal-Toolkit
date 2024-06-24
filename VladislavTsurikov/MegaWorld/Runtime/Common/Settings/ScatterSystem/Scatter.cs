using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public abstract class Scatter : Component
    {
	    protected ScatterStack ScatterStack => (ScatterStack)Stack;
	    
	    public abstract IEnumerator Samples(BoxArea boxArea, List<Vector2> samples, Action<Vector2> onSpawn = null);
    }
}