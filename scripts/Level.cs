using Godot;
using System;

public partial class Level : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetCameraLimit();
    }

    private void SetCameraLimit()
    {
        var ground = GetNode<TileMapLayer>("Ground");
        var camera = GetNode<Camera2D>("Entities/Player/Camera2D");

        Rect2I cells = ground.GetUsedRect();   // painted area, in tiles
        int tile = 16;

        camera.LimitLeft = cells.Position.X * tile;
        camera.LimitTop = cells.Position.Y * tile;
        camera.LimitRight = cells.End.X * tile;
        camera.LimitBottom = cells.End.Y * tile;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
