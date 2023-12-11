using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem.Attributes;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem.Components
{
    [MenuItem("Spawn Prefabs")]
    [AfterLoadScene]
    public class SpawnPrefabs : Operation
    {
        public List<GameObject> GameObjects = new List<GameObject>();
        
        public override IEnumerator DoOperation(SceneCollection loadSceneCollection)
        {
            foreach (var gameObject in GameObjects)
            {
                Object.Instantiate(gameObject);
            }

            yield return null;
        }
    }
}