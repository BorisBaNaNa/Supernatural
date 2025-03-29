namespace Assets.Supernatural.Scripts.AnimStateMachine.BaseStates
{
    public abstract class AnimationTempStateBase : AnimationStateBase
    {
        bool IsComplete { get; }

        public override void Update()
        {
            if (IsComplete)
                GoToNextState();
        }

        protected virtual void GoToNextState() => _switcher.GoToNextState();
    }
}
