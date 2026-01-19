using MelonLoader;
using Il2CppInterop.Runtime.Injection;

[assembly: MelonInfo(typeof(customAssetTestMod.Main), "LevelInjector", "0.0.1", "BuffYoda21")]
[assembly: MelonGame("Vortex Bros.", "GLYPHS")]

namespace customAssetTestMod {
    public class Main : MelonMod {
        [System.Obsolete]
        public override void OnApplicationStart() {
            if (isInitialized) return;
            var harmony = new HarmonyLib.Harmony("LevelInjector.Patches");
            harmony.PatchAll();

            // class injection here

            isInitialized = true;
        }
        bool isInitialized = false;
    }
}