using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmorpgToolkit
{
    public class Data : PropertyNotifier
    {
        private bool m_HasUnsavedChanges;

        public bool HasUnsavedChanges { 
            get => GetProperty(ref m_HasUnsavedChanges);
            set => SetProperty(ref m_HasUnsavedChanges, value);
        }

        public Database Database { get; set; }

        public ObservableCollection<DataEntry> NpcEntries { get; set; } = new ObservableCollection<DataEntry>();

        public HashSet<DataEntry> UnsavedEntries { get; private set; } = new HashSet<DataEntry>();

        public HashSet<DataEntry> DeletedEntries { get; private set; } = new HashSet<DataEntry>();

        public Data()
        {
            Database = new Database();
            LoadAll();

            NpcEntries.CollectionChanged += OnNpcEntriesChanged;
        }

        private void OnNpcEntriesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                UnsavedEntries.Add(e.NewItems[0] as DataEntry);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                DeletedEntries.Add(e.OldItems[0] as DataEntry);
            }

            UpdateUnsavedChangesFlag();
        }

        private void OnNpcPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Unsaved")
                UnsavedEntries.Add(sender as DataEntry);

            UpdateUnsavedChangesFlag();
        }

        private void UpdateUnsavedChangesFlag() => HasUnsavedChanges = UnsavedEntries.Any() || DeletedEntries.Any();

        public void LoadAll()
        {
            LoadNpcData();
        }

        public bool SaveNpc(DataEntry dataEntry)
        {
            if (Database.TrySendCommand(dataEntry.GetSqlCommand("npcs")))
            {
                dataEntry.Unsaved = false;
                UnsavedEntries.Remove(dataEntry);
                UpdateUnsavedChangesFlag();
                return true;
            }

            return !dataEntry.Unsaved;
        }

        public void SaveNpcData()
        {
            foreach (DataEntry dataEntry in NpcEntries)
                SaveNpc(dataEntry);

            foreach (DataEntry dataEntry in DeletedEntries)
                Database.TrySendCommand($"DELETE FROM npcs WHERE id={dataEntry.ID}");

            DeletedEntries.Clear();
        }

        public void LoadNpcData()
        {
            string cmd = "SELECT * FROM npcs";
            if (Database.TrySendCommand(cmd, out SqlDataReader? reader))
            {
                //  Remove any event listeners
                foreach (DataEntry npc in NpcEntries)
                    npc.PropertyChanged -= OnNpcPropertyChanged;

                //  We want to be capable of continuing work without losing previously loaded data.
                //  Only clear data if the command was successful.
                NpcEntries.Clear();

                using (reader)
                {
                    while (reader?.Read() ?? false)
                    {
                        int id = reader.GetInt32(0);
                        string label = reader.GetString(1);
                        string name = reader.GetString(2);
                        string title = reader.GetString(3);
                        int level = reader.GetInt32(4);
                        int hp = reader.GetInt32(5);
                        Faction faction = (Faction)Enum.Parse(typeof(Faction), reader.GetString(6), true);
                        Alignment alignment = (Alignment)Enum.Parse(typeof(Alignment), reader.GetString(7), true);
                        NpcType type = (NpcType)Enum.Parse(typeof(NpcType), reader.GetString(8), true);
                        string description = reader.GetString(9);

                        DataEntry npc = new DataEntry {
                            Unsaved = false,

                            ID = id,
                            Label = label,
                            Name = name,
                            Title = title,
                            Lvl = level,
                            HP = hp,
                            Faction = faction,
                            Alignment = alignment,
                            Type = type,
                            Description = description
                        };

                        npc.PropertyChanged += OnNpcPropertyChanged;

                        NpcEntries.Add(npc);
                    }

                    //  Ensure no unsaved changes are flagged
                    UnsavedEntries.Clear();
                    DeletedEntries.Clear();
                    HasUnsavedChanges = false;

                    reader?.Close();
                }
            }
        }
    }
}
