using System.Collections.Generic;

namespace VladislavTsurikov.ComponentStack.Runtime.Core.Extensions
{
    public static class ComponentEx
    {
        public static bool IsValid(this Component component)
        {
            if (component == null || !component.Active)
            {
                return false;
            }

            return true;
        }
    }
}