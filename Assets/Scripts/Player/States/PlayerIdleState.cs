using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
    }

    public override void UpdateState()
    {
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void InitializeSubState() {}

    public override void CheckSwitchStates()
    {
        if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        } else if (Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
}
