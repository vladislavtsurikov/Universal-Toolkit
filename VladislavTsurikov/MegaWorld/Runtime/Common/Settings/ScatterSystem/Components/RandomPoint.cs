using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem.Components
{
    [MenuItem("Random Point")]  
    public class RandomPoint : Scatter
    {
        public int MinChecks = 15;
		public int MaxChecks = 15;
        
        public override IEnumerator Samples(BoxArea boxArea, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
            int numberOfChecks = UnityEngine.Random.Range(MinChecks, MaxChecks);
        
            for (int checks = 0; checks < numberOfChecks; checks++)
            {
                if (IsWaitForNextFrame())
                {
                    yield return null;
                }
                
                Vector2 point = GetRandomPoint(boxArea);
                onSpawn?.Invoke(point);
                samples.Add(point);
            }
        }

        private Vector2 GetRandomPoint(BoxArea boxArea)
        {
            Vector2 spawnOffset = new Vector3(UnityEngine.Random.Range(-boxArea.Radius, boxArea.Radius), UnityEngine.Random.Range(-boxArea.Radius, boxArea.Radius));
            return new Vector2(spawnOffset.x + boxArea.RayHit.Point.x, spawnOffset.y + boxArea.RayHit.Point.z);
        }
    }
}