using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MmorpgToolkit
{
    public class DataEntry : PropertyNotifier
    {
        private string? m_Description;
        private string? m_Label;
        private string? m_Name;
        private string? m_Title;
        private int m_ID;
        private int m_Lvl;
        private int m_HP;
        private Faction m_Faction;
        private Alignment m_Alignment;
        private Type m_Type;

        [SqlProperty]
        public int ID
        {
            get => GetProperty(ref m_ID);
            set => SetProperty(ref m_ID, value);
        }

        [SqlProperty]
        public string Label
        {
            get => GetProperty(ref m_Label);
            set => SetProperty(ref m_Label, value);
        }

        [SqlProperty]
        public string Name
        {
            get => GetProperty(ref m_Name);
            set => SetProperty(ref m_Name, value);
        }

        [SqlProperty]
        public string Title
        {
            get => GetProperty(ref m_Title);
            set => SetProperty(ref m_Title, value);
        }

        [SqlProperty]
        public int Lvl
        {
            get => GetProperty(ref m_Lvl);
            set => SetProperty(ref m_Lvl, value);
        }

        [SqlProperty]
        public int HP
        {
            get => GetProperty(ref m_HP);
            set => SetProperty(ref m_HP, value);
        }

        [SqlProperty]
        public Faction Faction
        {
            get => GetProperty(ref m_Faction);
            set => SetProperty(ref m_Faction, value);
        }

        [SqlProperty]
        public Alignment Alignment
        {
            get => GetProperty(ref m_Alignment);
            set => SetProperty(ref m_Alignment, value);
        }

        [SqlProperty]
        public Type Type
        {
            get => GetProperty(ref m_Type);
            set => SetProperty(ref m_Type, value);
        }

        [SqlProperty]
        public string Description {
            get => GetProperty(ref m_Description);
            set => SetProperty(ref m_Description, value);
        }

        private bool m_Unsaved = true;
        public bool Unsaved
        {
            get => GetProperty(ref m_Unsaved);
            set => SetProperty(ref m_Unsaved, value);
        }

        public string DisplayMember {
            get
            {
                string str = string.IsNullOrEmpty(Label) ? $"{ID} {Name}" : $"{ID} {Label} {Name}";

                if (Unsaved)
                    str = $"* {str}";

                return str;
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName != nameof(Unsaved))
                Unsaved = true;

            RaisePropertyChanged(nameof(DisplayMember));
        }

        public string GetSqlCommand(string table)
        {
            List<string> columns = new List<string>();
            List<string> values = new List<string>();
            List<string> keyValues = new List<string>();

            foreach (PropertyInfo property in this.GetSqlProperties())
            {
                string value = property.GetValue(this)?.ToString() ?? string.Empty;
                value = value.Replace("'", "''");

                columns.Add(property.Name);
                values.Add($"'{value}'");
                keyValues.Add($"{property.Name}='{value}'");
            }

            string columnsString = string.Join(',', columns);
            string valuesString = string.Join(',', values);
            string keyValuesString = string.Join(',', keyValues);

            string command = @$"UPDATE {table} SET {keyValuesString} WHERE id={ID}; IF @@ROWCOUNT = 0 INSERT INTO {table} ({columnsString}) VALUES({valuesString});";

            return command;
        }
    }
}
