using UnityEngine;

namespace Architecture.Services {
    public interface IAssetProvider {
        GameObject LoadAsset(string path);
        T LoadAsset<T>(string path) where T : Object;
    }
}