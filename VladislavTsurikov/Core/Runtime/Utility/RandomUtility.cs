using UnityEngine;

namespace VladislavTsurikov.Core.Runtime.Utility
{
    public static class RandomUtility
    {
        public static void ChangeRandomSeed() 
        {
            int randomSeed = Random.Range(0, int.MaxValue);
            
            Random.InitState(randomSeed);
        }
    }
}