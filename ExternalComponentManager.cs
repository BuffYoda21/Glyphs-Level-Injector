using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelInjector.API {
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

        private static readonly Dictionary<string, Action<GameObject>> registry;
        public static bool isInitialized = false;
    }
}