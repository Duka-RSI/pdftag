using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// AdminSelectDao 2015/01 L.W pen
/// </summary>
public class AdminSelectDao
{
	public AdminSelectDao()
	{
	}

    //刪除，管理者權限分類資料
    public static int del(string Account)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "DELETE FROM Account_Select WHERE Account = @Account ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@Account", Account);
        return sql.ExecuteNonQuery();
    }

    //修改，管理者權限分類資料
    public static int update(AdminSelect obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "UPDATE Account_Select SET Account=@Account,Name=@Name,Password=@Password,Unit=@Unit,Asiid_list=@Asiid_list,Ad_role_Common=@Ad_role_Common,Ad_role_material=@Ad_role_material,Ad_role_items=@Ad_role_items,Ad_role_itemOrder=@Ad_role_itemOrder,Ad_role_forums=@Ad_role_forums,Ad_role_admin=@Ad_role_admin,Ad_role_student=@Ad_role_student,Ad_role_teachingDemon=@Ad_role_teachingDemon,Ad_role_teachingEval=@Ad_role_teachingEval,Ad_role_teacherSeminar=@Ad_role_teacherSeminar,Ad_role_soureTable=@Ad_role_soureTable,Test_role_exams=@Test_role_exams,Test_role_testExam=@Test_role_testExam,Self_role_proposal=@Self_role_proposal,Self_role_SeminarOrder=@Self_role_SeminarOrder,Self_role_OfficeHour=@Self_role_OfficeHour,Self_role_question=@Self_role_question,Self_role_CoachRecord=@Self_role_CoachRecord,Self_role_tplan=@Self_role_tplan,Practice_role_Unit=@Practice_role_Unit,Practice_role_Echelon=@Practice_role_Echelon,Practice_role_Group=@Practice_role_Group,Practice_role_PD=@Practice_role_PD,Practice_role_Provision=@Practice_role_Provision WHERE Account=@Account AND Password=@Password ";
        sql._Cmd = sqlCmd;

        sql._Params.AddWithValue("@Account", obj.Account);
        sql._Params.AddWithValue("@Name", obj.Name);
        sql._Params.AddWithValue("@Password", obj.Password);
        sql._Params.AddWithValue("@Unit", obj.Unit);
        sql._Params.AddWithValue("@Asiid_list", obj.Asiid_list);

        sql._Params.AddWithValue("@Ad_role_Common", obj.Ad_role_Common);
        sql._Params.AddWithValue("@Ad_role_material", obj.Ad_role_material);
        sql._Params.AddWithValue("@Ad_role_items", obj.Ad_role_items);
        sql._Params.AddWithValue("@Ad_role_itemOrder", obj.Ad_role_itemOrder);
        sql._Params.AddWithValue("@Ad_role_forums", obj.Ad_role_forums);
        sql._Params.AddWithValue("@Ad_role_admin", obj.Ad_role_admin);
        sql._Params.AddWithValue("@Ad_role_student", obj.Ad_role_student);
        sql._Params.AddWithValue("@Ad_role_teachingDemon", obj.Ad_role_teachingDemon);
        sql._Params.AddWithValue("@Ad_role_teachingEval", obj.Ad_role_teachingEval);
        sql._Params.AddWithValue("@Ad_role_teacherSeminar", obj.Ad_role_teacherSeminar);
        sql._Params.AddWithValue("@Ad_role_soureTable", obj.Ad_role_soureTable);

        sql._Params.AddWithValue("@Test_role_exams", obj.Test_role_exams);
        sql._Params.AddWithValue("@Test_role_testExam", obj.Test_role_testExam);

        sql._Params.AddWithValue("@Self_role_proposal", obj.Self_role_proposal);
        sql._Params.AddWithValue("@Self_role_SeminarOrder", obj.Self_role_SeminarOrder);
        sql._Params.AddWithValue("@Self_role_OfficeHour", obj.Self_role_OfficeHour);
        sql._Params.AddWithValue("@Self_role_question", obj.Self_role_question);
        sql._Params.AddWithValue("@Self_role_CoachRecord", obj.Self_role_CoachRecord);
        sql._Params.AddWithValue("@Self_role_tplan", obj.Self_role_tplan);

        sql._Params.AddWithValue("@Practice_role_Unit", obj.Practice_role_Unit);
        sql._Params.AddWithValue("@Practice_role_Echelon", obj.Practice_role_Echelon);
        sql._Params.AddWithValue("@Practice_role_Group", obj.Practice_role_Group);
        sql._Params.AddWithValue("@Practice_role_PD", obj.Practice_role_PD);
        sql._Params.AddWithValue("@Practice_role_Provision", obj.Practice_role_Provision);

        return sql.ExecuteNonQuery();
    }

    //讀取，管理者權限分類資料
    public static AdminSelect get(string Account, string Password)
    {
        AdminSelect obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Account_Select WHERE Account=@Account AND Password=@Password ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@Account", Account);
        sql._Params.AddWithValue("@Password", Password);

        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = new AdminSelect();
            DbUtil.DataRow2Object(dt.Rows[0], obj);
        }
        return obj;
    }

    //新增，管理者權限分類資料
    public static int add(AdminSelect obj)
    {
        SQLHelper sql = new SQLHelper();
        string sqlCmd =
            "INSERT INTO Account_Select(Account,Name,Password,Unit,Asiid_list,Ad_role_Common,Ad_role_material,Ad_role_items,Ad_role_itemOrder,Ad_role_forums,Ad_role_admin,Ad_role_student,Ad_role_teachingDemon,Ad_role_teachingEval,Ad_role_teacherSeminar,Ad_role_soureTable,Test_role_exams,Test_role_testExam,Self_role_proposal,Self_role_SeminarOrder,Self_role_OfficeHour,Self_role_question,Self_role_CoachRecord,Self_role_tplan,Practice_role_Unit,Practice_role_Echelon,Practice_role_Group,Practice_role_PD,Practice_role_Provision ) VALUES(@Account,@Name,@Password,@Unit,@Asiid_list,@Ad_role_Common,@Ad_role_material,@Ad_role_items,@Ad_role_itemOrder,@Ad_role_forums,@Ad_role_admin,@Ad_role_student,@Ad_role_teachingDemon,@Ad_role_teachingEval,@Ad_role_teacherSeminar,@Ad_role_soureTable,@Test_role_exams,@Test_role_testExam,@Self_role_proposal,@Self_role_SeminarOrder,@Self_role_OfficeHour,@Self_role_question,@Self_role_CoachRecord,@Self_role_tplan,@Practice_role_Unit,@Practice_role_Echelon,@Practice_role_Group,@Practice_role_PD,@Practice_role_Provision);  ";
        sql._Cmd = sqlCmd;

        sql._Params.AddWithValue("@Account", obj.Account);
        sql._Params.AddWithValue("@Name", obj.Name);
        sql._Params.AddWithValue("@Password", obj.Password);
        sql._Params.AddWithValue("@Unit", obj.Unit);
        sql._Params.AddWithValue("@Asiid_list", obj.Asiid_list);

        sql._Params.AddWithValue("@Ad_role_Common", obj.Ad_role_Common);
        sql._Params.AddWithValue("@Ad_role_material", obj.Ad_role_material);
        sql._Params.AddWithValue("@Ad_role_items", obj.Ad_role_items);
        sql._Params.AddWithValue("@Ad_role_itemOrder", obj.Ad_role_itemOrder);
        sql._Params.AddWithValue("@Ad_role_forums", obj.Ad_role_forums);
        sql._Params.AddWithValue("@Ad_role_admin", obj.Ad_role_admin);
        sql._Params.AddWithValue("@Ad_role_student", obj.Ad_role_student);
        sql._Params.AddWithValue("@Ad_role_teachingDemon", obj.Ad_role_teachingDemon);
        sql._Params.AddWithValue("@Ad_role_teachingEval", obj.Ad_role_teachingEval);
        sql._Params.AddWithValue("@Ad_role_teacherSeminar", obj.Ad_role_teacherSeminar);
        sql._Params.AddWithValue("@Ad_role_soureTable", obj.Ad_role_soureTable);

        sql._Params.AddWithValue("@Test_role_exams", obj.Test_role_exams);
        sql._Params.AddWithValue("@Test_role_testExam", obj.Test_role_testExam);

        sql._Params.AddWithValue("@Self_role_proposal", obj.Self_role_proposal);
        sql._Params.AddWithValue("@Self_role_SeminarOrder", obj.Self_role_SeminarOrder);
        sql._Params.AddWithValue("@Self_role_OfficeHour", obj.Self_role_OfficeHour);
        sql._Params.AddWithValue("@Self_role_question", obj.Self_role_question);
        sql._Params.AddWithValue("@Self_role_CoachRecord", obj.Self_role_CoachRecord);
        sql._Params.AddWithValue("@Self_role_tplan", obj.Self_role_tplan);

        sql._Params.AddWithValue("@Practice_role_Unit", obj.Practice_role_Unit);
        sql._Params.AddWithValue("@Practice_role_Echelon", obj.Practice_role_Echelon);
        sql._Params.AddWithValue("@Practice_role_Group", obj.Practice_role_Group);
        sql._Params.AddWithValue("@Practice_role_PD", obj.Practice_role_PD);
        sql._Params.AddWithValue("@Practice_role_Provision", obj.Practice_role_Provision);
        return sql.ExecuteNonQuery();
    }

    //讀取，管理者權限分類資料之列表
    public static List<AdminSelect> list()
    {
        List<AdminSelect> result = new List<AdminSelect>();
        AdminSelect obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + " FROM Account_Select ";
        sql._Cmd = sqlCmd;
        DataTable dt = sql.GetDataTable();
        foreach (DataRow row in dt.Rows)
        {
            obj = new AdminSelect();
            DbUtil.DataRow2Object(row, obj);
            result.Add(obj);
        }
        return result;
    }

    //登入時取得，管理者權限分類資料
    public static AdminSelect getAccount(string account, string password)
    {
        AdminSelect obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Account_Select WHERE Account=@account AND Password=@password  ";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@account", account);
        sql._Params.AddWithValue("@password", password);
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = new AdminSelect();
            DbUtil.DataRow2Object(dt.Rows[0], obj);
        }
        return obj;
    }

    //登入時取得，管理者權限分類資料
    public static AdminSelect getAccount(string account)
    {
        AdminSelect obj = null;
        SQLHelper sql = new SQLHelper();
        string sqlCmd = "SELECT * ";
        sqlCmd = sqlCmd + "FROM Account_Select WHERE Account=@account";
        sql._Cmd = sqlCmd;
        sql._Params.AddWithValue("@account", account);
        DataTable dt = sql.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            obj = new AdminSelect();
            DbUtil.DataRow2Object(dt.Rows[0], obj);
        }
        return obj;
    }
}

