using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using XmasHell.Controls;

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
        protected bool Enabled = true;

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

        protected bool InputDown;
        private bool _previousInputDown;
        protected bool InputDownStateChanged;

#if ANDROID
        public event EventHandler<Point> TouchStarted;
        public event EventHandler<Point> Tap;
#else
        public event EventHandler<Point> MouseDown;
        public event EventHandler<Point> Click;
#endif

        public event EventHandler<Point> Action;

        public AbstractGuiButton(ViewportAdapter viewportAdapter, String name)
        {
            ViewportAdapter = viewportAdapter;
            Name = name;

            InputDown = false;
            _previousInputDown = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            _previousInputDown = InputDown;

#if ANDROID
            UpdateTouchState();
#else
            UpdateMouseState();
#endif

            InputDownStateChanged = _previousInputDown != InputDown;
        }

#if ANDROID
        private void UpdateTouchState()
        {
            var position = ViewportAdapter.PointToScreen(InputManager.TouchPosition());

            if (BoundingRectangle().Intersects(new Rectangle(position, Vector2.One.ToPoint())))
            {
                if (InputManager.TouchDown())
                {
                    InputDown = true;
                    TouchStarted?.Invoke(this, position);
                }
                else if (InputManager.TouchUp() && InputDown)
                {
                    InputDown = false;
                    Tap?.Invoke(this, position);
                    Action?.Invoke(this, position);
                }
            }
            else
            {
                InputDown = false;
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
                    InputDown = true;
                    MouseDown?.Invoke(this, position);
                }
                else if (InputManager.LeftClickReleased() && InputDown)
                {
                    InputDown = false;

                    Click?.Invoke(this, position);
                    Action?.Invoke(this, position);
                }
            }
            else
            {
                InputDown = false;
            }
        }
#endif
    }
}
