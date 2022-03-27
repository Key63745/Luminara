using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        HandleJump();
    }
    
    public override void UpdateState() 
    {
        CheckSwitchStates();
        HandleGravity();
    }

    public override void ExitState()
    {
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates() 
    { 
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    void HandleJump()
    {
        Ctx.IsJumping = true;
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }

    void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        float fallSpeed = 2.0f;

        if (isFalling)
        {
            float prevYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.JumpGravity * fallSpeed * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((prevYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);
        } 
        else
        {
            float prevYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.JumpGravity * Time.deltaTime);
            Ctx.AppliedMovementY = (prevYVelocity + Ctx.CurrentMovementY) * .5f;
        }
    }
}
