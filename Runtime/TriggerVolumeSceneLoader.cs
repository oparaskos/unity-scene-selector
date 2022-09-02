using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSelector.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class TriggerVolumeSceneLoader : MonoBehaviour
    {
        public string[] tags = { "Player" };
        public SceneSelection scenesToLoad;

        public void OnTriggerEnter(Collider other) {
            foreach (string t in tags)
            {
                if(other.tag == t) {
                    LoadScenesAsync();
                    return;
                }
            }
        }

        public void LoadScenesAsync() {
            foreach (int buildIndex in scenesToLoad.BuildIndices) {
                Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
                if (!scene.IsValid() || !scene.isLoaded) {
                    Debug.Log("Load Scene " + scene);
                    SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
                }
            }
        }
    }
}