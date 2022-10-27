using Godot;
using System;

public class Alter : StaticBody2D
{

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        
    }

    private void _on_AlterBox_area_entered(Area2D area)
    {
        if (Player.Souls >= GetSoulsCost())
        {
            Player.Souls -= GetSoulsCost();

            Player.Damage += 0.5f;

            GetNode<Label>("CostLabel").Text = GetSoulsCost().ToString() + " Souls";

            GetNode<AnimationPlayer>("AnimationPlayer").Play("Use");

            // Reset the players health to heal them.
            Player.Health = 5;
        }
    }

    private uint GetSoulsCost()
    {
        const uint COST_PER_LEV = 25;

        // Ex: If the player level 1:
        // Damage = 1 * 2 which = 2:
        // 2 - 1 = 1:
        // * 25 = the starting cost of 25 souls to upgrade.
        return (uint)(COST_PER_LEV * ((Player.Damage * 2) - 1));
    }
}
