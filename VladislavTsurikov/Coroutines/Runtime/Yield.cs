using System.Collections;
using UnityEngine;
#if SCENE_MANAGER_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

namespace VladislavTsurikov.Coroutines.Runtime
{
	internal class YieldDefault : ICoroutineYield
	{
		private bool _waitNextFrame = true;

		public bool IsDone()
		{
			if (_waitNextFrame)
			{
				_waitNextFrame = false;
				return false;
			}
			
			return true;
		}
	}

	internal class YieldWaitForSeconds : ICoroutineYield
	{
		private float _timeToFinish;

		public YieldWaitForSeconds(float seconds)
		{
			_timeToFinish = Time.time + seconds;
		}

		public bool IsDone()
		{
			return _timeToFinish <= Time.time;
		}
	}

	internal class YieldCustomYieldInstruction : ICoroutineYield
	{
		private CustomYieldInstruction _customYield;
		
		public YieldCustomYieldInstruction(CustomYieldInstruction customYield)
		{
			_customYield = customYield;
		}

		public bool IsDone()
		{
			return !_customYield.keepWaiting;
		}
	}

	internal class YieldAsync : ICoroutineYield
	{
		private AsyncOperation _asyncOperation;

		public YieldAsync(AsyncOperation asyncOperation)
		{
			_asyncOperation = asyncOperation;
		}

		public bool IsDone()
		{
			return _asyncOperation.isDone;
		}
	}
	
#if SCENE_MANAGER_ADDRESSABLES
	internal class YieldAsyncOperationHandle : ICoroutineYield
	{
		private AsyncOperationHandle<SceneInstance> _asyncOperationHandle;

		public YieldAsyncOperationHandle(AsyncOperationHandle<SceneInstance> asyncOperationHandle)
		{
			_asyncOperationHandle = asyncOperationHandle;
		}

		public bool IsDone()
		{
			return _asyncOperationHandle.IsDone;
		}
	}
#endif

	internal class YieldNestedCoroutine : ICoroutineYield
	{
		private Coroutine _coroutine;

		public YieldNestedCoroutine(Coroutine coroutine)
		{
			_coroutine = coroutine;
		}

		public bool IsDone()
		{
			if (!_coroutine.MoveNextIfNecessary())
				return true;

			return false;
		}
	}
	
	internal class YieldRoutine : ICoroutineYield
	{
		private Coroutine _coroutine;

		public YieldRoutine(IEnumerator enumerator)
		{
			_coroutine = new Coroutine(enumerator);
		}

		public bool IsDone()
		{
			return !_coroutine.MoveNextIfNecessary();
		}
	}
	
	public class YieldCustom : ICoroutineYield
	{
		public delegate bool IsDoneDelegate();
		private readonly IsDoneDelegate _isDoneFunction;

		public YieldCustom(IsDoneDelegate isDoneFunction)
		{
			_isDoneFunction = isDoneFunction;
		}
		
		public bool IsDone()
		{
			return _isDoneFunction.Invoke();
		}
	}
	
	public class YieldTimerRoutine : ICoroutineYield
	{
		private Timer.Runtime.Timer _timer;

		public YieldTimerRoutine(Timer.Runtime.Timer timer)
		{
			_timer = timer;
		}

		public bool IsDone()
		{
			return _timer.IsDone;
		}
	}
}