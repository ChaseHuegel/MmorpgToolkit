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
        private DataEntry? m_SelectedNpc;
        private DataEntry? m_SelectedItem;

        public Data Data
        {
            get => GetProperty(ref m_Data);
            set => SetProperty(ref m_Data, value);
        }

        public DataEntry SelectedNpc
        {
            get => GetProperty(ref m_SelectedNpc);
            set => SetProperty(ref m_SelectedNpc, value);
        }

        public DataEntry SelectedItem
        {
            get => GetProperty(ref m_SelectedItem);
            set => SetProperty(ref m_SelectedItem, value);
        }
    }
}
