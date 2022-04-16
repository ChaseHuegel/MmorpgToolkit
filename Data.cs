using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmorpgToolkit
{
    public class Data : INotifyPropertyChanged
    {
        private bool m_HasUnsavedChanges;

        public bool HasUnsavedChanges { 
            get => m_HasUnsavedChanges;
            private set
            {
                m_HasUnsavedChanges = value;
                NotifyPropertyChanged(nameof(HasUnsavedChanges));
            }
        }

        public Database Database { get; set; }

        public ObservableCollection<DataEntry> NpcEntries { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Data()
        {
            Database = new Database();
            LoadAll();

            NpcEntries.CollectionChanged += OnCollectionChanged;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HasUnsavedChanges = true;
        }

        public void LoadAll()
        {
            LoadNpcData();
        }

        public void LoadNpcData()
        {
            if (NpcEntries == null)
                NpcEntries = new ObservableCollection<DataEntry>();

            string cmd = "SELECT * FROM npcs";
            if (Database.TrySendCommand(cmd, out SqlDataReader reader))
            {
                //  We want to be capable of continuing work without losing previously loaded data.
                //  Only clear data if the command was successful.
                NpcEntries.Clear();

                using (reader)
                {
                    while (reader.Read())
                    {
                        string id = reader.GetString(0);
                        string label = reader.GetString(1);
                        string name = reader.GetString(2);
                        string title = reader.GetString(3);
                        int level = reader.GetInt32(4);
                        int hp = reader.GetInt32(5);
                        Faction faction = (Faction)Enum.Parse(typeof(Faction), reader.GetString(6), true);
                        Alignment alignment = (Alignment)Enum.Parse(typeof(Alignment), reader.GetString(7), true);
                        Type type = (Type)Enum.Parse(typeof(Type), reader.GetString(8), true);
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

                        NpcEntries.Add(npc);
                    }

                    //  Ensure unsaved changes isn't flagged
                    HasUnsavedChanges = false;
                }
            }
        }
    }
}
