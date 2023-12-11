using System.Collections;
using UnityEngine;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation
{
    public abstract class SceneOperation : MonoBehaviour
    {
        public abstract IEnumerator OnLoad();
        public abstract IEnumerator OnUnload();
    }
}