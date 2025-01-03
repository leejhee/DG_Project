using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// AudioPlayer ���� AudioSystem 
    /// </summary>
    public class AudioSystem 
    {
        private static AudioSystem _instance    = null; // AudioSystem ��ü
        private AudioPlayer[]      _audioPlayer = null; // AudioPlayer Systems
        private GameObject         _audioRoot   = null; // AudioRoot

        #region ������
        AudioSystem() { }
        #endregion ������

        /// <summary>
        /// AudioSystem ��ü
        /// </summary>
        public static AudioSystem Instance { get { Init(); return _instance; } }

        /// <summary>
        /// �ʱ�ȭ
        /// </summary>
        private static void Init()
        {
            if (_instance == null)
            {
                _instance = new AudioSystem();
                _instance._audioPlayer = new AudioPlayer[(int)eSounds.MaxCount];
                _instance._audioRoot = new GameObject { name = "AudioRoot" };
                GameObject.DontDestroyOnLoad(_instance._audioRoot);
                for (eSounds sound = 0; sound < eSounds.MaxCount; sound++)
                {
                    GameObject audioObject = new GameObject { name = $"{sound}Player" };
                    AudioPlayer audioPlayer = audioObject.AddComponent<AudioPlayer>();
                    audioPlayer.soundType = sound;
                    if (sound == eSounds.BGM)
                    {
                        audioPlayer.audioSource.loop = true;
                    }
                    audioPlayer.SetVolume(AudioManager.Instance.GetValue(sound));
                    _instance._audioPlayer[(int)sound] = audioPlayer;
                    audioObject.transform.parent = _instance._audioRoot.transform;
                }
            }
        }

        /// <summary>
        /// ����� �÷��̾� 
        /// </summary>
        /// <param name="audioType"></param>
        /// <returns></returns>
        public AudioPlayer GetAudioPlayer(eSounds audioType)
        {
            return Instance._audioPlayer[(int)audioType];
        }


    }
}