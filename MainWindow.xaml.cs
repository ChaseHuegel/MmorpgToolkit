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
    public partial class MainWindow : Window
    {
        private ViewModel m_ViewModel;
        public ViewModel ViewModel => m_ViewModel ?? (m_ViewModel = (ViewModel)DataContext);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedEntry = (DataEntry)listBox.SelectedValue;
        }

        private void SaveNpc_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedEntry == null)
                return;

            ViewModel.Data.SaveNpc(ViewModel.SelectedEntry);
        }

        private void SaveAllNpcs_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.Data.HasUnsavedChanges)
                return;

            MessageBoxResult confirmResult = MessageBox.Show($"Save all entries to the database?\n\nThis cannot be undone.", "Save All", MessageBoxButton.OKCancel);

            if (confirmResult == MessageBoxResult.OK)
                ViewModel.Data.SaveNpcData();
        }

        private void NewNpc_Click(object sender, RoutedEventArgs e)
        {
            DataEntry npc = new DataEntry {
                Unsaved = true,
                ID = ViewModel.Data.NpcEntries.Count,
                Name = "New NPC"
            };

            ViewModel.Data.NpcEntries.Add(npc);
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        private void RemoveNpc_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedEntry == null)
                return;

            MessageBoxResult confirmResult = MessageBox.Show($"ID: {ViewModel.SelectedEntry.ID}\n\nName: {ViewModel.SelectedEntry.Name}", "Delete", MessageBoxButton.OKCancel);

            if (confirmResult == MessageBoxResult.OK)
                ViewModel.Data.NpcEntries.RemoveAt(listBox.SelectedIndex);
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Data.HasUnsavedChanges)
            {
                MessageBoxResult confirmResult = MessageBox.Show("You have unsaved changes.", "Refresh", MessageBoxButton.OKCancel);

                if (confirmResult == MessageBoxResult.Cancel)
                    return;
            }

            int index = listBox.SelectedIndex;
            ViewModel.Data.LoadAll();
            listBox.SelectedIndex = Math.Clamp(index, -1, listBox.Items.Count - 1);
        }
    }
}
