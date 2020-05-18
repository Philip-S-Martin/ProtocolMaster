using Google.Apis.Drive.v3.Data;
using ProtocolMaster.Component.Debug;
using System.Collections.Generic;

namespace ProtocolMaster.Component.Google
{
    public delegate void MarkupCallback(string parentID, string ID, string header);
    public class TreeFile
    {
        private readonly TreeFile parent;
        private List<TreeFile> children;
        private readonly File file;

        public File File => file;

        public TreeFile(File file, TreeFile parent = null)
        {
            Log.Error("Treefile(): " + file.Name + " created");
            this.file = file;
            this.parent = parent;
            children = new List<TreeFile>();
            FindChildren();
        }

        public TreeFile AddChild(File file)
        {
            if (file == this.file)
                return this;
            TreeFile child = new TreeFile(file, this);
            children.Add(child);
            return child;
        }

        private void FindChildren()
        {
            if (parent == null)
            {
                List<File> childList = Drive.Instance.GetChildren(File);
                if (childList == null) return;
                foreach (File child in childList)
                {
                    children.Add(new TreeFile(child, this));
                }
            }
        }

        public void CallbackPreBF(MarkupCallback callback)
        {
            if(parent != null)
                Callback(callback);
            foreach (TreeFile child in children)
            {
                child.CallbackPreBF(callback);
            }
        }
        public void Callback(MarkupCallback callback)
        {
                callback(null, File.Id, File.Name);
        }
    }
}

