using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using Component = VladislavTsurikov.ComponentStack.Runtime.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    [MenuItem("Random Seed Settings")]
    public class RandomSeedSettings : Component
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