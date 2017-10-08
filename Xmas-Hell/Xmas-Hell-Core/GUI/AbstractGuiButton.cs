using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using Xmas_Hell_Core.Controls;

namespace XmasHell.GUI
{
    public abstract class AbstractGuiButton : IScalable, IRotatable, IMovable
    {
        protected ViewportAdapter ViewportAdapter;
        public String Name;

        public abstract Vector2 Position();
        public abstract void Position(Vector2 value);
        public abstract Vector2 Scale();
        public abstract void Scale(Vector2 value);
        public abstract float Rotation();
        public abstract void Rotation(float value);
        public abstract BoundingRectangle BoundingRectangle();

        Vector2 IMovable.Position
        {
            get { return Position(); }
            set { Position(value); }
        }

        float IRotatable.Rotation
        {
            get { return Rotation(); }
            set { Rotation(value); }
        }

        Vector2 IScalable.Scale
        {
            get { return Scale(); }
            set { Scale(value); }
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

        public event EventHandler<Point> Action;

        public AbstractGuiButton(ViewportAdapter viewportAdapter, String name)
        {
            ViewportAdapter = viewportAdapter;
            Name = name;

#if ANDROID
            _touchedDown = false;
#else
            _mouseDown = false;
#endif
        }

        public virtual void Update(GameTime gameTime)
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
            var position = ViewportAdapter.PointToScreen(InputManager.TouchPosition());

            if (BoundingRectangle().Intersects(new Rectangle(position, Vector2.One.ToPoint())))
            {
                if (InputManager.TouchDown())
                {
                    _touchedDown = true;
                    TouchStarted?.Invoke(this, position);
                    Action?.Invoke(this, position);
                }
                else if (InputManager.TouchUp() && _touchedDown)
                {
                    _touchedDown = false;
                    Tap?.Invoke(this, position);
                    Action?.Invoke(this, position);
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
            var position = ViewportAdapter.PointToScreen(InputManager.ClickPosition());

            if (BoundingRectangle().Intersects(new Rectangle(position, Point.Zero)))
            {
                if (InputManager.Clicked())
                {
                    _mouseDown = true;
                    MouseDown?.Invoke(this, position);
                    Action?.Invoke(this, position);
                }
                else if (InputManager.LeftClickReleased() && _mouseDown)
                {
                    _mouseDown = false;

                    Click?.Invoke(this, position);
                    Action?.Invoke(this, position);
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
