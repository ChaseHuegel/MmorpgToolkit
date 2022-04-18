using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MmorpgToolkit
{
    public class PropertyNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) { }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                OnPropertyChanged(propertyName);
            }
        }

        protected T GetProperty<T>(ref T property) => property;

        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string caller = "")
        {
            property = value;
            NotifyPropertyChanged(caller);
        }
    }
}
