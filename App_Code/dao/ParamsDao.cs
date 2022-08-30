using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class ParamsDao
{
    public ParamsDao()
	{
	}

    public static String getParam(string param)
    {
        String obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Params WHERE param = @param  ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@param", param);
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = Convert.ToString(dt.Rows[0]["val"]);
        }
        return obj;
    }

    public static int addParam(string param, string val)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "INSERT INTO Params(val,param) VALUES(@val,@param) ";
        if (getParam(param) != null)
        {
            sqlCmd = "UPDATE Params SET val=@val WHERE param=@param ";
        }
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@param", param);
        sql._Params.AddWithValue("@val", val);
        return sql.ExecuteNonQuery();
    }
}
