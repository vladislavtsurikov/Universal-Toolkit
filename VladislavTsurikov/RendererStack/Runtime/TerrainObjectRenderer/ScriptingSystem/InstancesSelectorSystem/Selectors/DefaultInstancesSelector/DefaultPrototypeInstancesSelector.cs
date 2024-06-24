using System;
using UnityEngine.Profiling;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using Object = System.Object;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.DefaultInstancesSelector
{
	public class DefaultPrototypeInstancesSelector : PrototypeInstancesSelector
	{
		private readonly ScriptingSystemPool _scriptingSystemPool;

		public DefaultPrototypeInstancesSelector(PrototypeScriptingManager prototypeScriptingManager, PrototypeTerrainObject proto) : base(proto)
		{
			_scriptingSystemPool = new ScriptingSystemPool(prototypeScriptingManager);
		}
		
		public override void SetColliders(Sphere sphere, Object usedObj)
		{
			DefaultPrototypeInstancesSelectorData defaultPrototypeInstancesSelectorData = (DefaultPrototypeInstancesSelectorData)GetPrototypeInstancesSelectorData(usedObj);

			Profiler.BeginSample("ColliderUtility.OverlapSphere");
				
			var newInstanceList = TerrainObjectRendererAPI.OverlapSphere(sphere.Center, sphere.Radius,
				ObjectFilter, true);
				
			Profiler.EndSample();
				
			Profiler.BeginSample("After finding objects");
				
			foreach (var colliderObject in newInstanceList)
			{
				var objectCollider = (TerrainObjectCollider)colliderObject;

				if (objectCollider.Instance.HierarchyTerrainObjectInstance == null)
				{
					EnableCollider(objectCollider, usedObj);
					continue;
				}
				
				if (defaultPrototypeInstancesSelectorData.LastDistanceCullingSphere.IsValid)
				{
					if (!defaultPrototypeInstancesSelectorData.LastDistanceCullingSphere.Contains(objectCollider.Instance.Position))
					{
						EnableCollider(objectCollider, usedObj);
					}
					else
					{
						defaultPrototypeInstancesSelectorData.LastInstanceList.Remove(objectCollider);
					}
				}
				else
				{
					EnableCollider(objectCollider, usedObj);
				}
			}
				
			foreach (var colliderObject in defaultPrototypeInstancesSelectorData.LastInstanceList)
			{
				TerrainObjectCollider terrainObjectCollider = (TerrainObjectCollider)colliderObject;
				DisableCollider(terrainObjectCollider, usedObj);
			}
				
			defaultPrototypeInstancesSelectorData.LastDistanceCullingSphere = sphere;
			defaultPrototypeInstancesSelectorData.LastInstanceList.Clear();
			defaultPrototypeInstancesSelectorData.LastInstanceList = newInstanceList;
			
			Profiler.EndSample();
		}
		
		public override void OnNewInstanceVisibility(TerrainObjectInstance instance)
		{
			_scriptingSystemPool.Get(instance);
		}

		public override void OnInstanceInvisible(TerrainObjectInstance instance)
		{
			if (instance.HierarchyTerrainObjectInstance == null)
			{
				return;
			}
			
			_scriptingSystemPool.Release(instance.HierarchyTerrainObjectInstance.gameObject);
			instance.HierarchyTerrainObjectInstance = null;
		}

		protected override void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData, object usedObj)
		{
			DefaultPrototypeInstancesSelectorData defaultPrototypeInstancesSelectorData = (DefaultPrototypeInstancesSelectorData)prototypeInstancesSelectorData;
			
			foreach (var colliderObject in defaultPrototypeInstancesSelectorData.LastInstanceList)
			{
				TerrainObjectCollider terrainObjectCollider = (TerrainObjectCollider)colliderObject;

				DisableCollider(terrainObjectCollider, usedObj);
			}
		}

		protected override void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData)
		{
			DefaultPrototypeInstancesSelectorData defaultPrototypeInstancesSelectorData = (DefaultPrototypeInstancesSelectorData)prototypeInstancesSelectorData;
			
			foreach (var colliderObject in defaultPrototypeInstancesSelectorData.LastInstanceList)
			{
				TerrainObjectCollider terrainObjectCollider = (TerrainObjectCollider)colliderObject;

				DisableCollider(terrainObjectCollider);
			}
		}

		public override Type GetInstanceSelectorData()
		{
			return typeof(DefaultPrototypeInstancesSelectorData);
		}
	}
}