using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmasHell.Screens
{
    public class ScreenManager
    {
        private XmasHell _game;
        private List<Screen> _screens;
        private Screen _currentScreen;
        private Screen _previousScreen;

        public ScreenManager(XmasHell game)
        {
            _game = game;
            _screens = new List<Screen>();
            _currentScreen = null;
            _previousScreen = null;
        }

        public void AddScreen(Screen screen)
        {
            if (_screens.Contains(screen))
                throw new Exception("This screen already exists!");

            _screens.Add(screen);
        }

        public T GetScreen<T>() where T : Screen
        {
            return _screens.OfType<T>().FirstOrDefault();
        }

        public void GoTo<T>() where T : Screen
        {
            var screen = GetScreen<T>();

            if (screen == null)
                throw new InvalidOperationException($"{typeof(T).Name} not registered");

            GoTo(screen);
        }

        private void GoTo(Screen screen)
        {
            _currentScreen?.Hide();
            _previousScreen = _currentScreen;
            _currentScreen = screen;
            _currentScreen.Show();
        }

        public void Back()
        {
            if (_previousScreen != null)
                GoTo(_previousScreen);
        }

        public void Update(GameTime gameTime)
        {
            _currentScreen?.Update(gameTime);
        }
    }
}
