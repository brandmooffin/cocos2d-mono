using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core.Managers
{
    /// <summary>
    /// Wrapper for CCSimpleAudioEngine to streamline audio playback
    /// </summary>
    public class AudioManager
    {
        private static AudioManager audioManager;

        public static AudioManager Instance
        {
            get
            {
                if (audioManager == null)
                {
                    audioManager = new AudioManager();
                }
                return audioManager;
            }
        }

        public AudioManager()
        {
            LoadAudio();
        }

        private void LoadAudio()
        {
            CocosDenshion.CCSimpleAudioEngine.SharedEngine.PreloadBackgroundMusic("bgm/main");

            CocosDenshion.CCSimpleAudioEngine.SharedEngine.PreloadEffect("sound/clear");
            CocosDenshion.CCSimpleAudioEngine.SharedEngine.PreloadEffect("sound/click");
            CocosDenshion.CCSimpleAudioEngine.SharedEngine.PreloadEffect("sound/levelup");
        }

        public void PlayBackgroundMusic(string fileName, bool loop = true)
        {
            if (AppDataManager.Instance.AppSettings.IsMusicEnabled)
            {
                CocosDenshion.CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic($"bgm/{fileName}", loop);
            }
        }

        public void StopBackgroundMusic()
        {
            CocosDenshion.CCSimpleAudioEngine.SharedEngine.StopBackgroundMusic();
        }

        public void PlaySoundEffect(string fileName)
        {
            if (AppDataManager.Instance.AppSettings.IsSoundEnabled)
            {
                CocosDenshion.CCSimpleAudioEngine.SharedEngine.PlayEffect($"sound/{fileName}");
            }
        }

        public void StopSoundEffects()
        {
            CocosDenshion.CCSimpleAudioEngine.SharedEngine.StopAllEffects();
        }
    }
}
