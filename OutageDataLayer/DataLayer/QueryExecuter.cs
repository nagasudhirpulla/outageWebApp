using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OutageDataLayer.DataLayer
{
    public class QueryExecuter
    {
        public string ConnStr { get; set; } = "";

        public TableRowsApiResultModel GetDbTableRows(string sqlStr, List<OracleParameter> parameters)
        {
            // initiate the result
            TableRowsApiResultModel rows = new TableRowsApiResultModel();

            // create and open the connection
            OracleConnection conn = new OracleConnection(ConnStr); // C#
            conn.Open();
            try
            {
                // create the command for querying
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = sqlStr,
                    CommandType = CommandType.Text
                };

                if (parameters.Count > 0)
                {
                    cmd.BindByName = true;
                    for (int paramIter = 0; paramIter < parameters.Count; paramIter++)
                    {
                        cmd.Parameters.Add(parameters[paramIter]);
                    }
                }

                // execute command and read into an oracle data reader object
                OracleDataReader dr = cmd.ExecuteReader();

                // populate the column names
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string colName = dr.GetName(i);
                    rows.TableColNames.Add(colName);
                    string colType = dr.GetFieldType(i).Name;
                    rows.TableColTypes.Add(colType);
                }

                // populate the rows of the query table
                while (dr.Read())
                {
                    object[] objs = new object[dr.FieldCount];
                    dr.GetValues(objs);
                    rows.TableRows.Add(new List<object>(objs));
                    //Console.WriteLine(dr.GetInt32(dr.GetOrdinal("ID")));
                    //Console.WriteLine(dr.GetString(dr.GetOrdinal("LINE_NAME")));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //close the connection
            conn.Close();

            // free the resources
            conn.Dispose();

            // return the result
            return rows;
        }


    }
}
