using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using SPTOpenSesame.Components;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using SPTOpenSesame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SPTOpenSesame
{
    [BepInPlugin("com.danw.opensesame", "DanW-OpenSesame", "4.0.0")]
    [BepInDependency("com.sptushonka.core", "5.0.0")]
    public class OpenSesamePlugin : BasePlugin
    {
        [Flags]
        public enum EFeaturesEnabled
        {
            UnlockDoors = 1,
            TurnOnPower = 2,
            DoNothing = 4,

            All = UnlockDoors | TurnOnPower | DoNothing,
        }

        [Flags]
        public enum EDebugMessagesEnabled
        {
            DoorInteractions = 1,
            UnlockingDoors = 2,
            TogglingSwitches = 4,

            All = DoorInteractions | UnlockingDoors | TogglingSwitches,
        }

        public static ConfigEntry<EFeaturesEnabled> FeaturesEnabled;
        public static ConfigEntry<EDebugMessagesEnabled> DebugMessagesEnabled;

        public override void Load()
        {
            Log.LogInfo("Loading OpenSesame...");
            LoggingUtil.Instance = new LoggingUtil(Log);
            addConfigOptions();
            ClassInjector.RegisterTypeInIl2Cpp<PowerSwitchIdentificationComponent>();

            new Patches.InteractiveObjectInteractionPatch().Enable();
            new Patches.KeycardDoorInteractionPatch().Enable();
            new Patches.NoPowerTipInteractionPatch().Enable();
            new Patches.SwitchInteractionPatch().Enable();

            // Add a listener to automatically add translations when EFT first loads and when the user switches languages
            LoggingUtil.Instance.LogInfo("Adding locale update listener...");
            LocalizationManager.Instance.AddLocaleUpdateListener(
                Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<Il2CppSystem.Action>(
                    (Action)Helpers.LocalizationHelpers.AddNewTranslationsForLoadedLocales));

            LoggingUtil.Instance.LogInfo("Loading OpenSesame...done.");
        }

        private void addConfigOptions()
        {
            FeaturesEnabled = Config.Bind("Main", "Enabled Features",
                EFeaturesEnabled.All, "Enabled features of this mod");

            DebugMessagesEnabled = Config.Bind("Main", "Enabled Debug Messages",
                (EDebugMessagesEnabled)0, "Enabled debugging messages");
        }
    }
}
