using System;
using Microsoft.Xna.Framework;
using XmasHell.Entities;
using XmasHell.Entities.Bosses;
using Xmas_Hell_Core.Controls;
using System.Collections.Generic;
using XmasHell.Spriter;
using XmasHell.Rendering;

namespace XmasHell.Screens
{
    public class GameScreen : Screen
    {
        private Player _player;
        private Boss _boss;
        private TimeSpan _playTime;
        private bool _endGamePopupOpened = false;

        // GUI
        private Dictionary<string, CustomSpriterAnimator> _spriterFile;

        private float GetRank()
        {
            return 1f;
        }

        public GameScreen(XmasHell game) : base(game)
        {
            ShouldBeStackInHistory = true;
            GameManager.GameDifficulty = GetRank;

            _player = new Player(game);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _spriterFile = Assets.GetSpriterAnimators("Graphics/GUI/game-screen");
            InitializeSpriterGui();
        }

        private void InitializeSpriterGui()
        {
            _spriterFile["EndGamePanel"].AnimationFinished += EndGamePanel_AnimationFinished;
        }

        #region Animations finished
        private void EndGamePanel_AnimationFinished(string animationName)
        {
            if (animationName == "Show")
                _spriterFile["EndGamePanel"].Play("Idle");
            else if (animationName == "Hide")
                CloseEndGamePopup();
        }
        #endregion

        public void LoadBoss(BossType bossType)
        {
            _boss = BossFactory.CreateBoss(bossType, Game, _player.Position);
        }

        private void OpenEndGamePopup()
        {
            if (_endGamePopupOpened)
                return;

            _endGamePopupOpened = true;
            Game.SpriteBatchManager.AddSpriterAnimator(_spriterFile["EndGamePanel"], Layer.UI);
            _spriterFile["EndGamePanel"].Play("Show");
        }

        private void CloseEndGamePopup()
        {
            _endGamePopupOpened = false;
        }

        // TODO: This should be handled by the ScreenManager
        public override void Show(bool reset = false)
        {
            base.Show(reset);

            _player.Initialize();
            _boss.Initialize();

            _playTime = TimeSpan.Zero;
            Game.PlayerData.BossAttempts(_boss.BossType, Game.PlayerData.BossAttempts(_boss.BossType) + 1);

            // Should play music (doesn't seem to work for now...)
            //MediaPlayer.Volume = 1f;
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Assets.GetMusic("Audio/BGM/boss-theme"));
        }

        public override void Hide()
        {
            base.Hide();

            _boss.Dispose();
            _player.Dispose();
            Game.GameManager.Clear();

            Game.PlayerData.BossPlayTime(_boss.BossType, Game.PlayerData.BossPlayTime(_boss.BossType) + _playTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.PressedCancel())
                Game.ScreenManager.GoTo<BossSelectionScreen>();

            _playTime += gameTime.ElapsedGameTime;

            if (Game.GameManager.GameIsFinished() && !_endGamePopupOpened)
                OpenEndGamePopup();

            if (_player.Alive())
                _player.Update(gameTime);

            if (_boss.Alive())
                _boss.Update(gameTime);
        }
    }
}