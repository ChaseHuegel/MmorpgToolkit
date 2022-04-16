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
    public class Database : PropertyNotifier
    {
        private const int Timeout = 2;

        public string ConnectionString => $"Data Source={Address},{Port};Initial Catalog={Name};Trusted_Connection=True;Connection Timeout={Timeout}";

        public ConnectionState State => Connection?.State ?? ConnectionState.Closed;

        private SqlConnection? Connection;

        private string m_Name = "mmorpg";
        private string m_Address = "127.0.0.1";
        private int m_Port = 16261;

        public string Name
        {
            get => GetProperty(ref m_Name);
            set => SetProperty(ref m_Name, value);
        }

        public string Address
        {
            get => GetProperty(ref m_Address);
            set => SetProperty(ref m_Address, value);
        }

        public int Port
        {
            get => GetProperty(ref m_Port);
            set => SetProperty(ref m_Port, value);
        }

        public Database()
        {
            Connect();
        }

        private void Connect()
        {
            TryOpenConnection(out Connection);

            if (IsAvailable() && Connection != null)
            {
                Connection.StateChange += OnStateChanged;
            }
        }

        public void CloseConnection()
        {
            if (Connection == null)
                return;

            Connection.StateChange -= OnStateChanged;
            Connection.Close();
            Connection = null;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            CloseConnection();
            Connect();

            RaisePropertyChanged(nameof(State));
        }

        public bool IsAvailable() => Connection != null && State == ConnectionState.Open;

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
                cmd.CommandTimeout = Timeout;
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
                cmd.CommandTimeout = Timeout;
                reader = cmd.ExecuteReader();
                return true;
            }

            reader = null;
            return false;
        }

        private void OnStateChanged(object sender, StateChangeEventArgs e)
        {
            RaisePropertyChanged(nameof(State));
        }
    }
}
