using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
#endif

namespace SceneSelector.Runtime
{

    #if UNITY_EDITOR
    [Serializable]
    public class SceneSelection : IPreprocessBuildWithReport
    #else
    [Serializable]
    public class SceneSelection
    #endif
    {
        [SerializeField]
        private GUID[] sceneGUIDs = { };

        [SerializeField]
        private int[] buildIndices = { };

        public GUID[] SceneGUIDs {
            get { return sceneGUIDs ?? new GUID[] { }; }
    #if UNITY_EDITOR
            set { sceneGUIDs = value; RebuildSceneIndices(); }
    #endif
        }

        public int[] BuildIndices {
            get { return buildIndices ?? new int[] { }; }
    #if UNITY_EDITOR
            private set { buildIndices = value; }
    #endif
        }

        public IEnumerable<Scene> Scenes {
            get {
                #if UNITY_EDITOR
                    RebuildSceneIndices();
                #endif
                return BuildIndices.Select(buildIndex => SceneManager.GetSceneByBuildIndex(buildIndex));
            }
        }

    #if UNITY_EDITOR
        public int callbackOrder { get; } = 0;

        public void Clear()
        {
            sceneGUIDs = new GUID[] { };
            buildIndices = new int[] { };
        }

        public void AddGUID(GUID guid)
        {
            if (!sceneGUIDs.Contains(guid))
            {
                sceneGUIDs = sceneGUIDs.Append(guid).ToArray();
                RebuildSceneIndices();
            }
        }

        public void RemoveGUID(GUID guid)
        {
            sceneGUIDs = sceneGUIDs.Where(it => it != guid).ToArray();
            RebuildSceneIndices();
        }

        // Generate list of build incides from scenesToLoad.
        public void RebuildSceneIndices() {
            BuildIndices = sceneGUIDs?.Select(guid => {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                int buildIndex = SceneUtility.GetBuildIndexByScenePath(path);
                return buildIndex;
            })?.ToArray();
        }

        // Re-sync build indices before a build.
        public void OnPreprocessBuild(BuildReport report) { RebuildSceneIndices(); }
    #endif
    }
}