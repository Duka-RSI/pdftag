<%@ WebHandler Language="C#" Class="Passport" %>

using System;
using System.Web;
using System.IO;
using System.Data;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Web.SessionState;

public class Passport : IHttpHandler, IRequiresSessionState
{



    public void ProcessRequest(HttpContext context)
    {

        switch (context.Request["fun"])
        {
            case "get":
                get(context);
                break;

            case "saveByCol":
                saveByCol(context);
                break;
            case "get_org":
                get_org(context);
                break;
            case "upload_stylesketch":
                upload_stylesketch(context);
                break;
            case "delete_stylesketch":
                delete_stylesketch(context);
                break;

            case "delete":
                delete(context);
                break;
            case "copy":
                copy(context);
                break;
        }
    }
    public void delete(HttpContext context)
    {
        string luhid = context.Request["luhid"];

        string sSql = "";


        sSql = "delete  PDFTAG.dbo.UA_Header  where luhid=@luhid  ";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Execute(sSql, new { luhid = luhid });

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }
    public void copy(HttpContext context)
    {
        string luhid = context.Request["luhid"];

        string sSql = "";


        sSql = @"insert into PDFTAG.dbo.UA_Header 
(pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate,org_luhid) 
 select pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate, luhid as org_luhid
from PDFTAG.dbo.UA_Header where luhid=@luhid;";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Execute(sSql, new { luhid = luhid });

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }
    public void get(HttpContext context)
    {
        string pipid = context.Request["pipid"];
        string sSql = "";

        sSql = "select * \n";
        sSql += "from PDFTAG.dbo.P_inProcess  a             \n";
        sSql += "where 1=1 and isShow=0\n";
        sSql += " and a.pipid =@pipid \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query(sSql, new { pipid = pipid }).FirstOrDefault();

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }
    public void get_org(HttpContext context)
    {
        string luhid = context.Request["id"];
        string new_luhid = context.Request["newid"];
        string col = context.Request["col"];
        string sSql = "";

        sSql = "select " + col + " as text \n";
        sSql += "from PDFTAG.dbo.UA_Header  a             \n";
        sSql += "where 1=1 and isShow=0\n";
        sSql += " and a.luhid =@luhid \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query(sSql, new { luhid = luhid }).FirstOrDefault();
            var resNew = cn.Query(sSql, new { luhid = new_luhid }).FirstOrDefault();

            context.Response.Write(JsonConvert.SerializeObject(new { orgText = res.text, newText = resNew.text }));
        }
    }
    public void saveByCol(HttpContext context)
    {
        string luhid = context.Request["id"];
        string col = context.Request["col"];
        string text = context.Request["text"];
        string chNote = context.Request["chNote"];
        string sSql = "";

        sSql = "update PDFTAG.dbo.UA_Header \n";
        sSql += "set " + col + "=@text,isEdit=1              \n";
        sSql += "where luhid=@luhid\n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Execute(sSql, new { luhid = luhid, text = text });


            if (!string.IsNullOrEmpty(chNote))
            {
                sSql = @"IF NOT Exists (select * from PDFTAG.dbo.UA_Ch_Note where IdName=@IdName and id=@id and ColName=@ColName )
                             begin
                                  insert into PDFTAG.dbo.UA_Ch_Note
                                   (IdName,id,ColName,note,creator,createordate)
                                    values 
                              (@IdName,@id,@ColName,@note,@creator,@createordate)
                              end 
                             else 
                                begin
                                update PDFTAG.dbo.UA_Ch_Note
                                set note=@note
                             where IdName=@IdName and id=@id and ColName=@ColName
                             end ";

                var res2 = cn.Execute(sSql, new { IdName = "luhid", id = luhid, ColName = col, note = chNote, creator = LoginUser.PK, createordate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") });
            }

            context.Response.Write(JsonConvert.SerializeObject(new { data = res, learnmgrItem = "" }));
        }
    }

    public void delete_stylesketch(HttpContext context)
    {

        string luhid = context.Request["luhid"];


        string sSql = "";

        sSql = "update PDFTAG.dbo.UA_Header    \n";
        sSql += " set stylesketch=''     \n";
        sSql += " where luhid=@luhid    \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Execute(sSql, new { luhid = luhid, account = LoginUser.PK });
            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }

    public void upload_stylesketch(HttpContext context)
    {

        string luhid = context.Request["luhid"];
        string sDir2 = "upload/";
        string sSql = "";

        try
        {

            if (context.Request.Files.Count > 0)
            {
                string filepath = "";
                HttpPostedFile file = null;

                //for (int i = 0; i < context.Request.Files.Count; i++)
                //{
                //    file = context.Request.Files[i];
                //    if (file.ContentLength > 0)
                //    {
                //        var fileName = Path.GetFileName(file.FileName);
                //        filepath = sDir2 + fileName;
                //        file.SaveAs(context.Server.MapPath(filepath));
                //    }
                //}
                file = context.Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);
                filepath = sDir2 + fileName;
                file.SaveAs(context.Server.MapPath(filepath));

                sSql = @"update PDFTAG.dbo.UA_Header 
                                  set stylesketch=@stylesketch
                                  where luhid=@luhid   ";

                using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
                {
                    var res = cn.Execute(sSql, new { luhid = luhid, stylesketch = filepath });


                    sSql = @"select luhid,stylesketch from  PDFTAG.dbo.UA_Header
                                  where luhid=@luhid  ";

                    var res2 = cn.Query(sSql, new { luhid = luhid }).FirstOrDefault();

                    var result = new
                    {
                        success = true,
                        luhid = res2.luhid,
                        file = res2.stylesketch,
                        fileName = Path.GetFileName(res2.stylesketch),
                        filepath = filepath
                    };


                    context.Response.Write(JsonConvert.SerializeObject(result));
                }

            }


        }
        catch (Exception ex)
        {
            context.Response.Write(JsonConvert.SerializeObject(new { success = false, res = ex.ToString() }));
        }
    }



    public bool IsReusable
    {
        get
        {
            return false;
        }
    }



}

