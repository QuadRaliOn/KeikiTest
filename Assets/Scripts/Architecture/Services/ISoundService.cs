using UnityEngine;

namespace Architecture.Services
{
    public interface ISoundService
    {
        float PlayAudio(string path);
        float PlayRandomPhrase(string[] phrases);
        void StopAudio();
    }
}
