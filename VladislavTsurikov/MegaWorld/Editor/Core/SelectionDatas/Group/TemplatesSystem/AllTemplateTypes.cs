#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem
{
	public static class AllTemplateTypes 
    {
        public static List<System.Type> TypeList = new List<System.Type>();

        static AllTemplateTypes()
        {
            var types = AllTypesDerivedFrom<Template>.TypeList
                                .Where(
                                    t => t.IsDefined(typeof(TemplateAttribute), false)
                                      && !t.IsAbstract
                                ); 

            foreach (var type in types)
            {
                TypeList.Add(type);
            }
        }
    }
}
#endif