using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.Utility
{
    public class SpawnVisualizerPixel
    {
        public Vector3 Position;
        public readonly float Fitness;
        public readonly float Alpha;
    
        public SpawnVisualizerPixel(Vector3 position, float fitness, float alpha)
        {
            Position = position;
            Fitness = fitness;
            Alpha = alpha;
        }
    }
}
