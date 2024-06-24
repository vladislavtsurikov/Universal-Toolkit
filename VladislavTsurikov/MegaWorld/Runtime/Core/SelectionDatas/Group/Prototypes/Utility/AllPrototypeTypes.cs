using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility
{
    public static class AllPrototypeTypes
    {
        public static readonly List<System.Type> TypeList;

        static AllPrototypeTypes()
        {
            var types = AllTypesDerivedFrom<Prototype>.TypeList
                .Where(
                    t => t.IsDefined(typeof(MenuItemAttribute), false) && !t.IsAbstract
                );

            TypeList = types.ToList();
        }
    }
}