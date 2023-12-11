#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Components;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise;
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
	[Template("Cliffs/Big Rocks", new[]{typeof(AdvancedBrushTool.AdvancedBrushTool), typeof(Runtime.TerrainSpawner.TerrainSpawner)},
		new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)})]
	public class BigCliffs : Template
	{
		protected override void Apply(Group group)
    	{
	        FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));
			ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));

			#region Scatter Settings
			scatterComponentSettings.Stack.Clear();

            RandomGrid randomGrid = (RandomGrid)scatterComponentSettings.Stack.CreateIfMissingType(typeof(RandomGrid));
            randomGrid.RandomisationType = RandomisationType.Square;
    		randomGrid.Vastness = 1;
    		randomGrid.GridStep = new Vector2(1.7f, 1.7f);
            randomGrid.FailureRate = 60;
            #endregion

			#region Mask Filters
			filterSettings.FilterType = FilterType.MaskFilter;
			filterSettings.MaskFilterComponentSettings.MaskFilterStack.Clear();

    		NoiseFilter noiseFilter = (NoiseFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateComponent(typeof(NoiseFilter));
            noiseFilter.NoiseSettings = new NoiseSettings
            {
	            TransformSettings = new NoiseSettings.NoiseTransformSettings
	            {
		            Scale = new Vector3(31, 40, 31)
	            }
            };

            MaskOperationsFilter remapFilter = (MaskOperationsFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateComponent(typeof(MaskOperationsFilter));
			remapFilter.MaskOperations = MaskOperations.Remap;
    		remapFilter.RemapRange.x = 0.44f;
    		remapFilter.RemapRange.y = 0.47f;

    		SlopeFilter slopeFilter = (SlopeFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateComponent(typeof(SlopeFilter));
    		slopeFilter.MinSlope = 48;
    		slopeFilter.MaxSlope = 90;
    		slopeFilter.AddSlopeFalloff = 17;
			#endregion
		}

		protected override void Apply(Prototype proto)
    	{
			TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

    		#region Transform Components
    		transformComponentSettings.Stack.Clear();

			transformComponentSettings.Stack.CreateIfMissingType(typeof(CliffsAlign));
    		transformComponentSettings.Stack.CreateIfMissingType(typeof(SlopePosition));
    		Scale scale = (Scale)transformComponentSettings.Stack.CreateIfMissingType(typeof(Scale)); 
			scale.MaxScale = new Vector3(1.4f, 1.4f, 1.4f);
    		ScaleFitness scaleFitness = (ScaleFitness)transformComponentSettings.Stack.CreateIfMissingType(typeof(ScaleFitness));
			scaleFitness.OffsetScale = -1;
			transformComponentSettings.Stack.CreateIfMissingType(typeof(ScaleClamp));
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckSettings.OverlapShapeEnum = OverlapShapeEnum.OBB;
			overlapCheckSettings.ObbCheck.MultiplyBoundsSize = 0.4f;
    		#endregion
		}
	}
}
#endif