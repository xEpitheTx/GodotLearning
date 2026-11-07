using Godot;
using System;

public partial class Spinner : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Spinner is ready!");
		GetNode<Timer>("Timer").Timeout += OnTimerTimeout;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	[Export] public float SpinSpeed { get; set; } = 2.0f; // Radians per second
    public override void _Process(double delta)
	{
        Rotation += SpinSpeed * (float)delta; 
    }

	private void OnTimerTimeout()
    {
        SpinSpeed = -SpinSpeed;
		GD.Print("Spin direction changed! New SpinSpeed: " + SpinSpeed);
    }
}
