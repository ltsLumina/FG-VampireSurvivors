using System.Linq;

public static class StringExtensions
{
    // method that converts camelCase to PascalCase
    public static string ToPascalCase(this string str)
    {
        // if the string is null or empty, or already in PascalCase, return the string
        if (string.IsNullOrEmpty(str) || char.IsUpper(str[0])) return str;

        // return the string with the first letter capitalized
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    // method that converts PascalCase to camelCase
    public static string ToCamelCase(this string str)
    {
        // if the string is null or empty, or already in camelCase, return the string
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0])) return str;

        // return the string with the first letter lowercased
        return char.ToLower(str[0]) + str.Substring(1);
    }

    // method that converts PascalCase to Normal Case
    public static string ToNormalCase(this string str)
    {
        // if the string is null or empty, or already in Normal Case, return the string
        if (string.IsNullOrEmpty(str) || str.Any(char.IsWhiteSpace)) return str;

        // return the string with spaces between each word
        return string.Concat(str.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
    }

    // method that converts Normal Case to PascalCase
    public static string ToPascalCaseFromNormalCase(this string str)
    {
        // if the string is null or empty, or already in PascalCase, return the string
        if (string.IsNullOrEmpty(str) || !str.Any(char.IsWhiteSpace)) return str;

        // return the string with the first letter of each word capitalized
        return string.Concat(str.Split(' ').Select(x => char.ToUpper(x[0]) + x.Substring(1)));
    }
}
