using Godot;
using System;

public class Player : KinematicBody2D
{
	
	private static Node _animationTree;
	private static AnimationNodeStateMachinePlayback _animationState;
	private static States _playerState;     // The state the player is in, ex: "Dead" or "Move". Used in the main physics loop.
	private static Vector2 _playerVelocity; // The velocity of the player.
	private static PackedScene _bullet; // The bullet scene thats used to create bullets when shooting.
	private const float _BULLET_PUSHBACK = -70; // How much the bullet pushes you when the player shoots.
	private const float _DEFAULT_SPEED = 50; // The max speed of the player.
	private const float _ACCELERATION = 140; // Rate of acceleration the player uses.
	private const float _FRICTION = 240;     // Rate of friction the player uses.
	private Vector2 _spawnpoint; // Where does the player re-spawn?

	// ****
	// ****
	// Public Variables of the player:

	public static uint Health { get; set; } // Health of the player?
	public static uint Souls { get; set; } // How many souls does the player have?
	public static float Damage { get; set; } // How much damage does the player do?
	public static uint WaveNum { get; set; } // Current wave
	public static bool Reloading { get; set; } // Is the player reloading?
	public static Vector2 PlayerPosition { get; set; } // Where is the player.

	// Public Variables of the player:
	// ****
	// ****
	
	private enum States 
	{
		Move,
		Shooting,
		Dying
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

		_playerState = States.Move;

		_playerVelocity = Vector2.Zero;
		
		_bullet = ResourceLoader.Load<PackedScene>("res://Player/Game/Bullet.tscn");

		_spawnpoint = GlobalPosition;

		Health = 5;
		Souls = 0;
		Damage = 1;

		Reloading = false;

		PlayerPosition = GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsMouseButtonPressed(1) && GetNode<Timer>("ShootTimer").IsStopped())
		{
			
			_playerState = States.Shooting;
		}

		if (Health == 0 && _playerState != States.Dying)
		{
			_playerState = States.Dying;
			Death();
		}
		
		switch (_playerState)
		{
			case States.Move:
				MovePlayer(delta);
				break;

			case States.Shooting:
				Shoot(delta);

				break;

			case States.Dying:
				break;
		}

		PlayerPosition = GlobalPosition;
	}
	
	// Moves the player with the given delta value.
	private void MovePlayer(float delta)
	{
		// Define the inputVector as (0, 0).
		var inputVector = Vector2.Zero;

		// Lets get the users inputs as a vector into inputVector,
		inputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		inputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
		inputVector = inputVector.Normalized();

		// If the input is 0, have the players velocity approach 0.
		if (inputVector == Vector2.Zero)
		{
			// Set the Travel state to idle.
			_animationState.Travel("Idle");

			// Update the velocity to approach 0.
			_playerVelocity = _playerVelocity.MoveToward(inputVector, delta * _FRICTION);
		} 
		// The player is moving.
		else 
		{
			// Set the inputVector into the animation tree, that way it knows how to orientate the sprite, right or left ect.
			_animationTree.Set("parameters/Idle/blend_position", inputVector);
			_animationTree.Set("parameters/Run/blend_position", inputVector);

			// Set the state to running.
			_animationState.Travel("Run");

			// Update the velocity to approach the inputVector.
			_playerVelocity = _playerVelocity.MoveToward(inputVector * _DEFAULT_SPEED, delta * _ACCELERATION);

			// Play the walking sound of the player.
			PlayFootStep();
		}

		// Move the player with the new velocity.
		MoveAndSlide(_playerVelocity);
	}

	// Plays the footstep sound of the player when called.
	private void PlayFootStep()
	{
		
		// If the StepTimer has run out, play the sound.
		if (GetNode<Timer>("StepTimer").IsStopped())
		{
			// Add some pitch variation.
			GetNode<AudioStreamPlayer>("StepPlayer").PitchScale = (float)GD.RandRange(0.95, 1.05);
			
			// Play the sound.
			GetNode<AudioStreamPlayer>("StepPlayer").Play();

			// Restart the timer so this sound can only be played every 1.1 seconds.
			GetNode<Timer>("StepTimer").Start(1.0f);
		}
	}

	private void Shoot(float delta)
	{

		Reloading = true;
		
		GetNode<Timer>("ShootTimer").Start(2.0f);

		ShootSound();

		KinematicBody2D bullet1 = _bullet.Instance<Bullet>();
		KinematicBody2D bullet2 = _bullet.Instance<Bullet>();
		KinematicBody2D bullet3 = _bullet.Instance<Bullet>();

		GetParent().AddChild(bullet1);
		GetParent().AddChild(bullet2);
		GetParent().AddChild(bullet3);

		bullet1.GlobalPosition = GetNode<Node2D>("BulletSpawn").GlobalPosition;
		bullet2.GlobalPosition = GetNode<Node2D>("BulletSpawn").GlobalPosition;
		bullet3.GlobalPosition = GetNode<Node2D>("BulletSpawn").GlobalPosition;

		Vector2 mouseGlobal = GetGlobalMousePosition();

		bullet1.Call("SetVelocity", bullet1.GlobalPosition.DirectionTo(mouseGlobal));
		bullet2.Call("SetVelocity", bullet1.GlobalPosition.DirectionTo(mouseGlobal).Rotated(3.14f / (float)GD.RandRange(6, 24)));
		bullet3.Call("SetVelocity", bullet1.GlobalPosition.DirectionTo(mouseGlobal).Rotated(-3.14f / (float)GD.RandRange(6, 24)));

		_playerVelocity = _playerVelocity.MoveToward(GetLocalMousePosition().Normalized() * _BULLET_PUSHBACK, _ACCELERATION);

		_animationTree.Set("parameters/Shoot/blend_position", GetLocalMousePosition().Normalized());
		_animationState.Travel("Shoot");

		MoveAndSlide(_playerVelocity);

		_playerState = States.Move;
	}

	private void ShootSound()
	{
		GetNode<AudioStreamPlayer>("Shotgun").Play();
	}

	private void _on_ShootTimer_timeout()
	{
		Reloading = false;
	}

	private void Death()
	{
		GetNode<AudioStreamPlayer>("DeathSound").Play();

		MoveAndSlide(new Vector2(0, 0));

		_animationState.Travel("Death");
	}

	public void Reset()
	{
		Health = 5;
		Souls = 0;
		Damage = 1.0f;

		GlobalPosition = _spawnpoint;

		_playerState = States.Move;

		WaveNum = 0;

		GetNode<Timer>("WaveTimer").Start(20.0f);
	}

	private void _on_WaveTimer_timeout()
	{
		WaveNum += 1;

		GetNode<Timer>("WaveTimer").Start(20.0f);
	}
}
