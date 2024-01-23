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
            case "get_org":
                get_org(context);
                break;
            case "get_LearnmgrItem":
                get_LearnmgrItem(context);
                break;
            case "saveByCol":
                saveByCol(context);
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
        string lubid = context.Request["lubid"];

        string sSql = "";


        sSql = "delete  PDFTAG.dbo.GAP_BOM  where lubid=@lubid  ";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Execute(sSql, new { lubid = lubid });

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }
    public void copy(HttpContext context)
    {
        string lubid = context.Request["lubid"];

        string sSql = "";


        sSql = @"insert into PDFTAG.dbo.GAP_BOM 
(pipid,luhid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,QualityDetails,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,org_lubid,lubcid,isEdit) 
 select pipid,luhid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,QualityDetails,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,org_lubid,lubcid, 0 as isEdit
  from PDFTAG.dbo.GAP_BOM where lubid=@lubid";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Execute(sSql, new { lubid = lubid });

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }
    public void get(HttpContext context)
    {
        string pipid = context.Request["id"];
        string col = context.Request["col"];
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
        string lubid = context.Request["id"];
        string new_lubid = context.Request["newid"];
        string col = context.Request["col"];
        string sSql = "";

        sSql = "select " + col + " as text \n";
        sSql += "from PDFTAG.dbo.GAP_BOM  a             \n";
        sSql += "where 1=1 \n";
        sSql += " and a.lubid =@lubid \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query(sSql, new { lubid = lubid }).FirstOrDefault();
            var resNew = cn.Query(sSql, new { lubid = new_lubid }).FirstOrDefault();


            string orgText = (res == null ? "" : res.text);
            string newText = (resNew == null ? "" : resNew.text);

            //double d = 0;
            //if (orgText.Contains("/") && Double.TryParse(orgText.Split('/')[1], out d)) orgText = orgText.Replace(" ", "-");
            //if (newText.Contains("/") && Double.TryParse(newText.Split('/')[1], out d)) newText = newText.Replace(" ", "-");

            context.Response.Write(JsonConvert.SerializeObject(new { orgText = orgText, newText = newText }));
        }
    }
    public class Text
    {
        public string TextOrg { get; set; }
        public string SupplierArticle { get; set; }
        public string usage { get; set; }

    }

    public void get_LearnmgrItem(HttpContext context)
    {
        string lubid_org = context.Request["orgId"];
        string col = context.Request["col"];
        string style = context.Request["style"];
        string learnmgrItem = context.Request["learnmgrItem"];

        string sSql = "";


        sSql = "select " + col + " as TextOrg  from  PDFTAG.dbo.GAP_BOM where lubid=@lubid";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var resLu_BOM_Org = cn.Query<Text>(sSql, new { lubid = lubid_org }).FirstOrDefault();

            string sTermname_org = resLu_BOM_Org.TextOrg.Trim().Replace(" ", "").ToLower();

            if (PublicFunction.arrGapBomColors.Contains(col))
                col = "GarmentColor";


            sSql = "select distinct " + learnmgrItem + " from PDFTAG.dbo.GAP_LearnmgrItem where ColSource=@ColSource and ColName=@ColName and termname_org=@termname_org  \n";
            var list = cn.Query(sSql, new
            {
                ColSource = "BOM",
                ColName = col,
                termname_org = sTermname_org,
            }).ToList();

            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }

    public void saveByCol(HttpContext context)
    {
        string lubid_org = context.Request["orgId"];
        string lubid = context.Request["id"];
        string col = context.Request["col"];
        string colorCol = context.Request["colorCol"];
        string text = context.Request["text"];
        string chNote = context.Request["chNote"];
        string style = context.Request["style"];

        string PARTS_TYPE = context.Request["PARTS_TYPE"];
        string PARTS_CODE = context.Request["PARTS_CODE"];
        string PARTS_DESC = context.Request["PARTS_DESC"];
        string MAT_ID = context.Request["MAT_ID"];
        string isRecord = context.Request["isRecord"];

        DateTime dtNow = DateTime.Now;

        string sSql = "";


        sSql = "select " + col + " as TextOrg,SupplierArticle,usage  from  PDFTAG.dbo.GAP_BOM where lubid=@lubid";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var resLu_BOM_Org = cn.Query<Text>(sSql, new { lubid = lubid_org }).FirstOrDefault();

            sSql = "update PDFTAG.dbo.GAP_BOM \n";
            sSql += "set " + col + "=@text,isEdit=1             \n";

            if (col == "SupplierArticle")
            {
                sSql += ",PARTS_TYPE=@PARTS_TYPE,PARTS_CODE=@PARTS_CODE,PARTS_DESC=@PARTS_DESC,MAT_ID=@MAT_ID             \n";
            }

            sSql += "where lubid=@lubid\n";

            var res = cn.Execute(sSql, new { lubid = lubid, text = text, PARTS_TYPE = PARTS_TYPE, PARTS_CODE = PARTS_CODE, PARTS_DESC = PARTS_DESC, MAT_ID = MAT_ID });

            if (!string.IsNullOrEmpty(chNote))
            {
                sSql = @"IF NOT Exists (select * from PDFTAG.dbo.GAP_Ch_Note where IdName=@IdName and id=@id and ColName=@ColName )
                             begin
                                  insert into PDFTAG.dbo.GAP_Ch_Note
                                   (IdName,id,ColName,note,creator,createordate)
                                    values 
                              (@IdName,@id,@ColName,@note,@creator,@createordate)
                              end 
                             else 
                                begin
                                update PDFTAG.dbo.GAP_Ch_Note
                                set note=@note
                             where IdName=@IdName and id=@id and ColName=@ColName
                             end ";

                var res2 = cn.Execute(sSql, new { IdName = "lubid", id = lubid, ColName = col, note = chNote, creator = LoginUser.PK, createordate = dtNow.ToString("yyyy/MM/dd HH:mm:ss") });
            }

            int iCntLearnmgrItem = 0;

            if (isRecord == "1")
            {
                string sFirstCharTermname_org = resLu_BOM_Org.TextOrg.Substring(0, 1);
                string sTermname_org = resLu_BOM_Org.TextOrg.Trim().Replace(" ", "").ToLower();
                string sSupplierArticleNumber = "";
                string sUsage = "";
                    string sColorname = "";

                sSql = @" insert into PDFTAG.dbo.GAP_LearnmgrItem
                                   (ColSource,ColName,FirstCharTermname_org,termname_org,termname,style,SupplierArticleNumber,usage,Colorname,creator,creatordate)
                                    values 
                              (@ColSource,@ColName,@FirstCharTermname_org,@termname_org,@termname,@style,@SupplierArticleNumber,@usage,@Colorname,@creator,@creatordate)";

                if (col == "B1" || col == "B2" || col == "B3" || col == "B4" || col == "B5" || col == "B6" || col == "B7" || col == "B8" || col == "B9" || col == "B10")
                {
                    //col = colorCol;
                    //20220803 不會針對0002-WHT做判斷，只會針對White的內容做取代，並顯示 修: PreWhite。trm 也有一個 0002-WHT。點[學習]後，不會把 DTM 變成 PreWhite
                    col = "GarmentColor";
                    sSupplierArticleNumber = resLu_BOM_Org.SupplierArticle;
                    sUsage = resLu_BOM_Org.usage;
                         sColorname = colorCol;
                }


                iCntLearnmgrItem = cn.Execute(sSql, new
                {
                    ColSource = "BOM",
                    ColName = col,
                    FirstCharTermname_org = sFirstCharTermname_org,
                    termname_org = sTermname_org,
                    termname = text,
                    style = style,
                    SupplierArticleNumber = sSupplierArticleNumber,
                    usage = sUsage,
                    Colorname = sColorname,
                    creator = LoginUser.PK,
                    creatordate = dtNow.ToString("yyyy/MM/dd HH:mm:ss"),
                    UpdateUser = LoginUser.PK,
                    updateDate = dtNow.ToString("yyyy/MM/dd HH:mm:ss"),
                });
            }

            context.Response.Write(JsonConvert.SerializeObject(new { data = res, learnmgrItem = iCntLearnmgrItem }));
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

