using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// TplanDao 的摘要描述
/// </summary>
public class TplanDao
{
    public TplanDao()
	{
	}

    public static Tplan get(long id)
    {
        Tplan obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Tplan WHERE id = @id   ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@id", id);
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = new Tplan();
            DbUtil.DataRow2Object(dt.Rows[0], obj);
        }
        return obj;
    }

    public static int del(long id)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "DELETE FROM Tplan WHERE id = @id  ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@id", id);
        return sql.ExecuteNonQuery();
    }

    public static int update(Tplan obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "UPDATE Tplan SET eduSystem=@eduSystem ,departmentId=@departmentId ,class1=@class1 ,prerequisiteKnowledge=@prerequisiteKnowledge  ";
        sqlCmd = sqlCmd + ",goal=@goal ,objectives=@objectives ,scenario=@scenario ,scenarioStem=@scenarioStem ,roles=@roles ,objectivesKAS=@objectivesKAS  ";
        sqlCmd = sqlCmd + ",objectivesEvent1=@objectivesEvent1 ,objectivesEvent2=@objectivesEvent2 ,objectivesResponse3=@objectivesResponse3 ,objectivesEvent4=@objectivesEvent4 ,objectivesResponse5=@objectivesResponse5 ";
        sqlCmd = sqlCmd + ",roomDressUp=@roomDressUp ,things=@things ,rolesDressUp=@rolesDressUp ,initialState=@initialState ,dressUp=@dressUp ,account=@account ,audit=@audit ,isPub=@isPub ,name=@name ,type=@type ,year=@year,subjectid=@subjectid,semester=@semester ";
        sqlCmd = sqlCmd + "WHERE id = @id ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@eduSystem", obj.eduSystem);
        sql._Params.AddWithValue("@departmentId", obj.departmentId);
        sql._Params.AddWithValue("@class1", obj.class1);
        sql._Params.AddWithValue("@prerequisiteKnowledge", obj.prerequisiteKnowledge);
        sql._Params.AddWithValue("@goal", obj.goal);
        sql._Params.AddWithValue("@objectives", obj.objectives);
        sql._Params.AddWithValue("@scenario", obj.scenario);
        sql._Params.AddWithValue("@scenarioStem", obj.scenarioStem);
        sql._Params.AddWithValue("@roles", obj.roles);
        sql._Params.AddWithValue("@objectivesKAS", obj.objectivesKAS);
        sql._Params.AddWithValue("@objectivesEvent1", obj.objectivesEvent1);
        sql._Params.AddWithValue("@objectivesResponse1", obj.objectivesResponse1);
        sql._Params.AddWithValue("@objectivesEvent2", obj.objectivesEvent2);
        sql._Params.AddWithValue("@objectivesResponse2", obj.objectivesResponse2);
        sql._Params.AddWithValue("@objectivesEvent3", obj.objectivesEvent3);
        sql._Params.AddWithValue("@objectivesResponse3", obj.objectivesResponse3);
        sql._Params.AddWithValue("@objectivesEvent4", obj.objectivesEvent4);
        sql._Params.AddWithValue("@objectivesResponse4", obj.objectivesResponse4);
        sql._Params.AddWithValue("@objectivesEvent5", obj.objectivesEvent5);
        sql._Params.AddWithValue("@objectivesResponse5", obj.objectivesResponse5);
        sql._Params.AddWithValue("@roomDressUp", obj.roomDressUp);
        sql._Params.AddWithValue("@things", obj.things);
        sql._Params.AddWithValue("@rolesDressUp", obj.rolesDressUp);
        sql._Params.AddWithValue("@initialState", obj.initialState);
        sql._Params.AddWithValue("@dressUp", obj.dressUp);
        sql._Params.AddWithValue("@account", obj.account);
        sql._Params.AddWithValue("@audit", obj.audit);
        sql._Params.AddWithValue("@isPub", obj.isPub);
        sql._Params.AddWithValue("@name", obj.name);
        sql._Params.AddWithValue("@type", obj.type);
        sql._Params.AddWithValue("@year", obj.year);
        sql._Params.AddWithValue("@id", obj.id);
        sql._Params.AddWithValue("@subjectid", obj.subjectid);
        sql._Params.AddWithValue("@semester", obj.semester);
        sql._Params.AddWithValue("@runyear", obj.runyear);
        return sql.ExecuteNonQuery();
    }

    public static long add(Tplan obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "INSERT INTO Tplan(eduSystem, departmentId, class1, prerequisiteKnowledge, goal, objectives, scenario, scenarioStem, roles, objectivesKAS, objectivesEvent1, objectivesResponse1, objectivesEvent2, objectivesResponse2, objectivesEvent3, objectivesResponse3, objectivesEvent4, objectivesResponse4, objectivesEvent5, objectivesResponse5, roomDressUp, things, rolesDressUp, initialState, dressUp, account, isPub, name, type, year,subjectid,semester) VALUES(@eduSystem, @departmentId, @class1, @prerequisiteKnowledge, @goal, @objectives, @scenario, @scenarioStem, @roles, @objectivesKAS, @objectivesEvent1, @objectivesResponse1, @objectivesEvent2, @objectivesResponse2, @objectivesEvent3, @objectivesResponse3, @objectivesEvent4, @objectivesResponse4, @objectivesEvent5, @objectivesResponse5, @roomDressUp, @things, @rolesDressUp, @initialState, @dressUp, @account, @isPub, @name, @type, @year,@subjectid,@semester); SELECT @@IDENTITY AS 'Identity' ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@eduSystem", obj.eduSystem);
        sql._Params.AddWithValue("@departmentId", obj.departmentId);
        sql._Params.AddWithValue("@class1", obj.class1);
        sql._Params.AddWithValue("@prerequisiteKnowledge", obj.prerequisiteKnowledge);
        sql._Params.AddWithValue("@goal", obj.goal);
        sql._Params.AddWithValue("@objectives", obj.objectives);
        sql._Params.AddWithValue("@scenario", obj.scenario);
        sql._Params.AddWithValue("@scenarioStem", obj.scenarioStem);
        sql._Params.AddWithValue("@roles", obj.roles);
        sql._Params.AddWithValue("@objectivesKAS", obj.objectivesKAS);
        sql._Params.AddWithValue("@objectivesEvent1", obj.objectivesEvent1);
        sql._Params.AddWithValue("@objectivesResponse1", obj.objectivesResponse1);
        sql._Params.AddWithValue("@objectivesEvent2", obj.objectivesEvent2);
        sql._Params.AddWithValue("@objectivesResponse2", obj.objectivesResponse2);
        sql._Params.AddWithValue("@objectivesEvent3", obj.objectivesEvent3);
        sql._Params.AddWithValue("@objectivesResponse3", obj.objectivesResponse3);
        sql._Params.AddWithValue("@objectivesEvent4", obj.objectivesEvent4);
        sql._Params.AddWithValue("@objectivesResponse4", obj.objectivesResponse4);
        sql._Params.AddWithValue("@objectivesEvent5", obj.objectivesEvent5);
        sql._Params.AddWithValue("@objectivesResponse5", obj.objectivesResponse5);
        sql._Params.AddWithValue("@roomDressUp", obj.roomDressUp);
        sql._Params.AddWithValue("@things", obj.things);
        sql._Params.AddWithValue("@rolesDressUp", obj.rolesDressUp);
        sql._Params.AddWithValue("@initialState", obj.initialState);
        sql._Params.AddWithValue("@dressUp", obj.dressUp);
        sql._Params.AddWithValue("@isPub", obj.isPub);
        sql._Params.AddWithValue("@name", obj.name);
        sql._Params.AddWithValue("@type", obj.type);
        sql._Params.AddWithValue("@year", obj.year);
        sql._Params.AddWithValue("@account", obj.account);
        sql._Params.AddWithValue("@subjectid", obj.subjectid);
        sql._Params.AddWithValue("@semester", obj.semester);
        sql._Params.AddWithValue("@runyear", obj.runyear);
        
        long id = 0;
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            id = Convert.ToInt64(dt.Rows[0][0]);
            obj.id = id;
        }
        return id;
    }

    public static List<Tplan> list(string account,int audit,int isPub)
    {
        List<Tplan> result = new List<Tplan>();
        Tplan obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT Tplan.*, Account.Name as teacherName ";
        sqlCmd = sqlCmd + "FROM Tplan LEFT JOIN Account ON Tplan.account= Account.Account ";
        sqlCmd = sqlCmd + "WHERE (@account='' OR Tplan.account=@account)  AND (@audit=-1 OR Tplan.audit=@audit) AND (@isPub=-1 OR Tplan.isPub=@isPub) ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@account", (account==null?"":account));
        sql._Params.AddWithValue("@audit", audit);
        sql._Params.AddWithValue("@isPub", isPub);
        DataTable dt = sql.GetDataTable();
        foreach (DataRow row in dt.Rows)
        {
            obj = new Tplan();
            DbUtil.DataRow2Object(row, obj);
            result.Add(obj);
        }
        return result;
    }

}

