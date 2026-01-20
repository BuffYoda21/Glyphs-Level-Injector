using MelonLoader;
using Il2CppInterop.Runtime.Injection;

[assembly: MelonInfo(typeof(LevelInjector.Main), "LevelInjector", "0.0.1", "BuffYoda21")]
[assembly: MelonGame("Vortex Bros.", "GLYPHS")]

namespace LevelInjector {
    public class Main : MelonMod {
        [System.Obsolete]
        public override void OnApplicationStart() {
            if (isInitialized) return;
            // class injection here

            isInitialized = true;
        }

        private bool isInitialized = false;
    }
}