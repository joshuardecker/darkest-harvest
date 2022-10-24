using Godot;
using System;

public class Spawner : Node2D
{
    private uint _wave;
    private const float _MAX_DISTANCE = 100.0f;
    private static PackedScene _ghost = ResourceLoader.Load<PackedScene>("res://Enemies/Game/Ghost.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _wave = 0;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {  
        if (Player.WaveNum != _wave)
        {
            _wave = Player.WaveNum;

            NextWave();
        }
    }

    private void NextWave()
    {

        if (_wave == 0)
        {
            foreach (Node child in GetChildren())
            {
                child.QueueFree();
            }

            return;
        }
        
        if (GlobalPosition.DistanceTo(Player.PlayerPosition) <= _MAX_DISTANCE)
        {
            var ghost = _ghost.Instance();

            AddChild(ghost);
        }
    }
}
