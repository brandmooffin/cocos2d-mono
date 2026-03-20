using System;
using Cocos2D;
using CocosDenshion;

namespace tests
{
    /// <summary>
    /// Extended audio tests covering new features: per-effect volume, throttled playback, and music fade.
    /// </summary>
    public class CocosDenshionExtendedTest : CCLayer
    {
        string EFFECT_FILE = "Sounds/effect1";
        string MUSIC_FILE = "Sounds/background";
        int LINE_SPACE = 40;

        CCMenu m_pItmeMenu;
        CCPoint m_tBeginPos;
        int m_nTestCount;
        CCLabelTTF _statusLabel;

        public CocosDenshionExtendedTest()
        {
            m_pItmeMenu = null;
            m_tBeginPos = new CCPoint(0, 0);

            string[] testItems = {
                "play effect at 100% volume",
                "play effect at 50% volume",
                "play effect at 25% volume",
                "play effect throttled (0.5s min)",
                "play effect throttled (rapid fire)",
                "play background music",
                "fade music to 0% (2s)",
                "fade music to 100% (2s)",
                "fade music to 50% (1s)",
                "stop background music",
            };

            m_pItmeMenu = new CCMenu(null);
            CCSize s = CCDirector.SharedDirector.WinSize;
            m_nTestCount = testItems.Length;

            for (int i = 0; i < m_nTestCount; ++i)
            {
                CCLabelTTF label = new CCLabelTTF(testItems[i], "arial", 24);
                CCMenuItemLabel pMenuItem = new CCMenuItemLabel(label, menuCallback);
                m_pItmeMenu.AddChild(pMenuItem, i + 10000);
                pMenuItem.Position = new CCPoint(s.Width / 2, (s.Height - (i + 1) * LINE_SPACE));
            }

            m_pItmeMenu.ContentSize = new CCSize(s.Width, (m_nTestCount + 1) * LINE_SPACE);
            m_pItmeMenu.Position = new CCPoint(0, 0);
            AddChild(m_pItmeMenu);

            _statusLabel = new CCLabelTTF("Ready", "arial", 18);
            _statusLabel.Position = new CCPoint(s.Width / 2, 50);
            AddChild(_statusLabel, 100);

            this.TouchEnabled = true;

            CCSimpleAudioEngine.SharedEngine.PreloadBackgroundMusic(CCFileUtils.FullPathFromRelativePath(MUSIC_FILE));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(CCFileUtils.FullPathFromRelativePath(EFFECT_FILE));
            CCSimpleAudioEngine.SharedEngine.EffectsVolume = 1.0f;
            CCSimpleAudioEngine.SharedEngine.BackgroundMusicVolume = 1.0f;

            Schedule(UpdateAudio);
        }

        private void UpdateAudio(float dt)
        {
            CCSimpleAudioEngine.SharedEngine.Update(dt);
        }

        public override void OnExit()
        {
            base.OnExit();
            CCSimpleAudioEngine.SharedEngine.End();
        }

        public void menuCallback(object pSender)
        {
            CCMenuItem pMenuItem = (CCMenuItem)(pSender);
            int nIdx = pMenuItem.ZOrder - 10000;

            string effectPath = CCFileUtils.FullPathFromRelativePath(EFFECT_FILE);
            string musicPath = CCFileUtils.FullPathFromRelativePath(MUSIC_FILE);

            switch (nIdx)
            {
                // Play effect at 100% volume
                case 0:
                    CCSimpleAudioEngine.SharedEngine.PlayEffect(effectPath, 1.0f);
                    _statusLabel.Text = "Playing effect at 100% volume";
                    break;

                // Play effect at 50% volume
                case 1:
                    CCSimpleAudioEngine.SharedEngine.PlayEffect(effectPath, 0.5f);
                    _statusLabel.Text = "Playing effect at 50% volume";
                    break;

                // Play effect at 25% volume
                case 2:
                    CCSimpleAudioEngine.SharedEngine.PlayEffect(effectPath, 0.25f);
                    _statusLabel.Text = "Playing effect at 25% volume";
                    break;

                // Play effect throttled (0.5s interval)
                case 3:
                    CCSimpleAudioEngine.SharedEngine.PlayEffect(effectPath, 1.0f, 0.5f);
                    _statusLabel.Text = "Throttled play (0.5s). Spam-click to test.";
                    break;

                // Play effect throttled (rapid fire - should skip some)
                case 4:
                    CCSimpleAudioEngine.SharedEngine.PlayEffect(effectPath, 1.0f, 0.5f);
                    _statusLabel.Text = "Throttled rapid fire. Some plays should be skipped.";
                    break;

                // Play background music
                case 5:
                    CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic(musicPath, true);
                    _statusLabel.Text = "Background music playing";
                    break;

                // Fade music to 0%
                case 6:
                    CCSimpleAudioEngine.SharedEngine.FadeBackgroundMusic(0f, 2f);
                    _statusLabel.Text = "Fading music to 0% over 2s";
                    break;

                // Fade music to 100%
                case 7:
                    CCSimpleAudioEngine.SharedEngine.FadeBackgroundMusic(1f, 2f);
                    _statusLabel.Text = "Fading music to 100% over 2s";
                    break;

                // Fade music to 50%
                case 8:
                    CCSimpleAudioEngine.SharedEngine.FadeBackgroundMusic(0.5f, 1f);
                    _statusLabel.Text = "Fading music to 50% over 1s";
                    break;

                // Stop music
                case 9:
                    CCSimpleAudioEngine.SharedEngine.StopBackgroundMusic();
                    _statusLabel.Text = "Music stopped";
                    break;
            }
        }

        public override void TouchesBegan(System.Collections.Generic.List<CCTouch> pTouches)
        {
            CCTouch touch = pTouches[0];
            m_tBeginPos = touch.Location;
        }

        public override void TouchesMoved(System.Collections.Generic.List<CCTouch> pTouches)
        {
            CCTouch touch = pTouches[0];
            CCPoint touchLocation = touch.LocationInView;
            touchLocation = CCDirector.SharedDirector.ConvertToGl(touchLocation);
            float nMoveY = touchLocation.Y - m_tBeginPos.Y;

            CCPoint curPos = m_pItmeMenu.Position;
            CCPoint nextPos = new CCPoint(curPos.X, curPos.Y + nMoveY);
            CCSize winSize = CCDirector.SharedDirector.WinSize;

            if (nextPos.Y < 0.0f)
            {
                m_pItmeMenu.Position = new CCPoint(0, 0);
                return;
            }
            if (nextPos.Y > ((m_nTestCount + 1) * LINE_SPACE - winSize.Height))
            {
                m_pItmeMenu.Position = new CCPoint(0, ((m_nTestCount + 1) * LINE_SPACE - winSize.Height));
                return;
            }

            m_pItmeMenu.Position = nextPos;
            m_tBeginPos = touchLocation;
        }
    }
}
