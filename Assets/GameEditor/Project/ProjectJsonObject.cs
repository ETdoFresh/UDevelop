using System;

namespace GameEditor.Project
{
    [Serializable]
    public class ProjectJsonObject
    {
        public string name;
        public string guid;
        public long createdTick;
        public string createdBy;
        public long lastModifiedTick;
        public string lastModifiedBy;
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