using Microsoft.Xna.Framework;

namespace XmasHell.Background
{
    public abstract class AbstractBackground
    {
        protected XmasHell Game;

        protected AbstractBackground(XmasHell game)
        {
            Game = game;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw();
    }
}
