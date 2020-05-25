using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sporcard
{
    public class AccessOperator
    {
        string connectionString = "";

        public AccessOperator()
        {
            connectionString = @"Provider = Microsoft.Jet.OLEDB.4.0; Data Source =" + Application.StartupPath + "\\CardData.mdb;";
            //connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\CardData.accdb;";
        }

        public AccessOperator(string strCnn)
        {
            connectionString = strCnn;
        }

        #region  ExecuteNonQuery操作，对数据库进行 增、删、改 操作(（1）
        /// <summary>
        /// ExecuteNonQuery操作，对数据库进行 增、删、改 操作(（1）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <returns> </returns>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }

        /// <summary>
        /// ExecuteNonQuery操作，对数据库进行 增、删、改 操作（2）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param>
        /// <returns> </returns>
        public int ExecuteNonQuery(string sql, CommandType commandType)
        {
            return ExecuteNonQuery(sql, commandType, null);
        }

        /// <summary>
        /// ExecuteNonQuery操作，对数据库进行 增、删、改 操作（3）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param>
        /// <param name="parameters">参数数组 </param>
        /// <returns> </returns>
        public int ExecuteNonQuery(string sql, CommandType commandType, OleDbParameter[] parameters)
        {
            int count = 0;
            try
            {
               
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {

                    using (OleDbCommand command = new OleDbCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (OleDbParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        connection.Open();
                        count = command.ExecuteNonQuery();
                    }
                }
                return count;
            }
            catch(Exception EX)
            {
                return 0;
            }
            
        }

        #endregion

        #region OleDbDataAdapter的Fill方法执行一个查询，并返回一个DataSet类型结果（1）
        /// <summary>
        /// OleDbDataAdapter的Fill方法执行一个查询，并返回一个DataSet类型结果（1）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <returns> </returns>
        public DataSet ExecuteDataSet(string sql)
        {
            return ExecuteDataSet(sql, CommandType.Text, null);
        }

        /// <summary>
        /// OleDbDataAdapter的Fill方法执行一个查询，并返回一个DataSet类型结果（2）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param>
        /// <returns> </returns>
        public DataSet ExecuteDataSet(string sql, CommandType commandType)
        {
            return ExecuteDataSet(sql, commandType, null);
        }

        /// <summary>
        /// OleDbDataAdapter的Fill方法执行一个查询，并返回一个DataSet类型结果（3）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param>
        /// <param name="parameters">参数数组 </param>
        /// <returns> </returns>
        public DataSet ExecuteDataSet(string sql, CommandType commandType, OleDbParameter[] parameters)
        {
            DataSet ds = new DataSet();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (OleDbParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                    adapter.Fill(ds);
                }
            }
            return ds;
        }

        /// <summary>
        /// OleDbDataAdapter的Fill方法执行一个查询，并返回一个DataTable类型结果（1）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <returns> </returns>
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(sql, CommandType.Text, null);
        }

        /// <summary>
        /// OleDbDataAdapter的Fill方法执行一个查询，并返回一个DataTable类型结果（2）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param>
        /// <returns> </returns>
        public DataTable ExecuteDataTable(string sql, CommandType commandType)
        {
            return ExecuteDataTable(sql, commandType, null);
        }

        /// <summary>
        /// OleDbDataAdapter的Fill方法执行一个查询，并返回一个DataTable类型结果（3）
        /// </summary>
        /// <param name="sql">要执行的SQL语句 </param>
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param>
        /// <param name="parameters">参数数组 </param>
        /// <returns> </returns>
        public DataTable ExecuteDataTable(string sql, CommandType commandType, OleDbParameter[] parameters)
        {
            DataTable data = new DataTable();
            try
            { using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    using (OleDbCommand command = new OleDbCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (OleDbParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                        adapter.Fill(data);
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return data;
        }

        #endregion

        public DataTable GetTables(string tablename)
        {
            DataTable data = null;
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                data = connection.GetSchema(tablename);
            }
            return data;
        }

        public bool CheckExistsTable(string tablename)
        {
            DataTable data = new DataTable();
            //String tableNameStr = "select count(1) from msysobjects where name = '" + tablename + "'";
            string tableNameStr = "select count(*) from BatchInfo where BatchNo04 = '" + tablename + "'";
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(tableNameStr, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    //OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    //adapter.Fill(data);
                    conn.Open();
                    object ob = cmd.ExecuteScalar();
                    int result = Convert.ToInt32(ob);
                    conn.Close();
                    if (result == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
    }
}
