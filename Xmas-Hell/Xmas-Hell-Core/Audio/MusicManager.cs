using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

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

        private Dictionary<string, SoundEffectInstance> _musics;

        public MusicManager()
        {
        }

        public void Initialize()
        {
            _currentState = MusicState.None;

            // Create music instances
            var menuThemeInstance = Assets.GetMusic("Audio/BGM/main-menu").CreateInstance();
            var bossIntroInstance = Assets.GetMusic("Audio/BGM/boss-intro").CreateInstance();
            var bossThemeInstance = Assets.GetMusic("Audio/BGM/boss-theme").CreateInstance();

            menuThemeInstance.IsLooped = true;
            bossIntroInstance.IsLooped = false;
            bossThemeInstance.IsLooped = true;

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
        }

        public void Update(GameTime gameTime)
        {
            if (_currentState == MusicState.BossIntro)
            {
                if (_musics["boss-intro"].State == SoundState.Stopped)
                {
                    _musics["boss-theme"].Play();
                    _currentState = MusicState.BossTheme;
                }
            }
        }
    }
}
