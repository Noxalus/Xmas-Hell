using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace XmasHell.GUI
{
    public class GuiManager
    {
        private XmasHell _game;
        private List<GuiButton> _buttons;

        public GuiManager(XmasHell game)
        {
            _game = game;
            _buttons = new List<GuiButton>();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Update(gameTime);
            }
        }

        public void AddButton(GuiButton button)
        {
            _buttons.Add(button);
            _game.SpriteBatchManager.UISprites.Add(button.Sprite);
        }

        public void RemoveButton(GuiButton button)
        {
            _game.SpriteBatchManager.UISprites.Remove(button.Sprite);
            _buttons.Remove(button);
        }

        public void RemoveButton(String buttonName)
        {
            var buttons = _buttons.FindAll(b => b.Name == buttonName);

            foreach (var button in buttons)
                RemoveButton(button);
        }
    }
}
