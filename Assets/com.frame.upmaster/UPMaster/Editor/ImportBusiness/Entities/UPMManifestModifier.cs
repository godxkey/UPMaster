using System;
using System.IO;
using System.Collections.Generic;

namespace JackFrame.UPMaster.Subscription {

    public class UPMManifestModifier {

        List<ManifestDepModel> all;

        public UPMManifestModifier() {
            this.all = new List<ManifestDepModel>();
        }

        public void Initialize() {

            string path = Path.Combine(Environment.CurrentDirectory, "Packages", "manifest.json");
            var json = File.ReadAllText(path);

            // TRIM
            json = json.Replace("\r\n", "");
            json = json.Replace(" ", "");

            // CACHE ALL
            var arr = json.Split("{");
            arr = arr[2].Split("}");
            arr = arr[0].Split(",");
            foreach (var str in arr) {
                var kv = str.Split("\":\"");
                string key = kv[0];
                string value = kv[1];
                var model = new ManifestDepModel(key.Replace("\"", "").Trim(), value.Replace("\"", "").Trim());
                all.Add(model);
            }

        }

        public ManifestDepModel Find(string name) {
            return all.Find(value => value.name == name);
        }

        public void Add(string key, string value) {
            int existIndex = all.FindIndex(value => value.name == key);
            if (existIndex != -1) {
                return;
            } else {
                all.Add(new ManifestDepModel(key, value));
            }
        }

        public void AddOrReplace(string key, string value) {
            int existIndex = all.FindIndex(value => value.name == key);
            if (existIndex != -1) {
                var model = all[existIndex];
                model.version = value;
            } else {
                all.Add(new ManifestDepModel(key, value));
            }
        }

        public void Replace(string key, string value) {
            int existIndex = all.FindIndex(value => value.name == key);
            if (existIndex != -1) {
                var model = all[existIndex];
                model.version = value;
            }
        }

        public void Remove(string key) {
            int existIndex = all.FindIndex(value => value.name == key);
            if (existIndex != -1) {
                all.RemoveAt(existIndex);
            }
        }

        public string Generate() {
            string data = "";
            for (int i = 0; i < all.Count; i += 1) {
                var value = all[i];
                if (string.IsNullOrEmpty(value.name)) {
                    continue;
                }
                string per;
                if (i != all.Count - 1) {
                    per = "\t\t\"" + value.name + "\":\"" + @value.version + "\",\r\n";
                } else {
                    per = "\t\t\"" + value.name + "\":\"" + @value.version + "\"\r\n";
                    UnityEngine.Debug.Log("HERE");
                }
                UnityEngine.Debug.Log($"{i}/{all.Count}");
                data += per;
            }
            string str = "{\r\n\t\"dependencies\": {\r\n" + data + "\t}\r\n}";
            return str;
        }

    }
}