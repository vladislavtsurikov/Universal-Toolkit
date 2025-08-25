using UnityEngine;
using VladislavTsurikov.ObjectPool.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem
{
    public class ScriptingSystemPool : ObjectPool<GameObject>
    {
        private readonly PrototypeScriptingManager _prototypeScriptingManager;

        public ScriptingSystemPool(PrototypeScriptingManager prototypeScriptingManager) : base(false) =>
            _prototypeScriptingManager = prototypeScriptingManager;

        public GameObject Get(TerrainObjectInstance instance)
        {
            GameObject go = Get();

            _prototypeScriptingManager.MoveGameObject(instance, go);

            return go;
        }

        protected override void OnGet(GameObject go) => go.SetActive(true);

        protected override void OnRelease(GameObject go) => go.SetActive(false);

        protected override GameObject OnCreateObject() =>
            _prototypeScriptingManager.CreateGameObject(CountAll.ToString());
    }
}
