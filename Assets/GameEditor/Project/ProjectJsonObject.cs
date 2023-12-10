using System;
using GameEditor.Organizations;

namespace GameEditor.Project
{
    [Serializable]
    public class ProjectJsonObject : CommonJsonObjectFields
    {
        public string version;
        public string description;
        public string[] authors;
        public string[] contributors;
        public string localPath;
        public string localImagePath;
        public string localAudioPath;
        public string localVideoPath;
        public string localFontPath;
        public string localScriptPath;
        public string localPrefabPath;
        public string localScriptableObjectPath;
        public string localScenePath;
        public string localMaterialPath;
        public string localAnimationPath;
    }
}