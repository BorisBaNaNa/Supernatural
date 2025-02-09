using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;
using static Player;

public class PlayerAnimController : MonoBehaviour
{
    [Header("Components")]
    public Player Player;
    public SkeletonAnimation SkeletonAnim;
    public string WalkEventName;

    public int ActiveAnimTrack = 0;
    public int StableAnimTrack = 1;

    public AnimationReferenceAsset Idle, Walk, Crouch, Crouching, UnCrouch, Jump, Fall, Land, SlideFaceToWall, SlideBackToWall, 
        MeleeAttackAnim, RangeAttackAnim, TackeDamageAnim, Death, Win, Respawn;

    private EventData _walkEventData;


    public void Start()
    {
        if (SkeletonAnim == null)
            SkeletonAnim = GetComponent<SkeletonAnimation>();

        _walkEventData = SkeletonAnim.Skeleton.Data.FindEvent(WalkEventName);
        SkeletonAnim.AnimationState.Event += HandleAnimationStateEvent;
    }

    public void PlayStableAnimation(IState playerState)
    {
        TrackEntry currentAnim = SkeletonAnim.AnimationState.GetCurrent(StableAnimTrack);

        switch (playerState.GetType().Name)
        {
            case "IdleState":
                SoftSetAnimation(StableAnimTrack, currentAnim, Idle, true);
                break;
            case "WalkState":
                SoftSetAnimation(StableAnimTrack, currentAnim, Walk, true);
                break;
            case "CrouchState":
                SkeletonAnim.AnimationState.SetAnimation(StableAnimTrack, Crouch, false);
                SkeletonAnim.AnimationState.AddAnimation(StableAnimTrack, Crouching, true, 0);
                break;
            case "FallState":
                SoftSetAnimation(StableAnimTrack, currentAnim, Fall, true);
                break;
            case "SlideState":
                if (Player.transform.localScale.x == Player.WallDirX)
                    SoftSetAnimation(StableAnimTrack, currentAnim, SlideFaceToWall, true);
                else
                    SoftSetAnimation(StableAnimTrack, currentAnim, SlideBackToWall, true);
                break;
            default:
                Debug.LogError($"Error type, PlayStableAnimation has not {playerState.GetType().Name} animation type!");
                break;
        }

    }

    public void PlayExitStateAnimation(IState playerState)
    {
        switch (playerState.GetType().Name)
        {
            case "CrouchState":
                SkeletonAnim.AnimationState.SetAnimation(0, UnCrouch, false);
                break;
            default:
                Debug.LogError($"Error type, PlayStableAnimation has not {playerState.GetType().Name} animation type!");
                break;
        }
    }

    public void PlayOneTimeAnimation(IState playerState)
    {
        TrackEntry currentStableAnim = SkeletonAnim.AnimationState.GetCurrent(StableAnimTrack);
        TrackEntry currentActiveAnim = SkeletonAnim.AnimationState.GetCurrent(ActiveAnimTrack);

        switch (playerState.GetType().Name)
        {
            case "LandState":
                SoftSetAnimation(StableAnimTrack, currentStableAnim, Land, false);
                break;
            case "JumpState":
                SoftSetAnimation(StableAnimTrack, currentStableAnim, Jump, false);
                break;
            case "MelleAttackState":
                SoftSetAnimation(StableAnimTrack, currentStableAnim, MeleeAttackAnim, false);
                //SoftSetAnimation(activeAnimTrack, currentActiveAnim, MeleeAttackAnim, false);
                //SkeletonAnim.AnimationState.AddEmptyAnimation(0, 0.05, currentStableAnim.Animation.Duration - 0.04f);
                break;
            case "RangeAttackState":
                SoftSetAnimation(StableAnimTrack, currentStableAnim, RangeAttackAnim, false, true);
                //SoftSetAnimation(activeAnimTrack, currentActiveAnim, RangeAttackAnim, false, true);
                //SkeletonAnim.AnimationState.AddEmptyAnimation(0, 0.02, currentStableAnim.Animation.Duration - 0.01f);
                break;
            case "TakeDamageState":
                SoftSetAnimation(StableAnimTrack, currentStableAnim, TackeDamageAnim, false);
                break;
            case "DeathState":
                SoftSetAnimation(StableAnimTrack, currentStableAnim, Death, false);
                break;
            case "FinishState":
                SoftSetAnimation(StableAnimTrack, currentStableAnim, Win, false);
                break;
            case "RespawnState":
                SoftSetAnimation(StableAnimTrack, currentStableAnim, Respawn, false);
                break;
            default:
                Debug.LogError($"Error type, PlayStableAnimation has not {playerState.GetType().Name} animation type!");
                break;
        }
    }

    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        // Debug.Log("Event fired! " + e.Data.Name);
        //bool eventMatch = string.Equals(e.Data.Name, eventName, System.StringComparison.Ordinal); // Testing recommendation: String compare.
        bool eventMatch = (_walkEventData == e.Data); // Performance recommendation: Match cached reference instead of string.
        if (eventMatch)
        {
            SoundManager.PlaySfx(Player.WalkSound);
        }
    }

    private void SoftSetAnimation(int trakIndex, TrackEntry cur, Spine.Animation animation, bool loop, bool hardSet = false)
    {
        if (cur == null || cur.Loop || hardSet)
            SkeletonAnim.AnimationState.SetAnimation(trakIndex, animation, loop);
        else
            SkeletonAnim.AnimationState.AddAnimation(trakIndex, animation, loop, 0);
    }

}
