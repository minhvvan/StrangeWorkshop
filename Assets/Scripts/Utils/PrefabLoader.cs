using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Utils
{
    public static class PrefabLoader
    {
        private static readonly Dictionary<string, GameObject> _caches = new();

        public static async UniTask<GameObject> LoadAsync(string address)
        {
            if (_caches.TryGetValue(address, out var cached))
            {
                return cached;
            }

            var asset = await Addressables.LoadAssetAsync<GameObject>(address).ToUniTask();
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