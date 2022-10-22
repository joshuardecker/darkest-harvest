using Godot;
using System;

public class Ghost : KinematicBody2D
{
    private const float _SPEED = 40;
    private const float _ACCELERATION = 100;
    private const float _FRICTION = 20;
    private bool _trackTarget = false;
    private PhysicsBody2D _body;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<MovementEngine>("MovementEngine").SetMovementStats(_SPEED, _ACCELERATION, _FRICTION);
        GetNode<MovementEngine>("MovementEngine").State = MovementEngine.States.IDLE;
        GetNode<MovementEngine>("MovementEngine").WanderDistance = 10;
        GetNode<MovementEngine>("MovementEngine").StartPoint = GlobalPosition;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        if (_trackTarget)
        {
            GetNode<MovementEngine>("MovementEngine").Target = _body.GlobalPosition;
        }
        
        var moveVector = GetNode<MovementEngine>("MovementEngine").GetMovementVector(delta);

        MoveAndSlide(moveVector);
    }

    private void _on_PlayerDetection_body_entered(PhysicsBody2D body)
    {
        _trackTarget = true;

        _body = body;

        const float REPULSION_DIS = 50.0f;

        GetNode<MovementEngine>("MovementEngine").State = MovementEngine.States.CIRCLE;
        GetNode<MovementEngine>("MovementEngine").RepulsedBy(_body, REPULSION_DIS);
    }

    private void _on_PlayerDetection_body_exited(PhysicsBody2D body)
    {
        _trackTarget = false;

        GetNode<MovementEngine>("MovementEngine").State = MovementEngine.States.IDLE;
    }
}
