using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility
{
    public static class AllAvailableGroups
	{
        private static readonly List<Group> _groupList = new List<Group>();

        static AllAvailableGroups()
        {
            if (Resources.FindObjectsOfTypeAll(typeof(Group)) is Group[] groups)
            {
                foreach (Group group in groups)
                {
                    AddGroup(group);
                }
            }
        } 

        public static List<Group> GetAllGroups()
        {
            return _groupList;
        }

        public static void AddGroup(Group group)
        {
            if(!_groupList.Contains(group))
            {
                _groupList.Add(group);
            }
        }
	}
}