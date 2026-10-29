using System;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using EFT.UI;
using SPTOpenSesame;
using SPTOpenSesame.Helpers;

[BepInPlugin("com.jpeckham.opensesame.tests", "OpenSesame Runtime Tests", "1.0.0")]
[BepInDependency("com.danw.opensesame")]
public class RuntimeTests : BasePlugin
{
    public override void Load()
    {
        var saved = OpenSesamePlugin.FeaturesEnabled.Value;
        try
        {
            foreach (var locale in new[] { "en", "ch", "ru", "po", "kr", "jp", "ge", "fr", "es", "es-mx", "unknown-test" })
            {
                var translations = new EFT.Locale(4);
                if (!LocalizationHelpers.TryAddNewTranslationsForLocale(locale, translations) || !translations.ContainsKey("OpenSesame"))
                    throw new Exception("Translation test failed: " + locale);
            }
            AvailableInteractionState state = null;
            OpenSesamePlugin.FeaturesEnabled.Value = OpenSesamePlugin.EFeaturesEnabled.UnlockDoors;
            InteractionHelpers.AddDoNothingToActionList(ref state);
            if (state?.Actions == null || state.Actions.Count != 0) throw new Exception("Disabled DoNothing/null state failed");
            OpenSesamePlugin.FeaturesEnabled.Value = OpenSesamePlugin.EFeaturesEnabled.All;
            state.Actions = null;
            InteractionHelpers.AddDoNothingToActionList(ref state);
            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
            if (state.Actions.Count != 1 || state.Actions[0].Name != "DoNothing") throw new Exception("Native action list failed");
            state.Actions[0].Action.Invoke();
            Log.LogInfo("PASS: 10 locales, fallback, null state/list, feature toggle, native callback after managed GC");
        }
        catch (Exception ex) { Log.LogError("FAIL: " + ex); }
        finally { OpenSesamePlugin.FeaturesEnabled.Value = saved; }
    }
}
