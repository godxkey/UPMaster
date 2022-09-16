using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace JackFrame.UPMaster.Publish {

    public class UnityPackagePackerGUI {

        // -- UNITY PACKAGE
        public void GUIPackUnityPackage(GitRepoConfigModel model, string currentVersion) {

            string packageName = model.dependancyModel.packageName;
            GUILayout.Label($"当前版本号: {currentVersion}");
            GUILayout.Label($"包名: {packageName}");

            string srcDir = Path.Combine(Application.dataPath, packageName);
            GUILayout.Space(10);
            GUILayout.Label($"源目录: {srcDir}");

            string dstDir = Path.Combine(Environment.CurrentDirectory, "Assets", "UnityPackage");
            string outputFile = Path.Combine(dstDir, packageName + currentVersion + ".unitypackage");
            GUILayout.Space(10);
            GUILayout.Label($"导出地址: {outputFile}");

            GUILayout.Space(10);
            if (GUILayout.Button("打包")) {
                FileHelper.CreateDirIfNorExist(dstDir);
                string inputDir = Path.Combine("Assets", srcDir);
                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(inputDir);
                var list = new List<string>();
                for (int i = 0; i < objs.Length; i += 1) {
                    var obj = objs[i];
                    var path = AssetDatabase.GetAssetPath(obj);
                    list.Add(path);
                }
                AssetDatabase.ExportPackage(list.ToArray(), outputFile, ExportPackageOptions.Recurse);
            }
        }

    }

}