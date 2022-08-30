using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class SubjectDao
{
    public SubjectDao()
	{
	}

    public static Subject getSubject(long id)
    {
        Subject obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Subject WHERE id = @id  ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@id", id);
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = new Subject();
            DbUtil.DataRow2Object(dt.Rows[0], obj);
        }
        return obj;
    }

    public static int delSubject(long id)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "DELETE FROM Subject WHERE id = @id ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@id", id);
        return sql.ExecuteNonQuery();
    }

    public static int updateSubject(Subject obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "UPDATE Subject SET subjectName=@subjectName WHERE id = @id ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@subjectName", obj.subjectName);
        sql._Params.AddWithValue("@id", obj.id);
        return sql.ExecuteNonQuery();
    }

    public static int addSubject(Subject obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "INSERT INTO Subject(subjectName) VALUES(@subjectName) ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@subjectName", obj.subjectName);
        return sql.ExecuteNonQuery();
    }

    public static List<Subject> listSubject()
    {
        List<Subject> result = new List<Subject>();
        Subject obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Subject ";
        sqlCmd = sqlCmd + "Order BY id";
        sql._Cmd = sqlCmd;
        DataTable dt = sql.GetDataTable();
        foreach (DataRow row in dt.Rows)
        {
            obj = new Subject();
            DbUtil.DataRow2Object(row, obj);
            result.Add(obj);
        }
        return result;
    }

}

public class Subject
{
    public Subject()
    {
    }
    private long _id;
    public long id
    {
        get { return _id; }
        set { _id = value; }
    }
    private string _subjectName;
    public string subjectName
    {
        get { return _subjectName; }
        set { _subjectName = value; }
    }
}