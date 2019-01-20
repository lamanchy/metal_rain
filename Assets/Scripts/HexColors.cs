using UnityEngine;

public static class HexColors {
    public static Color unseen => new Color(0.12f, 0.05f, 0f, 0.75f);
    public static Color visible => new Color(0.3f, 0.28f, 0.17f, 0.75f);
    public static Color path => new Color(0.25f, 0.6f, 0.24f, 0.75f);
    public static Color blocked => new Color(0.6f, 0.24f, 0.24f, 0.75f);

    public static Color movement => new Color(0.25f, 0.24f, 0.6f, 0.75f);
    public static Color interaction => new Color(0.59f, 0.6f, 0.24f, 0.75f);
}
