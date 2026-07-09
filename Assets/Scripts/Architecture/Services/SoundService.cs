using UnityEngine;

namespace Architecture.Services
{
    public class SoundService : ISoundService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly AudioSource _audioSource;

        public SoundService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;

            GameObject soundGo = new GameObject("SoundService_AudioSource");
            Object.DontDestroyOnLoad(soundGo);
            _audioSource = soundGo.AddComponent<AudioSource>();
        }

        public float PlayAudio(string path)
        {
            if (string.IsNullOrEmpty(path)) return 0f;

            AudioClip clip = _assetProvider.LoadAsset<AudioClip>(path);
            if (clip != null)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                return clip.length;
            }
            return 0f;
        }

        public float PlayRandomPhrase(string[] phrases)
        {
            if (phrases == null || phrases.Length == 0) return 0f;

            string selectedPhrase = phrases[Random.Range(0, phrases.Length)];
            return PlayAudio($"Sounds/{selectedPhrase}");
        }
    }
}
