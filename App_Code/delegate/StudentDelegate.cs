using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class StudentDelegate
{
    public StudentDelegate()
    {
    }

    public static int upgradeStudentYear()
    {
        string sqlCmd1 = "UPDATE StudentInfo SET class1=class1+1 WHERE eduSystem=@eduSystem AND class1<=@class1  ";
        string sqlCmd2 = "UPDATE Account SET StRole=0,AlumnusRole=1 WHERE Account IN (SELECT studentAccount FROM StudentInfo WHERE eduSystem=@eduSystem AND class1>@class1 )  ";


        SQLHelper sql = null;

        sql = new SQLHelper();
        sql._Cmd = sqlCmd1;
        sql._Params.AddWithValue("@eduSystem", "二技進修");
        sql._Params.AddWithValue("@class1", 2);
        sql.ExecuteNonQuery();

        sql = new SQLHelper();
        sql._Cmd = sqlCmd2;
        sql._Params.AddWithValue("@eduSystem", "二技進修");
        sql._Params.AddWithValue("@class1", 2);
        sql.ExecuteNonQuery();


        sql = new SQLHelper();
        sql._Cmd = sqlCmd1;
        sql._Params.AddWithValue("@eduSystem", "二技");
        sql._Params.AddWithValue("@class1", 2);
        sql.ExecuteNonQuery();

        sql = new SQLHelper();
        sql._Cmd = sqlCmd2;
        sql._Params.AddWithValue("@eduSystem", "二技");
        sql._Params.AddWithValue("@class1", 2);
        sql.ExecuteNonQuery();

        sql = new SQLHelper();
        sql._Cmd = sqlCmd1;
        sql._Params.AddWithValue("@eduSystem", "四技");
        sql._Params.AddWithValue("@class1", 4);
        sql.ExecuteNonQuery();

        sql = new SQLHelper();
        sql._Cmd = sqlCmd2;
        sql._Params.AddWithValue("@eduSystem", "四技");
        sql._Params.AddWithValue("@class1", 4);
        sql.ExecuteNonQuery();


        sql = new SQLHelper();
        sql._Cmd = sqlCmd1;
        sql._Params.AddWithValue("@eduSystem", "五專");
        sql._Params.AddWithValue("@class1", 5);
        sql.ExecuteNonQuery();

        sql = new SQLHelper();
        sql._Cmd = sqlCmd2;
        sql._Params.AddWithValue("@eduSystem", "五專");
        sql._Params.AddWithValue("@class1", 5);
        sql.ExecuteNonQuery();



        return 0;
    }


}

