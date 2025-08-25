using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility
{
    public static class AllPrototypeTypes
    {
        public static readonly List<System.Type> TypeList;

        static AllPrototypeTypes()
        {
            var types = AllTypesDerivedFrom<Prototype>.Types
                .Where(
                    t => t.IsDefined(typeof(NameAttribute), false) && !t.IsAbstract
                );

            TypeList = types.ToList();
        }
    }
}