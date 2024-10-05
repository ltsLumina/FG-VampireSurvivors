using UnityEngine;

// ReSharper disable once UnusedType.Global
public class Percent
{
    float value;

    // Constructor to initialize the Percent
    public Percent(float value)
    {
        this.value = Mathf.Clamp01(value); // Ensure percent value is between 0 and 1
    }

    // Property to get or set the percentage as a decimal (e.g., 0.1 for 10%)
    public float Value
    {
        get => value;
        set => this.value = Mathf.Clamp01(value); // Ensure percent value is between 0 and 1
    }

    // Method to add a percentage to a value
    public float AddTo(float baseValue) => baseValue * (1 + value);

    // Method to remove a percentage from a value
    public float RemoveFrom(float baseValue) => baseValue * (1 - value);

    // Static utility method to add a percentage to a value
    public static float Add(float baseValue, float percent) => baseValue * (1 + Mathf.Clamp01(percent));

    // Static utility method to remove a percentage from a value
    public static float Remove(float baseValue, float percent) => baseValue * (1 - Mathf.Clamp01(percent));
}

// Extension methods for float
public static class PercentExtensions
{
    // Extension method to add a percentage to a float value
    public static float AddPercent(this float baseValue, float percent) { return baseValue * (1 + Mathf.Clamp01(percent)); }

    // Extension method to remove a percentage from a float value
    public static float RemovePercent(this float baseValue, float percent) { return baseValue * (1 - Mathf.Clamp01(percent)); }
}
