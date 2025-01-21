#if UNITY_EDITOR
using System;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEditor;

public class VIRDYAssetPostProcessor : AssetPostprocessor
{
    public struct DefineSymbolData
    {
        public BuildTargetGroup BuildTargetGroup;
        public string FullSymbolString;
        public Regex SymbolRegex;

        public DefineSymbolData(string symbol)
        {
            BuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            FullSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup);
            SymbolRegex = new Regex(@"\b" + symbol + @"\b(;|$)");
        }
    }

    private static readonly string VIRDY_DOTWEEN =                          "DG.Tweening";

    private static readonly string VIRDY_VOLUMETRICLIGHTS =                 "VolumetricLights";
    private static readonly string VIRDY_VOLUMETRICFOGANDMIST2 =            "VolumetricFogAndMist2";
    private static readonly string VIRDY_HORIZONBASEDAMBIENTOCCLUSION =     "HorizonBasedAmbientOcclusion.Universal";
    private static readonly string VIRDY_OCCASOFTWARE_ALTOS =               "OccaSoftware.Altos";
    private static readonly string VIRDY_UWC =                              "uWindowCapture";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        bool dotween = false;

        bool volumetricLights = false;
        bool volumetricFogAndMist = false;
        bool hbao = false;
        bool altos = false;
        bool uwc = false;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Namespace != null)
                {
                    dotween = dotween || type.Namespace.Equals(VIRDY_DOTWEEN);

                    volumetricLights = volumetricLights || type.Namespace.Equals(VIRDY_VOLUMETRICLIGHTS);
                    volumetricFogAndMist = volumetricFogAndMist || type.Namespace.Equals(VIRDY_VOLUMETRICFOGANDMIST2);
                    hbao = hbao || type.Namespace.Equals(VIRDY_HORIZONBASEDAMBIENTOCCLUSION);
                    altos = altos || type.Namespace.Equals(VIRDY_OCCASOFTWARE_ALTOS);
                    uwc = uwc || type.Namespace.Equals(VIRDY_UWC);
                }
            }
        }

        if (dotween) AddDefineSymbol(nameof(VIRDY_DOTWEEN));
        else RemoveDefineSymbol(nameof(VIRDY_DOTWEEN));
        if (volumetricLights) AddDefineSymbol(nameof(VIRDY_VOLUMETRICLIGHTS));
        else RemoveDefineSymbol(nameof(VIRDY_VOLUMETRICLIGHTS));
        if (volumetricFogAndMist) AddDefineSymbol(nameof(VIRDY_VOLUMETRICFOGANDMIST2));
        else RemoveDefineSymbol(nameof(VIRDY_VOLUMETRICFOGANDMIST2));
        if (hbao) AddDefineSymbol(nameof(VIRDY_HORIZONBASEDAMBIENTOCCLUSION));
        else RemoveDefineSymbol(nameof(VIRDY_HORIZONBASEDAMBIENTOCCLUSION));
        if (altos) AddDefineSymbol(nameof(VIRDY_OCCASOFTWARE_ALTOS));
        else RemoveDefineSymbol(nameof(VIRDY_OCCASOFTWARE_ALTOS));
        if (uwc) AddDefineSymbol(nameof(VIRDY_UWC));
        else RemoveDefineSymbol(nameof(VIRDY_UWC));
    }

    public static bool IsSymbolAlreadyDefined(string symbol, out DefineSymbolData dsd)
    {
        dsd = new DefineSymbolData(symbol);
        return dsd.SymbolRegex.IsMatch(dsd.FullSymbolString);
    }

    public static void AddDefineSymbol(string symbol)
    {
        if (!IsSymbolAlreadyDefined(symbol, out var dsd))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.BuildTargetGroup, $"{dsd.FullSymbolString};{symbol}");
        }
    }

    public static void RemoveDefineSymbol(string symbol)
    {
        if (IsSymbolAlreadyDefined(symbol, out var dsd))
        {
            string strResult = dsd.SymbolRegex.Replace(dsd.FullSymbolString, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.BuildTargetGroup, strResult);
        }
    }
}
#endif
