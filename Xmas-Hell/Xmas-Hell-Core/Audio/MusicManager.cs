using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using XmasHell.PlayerData;

namespace XmasHell.Audio
{
    public class MusicManager
    {
        private enum MusicState
        {
            None,
            MenuTheme,
            BossIntro,
            BossTheme
        }

        private MusicState _currentState;
        private TimeSpan _introProgress;
        private Dictionary<string, SoundEffectInstance> _musics;

        public MusicManager()
        {
        }

        public void LoadContent()
        {
            _currentState = MusicState.None;

            // Create music instances
            var menuThemeInstance = Assets.GetMusic("Audio/BGM/menu-theme").CreateInstance();
            var bossIntroInstance = Assets.GetMusic("Audio/BGM/boss-intro").CreateInstance();
            var bossThemeInstance = Assets.GetMusic("Audio/BGM/boss-theme").CreateInstance();

            menuThemeInstance.IsLooped = true;
            bossIntroInstance.IsLooped = false;
            bossThemeInstance.IsLooped = true;

            menuThemeInstance.Volume = PlayerSettings.MasterVolume * PlayerSettings.MusicVolume;
            bossIntroInstance.Volume = PlayerSettings.MasterVolume * PlayerSettings.MusicVolume;
            bossThemeInstance.Volume = PlayerSettings.MasterVolume * PlayerSettings.MusicVolume;


            _musics = new Dictionary<string, SoundEffectInstance>
            {
                { "menu-theme", menuThemeInstance },
                { "boss-intro", bossIntroInstance },
                { "boss-theme", bossThemeInstance }
            };
        }

        public void StopMusic()
        {
            foreach (var pair in _musics)
                pair.Value.Stop(true);
        }

        public void PlayMenuMusic()
        {
            if (_currentState == MusicState.MenuTheme)
                return;

            StopMusic();
            _musics["menu-theme"].Play();
            _currentState = MusicState.MenuTheme;
        }

        public void PlayGameMusic(bool force = false)
        {
            if (!force && (_currentState == MusicState.BossIntro || _currentState == MusicState.BossTheme))
                return;

            StopMusic();
            _musics["boss-intro"].Play();
            _currentState = MusicState.BossIntro;

            _introProgress = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (_currentState == MusicState.BossIntro)
            {
                _introProgress += gameTime.ElapsedGameTime;

                if (_introProgress.TotalMilliseconds >= Assets.GetMusic("Audio/BGM/boss-intro").Duration.TotalMilliseconds * 0.97f)
                {
                    _musics["boss-theme"].Play();
                    _currentState = MusicState.BossTheme;
                }
            }
        }
    }
}
