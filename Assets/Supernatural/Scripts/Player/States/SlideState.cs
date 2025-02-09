using System;
using UnityEngine;

public class SlideState : IState
{
    private readonly PlayerStateMachine _stateMachine;
    private readonly Player _player;
    private float _timeToWallUnstick;
    private Vector3 _playerVelocity;
    private float _lastPlayerDir;

    public SlideState(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _player = _stateMachine.Player;
        _stateMachine.Jump = JumpSliding;
    }

    public void Enter()
    {
        if (!_player.SoundFx.isPlaying)
            _player.SoundFx.Play();

        _stateMachine.CurrentAction += Sliding;
        _stateMachine.Player.AnimController.PlayStableAnimation(this);
    }

    public void Exit()
    {
        if (_player.SoundFx.isPlaying)
            _player.SoundFx.Stop();

        _stateMachine.CurrentAction -= Sliding;
    }

    private void Sliding()
    {
        UpdateVals();

        if (_playerVelocity.y < -_player.wallSlideSpeedMax)
            _playerVelocity.y = -_player.wallSlideSpeedMax;

        StartAnimation();

        if (_timeToWallUnstick > 0)
        {
            //_player.velocityXSmoothing = 0;
            _playerVelocity.x = 0;

            if (_player.MoveDir.x != 0 && _player.MoveDir.x != _player.WallDirX)
                _timeToWallUnstick -= Time.deltaTime;
            else
                _timeToWallUnstick = _player.wallStickTime;
        }
        else
            _timeToWallUnstick = _player.wallStickTime;

        _player.Velocity = _playerVelocity;
    }

    private void StartAnimation()
    {
        if (_lastPlayerDir != _player.MoveDir.x)
        {
            _stateMachine.Player.AnimController.PlayStableAnimation(this);
            _lastPlayerDir = _player.MoveDir.x;
        }
    }

    private void JumpSliding()
    {
        if (_player.MoveDir.x == _player.WallDirX)
        {
            _playerVelocity.x = -_player.WallDirX * _player.wallJumpClimb.x;
            _playerVelocity.y = _player.wallJumpClimb.y;
        }
        else if (_player.MoveDir.x == 0)
        {
            _playerVelocity.x = -_player.WallDirX * _player.wallJumpOff.x;
            _playerVelocity.y = _player.wallJumpOff.y;
            _player.Flip();
        }
        else
        {
            _playerVelocity.x = -_player.WallDirX * _player.wallLeap.x;
            _playerVelocity.y = _player.wallLeap.y;
        }

        _player.Velocity = _playerVelocity;
        SoundManager.PlaySfx(_player.jumpSound);
    }

    private void UpdateVals()
    {
        _playerVelocity = _player.Velocity;
    }
}
