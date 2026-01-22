using UnityEngine;

namespace LevelInjector {
    public class KeyboardShortcuts : MonoBehaviour {
        public void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                SceneLoader.LoadCustomScene("Sandbox");
            }
        }
    }
}