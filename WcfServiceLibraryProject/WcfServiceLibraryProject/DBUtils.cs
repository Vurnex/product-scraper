using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace WcfServiceLibraryProject
{
    static class DBUtils
    {
        public static MySqlConnection CreateConnection()
        {
            String connStr = @"Connection Details Here";

            MySqlConnection conn = new MySqlConnection(connStr);

            return conn;

        }
    }
}

