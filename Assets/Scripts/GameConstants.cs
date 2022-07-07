
public static class GameConstants
{
    // Physics layers

    public const int DefaultLayerId = 0;
    public const int GroundLayerId = 6;
    public const int GameplayEntityLayerId = 7;

    public const int DefaultLayerMask = 1 << DefaultLayerId;
    public const int GroundLayerMask = 1 << GroundLayerId;
    public const int GameplayEntityLayerMask = 1 << GameplayEntityLayerId;
}
