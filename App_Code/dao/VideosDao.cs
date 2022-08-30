using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class VideosDao
{
    public VideosDao()
	{
	}
    
    public static int add(Video obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "INSERT INTO Videos(orderId,filename) VALUES(@orderId,@filename); ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@orderId", obj.orderId);
        sql._Params.AddWithValue("@filename", obj.filename);
        return sql.ExecuteNonQuery();
    }

    public static List<Video> list(long orderId) {
        List<Video> result = new List<Video>();
        Video obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Videos WHERE orderId=@orderId ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@orderId", orderId);
        DataTable dt = sql.GetDataTable();
        foreach (DataRow row in dt.Rows) {
            obj = new Video();
            DbUtil.DataRow2Object(row, obj);
            result.Add(obj);
        }
        return result;
    }

}


public class Video
{
    public Video()
    {
    }

    public long orderId
    {
        get;
        set;
    }

    public string filename
    {
        get;
        set;
    }
}
