using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.Spawner.Utility
{
    public static class SpawnGroup 
    {
        public static async UniTask SpawnGameObject(CancellationToken token, Group group, BoxArea boxArea, bool displayProgressBar)
        {
            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(displayProgressBar
                ? new DefaultWaitingNextFrame(0.2f)
                : null);

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;
            
            await scatterComponentSettings.ScatterStack.Samples(boxArea, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, boxArea.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                
                if(rayHit != null)
                {
                    PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);;

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }

                    Common.Utility.Spawn.SpawnPrototype.SpawnGameObject(group, proto, rayHit, 1);
                }
            }, token);
        }
    }
}