//管理者權限分類之Class
public class AdminSelect
{
    public AdminSelect()
    {
    }

    //使用者資料
    private string _Account;
    public string Account
    {
        get { return _Account; }
        set { _Account = value; }
    }
    private string _Password;
    public string Password
    {
        get { return _Password; }
        set { _Password = value; }
    }
    private string _Name;
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }

    private string _Unit;
    public string Unit
    {
        get { return _Unit; }
        set { _Unit = value; }
    }

    private string _Asiid_list;
    public string Asiid_list
    {
        get { return _Asiid_list; }
        set { _Asiid_list = value; }
    }

    private List<string> _AsiidList;
    public List<string> AsiidList
    {
        get { return _AsiidList; }
        set { _AsiidList = value; }
    }

    //行政管理事務
    private int _Ad_role_Common;
    public int Ad_role_Common
    {
        get { return _Ad_role_Common; }
        set { _Ad_role_Common = value; }
    }

    private int _Ad_role_material;
    public int Ad_role_material
    {
        get { return _Ad_role_material; }
        set { _Ad_role_material = value; }
    }
    private int _Ad_role_items;
    public int Ad_role_items
    {
        get { return _Ad_role_items; }
        set { _Ad_role_items = value; }
    }
    private int _Ad_role_itemOrder;
    public int Ad_role_itemOrder
    {
        get { return _Ad_role_itemOrder; }
        set { _Ad_role_itemOrder = value; }
    }
    private int _Ad_role_forums;
    public int Ad_role_forums
    {
        get { return _Ad_role_forums; }
        set { _Ad_role_forums = value; }
    }
    private int _Ad_role_admin;
    public int Ad_role_admin
    {
        get { return _Ad_role_admin; }
        set { _Ad_role_admin = value; }
    }
    private int _Ad_role_student;
    public int Ad_role_student
    {
        get { return _Ad_role_student; }
        set { _Ad_role_student = value; }
    }
    private int _Ad_role_teachingDemon;
    public int Ad_role_teachingDemon
    {
        get { return _Ad_role_teachingDemon; }
        set { _Ad_role_teachingDemon = value; }
    }
    private int _Ad_role_teachingEval;
    public int Ad_role_teachingEval
    {
        get { return _Ad_role_teachingEval; }
        set { _Ad_role_teachingEval = value; }
    }
    private int _Ad_role_teacherSeminar;
    public int Ad_role_teacherSeminar
    {
        get { return _Ad_role_teacherSeminar; }
        set { _Ad_role_teacherSeminar = value; }
    }
    private int _Ad_role_soureTable;
    public int Ad_role_soureTable
    {
        get { return _Ad_role_soureTable; }
        set { _Ad_role_soureTable = value; }
    }

    //大會考事務
    private int _Test_role_exams;
    public int Test_role_exams
    {
        get { return _Test_role_exams; }
        set { _Test_role_exams = value; }
    }

    private int _Test_role_testExam;
    public int Test_role_testExam
    {
        get { return _Test_role_testExam; }
        set { _Test_role_testExam = value; }
    }

    //個人事務
    private int _Self_role_proposal;
    public int Self_role_proposal
    {
        get { return _Self_role_proposal; }
        set { _Self_role_proposal = value; }
    }
    private int _Self_role_SeminarOrder;
    public int Self_role_SeminarOrder
    {
        get { return _Self_role_SeminarOrder; }
        set { _Self_role_SeminarOrder = value; }
    }
    private int _Self_role_OfficeHour;
    public int Self_role_OfficeHour
    {
        get { return _Self_role_OfficeHour; }
        set { _Self_role_OfficeHour = value; }
    }
    private int _Self_role_question;
    public int Self_role_question
    {
        get { return _Self_role_question; }
        set { _Self_role_question = value; }
    }
    private int _Self_role_CoachRecord;
    public int Self_role_CoachRecord
    {
        get { return _Self_role_CoachRecord; }
        set { _Self_role_CoachRecord = value; }
    }
    private int _Self_role_tplan;
    public int Self_role_tplan
    {
        get { return _Self_role_tplan; }
        set { _Self_role_tplan = value; }
    }

    //實習事務
    private int _Practice_role_Unit;
    public int Practice_role_Unit
    {
        get { return _Practice_role_Unit; }
        set { _Practice_role_Unit = value; }
    }

    private int _Practice_role_Echelon;
    public int Practice_role_Echelon
    {
        get { return _Practice_role_Echelon; }
        set { _Practice_role_Echelon = value; }
    }

    private int _Practice_role_Group;
    public int Practice_role_Group
    {
        get { return _Practice_role_Group; }
        set { _Practice_role_Group = value; }
    }

    private int _Practice_role_PD;
    public int Practice_role_PD
    {
        get { return _Practice_role_PD; }
        set { _Practice_role_PD = value; }
    }

    private int _Practice_role_Provision;
    public int Practice_role_Provision
    {
        get { return _Practice_role_Provision; }
        set { _Practice_role_Provision = value; }
    }

}