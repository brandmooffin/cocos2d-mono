using System;
using Microsoft.Xna.Framework.Audio;
using Cocos2D;

namespace CocosDenshion
{
    public class CCEffectPlayer
    {
        public static ulong s_mciError;
        private SoundEffect m_effect;
        private SoundEffectInstance _sfxInstance;
        private int m_nSoundId;

        public CCEffectPlayer()
        {
            m_nSoundId = 0;
        }

        public static float Volume
        {
            get { return SoundEffect.MasterVolume; }
            set
            {
                if (value >= 0.0f && value <= 1.0f)
                {
                    SoundEffect.MasterVolume = value;
                }
            }
        }

        ~CCEffectPlayer()
        {
            Close();
        }

        public void Open(string pFileName, int uId)
        {
            if (string.IsNullOrEmpty(pFileName))
            {
                return;
            }

            Close();

            try
            {
                m_effect = CCContentManager.SharedContentManager.Load<SoundEffect>(pFileName);
            }
            catch (Exception)
            {
                string srcfile = pFileName;
                if (srcfile.IndexOf('.') > -1)
                {
                    srcfile = srcfile.Substring(0, srcfile.LastIndexOf('.'));
                    m_effect = CCContentManager.SharedContentManager.Load<SoundEffect>(srcfile);
                }
            }
            // Do not get an instance here b/c it is very slow. 
            //_sfxInstance = m_effect.CreateInstance();
            m_nSoundId = uId;
        }

        public void Play(bool bLoop)
        {
            if (null == m_effect)
            {
                return;
            }
            if (bLoop)
            {
                // If looping, then get an instance of this sound effect so that it can be
                // stopped.
                _sfxInstance = m_effect.CreateInstance();
                _sfxInstance.IsLooped = true;
            }
            if (_sfxInstance != null)
            {
                _sfxInstance.Play();
            }
            else
            {
                m_effect.Play();
            }
        }

        public void Play()
        {
            Play(false);
        }

        /// <summary>
        /// Plays the sound effect with per-instance volume control.
        /// </summary>
        /// <param name="bLoop">Whether to loop the sound.</param>
        /// <param name="volume">Volume from 0.0 to 1.0.</param>
        public void Play(bool bLoop, float volume)
        {
            if (null == m_effect)
            {
                return;
            }

            _sfxInstance = m_effect.CreateInstance();
            _sfxInstance.IsLooped = bLoop;
            _sfxInstance.Volume = Math.Max(0f, Math.Min(1f, volume));
            _sfxInstance.Play();
        }

        public void Close()
        {
            Stop();

            m_effect = null;
        }

        public void Pause()
        {
            if (_sfxInstance != null && !_sfxInstance.IsDisposed && _sfxInstance.State == SoundState.Playing)
            {
                _sfxInstance.Pause();
            }
//            CCLog.Log("Pause is invalid for sound effect");
        }

        public void Resume()
        {
            if (_sfxInstance != null && !_sfxInstance.IsDisposed && _sfxInstance.State == SoundState.Paused)
            {
                _sfxInstance.Play();
            }
//            CCLog.Log("Resume is invalid for sound effect");
        }

        public void Stop()
        {
            if (_sfxInstance != null && !_sfxInstance.IsDisposed && _sfxInstance.State == SoundState.Playing)
            {
                _sfxInstance.Stop();
            }
//            CCLog.Log("Stop is invalid for sound effect");
        }

        public void Rewind()
        {
            CCLog.Log("Rewind is invalid for sound effect");
        }

        public bool IsPlaying()
        {
            if (_sfxInstance != null)
            {
                return (_sfxInstance.State == SoundState.Playing);
            }
//            CCLog.Log("IsPlaying is invalid for sound effect");
            return false;
        }

        public int SoundID
        {
            get { return m_nSoundId; }
        }

        // the volume is gloabal, it will affect other effects' volume
    }
}