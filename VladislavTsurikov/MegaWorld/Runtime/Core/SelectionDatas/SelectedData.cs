﻿using System;
using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using Prototype = VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Prototype;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas
{
	using Prototype = Group.Prototypes.Prototype;

	public class SelectedData
	{
		private SelectionData _selectionData;
		
		private List<SelectedPrototypes> _selectedPrototypeTypes;
		private List<Group.Group> _selectedGroupList = new List<Group.Group>();

		public Group.Group SelectedGroup { get; private set; }
		public Prototype SelectedPrototype { get; private set; }
		public IReadOnlyList<Group.Group> SelectedGroupList => _selectedGroupList;
		public IReadOnlyList<Prototype> SelectedPrototypeList
		{
			get
			{
				List<Prototype> prototypeListTest = new List<Prototype>();

				foreach (var selectedPrototypeType in _selectedPrototypeTypes)
				{
					prototypeListTest.AddRange(selectedPrototypeType.SelectedPrototypeList);
				}

				return prototypeListTest;
			}
		}

		public void Setup(SelectionData selectionData)
		{
			_selectionData = selectionData;
			SetupSelectedPrototypeTypes();
		}
		
		public void SetSelectedData()
		{
			ClearSelectedList();
			SetSelectedList(_selectionData.GroupList);
			SetSelected();
		}

		private void ClearSelectedList()
		{
			_selectedGroupList.Clear();

			foreach (var item in _selectedPrototypeTypes)
			{
				item.SelectedPrototypeList.Clear();
			}
		}

		private void SetSelected()
		{
			SetSelectedGroup();
			SetSelectedPrototype();
		}

		private void SetSelectedList(List<Group.Group> groupList)
		{
		    for (int index = 0; index < groupList.Count; index++)
		    {
		    	if(groupList[index].Selected)
		    	{
					Group.Group selectedGroup = groupList[index];
					_selectedGroupList.Add(selectedGroup);
					
					foreach (var item in _selectedPrototypeTypes)
					{
						if (item.PrototypeType == selectedGroup.PrototypeType)
						{
							SetSelectedPrototypeListFromAssets(selectedGroup.PrototypeList, item.SelectedPrototypeList);
						}
					}
		        }
		    }
		}

		private void SetSelectedGroup()
		{
			if(_selectedGroupList.Count == 1)
			{
				SelectedGroup = _selectedGroupList[SelectedGroupList.Count - 1];
			}
			else
			{
				SelectedGroup = null;
			}
		}

		private void SetSelectedPrototype()
		{
			if(SelectedPrototypeList.Count == 1)
			{
				SelectedPrototype = SelectedPrototypeList[SelectedPrototypeList.Count - 1];
			}
			else
			{
				SelectedPrototype = null;
			}
		}

		public bool HasOneSelectedGroup()
		{
			return SelectedGroup != null;
		}
		
		public bool HasOneSelectedPrototype()
		{
			return SelectedPrototype != null;
		}

		private static void SetSelectedPrototypeListFromAssets<T>(IReadOnlyList<T> baseList, List<T> setPrototypeList) where T : Prototype
        {
            foreach (T asset in baseList)
            {
	            if (asset == null)
	            {
		            continue;
	            }

	            if(asset.Selected)
	            {
		            setPrototypeList.Add(asset);
	            }
            }
        }

		public List<Prototype> GetSelectedPrototypes(Type prototypeType)
		{
			foreach (var item in _selectedPrototypeTypes)
			{
				if (item.PrototypeType == prototypeType)
				{
					return item.SelectedPrototypeList;
				}
			}

			return null;
		}
		
		private void SetupSelectedPrototypeTypes()
		{
			_selectedPrototypeTypes = new List<SelectedPrototypes>();
			_selectedGroupList = new List<Group.Group>();
				
			foreach (var type in AllPrototypeTypes.TypeList)
			{
				_selectedPrototypeTypes.Add(new SelectedPrototypes(type));
			}
		}
	}
}