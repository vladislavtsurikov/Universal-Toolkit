using System.Linq;
using UnityEngine;
using VladislavTsurikov.SceneManagerTool.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Examples.LevelSelect
{
    public class LevelSelector : MonoBehaviour
    {
        public GameObject Prefab;

        private void Start()
        {
            var search = Profile.Current.BuildSceneCollectionList.ActiveBuildSceneCollection.GetAllSceneCollections().Where(c => c.Name.Contains("Level"));

            foreach (var item in search)
            {
                GameObject newButton = Instantiate(Prefab, transform);
                newButton.GetComponent<LevelSelectorButton>().ApplyData(item.Name, item);
            }
        }
    }
}