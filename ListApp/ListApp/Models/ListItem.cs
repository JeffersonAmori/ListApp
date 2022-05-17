using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ListApp.Models
{
    public class ListItem : INotifyPropertyChanged
    {
        private bool _checked;

        public string Id { get; set; }
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged();
            }
        }

        public string Text { get; set; }
        public string Description { get; set; }
        public string ListId { get; set; }
        public int Index { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastChangedDate { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}