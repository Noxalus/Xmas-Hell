using Microsoft.Xna.Framework;

namespace XmasHell.Screens
{
    public abstract class Screen
    {
        public readonly string Name;

        protected XmasHell Game;
        protected bool IsVisible;
        private bool _neverShown;
        protected bool ShouldBeStackInHistory;

        protected bool UIReseted = false;

        public bool StackInHistory => ShouldBeStackInHistory;

        public Screen(XmasHell game)
        {
            Game = game;
            _neverShown = true;
            ShouldBeStackInHistory = true;
            Name = GetType().Name;
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
            IsVisible = true;

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
            IsVisible = false;
            UIReseted = false;

            // Remove elements from the SpriteBatchManager
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
