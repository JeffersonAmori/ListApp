using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ListApp.Models
{
    public class ListItem : INotifyPropertyChanged
    {
        private bool _checked;

        public long Id { get; set; }
        public string Guid { get; set; }
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
        public long ListId { get; set; }
        public int Index { get; set; }

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