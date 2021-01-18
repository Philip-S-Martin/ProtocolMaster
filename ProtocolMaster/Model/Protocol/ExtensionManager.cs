using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ProtocolMaster.Model.Protocol
{
    abstract class ExtensionManager<E, T> where T : IExtensionMeta where E : IExtension
    {
        [ImportMany]
        IEnumerable<ExportFactory<E, T>> AvailableExtensions { get; set; }
        ExportFactory<E, T> extensionFactory;
        ExportLifetimeContext<E> extensionContext;
        E extension;
        public bool IsRunning { get => !isDisposed; }
        bool isDisposed = true;
        bool isCanceled = false;

        public event EventHandler OnOptionsLoaded;
        protected Dispatcher UIDispatcher { get; set; }
        public void LoadOptions(CompositionContainer container)
        {
            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Debug.Log.Error(compositionException.ToString());
            }

            foreach (ExportFactory<E, T> i in AvailableExtensions)
            {
                if (i.Metadata.Name == "None" && i.Metadata.Version == "")
                {
                    Selected = i.Metadata;
                }
                Debug.Log.Error(typeof(E).ToString() + " found: '" + i.Metadata.Name + "' version: '" + i.Metadata.Version + "'");
            }
            UIDispatcher = Dispatcher.CurrentDispatcher;
            OnOptionsLoaded?.Invoke(this, new EventArgs());
        }
        public IExtensionMeta Selected
        {
            get { return extensionFactory.Metadata; }
            set
            {
                foreach (ExportFactory<E, T> i in AvailableExtensions)
                {
                    if (i.Metadata.Name == value.Name && i.Metadata.Version == value.Version)
                    {
                        extensionFactory = i;
                    }
                }
            }
        }
        public IEnumerable<IExtensionMeta> Options
        {
            get
            {
                List<IExtensionMeta> optionsList = new List<IExtensionMeta>();
                foreach (ExportFactory<E, T> i in AvailableExtensions)
                {
                    optionsList.Add(i.Metadata);
                }
                return optionsList;
            }
        }
        protected E CreateSelectedExtension()
        {
            if (isDisposed)
            {
                isDisposed = false;
                extensionContext = extensionFactory.CreateExport();
                extension = extensionContext.Value;
                if (typeof(IPromptUserSelect).IsAssignableFrom(extension.GetType()))
                {
                    (extension as IPromptUserSelect).UserSelectPrompt = PromptHandler.CallDropdown;
                }
                return extension;
            }
            else
            {
                throw new Exception("A new extension cannot be started before currently running extension is disposed");
            }
        }
        protected void DisposeSelectedExtension()
        {
            extensionContext.Dispose();
            isDisposed = true;
        }
        public void Cancel()
        {
            extension.IsCanceled = true;
            DisposeSelectedExtension();
        }
    }
}
