using System;
using System.IO;
using UnityEngine;

namespace JackFrame.UPMaster {

    [Serializable]
    public class UPMasterDependancyModel {

        public string packageName;
        public string gitUrl;
        public string branchOrTag;

        public UPMasterDependancyModel() {
            this.packageName = "";
            this.gitUrl = "";
            this.branchOrTag = "";
        }

        public string GetPackageDir() {
            return Path.Combine(Application.dataPath, packageName);
        }

        public string GetFullURL() {
            var realUrl = gitUrl.Replace(".com:", ".com/");
            return realUrl + "?path=" + "/Assets/" + packageName + "#" + branchOrTag;
        }

        public string GetPackageJsonPath() {
            return Path.Combine(GetPackageDir(), "package.json");
        }

        public string GetChangeLogPath() {
            return Path.Combine(GetPackageDir(), "CHANGELOG.md");
        }

        public bool Check() {

            if (string.IsNullOrEmpty(packageName)) {
                return false;
            }

            if (string.IsNullOrEmpty(gitUrl)) {
                return false;
            }

            if (string.IsNullOrEmpty(branchOrTag)) {
                return false;
            }

            if (!gitUrl.StartsWith("https://") && !gitUrl.StartsWith("ssh://")) {
                return false;
            }

            return true;

        }

    }

}