using ProtocolMasterWPF.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProtocolMasterWPF.ViewModel
{
    public class PublishedProtocolDataViewModel : ViewModelBase, IDataErrorInfo
    {
        PublishedFileStreamer target;
        internal PublishedProtocolDataViewModel()
        {
        }
        internal PublishedProtocolDataViewModel(PublishedFileStreamer target)
        {
            this.target = target;
            this.URL = target.URL;
            this.Label = target.Name;
        }
        public string this[string columnName]
        {
            get
            {
                return OnValidate(columnName);
            }
        }
        private string OnValidate(string columnName)
        {
            string result = null;
            switch (columnName)
            {
                case "Label":
                    labelError = true;
                    if (string.IsNullOrEmpty(Label))
                        result = "Name is required.";
                    else if (PublishedFileStore.Instance.PublishedFiles.Any(a => a.Name == Label && a!= target))
                        result = "Name must be unique.";
                    else labelError = false;
                    break;
                case "URL":
                    urlError = true;
                    if (string.IsNullOrEmpty(URL))
                        result = "URL is required.";
                    else if (PublishedFileStore.Instance.PublishedFiles.Any(a => a.URL == URL && a != target))
                        result = "URL must be unique.";
                    else if (!Regex.IsMatch(URL, "^(?:http(s)?:\\/\\/)?[\\w.-]+(?:\\.[\\w\\.-]+)+[\\w\\-\\._~:/?#[\\]@!\\$&'\\(\\)\\*\\+,;=.]+$"))
                        result = "URL must be valid.";
                    else urlError = false;
                    break;
            }
            OnPropertyChanged("Error");
            return result;
        }
        private string label;
        public string Label
        {
            get => label;
            set
            {
                label = value;
                OnPropertyChanged();
            }
        }
        bool labelError = false;

        private string url;
        public string URL
        {
            get => url;
            set
            {
                url = value;
                OnPropertyChanged();
            }
        }
        bool urlError = false;

        private string _error;
        public string Error
        {
            get
            {
                if (urlError || labelError) return "Error";
                else return null;
            }
        }
        public bool TryUpdateTarget()
        {
            if (Error != null) return false;
            else
            {
                PublishedFileStore.Instance.Update(target, Label, URL);
                return true;
            }
        }
    }
}
