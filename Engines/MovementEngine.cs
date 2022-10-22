using Godot;
using System;

public class MovementEngine : Node2D
{
    private Vector2 _goingFor; // The vector the movement engine is actually going for.
    private float _speed; // Speed the movement engine uses on the entity.
    private float _acceleration; // Acceleration the movement engine uses.
    private float _friction; // Friction the movement engine uses.
    private float _rotation; // The rotation that the movement engine is applying to the movement vector.
    private PhysicsBody2D _repulsedTarget; // The physics body the movement engine will avoid.
    private float _repulsedLen; // How far should the movement engine be repulsed?
    private bool _repulsed = false; // Is the engine currently repulsed by something.
    private Vector2 _startPoint; // Where the engine should wander around.
    private float _wanderDistance; // How far the engine will wander.
    private float _circleModifier; // Modifys how the engine orbits the target. If set to '-1' will reverse the orbit.

    // The possible states of the movement engine:
    private enum _states
    {
        HUNT,
        CIRCLE,
        WANDER,
        IDLE
    }

    // Allows to set the state of the movement engine.
    private _states _state;

    // ****
    // ****
    // Public Funcs important to use:
    
    public Vector2 Target { get; set; } // The target of the movement engine.

    // Sets the movement stats of the engine, the default speed, accel. and friction.
    public void SetMovementStats(float speed, float acceleration, float friction)
    {
        _speed = speed;
        _acceleration = acceleration;
        _friction = friction;
    }
    
    // Set the engine to hunt the target.
    public void SetStateHunt()
    {
        _state = _states.HUNT;
    }

    // Set the engine to circle / orbit the target. The modifier modifys the orbit of the enitiy, recommended to only use -1 or 1
    // to change whether the orbit is clockwise or counter-clockwise.
    public void SetStateCircle(float modifier)
    {
        _state = _states.CIRCLE;
        
        _circleModifier = modifier;
    }

    // Set the engine to idle, which will then wander.
    public void SetStateIdle()
    {
        _state = _states.IDLE;
    }

    // Get the actual movement vector of the engine.
    // Used to move the enitity.
    public Vector2 GetMovementVector(float delta)
    {
        return _goingFor;
    }

    // Sets the Physics body that the movement engine will avoid, and the distance of that repulsion.
    public void RepulsedBy(PhysicsBody2D replusedBy, float repulsionLen)
    {
        _repulsedTarget = replusedBy;
        _repulsedLen = repulsionLen;
        _repulsed = true;
    }

    public void SetWanderStats(Vector2 startPoint, float wanderDistance)
    {
        _startPoint = startPoint;
        _wanderDistance = wanderDistance;
    }

    // Public Funcs important to use:
    // ****
    // ****

    public override void _Ready()
    {
        // When the game starts, the default state is idle.
        _state = _states.IDLE;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        
        switch (_state)
        {
            case _states.HUNT:
                
                _goingFor = Hunt(delta);
                
                // If the engine should check that it should be repulsed.
                if (_repulsed)
                {
                    Repluse(delta);
                }

                break;
            case _states.IDLE:
                
                // Has the idle stopped moving? If so, start to wander.
                if (_goingFor.Length() == 0)
                {
                    _state = _states.WANDER;

                    break;
                }
                
                _goingFor = Idle(delta);

                break;

            case _states.CIRCLE:

                _goingFor = Circle(delta);

                // If the engine should check that it should be repulsed.
                if (_repulsed)
                {
                    Repluse(delta);
                }

                break;

            case _states.WANDER:
                
                
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
        
        return _goingFor.MoveToward(GlobalPosition.DirectionTo(Target).Rotated(NINETY_DEGREES * _circleModifier).Normalized() * _speed, delta * _acceleration);
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
    
    // Wanders the engine randomly around the StartPoint with the Wander distance.
    private Vector2 Wander(float delta)
    {
        if (!GetNode<Timer>("WanderTimer").IsStopped())
        {
            return _goingFor;
        }

        GetNode<Timer>("WanderTimer").Start(3.0f);
        
        Vector2 whereTo = GlobalPosition.DirectionTo(_startPoint);
        whereTo.x += (float)GD.RandRange(-_wanderDistance, _wanderDistance);
        whereTo.y += (float)GD.RandRange(-_wanderDistance, _wanderDistance);

        // The *2 is bc otherwise it is too slow. Why this is I dont know, just is.
        return _goingFor.MoveToward(whereTo.Normalized() * _speed, delta * _acceleration * 2);
    }
}
