using System.Linq;
using System.IO;
using System.Net.Http;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace JackFrame.UPMaster.Subscription {

    public class SubscriptionGUI {

        const string URI = "http://upmaster.utea.fun/get_packages";
        Vector2 scrollPos;

        UPMasterDependancyModel[] all;
        string[] packageNames;
        int selectedPackage;

        public SubscriptionGUI() { }

        public UPMasterDependancyModel GUIListAndChooseDependancy() {

            UPMasterDependancyModel chosen = null;

            GUILayout.Label("订阅源");
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            // List All
            if (all != null) {
                selectedPackage = GUILayout.SelectionGrid(selectedPackage, packageNames, 1);
                chosen = all[selectedPackage];
            }

            // Get
            if (GUILayout.Button("获取订阅源", GUILayout.Width(120))) {
                all = GetSubscription();
                packageNames = all.Select(model => model.packageName).ToArray();
            }

            GUILayout.EndScrollView();

            return chosen;

        }

        UPMasterDependancyModel[] GetSubscription() {

            HttpClient client = new HttpClient();
            var res = client.GetAsync(URI).Result;
            var content = res.Content.ReadAsStringAsync().Result;
            Debug.Log($"获取: {content}"); 
            var arr = JsonConvert.DeserializeObject<UPMasterDependancyModel[]>(content);
            return arr;

        }

    }

}
