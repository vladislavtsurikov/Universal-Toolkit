#if UNITY_EDITOR
#if !DISABLE_UIELEMENTS
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Components;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Enums;
using VladislavTsurikov.UIElementsUtility.Editor.Utility;
using VladislavTsurikov.Utility.Runtime.Extensions;
using Button = VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Components.Button;
using ButtonStyle = VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Enums.ButtonStyle;
using GameObjectUtility = VladislavTsurikov.Utility.Runtime.GameObjectUtility;
using ListView = VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Components.ListView;

namespace VladislavTsurikov.SceneDataSystem.Editor
{
	public class SceneDataManagerWindow : BaseWindow<SceneDataManagerWindow>
	{
	    private List<SceneDataManager> _sceneDataManagerList = new List<SceneDataManager>();
		
		private ListView _listView;
		private Button _loadFilesFromFolderButton;
		
		[MenuItem("Window/Vladislav Tsurikov/Debug/Scene Data Manager", false, 0)]
		private static void OpenMegaWorldWindow()
		{
			OpenWindow("Scene Data Manager");  
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			
			FindAllSceneDataManager();

			Reload();
		}
		
		internal static void Reload()
		{
			Instance.InitializeEditor();

			Instance.Root
				.AddPaddingSpace(5, DesignUtils.k_Spacing * 2, 5, 0)
				.AddChild
				(
					DesignUtils.row
						.AddChild(DesignUtils.flexibleSpace)
						.AddChild(Instance._loadFilesFromFolderButton)
						.AddChild(DesignUtils.flexibleSpace)
				)
				.AddSpace(0, DesignUtils.k_Spacing * 2)
				.AddChild(Instance._listView);
		}
		
	    private void InitializeEditor()
	    {
		    Root.Clear();
		    
		    _loadFilesFromFolderButton = new Button()
			    .SetLabelText("Find Scene Data Managers")
			    .SetAccentColor(EditorSelectableColors.EditorUI.Amber)
			    .SetButtonStyle(ButtonStyle.Contained)
			    .SetElementSize(ElementSize.Normal)
			    .SetOnClick(Instance.FindAllSceneDataManager);
		    
		    InitializeListView();
	    }
	    
	    private void InitializeListView()
        {
            _listView = new ListView();
            _listView.SetListTitle("Scene Data Managers");

            List<VisualElement> listViewItems = new List<VisualElement>();

		    for (int i = 0; i < _sceneDataManagerList.Count; i++)
		    {
			    SceneDataManager sceneDataManager = _sceneDataManagerList[i];
			    
			    VisualElement raw = DesignUtils.column;

			    EnumField enumField = new EnumField("Scene Type", sceneDataManager.SceneType);
			    enumField.RegisterValueChangedCallback(evt =>
			    {
				    sceneDataManager.SceneType = (SceneType)evt.newValue;
			    });

			    Label label = new Label(sceneDataManager.Scene.name)
				    .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
				    .SetStyleFontSize(12)
				    .SetStyleUnityFontStyle(FontStyle.Bold);

			    raw.Add(label);
			    raw.AddSpace(0, DesignUtils.k_Spacing * 2);
			    raw.Add(enumField);

			    VisualElementListViewItem sceneManagerItem = new VisualElementListViewItem(raw, i, _listView)
				    {
					    ItemRemoveButton =
					    {
						    OnClick = () => DeleteSceneDataManager(sceneDataManager)
					    }
				    };

			    ListView sceneDatasListView = new ListView();
			    sceneDatasListView.SetListTitle("Scene Datas");
			    sceneDatasListView.DisableTopAndBottom();
			    sceneDatasListView.HeaderContainer.SetStylePaddingLeft(5);
			    
			    List<VisualElement> sceneDataElements = new List<VisualElement>();
			    
			    for (int j = 0; j < sceneDataManager.SceneDataStack.ElementList.Count; j++)
			    {
				    SceneData sceneData = sceneDataManager.SceneDataStack.ElementList[j];
					    
				    Label sceneDataLabel = new Label(sceneData.Name)
					    .SetStyleUnityFont(EditorFonts.Ubuntu.Light)
					    .SetStyleFontSize(12)
					    .SetStyleUnityFontStyle(FontStyle.Bold);

				    var index = j;
				    VisualElementListViewItem sceneDataItem = new VisualElementListViewItem(sceneDataLabel, index, _listView)
					    {
						    ItemRemoveButton =
						    {
							    OnClick = () => DeleteSceneData(sceneDataManager, index) 
						    }
					    };

				    sceneDataElements.Add(sceneDataItem);
			    }
			    
			    sceneDatasListView.AddListViewItems(sceneDataElements);
			    
			    raw.Add(sceneDatasListView);
			    
			    listViewItems.Add(sceneManagerItem);
			    listViewItems.Add(new VisualElement().SetName("Space").SetStyleSize(0, DesignUtils.k_Spacing * 2));
		    }

		    _listView.AddListViewItems(listViewItems);

		    _listView?.Update();
        }

	    private void FindAllSceneDataManager()
	    {
		    var objs = GameObjectUtility.FindObjectsOfType(typeof(SceneDataManager));
		    _sceneDataManagerList.Clear();
		    foreach(var obj in objs)
		    {
			    SceneDataManager sceneDataManager = (SceneDataManager)obj;

			    if (sceneDataManager.SceneDataStack == null)
			    {
				    Debug.Log("Delete");
				    DeleteSceneDataManager(sceneDataManager);
				    continue;
			    }
			    
			    _sceneDataManagerList.Add((SceneDataManager)obj);
		    }

		    Reload();
	    }
	    
	    private void DeleteSceneDataManager(SceneDataManager sceneDataManager)
	    {
		    SceneDataManager component = _sceneDataManagerList.Find(component => component == sceneDataManager);
		    
		    if(component == null)
			    return;
		    
		    GameObject go = component.gameObject;
		    
		    DestroyImmediate(go);
		    FindAllSceneDataManager();
		    GUIUtility.ExitGUI();
		    
		    Reload();
	    }
	    
	    private void DeleteSceneData(SceneDataManager sceneDataManager, int index)
	    {
		    sceneDataManager.SceneDataStack.Remove(index);

		    Reload();
	    }
	}
}
#endif
#endif