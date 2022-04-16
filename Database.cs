using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmorpgToolkit
{
    public class Database : INotifyPropertyChanged
    {
        public string ConnectionString = "Server=CHASE-PC\\SQLEXPRESS01;Database=mmorpg;Trusted_Connection=True;";

        public ConnectionState State => Connection?.State ?? ConnectionState.Closed;

        public event PropertyChangedEventHandler? PropertyChanged;

        private SqlConnection? Connection;

        public Database()
        {
            TryOpenConnection(out Connection);

            if (IsAvailable() && Connection != null)
            {
                Connection.StateChange += OnStateChanged;
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsAvailable() => Connection != null && State == ConnectionState.Open;

        public void CloseConnection() => Connection?.Close();

        public bool TryOpenConnection(out SqlConnection? connection)
        {
            try
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();
                return true;
            }
            catch
            {
                connection = null;
                return false;
            }
        }

        public bool TrySendCommand(string command)
        {
            if (IsAvailable())
            {
                SqlCommand cmd = new SqlCommand(command, Connection);
                cmd.ExecuteNonQuery();
                return true;
            }

            return false;
        }

        public bool TrySendCommand(string command, out SqlDataReader? reader)
        {
            if (IsAvailable())
            {
                SqlCommand cmd = new SqlCommand(command, Connection);
                reader = cmd.ExecuteReader();
                return true;
            }

            reader = null;
            return false;
        }

        private void OnStateChanged(object sender, StateChangeEventArgs e)
        {
            NotifyPropertyChanged(nameof(State));
        }
    }
}
