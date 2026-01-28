using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelInjector.API {
    [HarmonyPatch]
    public static class ExternalComponentManager {
        static ExternalComponentManager() {
            registry = new Dictionary<string, Action<GameObject>>();
        }

        public static void Init() {
            isInitialized = true;
        }

        // allows usage of custom components in the level editor
        public static void Register<T>(string name) where T : MonoBehaviour {
            registry[name] = (obj) => obj.AddComponent<T>();
        }

        public static bool TryAddComponent(string name, GameObject obj) {
            if (!registry.TryGetValue(name, out var addComp)) return false;
            addComp(obj);
            return true;
        }

        public static PlayerController GetPlayer() {
            return player;
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.name != "Game" && scene.name != "Memory" && scene.name != "Outer Void") return;
            player = GameObject.Find("Player")?.GetComponent<PlayerController>();
        }

        private static readonly Dictionary<string, Action<GameObject>> registry;
        private static PlayerController player;
        public static bool isInitialized = false;
    }
}