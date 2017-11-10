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

            XnaMediaPlayer.Stop();
            XnaMediaPlayer.Volume = 1f;
            XnaMediaPlayer.IsRepeating = true;
            XnaMediaPlayer.Play(Assets.GetMusic("main-menu"));

            _gameMusicIsPlaying = false;
            _menuMusicIsPlaying = true;
        }

        public static void PlayGameMusic(bool force = false)
        {
            if (_gameMusicIsPlaying && !force)
                return;

            XnaMediaPlayer.Stop();
            XnaMediaPlayer.Volume = 1f;
            XnaMediaPlayer.IsRepeating = true;
            XnaMediaPlayer.Play(Assets.GetMusic("boss-theme"));

            _gameMusicIsPlaying = true;
            _menuMusicIsPlaying = false;
        }
    }
}
