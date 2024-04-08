using System.Collections.Generic;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn.Actions
{
    public class RespawnUnityTerrainDetail : Respawn
    {
        private readonly PrototypeTerrainDetail _modifiedTerrainDetailProto;
        
        public RespawnUnityTerrainDetail(PrototypeTerrainDetail proto, StamperTool stamperToolTool) : base(stamperToolTool)
        {
            _modifiedTerrainDetailProto = proto;
        }

        public override void OnRespawn()
        {
            Area area = (Area)StamperTool.GetElement(typeof(Area));
            
            CallbackList<Prototype> protoTerrainDetailList = new CallbackList<Prototype>();
            protoTerrainDetailList.Add(_modifiedTerrainDetailProto);
            UnspawnTerrainDetail.Unspawn(protoTerrainDetailList, false);

            Group group = StamperTool.Data.SelectedData.SelectedGroup;

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(StamperTool.transform.position), 
                layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                    	
            if(rayHit != null)
            {
                BoxArea boxArea = area.GetAreaVariables(rayHit);

                SpawnGroup.SpawnTerrainDetails(group, protoTerrainDetailList, boxArea);
            }
        }
    }
}