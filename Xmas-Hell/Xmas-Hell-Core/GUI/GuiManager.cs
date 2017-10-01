using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace XmasHell.GUI
{
    public class GuiManager
    {
        private XmasHell _game;
        private List<AbstractGuiButton> _buttons;

        public GuiManager(XmasHell game)
        {
            _game = game;
            _buttons = new List<AbstractGuiButton>();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Update(gameTime);
            }
        }

        public void AddButton(AbstractGuiButton button)
        {
            _buttons.Add(button);

            if (button is SpriteGuiButton)
                _game.SpriteBatchManager.UISprites.Add((button as SpriteGuiButton).Sprite);
            else if (button is SpriterGuiButton)
                _game.SpriteBatchManager.UISpriterAnimators.Add((button as SpriterGuiButton).Animator);
        }

        public void RemoveButton(AbstractGuiButton button)
        {
            if (button is SpriteGuiButton)
                _game.SpriteBatchManager.UISprites.Remove((button as SpriteGuiButton).Sprite);
            else if (button is SpriterGuiButton)
                _game.SpriteBatchManager.UISpriterAnimators.Remove((button as SpriterGuiButton).Animator);

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
