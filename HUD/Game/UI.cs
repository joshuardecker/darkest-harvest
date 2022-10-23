using Godot;
using System;

public class UI : Control
{
    private static Texture heart = ResourceLoader.Load<Texture>("res://HUD/Assets/heart.png");
    private static Texture brokenHeart = ResourceLoader.Load<Texture>("res://HUD/Assets/broken_heart.png");
    private static Texture shotgun = ResourceLoader.Load<Texture>("res://HUD/Assets/shotgun.png");
    private static Texture reloadingShotgun = ResourceLoader.Load<Texture>("res://HUD/Assets/shotgun_reloading.png");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        // ****
        // Wave Number:

        if (Player.WaveNum < 1)
        {
            GetNode<Label>("WaveNum").Text = "Prepare!";
        }
        else
        {
            GetNode<Label>("WaveNum").Text = "Wave " + Player.WaveNum.ToString();
        }

        // Wave Number:
        // ****

        // ****
        // Health:

        if (Player.Health == 5)
        {
            GetNode<TextureRect>("Heart1").Texture = heart;
            GetNode<TextureRect>("Heart2").Texture = heart;
            GetNode<TextureRect>("Heart3").Texture = heart;
            GetNode<TextureRect>("Heart4").Texture = heart;
            GetNode<TextureRect>("Heart5").Texture = heart;
        }
        else if (Player.Health == 4)
        {
            GetNode<TextureRect>("Heart1").Texture = heart;
            GetNode<TextureRect>("Heart2").Texture = heart;
            GetNode<TextureRect>("Heart3").Texture = heart;
            GetNode<TextureRect>("Heart4").Texture = heart;
            GetNode<TextureRect>("Heart5").Texture = brokenHeart;
        }
        else if (Player.Health == 3)
        {
            GetNode<TextureRect>("Heart1").Texture = heart;
            GetNode<TextureRect>("Heart2").Texture = heart;
            GetNode<TextureRect>("Heart3").Texture = heart;
            GetNode<TextureRect>("Heart4").Texture = brokenHeart;
            GetNode<TextureRect>("Heart5").Texture = brokenHeart;
        }
        else if (Player.Health == 2)
        {
            GetNode<TextureRect>("Heart1").Texture = heart;
            GetNode<TextureRect>("Heart2").Texture = heart;
            GetNode<TextureRect>("Heart3").Texture = brokenHeart;
            GetNode<TextureRect>("Heart4").Texture = brokenHeart;
            GetNode<TextureRect>("Heart5").Texture = brokenHeart;
        }
        else if (Player.Health == 1)
        {
            GetNode<TextureRect>("Heart1").Texture = heart;
            GetNode<TextureRect>("Heart2").Texture = brokenHeart;
            GetNode<TextureRect>("Heart3").Texture = brokenHeart;
            GetNode<TextureRect>("Heart4").Texture = brokenHeart;
            GetNode<TextureRect>("Heart5").Texture = brokenHeart;
        }
        else 
        {
            GetNode<TextureRect>("Heart1").Texture = brokenHeart;
            GetNode<TextureRect>("Heart2").Texture = brokenHeart;
            GetNode<TextureRect>("Heart3").Texture = brokenHeart;
            GetNode<TextureRect>("Heart4").Texture = brokenHeart;
            GetNode<TextureRect>("Heart5").Texture = brokenHeart;
        }

        // Health:
        // ****

        // ****
        // Reloading:

        if (Player.Reloading)
        {
            GetNode<TextureRect>("Shotgun").Texture = reloadingShotgun;
        }
        else 
        {
            GetNode<TextureRect>("Shotgun").Texture = shotgun;
        }

        // Reloading:
        // ****
    }
}
