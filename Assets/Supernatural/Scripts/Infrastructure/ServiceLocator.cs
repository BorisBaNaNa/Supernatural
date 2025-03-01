using UnityEngine;

namespace Assets.Supernatural.Scripts.Infrastructure
{
    public class ServiceLocator : MonoBehaviour
    {
        public static void RegService<TService>(TService serviceObj) where TService : IService
            => Implement<TService>.Instance = serviceObj;

        public static TService GetService<TService>() where TService : IService
            => Implement<TService>.Instance;

        public static void DisposeService<TService>() where TService : IService
        {
            if (Implement<TService>.Instance == null)
                return;

            Implement<TService>.Instance.Dispose();
            Implement<TService>.Instance = default;
        }

        private class Implement<TService> where TService : IService
        {
            public static TService Instance;
        }
    }

    public interface IService
    {
        void Dispose();
    };
}