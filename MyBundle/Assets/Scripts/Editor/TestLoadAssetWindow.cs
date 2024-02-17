// TestLoadAsset.cs
// Created by Cui Lingzhi
// on 2024 - 02 - 15

using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TestLoadAssetWindow : OdinEditorWindow
    {
        [BoxGroup("Load")]
        [LabelText("ab包路径")] public string testLoadPath;
        [BoxGroup("Load")]
        [LabelText("ab资源名")] public string testLoadName;


        [MenuItem("AssetBundle/加载")]
        public static void CreateWizard()
        {
            TestLoadAssetWindow w = GetWindow<TestLoadAssetWindow>();
            w.titleContent = new GUIContent("加载测试");
        }

        [BoxGroup("Load")]
        [Button("从ab实例化prefab")]
        public void InstantiatePrefabFromBundle()
        {
            LoadAssetBundle.InstantiatePrefabFromBundle(testLoadPath, testLoadName);
            RefreshList();
        }
        [BoxGroup("Load")]
        [Button("加载bundle")]
        public void LoadBundle()
        {
            var bundle = LoadAssetBundle.LoadBundle(testLoadPath);
            RefreshList();
        }

        [BoxGroup("Load")]
        [ReadOnly][SerializeField] private IEnumerable<AssetBundle> list;

        [BoxGroup("clear")] [LabelText("清除所有实例化对象")] 
        public bool unloadObjects;
        [BoxGroup("clear")]
        [Button("清理已加载所有ab")]
        public void ClearAllBundleMemory()
        {
            LoadAssetBundle.ClearAllBundleMemory(unloadObjects);
            RefreshList();
        }
        void RefreshList()
        {
            list = LoadAssetBundle.GetAllLoadedBundles();
        }
    }
}