namespace Task15.NHibernate
{
    public class DBContextBindings
    {
        public string Host;
        public int Port;
        public string Database;
        public string Username;
        public string Password;
        public DBContextBindings(string host, int port, string dataBase, string userName, string password)
        {
            Host = host;
            Port = port;
            Database = dataBase;
            Username = userName;
            Password = password;
        }
    }
}