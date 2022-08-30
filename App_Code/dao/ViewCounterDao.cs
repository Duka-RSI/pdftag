using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class ViewCounterDao
{
    public ViewCounterDao()
	{
	}

    public static long getCountAll()
    {
        long obj = 0;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT count(*) as c ";
        sqlCmd = sqlCmd + "FROM ViewCounters ";
        sql._Cmd = sqlCmd;
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = Convert.ToInt64(dt.Rows[0]["c"]);
        }
        return obj;
    }

    public static long getCountToday()
    {
        long obj = 0;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT count(*) as c ";
        sqlCmd = sqlCmd + "FROM ViewCounters WHERE [LogTime]>=@start AND [LogTime]<@end ";
        sql._Cmd = sqlCmd;

        DateTime startDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
        DateTime endDate = startDate.AddDays(1);
        sql._Params.AddWithValue("@start", startDate);
        sql._Params.AddWithValue("@end", endDate);
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = Convert.ToInt64(dt.Rows[0]["c"]);
        }
        return obj;
    }

    public static int addViewCounte(string fromIp)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "INSERT INTO ViewCounters(FromIp,LogTime) VALUES(@FromIp,@LogTime) ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@FromIp", fromIp);
        sql._Params.AddWithValue("@LogTime", DateTime.Now);
        return sql.ExecuteNonQuery();
    }
}
