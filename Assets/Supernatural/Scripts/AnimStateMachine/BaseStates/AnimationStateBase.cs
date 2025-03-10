using Assets.Supernatural.Scripts.AnimStateMachine.Interfces;

namespace Assets.Supernatural.Scripts.AnimStateMachine.BaseStates
{
    public abstract class AnimationStateBase : IAnimationState
    {
        protected IAnimationStateSwitcher _switcher;

        public virtual void Init(IAnimationStateSwitcher switcher) => _switcher = switcher;

        public abstract void Enter();
        public abstract void Exit();
        public virtual void Update() { }
    }
}
