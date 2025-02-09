using UnityEngine;

public class JumpState : IState, IService
{
    private readonly PlayerStateMachine _stateMachine;
    private readonly Player _player;
    private Vector3 _playerVelocity;
    private int _numberOfJumpLeft;
    private float _maxJumpVelocity;
    private float _minJumpVelocity;

    public JumpState(PlayerStateMachine stateMachine)
    {
        AllServices.Instance.RegisterService(this);
        _stateMachine = stateMachine;

        _player = _stateMachine.Player;
        _numberOfJumpLeft = _player.NumberOfJumpMax;

        TurnGravityAndJump();
    }

    public void Enter()
    {
        if (_player.IsSliding)
            _stateMachine.Jump();
        else
            Jump();
    }

    public void Exit() { }

    public void Jump()
    {
        if (_player.IsGrounded)
        {
            Jump(_maxJumpVelocity);
            _numberOfJumpLeft = _player.NumberOfJumpMax;
        }
        else if (_numberOfJumpLeft-- > 0)
        {
            Jump(_minJumpVelocity);
        }
    }

    public void JumpOff()
    {
        if (_player.Velocity.y > _minJumpVelocity)
        {
            _playerVelocity = _player.Velocity;
            _playerVelocity.y = _minJumpVelocity;
            _player.Velocity = _playerVelocity;
        }
    }

    private void Jump(float jumpVelocity)
    {
        _player.AnimController.PlayOneTimeAnimation(this);
        _playerVelocity = _player.Velocity;
        _playerVelocity.y = jumpVelocity;
        _player.Velocity = _playerVelocity;

        if (_player.JumpEffect != null)
            Object.Instantiate(_player.JumpEffect, _player.transform.position, _player.transform.rotation);
        SoundManager.PlaySfx(_player.jumpSound);
    }

    private void TurnGravityAndJump()
    {
        _player.Gravity = -(2 * _player.MaxJumpHeight) / Mathf.Pow(_player.TimeToJumpApex, 2);
        _maxJumpVelocity = Mathf.Abs(_player.Gravity) * _player.TimeToJumpApex;
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_player.Gravity) * _player.MinJumpHeight);
        //		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
    }
}
