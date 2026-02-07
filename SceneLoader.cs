using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelInjector {
    [HarmonyPatch]
    public static class SceneLoader {
        public static void LoadCustomScene(string scene) {
            overrideSpawnPosition = false;
            LoadCustomScene(scene, Vector2.zero);
        }

        public static void LoadCustomScene(string scene, Vector2 pos) {
            spawnPosition = pos;
            CachePlayerAbilities();
            if (scene != "Game" && scene != "Memory" && scene != "Outer Void") {
                customSceneToLoad = scene;
                SceneManager.LoadScene("Memory");
            } else {
                SceneManager.LoadScene(scene);
            }
        }

        [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
        [HarmonyPostfix]
        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.handle == lastSceneHandle) return;
            lastSceneHandle = scene.handle;
            string sceneToLoad = scene.name;
            if (sceneToLoad == "Memory" && !string.IsNullOrEmpty(customSceneToLoad)) {
                sceneToLoad = customSceneToLoad;
                customSceneName = customSceneToLoad;
                customSceneToLoad = null;
                GameObject.Find("Canvas/memoryvisual")?.SetActive(false);
                MelonCoroutines.Start(FullClearScene());
                player = GameObject.Find("Player")?.GetComponent<PlayerController>();
                RestorePlayerAbilities();
                if (overrideSpawnPosition && player)
                    player.transform.position = spawnPosition;
            } else {
                customSceneName = null;
            }
            RoomLoader.LoadRooms(sceneToLoad);
            foreach (System.Action roomLoadHook in postRoomLoadHooks) {
                try {
                    roomLoadHook();
                } catch (System.Exception ex) {
                    MelonLogger.Error($"RoomLoadHook threw: {ex}");
                }
            }
            overrideSpawnPosition = true;
        }

        private static IEnumerator FullClearScene() {
            ClearMemoryScene();
            yield return null;
            ClearMemoryScene();
        }

        private static void ClearMemoryScene() {
            Object.Destroy(GameObject.Find("World"));
            Object.Destroy(GameObject.Find("Square (19)"));
            Object.Destroy(GameObject.Find("Square (20)"));
            Object.Destroy(GameObject.Find("Square (21)"));
            Object.Destroy(GameObject.Find("Square (22)"));
            Object.Destroy(GameObject.Find("BackGround"));
            Object.Destroy(GameObject.Find("BackGround (1)"));
            Object.Destroy(GameObject.Find("MapManager"));
        }

        private static void RestorePlayerAbilities() {
            if (!player) return;
            player.midairJumpsMax = playerState.jumps;
            player.saveCrystals = playerState.saveCrystals;
            player.fragments = playerState.fragments;
            player.hp = playerState.hp;
            player.maxHp = playerState.maxHp;
            player.attackBonus = playerState.attackBonus;
            player.goldfragments = playerState.goldFragments;
            player.parryCD = playerState.parryCD;
            player.dashAttackChargeMax = playerState.dashAttackCD;
            player.hasGrapple = playerState.grapple;
            player.hasParry = playerState.parry;
            player.dashAttack = playerState.dashAttack;
            player.hasWeapon = playerState.hasWeapon;
            player.hasShroud = playerState.shroud;
        }

        private static void CachePlayerAbilities() {
            PlayerController player = GameObject.Find("Player")?.GetComponent<PlayerController>();
            if (!player) return;
            MelonLogger.Msg("Caching player abilities");
            playerState.jumps = player.midairJumpsMax;
            playerState.saveCrystals = player.saveCrystals;
            playerState.fragments = player.fragments;
            playerState.hp = player.hp;
            playerState.maxHp = player.maxHp;
            playerState.attackBonus = player.attackBonus;
            playerState.goldFragments = player.goldfragments;
            playerState.parryCD = player.parryCD;
            playerState.dashAttackCD = player.dashAttackChargeMax;
            playerState.grapple = player.hasGrapple;
            playerState.parry = player.hasParry;
            playerState.dashAttack = player.dashAttack;
            playerState.hasWeapon = player.hasWeapon;
            playerState.shroud = player.hasShroud;
        }

        public static void RegisterRoomLoadHook(System.Action hook) {
            if (hook != null && !postRoomLoadHooks.Contains(hook)) postRoomLoadHooks.Add(hook);
        }

        private class PlayerState {
            public int jumps = 0;
            public int saveCrystals = 0;
            public int fragments = 0;
            public int goldFragments = 0;
            public float hp = 100;
            public float maxHp = 100;
            public float attackBonus = 0;
            public float parryCD = 2f;
            public float dashAttackCD = 1f;
            public bool grapple = false;
            public bool parry = false;
            public bool dashAttack = false;
            public bool hasWeapon = false;
            public bool shroud = false;
        }

        private static int lastSceneHandle = -1;
        private static string customSceneToLoad = null;
        public static string customSceneName;
        private static PlayerController player;
        private static PlayerState playerState = new PlayerState();
        private static bool overrideSpawnPosition = true;
        private static Vector2 spawnPosition;
        private static List<System.Action> postRoomLoadHooks = new List<System.Action>();
    }
}