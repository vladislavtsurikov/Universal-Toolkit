using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Examples.LevelSelect
{
    public class LevelSelectorButton : MonoBehaviour
    {
        public Text Text;
        public SceneCollection Collection;

        public void OpenLevel()
        {
            CoroutineRunner.StartCoroutine(Collection.Load());
        }

        public void ApplyData(string text, SceneCollection collection)
        {
            Text.text = text;
            Collection = collection;
        }
    }
}
