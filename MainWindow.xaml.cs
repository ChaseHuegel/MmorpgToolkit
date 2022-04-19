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
            ViewModel.SelectedNpc = (DataEntry)listBox.SelectedValue;
        }

        private void SaveNpc_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedNpc == null)
                return;

            ViewModel.Data.SaveNpc(ViewModel.SelectedNpc);
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
            int id;

            List<int> recycledIDs = ViewModel.Data.DeletedEntries.Select(npc => npc.ID).ToList();

            //  Attempt to recycle IDs from deleted entities
            if (recycledIDs.Count > 0)
            {
                recycledIDs.Sort();
                id = recycledIDs.Last();
            }
            //  Otherwise claim the first ID that is not in use
            else
            {
                List<int> claimedIDs = ViewModel.Data.NpcEntries.Select(npc => npc.ID).ToList();
                claimedIDs.Sort();

                id = 0;
                foreach (int claimedID in claimedIDs)
                    if (claimedID == id)
                        id++;
            }

            DataEntry npc = new DataEntry {
                Unsaved = true,
                ID = id,
                Name = "New NPC"
            };

            ViewModel.Data.NpcEntries.Insert(id, npc);
            listBox.SelectedIndex = id;
        }

        private void RemoveNpc_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedNpc == null)
                return;

            MessageBoxResult confirmResult = MessageBox.Show($"ID: {ViewModel.SelectedNpc.ID}\n\nName: {ViewModel.SelectedNpc.Name}", "Delete", MessageBoxButton.OKCancel);

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

        private void ReconnectDatabase_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Data.Database.Reconnect();
        }
    }
}
