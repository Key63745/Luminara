using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : MonoBehaviour
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    public override void EnterState()
    {
        _ctx.CurrentMovementY = _ctx.GroundedGravity;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void InitializeSubState() 
    {
        if (!_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Idle());
        } else if ()
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.isJumpPressed && !_ctx.RequireNewJumpPress)
        {
            SwitchState(_factory.Jump());
        }
    }
}
