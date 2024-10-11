using Assets.Features.Fragments.ScriptableObjectEvents;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Features.Systems
{
    public class AudioBehavior: MonoBehaviour
    {
        public CinemachineImpulseSource impulseSource; 
        public AudioSource audioSource;
        public VoidEventSO onScore;
        public AudioClip fireSound;
        private void OnEnable()
        {
            onScore.Subscribe(PlaySound);
        }

        private void OnDisable()
        {
            onScore.Unsubscribe(PlaySound);
        }

        private void PlaySound()
        {
            audioSource.PlayOneShot(fireSound);
            impulseSource.GenerateImpulse(5f);
        }
    }
}
