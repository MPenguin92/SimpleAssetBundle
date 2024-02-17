using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CreateAssetBundlesWindow : OdinEditorWindow
    {
        [LabelText("导出目录")] [DisplayAsString] [SerializeField]
        private string outPutFullPath;



        [MenuItem("AssetBundle/打包")]
        public static void CreateWizard()
        {
            CreateAssetBundlesWindow w = GetWindow<CreateAssetBundlesWindow>();
            w.titleContent = new GUIContent("打包测试");
            w.outPutFullPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6),
                Util.ASSET_BUNDLE_OUTPUT);
        }

        [Button("打包Win")]
        void BuildAllAssetBundles_Win()
        {
            CheckAndCreateFolder(Util.ASSET_BUNDLE_OUTPUT);

            AssetBundleManifest abm = BuildPipeline.BuildAssetBundles(Util.ASSET_BUNDLE_OUTPUT,
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows);


            MyBundleManifest myBundleManifest = ScriptableObject.CreateInstance<MyBundleManifest>();

            string[] allBundles = abm.GetAllAssetBundles();
            foreach (var bundleName in allBundles)
            {
                MyBundleManifest.BundleData data = new MyBundleManifest.BundleData();
                data.name = bundleName;
                data.hash = abm.GetAssetBundleHash(bundleName).ToString();
                string[] directDependencies = abm.GetAllDependencies(bundleName);
                foreach (var dependency in directDependencies)
                {
                    data.dependencies.Add(dependency);
                }

                myBundleManifest.bundles.Add(data);
            }
            
            //看着方便
            AssetDatabase.CreateAsset(myBundleManifest, Path.Combine("Assets", Util.MANIFEST_NAME));

            myBundleManifest.Serializable(Path.Combine(outPutFullPath, Util.MANIFEST_NAME));
        }


        [Button("复制到sa目录")]
        void CopyAllAssetBundles2Sa()
        {
            string sa = Application.streamingAssetsPath;
            if (!Directory.Exists(Util.ASSET_BUNDLE_OUTPUT))
            {
                return;
            }

            CheckAndCreateFolder(sa);


            string path = Path.Combine(sa, Util.ASSET_BUNDLE_UNDER_SA_PATH);

            CheckAndCreateFolder(path);

            Util.CopyFolder(
                outPutFullPath,
                path);

            AssetDatabase.Refresh();
        }

        // [Button("清空sa的bundle目录")]
        // void ClearAssetBundlesFolder()
        // {
        //     string path = Path.Combine(Application.streamingAssetsPath, ASSET_BUNDLE_UNDER_SA_PATH);
        //     DeleteFolder(path);
        //     AssetDatabase.Refresh();
        // }

        void CheckAndCreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

    }
}