using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Drive.v3.Data;

namespace ProtocolMaster.Component.Google
{
    public delegate void MarkupCallback(string parentID, string ID, string header);
    public class TreeFile : File
    {
        private readonly TreeFile parent;
        private List<TreeFile> children;
        private readonly File file;

        public TreeFile(File file, TreeFile parent = null)
        {
            Log.Error("Treefile(): " + file.Name + " created");
            this.file = file;
            this.parent = parent;
            children = new List<TreeFile>();
            FindChildren();
        }

        private void FindChildren()
        {
            List<File> childList = Drive.Instance.GetChildren(file);
            if (childList == null) return;
            foreach(File child in childList)
            {
                children.Add(new TreeFile(child, this));
            }
        }

        public void CallbackPreBF(MarkupCallback callback)
        {
            if(file.Parents != null)
                callback(file.Parents[0], file.Id, file.Name);
            else
                callback(null, file.Id, file.Name);
            foreach (TreeFile child in children)
            {
                CallbackPreBF(callback);
            }
        }
    }
}
