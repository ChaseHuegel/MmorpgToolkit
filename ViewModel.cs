using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmorpgToolkit
{
    public class ViewModel : PropertyNotifier
    {
        private Data m_Data = new Data();
        private DataEntry? m_SelectedEntry;

        public Data Data
        {
            get => GetProperty(ref m_Data);
            set => SetProperty(ref m_Data, value);
        }

        public DataEntry SelectedEntry
        {
            get => GetProperty(ref m_SelectedEntry);
            set => SetProperty(ref m_SelectedEntry, value);
        }
    }
}
