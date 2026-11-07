using Godot;

public enum Team { Player, Enemy }

[GlobalClass]
public partial class Hitbox : Area2D
{
    [Export] public int Damage { get; set; } = 1;
    [Export] public Team Team { get; set; } = Team.Player;

    public override void _Ready()
    {
        CollisionLayer = Team == Team.Player ? Layers.Bits(Layers.PlayerAttack) : Layers.Bits(Layers.EnemyAttack);
        CollisionMask = Layers.Bits(0);
    }
}