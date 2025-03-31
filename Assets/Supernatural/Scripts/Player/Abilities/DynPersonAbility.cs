using Assets.Supernatural.Scripts.Interfaces;
using Assets.Supernatural.Scripts.Player.AnimationStates.Once;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Assets.Supernatural.Scripts.Player.Abilities
{
    [Serializable]
    public class DynPersonAbility : UltimateAbilityBase
    {
        [SerializeField] private float _duration;

        private float _timer;
        private CancellationTokenSource _healCTS;

        private const float HEAL_AMOUNT = 30f;

        public override void Activate(PlayerController playerContext)
        {
            base.Activate(playerContext);
            _timer = _duration;
            playerContext.AnimStateMachine.TryGetState<UltimateState>(out var state);
            state.SetLoop(true);
            playerContext.AnimStateMachine.StateSwitch<UltimateState>();
            HealProcessAsync(playerContext, HEAL_AMOUNT, _duration - 0.5f).Forget();
        }

        public override void Deactivate(PlayerController playerContext)
        {
            base.Deactivate(playerContext);
            _healCTS?.Cancel();
            playerContext.AnimStateMachine.DropCurrentState();
        }

        public override void UpdateAbility(PlayerController playerContext)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
                Deactivate(playerContext);
        }

        public override bool HandleDamageTaken(PlayerController playerContext, float damage, Vector2 force, GameObject instigator)
        {
            Debug.Log("Ultaimate save from damage");
            return true;
        }

        private async UniTask HealProcessAsync(ICanTakeHealth playerContext, float healAmount, float duration)
        {
            float timer = duration;
            float healPerTick = healAmount / duration;
            _healCTS = new CancellationTokenSource();

            try
            {
                while (timer > 0 && !_healCTS.IsCancellationRequested)
                {
                    timer -= Time.deltaTime;
                    playerContext.TakeHealth(healPerTick * Time.deltaTime);
                    await UniTask.Yield(cancellationToken: _healCTS.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _healCTS.Dispose();
                _healCTS = null;
            }
        }
    }
}
