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
    /// <summary>
    ///    Extension method to add a percentage to a float value and modify the original value.
    /// </summary>
    /// <param name="baseValue"> The original value to modify. </param>
    /// <param name="percent"> The percentage to add. </param>
    /// <param name="round"> Whether to round the result. </param>
    /// <param name="decimals"> The number of decimal places to round to. </param>
    /// <returns> The modified value. </returns>
    /// <remarks> This method modifies the original value, rather than returning a new value. </remarks>
    public static float AddPercent(ref this float baseValue, float percent, bool round = true, int decimals = 3)
    {
        baseValue *= 1 + Mathf.Clamp01(percent);

        if (round)
        {
            float     multiplier = Mathf.Pow(10, decimals);
            baseValue = Mathf.Round(baseValue * multiplier) / multiplier;
        }

        return baseValue;
    }
    
    /// <summary>
    /// Overload for int values.
    /// </summary>
    /// <param name="baseValue"> The original value to modify. </param>
    /// <param name="percent"> The percentage to add. </param>
    /// <param name="round"> Whether to round the result. </param>
    /// <param name="decimals"> The number of decimal places to round to. </param>
    /// <returns> The modified value. </returns>
    /// <remarks> This method modifies the original value, rather than returning a new value. </remarks>
    public static int AddPercent(ref this int baseValue, float percent, bool round = true, int decimals = 3)
    {
        baseValue = (int) (baseValue * (1 + Mathf.Clamp01(percent)));

        if (round)
        {
            float     multiplier = Mathf.Pow(10, decimals);
            baseValue = (int) (Mathf.Round(baseValue * multiplier) / multiplier);
        }

        return baseValue;
    }

    // Extension method to remove a percentage from a float value and modify the original value
    public static void RemovePercent(ref this float baseValue, float percent) => baseValue *= 1 - Mathf.Clamp01(percent);
}
