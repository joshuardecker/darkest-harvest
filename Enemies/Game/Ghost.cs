using Godot;
using System;

public class Ghost : KinematicBody2D
{
    private const float _SPEED = 40;
    private const float _ACCELERATION = 100;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<MovementEngine>("MovementEngine")._target = new Vector2(1, 0);
        GetNode<MovementEngine>("MovementEngine").SetMovementStats(_SPEED, _ACCELERATION);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        var moveVector = GetNode<MovementEngine>("MovementEngine").GetMovementVector(delta);

        MoveAndSlide(moveVector);
    }
}
