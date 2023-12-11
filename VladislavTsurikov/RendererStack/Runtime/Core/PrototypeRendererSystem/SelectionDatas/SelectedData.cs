using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas
{
    public class SelectedData
	{
		public List<Group> SelectedGroupList = new List<Group>();
		public List<Prototype> SelectedProtoList = new List<Prototype>();

		public void RefreshSelectedParameters(SelectionData selectionData)
		{
			ClearSelectedList();
            SetSelectedList(selectionData);
		}

		private void ClearSelectedList()
		{
			SelectedProtoList.Clear();
			SelectedGroupList.Clear();
		}

		private void SetSelectedList(SelectionData selectionData)
		{
			foreach (Group group in selectionData.GroupList)
			{
				if(group.Selected)
		    	{
					SelectedGroupList.Add(group);
		    	}
			}

			foreach (Prototype proto in selectionData.PrototypeList)
			{
				if(proto.Selected)
		    	{
					SelectedProtoList.Add(proto);
		    	}
			}
		}

		public bool HasOneSelectedProto()
		{
			return SelectedProtoList.Count == 1;
		}

		public bool HasOneSelectedGroup()
		{
			return SelectedGroupList.Count == 1;
		}

		public Prototype GetLastPrototype()
		{
			return SelectedProtoList.Last();
		}

		public Group GetLastGroup()
		{
			if(SelectedGroupList.Count == 0)
			{
				return null;
			}
			
			return SelectedGroupList.Last();
		}
	}
}