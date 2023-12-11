using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public abstract class Scatter : ComponentStack.Runtime.Component
    {
	    private Stopwatch _stopwatch;
	    private ScatterStack ScatterStack => (ScatterStack)Stack;
	    
	    public abstract IEnumerator Samples(BoxArea boxArea, List<Vector2> samples, Action<Vector2> onSpawn = null);

	    protected bool IsWaitForNextFrame()
	    {
		    if (!ScatterStack.WaitForNextFrame)
		    {
			    return false;
		    }

		    if (_stopwatch == null)
		    {
			    _stopwatch = new Stopwatch();
			    _stopwatch.Start();
			    return false;
		    }
		    
		    if (_stopwatch.ElapsedMilliseconds >= ScatterStack.MillisecondsUntilNextFrame)
		    {
			    _stopwatch.Restart();
			    return true;
		    }

		    return false;
        }
    }
}