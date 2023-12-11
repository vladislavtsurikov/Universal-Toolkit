using System.Collections;
using System.Collections.Generic;
using VladislavTsurikov.Runtime;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.Coroutines.Runtime
{
	public static class CoroutineRunner
	{
        private static readonly List<Coroutine> _coroutines = new List<Coroutine>();

        public static Coroutine StartCoroutine(IEnumerator routine, object obj)
        {
            if (_coroutines.Count == 0)
            {
                EditorAndRuntimeUpdate.AddListener(OnUpdate);
            }
            
            Coroutine coroutine = new Coroutine(routine, obj);

            if (coroutine.MoveNextIfNecessary())
            {
                _coroutines.Add(coroutine);
            }
            
            return coroutine;
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (_coroutines.Count == 0)
            {
                EditorAndRuntimeUpdate.AddListener(OnUpdate);
            }
            
            Coroutine coroutine = new Coroutine(routine);

            if (coroutine.MoveNextIfNecessary())
            {
                _coroutines.Add(coroutine);
            }

            return coroutine;
        }
        
        public static void StopCoroutine(Coroutine routine)
        {
            for (int j = _coroutines.Count - 1; j >= 0; j--)
            {
                Coroutine coroutine = _coroutines[j];

                if (coroutine == routine)
                {
                    _coroutines.RemoveAt(j);
                    return;
                }
            }
        }

        public static void StopCoroutine(IEnumerator routine)
        {
            for (int j = _coroutines.Count - 1; j >= 0; j--)
            {
                Coroutine coroutine = _coroutines[j];

                if (coroutine.Routine.MethodName() == routine.MethodName())
                {
                    _coroutines.RemoveAt(j);
                }
            }
        }
        
        public static void StopCoroutine(IEnumerator routine, object reference)
        {
            for (int j = _coroutines.Count - 1; j >= 0; j--)
            {
                Coroutine coroutine = _coroutines[j];

                if (coroutine.Owner == reference)
                {
                    if (coroutine.Routine.MethodName() == routine.MethodName())
                    {
                        _coroutines.RemoveAt(j);
                    }
                }
            }
        }

        public static void StopCoroutines(object reference)
        {
            for (int j = _coroutines.Count - 1; j >= 0; j--)
            {
                Coroutine coroutine = _coroutines[j];

                if (coroutine.Owner == reference)
                {
                    _coroutines.RemoveAt(j);
                }
            }
        }
        
        public static void StopAllCoroutines()
        {
            _coroutines.Clear();
        }

        private static void OnUpdate()
        {
            if (_coroutines.Count == 0)
            {
                EditorAndRuntimeUpdate.RemoveListener(OnUpdate);
                return;
            }

            for (int j = _coroutines.Count - 1; j >= 0; j--)
            {
                Coroutine coroutine = _coroutines[j];

                if (coroutine.Finished || !coroutine.MoveNextIfNecessary())
                {
                    _coroutines.RemoveAt(j);
                    coroutine.CurrentYield = null;
                    coroutine.Finished = true;
                }
            }
        }
	}
}