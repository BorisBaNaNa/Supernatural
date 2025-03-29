using Assets.Supernatural.Scripts.Interfaces.Infrastructure;
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Infrastructure.Services
{
    public class ResourcesLoader : IResourceLoader
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _loadingResources = new();
        private readonly Dictionary<string, Object> _loadedResources = new();

        public async UniTask<TResouce> LoadAsync<TResouce>(string path, CancellationToken cancellationToken = default) where TResouce : Object
        {
            if (_loadedResources.TryGetValue(path, out var loadedObj))
                return loadedObj as TResouce;

            TResouce resouce = await TryWaitLoadingAsync<TResouce>(path, cancellationToken);

            if (resouce != null)
                return resouce;

            return await LoadFromResourcesAsync<TResouce>(path, cancellationToken);
        }

        public void Dispose()
        {
            foreach (var loadedResourcePair in _loadedResources)
                Resources.UnloadAsset(loadedResourcePair.Value);

            _loadedResources.Clear();
        }

        private async UniTask<TResouce> TryWaitLoadingAsync<TResouce>(string path, CancellationToken cancellationToken) where TResouce : Object
        {
            try
            {
                if (_loadingResources.TryGetValue(path, out var loadingSemaphore))
                {
                    await loadingSemaphore.WaitAsync(cancellationToken);
                    loadingSemaphore.Release();

                    if (_loadedResources.TryGetValue(path, out var resource))
                        return resource as TResouce;
                    return null;
                }
                return null;
            }
            catch (System.OperationCanceledException) { throw; }
        }

        private async UniTask<TResouce> LoadFromResourcesAsync<TResouce>(string path, CancellationToken cancellationToken) where TResouce : Object
        {
            SemaphoreSlim loadingSemaphore = new(1);

            try
            {
                loadingSemaphore.Wait();
                _loadingResources.TryAdd(path, loadingSemaphore);
                var result = await Resources.LoadAsync<TResouce>(path)
                    .ToUniTask(cancellationToken: cancellationToken) as TResouce;

                if (result != null)
                    _loadedResources.Add(path, result);
                return result;
            }
            catch (System.OperationCanceledException) { throw; }
            finally
            {
                loadingSemaphore.Release();
                _loadingResources.TryRemove(path, out _);
            }
        }
    }
}