#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.Common 
{
    [Serializable]
    public static class UndoEditor
    {
        public static void OnGUI()
		{
            GUILayout.BeginHorizontal();
            {
				GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
				if(CustomEditorGUILayout.ClickButton("Undo (" + Undo.Editor.Undo.UndoRecordCount + "/" + Undo.Editor.Undo.MaxNumberOfUndo + ")"))
				{
					Undo.Editor.Undo.PerformUndo();
				}
				GUILayout.Space(3);
				if(CustomEditorGUILayout.ClickButton("Undo All"))
				{
					Undo.Editor.Undo.PerformUndoAll();
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();
        }
    }
}
#endif