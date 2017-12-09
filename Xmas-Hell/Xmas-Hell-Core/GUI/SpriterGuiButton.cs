using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.IO;
using XmasHell.Audio;
using XmasHell.Spriter;

namespace XmasHell.GUI
{
    public class SpriterGuiButton : AbstractGuiButton
    {
        public SpriterSubstituteEntity SubstituteEntity;
        private string _animationName;
        private string _clickAnimationName;
        private string _inputDownSoundName;
        private string _inputUpSoundName;
        private bool _buttonClicked;
        private bool _stopAnimationWhenClicked;

        public CustomSpriterAnimator Animator()
        {
            return SubstituteEntity.SubstituteAnimator;
        }

        public override Vector2 Position()
        {
            return SubstituteEntity.SubstituteAnimator.Position;
        }

        public override void Scale(Vector2 value)
        {
            SubstituteEntity.SubstituteAnimator.Scale = value;
        }

        public override float Rotation()
        {
            return SubstituteEntity.SubstituteAnimator.Rotation;
        }

        public override void Rotation(float value)
        {
            SubstituteEntity.SubstituteAnimator.Rotation = value;
        }

        public override void Position(Vector2 value)
        {
            SubstituteEntity.SubstituteAnimator.Position = value;
        }

        public override Vector2 Scale()
        {
            return SubstituteEntity.SubstituteAnimator.Scale;
        }

        public override BoundingRectangle BoundingRectangle()
        {
            return SubstituteEntity.BoundingRectangle();
        }

        public void Enable(bool value)
        {
            Enabled = value;
        }

        public void Synchronize()
        {
            SubstituteEntity.Synchronize();
        }

        public SpriterGuiButton(
            ViewportAdapter viewportAdapter,
            String buttonName,
            String spritePartCompleteFilename,
            CustomSpriterAnimator animator,
            CustomSpriterAnimator referenceAnimator,
            string animationName = null,
            string clickAnimationName = null,
            string inputDownSoundName = null,
            string inputUpSoundName = null,
            bool stopAnimationWhenClicked = false) :
            base(viewportAdapter, buttonName)
        {
            SubstituteEntity = new SpriterSubstituteEntity(Path.GetFileName(spritePartCompleteFilename), referenceAnimator, animator);
            _animationName = animationName;
            _clickAnimationName = clickAnimationName;

            _inputDownSoundName = inputDownSoundName;
            _inputUpSoundName = inputUpSoundName;
            _stopAnimationWhenClicked = stopAnimationWhenClicked;

            if (_animationName != null)
                SubstituteEntity.SubstituteAnimator.Play(_animationName);

            Reset();
        }

        public override void Reset()
        {
            base.Reset();

            _buttonClicked = false;

            if (_stopAnimationWhenClicked)
                Action += ButtonAction;

            SubstituteEntity.SubstituteAnimator.Speed = 1;

            if (_animationName != null)
                SubstituteEntity.SubstituteAnimator.Play(_animationName);
        }

        private void ButtonAction(object sender, Point e)
        {
            _buttonClicked = true;
            Action -= ButtonAction;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputDownStateChanged)
            {
                if (InputDown)
                {
                    if (_clickAnimationName != null)
                        SubstituteEntity.SubstituteAnimator.Play(_clickAnimationName);
                    if (_inputDownSoundName != null)
                        SoundManager.PlaySound(Assets.GetSound(_inputDownSoundName));
                }
                else
                {
                    if (_animationName != null)
                    {
                        if (_buttonClicked && _stopAnimationWhenClicked)
                            SubstituteEntity.SubstituteAnimator.Speed = 0;
                        else
                            SubstituteEntity.SubstituteAnimator.Play(_animationName);
                    }
                    if (_inputUpSoundName != null)
                        SoundManager.PlaySound(Assets.GetSound(_inputUpSoundName));
                }
            }

            SubstituteEntity.Update(gameTime);
        }
    }
}
