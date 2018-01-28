using SoftIms.Data.Infrastructure;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MediCare.Data.Helper
{
    public class SqlHelper : IDisposable
    {
        #region Constructor
        private DatabaseContext ctx;
        public SqlHelper()
        {
            ctx = new DatabaseContext();
            this.ConnectionString = ctx.Database.Connection.ConnectionString;
        }
        public string ConnectionString { get; set; }
        #endregion

        #region Data Read Function
        public DataSet ExecuteDataSet(CommandType cmdType, string cmdString)
        {
            try
            {
                SqlConnection cn = new SqlConnection(this.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                cmd.CommandType = cmdType;
                cmd.CommandText = cmdString;
                cmd.Connection = cn;
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet ExecuteDataSet(CommandType cmdType, string cmdString, SqlParameter[] sqlParameters)
        {
            try
            {
                SqlConnection cn = new SqlConnection(this.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                cmd.CommandType = cmdType;
                cmd.CommandText = cmdString;
                cmd.Parameters.AddRange(sqlParameters);
                cmd.Connection = cn;
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ExecuteScalar(CommandType cmdType, string cmdString)
        {
            return ExecuteScalar(cmdType, cmdString, new SqlParameter[0]);
        }

        public object ExecuteScalar(CommandType cmdType, string cmdString, SqlParameter[] sqlParameters)
        {
            SqlConnection cn = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdString;

            if (sqlParameters.Length > 0)
                cmd.Parameters.AddRange(sqlParameters);

            cmd.Connection = cn;

            if (cn.State != ConnectionState.Open)
                cn.Open();

            object result = cmd.ExecuteScalar();
            cn.Close();
            cn.Dispose();

            return result;
        }
        #endregion

        #region Data Write Function
        public string ExecuteNonQuery(CommandType cmdType, string cmdString)
        {
            SqlConnection cn = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = cmdString;
            cmd.CommandType = cmdType;
            cmd.Connection = cn;
            cn.Open();
            try
            {
                cmd.ExecuteNonQuery();
                return "success";
            }
            catch (Exception ex)
            {
                return "error \n" + ex.Message;
            }
            finally
            {
                cn.Close();
                cn.Dispose();
            }
        }

        public string ExecuteNonQuery(CommandType cmdType, string cmdString, SqlParameter[] sqlParameters)
        {
            SqlConnection cn = new SqlConnection(this.ConnectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = cmdType;
            cmd.CommandText = cmdString;
            cmd.Parameters.AddRange(sqlParameters);
            cmd.Connection = cn;
            cn.Open();
            try
            {
                cmd.ExecuteNonQuery();
                return "success";
            }
            catch (Exception ex)
            {
                return "error \n" + ex.Message;
            }
            finally
            {
                cn.Close();
                cn.Dispose();
            }
        }

        public void Dispose()
        {
            ctx.Dispose();
        }
        #endregion
    }
}

