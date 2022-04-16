using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmorpgToolkit
{
    public class DataEntry : INotifyPropertyChanged
    {
        private string m_Description;

        public bool Unsaved = true;
        private string m_ID;
        private string m_Label;
        private string m_Name;
        private string m_Title;
        private int m_Lvl;
        private int m_HP;
        private Faction m_Faction;
        private Alignment m_Alignment;
        private Type m_Type;

        public string ID {
            get => m_ID;
            set
            {
                m_ID = value;
                NotifyPropertyChanged(nameof(ID));
            }
        }

        public string Label {
            get => m_Label;
            set
            {
                m_Label = value;
                NotifyPropertyChanged(nameof(Label));
            }
        }

        public string Name {
            get => m_Name;
            set
            {
                m_Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        public string Title {
            get => m_Title;
            set
            {
                m_Title = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }

        public int Lvl {
            get => m_Lvl;
            set
            {
                m_Lvl = value;
                NotifyPropertyChanged(nameof(Lvl));
            }
        }

        public int HP {
            get => m_HP;
            set
            {
                m_HP = value;
                NotifyPropertyChanged(nameof(HP));
            }
        }

        public Faction Faction {
            get => m_Faction;
            set
            {
                m_Faction = value;
                NotifyPropertyChanged(nameof(Faction));
            }
        }

        public Alignment Alignment {
            get => m_Alignment;
            set
            {
                m_Alignment = value;
                NotifyPropertyChanged(nameof(Alignment));
            }
        }

        public Type Type {
            get => m_Type;
            set
            {
                m_Type = value;
                NotifyPropertyChanged(nameof(Type));
            }
        }

        public string Description {
            get => m_Description;
            set
            {
                m_Description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        public string DisplayMember {
            get
            {
                string str = string.IsNullOrEmpty(Label) ? $"{ID}:{Name}" : $"{Label} {ID}:{Name}";

                if (Unsaved)
                    str = $"* {ID}:{Name}";

                return str;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(DisplayMember)));
            }
        }
    }
}
