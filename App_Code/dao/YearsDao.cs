using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class YearsDao
{
    private static int _startSchoolYear = 104;

    public YearsDao()
    {
    }

    public static Years getYears(long id)
    {
        Years obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Years WHERE id = @id  ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@id", id);
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = new Years();
            DbUtil.DataRow2Object(dt.Rows[0], obj);
        }
        return obj;
    }

    public static int delYears(long id)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "DELETE FROM Years WHERE id = @id ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@id", id);
        return sql.ExecuteNonQuery();
    }

    public static int updateYears(Years obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "UPDATE Years SET years=@years WHERE id = @id ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@years", obj.years);
        sql._Params.AddWithValue("@id", obj.id);
        return sql.ExecuteNonQuery();
    }

    public static int addYears(Years obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "INSERT INTO Years(years) VALUES(@years) ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@years", obj.years);
        return sql.ExecuteNonQuery();
    }

    public static List<Years> listYears()
    {
        List<Years> result = new List<Years>();
        Years obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Years ";
        sqlCmd = sqlCmd + "Order BY id";
        sql._Cmd = sqlCmd;
        DataTable dt = sql.GetDataTable();
        foreach (DataRow row in dt.Rows)
        {
            obj = new Years();
            DbUtil.DataRow2Object(row, obj);
            result.Add(obj);
        }
        return result;
    }

    private static List<SchoolYear> _schoolYears = null;

    public static List<SchoolYear> SchoolYears
    {
        get { return _schoolYears ?? (_schoolYears = ListSchoolYears()); }
    }

    public static List<SchoolYear> ListSchoolYears()
    {
        var result = new List<SchoolYear>();

        var sql = new SQLHelper();
        const string sqlCmd = @"
SELECT
    Id,
    Name,
    [Year]
FROM SchoolYears
ORDER BY Sort DESC, Name DESC
";
        sql._Cmd = sqlCmd;
        var dt = sql.GetDataTable();
        foreach (DataRow row in dt.Rows)
        {
            var obj = new SchoolYear();

            DbUtil.DataRow2Object(row, obj);

            result.Add(obj);
        }
        return result;
    }

}

public class Years
{
    public Years()
    {
    }
    public long id
    {
        get;
        set;
    }
    public string years
    {
        get;
        set;
    }
}

public class SchoolYear
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Year { get; set; }
}