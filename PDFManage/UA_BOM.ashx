﻿<%@ WebHandler Language="C#" Class="Passport" %>

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
            case "get_EWLearnmgrItem":
                get_EWLearnmgrItem(context);
                break;
            case "saveByCol":
                saveByCol(context);
                break;
            case "saveByCol_check":
                saveByCol_check(context);
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


        sSql = "delete  PDFTAG.dbo.UA_BOM  where lubid=@lubid  delete  PDFTAG.dbo.UA_TagData  where lubid=@lubid  ";


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

        //UA_BOM
        sSql = @"insert into PDFTAG.dbo.UA_BOM 
(luhid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,QTY,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,org_lubid,lubcid,pipid,COLOR_SET) 
 select luhid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,QTY,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,org_lubid,lubcid,pipid,COLOR_SET
  from PDFTAG.dbo.UA_BOM where lubid=@lubid";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Execute(sSql, new { lubid = lubid });

            context.Response.Write(JsonConvert.SerializeObject(res));
        }

        //UA_TagData
        sSql = @"insert into PDFTAG.dbo.UA_TagData 
(hdid,type,lubid,tagnum,W1,W2,W3,W4,W5,W6,W7,W8,W9,W10,creator,creatordate,EW1,EW2,EW3,EW4,EW5,EW6,EW7,EW8,EW9,EW10)  
select t.hdid,t.type,bc.lubid,t.tagnum,t.W1,t.W2,t.W3,t.W4,t.W5,t.W6,t.W7,t.W8,t.W9,t.W10,@creator,@creatordate,t.EW1,t.EW2,t.EW3,t.EW4,t.EW5,t.EW6,t.EW7,t.EW8,t.EW9,t.EW10
  from PDFTAG.dbo.UA_TagData t
  inner join UA_BOM b on t.lubid = b.lubid
  inner join UA_BOM bc on b.org_lubid = bc.org_lubid and b.lubid <> bc.lubid
  where t.lubid=@lubid";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            DateTime dtNow = DateTime.Now;
            var res = cn.Execute(sSql, new { lubid = lubid, creator = LoginUser.PK, creatordate = dtNow.ToString("yyyy/MM/dd HH:mm:ss") });

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
        sSql += "from PDFTAG.dbo.UA_BOM  a             \n";
        sSql += "where 1=1 \n";
        sSql += " and a.lubid =@lubid \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query(sSql, new { lubid = lubid }).FirstOrDefault();
            var resNew = cn.Query(sSql, new { lubid = new_lubid }).FirstOrDefault();


            string orgText = (res == null ? "" : res.text);
            string newText = (resNew == null ? "" : resNew.text);

            double d = 0;
            if (orgText.Contains("/") && Double.TryParse(orgText.Split('/')[1], out d)) orgText = orgText.Replace(" ", "-");
            if (newText.Contains("/") && Double.TryParse(newText.Split('/')[1], out d)) newText = newText.Replace(" ", "-");

            context.Response.Write(JsonConvert.SerializeObject(new { orgText = orgText, newText = newText }));
        }
    }
    public class Text
    {
        public string TextOrg { get; set; }
        public string usage { get; set; }

    }

    public void get_LearnmgrItem(HttpContext context)
    {
        string lubid_org = context.Request["orgId"];
        string col = context.Request["col"];
        string style = context.Request["style"];
        string learnmgrItem = context.Request["learnmgrItem"];

        string sSql = "";


        sSql = "select " + col + " as TextOrg  from  PDFTAG.dbo.UA_BOM where lubid=@lubid";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var resLu_BOM_Org = cn.Query<Text>(sSql, new { lubid = lubid_org }).FirstOrDefault();

            string sTermname_org = resLu_BOM_Org.TextOrg.Trim().Replace(" ", "").ToLower();

            if (PublicFunction.arrUABomColors.Contains(col))
                col = "GarmentColor";

            sSql = "select distinct " + learnmgrItem + " from PDFTAG.dbo.UA_LearnmgrItem where ColSource=@ColSource and ColName=@ColName and termname_org=@termname_org  \n";
            var list = cn.Query(sSql, new
            {
                ColSource = "BOM",
                ColName = col,
                termname_org = sTermname_org,
            }).ToList();

            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }
    public void get_EWLearnmgrItem(HttpContext context)
    {
        string lubid_org = context.Request["orgId"];
        string col = context.Request["col"];
        string style = context.Request["style"];
        string learnmgrItem = context.Request["learnmgrItem"];
        string subColName = context.Request["subColName"];


        string sSql = "";


        sSql = "select " + subColName + " as TextOrg  from  PDFTAG.dbo.UA_TagData  where lubid=@lubid";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var resLu_BOM_Org = cn.Query<Text>(sSql, new { lubid = lubid_org }).FirstOrDefault();

            string sTermname_org = resLu_BOM_Org.TextOrg.Trim().Replace(" ", "").ToLower();


            sSql = "select distinct " + learnmgrItem + " from PDFTAG.dbo.UA_LearnmgrItem where "+learnmgrItem+" !='null' and ColSource=@ColSource and ColName=@ColName and subColName=@subColName and termname_org=@termname_org   \n";
            var list = cn.Query(sSql, new
            {
                ColSource = "BOM",
                ColName = col,
                subColName = subColName,
                termname_org = sTermname_org,
                //style = style //20240219
            }).ToList();

            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }
    public void saveByCol_check(HttpContext context)
    {
        string col = context.Request["col"];
        string style = context.Request["style"];
        string text = context.Request["text"];
        string chNote = context.Request["chNote"];

        string sSql = "";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            if (PublicFunction.arrGapBomColors.Contains(col))
                col = "GarmentColor";


            sSql = @"select * from PDFTAG.dbo.UA_LearnmgrItem 
where ColSource=@ColSource
and ColName=@ColName
and style=@style  
and termname=@termname 
and ISNULL(Ctermname,'')=@Ctermname 
";
            var list = cn.Query(sSql, new
            {
                ColSource = "BOM",
                ColName = col,
                style = style,
                termname = text,
                Ctermname = chNote,
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


        sSql = "select " + col + " as TextOrg,usage  from  PDFTAG.dbo.UA_BOM where lubid=@lubid";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var resLu_BOM_Org = cn.Query<Text>(sSql, new { lubid = lubid_org }).FirstOrDefault();

            sSql = "update PDFTAG.dbo.UA_BOM \n";
            sSql += "set " + col + "=@text,isEdit=1             \n";

            if (col == "SupplierArticle")
            {
                sSql += ",PARTS_TYPE=@PARTS_TYPE,PARTS_CODE=@PARTS_CODE,PARTS_DESC=@PARTS_DESC,MAT_ID=@MAT_ID             \n";
            }

            sSql += "where lubid=@lubid\n";

            var res = cn.Execute(sSql, new { lubid = lubid, text = text, PARTS_TYPE = PARTS_TYPE, PARTS_CODE = PARTS_CODE, PARTS_DESC = PARTS_DESC, MAT_ID = MAT_ID });

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

                var res2 = cn.Execute(sSql, new { IdName = "lubid", id = lubid, ColName = col, note = chNote, creator = LoginUser.PK, createordate = dtNow.ToString("yyyy/MM/dd HH:mm:ss") });
            }

            int iCntLearnmgrItem = 0;


            if (isRecord == "1")
            {
                if (col == "SupplierArticle")
                {
                    string EW1 = context.Request["EW1"];
                    string EW2 = context.Request["EW2"];
                    string EW3 = context.Request["EW3"];
                    string EW4 = context.Request["EW4"];
                    string EW5 = context.Request["EW5"];
                    string EW6 = context.Request["EW6"];
                    string EW7 = context.Request["EW7"];
                    string EW8 = context.Request["EW8"];

                    sSql = "select *  from   PDFTAG.dbo.UA_TagData where lubid=@lubid";

                    //var resUA_TagData = cn.Query(sSql, new { lubid = lubid }).FirstOrDefault();

                    DataTable dtUUA_TagData = new DataTable();
                    using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, cn))
                    {
                        cm.CommandText = sSql;
                        cm.Parameters.AddWithValue("@lubid", lubid);
                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                        {
                            da.Fill(dtUUA_TagData);
                        }
                    }

                    if (dtUUA_TagData.Rows.Count > 0)
                    {
                        string utdid = dtUUA_TagData.Rows[0]["utdid"].ToString();

                        for (int i = 1; i <= 8; i++)
                        {
                            string EditText = context.Request["EW" + i];
                            string EditNoteText = context.Request["EW" + i + "_Note"];
                            string sTermname_org = dtUUA_TagData.Rows[0]["W" + i].ToString();

                            if ((EditText == null || EditText == sTermname_org) && string.IsNullOrEmpty(EditNoteText))
                                continue;

                            sSql = @"IF NOT Exists (select * from PDFTAG.dbo.UA_LearnmgrItem where ColSource=@ColSource and ColName=@ColName and SubColName=@SubColName and termname=@termname and termname_org=@termname_org and Ctermname=@Ctermname and style=@style )
                             begin
                                  insert into PDFTAG.dbo.UA_LearnmgrItem
                                   (ColSource,ColName,FirstCharTermname_org,termname_org,termname,utdid,SubColName,Ctermname,style,creator,creatordate)
                                    values 
                              (@ColSource,@ColName,@FirstCharTermname_org,@termname_org,@termname,@utdid,@SubColName,@Ctermname,@style,@creator,@creatordate)
                              end ";

                            iCntLearnmgrItem = cn.Execute(sSql, new
                            {
                                ColSource = "BOM",
                                ColName = col,
                                FirstCharTermname_org = "",
                                termname_org = sTermname_org.Trim().Replace(" ", "").ToLower(),
                                termname = EditText,
                                utdid = utdid,
                                SubColName = "W" + i,
                                style = style,
                                Ctermname = EditNoteText,
                                creator = LoginUser.PK,
                                creatordate = dtNow.ToString("yyyy/MM/dd HH:mm:ss"),
                                UpdateUser = LoginUser.PK,
                                updateDate = dtNow.ToString("yyyy/MM/dd HH:mm:ss"),
                            });
                        }
                    }

                }
                else
                {

                    string sFirstCharTermname_org = resLu_BOM_Org.TextOrg.Substring(0, 1);
                    string sTermname_org = resLu_BOM_Org.TextOrg.Trim().Replace(" ", "").ToLower();
                    string sUsage = "";
                    string sW1 = "";
                    string sColorname = "";




                    if (col == "B1" || col == "B2" || col == "B3" || col == "B4" || col == "B5" || col == "B6" || col == "B7" || col == "B8" || col == "B9" || col == "B10")
                    {
                        //col = colorCol;
                        //20220803 不會針對0002-WHT做判斷，只會針對White的內容做取代，並顯示 修: PreWhite。trm 也有一個 0002-WHT。點[學習]後，不會把 DTM 變成 PreWhite
                        col = "GarmentColor";
                        sUsage = resLu_BOM_Org.usage;
                        sColorname = colorCol;

                        sSql = "select *  from   PDFTAG.dbo.UA_TagData where lubid=@lubid";
                        DataTable dtUUA_TagData = new DataTable();
                        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, cn))
                        {
                            cm.CommandText = sSql;
                            cm.Parameters.AddWithValue("@lubid", lubid);
                            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                            {
                                da.Fill(dtUUA_TagData);
                            }
                        }

                        if (dtUUA_TagData.Rows.Count > 0)
                        {
                            sW1 = dtUUA_TagData.Rows[0]["W1"].ToString();
                        }
                    }

                    sSql = @" insert into PDFTAG.dbo.UA_LearnmgrItem
                                   (ColSource,ColName,FirstCharTermname_org,termname_org,termname,Ctermname,style,usage,W1,Colorname,creator,creatordate)
                                    values 
                              (@ColSource,@ColName,@FirstCharTermname_org,@termname_org,@termname,@Ctermname,@style,@usage,@W1,@Colorname,@creator,@creatordate)";

                    iCntLearnmgrItem = cn.Execute(sSql, new
                    {
                        ColSource = "BOM",
                        ColName = col,
                        FirstCharTermname_org = sFirstCharTermname_org,
                        termname_org = sTermname_org,
                        termname = text,
                        Ctermname = chNote,
                        style = style,
                        usage = sUsage,
                        W1 = sW1,
                        Colorname = sColorname,
                        creator = LoginUser.PK,
                        creatordate = dtNow.ToString("yyyy/MM/dd HH:mm:ss"),
                        UpdateUser = LoginUser.PK,
                        updateDate = dtNow.ToString("yyyy/MM/dd HH:mm:ss"),
                    });

                }



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

