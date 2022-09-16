using System;
using System.Text;
using System.Net.Http;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace JackFrame.UPMaster.Publish {

    public class PackagePublishGUI {

        const string OK = "确认";
        const string CANCEL = "取消";

        string uri = "http://upmaster.utea.fun/add_package"; // default

        public sbyte GUIPublishToWeb(GitRepoConfigModel configModel) {

            sbyte result = 0;

            var model = configModel.dependancyModel;

            GUILayoutOption labelWidth = GUILayout.Width(80);
            GUILayoutOption textFieldWidth = GUILayout.Width(360);
            GUILayoutOption buttonWidth = GUILayout.Width(160);

            GUILayout.Label("---- Package 信息 ----");

            GUILayout.BeginHorizontal();
            GUILayout.Label("包名: ", labelWidth);
            model.packageName = GUILayout.TextField(model.packageName, textFieldWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Git 地址: ", labelWidth);
            model.gitUrl = GUILayout.TextField(model.gitUrl, textFieldWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("标签/分支: ", labelWidth);
            model.branchOrTag = GUILayout.TextField(model.branchOrTag, textFieldWidth);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("保存修改", buttonWidth)) {
                result = 1;
            }

            GUILayout.Space(20);
            GUILayout.Label("---- 发布 ----");

            GUILayout.BeginHorizontal();
            GUILayout.Label("订阅源地址: ", labelWidth);
            uri = GUILayout.TextField(uri, textFieldWidth);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("发布至订阅源", buttonWidth)) {
                PublishToWeb(uri, model);
            }

            return result;

        }

        void PublishToWeb(string uri, UPMasterDependancyModel model) {
            string title = "发布确认";
            string content = $"依赖信息将会发布至订阅源, 如果仓库非开源请勿发布";
            if (EditorUtility.DisplayDialog(title, content, OK, CANCEL)) {
                var msg = new PublishPackageReqMessage() {
                    packageName = model.packageName,
                    gitUrl = model.gitUrl,
                    branchOrTag = model.branchOrTag
                };
                string jsonStr = JsonConvert.SerializeObject(msg);
                HttpClient client = new HttpClient();
                HttpContent req = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonStr));
                var res = client.PostAsync(uri, req).Result;
                EditorUtility.DisplayDialog("发布结果", $"{res.StatusCode.ToString()}", OK);

            }
        }

    }

}