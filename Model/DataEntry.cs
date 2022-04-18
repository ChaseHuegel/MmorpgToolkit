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
        private NpcType m_Type;

        [SqlProperty("ID")]
        public int ID
        {
            get => GetProperty(ref m_ID);
            set => SetProperty(ref m_ID, value);
        }

        [SqlProperty("Label")]
        public string Label
        {
            get => GetProperty(ref m_Label);
            set => SetProperty(ref m_Label, value);
        }

        [SqlProperty("Name")]
        public string Name
        {
            get => GetProperty(ref m_Name);
            set => SetProperty(ref m_Name, value);
        }

        [SqlProperty("Title")]
        public string Title
        {
            get => GetProperty(ref m_Title);
            set => SetProperty(ref m_Title, value);
        }

        [SqlProperty("Lvl")]
        public int Lvl
        {
            get => GetProperty(ref m_Lvl);
            set => SetProperty(ref m_Lvl, value);
        }

        [SqlProperty("HP")]
        public int HP
        {
            get => GetProperty(ref m_HP);
            set => SetProperty(ref m_HP, value);
        }

        [SqlProperty("Faction")]
        public Faction Faction
        {
            get => GetProperty(ref m_Faction);
            set => SetProperty(ref m_Faction, value);
        }

        [SqlProperty("Alignment")]
        public Alignment Alignment
        {
            get => GetProperty(ref m_Alignment);
            set => SetProperty(ref m_Alignment, value);
        }

        [SqlProperty("Type")]
        public NpcType Type
        {
            get => GetProperty(ref m_Type);
            set => SetProperty(ref m_Type, value);
        }

        [SqlProperty("Description")]
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
                string columnNameAttribute = property.GetCustomAttribute<SqlPropertyAttribute>().ColumnName;

                string value = property.GetValue(this)?.ToString() ?? string.Empty;
                value = value.Replace("'", "''");
                string column = string.IsNullOrEmpty(columnNameAttribute) ? property.Name : columnNameAttribute;

                columns.Add(column);
                values.Add($"'{value}'");
                keyValues.Add($"{column}='{value}'");
            }

            string columnsString = string.Join(',', columns);
            string valuesString = string.Join(',', values);
            string keyValuesString = string.Join(',', keyValues);

            string command = @$"UPDATE {table} SET {keyValuesString} WHERE id={ID}; IF @@ROWCOUNT = 0 INSERT INTO {table} ({columnsString}) VALUES({valuesString});";

            return command;
        }
    }
}
