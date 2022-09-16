using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace JackFrame.UPMaster.Publish {

    public static class VersionHelper {

        public static string ReadCurrentVersion(UPMasterDependancyModel model) {
            var filePath = model.GetPackageJsonPath();
            if (!File.Exists(filePath)) {
                Debug.LogWarning($"文件不存在: {filePath}");
                return "unknown";
            }
            string jsonStr = FileHelper.LoadTextFromFile(filePath);
            var json = JsonConvert.DeserializeObject<PackageJsonModel>(jsonStr);
            return json.version;
        }
    }
}