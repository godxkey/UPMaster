using System.Text;
using System.Net.Http;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace JackFrame.UPMaster.Publish {

    public class PackageModifierGUI {

        const string OK = "确认";
        const string CANCEL = "取消";

        public PackageModifierGUI() { }

        // ==== 保存发布信息到文件 ====
        public void GUIModifyPublishInfo(UPMasterDependancyModel model, PublishInfoWriterModel publishInfo) {

            publishInfo.currentVersion = VersionHelper.ReadCurrentVersion(model);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label($"当前版本号: {publishInfo.currentVersion}", GUILayout.Width(160));
            if (GUILayout.Button("重新获取", GUILayout.Width(100))) {
                publishInfo.currentVersion = VersionHelper.ReadCurrentVersion(model);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("待发布版本号(例1.1.0, 前后不可加符号)");
            publishInfo.semanticVersion = GUILayout.TextField(publishInfo.semanticVersion);

            GUILayout.Space(10);
            GUILayout.Label("ChangeLog Added");
            publishInfo.changeLogAdded = GUILayout.TextArea(publishInfo.changeLogAdded, GUILayout.MinHeight(50));

            GUILayout.Space(10);
            GUILayout.Label("ChangeLog Changed");
            publishInfo.changeLogChanged = GUILayout.TextArea(publishInfo.changeLogChanged, GUILayout.MinHeight(50));

            GUILayout.Space(10);
            GUILayout.Label("ChangeLog Removed");
            publishInfo.changeLogRemoved = GUILayout.TextArea(publishInfo.changeLogRemoved, GUILayout.MinHeight(50));

            GUILayout.Space(10);
            GUILayout.Label("ChangeLog Fixed");
            publishInfo.changeLogFixed = GUILayout.TextArea(publishInfo.changeLogFixed, GUILayout.MinHeight(50));

            GUILayout.Space(10);
            GUILayout.Label("ChangeLog Other");
            publishInfo.changeLogOther = GUILayout.TextArea(publishInfo.changeLogOther, GUILayout.MinHeight(50));

            if (GUILayout.Button("保存到本地")) {
                GUIPublishLocal(model, publishInfo);
            }

        }

        void GUIPublishLocal(UPMasterDependancyModel model, PublishInfoWriterModel publishInfo) {
            string title = "信息确认";
            string content = $"以下文件将会被修改:\r\n"
                            + $"  {model.GetChangeLogPath()}\r\n"
                            + $"  {model.GetPackageJsonPath()}\r\n"
                            + $"是否确认保存?";
            if (EditorUtility.DisplayDialog(title, content, OK, CANCEL)) {
                PlayerSettings.bundleVersion = publishInfo.semanticVersion;
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                SaveLocalChange(model, publishInfo);
            }
        }

        void SaveLocalChange(UPMasterDependancyModel model, PublishInfoWriterModel publishInfo) {

            // class PackageInfo
            // SAVE PACKAGEJSON:
            //      VERSION
            var packageJsonPath = model.GetPackageJsonPath();
            if (!File.Exists(packageJsonPath)) {
                EditorUtility.DisplayDialog("错误", $"文件不存在: {packageJsonPath}", "确认");
                return;
            }
            string jsonStr = FileHelper.LoadTextFromFile(packageJsonPath);
            PackageJsonModel json = JsonConvert.DeserializeObject<PackageJsonModel>(jsonStr);
            if (json == null) {
                json = new PackageJsonModel();
            }
            json.version = publishInfo.semanticVersion;
            jsonStr = JsonConvert.SerializeObject(json, Formatting.Indented);
            FileHelper.SaveFileText(jsonStr, packageJsonPath);

            // SAVE CHANGELOG:
            //      VERSION
            //      ADDED/CHANGED/REMOVED/FIXED/OTHER
            var changeLogPath = model.GetChangeLogPath();
            if (!File.Exists(changeLogPath)) {
                EditorUtility.DisplayDialog("错误", $"文件不存在: {changeLogPath}", "确认");
                return;
            }
            ChangeLogModifier changeLog = new ChangeLogModifier();
            changeLog.Analyze(File.ReadAllLines(changeLogPath));
            changeLog.AddInfo(publishInfo.semanticVersion, ChangeLogElementTagCollection.TAG_ADDED, publishInfo.changeLogAdded);
            changeLog.AddInfo(publishInfo.semanticVersion, ChangeLogElementTagCollection.TAG_CHANGED, publishInfo.changeLogChanged);
            changeLog.AddInfo(publishInfo.semanticVersion, ChangeLogElementTagCollection.TAG_REMOVED, publishInfo.changeLogRemoved);
            changeLog.AddInfo(publishInfo.semanticVersion, ChangeLogElementTagCollection.TAG_FIXED, publishInfo.changeLogFixed);
            changeLog.AddInfo(publishInfo.semanticVersion, ChangeLogElementTagCollection.TAG_OTHER, publishInfo.changeLogOther);
            changeLog.EndEdit();
            FileHelper.SaveFileText(changeLog.ToString(), changeLogPath);

            publishInfo.currentVersion = VersionHelper.ReadCurrentVersion(model);

        }

    }

}