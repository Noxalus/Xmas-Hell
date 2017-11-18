using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Entities.Bosses
{
    public abstract class AbstractBossBehaviour
    {
        protected Boss Boss;
        protected float InitialBehaviourLife;
        protected float CurrentBehaviourLife;
        protected bool BehaviourEnded;

        public float GetCurrentBehaviourLife()
        {
            return CurrentBehaviourLife;
        }

        public float GetLifePercentage()
        {
            return CurrentBehaviourLife / InitialBehaviourLife;
        }

        public bool IsBehaviourEnded()
        {
            return BehaviourEnded;
        }

        protected AbstractBossBehaviour(Boss boss)
        {
            Boss = boss;
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife;
        }

        public virtual void Start()
        {
            Reset();
        }

        public virtual void Reset()
        {
            CurrentBehaviourLife = InitialBehaviourLife;
            BehaviourEnded = false;
            Stop();
        }

        public virtual void Stop()
        {
        }

        public virtual void TakeDamage(float amount)
        {
            CurrentBehaviourLife -= amount;
        }

        protected virtual void CheckBehaviourIsEnded()
        {
            if (CurrentBehaviourLife < 0)
                BehaviourEnded = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            CheckBehaviourIsEnded();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void DrawAfter(SpriteBatch spriteBatch)
        {
        }
    }
}