﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.UnityUtility.Runtime;
using DrawHandles = VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint.DrawHandles;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings
{
    public static class SimpleFilterVisualisation
    {
        public static void DrawSimpleFilter(Group group, BoxArea boxArea, SimpleFilter filter, LayerSettings layerSettings, bool showPointsAroundRadius = false)
        {
            VisualisationSimpleFilterPreference visualisationSimpleFilterPreference = PreferenceElementSingleton<VisualisationSimpleFilterPreference>.Instance;

            if(!visualisationSimpleFilterPreference.EnableSpawnVisualization)
            {
                return;
            }

            float stepIncrement = boxArea.BoxSize / (visualisationSimpleFilterPreference.VisualiserResolution - 1f);

            Vector3 position = Vector3.zero;
            position.y = boxArea.RayHit.Point.y;

            float halfSpawnRange = boxArea.Radius;

            Vector3 maxPosition = boxArea.Center + Vector3.one * halfSpawnRange;
            float step = boxArea.BoxSize  / (visualisationSimpleFilterPreference.VisualiserResolution - 1f);

            for (position.x = boxArea.Center.x - halfSpawnRange; position.x < maxPosition.x; position.x += step)
            {
                for (position.z = boxArea.Center.z - halfSpawnRange; position.z < maxPosition.z; position.z += step)
                {
                    if(showPointsAroundRadius)
                    {
                        if(Vector3.Distance(boxArea.Center, position) > halfSpawnRange)
                        {
                            continue;
                        }
                    }
                    
                    float maskFitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, position, boxArea.Mask);

                    float fitness = filter.GetFitness(group, position, layerSettings, out var localPoint);

                    fitness *= maskFitness;

                    float alpha = visualisationSimpleFilterPreference.Alpha;

                    DrawSpawnVisualizerPixel(new SpawnVisualizerPixel(localPoint, fitness, alpha), stepIncrement);
                }
            }
        }

        private static void DrawSpawnVisualizerPixel(SpawnVisualizerPixel spawnVisualizerPixel, float stepIncrement)
        {
            VisualisationSimpleFilterPreference visualisationSimpleFilterPreference = PreferenceElementSingleton<VisualisationSimpleFilterPreference>.Instance;

            if(visualisationSimpleFilterPreference.ColorHandlesType == ColorHandlesType.Custom)
            {
                Handles.color = Color.Lerp(visualisationSimpleFilterPreference.InactiveColor, visualisationSimpleFilterPreference.ActiveColor, spawnVisualizerPixel.Fitness).WithAlpha(spawnVisualizerPixel.Alpha);
            }
            else
            {
                if(spawnVisualizerPixel.Fitness < 0.5)
                {
                    float difference = spawnVisualizerPixel.Fitness / 0.5f;
                    Handles.color = Color.Lerp(Color.red, Color.yellow, difference).WithAlpha(spawnVisualizerPixel.Alpha);
                }
                else
                {
                    float difference = (spawnVisualizerPixel.Fitness - 0.5f) / 0.5f;
                    Handles.color = Color.Lerp(Color.yellow, Color.green, difference).WithAlpha(spawnVisualizerPixel.Alpha);
                }
            }

            if(visualisationSimpleFilterPreference.HandlesType == HandlesType.DotCap)
            {
                if(visualisationSimpleFilterPreference.HandleResizingType == HandleResizingType.Resolution)
                {
                    DrawHandles.DotCap(0, spawnVisualizerPixel.Position, Quaternion.identity, stepIncrement / 3);
                }
                else if(visualisationSimpleFilterPreference.HandleResizingType == HandleResizingType.Distance)
                {
                    DrawHandles.DotCap(0, spawnVisualizerPixel.Position, Quaternion.identity, HandleUtility.GetHandleSize(spawnVisualizerPixel.Position) * 0.03f);
                }
                else
                {
                    DrawHandles.DotCap(0, spawnVisualizerPixel.Position, Quaternion.identity, visualisationSimpleFilterPreference.CustomHandleSize);
                }
            }
            else
            {
                if(visualisationSimpleFilterPreference.HandleResizingType == HandleResizingType.Resolution)
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.Position.x, spawnVisualizerPixel.Position.y, spawnVisualizerPixel.Position.z), Quaternion.LookRotation(Vector3.up), 
                        stepIncrement / 2, EventType.Repaint);
                }
                else if(visualisationSimpleFilterPreference.HandleResizingType == HandleResizingType.Distance)
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.Position.x, spawnVisualizerPixel.Position.y, spawnVisualizerPixel.Position.z), Quaternion.LookRotation(Vector3.up), 
                        HandleUtility.GetHandleSize(spawnVisualizerPixel.Position) * 0.05f, EventType.Repaint);
                }
                else
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.Position.x, spawnVisualizerPixel.Position.y, spawnVisualizerPixel.Position.z), Quaternion.LookRotation(Vector3.up), 
                        visualisationSimpleFilterPreference.CustomHandleSize, EventType.Repaint);
                }
            }
        }
    }
}
#endif