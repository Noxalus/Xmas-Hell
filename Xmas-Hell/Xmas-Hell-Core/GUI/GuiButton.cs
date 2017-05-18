using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Text;
using Xmas_Hell_Core.Controls;

namespace XmasHell.GUI
{
    public class GuiButton : IScalable, IRotatable, IMovable
    {
        private ViewportAdapter _viewportAdapter;
        public String Name;
        public Sprite Sprite;

        public Vector2 Position
        {
            get { return Sprite.Position; }
            set { Sprite.Position = value; }
        }

        public Vector2 Scale
        {
            get { return Sprite.Scale; }
            set { Sprite.Scale = value; }
        }

        public float Rotation
        {
            get { return Sprite.Rotation; }
            set { Sprite.Rotation = value; }
        }

#if ANDROID
        private bool _touchedDown;
        public event EventHandler<Point> TouchStarted;
        public event EventHandler<Point> Tap;
#else
        private bool _mouseDown;
        public event EventHandler<Point> MouseDown;
        public event EventHandler<Point> Click;
#endif

        public GuiButton(ViewportAdapter viewportAdapter, String name, Sprite sprite)
        {
            _viewportAdapter = viewportAdapter;
            Name = name;
            Sprite = sprite;

#if ANDROID
            _touchedDown = false;
#else
            _mouseDown = false;
#endif
        }

        public void Update(GameTime gameTime)
        {
#if ANDROID
            UpdateTouchState();
#else
            UpdateMouseState();
#endif
        }

#if ANDROID
        private void UpdateTouchState()
        {
            var position = _viewportAdapter.PointToScreen(InputManager.TouchPosition());

            if (Sprite.BoundingRectangle.Intersects(new Rectangle(position, Vector2.One.ToPoint())))
            {
                if (InputManager.TouchDown())
                {
                    _touchedDown = true;
                    TouchStarted?.Invoke(this, position);
                }
                else if (InputManager.TouchUp() && _touchedDown)
                {
                    _touchedDown = false;
                    Tap?.Invoke(this, position);
                }
            }
            else
            {
                _touchedDown = false;
            }
        }
#else

        private void UpdateMouseState()
        {
            var position = _viewportAdapter.PointToScreen(InputManager.ClickPosition());

            if (Sprite.BoundingRectangle.Intersects(new Rectangle(position, Point.Zero)))
            {
                if (InputManager.Clicked())
                {
                    _mouseDown = true;
                    MouseDown?.Invoke(this, position);
                }
                else if (InputManager.LeftClickReleased() && _mouseDown)
                {
                    _mouseDown = false;

                    Click?.Invoke(this, position);
                }
            }
            else
            {
                _mouseDown = false;
            }
        }
#endif
    }
}
