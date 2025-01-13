using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Utils
{
    public static class DataLoader
    {
        private static readonly Dictionary<string, ScriptableObject> _caches = new();

        public static async UniTask<T> LoadAsync<T>(string address) where T : ScriptableObject
        {
            if (_caches.TryGetValue(address, out var cached))
            {
                return cached as T;
            }

            var asset = await Addressables.LoadAssetAsync<T>(address).ToUniTask();
            _caches[address] = asset;
            return asset;
        }

        public static void ClearCache()
        {
            foreach (var asset in _caches.Values)
            {
                Addressables.Release(asset);
            }
            _caches.Clear();
        }

        //Debug
        public static int CachedCount => _caches.Count;
    }
}
