using System;
using System.Reflection;
using System.Collections;

public static class SSGICheck
{
    private static Type ssgiType;
    private static Type baseType;
    private static FieldInfo settingsMapField;
    private static Type levelEnumType;
    private static MethodInfo setLevelMethod;
    private static MethodInfo getLevelMethod;

    static SSGICheck()
    {
        Lumina.Mod.Log.Info("[SSGI] Initializing reflection...");

        ssgiType = Type.GetType("Game.Settings.SSGIQualitySettings, Game");
        if (ssgiType == null)
        {
            Lumina.Mod.Log.Info("[SSGI] ❌ Type not found.");
            return;
        }

        baseType = ssgiType.BaseType;

        levelEnumType = baseType.GetNestedType(
            "Level",
            BindingFlags.Public | BindingFlags.NonPublic);

        settingsMapField = baseType.GetField(
            "s_SettingsMap",
            BindingFlags.Static | BindingFlags.NonPublic);

        setLevelMethod = baseType.GetMethod(
            "SetLevel",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        getLevelMethod = baseType.GetMethod(
            "GetLevel",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Lumina.Mod.Log.Info("[SSGI] Reflection initialized.");
    }

    private static object GetAnyInstance()
    {
        var map = settingsMapField?.GetValue(null) as IDictionary;
        if (map == null || map.Count == 0)
            return null;

        foreach (DictionaryEntry entry in map)
            return entry.Value; // first instance is enough

        return null;
    }

    public static void SetSSGIEnabled(bool enabled)
    {
        var instance = GetAnyInstance();
        if (instance == null)
        {
            Lumina.Mod.Log.Info("[SSGI] ❌ No SSGI instances registered yet.");
            return;
        }

        string target = enabled ? "High" : "Disabled";
        var levelValue = Enum.Parse(levelEnumType, target);

        Lumina.Mod.Log.Info($"[SSGI] Forcing level → {target}");

        setLevelMethod.Invoke(instance, new object[] { levelValue, true });
    }

    public static bool IsSSGIEnabled()
    {
        var instance = GetAnyInstance();
        if (instance == null)
            return false;

        var level = getLevelMethod.Invoke(instance, null);

        Lumina.Mod.Log.Info($"[SSGI] Current Level = {level}");

        return !level.ToString().Equals("Disabled", StringComparison.OrdinalIgnoreCase);
    }
}
