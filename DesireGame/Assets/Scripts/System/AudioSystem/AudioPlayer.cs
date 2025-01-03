using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// ���� ����� �÷��̾�
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] 
        private AudioSource _audioSource = new AudioSource(); // ����� �ҽ�
        [SerializeField] 
        private AudioClip   _audioClip   = null;              // ����� Ŭ��
        [SerializeField]
        private eSounds      _soundType   = eSounds.MaxCount;   // ����� Ÿ��

        public AudioSource audioSource { get { return GetAudioSource(); } }                // ����� �ҽ�
        public eSounds soundType { get { return _soundType; } set { _soundType = value; } } // ����� Ÿ��

        /// <summary>
        /// Get AudioSource 
        /// </summary>
        /// <returns></returns>
        public AudioSource GetAudioSource()
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            return _audioSource;
        }

        /// <summary>
        /// ���� �÷��� ��Ʈ�� ���� AudioClip
        /// </summary>
        /// <returns></returns>
        public AudioClip GetAudioClip()
        {
            return _audioClip;
        }

        /// <summary>
        /// PlayOneShot (AudioClip ���� X)
        /// </summary>
        /// <param name="audioClip"></param>
        public void PlayAudioOneShot(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }

        /// <summary>
        /// Play (AudioClip ���� O)
        /// </summary>
        /// <param name="audioClip"></param>
        public void PlayAudio(AudioClip audioClip)
        {
            _audioClip = audioClip;
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        /// <summary>
        /// Stop AudioSource
        /// </summary>
        public void StopAudio()
        {
            _audioSource.Stop();
            _audioClip = null;
            _audioSource.clip = null;
        }

        /// <summary>
        /// PauseaudioSource
        /// </summary>
        public void PauseAudio()
        {
            _audioSource.Pause();
        }

        public void SetVolume(float volume)
        {
            _audioSource.volume = volume;
        }

    }
}