using System.IO;
using UnityEngine;

namespace JackFrame.UPMaster.Publish {

    [CreateAssetMenu(menuName = nameof(JackFrame) + "/" + nameof(GitRepoConfigModel))]
    public class GitRepoConfigModel : ScriptableObject {

        public UPMasterDependancyModel dependancyModel;

    }

}