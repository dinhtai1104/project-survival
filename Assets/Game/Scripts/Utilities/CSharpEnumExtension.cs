using System;

public static class CSharpEnumExtension
{
    public static T ParseEnum<T>(this string hero)
    {
        return (T)Enum.Parse(typeof(T), hero);
    }
}