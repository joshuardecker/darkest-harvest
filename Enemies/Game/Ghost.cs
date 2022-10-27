using Godot;
using System;

public class Ghost : KinematicBody2D
{
    private const float _SPEED = 40;
    private const float _ACCELERATION = 100;
    private const float _FRICTION = 20;
    private bool _trackTarget = false;
    private PhysicsBody2D _body;
    private Vector2 _moveVector;
    private float _health;
    private bool _alive;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<MovementEngine>("MovementEngine").SetMovementStats(_SPEED, _ACCELERATION, _FRICTION);
        GetNode<MovementEngine>("MovementEngine").SetStateIdle();
        
        const float WANDER_DISTANCE = 10;

        GetNode<MovementEngine>("MovementEngine").SetWanderStats(GlobalPosition, WANDER_DISTANCE);

        _health = 2 + (Player.WaveNum * 0.1f); // Goes up 1 health every 10 waves.

        _alive = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        if (_health <= 0 && _alive)
        {
            _alive = false;

            Dying();
        }
        
        if (_trackTarget)
        {
            GetNode<MovementEngine>("MovementEngine").Target = _body.GlobalPosition;
        }
        
        _moveVector = GetNode<MovementEngine>("MovementEngine").GetMovementVector(delta);

        MoveAndSlide(_moveVector);
    }

    private void _on_PlayerDetection_body_entered(PhysicsBody2D body)
    {
        _trackTarget = true;
        _body = body;

        GetNode<MovementEngine>("MovementEngine").SetStateHunt();
    }

    private void _on_PlayerDetection_body_exited(PhysicsBody2D body)
    {
        _trackTarget = false;

        GetNode<MovementEngine>("MovementEngine").SetStateIdle();
    }

    private void Dying()
    {
        GetNode<Area2D>("Hurtbox").QueueFree();
        GetNode<Area2D>("Hitbox").QueueFree();
        GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();
        GetNode<Light2D>("Light2D").Energy = 0.3f;

        GetNode<MovementEngine>("MovementEngine").SetStateIdle();

        GetNode<AnimationPlayer>("AnimationPlayer").Play("Death");

        GetNode<AudioStreamPlayer2D>("DeathSound").Play();

        GetNode<Timer>("DeathTimer").Start(3.0f);

        Player.Souls += 1;
    }

    public void DeleteSprite()
    {
        GetNode<Sprite>("Sprite").QueueFree();
        GetNode<Light2D>("Light2D").Hide();
    }
    
    public void _on_DeathTimer_timeout()
    {
        QueueFree();
    }

    // When the ghost hits the player.
    private void _on_Hurtbox_area_entered(Area2D area)
    {   
        Player.Health -= 1;

        Dying();
    }

    private void _on_Hitbox_area_entered(Area2D area)
    {
        _health -= Player.Damage;
    }
}
