using XnaMediaPlayer = Microsoft.Xna.Framework.Media.MediaPlayer;

namespace XmasHell.Audio
{
    public static class MusicManager
    {
        private static bool _menuMusicIsPlaying = false;
        private static bool _gameMusicIsPlaying = false;

        public static void PlayMenuMusic()
        {
            if (_menuMusicIsPlaying)
                return;

            Assets.GetMusic("boss-theme").Stop(true);

            var mainMenuMusic = Assets.GetMusic("main-menu");
            mainMenuMusic.IsLooped = true;
            mainMenuMusic.Play();

            _gameMusicIsPlaying = false;
            _menuMusicIsPlaying = true;
        }

        public static void PlayGameMusic(bool force = false)
        {
            if (_gameMusicIsPlaying && !force)
                return;

            Assets.GetMusic("main-menu").Stop(true);

            var bossMusic = Assets.GetMusic("boss-theme");
            bossMusic.IsLooped = true;
            bossMusic.Play();

            _gameMusicIsPlaying = true;
            _menuMusicIsPlaying = false;
        }
    }
}
