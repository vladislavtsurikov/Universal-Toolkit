﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner;

namespace VladislavTsurikov.MegaWorld.Editor.TerrainSpawner
{
    [ElementEditor(typeof(StamperControllerSettings))]
    public class StamperControllerSettingsEditor : IMGUIElementEditor
    {
        private StamperControllerSettings _stamperControllerSettings => (StamperControllerSettings)Target;
        
        private GUIContent _visualisation = new GUIContent("Visualisation", "Allows you to see the Mask Filter Settings visualization.");
        private GUIContent _autoRespawn = new GUIContent("Auto Respawn", "Allows you to do automatic deletion and then spawn when you changed the settings.");
        private GUIContent _delayAutoSpawn = new GUIContent("Delay Auto Spawn", "Respawn delay in seconds.");
        
        public override void OnGUI()
        {
            _stamperControllerSettings.Visualisation = CustomEditorGUILayout.Toggle(_visualisation, _stamperControllerSettings.Visualisation);
            
            _stamperControllerSettings.AutoRespawn = CustomEditorGUILayout.Toggle(_autoRespawn, _stamperControllerSettings.AutoRespawn);
            
            if(_stamperControllerSettings.AutoRespawn)
            {
	            EditorGUI.indentLevel++;
	            _stamperControllerSettings.DelayAutoRespawn = CustomEditorGUILayout.Slider(_delayAutoSpawn, _stamperControllerSettings.DelayAutoRespawn, 0, 3);
	            EditorGUI.indentLevel--;
            						
	            GUILayout.BeginHorizontal();
	            {
		            GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
		            if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
		            {
			            UnspawnUtility.UnspawnGroups(_stamperControllerSettings.StamperTool.Data.GroupList, false);
			            _stamperControllerSettings.StamperTool.Spawn(true);
		            }
		            GUILayout.Space(5);
	            }
	            GUILayout.EndHorizontal();
            
	            GUILayout.Space(3);
            }
            else
            {
	            DrawSpawnControls();
            }
            
            if (_stamperControllerSettings.StamperTool.SpawnProgress == 0)
            {
	            GUILayout.BeginHorizontal();
	            {
		            GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
		            if(CustomEditorGUILayout.ClickButton("Unspawn Selected Prototypes", ButtonStyle.Remove, ButtonSize.ClickButton))
		            {
			            if (EditorUtility.DisplayDialog("WARNING!",
				                "Are you sure you want to remove all resource instances that have been selected from the scene?",
				                "OK", "Cancel"))
			            {
				            UnspawnUtility.UnspawnGroups(_stamperControllerSettings.StamperTool.Data.SelectedData.SelectedGroupList, true);
				            GUILayout.BeginHorizontal();
			            }
		            }
            
		            GUILayout.Space(5);
	            }
	            GUILayout.EndHorizontal();
            
	            GUILayout.Space(3);
            }
        }
        
        private void DrawSpawnControls()
        {
	        if (!_stamperControllerSettings.StamperTool.SpawnComplete)
	        {
		        GUILayout.BeginHorizontal();
		        {
			        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
			        if(CustomEditorGUILayout.ClickButton("Cancel", ButtonStyle.Remove))
			        {
				        CancelSpawn();
			        }
			        GUILayout.Space(5);
		        }
		        GUILayout.EndHorizontal();

		        GUILayout.Space(3);
	        }
	        else
	        {
		        if(_stamperControllerSettings.StamperTool.Data.SelectedData.GetSelectedPrototypes(typeof(PrototypeTerrainTexture)).Count == 0)
		        {
			        GUILayout.BeginHorizontal();
			        {
				        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
				        if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
				        {
					        UnspawnUtility.UnspawnGroups(_stamperControllerSettings.StamperTool.Data.GroupList, false);
					        _stamperControllerSettings.StamperTool.Spawn(true);
				        }

				        GUILayout.Space(5);
			        }
			        GUILayout.EndHorizontal();

			        GUILayout.Space(3);
		        }

		        GUILayout.BeginHorizontal();
		        {
			        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
			        if(CustomEditorGUILayout.ClickButton("Spawn", ButtonStyle.Add))
			        {
				        _stamperControllerSettings.StamperTool.Spawn(true);
			        }
			        GUILayout.Space(5);
		        }
		        GUILayout.EndHorizontal();

		        GUILayout.Space(3);
	        }
        }

        private void CancelSpawn()
        {
	        _stamperControllerSettings.StamperTool.CancelSpawn = true;
	        _stamperControllerSettings.StamperTool.SpawnComplete = true;
	        _stamperControllerSettings.StamperTool.SpawnProgress = 0f;
	        CoroutineRunner.StopCoroutines(_stamperControllerSettings.StamperTool);
	        EditorUtility.ClearProgressBar();
        }
    }
}
#endif