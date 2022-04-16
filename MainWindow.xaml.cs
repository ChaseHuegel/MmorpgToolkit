using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MmorpgToolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public DataEntry SelectedEntry => listBox.SelectedValue == null ? null : (DataEntry)listBox.SelectedValue;

        public Data Data { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            Data = (Data) DataContext;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(SelectedEntry));
        }

        private void NewNpc_Click(object sender, RoutedEventArgs e)
        {
            DataEntry npc = new DataEntry {
                Unsaved = true,
                ID = Data.NpcEntries.Count.ToString(),
                Name = "New NPC"
            };
            Data.NpcEntries.Add(npc);
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        private void RemoveNpc_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            MessageBoxResult confirmResult = MessageBox.Show($"ID: {SelectedEntry.ID}\n\nName: {SelectedEntry.Name}", "Delete", MessageBoxButton.OKCancel);

            if (confirmResult == MessageBoxResult.OK)
                Data.NpcEntries.RemoveAt(listBox.SelectedIndex);
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            if (Data.HasUnsavedChanges)
            {
                MessageBoxResult confirmResult = MessageBox.Show("You have unsaved changes.", "Refresh", MessageBoxButton.OKCancel);

                if (confirmResult == MessageBoxResult.Cancel)
                    return;
            }

            int index = listBox.SelectedIndex;
            Data.LoadAll();
            listBox.SelectedIndex = Math.Clamp(index, -1, listBox.Items.Count - 1);
        }
    }
}
