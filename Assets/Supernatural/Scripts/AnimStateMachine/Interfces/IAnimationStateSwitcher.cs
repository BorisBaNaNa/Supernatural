namespace Assets.Supernatural.Scripts.AnimStateMachine.Interfces
{
    public interface IAnimationStateSwitcher
    {
        void StateSwitch<TState>() where TState : IAnimationState;
    }
}
