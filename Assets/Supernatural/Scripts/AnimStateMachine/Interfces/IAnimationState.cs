namespace Assets.Supernatural.Scripts.AnimStateMachine.Interfces
{
    public interface IAnimationState
    {
        void Init(IAnimationStateSwitcher switcher);
        void Enter();
        void Exit();
        void Update();
    }
}
