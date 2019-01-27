using UnityEngine;

public static class HexColors {
    static HexColors() {
        var colorKey = new GradientColorKey[3];
        colorKey[0].color = new Color(0.45f, 0.08f, 0.33f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = new Color(0.15f, 0.45f, 0.08f);
        colorKey[1].time = 0.5f;
        colorKey[2].color = new Color(0.08f, 0.44f, 0.45f);
        colorKey[2].time = 1.0f;

        energyGradient.SetKeys(colorKey, new GradientAlphaKey[0]);
    }

    public static readonly Color Unseen = new Color(0.12f, 0.05f, 0f, 0.75f);
    public static readonly Color Visible = new Color(0.3f, 0.28f, 0.17f, 0.75f);
    public static readonly Color Path = new Color(0.25f, 0.6f, 0.24f, 0.75f);
    public static readonly Color Blocked = new Color(0.6f, 0.24f, 0.24f, 0.75f);

    public static readonly Color Movement = new Color(0.25f, 0.24f, 0.6f, 0.75f);
    public static readonly Color Interaction = new Color(0.59f, 0.6f, 0.24f, 0.75f);
    public static readonly Color Build = new Color(0.24f, 0.57f, 0.6f, 0.75f);

    private static readonly Gradient energyGradient = new Gradient();

    public static Color EnergyColor(float value) => energyGradient.Evaluate(value / Constants.HighEnergyValue);
}
