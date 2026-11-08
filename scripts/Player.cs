using Godot;
using System.Threading.Tasks;
/// <summary>
/// Ctrl+M, Ctrl+O collapses everything down to the region names.
/// Ctrl + M, Ctrl + L toggles all of them. Press it once to expand everything, and again to collapse.
/// Ctrl+M, Ctrl+M toggles just the region your cursor is in.
/// </summary>

public partial class Player : CharacterBody2D
{
    #region Tuning (shows in Inspector)
    [Export] public float WalkSpeedInPixelsPerSecond = 80f;
    [Export] public float RunMultiplier = 1.5f;
    [Export] public float Acceleration = 600f;
    [Export] public float Friction = 800f;
    [Export] public float ToughTerrainMultiplier = .6f;
    [Export] public float AttackDuration = .25f;
    #endregion

    #region Level references (assigned in the level scene)
    [Export] public TileMapLayer Shallow;
    [Export] public TileMapLayer Ground;
    #endregion

    #region State
    public Vector2 Facing { get; private set; } = Vector2.Down;
    private float _currentSpeed;
    private enum State { Move, Attack };
    private State _state = State.Move;
    private float _attackAnimationRemainingDuration = 0f;
    #endregion

    #region Nodes
    private AnimatedSprite2D _sprite;
    private RayCast2D _rayCast;
    private Node2D _swordDirection;
    private CollisionShape2D _swordCollision;
    private Sprite2D _swordSprite;
    #endregion

    #region Setup
    public override void _Ready()
    {
        InitializeNodes();
        // Layers are set here, not in the Inspector. The Inspector checkboxes are ignored in Godot.
        CollisionLayer = Layers.Bits(Layers.Player);
        CollisionMask = Layers.Bits(Layers.World);
        _rayCast.CollisionMask = Layers.Bits(Layers.Interactable);
    }

    private void InitializeNodes()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _rayCast = GetNode<RayCast2D>("RayCast2D");
        _swordDirection = GetNode<Node2D>("SwordPivot");
        _swordCollision = GetNode<CollisionShape2D>("SwordPivot/SwordHitBox/CollisionShape2D");
        _swordSprite = GetNode<Sprite2D>("SwordPivot/SwordSprite");
    }
    #endregion

    #region Every physics frame
    public override void _PhysicsProcess(double delta)
    {
        switch (_state)
        {
            case State.Move:
                MoveState(delta);
                break;
            case State.Attack:
                AttackState(delta);
                break;
            default:
                break;
        }
        Interact();
    }
    #endregion

    #region States
    private void MoveState(double delta)
    {
        Vector2 input = Input.GetVector(Actions.MoveLeft, Actions.MoveRight, Actions.MoveUp, Actions.MoveDown);
        UpdateFacing(input);
        Move(input, (float)delta);
        UpdateAnimation();

        if (Input.IsActionJustPressed(Actions.Attack))
        {
            StartAttack();
        }
    }

    private void AttackState(double delta)
    {
        Velocity = Vector2.Zero;
        _attackAnimationRemainingDuration -= (float)delta;

        if (_attackAnimationRemainingDuration < 0)
        {
            _swordSprite.Visible = false;
            _swordCollision.Disabled = true;
            _state = State.Move;
        }
    }
    #endregion

    #region Movement
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
        _currentSpeed = isRunning ? WalkSpeedInPixelsPerSecond * RunMultiplier : WalkSpeedInPixelsPerSecond;

        Vector2 maxSpeed = IsToughTerrain() ? input * _currentSpeed * ToughTerrainMultiplier : input * _currentSpeed;
        float speedChangePerSecond = input == Vector2.Zero ? Friction : Acceleration;

        Velocity = Velocity.MoveToward(maxSpeed, speedChangePerSecond * delta);
        MoveAndSlide();
    }

    private bool IsToughTerrain()
    {
        Vector2I shallowCell = Shallow.LocalToMap(Shallow.ToLocal(GlobalPosition));
        TileData shallowData = Shallow.GetCellTileData(shallowCell);
        string shallowType = shallowData?.GetCustomData("ground_type").AsString() ?? string.Empty;

        return shallowType == "water";
    }
    #endregion

    #region Attack
    private void StartAttack()
    {
        _attackAnimationRemainingDuration = AttackDuration;
        _state = State.Attack;
        _swordDirection.Rotation = Facing.Angle();
        _swordCollision.Disabled = false;
        _swordSprite.Visible = true;
        SetSpriteDirection("attack");
    }
    #endregion

    #region Interaction
    private void Interact()
    {
        _rayCast.TargetPosition = Facing * 12;

        if (Input.IsActionPressed(Actions.Interact) && _rayCast.GetCollider() is Sign sign)
        {
            sign.Read();
        }
    }
    #endregion

    #region Animation
    private void UpdateAnimation()
    {
        string state = Velocity == Vector2.Zero ? "idle" : "walk";
        SetSpriteDirection(state);
        _sprite.SpeedScale = _currentSpeed / WalkSpeedInPixelsPerSecond;
    }

    private void SetSpriteDirection(string state)
    {
        _sprite.Play($"{state}_{DirectionName(Facing)}");
        _sprite.FlipH = Facing == Vector2.Left;
    }
    #endregion

    #region Helpers
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
    #endregion
}
