using Godot;
using System;

public class MovementEngine : Node2D
{

    public Vector2 Target { get; set; } // The target of the movement engine.
    private Vector2 _goingFor; // The vector the movement engine is actually going for.
    private float _speed; // Speed the movement engine uses on the entity.
    private float _acceleration; // Acceleration the movement engine uses.
    private float _friction; // Friction the movement engine uses.
    private float _rotation; // The rotation that the movement engine is applying to the movement vector.
    private PhysicsBody2D _repulsedTarget; // The physics body the movement engine will avoid.
    private float _repulsedLen; // How far should the movement engine be repulsed?
    private bool _repulsed = false; // Is the engine currently repulsed by something.

    // The possible states of the movement engine:
    public enum States
    {
        HUNT,
        CIRCLE,
        WANDER,
        IDLE
    }

    // Allows to set the state of the movement engine.
    public States State {get; set;}

    // Get the actual movement vector of the engine.
    // Used to move the enitity.
    public Vector2 GetMovementVector(float delta)
    {
        return _goingFor;
    }

    public override void _Ready()
    {
        // When the game starts, the default state is idle.
        State = States.IDLE;
    }

    // Sets the movement stats of the engine, the default speed, accel. and friction.
    public void SetMovementStats(float speed, float acceleration, float friction)
    {
        _speed = speed;
        _acceleration = acceleration;
        _friction = friction;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        
        switch (State)
        {
            case States.HUNT:
                
                _goingFor = Hunt(delta);
                
                // If the engine should check that it should be repulsed.
                if (_repulsed)
                {
                    Repluse(delta);
                }

                break;
            case States.IDLE:
                
                // Has the idle stopped moving? If so, start to wander.
                if (_goingFor.Length() == 0)
                {
                    State = States.WANDER;

                    break;
                }
                
                _goingFor = Idle(delta);

                break;

            case States.CIRCLE:

                _goingFor = Circle(delta);

                // If the engine should check that it should be repulsed.
                if (_repulsed)
                {
                    Repluse(delta);
                }

                break;

            case States.WANDER:
                
                
                _goingFor = Wander(delta);
                
                break;
        }
    }

    // Hunts the target vector.
    private Vector2 Hunt(float delta)
    {
        return _goingFor.MoveToward(GlobalPosition.DirectionTo(Target).Rotated(_rotation).Normalized() * _speed, delta * _acceleration);
    }

    // Every 3 seconds while hunting, the path can be slightly rotated, making the movements slightly more random.
    // Very small feature but adds a nice subtle hint of polish.
    private void _on_SwitchTimer_timeout()
    {
        GetNode<Timer>("SwitchTimer").Start(3.0f);
        
        var rand = GD.RandRange(6, 12); 
        
        // Constands used in rotation (in radians).
        const float PI = 3.14f;
        
        _rotation = PI / (float)rand;
    }

    // The idle state, aka slow down to 0.
    private Vector2 Idle(float delta)
    {
        return _goingFor.MoveToward(new Vector2(0, 0), delta * _friction);
    }

    // Circles the target.
    private Vector2 Circle(float delta)
    {
        const float NINETY_DEGREES = 3.14f / 2.0f;
        
        return _goingFor.MoveToward(GlobalPosition.DirectionTo(Target).Rotated(NINETY_DEGREES).Normalized() * _speed, delta * _acceleration);
    }

    // Sets the Physics body that the movement engine will avoid, and the distance of that repulsion.
    public void RepulsedBy(PhysicsBody2D replusedBy, float repulsionLen)
    {
        _repulsedTarget = replusedBy;
        _repulsedLen = repulsionLen;
        _repulsed = true;
    }

    // Repulses the engine away from the repulsion entity if this entity is within the repulsion length.
    private void Repluse(float delta)
    {
        if (GlobalPosition.DistanceTo(_repulsedTarget.GlobalPosition) > _repulsedLen)
        {
            return;
        }

        _goingFor = _goingFor.MoveToward(GlobalPosition.DirectionTo(_repulsedTarget.GlobalPosition) * _speed * -1, delta * _acceleration);
    }

    // Where the engine should wander around.
    public Vector2 StartPoint { get; set; }
    // How far the engine will wander.
    public float WanderDistance { get; set; }
    
    // Wanders the engine randomly around the StartPoint with the Wander distance.
    private Vector2 Wander(float delta)
    {
        if (!GetNode<Timer>("WanderTimer").IsStopped())
        {
            return _goingFor;
        }

        GetNode<Timer>("WanderTimer").Start(3.0f);
        
        Vector2 whereTo = GlobalPosition.DirectionTo(StartPoint);
        whereTo.x += (float)GD.RandRange(-WanderDistance, WanderDistance);
        whereTo.y += (float)GD.RandRange(-WanderDistance, WanderDistance);

        // The *2 is bc otherwise it is too slow. Why this is I dont know, just is.
        return _goingFor.MoveToward(whereTo.Normalized() * _speed, delta * _acceleration * 2);
    }
}
