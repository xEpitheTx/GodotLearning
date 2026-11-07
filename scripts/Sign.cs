using Godot;
using System;

public partial class Sign : StaticBody2D
{
	[Export] public string Text { get; set; } = String.Empty;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Read()
    {
        Label label = GetNode<Label>("Label");
		label.Text = Text;
        label.Visible = true;
    }
}
