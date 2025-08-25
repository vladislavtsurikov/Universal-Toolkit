using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
	[Name("Tiles")]  
    public class Tiles : Scatter
    {
	    public Vector2 Size = new Vector2(1, 1);
        
        public override async UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
	        Vector2Int tileMapSize = new Vector2Int(
		        Mathf.RoundToInt(boxArea.BoxSize),
		        Mathf.RoundToInt(boxArea.BoxSize)
	        );

	        for (int x = 0; x < tileMapSize.x; x++)
	        {
		        for (int y = 0; y < tileMapSize.y; y++)
		        {
			        token.ThrowIfCancellationRequested();

			        if (ScatterStack.IsWaitForNextFrame())
			        {
				        await UniTask.Yield();
			        }

			        Vector2 cellCenter = new Vector2(
				        x * Size.x + Size.x / 2,
				        y * Size.y + Size.y / 2
			        );
			        
			        if (boxArea.Contains(cellCenter))
			        {
				        onSpawn?.Invoke(cellCenter);
				        samples.Add(cellCenter);
			        }
		        }
	        }
        }
    }
}