using Engine;
using ExtensionKit;
using UnityEngine;

public static class ConstantValue
{
    public const int MonsterTeamId = 0;
    public const int PlayerTeamId = 1;

    public const string TagBoss = "Boss";
    public const float ClusterRange = 1.5f;

    public const int Weapon_MaxSlot = 6;

    public static Stat DefaultSpeed = new Stat(10);
    public static Color ColorGreen = "".HtmlStringToColor();
    public static Color ColorRed = "".HtmlStringToColor();
}