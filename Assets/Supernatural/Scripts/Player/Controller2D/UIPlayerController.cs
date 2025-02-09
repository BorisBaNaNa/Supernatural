using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerController : MonoBehaviour
{
    public Button RangeAttackBtn;

    private Player _player;
    private LevelManager _levelManager;
    private Coroutine _rangeAttackCorutine;
    private Coroutine _meleeAttackCorutine;
    private float _playerRangeAttackRate;
    private float _playerMeleeAttackRate;

    public void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (!_levelManager) return;

        RangeAttackBtn.interactable = _levelManager.BulletCount > 0;
    }

    public void MoveLeftStart() =>
        _player.MoveDir = Vector2.left;

    public void MoveRightStart() =>
        _player.MoveDir = Vector2.right;

    public void MoveDownStart() =>
        _player.MoveDir = Vector2.down;

    public void MoveStop() =>
        _player.MoveDir = Vector2.zero;

    public void Jump() => _player.Jump();

    public void JumpOff() => _player.JumpOff();

    public void RangeAttack() => _player.RangeAttack();

    public void MeleeAttack() => _player.MeleeAttack();

    public void StartRangeAttack() =>
        _rangeAttackCorutine = StartCoroutine(RangeAttacking());

    public void StopRangeAttack() =>
        StopCoroutine(_rangeAttackCorutine);

    public void StartMeleeAttack() =>
        _meleeAttackCorutine = StartCoroutine(MeleeAttacking());

    public void StopMeleeAttack() =>
        StopCoroutine(_meleeAttackCorutine);

    private void Initialize()
    {
        _levelManager = AllServices.Instance.GetService<LevelManager>();
        if (_levelManager)
        {
            _player = _levelManager.Player;
            _playerRangeAttackRate = _player.GetComponent<RangeAttack>().fireRate;
            _playerMeleeAttackRate = _player.GetComponent<MeleeAttack>().attackRate;
        }
        else
            _player = FindObjectOfType<Player>();
    }

    private IEnumerator RangeAttacking()
    {
        while (true)
        {
            _player.RangeAttack();
            yield return new WaitForSeconds(_playerRangeAttackRate);
        }
    }

    private IEnumerator MeleeAttacking()
    {
        while (true)
        {
            _player.MeleeAttack();
            yield return new WaitForSeconds(_playerMeleeAttackRate);
        }
    }
}