public class Tplan
{
    public Tplan()
    {
    }
    public long id
    {
        get;
        set;
    }
    public long eduSystem
    {
        get;
        set;
    }
    public long departmentId
    {
        get;
        set;
    }
    public int class1
    {
        get;
        set;
    }
    public string prerequisiteKnowledge
    {
        get;
        set;
    }
    public string goal
    {
        get;
        set;
    }
    public string objectives
    {
        get;
        set;
    }
    public string scenario
    {
        get;
        set;
    }

    public string scenarioStem
    {
        get;
        set;
    }

    public string roles
    {
        get;
        set;
    }

    public string objectivesKAS
    {
        get;
        set;
    }

    public string objectivesEvent1
    {
        get;
        set;
    }

    public string objectivesResponse1
    {
        get;
        set;
    }

    public string objectivesEvent2
    {
        get;
        set;
    }

    public string objectivesResponse2
    {
        get;
        set;
    }

    public string objectivesEvent3
    {
        get;
        set;
    }

    public string objectivesResponse3
    {
        get;
        set;
    }

    public string objectivesEvent4
    {
        get;
        set;
    }

    public string objectivesResponse4
    {
        get;
        set;
    }

    public string objectivesEvent5
    {
        get;
        set;
    }

    public string objectivesResponse5
    {
        get;
        set;
    }


    public string roomDressUp
    {
        get;
        set;
    }

    public string things
    {
        get;
        set;
    }

    public string rolesDressUp
    {
        get;
        set;
    }

    public long dressUp
    {
        get;
        set;
    }

    public string initialState
    {
        get;
        set;
    }

    public string account
    {
        get;
        set;
    }

    public int audit
    {
        get;
        set;
    }

    public int isPub
    {
        get;
        set;
    }

    public string teacherName
    {
        get;
        set;
    }

    public string name
    {
        get;
        set;
    }

    public string type
    {
        get;
        set;
    }

    public string year
    {
        get;
        set;
    }
    public int subjectid { get; set; }
    public bool semester { get; set; }
    public string runyear { get; set; }
    public string eduSystemName { get; set; }
    public string departmentName { get; set; }
}



