using Godot;
using System;

public class MovementEngine : Node2D
{

    public Vector2 _target { get; set;} // The target of the movement engine.
    private Vector2 _goingFor; // The vector the movement engine is actually going for.
    private float _speed;
    private float _acceleration;
    private float _rotation;

    // Get the actual movement vector of the engine.
    // Used to move the enitity.
    public Vector2 GetMovementVector(float delta)
    {
        return _goingFor;
    }

    public void SetMovementStats(float speed, float acceleration)
    {
        _speed = speed;
        _acceleration = acceleration;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        _goingFor = _goingFor.MoveToward(GlobalPosition.DirectionTo(_target).Normalized() * _speed, delta * _acceleration);

        _goingFor.Rotated(_rotation);
    }

    private void _on_SwitchTimer_timeout()
    {
        GetNode<Timer>("SwitchTimer").Start(3.0f);
        
        // Have a 20% chance that the direction Vector gets rotated slightly.
        var rand = GD.RandRange(0, 10); // Rand number between 0-9.
        
        // Constands used in rotation (in radians).
        const float PI = 3.14f;
        const float ROTATE = 3.0f;

        // 20% chance rotating by the constants above.
        if (rand == 0 || rand == 1)
        {
            _rotation = PI / ROTATE;
        }
        // 20% chance rotating by the negative value of the cosntants above.
        else if (rand == 2 || rand == 3)
        {
            _rotation = -PI / ROTATE;
        }
        else
        {
            _rotation = 0;
        }

        GD.Print(_rotation);
    }
}
