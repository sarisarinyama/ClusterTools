using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace sarisarinyama.common
{
    public static class MySQLUtil
    {
        private static string host = "sarisarinyama.ddns.net";
        private static string user = "guest";
        private static string password = "guest";
        private static bool pooling = true;
        static string _connectionString;

        private static string database = "Cluster";

        private static MySqlConnection _con = null;
        private static MySqlCommand _cmd = null;

        public static bool DBConnection()
        {
//            Debug.Log("db connection start");
            _connectionString = $"Server={host};Database={database};User={user};Password={password};Pooling=";
            if (pooling)
            {
                _connectionString += "True";
            }
            else
            {
                _connectionString += "False";
            }

            try
            {
                _con = new MySqlConnection();
                _con.ConnectionString = _connectionString;
                _con.Open();
//                Debug.Log("db connection open");
                return true;
            }


            catch (Exception e)
            {
                Debug.Log("db connection error");
                _con.Dispose();
                Debug.Log(e);
                return false;
            }
        }

        public static bool DBClose()
        {
            if (_con != null)
            {
                if (_con.State.ToString() != "Closed")
                {
                    _con.Close();
//                    Debug.Log("db connection closed");
                }

                _con.Dispose();
            }

            return true;
        }

        public static DataTable DBSelect(string selectSql, string[] parameters)
        {
            DataTable tbl = new DataTable();
            using (MySqlCommand _cmd = new MySqlCommand())
            {
                MySqlDataReader _rdr = null;
                _cmd.Connection = _con;
                _cmd.CommandText = selectSql;
                for (int i = 0; i < parameters.Length; i += 2)
                {
                    _cmd.Parameters.AddWithValue(parameters[i], parameters[i + 1]);
                }

                _rdr = _cmd.ExecuteReader();
                if (_rdr.HasRows)
                {
                    tbl.Load(_rdr);
                }
            }

            return tbl;
        }


        public static DataTable DBSelect(string _selectSql)
        {
            DataTable tbl = new DataTable();
            using (MySqlCommand _cmd = new MySqlCommand())
            {
                MySqlDataReader _rdr = null;
                _cmd.Connection = _con;
                _cmd.CommandText = _selectSql;
                _rdr = _cmd.ExecuteReader();
                if (_rdr.HasRows)
                {
                    tbl.Load(_rdr);
                }
            }

            return tbl;
        }

        public static bool DBInsert(string insertSql, string[] parameters)
        {
            try
            {
                using (_cmd = new MySqlCommand())
                {
                    _cmd.Connection = _con;
                    _cmd.CommandText = insertSql;
                    for (int i = 0; i < parameters.Length; i += 2)
                    {
                        _cmd.Parameters.AddWithValue(parameters[i], parameters[i + 1]);
                    }


                    var result = _cmd.ExecuteNonQuery();
                    if (result != 1)
                    {
                        Debug.Log("db insert error");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("db insert error");
                Debug.Log(e);
                MySQLUtil.DBClose();
                return false;
            }

            return true;
        }
    }
}