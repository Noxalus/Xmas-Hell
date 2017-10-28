﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using XmasHell.Rendering;

namespace XmasHell.GUI
{
    public class GuiManager
    {
        private XmasHell _game;
        private List<AbstractGuiButton> _buttons = new List<AbstractGuiButton>();
        private List<AbstractGuiLabel> _labels = new List<AbstractGuiLabel>();

        public GuiManager(XmasHell game)
        {
            _game = game;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _buttons.Count; i++)
                _buttons[i].Update(gameTime);

            for (int i = 0; i < _labels.Count; i++)
                _labels[i].Update(gameTime);
        }

        public void AddLabel(AbstractGuiLabel label)
        {
            _labels.Add(label);

            if (label is SpriterGuiLabel)
                _game.SpriteBatchManager.UILabels.Add(label);
        }

        public void RemoveLabel(AbstractGuiLabel label)
        {
            _game.SpriteBatchManager.UILabels.Remove(label);
            _labels.Remove(label);
        }

        public void AddButton(AbstractGuiButton button)
        {
            _buttons.Add(button);

            if (button is SpriteGuiButton)
                _game.SpriteBatchManager.UISprites.Add((button as SpriteGuiButton).Sprite);
            else if (button is SpriterGuiButton)
            {
                var spriterButton = (button as SpriterGuiButton);
                spriterButton.SubstituteEntity.Reset();
                _game.SpriteBatchManager.AddSpriterAnimator(spriterButton.Animator(), Layer.UI);
            }
        }

        public void RemoveButton(AbstractGuiButton button)
        {
            if (button is SpriteGuiButton)
                _game.SpriteBatchManager.UISprites.Remove((button as SpriteGuiButton).Sprite);
            else if (button is SpriterGuiButton)
                _game.SpriteBatchManager.RemoveSpriterAnimator((button as SpriterGuiButton).Animator(), Layer.UI);

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
