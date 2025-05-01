
public enum PlayerAnimationState
{
    Standby1,
    Standby2,
    Jump
}

// public class AnimationStateMachine
// {
//     private PlayerAnimationState currentState;
//     private Action<PlayerAnimationState> onStateChanged;

//     public AnimationStateMachine(Action<PlayerAnimationState> stateChangedCallback)
//     {
//         onStateChanged = stateChangedCallback;
//         currentState = PlayerAnimationState.Standby1; // 默认状态
//     }

//     public void UpdateState(Func<PlayerAnimationState, PlayerAnimationState?> stateTransitionLogic)
//     {
//         var newState = stateTransitionLogic(currentState);
//         if (newState.HasValue && newState.Value != currentState)
//         {
//             TransitionTo(newState.Value);
//         }
//     }

//     public void TransitionTo(PlayerAnimationState newState)
//     {
//         if (currentState == newState)
//             return;

//         currentState = newState;
//         onStateChanged?.Invoke(newState);
//     }

//     public PlayerAnimationState GetCurrentState() => currentState;
// }