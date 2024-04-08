using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.Utility.Runtime.Extensions;
#if SCENE_MANAGER_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

namespace VladislavTsurikov.Coroutines.Runtime
{
	public class Coroutine
	{
		public ICoroutineYield CurrentYield;
        public IEnumerator Routine;
        public bool Finished;
        
        public WeakReference Owner;
        
        internal Coroutine(IEnumerator routine)
        {
	        Owner = null;
            Routine = routine;
            Routine.MoveNext();
            SetCurrentYield();
        }

        internal Coroutine(IEnumerator routine, object owner)
        {
	        Owner = new WeakReference(owner);
	        Routine = routine;
	        Routine.MoveNext();
	        SetCurrentYield();
        }

        private void SetCurrentYield()
        {
	        object current = Routine.Current;
	        if (current == null)
	        {
		        CurrentYield = new YieldDefault();
	        }
	        else if (current is YieldCustom yieldCustom)
	        {
		        CurrentYield = yieldCustom;
	        }
	        else if (current is ICoroutineYield coroutineYield)
	        {
		        CurrentYield = coroutineYield;
	        }
	        else if (current is WaitForFixedUpdate || current is WaitForEndOfFrame)
	        {
		        CurrentYield = new YieldDefault();
	        }
	        else if(current is WaitForSeconds)
	        {
		        float seconds = float.Parse(GetInstanceField(typeof(WaitForSeconds), current, "m_Seconds").ToString());
		        CurrentYield = new YieldWaitForSeconds(seconds);
	        }
	        else if(current is CustomYieldInstruction customYieldInstruction)
	        {
		        CurrentYield = new YieldCustomYieldInstruction(customYieldInstruction);
	        }
	        else if(current is AsyncOperation asyncOperation)
	        {
		        CurrentYield = new YieldAsync(asyncOperation);
	        }
#if SCENE_MANAGER_ADDRESSABLES
	        else if(current is AsyncOperationHandle<SceneInstance> asyncOperationHandle)
	        {
		        CurrentYield = new YieldAsyncOperationHandle(asyncOperationHandle);
	        }
#endif
	        else if(current is Coroutine coroutine)
	        {
		        CurrentYield = new YieldNestedCoroutine(coroutine);
	        }
	        else if (current is IEnumerator enumerator)
	        {
		        CurrentYield = new YieldRoutine(enumerator);
	        }
	        else
	        {
		        Debug.LogException(
			        new Exception("<" + Routine.MethodName() + "> yielded an unknown or unsupported type! (" + current.GetType() + ")"),
			        null);
		        
		        CurrentYield = new YieldDefault();
	        }
        }
        
        internal bool MoveNext()
        {
	        if (CurrentYield == null)
	        {
		        return false;
	        }

	        if (CurrentYield.IsDone())
	        {
		        if (Routine.MoveNext())
		        {
			        SetCurrentYield();

			        if (MoveNext())
			        {
				        return true;
			        }
			        else
			        {
				        return false;
			        }
		        }
		        else
		        {
			        return false;
		        }
	        }
	        else
	        {
		        return true;
	        }
        }
        
        private static object GetInstanceField(Type type, object instance, string fieldName)
        {
	        BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
	        FieldInfo field = type.GetField(fieldName, bindFlags);
	        return field.GetValue(instance);
        }

        public void Cancel()
        {
	        Finished = true;
        }
	}
}