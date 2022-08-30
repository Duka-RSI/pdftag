using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class UnitReporterInfoDao
{
    public UnitReporterInfoDao()
    {
    }

    public static UnitReporterInfo getUnitReporterInfo(string account)
    {
        UnitReporterInfo obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM UnitReporterInfo ";
        sqlCmd = sqlCmd + "INNER JOIN Department ON  UnitReporterInfo.departmentId=Department.id ";
        sqlCmd = sqlCmd + "WHERE UnitReporterInfo.account = @account  ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@account", account);
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = new UnitReporterInfo();
            DbUtil.DataRow2Object(dt.Rows[0], obj);
        }
        return obj;
    }

    public static int delUnitReporterInfo(string account)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "DELETE FROM UnitReporterInfo WHERE account = @account ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@account", account);
        return sql.ExecuteNonQuery();
    }

    public static int updateUnitReporterInfo(UnitReporterInfo obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "UPDATE UnitReporterInfo SET departmentId=@departmentId,lv=@lv,name=@name,tel=@tel,mail=@mail WHERE account = @account ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@account", obj.account);
        sql._Params.AddWithValue("@departmentId", obj.departmentId);
        sql._Params.AddWithValue("@lv", obj.lv);
        sql._Params.AddWithValue("@name", obj.name);
        sql._Params.AddWithValue("@tel", obj.tel);
        sql._Params.AddWithValue("@mail", obj.mail);
        return sql.ExecuteNonQuery();
    }

    public static int addUnitReporterInfo(UnitReporterInfo obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "INSERT INTO UnitReporterInfo(departmentId,lv , account,name,tel,mail ) VALUES(@departmentId,@lv,@account,@name,@tel,@mail) ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@account", obj.account);
        sql._Params.AddWithValue("@departmentId", obj.departmentId);
        sql._Params.AddWithValue("@lv", obj.lv);
        sql._Params.AddWithValue("@name", obj.name);
        sql._Params.AddWithValue("@tel", obj.tel);
        sql._Params.AddWithValue("@mail", obj.mail);
        return sql.ExecuteNonQuery();
    }


    /// <summary>
    /// 列出學生資料
    /// <param name="keyword">輸入空白列出所有資料，不是空白則列出帳號或姓名含輸入文字的學生資料</param>
    /// </summary>
    public static List<UnitReporterInfo> listUnitReporterInfo(string keyword, long depatmentId)
    {
        List<UnitReporterInfo> result = new List<UnitReporterInfo>();
        UnitReporterInfo obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT UnitReporterInfo.*,Department.departmentName ";
        sqlCmd = sqlCmd + "FROM UnitReporterInfo  ";
        sqlCmd = sqlCmd + "INNER JOIN Department ON  UnitReporterInfo.departmentId=Department.id ";
        sqlCmd = sqlCmd + "WHERE (@depatmentId=0 OR @depatmentId=UnitReporterInfo.departmentId) AND ((account like @keyword) OR (name like @keyword) ) ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@keyword", "%" + keyword + "%");
        sql._Params.AddWithValue("@depatmentId", depatmentId);
        DataTable dt = sql.GetDataTable();
        foreach (DataRow row in dt.Rows)
        {
            obj = new UnitReporterInfo();
            DbUtil.DataRow2Object(row, obj);
            result.Add(obj);
        }
        return result;
    }

}

public class UnitReporterInfo
{
    public UnitReporterInfo()
    {
    }
    public string account
    {
        get;
        set;
    }
    public long departmentId
    {
        get;
        set;
    }
    public string departmentName
    {
        get;
        set;
    }
    public string name
    {
        get;
        set;
    }
    public string lv
    {
        get;
        set;
    }
    public string tel
    {
        get;
        set;
    }
    public string mail
    {
        get;
        set;
    }
}