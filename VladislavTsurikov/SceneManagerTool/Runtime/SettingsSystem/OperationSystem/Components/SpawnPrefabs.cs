using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    [MenuItem("Spawn Prefabs")]
    [AfterLoadSceneComponent]
    public class SpawnPrefabs : Operation
    {
        public List<GameObject> GameObjects = new List<GameObject>();
        
        public override IEnumerator DoOperation()
        {
            foreach (var gameObject in GameObjects)
            {
                Object.Instantiate(gameObject);
            }

            yield return null;
        }
    }
}