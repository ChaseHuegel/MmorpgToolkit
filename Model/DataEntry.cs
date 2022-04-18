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
        private List<PropertyInfo>? m_SqlDataProperties;
        private List<PropertyInfo> SqlDataProperties => m_SqlDataProperties ??= GetType().GetProperties().Where(property => property.GetCustomAttribute<SqlDataAttribute>() != null).ToList();

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

        [SqlData]
        public int ID
        {
            get => GetProperty(ref m_ID);
            set => SetProperty(ref m_ID, value);
        }

        [SqlData]
        public string Label
        {
            get => GetProperty(ref m_Label);
            set => SetProperty(ref m_Label, value);
        }

        [SqlData]
        public string Name
        {
            get => GetProperty(ref m_Name);
            set => SetProperty(ref m_Name, value);
        }

        [SqlData]
        public string Title
        {
            get => GetProperty(ref m_Title);
            set => SetProperty(ref m_Title, value);
        }

        [SqlData]
        public int Lvl
        {
            get => GetProperty(ref m_Lvl);
            set => SetProperty(ref m_Lvl, value);
        }

        [SqlData]
        public int HP
        {
            get => GetProperty(ref m_HP);
            set => SetProperty(ref m_HP, value);
        }

        [SqlData]
        public Faction Faction
        {
            get => GetProperty(ref m_Faction);
            set => SetProperty(ref m_Faction, value);
        }

        [SqlData]
        public Alignment Alignment
        {
            get => GetProperty(ref m_Alignment);
            set => SetProperty(ref m_Alignment, value);
        }

        [SqlData]
        public Type Type
        {
            get => GetProperty(ref m_Type);
            set => SetProperty(ref m_Type, value);
        }

        [SqlData]
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

            foreach (PropertyInfo property in SqlDataProperties)
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
