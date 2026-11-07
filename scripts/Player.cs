using Godot;
using System.Reflection;
using static Godot.TextServer;

public partial class Player : CharacterBody2D
{
    // ---- Tuning (shows in Inspector) ----
    [Export] public float WalkSpeed = 80f;      // pixels per second
    [Export] public float RunMultiplier = 1.5f;
    [Export] public float Acceleration = 600f;
    [Export] public float Friction = 800f;

    // ---- State ----
    public Vector2 Facing { get; private set; } = Vector2.Down;
    private float _currentSpeed;

    // ---- Nodes ----
    private AnimatedSprite2D _sprite;
    private RayCast2D _rayCast;

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _rayCast = GetNode<RayCast2D>("RayCast2D");

        // Layers are set here, not in the Inspector. The Inspector checkboxes are ignored in Godot.
        CollisionLayer = Layers.Bits(Layers.Player);
        CollisionMask = Layers.Bits(Layers.World);
        _rayCast.CollisionMask = Layers.Bits(Layers.Interactable);

    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 input = Input.GetVector(Actions.MoveLeft, Actions.MoveRight, Actions.MoveUp, Actions.MoveDown);
        Intereact();
        UpdateFacing(input);
        Move(input, (float)delta);
        UpdateAnimation();
    }

    private void Intereact()
    {
        _rayCast.TargetPosition = Facing * 12;

        if (Input.IsActionPressed(Actions.Interact) && _rayCast.GetCollider() is Sign sign)
        {
            sign.Read();
        }
    }

    private void UpdateFacing(Vector2 input)
    {
        if (input != Vector2.Zero)
        {
            Facing = ToCardinal(input);
        }
    }

    private void Move(Vector2 input, float delta)
    {
        bool isRunning = Input.IsActionPressed(Actions.Run);
        _currentSpeed = isRunning ? WalkSpeed * RunMultiplier : WalkSpeed;

        Vector2 target = input * _currentSpeed;
        float rate = input == Vector2.Zero ? Friction : Acceleration;

        Velocity = Velocity.MoveToward(target, rate * delta);
        MoveAndSlide();
    }

    private void UpdateAnimation()
    {
        string state = Velocity == Vector2.Zero ? "idle" : "walk";
        _sprite.Play($"{state}_{DirectionName(Facing)}");
        _sprite.FlipH = Facing == Vector2.Left;
        _sprite.SpeedScale = _currentSpeed / WalkSpeed;
    }

    // ---- Helpers ----
    public static Vector2 ToCardinal(Vector2 direction)
    {
        bool isMostlyHorizontal = Mathf.Abs(direction.X) > Mathf.Abs(direction.Y);
        if (isMostlyHorizontal)
            return direction.X > 0 ? Vector2.Right : Vector2.Left;
        return direction.Y > 0 ? Vector2.Down : Vector2.Up;
    }

    private static string DirectionName(Vector2 facing)
    {
        if (facing == Vector2.Up) return MagicStrings.Up;
        if (facing == Vector2.Down) return MagicStrings.Down;
        return MagicStrings.Side;
    }
}
