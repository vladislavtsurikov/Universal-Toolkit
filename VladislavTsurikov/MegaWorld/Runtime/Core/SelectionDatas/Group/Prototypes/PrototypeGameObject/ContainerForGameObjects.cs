using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    public class ContainerForGameObjects : DefaultGroupComponent
    {
        private Dictionary<Scene, GameObject> _containerForGameObjects = new Dictionary<Scene, GameObject>();

        public void ParentGameObject(GameObject gameObject)
        {
            _containerForGameObjects ??= new Dictionary<Scene, GameObject>();
            
            if(!_containerForGameObjects.ContainsKey(gameObject.scene) || _containerForGameObjects[gameObject.scene] == null)  
            {
                FindParentObject(gameObject.scene);
            }

            GameObjectUtility.ParentGameObject(gameObject, _containerForGameObjects[gameObject.scene]);
        }
        
        private void FindParentObject(Scene scene)
        {
            string groupName = Group.name;

            _containerForGameObjects.Remove(scene);
            _containerForGameObjects.Add(scene, GameObjectUtility.FindParentGameObject(groupName, scene));
        }
    }
}