using System.Collections;

namespace VladislavTsurikov.Coroutines.Runtime
{
    public static class IEnumeratorExtension
    {
        public static string MethodName(this IEnumerator routine)
        {
            if (routine != null)
            {
                string[] split = routine.ToString().Split('<', '>');
                if (split.Length == 3)
                {
                    return split[1];
                }
            }

            return "";
        }
    }
}