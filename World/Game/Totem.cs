using Godot;
using System;

public class Totem : StaticBody2D
{

    [Export]
    public uint WaveDestroyed = 10;

    [Export]
    public float Health = 1;

    private bool _attackable;
    private bool _faded;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");

        _attackable = false;
        _faded = false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        
        if (Player.WaveNum == 0)
        {
            Show();
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;

            GetNode<Light2D>("HeadLight").Show();
        GetNode<Light2D>("AreaLight").Show();
            
            GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
            
            _attackable = false;
            _faded = false;
        }
        
        if (WaveDestroyed == Player.WaveNum && !_faded)
        {
            _attackable = true;
            _faded = true;

            GetNode<AnimationPlayer>("AnimationPlayer").Play("Fade");
        }
    }

    // Called after the fade animation is played.
    private void Fade()
    {
        GetNode<Light2D>("HeadLight").Hide();
        GetNode<Light2D>("AreaLight").Hide();
    }

    private void _on_Area2D_area_entered(Area2D area)
    {
        if (!_attackable)
        {
            return;
        }
        
        Health -= Player.Damage;

        if (Health <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Break");
    }

    // Called after the death animation.
    private void Dead()
    {
        Hide();

        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
    }
}
