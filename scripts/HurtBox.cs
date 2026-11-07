using Godot;

[GlobalClass]
public partial class HurtBox : Area2D
{
    [Export] public int Damage { get; set; } = 1;
    [Export] public Team Team { get; set; } = Team.Player;

    public override void _Ready()
    {
        CollisionLayer = Layers.Bits(0);
        CollisionMask = Team == Team.Player ? Layers.Bits(Layers.EnemyAttack) : Layers.Bits(Layers.PlayerAttack);
        HurtBox hurtBox = new HurtBox();
        hurtBox.AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Hitbox hitbox && hitbox.Team != Team)
        {
            GD.Print($"HurtBox hit by {hitbox.Team} for {hitbox.Damage} damage!");
            // Here you would typically call a method to apply damage to the entity this HurtBox belongs to.
        }
    }
}