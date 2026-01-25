using MelonLoader;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(LevelInjector.Main), "LevelInjector", "0.0.5", "BuffYoda21")]
[assembly: MelonGame("Vortex Bros.", "GLYPHS")]

namespace LevelInjector {
    [HarmonyPatch]
    public class Main : MelonMod {
        [System.Obsolete]
        public override void OnApplicationStart() {
            if (isInitialized) return;

            // class injection here
            ClassInjector.RegisterTypeInIl2Cpp<KeyboardShortcuts>();

            isInitialized = true;
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPrefix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            if (shortcutManager) return;
            shortcutManager = GameObject.Find("Manager intro")?.AddComponent<KeyboardShortcuts>();
        }

        private bool isInitialized = false;
        private static int lastSceneHandle = -1;
        private static KeyboardShortcuts shortcutManager;
    }
}