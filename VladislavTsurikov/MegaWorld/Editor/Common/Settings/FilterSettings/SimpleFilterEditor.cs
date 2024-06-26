﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.CPUNoise.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings
{
	[ElementEditor(typeof(SimpleFilter))]
    public class SimpleFilterEditor : IMGUIElementEditor
    {
	    private bool _additionalNoiseSettingsFoldout = true;
	    
        private SimpleFilter _simpleFilter;
        
        public override void OnEnable()
        {
            _simpleFilter = (SimpleFilter)Target;
        }
        
        public override void OnGUI()
        {
	        DrawCheckHeight(_simpleFilter);
	        DrawCheckSlope(_simpleFilter);

	        if(_simpleFilter.UseFalloff)
	        {
		        DrawCheckFractalNoise(_simpleFilter);
	        }
        }

        private void DrawCheckHeight(SimpleFilter filter)
		{
			filter.CheckHeight = CustomEditorGUILayout.Toggle(_checkHeight, filter.CheckHeight);

			EditorGUI.indentLevel++;

			if(filter.CheckHeight)
			{
				filter.MinHeight = CustomEditorGUILayout.FloatField(new GUIContent("Min Height"), filter.MinHeight);
				filter.MaxHeight = CustomEditorGUILayout.FloatField(new GUIContent("Max Height"), filter.MaxHeight);

				DrawHeightFalloff(filter);
			}

			EditorGUI.indentLevel--;
		}

        private void DrawHeightFalloff(SimpleFilter filter)
		{
			if(!filter.UseFalloff)
			{
				return;
			}

			filter.HeightFalloffType = (FalloffType)CustomEditorGUILayout.EnumPopup(_heightFalloffType, filter.HeightFalloffType);

			if(filter.HeightFalloffType != FalloffType.None)
			{
				filter.HeightFalloffMinMax = CustomEditorGUILayout.Toggle(_heightFalloffMinMax, filter.HeightFalloffMinMax);
			
				if(filter.HeightFalloffMinMax)
				{
					filter.MinAddHeightFalloff = CustomEditorGUILayout.FloatField(_minAddHeightFalloff, filter.MinAddHeightFalloff);
					filter.MaxAddHeightFalloff = CustomEditorGUILayout.FloatField(_maxAddHeightFalloff, filter.MaxAddHeightFalloff);
				}
				else
				{
					filter.AddHeightFalloff = CustomEditorGUILayout.FloatField(_addHeightFalloff, filter.AddHeightFalloff);
				}
			}
		}

        private void DrawSlopeFalloff(SimpleFilter filter)
		{
			if(!filter.UseFalloff)
			{
				return;
			}
			
			filter.SlopeFalloffType = (FalloffType)CustomEditorGUILayout.EnumPopup(_slopeFalloffType, filter.SlopeFalloffType);

			if(filter.SlopeFalloffType != FalloffType.None)
			{
				filter.SlopeFalloffMinMax = CustomEditorGUILayout.Toggle(_slopeFalloffMinMax, filter.SlopeFalloffMinMax);

				if(filter.SlopeFalloffMinMax)
				{
					filter.MinAddSlopeFalloff = CustomEditorGUILayout.FloatField(_minAddSlopeFalloff, filter.MinAddSlopeFalloff);
					filter.MaxAddSlopeFalloff = CustomEditorGUILayout.FloatField(_maxAddSlopeFalloff, filter.MaxAddSlopeFalloff);
				}
				else
				{
					filter.AddSlopeFalloff = CustomEditorGUILayout.FloatField(_addSlopeFalloff, filter.AddSlopeFalloff);
				}
			}
		}

		private void DrawCheckSlope(SimpleFilter filter)
		{
			filter.CheckSlope = CustomEditorGUILayout.Toggle(_checkSlope, filter.CheckSlope);

			EditorGUI.indentLevel++;

			if(filter.CheckSlope)
			{
				CustomEditorGUILayout.MinMaxSlider(_slope, ref filter.MinSlope, ref filter.MaxSlope, 0f, 90);
				
				DrawSlopeFalloff(filter);
			}

			EditorGUI.indentLevel--;
		}

		private void DrawCheckFractalNoise(SimpleFilter filter)
		{
			EditorGUI.BeginChangeCheck();

			int width = 150;
			int height = 150;

			filter.CheckGlobalFractalNoise = CustomEditorGUILayout.Toggle(new GUIContent("Check Global Fractal Noise"), filter.CheckGlobalFractalNoise);
			
			if(filter.CheckGlobalFractalNoise)
			{
				EditorGUI.indentLevel++;

				filter.NoisePreviewTexture = CustomEditorGUILayout.Foldout(filter.NoisePreviewTexture, "Noise Preview Texture");

				GUILayout.BeginHorizontal();
				{
					if(filter.NoisePreviewTexture )
					{
						EditorGUI.indentLevel++;

						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());

						Rect textureRect = GUILayoutUtility.GetRect(250, 250, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
						GUI.DrawTexture(textureRect, filter.NoiseTexture);

						EditorGUI.indentLevel--;
					}
				}
				GUILayout.EndHorizontal();

				filter.Fractal.NoiseType = (NoiseType)CustomEditorGUILayout.EnumPopup(new GUIContent("Noise Type"), filter.Fractal.NoiseType);

				filter.Fractal.Seed = CustomEditorGUILayout.IntSlider(_seed, filter.Fractal.Seed, 0, 65000);
				filter.Fractal.Octaves = CustomEditorGUILayout.IntSlider(_octaves, filter.Fractal.Octaves, 1, 12);
				filter.Fractal.Frequency = CustomEditorGUILayout.Slider(_frequency, filter.Fractal.Frequency, 0f, 0.01f);

				filter.Fractal.Persistence = CustomEditorGUILayout.Slider(_persistence, filter.Fractal.Persistence, 0f, 1f);
				filter.Fractal.Lacunarity = CustomEditorGUILayout.Slider(_lacunarity, filter.Fractal.Lacunarity, 1f, 3.5f);

				_additionalNoiseSettingsFoldout = CustomEditorGUILayout.Foldout(_additionalNoiseSettingsFoldout, "Additional Settings");

				if(_additionalNoiseSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					filter.RemapNoiseMin = CustomEditorGUILayout.Slider(_remapNoiseMin, filter.RemapNoiseMin, 0f, 1f);
					filter.RemapNoiseMax = CustomEditorGUILayout.Slider(_remapNoiseMax, filter.RemapNoiseMax, 0f, 1f);

					filter.Invert = CustomEditorGUILayout.Toggle(_invert, filter.Invert);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			if (EditorGUI.EndChangeCheck())
            {		
				if(filter.NoisePreviewTexture)
				{
					FractalNoiseCPU fractal = new FractalNoiseCPU(filter.Fractal.GetNoise(), filter.Fractal.Octaves, filter.Fractal.Frequency / 7, filter.Fractal.Lacunarity, filter.Fractal.Persistence);
					filter.NoiseTexture = new Texture2D(width, height);

                	float[,] arr = new float[width, height];

                	for(int y = 0; y < height; y++)
                	{
                	    for (int x = 0; x < width; x++)
                	    { 
							arr[x,y] = fractal.Sample2D(x, y);
                	    }
                	}

					NoiseUtility.NormalizeArray(arr, width, height, ref filter.RangeMin, ref filter.RangeMax);
	
                	for (int y = 0; y < height; y++)
                	{
                	    for (int x = 0; x < width; x++)
                	    {
                	        float fractalValue = arr[x, y];
							
							if (filter.Invert)
                   			{
                   			    fractalValue = 1 - fractalValue;
                   			}

							if (fractalValue < filter.RemapNoiseMin) 
                			{
                			    fractalValue = 0;
                			}
                			else if(fractalValue > filter.RemapNoiseMax)
                			{
                			    fractalValue = 1;
                			}
							else
							{
								fractalValue = Mathf.InverseLerp(filter.RemapNoiseMin, filter.RemapNoiseMax, fractalValue);
							}

                	        filter.NoiseTexture.SetPixel(x, y, new Color(fractalValue, fractalValue, fractalValue, 1));
                	    }
                	}

                	filter.NoiseTexture.Apply();
				}
				else
				{
					FindNoiseRangeMinMax(filter, width, height);
				}	
			}
		}

		private void FindNoiseRangeMinMax(SimpleFilter filter, int width, int height)
		{
			FractalNoiseCPU fractal = new FractalNoiseCPU(filter.Fractal.GetNoise(), filter.Fractal.Octaves, filter.Fractal.Frequency / 7, filter.Fractal.Lacunarity, filter.Fractal.Persistence);
			filter.NoiseTexture = new Texture2D(150, 150);

            float[,] arr = new float[width, height];

            for(int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                { 
					arr[x,y] = fractal.Sample2D(x, y);
                }
            }

			NoiseUtility.NormalizeArray(arr, width, height, ref filter.RangeMin, ref filter.RangeMax);
		}

		private readonly GUIContent _checkHeight = new GUIContent("Check Height");
		private readonly GUIContent _heightFalloffType = new GUIContent("Height Falloff Type");
		private readonly GUIContent _heightFalloffMinMax = new GUIContent("Height Falloff Min Max");
		private readonly GUIContent _minAddHeightFalloff = new GUIContent("Min Add Height Falloff");
		private readonly GUIContent _maxAddHeightFalloff = new GUIContent("Max Add Height Falloff");
		private readonly GUIContent _addHeightFalloff = new GUIContent("Add Height Falloff");

		private readonly GUIContent _seed = new GUIContent("Seed");
		private readonly GUIContent _octaves = new GUIContent("Octaves");
		private readonly GUIContent _frequency = new GUIContent("Frequency");
		private readonly GUIContent _persistence = new GUIContent("Persistence");
		private readonly GUIContent _lacunarity = new GUIContent("Lacunarity");
		private readonly GUIContent _remapNoiseMin = new GUIContent("Remap Noise Min");
		private readonly GUIContent _remapNoiseMax = new GUIContent("Remap Noise Max");
		private readonly GUIContent _invert = new GUIContent("Invert");

		private readonly GUIContent _checkSlope = new GUIContent("Check Slope");
		private readonly GUIContent _slope = new GUIContent("Slope");	

		private readonly GUIContent _slopeFalloffType = new GUIContent("Slope Falloff Type");
		private readonly GUIContent _slopeFalloffMinMax = new GUIContent("Slope Falloff Min Max");
		private readonly GUIContent _minAddSlopeFalloff = new GUIContent("Min Add Slope Falloff");
		private readonly GUIContent _maxAddSlopeFalloff = new GUIContent("Max Add Slope Falloff");
		private readonly GUIContent _addSlopeFalloff = new GUIContent("Add Slope Falloff");
    }
}
#endif