using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace XmasHell.Screens
{
    public abstract class Screen
    {
        protected XmasHell Game;
        private bool _neverShown;

        public Screen(XmasHell game)
        {
            Game = game;
            _neverShown = true;
        }

        public virtual void Initialize()
        {
            // Reset the initial state of the screen
        }

        public virtual void LoadContent()
        {
            // Load content used only by this screen
        }

        public virtual void Show(bool reset = false)
        {
            if (_neverShown)
            {
                LoadContent();
                Initialize();

                _neverShown = false;
            }

            if (reset)
                Initialize();

            // Add elements to the SpriteBatchManager
        }

        public virtual void Hide()
        {
            // Remove elements from the SpriteBatchManager
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
