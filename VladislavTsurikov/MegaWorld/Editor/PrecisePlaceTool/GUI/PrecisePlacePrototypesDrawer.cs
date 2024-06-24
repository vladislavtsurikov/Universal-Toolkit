#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.IconStack;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack.Attributes;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.OdinSerializer.Utilities;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.GUI
{
	public class PrecisePlacePrototypesDrawer : PrototypesDrawer
    {
		private bool _selectPrototypeFoldout = true;
		private readonly IconStackEditor _iconStackEditor = new IconStackEditor(true);

		public PrecisePlacePrototypesDrawer(SelectionData data, Type toolType) : base(data, toolType)
		{
			_iconStackEditor.IconColor = SetIconColor;
		}

        public override void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
			
            if(Data.SelectedData.SelectedGroup != null)
            {
	            _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout, "Prototypes " + "(" + Data.SelectedData.SelectedGroup.GetPrototypeTypeName() + ")");
            }
            else
            {
	            _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout, "Prototypes ");
            }
            
            EditorGUILayout.EndHorizontal();

			++EditorGUI.indentLevel;
            
			if(_selectPrototypeFoldout)
			{
				if(Data.SelectedData.SelectedGroup == null)
				{
					_iconStackEditor.OnGUI("To Draw Prototype, you need to select one group");
				}
				else
				{
					_iconStackEditor.AddIconCallback = Data.SelectedData.SelectedGroup.AddMissingPrototype;
					_iconStackEditor.IconMenuCallback = PrototypeMenu;
					_iconStackEditor.ZeroIconsWarning = Data.SelectedData.SelectedGroup.PrototypeType.GetAttribute<MissingIconsWarningAttribute>().Text;
					_iconStackEditor.OnGUI(Data.SelectedData.SelectedGroup.PrototypeList, Data.SelectedData.SelectedGroup.PrototypeType);
				}
			}

			--EditorGUI.indentLevel;
        }

		private void SetIconColor(IShowIcon icon, out Color textColor, out Color rectColor)
		{
			Prototype proto = (Prototype)icon;
			if(ActiveObjectController.PlacedObjectData != null && proto == ActiveObjectController.PlacedObjectData.Proto)
			{
				rectColor = EditorColors.Instance.orangeNormal.WithAlpha(0.3f);

				if (EditorGUIUtility.isProSkin)
            	{
					textColor = EditorColors.Instance.orangeNormal; 
            	}
            	else
            	{
					textColor = EditorColors.Instance.orangeDark;
				}
			}
			else
			{
                textColor = EditorColors.Instance.LabelColor;

			    if(proto.Selected)
			    {
			    	rectColor = !proto.Active ? EditorColors.Instance.redNormal : EditorColors.Instance.ToggleButtonActiveColor;
			    }
			    else
			    {
			    	rectColor = !proto.Active ? EditorColors.Instance.redDark : EditorColors.Instance.ToggleButtonInactiveColor;
			    }
			}
		}
    }
}
#endif