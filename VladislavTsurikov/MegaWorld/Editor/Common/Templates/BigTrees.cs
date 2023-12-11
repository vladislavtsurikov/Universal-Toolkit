#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Components;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem.Components;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Templates
{
	[Template("Trees/Big Trees", new[]{typeof(AdvancedBrushTool.AdvancedBrushTool), typeof(Runtime.TerrainSpawner.TerrainSpawner)}, 
		new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)})]
	public class BigTrees : Template
	{
		protected override void Apply(Group group)
    	{
			ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
			FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

			#region Scatter Settings
			scatterComponentSettings.Stack.Clear();

            RandomGrid randomGrid = (RandomGrid)scatterComponentSettings.Stack.CreateIfMissingType(typeof(RandomGrid));

            randomGrid.RandomisationType = RandomisationType.Square;
    		randomGrid.Vastness = 1;
    		randomGrid.GridStep = new Vector2(3, 3);
            randomGrid.FailureRate = 80;
            #endregion

			#region Mask Filters

			filterSettings.FilterType = FilterType.MaskFilter;
    		filterSettings.MaskFilterComponentSettings.MaskFilterStack.Clear();

    		HeightFilter heightFilter = (HeightFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateComponent(typeof(HeightFilter));
    		heightFilter.MinHeight = 0;
    		heightFilter.MaxHeight = 620;
    		heightFilter.AddHeightFalloff = 100;

            filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateComponent(typeof(SlopeFilter));

            MaskOperationsFilter maskOperationsFilter = (MaskOperationsFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateComponent(typeof(MaskOperationsFilter));
            maskOperationsFilter.MaskOperations = MaskOperations.Remap;
            maskOperationsFilter.RemapRange = new Vector2(0.8f, 1f);
    		#endregion
		}

		protected override void Apply(Prototype proto)
    	{
			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

    		#region Transform Components
    		transformComponentSettings.Stack.Clear();

    		transformComponentSettings.Stack.CreateIfMissingType(typeof(TreeRotation));
    		transformComponentSettings.Stack.CreateIfMissingType(typeof(Align));
    		transformComponentSettings.Stack.CreateIfMissingType(typeof(PositionOffset));
    		transformComponentSettings.Stack.CreateIfMissingType(typeof(SlopePosition));
    		transformComponentSettings.Stack.CreateIfMissingType(typeof(Scale)); 
    		ScaleFitness scaleFitness = (ScaleFitness)transformComponentSettings.Stack.CreateIfMissingType(typeof(ScaleFitness));
            scaleFitness.OffsetScale = -1.2f;
            
    		transformComponentSettings.Stack.CreateIfMissingType(typeof(ScaleClamp));
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShapeEnum = OverlapShapeEnum.Sphere;
    		overlapCheckSettings.SphereCheck.VegetationMode = true;
    		overlapCheckSettings.SphereCheck.Priority = 0;
    		overlapCheckSettings.SphereCheck.ViabilitySize = 4f;
    		overlapCheckSettings.SphereCheck.TrunkSize = 0.8f;
			#endregion
		}
	}
}
#endif