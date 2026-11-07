public static class Layers
{
    public const int World = 1;
    public const int Player = 2;
    public const int Enemy = 3;
    public const int PlayerAttack = 4;
    public const int EnemyAttack = 5;
    public const int Interactable = 6;

    // Turns layer numbers into the bitmask Godot wants: Bits(1, 3) = layers 1 and 3
    //Without the collision debugging becomes a nightmare, so this is a convenience function to make it easier to set up collision layers and masks in code.
    public static uint Bits(params int[] layers)
    {
        uint mask = 0;
        foreach (int layer in layers)
        {
            mask |= 1u << (layer - 1);

        }

        return mask;
    }
}