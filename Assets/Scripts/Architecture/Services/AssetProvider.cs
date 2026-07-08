
using UnityEngine;

namespace Architecture.Services{
	public class AssetProvider : IAssetProvider{
		public GameObject LoadAsset(string path){
			return Resources.Load<GameObject>(path);
		}

		public T LoadAsset<T>(string path) where T : Object{
			return Resources.Load<T>(path);
		}
	}
}