using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Interfaces.Infrastructure
{
    /// <summary>
    /// Пока только для управления ресурсами на одной сцене
    /// </summary>
    public interface IResourceLoader : IService
    {
        UniTask<TResouce> LoadAsync<TResouce>(string path, CancellationToken cancellationToken = default) where TResouce : Object;
    }
}