using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    [Name("Random Seed Settings")]
    public class RandomSeedSettings : ComponentStack.Runtime.Core.Component
    {
        public int RandomSeed;
        public bool GenerateRandomSeed;

        public void GenerateRandomSeedIfNecessary()
        {
            if (GenerateRandomSeed)
            {
                ChangeRandomSeed();
            }
            else
            {
                Random.InitState(RandomSeed);
            }
        }
        
        private void ChangeRandomSeed() 
        {
            RandomSeed = Random.Range(0, int.MaxValue);
            
            Random.InitState(RandomSeed);
        }
    }
}