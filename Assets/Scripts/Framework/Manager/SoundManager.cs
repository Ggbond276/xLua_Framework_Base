using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Manager
{
    // FrameWorkBootstrap初始化 然后注入GameManager
    [XLua.LuaCallCSharp]
    public class SoundManager : MonoBehaviour
    {

        AudioSource m_MusicAudio; // 你把这个作为一个
        AudioSource m_SoundAudio;


        private float SoundVolume
        {
            get { return PlayerPrefs.GetFloat("SoundVolume", 1.0f);  }
            set
            {
                m_SoundAudio.volume = value;
                PlayerPrefs.SetFloat("SoundVolume", value);
            }
        }

        private float MusicVolume
        {
            get { return PlayerPrefs.GetFloat("MusicVolume", 1.0f); }
            set
            {
                m_MusicAudio.volume = value;
                PlayerPrefs.SetFloat("MusicVolume", value);
            }
        }

        public void Init()
        {
            m_MusicAudio = this.gameObject.AddComponent<AudioSource>();
            m_MusicAudio.playOnAwake = false;
            m_MusicAudio.loop = true;

            m_SoundAudio = this.gameObject.AddComponent<AudioSource>();
            m_SoundAudio.loop = false;
        }
        
        public void PlayMusic(string name)
        {
            if (this.MusicVolume < 0.1f) // 如果音量小于0.1 直接不播放
                return;
            

            string oldName = "";
            if(m_MusicAudio.clip != null)  // 获取旧唱片的名字
                oldName = m_MusicAudio.clip.name;

            if (oldName == name) // 与新唱片的名字进行比对 如果一样不用切歌 
            {
                m_MusicAudio.Play();
                return;
            }

            
            GameManager.Resources.LoadMusic(name, (UnityEngine.Object obj) => // 与新唱片名字一样 切歌
            {
                m_MusicAudio.clip = obj as AudioClip;
                m_MusicAudio.Play();
            });
        }


        public void PauseMusic()
        {
            m_MusicAudio.Pause();
        }

        public void OnUnPauseMusic()
        {
            m_MusicAudio.UnPause();
        }

        public void StopMusic()
        {
            m_MusicAudio.Stop();
        }

        public void PlaySound(string name)
        {
            if (this.SoundVolume < 0.1f)
                return;

            GameManager.Resources.LoadSound(name, (UnityEngine.Object obj) => {
                m_SoundAudio.PlayOneShot(obj as AudioClip);
            });
        }


        public void SetMusicVolume(float value)
        {
            this.MusicVolume = value;
        }

        public void SetSoundVolume(float value)
        {
            this.SoundVolume = value;
        }
    }
}
