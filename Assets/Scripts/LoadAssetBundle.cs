// LoadAssetBundle.cs
// Created by Cui Lingzhi
// on 2024 - 02 - 13

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LoadAssetBundle
{
    private static readonly string DefaultBundleRootPath =
        Path.Combine(Application.streamingAssetsPath, Util.ASSET_BUNDLE_UNDER_SA_PATH);

    private static MyBundleManifest bundleManifest
    {
        get
        {
            if (sBundleManifest == null)
            {
                sBundleManifest =
                    MyBundleManifest.Deserializable(Path.Combine(DefaultBundleRootPath, Util.MANIFEST_NAME));
            }

            return sBundleManifest;
        }
    }

    public class BundleReference
    {
        public AssetBundle bundle;
        public int referenceCount;
    }

    private static MyBundleManifest sBundleManifest;

    private static Dictionary<string, BundleReference> sPath2BundleMapping = new Dictionary<string, BundleReference>();

    public static GameObject InstantiatePrefabFromBundle(string bundlePath, string name)
    {
        var myLoadedAssetBundle
            = LoadBundleWithDependency(bundlePath);
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return null;
        }

        var prefab = myLoadedAssetBundle.LoadAsset<GameObject>(name);
        return Object.Instantiate(prefab);
    }

    /// <summary>
    /// 回收bundle引用，以及所有依赖的bundle
    /// 当引用计数不大于0,卸载bundle
    /// </summary>
    /// <param name="bundlePath"></param>
    public static void RecycleBundle(string bundlePath)
    {
        if (sPath2BundleMapping.TryGetValue(bundlePath, out BundleReference bundleReference))
        {
            bundleReference.referenceCount--;
            if (bundleReference.referenceCount <= 0)
            {
                bundleReference.bundle.Unload(true);
                sPath2BundleMapping.Remove(bundlePath);
            }
            
            var bundleData = bundleManifest.bundles.Find(o => o.name == bundlePath);
            int deCount = bundleData.dependencies.Count;
            for (int i = 0; i < deCount; i++)
            {
                RecycleBundle(bundleData.dependencies[i]);
            }
        }
    }

    /// <summary>
    /// 加载bundle，以及所有依赖的bundle
    /// </summary>
    /// <param name="bundlePath"></param>
    /// <returns></returns>
    private static AssetBundle LoadBundleWithDependency(string bundlePath)
    {
        var bundleData = bundleManifest.bundles.Find(o => o.name == bundlePath);
        if (bundleData != null)
        {
            var myLoadedAssetBundle
                = LoadBundle(bundleData.name);
            int deCount = bundleData.dependencies.Count;
            for (int i = 0; i < deCount; i++)
            {
                LoadBundle(bundleData.dependencies[i]);
            }

            return myLoadedAssetBundle;
        }

        return null;
    }

    public static AssetBundle LoadBundle(string bundlePath)
    {
        bundlePath = Path.Combine(DefaultBundleRootPath, bundlePath);
        if (sPath2BundleMapping.TryGetValue(bundlePath, out BundleReference bundleReference))
        {
            bundleReference.referenceCount++;
            return bundleReference.bundle;
        }

        var myLoadedAssetBundle
            = AssetBundle.LoadFromFile(bundlePath);
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return null;
        }

        sPath2BundleMapping.Add(bundlePath, new BundleReference()
        {
            bundle = myLoadedAssetBundle,
            referenceCount = 1,
        });
        return myLoadedAssetBundle;
    }

    public static void ClearAllBundleMemory(bool b)
    {
        AssetBundle.UnloadAllAssetBundles(b);
        sPath2BundleMapping.Clear();
    }

    public static IEnumerable<AssetBundle> GetAllLoadedBundles()
    {
        return AssetBundle.GetAllLoadedAssetBundles();
    }
}