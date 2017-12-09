using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;

namespace XmasHell.Controls
{
    internal class InputManager : GameComponent
    {
        #region Mouse Field Region

        private static MouseState _mouseState;
        private static MouseState _lastMouseState;

        #endregion Mouse Field Region

        #region Keyboard Field Region

        private static KeyboardState _keyboardState;
        private static KeyboardState _lastKeyboardState;

        #endregion Keyboard Field Region

        #region Game Pad Field Region

        private static GamePadState[] _gamePadStates;
        private static GamePadState[] _lastGamePadStates;

        #endregion Game Pad Field Region

        #region Touch Field Region

        private static TouchCollection _touchState;
        private static TouchCollection _lastTouchState;

        #endregion Touch Field Region

        #region Mouse Property Region

        public static MouseState MouseState => _mouseState;
        public static MouseState LastMouseState => _lastMouseState;

        #endregion Mouse Property Region

        #region Keyboard Property Region

        public static KeyboardState KeyboardState => _keyboardState;
        public static KeyboardState LastKeyboardState => _lastKeyboardState;

        #endregion Keyboard Property Region

        #region Game Pad Property Region

        public static GamePadState[] GamePadStates
        {
            get { return _gamePadStates; }
        }

        public static GamePadState[] LastGamePadStates
        {
            get { return _lastGamePadStates; }
        }

        #endregion Game Pad Property Region

        #region Touch Property Region

        public static TouchCollection TouchState => _touchState;
        public static TouchCollection LastTouchState => _lastTouchState;

        #endregion Touch Property Region

        #region Constructor Region

        public InputManager(Game game) : base(game)
        {
            _mouseState = Mouse.GetState();
            _keyboardState = Keyboard.GetState();
            _touchState = TouchPanel.GetState();
            _gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                _gamePadStates[(int)index] = GamePad.GetState(index);
        }

        #endregion Constructor Region

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

            _lastGamePadStates = (GamePadState[])_gamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                _gamePadStates[(int)index] = GamePad.GetState(index);

            base.Update(gameTime);
        }

        #endregion XNA methods

        #region General Method Region

        public static void Flush()
        {
            _lastMouseState = _mouseState;
            _lastKeyboardState = _keyboardState;
        }

        public static bool Up()
        {
            return
                (KeyDown(Keys.Up) ||
                ButtonDown(Buttons.DPadUp, PlayerIndex.One) ||
                ButtonDown(Buttons.LeftThumbstickUp, PlayerIndex.One));
        }

        public static bool Down()
        {
            return
                (KeyDown(Keys.Down) ||
                ButtonDown(Buttons.DPadDown, PlayerIndex.One) ||
                ButtonDown(Buttons.LeftThumbstickDown, PlayerIndex.One));
        }

        public static bool Left()
        {
            return
                (KeyDown(Keys.Left) ||
                ButtonDown(Buttons.DPadLeft, PlayerIndex.One) ||
                ButtonDown(Buttons.LeftThumbstickLeft, PlayerIndex.One));
        }

        public static bool Right()
        {
            return
                (KeyDown(Keys.Right) ||
                ButtonDown(Buttons.DPadRight, PlayerIndex.One) ||
                ButtonDown(Buttons.LeftThumbstickRight, PlayerIndex.One));
        }

        public static bool PressedUp()
        {
            return
                (KeyPressed(Keys.Up) ||
                ButtonPressed(Buttons.DPadUp, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickUp, PlayerIndex.One));
        }

        public static bool PressedDown()
        {
            return
                (KeyPressed(Keys.Down) ||
                ButtonPressed(Buttons.DPadDown, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickDown, PlayerIndex.One));
        }

        public static bool PressedLeft()
        {
            return
                (KeyPressed(Keys.Left) ||
                ButtonPressed(Buttons.DPadLeft, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickLeft, PlayerIndex.One));
        }

        public static bool PressedRight()
        {
            return
                (KeyPressed(Keys.Right) ||
                ButtonPressed(Buttons.DPadRight, PlayerIndex.One) ||
                ButtonPressed(Buttons.LeftThumbstickRight, PlayerIndex.One));
        }

        public static bool PressedAction()
        {
            return
                (KeyPressed(Keys.Enter) ||
                ButtonPressed(Buttons.A, PlayerIndex.One));
        }

        public static bool PressedCancel()
        {
            return
                (KeyPressed(Keys.Escape) ||
                ButtonPressed(Buttons.B, PlayerIndex.One) ||
                ButtonPressed(Buttons.Back, PlayerIndex.One));
        }

        #endregion General Method Region

        #region Mouse Region

        public static bool LeftClickReleased()
        {
            return _lastMouseState.LeftButton == ButtonState.Pressed &&
                   _mouseState.LeftButton == ButtonState.Released;
        }

        public static bool Clicked()
        {
            return _mouseState.LeftButton == ButtonState.Pressed &&
                   _lastMouseState.LeftButton == ButtonState.Released;
        }

        public static bool LeftClickDown()
        {
            return _mouseState.LeftButton == ButtonState.Pressed;
        }

        public static Point ClickPosition()
        {
            return new Point(_mouseState.X, _mouseState.Y);
        }

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

        #endregion Mouse Region

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

        #endregion Keyboard Region

        #region Touch Region

        public static int TouchCount()
        {
            return _touchState.Count;
        }

        public static bool TouchUp()
        {
            return _lastTouchState.Count > _touchState.Count;
        }

        public static bool TouchDown()
        {
            return _touchState.Count > 0 && _touchState.Count > _lastTouchState.Count && _touchState[_touchState.Count - 1].State == TouchLocationState.Pressed;
        }

        public static bool TouchIsDown()
        {
            return _touchState.Count > 0;
        }

        public static Point TouchPosition()
        {
            return (_touchState.Count > 0) ? _touchState[_touchState.Count - 1].Position.ToPoint() : (_lastTouchState.Count > 0) ? _lastTouchState[_lastTouchState.Count - 1].Position.ToPoint() : Point.Zero;
        }

        #endregion Touch Region

        #region Game Pad Region

        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return _gamePadStates[(int)index].IsButtonUp(button) &&
                _lastGamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return _gamePadStates[(int)index].IsButtonDown(button) &&
                _lastGamePadStates[(int)index].IsButtonUp(button);
        }

        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return _gamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool HavePressedButton(PlayerIndex index)
        {
            return _gamePadStates[(int)index] != _lastGamePadStates[(int)index];
        }

        public static Buttons[] GetPressedButton(PlayerIndex index)
        {
            return Enum.GetValues(typeof(Buttons)).Cast<Buttons>().Where(button => ButtonPressed(button, index)).ToArray();
        }

        #endregion Game Pad Region
    }
}