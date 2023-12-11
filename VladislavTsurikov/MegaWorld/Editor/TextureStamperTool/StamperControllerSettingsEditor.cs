#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool;

namespace VladislavTsurikov.MegaWorld.Editor.TextureStamperTool
{
    [ElementEditor(typeof(StamperControllerSettings))]
    public class StamperControllerSettingsEditor : IMGUIElementEditor
    {
        private StamperControllerSettings _stamperControllerSettings => (StamperControllerSettings)Target;

        private GUIContent _visualisation = new GUIContent("Visualisation", "Allows you to see the Mask Filter Settings visualization.");
        private GUIContent _autoRespawn = new GUIContent("Auto Respawn", "Allows you to do automatic deletion and then spawn when you changed the settings.");
        private GUIContent _delayAutoSpawn = new GUIContent("Delay Auto Spawn", "Respawn delay in seconds.");

        public TextureStamper TextureStamper => (TextureStamper)_stamperControllerSettings.StamperTool;

        public override void OnGUI()
		{
			_stamperControllerSettings.Visualisation = CustomEditorGUILayout.Toggle(_visualisation, _stamperControllerSettings.Visualisation);

			if(TextureStamper.Area.UseSpawnCells == false)
			{
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
							UnspawnUtility.UnspawnGroups(TextureStamper.Data.GroupList, false);
							TextureStamper.Spawn();
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
			}
			else
			{
				CustomEditorGUILayout.HelpBox("Auto Spawn does not support when \"Use Spawn Cells\" is enabled in \"Area Settings\".");
	
				DrawSpawnWithCellsControls();
			}
		}

		private void DrawSpawnControls()
        {
            if (TextureStamper.SpawnProgress > 0f && TextureStamper.SpawnProgress < 1f)
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
				if(TextureStamper.Data.SelectedData.GetSelectedPrototypes(typeof(PrototypeTerrainTexture)).Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							UnspawnUtility.UnspawnGroups(TextureStamper.Data.GroupList, false);
							TextureStamper.Spawn();
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
						TextureStamper.Spawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		private void DrawSpawnWithCellsControls()
        {
			if (TextureStamper.SpawnProgress > 0f && TextureStamper.SpawnProgress < 1f)
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
				if(TextureStamper.Data.SelectedData.GetSelectedPrototypes(typeof(PrototypeTerrainTexture)).Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Refresh", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							if(TextureStamper.Area.CellList.Count == 0)
							{
								TextureStamper.Area.CreateCells();
							}

							UnspawnUtility.UnspawnGroups(TextureStamper.Data.GroupList, false);
							TextureStamper.SpawnWithCells(TextureStamper.Area.CellList);
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
						if(TextureStamper.Area.CellList.Count == 0)
						{
							TextureStamper.Area.CreateCells();
						}

						TextureStamper.SpawnWithCells(TextureStamper.Area.CellList);
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		public void CancelSpawn()
        {
            TextureStamper.CancelSpawn = true;
            TextureStamper.SpawnProgress = 0f;
			EditorUtility.ClearProgressBar();
        }
    }
}
#endif