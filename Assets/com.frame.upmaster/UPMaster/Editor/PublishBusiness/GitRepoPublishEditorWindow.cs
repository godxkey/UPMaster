using System;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace JackFrame.UPMaster.Publish {

    public class GitRepoPublishEditorWindow : EditorWindow {

        // GUI
        UnityPackagePackerGUI packerGUI;
        PackageModifierGUI modifierGUI;
        PackagePublishGUI publisherGUI;

        // Cache
        GitRepoConfigModel[] allModel;
        PublishInfoWriterModel[] allWriter;
        string[] packageNames;

        int toolbarIndex = 0;

        [MenuItem(nameof(JackFrame) + "/UPMaster/PublishManager")]
        public static void Open() {
            GitRepoPublishEditorWindow window = EditorWindow.GetWindow<GitRepoPublishEditorWindow>();
            window.titleContent.text = "UPMaster 发布工具";
            window.Show();
        }

        void OnEnable() {

            Initialize();

        }

        void Initialize() {

            // GUI
            if (packerGUI == null) {
                packerGUI = new UnityPackagePackerGUI();
            }

            if (modifierGUI == null) {
                modifierGUI = new PackageModifierGUI();
            }

            if (publisherGUI == null) {
                publisherGUI = new PackagePublishGUI();
            }

            // Cache
            var list = new List<GitRepoConfigModel>();
            string[] assets = AssetDatabase.GetAllAssetPaths();
            foreach (var asset in assets) {
                if (!asset.EndsWith(".asset") || !asset.Contains("Assets")) {
                    continue;
                }
                var model = AssetDatabase.LoadAssetAtPath<GitRepoConfigModel>(asset);
                if (model != null) {
                    list.Add(model);
                }
            }
            allModel = list.ToArray();

            allWriter = new PublishInfoWriterModel[allModel.Length];
            for (int i = 0; i < allWriter.Length; i += 1) {
                allWriter[i] = new PublishInfoWriterModel();
            }

            packageNames = allModel.Select(value => value.dependancyModel.packageName).ToArray();

        }

        Vector2 leftScrollView;
        int selectedPackage;
        Vector2 rightScrollView;
        void OnGUI() {

            if (allModel.Length == 0) {
                GUILayout.Label("请创建配置");
                return;
            }

            // ==== Left ====
            float leftSize = 250f;
            GUILayout.BeginArea(new Rect(0, 0, leftSize, 500));
            GUILayout.Label("包列表");
            leftScrollView = GUILayout.BeginScrollView(leftScrollView);
            selectedPackage = GUILayout.SelectionGrid(selectedPackage, packageNames, 1);
            GitRepoConfigModel selectedSo = allModel[selectedPackage];
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            // ==== Right ====
            GUILayout.BeginArea(new Rect(leftSize, 0, 400, 600));
            GUILayout.Label("当前包管理");
            rightScrollView = GUILayout.BeginScrollView(rightScrollView);
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, new string[] { "修改package.json", "发布至远端" }, GUILayout.Width(320));

            if (toolbarIndex == 0) {
                modifierGUI.GUIModifyPublishInfo(selectedSo.dependancyModel, allWriter[selectedPackage]);
            } else if (toolbarIndex == 1) {
                sbyte result = publisherGUI.GUIPublishToWeb(selectedSo);
                if (result == 1) {
                    EditorUtility.SetDirty(selectedSo);
                    Initialize();
                }
                // packerGUI.GUIPackUnityPackage(selectedSo, VersionHelper.ReadCurrentVersion(selectedSo.dependancyModel));
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

        }

    }

}