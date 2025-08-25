#if UNITY_EDITOR
using OdinSerializer;
using UnityEditor;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
	[LocationAsset("MegaWorld/WindowData")]
    public class WindowData : SerializedScriptableObjectSingleton<WindowData>
    {
	    [OdinSerialize] 
	    private WindowToolStack _windowToolStack = new WindowToolStack();
	    [OdinSerialize] 
	    public SelectionData SelectionData = new SelectionData();
	    [OdinSerialize]
        public ToolWindow SelectedTool => _windowToolStack.SelectedElement;
        public SelectedData SelectedData => SelectionData.SelectedData;
        public WindowToolStack WindowToolStack => _windowToolStack;
        
        public WindowToolStackEditor WindowToolStackEditor
        {
	        get
	        {
		        if(_windowToolComponentsEditor == null || _windowToolComponentsEditor.Stack == null)
		        {
			        _windowToolComponentsEditor = new WindowToolStackEditor(WindowToolStack);
		        }

		        return _windowToolComponentsEditor;
	        }
        }
        
        private WindowToolStackEditor _windowToolComponentsEditor;

        private void OnEnable()
		{
			SelectionData.Setup();
			WindowToolStack.Setup();
		}
		
		private void OnDisable()
		{
			WindowToolStack.OnDisable();
		}

		public void Save()
		{
            EditorUtility.SetDirty(this);
            SelectionData.SaveAllData();
		}
    }
}
#endif