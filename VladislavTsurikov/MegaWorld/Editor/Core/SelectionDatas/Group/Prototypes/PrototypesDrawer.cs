#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Utility.Runtime;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes
{
	public abstract class PrototypesDrawer
	{
		private readonly List<ClipboardPrototype> _clipboardPrototypes = new List<ClipboardPrototype>();
		private readonly TemplateStackEditor _templateStackEditor = new TemplateStackEditor();
		private readonly Type _toolType;
		
		protected readonly SelectionData Data;

		protected PrototypesDrawer(SelectionData selectionData, Type toolType)
		{
			Data = selectionData;
			_toolType = toolType;

			foreach (var prototypeType in AllPrototypeTypes.TypeList)
			{
				_clipboardPrototypes.Add(new ClipboardPrototype(toolType, prototypeType));
			}
		}

		public abstract void OnGUI();

		protected GenericMenu PrototypeMenu(IShowIcon icon)
        {
            GenericMenu menu = new GenericMenu();

            Prototype prototype = (Prototype)icon;
			
			UnityEngine.Object prototypeObject = prototype.PrototypeObject;

			if(prototypeObject != null)
			{
				menu.AddItem(new GUIContent("Reveal in Project"), false, ContextMenuUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(prototypeObject)));
				menu.AddSeparator ("");
			}

			menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback, new Action(() => Data.SelectedData.SelectedGroup.DeleteSelectedPrototype()));

			ClipboardPrototype clipboardPrototype = ClipboardObject.GetCurrentClipboardObject(Data.SelectedData.SelectedGroup.PrototypeType, _toolType, _clipboardPrototypes);

			 clipboardPrototype?.PrototypeMenu(menu, Data.SelectedData);
			_templateStackEditor?.ShowMenu(menu, _toolType, Data.SelectedData.SelectedGroup, Data.SelectedData);
			
			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, ContextMenuUtility.ContextMenuCallback, new Action(() => Data.SelectedData.SelectedGroup.SelectAllPrototypes(true)));
			menu.AddItem(new GUIContent("Active"), prototype.Active, ContextMenuUtility.ContextMenuCallback, new Action(() => 
				Data.SelectedData.SelectedPrototypeList.ForEach (proto => { proto.Active = !proto.Active;})));
			
            return menu;
        }
    }
}
#endif