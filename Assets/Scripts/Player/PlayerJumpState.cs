using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

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
        if (_ctx.IsJumpPressed)
        {
            _ctx.RequireNewJumpPress = true;
        }
    }

    public override void IntiializeSubState() { }

    public override void CheckSwitchStates() 
    { 
        if (_ctx.CharacterController.isGrounded)
        {
            CheckSwitchStates(_factory.Grounded());
        }
    }

    void HandleJump()
    {
        _ctx.isJumping = true;
        _ctx.CurrentMovementY = _ctx.InitialJumpVelocity;
        _ctx.AppliedMovementY = _ctx.InitialJumpVelocity;
    }

    void HandleGravity()
    {
        bool isFalling = _ctx.CurrentMovementY <= 0.0f || _!ctx.IsJumpPressed;
        float fallSpeed = 2.0f;

        if (isFalling)
        {
            float prevYVelocity = _ctx.CurrentMovementY;
            _ctx.CurrentMovementY = _ctx.CurrentMovementY + (* fallSpeed * Time.deltaTime);
            _ctx.AppliedMovementY = Mathf.Max((prevYVelocity + _ctx.CurrentMovementY) * .5f, -20.0f);
        } 
        else
        {
            float prevYVelocity = _ctx.CurrentMovementY;
            _ctx.CurrentMovementY = _ctx.CurrentMovementY + (* fallSpeed * Time.deltaTime);
            _ctx.AppliedMovementY = (prevYVelocity + _ctx.CurrentMovementY) * .5f;
        }
    }
}
