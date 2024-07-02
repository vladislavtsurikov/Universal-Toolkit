using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    [Name("Spawn Prefabs")]
    [AfterLoadSceneComponent]
    public class SpawnPrefabs : Operation
    {
        public List<GameObject> GameObjects = new List<GameObject>();
        
        public override async UniTask DoOperation()
        {
            foreach (var gameObject in GameObjects)
            {
                Object.Instantiate(gameObject);
            }
        }
    }
}