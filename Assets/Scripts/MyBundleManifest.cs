// MyBundleManifest.cs
// Created by Cui Lingzhi
// on 2024 - 02 - 16

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu]
public class MyBundleManifest : ScriptableObject
{
    public List<BundleData> bundles = new List<BundleData>();
    public int versionCode;

    [Serializable]
    public class BundleData
    {
        public string hash;
        public string name;
        public List<string> dependencies = new List<string>();
    }

    public void Serializable(string path)
    {
        BinaryWriter w = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
        w.Write(versionCode);
        w.Write(bundles.Count);
        foreach (var bundleData in bundles)
        {
            w.Write(bundleData.hash);
            w.Write(bundleData.name);
            w.Write(bundleData.dependencies.Count);
            foreach (var dependency in bundleData.dependencies)
            {
                w.Write(dependency);
            }
        }

        w.Close();
    }

    public static MyBundleManifest Deserializable(string path)
    {
        using (Stream stream = File.OpenRead(path))
        {
            BinaryReader reader = new BinaryReader(stream);
            MyBundleManifest m = ScriptableObject.CreateInstance<MyBundleManifest>();
            m.versionCode = reader.ReadInt32();
            int bundleDataCount = reader.ReadInt32();
            m.bundles = new List<BundleData>(bundleDataCount);


            while(bundleDataCount > 0)
            {
                BundleData data = new BundleData();
                data.hash = reader.ReadString();
                data.name = reader.ReadString();
                int dependenciesCount = reader.ReadInt32();
                data.dependencies = new List<string>(dependenciesCount);
                while (dependenciesCount > 0)
                {
                    data.dependencies.Add(reader.ReadString());
                    dependenciesCount--;
                }
                m.bundles.Add(data);
                bundleDataCount--;
            }

            return m;
        }
    }
}