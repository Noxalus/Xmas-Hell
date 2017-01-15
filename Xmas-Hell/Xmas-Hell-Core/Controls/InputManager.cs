using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Xmas_Hell_Core.Controls
{
    class InputManager : GameComponent
    {
        #region Mouse Field Region

        static MouseState _mouseState;
        static MouseState _lastMouseState;

        #endregion

        #region Keyboard Field Region

        static KeyboardState _keyboardState;
        static KeyboardState _lastKeyboardState;

        #endregion

        #region Touch Field Region

        static TouchCollection _touchState;
        static TouchCollection _lastTouchState;

        #endregion

        #region Mouse Property Region

        public static MouseState MouseState => _mouseState;
        public static MouseState LastMouseState => _lastMouseState;

        #endregion

        #region Keyboard Property Region

        public static KeyboardState KeyboardState => _keyboardState;
        public static KeyboardState LastKeyboardState => _lastKeyboardState;

        #endregion

        #region Touch Property Region

        public static TouchCollection TouchState => _touchState;
        public static TouchCollection LastTouchState => _lastTouchState;

        #endregion

        #region Constructor Region

        public InputManager(Game game) : base(game)
        {
            _mouseState = Mouse.GetState();
            _keyboardState = Keyboard.GetState();
            _touchState = TouchPanel.GetState();
        }

        #endregion

#region XNA methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _lastMouseState = _mouseState;
            _mouseState = Mouse.GetState();

            _lastKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            _lastTouchState = _touchState;
            _touchState = TouchPanel.GetState();

            base.Update(gameTime);
        }

#endregion

#region General Method Region

        public static void Flush()
        {
            _lastMouseState = _mouseState;
            _lastKeyboardState = _keyboardState;
        }

        public static bool PressedUp()
        {
            return (KeyPressed(Keys.Up));
        }

        public static bool PressedDown()
        {
            return KeyPressed(Keys.Down);
        }

        public static bool PressedLeft()
        {
            return KeyPressed(Keys.Left);
        }

        public static bool PressedRight()
        {
            return KeyPressed(Keys.Right);
        }

        public static bool PressedAction()
        {
            return KeyPressed(Keys.Enter);
        }

        public static bool PressedCancel()
        {
            return KeyPressed(Keys.Escape);
        }

#endregion

#region Mouse Region

        public static bool Scroll()
        {
            return _mouseState.ScrollWheelValue == _lastMouseState.ScrollWheelValue;
        }

        public static bool ScrollUp()
        {
            return _mouseState.ScrollWheelValue > _lastMouseState.ScrollWheelValue;
        }

        public static bool ScrollDown()
        {
            return _mouseState.ScrollWheelValue < _lastMouseState.ScrollWheelValue;
        }

#endregion

#region Keyboard Region

        public static bool KeyReleased(Keys key)
        {
            return _keyboardState.IsKeyUp(key) &&
                _lastKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return _keyboardState.IsKeyDown(key) &&
                _lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return _keyboardState.IsKeyDown(key);
        }

        public static bool HavePressedKey()
        {
            return _keyboardState != _lastKeyboardState;
        }

        public static Keys[] GetPressedKeys()
        {
            return _keyboardState.GetPressedKeys();
        }

#endregion

#region Touch Region

        public static bool TouchUp()
        {
            return _lastTouchState.Count > 0 && _touchState.Count == 0;
        }

        public static bool TouchDown()
        {
            return _touchState.Count > 0 && _lastTouchState.Count == 0;
        }

        public static Vector2 GetTouchPosition()
        {
            return _lastTouchState[0].Position;
        }

#endregion
    }
}
