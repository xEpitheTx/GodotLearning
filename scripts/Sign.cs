using Godot;
using System;

public partial class Sign : StaticBody2D
{
	[Export] public string Text { get; set; } = String.Empty;
    private Label _label;

    public override void _Ready()
    {
        // The sign itself: solid (world) and readable (interactable)
        CollisionLayer = Layers.Bits(Layers.World, Layers.Interactable);

        _label = GetNode<Label>("Label");
        _label.Visible = false;

        // The circle around the sign: isn't anything, only watches for the player
        Area2D area = GetNode<Area2D>("Area2D");
        area.CollisionLayer = 0;
        area.CollisionMask = Layers.Bits(Layers.Player);
        area.BodyExited += OnBodyExited;
    }

    public void Read()
    {
        _label.Text = Text;
        _label.Visible = true;
    }
    private void OnBodyExited(Node2D body)
    {
        _label.Visible = false;
    }
}
