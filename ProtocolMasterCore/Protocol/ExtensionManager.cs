using ProtocolMasterCore.Prompt;
using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace ProtocolMasterCore.Protocol
{
    public delegate void OptionsLoadedCallback(List<IExtensionMeta> options);
    public abstract class ExtensionManager<E, T> where T : IExtensionMeta where E : IExtension
    {
        [ImportMany]
        IEnumerable<ExportFactory<E, T>> Extensions { get; set; }
        ExportFactory<E, T> extensionFactory;
        ExportLifetimeContext<E> extensionContext;
        E extension;
        internal bool IsRunning { get => !isDisposed; }
        bool isDisposed = true;
        public OptionsLoadedCallback OnOptionsLoaded { get; set; }
        public PromptTargetStore PromptTargets { get; set; }

        public void LoadOptions(CompositionContainer container)
        {
            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Log.Error(compositionException.ToString());
            }

            var extensionMeta = new List<IExtensionMeta>();
            foreach (ExportFactory<E, T> i in Extensions)
            {
                if (i.Metadata.Name == "None" && i.Metadata.Version == "")
                {
                    Selected = i.Metadata;
                }
                extensionMeta.Add(i.Metadata);
                Log.Error($"{typeof(E)} found: '{i.Metadata.Name}' version: '{i.Metadata.Version}'");
            }
            OnOptionsLoaded?.Invoke(extensionMeta);
        }
        public IExtensionMeta Selected
        {
            get { return extensionFactory.Metadata; }
            set
            {
                foreach (ExportFactory<E, T> i in Extensions)
                {
                    if (i.Metadata.Name == value.Name && i.Metadata.Version == value.Version)
                    {
                        extensionFactory = i;
                    }
                }
                throw new Exception($"No extension of type {typeof(E)} with name {value.Name} is loaded");
            }
        }
        public IEnumerable<IExtensionMeta> Options
        {
            get
            {
                List<IExtensionMeta> optionsList = new List<IExtensionMeta>();
                foreach (ExportFactory<E, T> i in Extensions)
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
                    (extension as IPromptUserSelect).UserSelectPrompt = PromptTargets.UserSelect;
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

        public void CancelRunningExtension()
        {
            extension.IsCanceled = true;
            DisposeSelectedExtension();
        }
    }
}
