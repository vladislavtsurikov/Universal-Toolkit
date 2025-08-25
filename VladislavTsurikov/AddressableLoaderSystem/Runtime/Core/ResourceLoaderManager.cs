using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Utility.Runtime;
using UnityEngine;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public class ResourceLoaderManager
    {
        private readonly DiContainer _container;
        private readonly List<ResourceLoader> _allLoaders = new();
        private readonly HashSet<ResourceLoader> _activeLoaders = new();

        public IReadOnlyList<ResourceLoader> GetAllLoaders() => _allLoaders;

        public ResourceLoaderManager(DiContainer container)
        {
            _container = container;
            LoaderRegistrarUtility.RegisterLoaderInitializers(this);
        }

        public async UniTask Load(Func<FilterAttribute, bool> attributePredicate, CancellationToken cancellationToken = default)
        {
            var selected = new HashSet<ResourceLoader>();

            foreach (var loader in _allLoaders)
            {
                var attributes = loader.GetType()
                    .GetCustomAttributes(typeof(FilterAttribute), true)
                    .Cast<FilterAttribute>()
                    .ToArray();

                if (attributes.Any(attributePredicate))
                {
                    selected.Add(loader);
                }
            }

#if ADDRESSABLE_LOADER_LOGS
            Debug.Log($"[ResourceLoaderManager] Start Loading" +
                      $" \nSelected loaders to load: {string.Join(", ", selected.Select(l => l.GetType().Name))}");
#endif

            await UnloadObsoleteLoaders(selected, cancellationToken);
            await LoadMissingLoaders(selected, cancellationToken);

            _activeLoaders.Clear();

            foreach (var loader in selected)
            {
                _activeLoaders.Add(loader);
            }

#if ADDRESSABLE_LOADER_LOGS
            Debug.Log($"[ResourceLoaderManager] End Loading");
#endif
        }
        
        internal bool Register(ResourceLoader loader)
        {
            var type = loader.GetType();

            if (_allLoaders.Any(l => l.GetType() == type))
            {
                return false;
            }

            _allLoaders.Add(loader);
            _container.Bind(type).FromInstance(loader).AsSingle();

            return true;
        }

        private async UniTask UnloadObsoleteLoaders(HashSet<ResourceLoader> desired, CancellationToken cancellationToken)
        {
            var toUnload = _activeLoaders.Except(desired);
            foreach (var loader in toUnload)
            {
                await loader.Unload(cancellationToken);
            }
        }

        private async UniTask LoadMissingLoaders(HashSet<ResourceLoader> desired, CancellationToken cancellationToken)
        {
            var toLoad = desired.Except(_activeLoaders);

#if ADDRESSABLE_LOADER_LOGS
            var totalLoadTime = System.Diagnostics.Stopwatch.StartNew();
#endif

            var loadTasks = new List<UniTask>();

            foreach (var loader in toLoad)
            {
                var task = UniTask.Create(async () =>
                {
#if ADDRESSABLE_LOADER_LOGS
                    var sw = System.Diagnostics.Stopwatch.StartNew();
#endif
                    try
                    {
                        await loader.LoadResourceLoader(cancellationToken);
#if ADDRESSABLE_LOADER_LOGS
                        sw.Stop();
                        Debug.Log($"[ResourceLoaderManager] {loader.GetType().Name} loaded in {sw.Elapsed.ToReadableString()}");
#endif
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[ResourceLoaderManager] Exception while loading {loader.GetType().Name}: {ex}");
                    }
                });

                loadTasks.Add(task);
            }

            await UniTask.WhenAll(loadTasks);

#if ADDRESSABLE_LOADER_LOGS
            totalLoadTime.Stop();
            Debug.Log($"[ResourceLoaderManager] Total parallel loading time: {totalLoadTime.Elapsed.ToReadableString()}");
#endif
        }
    }
}