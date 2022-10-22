using Godot;
using System;

public class Bullet : KinematicBody2D
{
    private Vector2 _velocity; // Velocity of the bullet.

    private const float _bulletSpeed = 4; // The speed multiplier of the bullet.
    
    // Set the velocity of the bullet.
    public void SetVelocity(Vector2 v)
    {
        // Normalizes the vector to make any inputs length 1, aka making the velocity from any input vector equal.
        _velocity = v.Normalized();
    }

    // Move the bullet every frame.
    public override void _PhysicsProcess(float delta)
    {
        MoveAndCollide(_velocity * _bulletSpeed);
    }

    // When the bullet has existed for 3 seconds, delete it.
    private void _on_BulletTimer_timeout()
    {
        QueueFree();
    }
}
