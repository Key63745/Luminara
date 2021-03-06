using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState, IRootState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        HandleJump();
    }
    
    public override void UpdateState() 
    {
        HandleGravity();
        CheckSwitchStates();
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
        if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    void HandleJump()
    {
        Ctx.IsJumping = true;
        Ctx.IsGrounded = false;
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }

    public void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        float fallSpeed = 2.0f;

        if (isFalling)
        {
            float prevYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * fallSpeed * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((prevYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);
            float DistanceToTheGround = Ctx.GetComponent<Collider>().bounds.extents.y;
            Ctx.IsGrounded = Physics.Raycast(Ctx.transform.position, Vector3.down, DistanceToTheGround + 0.1f, Ctx.GroundMask);
        } 
        else
        {
            float prevYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY = (prevYVelocity + Ctx.CurrentMovementY) * .5f;
        }
    }
}
