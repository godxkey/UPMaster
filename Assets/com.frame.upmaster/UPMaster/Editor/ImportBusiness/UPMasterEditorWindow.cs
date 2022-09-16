using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace JackFrame.UPMaster.Subscription {

    public class UPMasterEditorWindow : EditorWindow {

        SubscriptionGUI subscriptionGUI;
        ShowDependencyGUI showDependencyGUI;

        UPMManifestModifier modifier;

        [MenuItem(nameof(JackFrame) + "/UPMaster/DependenciesManager")]
        public static void OpenWindow() {
            var window = EditorWindow.GetWindow<UPMasterEditorWindow>();
            window.titleContent.text = "UPMaster 依赖管理";
            window.Show();
        }

        void OnEnable() {
            Initialize();
        }

        // ==== GUI ====
        void OnGUI() {

            GUILayout.BeginArea(new Rect(0, 0, 200, 500));
            var chosen = subscriptionGUI.GUIListAndChooseDependancy();
            GUILayout.EndArea();
            if (chosen == null) {
                return;
            }

            // 选中的包, 本地是否有
            // 如有, 显示本地信息: 包名、版本、路径，以及比对信息与更新按钮
            // 如无, 显示订阅按钮
            GUILayout.BeginArea(new Rect(210, 0, 500, 500));
            GUILayout.Label("依赖信息");
            var old = modifier.Find(chosen.packageName);
            if (old != null) {
                GUILayout.Box("包名: " + chosen.packageName);
                GUILayout.Box("Git: " + chosen.gitUrl);
                GUILayout.Box("分支: " + chosen.branchOrTag);
                if (GUILayout.Button("移除订阅", GUILayout.Width(100))) {
                    modifier.Remove(chosen.packageName);
                    ModifyManifest();
                }
            } else {
                if (GUILayout.Button("订阅", GUILayout.Width(50))) {
                    modifier.Add(chosen.packageName, chosen.GetFullURL());
                    ModifyManifest();
                }
            }
            GUILayout.EndArea();

        }

        void GUI_DrawInputLine(UPMasterDependancyModel model, int index) {

            GUILayout.Box("Index: " + index.ToString());

            const float LABEL_WIDTH = 80;

            GUILayout.BeginHorizontal();
            GUILayout.Label("包名:", GUILayout.Width(LABEL_WIDTH));
            model.packageName = GUILayout.TextField(model.packageName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Git URL:", GUILayout.Width(LABEL_WIDTH));
            model.gitUrl = GUILayout.TextField(model.gitUrl);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Git Branch:", GUILayout.Width(LABEL_WIDTH));
            model.branchOrTag = GUILayout.TextField(model.branchOrTag);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

        }

        // ==== LOGIC ====
        void Initialize() {

            if (subscriptionGUI == null) {
                subscriptionGUI = new SubscriptionGUI();
            }

            if (showDependencyGUI == null) {
                showDependencyGUI = new ShowDependencyGUI();
            }

            modifier = new UPMManifestModifier();
            modifier.Initialize();

        }

        void ModifyManifest() {
            var json = modifier.Generate();
            Debug.Log("GEN:" + json);
            string path = Path.Combine(Environment.CurrentDirectory, "Packages", "manifest.json");
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

    }

}