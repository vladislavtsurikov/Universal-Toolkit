#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.Coroutines.Runtime;

namespace VladislavTsurikov.Coroutines.Editor
{
	public static class EditorCoroutineExtensions
	{
		public static void StopCoroutine(this EditorWindow thisRef, Coroutine routine)
		{
			CoroutineRunner.StopCoroutine(routine);
		}

		public static void StopAllCoroutines(this EditorWindow thisRef)
		{
			CoroutineRunner.StopCoroutines(thisRef);
		}
	}
}
#endif