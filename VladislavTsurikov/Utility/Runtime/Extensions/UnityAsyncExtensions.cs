using System;
using System.Runtime.CompilerServices;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace VladislavTsurikov.Utility.Runtime.Extensions
{
#if SCENE_MANAGER_ADDRESSABLES
    public static partial class UnityAsyncExtensions
    {
        public static AddressableAsyncOperationAwaiter GetAwaiter(this AsyncOperationHandle<SceneInstance> asyncOperation)
        {
            return new AddressableAsyncOperationAwaiter(asyncOperation);
        }
    }

    public class AddressableAsyncOperationAwaiter : INotifyCompletion
    {
        private AsyncOperationHandle<SceneInstance> _asyncOperationHandle;

        public AddressableAsyncOperationAwaiter(AsyncOperationHandle<SceneInstance> asyncOperationHandle)
        {
            _asyncOperationHandle = asyncOperationHandle;
        }
        
        public bool IsCompleted => _asyncOperationHandle.IsDone;
        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            if (_asyncOperationHandle.IsDone)
            {
                continuation();
            }
            else
            {
                _asyncOperationHandle.Completed += _ =>
                {
                    if (_asyncOperationHandle.IsDone)
                    {
                        continuation();
                    }
                };
            }
        }
    }
    
    /*public struct AddressableAsyncOperationAwaiter : ICriticalNotifyCompletion
    {
        private AsyncOperationHandle<SceneInstance> _asyncOperation;
        Action<AsyncOperationHandle<SceneInstance>> continuationAction;

        public AddressableAsyncOperationAwaiter(AsyncOperationHandle<SceneInstance> asyncOperation)
        {
            _asyncOperation = asyncOperation;
            continuationAction = null;
        }

        public bool IsCompleted => _asyncOperation.IsDone;

        public void GetResult()
        {
            if (continuationAction != null)
            {
                _asyncOperation.completed -= continuationAction;
                continuationAction = null;
                _asyncOperation = null;
            }
            else
            {
                _asyncOperation = null;
            }
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            continuationAction = PooledDelegate<AsyncOperation>.Create(continuation);
            _asyncOperation.completed += continuationAction;
        }
    }*/
#endif
}