﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;
using OfficeOpenXml;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using System.Data.SqlClient;
using Dapper;
using System.Text.RegularExpressions;
using ImageResizer.Configuration.Xml;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using NPOI.SS.Formula.Functions;

public partial class Passport_Passport_A000 : System.Web.UI.Page
{
    protected string script;
    protected string parse = "0";
    List<string> listSampleStep = new List<string>();
    List<string> listSampleSize = new List<string>();
    bool isChina = false;
    CommonFunction _comFunction = new CommonFunction();
    protected void Page_Load(object sender, EventArgs e)
    {
        parse = Request["parse"];

        if (!IsPostBack)
        {
            setDl();
            DataBind();
        }
    }

    public string RemoveDateOnFileName(string fileName)
    {
        string sNewFile = fileName;
        try
        {
            sNewFile = sNewFile.Replace(sNewFile.Substring(0, sNewFile.IndexOf('_') + 1), "");
        }
        catch (Exception ex)
        {

        }
        return sNewFile;
    }

    private void setDl()
    {
        var sql = new SQLHelper();
        var dt = new DataTable();

        string sSql = "";


        sSql = "select gmid,gmname \n";
        sSql += "from PDFTAG.dbo.GroupManage a              \n";
        sSql += " where 1=1 and a.isshow=0  \n";

        sSql += " and a.gmcate in ('" + string.Join("','", LoginUser.CUST_NO) + "') \n";
        sSql += "order by a.gmname \n";

        Response.Write("<!--" + sSql + "-->");
        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            //ddlGroup.DataTextField = "gmname";
            //ddlGroup.DataValueField = "gmid";
            //ddlGroup.DataSource = dt;
            //ddlGroup.DataBind();
            ddlGroup.Items.Insert(0, new System.Web.UI.WebControls.ListItem("全部", ""));


            //ddlGroup2.DataTextField = "gmname";
            //ddlGroup2.DataValueField = "gmid";
            //ddlGroup2.DataSource = dt;
            //ddlGroup2.DataBind();
            //ddlGroup2.Items.Insert(0, new System.Web.UI.WebControls.ListItem("請選擇", ""));


            foreach (string sCust in LoginUser.CUST_NO)
            {
                sSql = @"select * from CUSTOMER_MAPPING where PVER = " + sCust;
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtCust = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtCust);
                }
                foreach (DataRow drCust in dtCust.Rows)
                {
                    dlVersion.Items.Add(new System.Web.UI.WebControls.ListItem(drCust["CUSTOMER"].ToString(), sCust));
                    ddlpver.Items.Add(new System.Web.UI.WebControls.ListItem(drCust["CUSTOMER"].ToString(), sCust));
                    dlgmcate.Items.Add(new System.Web.UI.WebControls.ListItem(drCust["CUSTOMER"].ToString(), sCust));
                }
            }
        }
        if (dlVersion.Items.Count > 0)
        {
            dlVersion_SelectedIndexChanged(null, null);
        }
    }



    private void DataBind()
    {
        bool _AllowPaging = true;
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        string txt = txtSearch.Text;
        string version = dlVersion.SelectedItem.Value;
        string gmid = ddlGroup.SelectedItem.Value;

        try
        {
            sSql += @"select a.*,b.season,b.style,b.generateddate,c.gmname, cust.CUSTOMER
                from PDFTAG.dbo.P_inProcess a 
                inner join CUSTOMER_MAPPING cust on a.pver = cust.pver ";

            if (version == "1")
                sSql += " left join PDFTAG.dbo.Lu_Header b on a.pipid=b.pipid and b.isshow=0             \n";
            else if (version == "2")
                sSql += " left join PDFTAG.dbo.UA_Header b on a.pipid=b.pipid and b.isshow=0             \n";
            else if (version == "3")
                sSql += " left join PDFTAG.dbo.GAP_Header b on a.pipid=b.pipid and b.isshow=0             \n";
            else
                sSql += " left join PDFTAG.dbo.HEADER_H b on a.pipid=b.pipid and b.isshow=0             \n";

            sSql += " left join PDFTAG.dbo.GroupManage c on a.gmid=c.gmid and c.isshow=0             \n";
            sSql += " where 1=1 and a.isshow=0 and a.hdid=0 \n"; //hdid > 0 為修改的版本

            if (!string.IsNullOrEmpty(version))
                sSql += " and (a.pver = '" + version + "'  ) \n";

            if (!string.IsNullOrEmpty(txt))
                sSql += " and (a.ptitle like '%" + txt + "%'  ) \n";

            if (!string.IsNullOrEmpty(gmid))
                sSql += " and (a.gmid = '" + gmid + "'  ) \n";

            //讓USER看得到同部門的
            if (LoginUser.role != LoginUser.ROLE_ADMIN)
                sSql += " and (a.pver in ('" + string.Join("','", LoginUser.CUST_NO.ToArray()) + "')  ) \n";

            sSql += "order by a.pipid desc \n";
            Response.Write("<!--" + sSql + "-->");

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }



            }

            this.LinkPageFirst1.Visible = true;
            this.LinkPageUp1.Visible = true;
            this.LinkPageDown1.Visible = true;
            this.LinkPageLast1.Visible = true;

            int pageSize = 30;//每頁顯示5筆           
            int nowpage = 1;
            int.TryParse(this.LabelNowPage1.Text, out nowpage); //當前頁計數器
            if (nowpage != 0) nowpage--;
            int PageCount = ((dt.Rows.Count / pageSize) <= 1) ? 1 : (dt.Rows.Count / pageSize);
            if ((dt.Rows.Count % pageSize) != 0)
            {
                PageCount++;
            }
            if (PageCount > 0) this.PageData1.Visible = true;
            else this.PageData1.Visible = false;

            this.LabelPageCount1.Text = PageCount.ToString();  //總共頁數
            this.LabelAllCount1.Text = dt.Rows.Count.ToString();
            if (nowpage == 0)
            {
                this.LinkPageFirst1.Visible = false;
                this.LinkPageUp1.Visible = false;
            }
            if (nowpage == (PageCount - 1))
            {
                this.LinkPageDown1.Visible = false;
                this.LinkPageLast1.Visible = false;
            }

            if (dt.Rows.Count < pageSize)
            {
                _AllowPaging = false;
                this.PageData1.Visible = false;
            }

            //叫用DBUtilty來分頁
            PagedDataSource pds = new PagedDataSource();
            pds.DataSource = dt.DefaultView;
            pds.AllowPaging = _AllowPaging;//是否啟用分頁
            pds.PageSize = pageSize;//一頁顯示的條數
            pds.CurrentPageIndex = nowpage;//獲取當前頁的索引

            //panView.Visible = true;
            this.panQuery.Visible = false;
            Repeater1.DataSource = pds;
            Repeater1.DataBind();
        }
        catch (Exception err)
        {
            this.panQuery.Visible = true;
            this.labErrMsg.Text = err.Message;
        }
    }

    #region 分頁控制1
    protected void btnInput_Click(object sender, EventArgs e)
    {
        string pipid = HiddenField1.Value;

        ParseExcel_All(pipid);
        DataBind();
    }
    protected void LinkPageFirst1_Click(object sender, EventArgs e)
    {
        this.LabelNowPage1.Text = "1";
        this.DataBind();
    }
    protected void LinkPageUp1_Click(object sender, EventArgs e)
    {
        int p = 0;
        int.TryParse(this.LabelNowPage1.Text, out p);
        p--;
        if (p <= 1) p = 1;
        this.LabelNowPage1.Text = p.ToString();
        this.DataBind();
    }
    protected void LinkPageDown1_Click(object sender, EventArgs e)
    {
        int p = 0;
        int.TryParse(this.LabelNowPage1.Text, out p);
        int PageCount = int.Parse(this.LabelPageCount1.Text);
        p++;
        if (p >= PageCount) p = PageCount;
        this.LabelNowPage1.Text = p.ToString();
        this.DataBind();
    }
    protected void LinkPageLast1_Click(object sender, EventArgs e)
    {
        int p = 0;
        int.TryParse(this.LabelPageCount1.Text, out p);
        this.LabelNowPage1.Text = p.ToString();
        this.DataBind();
    }
    #endregion

    private void CleanObject()
    {

    }

    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if ("del" == e.CommandName)
        {
            Delete(e.CommandArgument.ToString());
            DataBind();
        }
        if ("parsePDF" == e.CommandName)
        {
            ParsePDF(e.CommandArgument.ToString());
            DataBind();
        }
        if ("parsePDF_GAP" == e.CommandName)
        {
            string gaptype = PaseGAP_Header(e.CommandArgument.ToString());

            if (gaptype == "1")
            {
                parsePDF_GAP(e.CommandArgument.ToString());
                string new_pipid = Add_PDF_Manage_002_GAP(e.CommandArgument.ToString());
                LearnGAP(new_pipid);
            }
            else if (gaptype == "2")
            {
                //有格線
                parsePDF_GAP2(e.CommandArgument.ToString());
                string new_pipid = Add_PDF_Manage_002_GAP(e.CommandArgument.ToString());
                LearnGAP(new_pipid);
            }


            DataBind();
        }
        if ("parsePDF_UA" == e.CommandName)
        {
            isChina = false;
            PaseUA_Header(e.CommandArgument.ToString());
            parsePDF_UA(e.CommandArgument.ToString());
            parsePDF_UA_BOM_TagData(e.CommandArgument.ToString());
            string new_pipid = Add_PDF_Manage_002_UA(e.CommandArgument.ToString());
            LearnUA(new_pipid);
            DataBind();
        }
        if ("parsePDF_All" == e.CommandName)
        {
            ParsePDF_All(e.CommandArgument.ToString());
            DataBind();
        }
        if ("parseExcel_All" == e.CommandName)
        {
            ParseExcel_All(e.CommandArgument.ToString());
            DataBind();
        }
    }

    private void Delete(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        string sSql = "";
        DataTable dtV = new DataTable();
        sSql = "select pver from PDFTAG.dbo.P_inProcess where pipid = " + pipid;
        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtV);
            }
        }

        sSql = " update PDFTAG.dbo.P_inProcess set isshow = 1 where pipid = @pipid     \n";
        //編輯的版本也一併更新
        if (dtV.Rows[0][0].ToString() == "1")//LULU
        {
            sSql += @" update  P_inProcess set isShow = 1
                where pipid = (
                select e.pipid from Lu_Header o
                inner join Lu_Header e on e.org_luhid = o.luhid
                where o.pipid  = @pipid
                )";
        }
        else if (dtV.Rows[0][0].ToString() == "2")//UA
        {
            sSql += @" update  P_inProcess set isShow = 1
                where pipid = (
                select e.pipid from UA_Header o
                inner join UA_Header e on e.org_luhid = o.luhid
                where o.pipid  = @pipid
                )";
        }
        else if (dtV.Rows[0][0].ToString() == "3")//GAP
        {
            sSql += @" update  P_inProcess set isShow = 1
                where pipid = (
                select e.pipid from GAP_Header o
                inner join GAP_Header e on e.org_luhid = o.luhid
                where o.pipid  = @pipid
                )";
        }
        else
        {
            sSql += @" update  P_inProcess set isShow = 1
                where pipid = (
                select e.pipid from HEADER_H o
                inner join HEADER_H e on e.ORGID = o.ID
                where o.pipid  = @pipid
                );
                update HEADER_H set isShow = 1 where pipid = @pipid;
                update HEADER_H set isShow = 1 where pipid = (
                select e.pipid from HEADER_H o
                inner join HEADER_H e on e.ORGID = o.ID
                where o.pipid  = @pipid);";
        }

        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            cm.Parameters.AddWithValue("@pipid", pipid);

            cm.ExecuteNonQuery();
        }
    }

    #region LuLu
    private void ParsePDF(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        StringBuilder sbLog = new StringBuilder();

        List<Lu_LearnmgrItemDto> arrLu_LearnmgrItemDto = new List<Lu_LearnmgrItemDto>();

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            sSql = "select * from PDftag.dbo.Lu_LearnmgrItem  \n";

            arrLu_LearnmgrItemDto = cn.Query<Lu_LearnmgrItemDto>(sSql, new { }).ToList();
        }
        //20220714 不做取代,改成修改
        //arrLu_LearnmgrItemDto = new List<Lu_LearnmgrItemDto>();

        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {

            sSql = "select * from PDFTAG.dbo.P_inProcess where pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            //sSql = "select * from PDftag.dbo.Lu_LearnmgrItem  \n";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //DataTable dtLu_LearnmgrItem = new DataTable();
            //using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            //{
            //    da.Fill(dtLu_LearnmgrItem);
            //}

            string titleType = dt.Rows[0]["titleType"].ToString();
            string sPDFPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString());
            //string sSaveTxtPath = Server.MapPath("~/PdfToText/" + Path.GetFileNameWithoutExtension(dt.Rows[0]["piuploadfile"].ToString()) + ".txt");
            string sSaveTxtPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", ".txt")); ;

            if (parse == "1" || !System.IO.File.Exists(sSaveTxtPath))
                ConvertPDFToText(sPDFPath, sSaveTxtPath);

            if (!System.IO.File.Exists(sSaveTxtPath))
            {
                script = "alert('轉換失敗')";
                return;
            }

            //sSaveTxtPath = Server.MapPath("~/PDFManage/upload/PDF/20220415105352526/LW5CTES BOM 0607_2.txt");

            #region Parse Text for lulu



            string sLine = "";
            string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            bool isMaterialColorReport = false;
            bool isLu_BOMGarmentcolor = true;
            int iRow = 1;
            int iRowid = 1;
            long luhid = 0;
            string type = "";
            string sample = "";
            string sampleStep = "";
            int countHeader = -1;
            int colStandardPlacement = -1;
            int colPlacement = -1;
            int colSupplierArticle = -1;
            int colSupplier = -1;
            //int colTatol = -1;
            Dictionary<int, string> dicColor = new Dictionary<int, string>();
            List<string> arrColorNames = new List<string>();
            for (int a = 1; a <= 50; a++)
                arrColorNames.Add(a.ToString());
            using (StreamReader data = new StreamReader(sSaveTxtPath))
            {
                while (!data.EndOfStream)
                {
                    sLine = data.ReadLine();

                    if (iRow == 1)
                    {
                        #region Header:Lu_Header

                        string[] arrRow = sLine.Split(new string[] { "@Row" }, StringSplitOptions.None);

                        foreach (var row in arrRow)
                        {
                            if (row.Contains("Season:"))
                            {

                                string[] arrHeader = row.Split(new string[] { "&" }, StringSplitOptions.None);

                                string season = "";
                                string style = "";
                                string styleDesc = "";
                                string productsize = "";
                                string brand = "";
                                string dividion = "";
                                string sClass = "";
                                string pod = "";
                                string stylestatus = "";
                                string generateddate = "";
                                foreach (var header in arrHeader)
                                {
                                    if (header.Contains("Season:"))
                                        season = header.Split(':')[1].Trim();
                                    else if (header.Contains("Style #:"))
                                        style = header.Split(':')[1].Trim();
                                    else if (header.Contains("Style Desc:"))
                                        styleDesc = header.Split(':')[1].Trim();
                                    else if (header.Contains("Product Size Definition:"))
                                        productsize = header.Split(':')[1].Trim();
                                    else if (header.Contains("Brand:"))
                                        brand = header.Split(':')[1].Trim();
                                    else if (header.Contains("Division:"))
                                        dividion = header.Split(':')[1].Trim();
                                    else if (header.Contains("Class:"))
                                        sClass = header.Split(':')[1].Trim();
                                    else if (header.Contains("Pod:"))
                                        pod = header.Split(':')[1].Trim();
                                    else if (header.Contains("Style Status:"))
                                        stylestatus = header.Split(':')[1].Trim();
                                    else if (header.Contains("Generated Date:"))
                                        generateddate = header.Split(':')[1].Trim();
                                }

                                sSql = "delete PDFTAG.dbo.Lu_BOM where luhid=(select luhid from PDFTAG.dbo.Lu_Header where pipid=@pipid);";
                                sSql += "delete PDFTAG.dbo.Lu_BOMGarmentcolor where luhid=(select luhid from PDFTAG.dbo.Lu_Header where pipid=@pipid);";
                                sSql += "delete PDFTAG.dbo.Lu_SizeTable where luhid=(select luhid from PDFTAG.dbo.Lu_Header where pipid=@pipid);";
                                sSql += "delete PDFTAG.dbo.Lu_SizeTable_1 where luhid=(select luhid from PDFTAG.dbo.Lu_Header where pipid=@pipid);";
                                sSql += "delete PDFTAG.dbo.Lu_SizeTable_Header where pipid=@pipid;";
                                sSql += "delete PDFTAG.dbo.Lu_SizeTable_Header_1 where pipid=@pipid;";
                                sSql += "delete PDFTAG.dbo.Lu_Header where pipid=@pipid;";
                                sSql += @"insert into PDFTAG.dbo.Lu_Header 
(pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate) 
values
(@pipid, @season, @style, @styledesc, @productsize, @brand, @dividion, @class, @pod, @stylestatus, @generateddate, @stylesketch, @creator, @createordate, @mdate); SELECT SCOPE_IDENTITY();";

                                cm.CommandText = sSql;
                                cm.Parameters.Clear();

                                cm.Parameters.AddWithValue("@pipid", pipid);
                                cm.Parameters.AddWithValue("@season", season);
                                cm.Parameters.AddWithValue("@style", style);
                                cm.Parameters.AddWithValue("@styledesc", styleDesc);
                                cm.Parameters.AddWithValue("@productsize", productsize);
                                cm.Parameters.AddWithValue("@brand", brand);
                                cm.Parameters.AddWithValue("@dividion", dividion);
                                cm.Parameters.AddWithValue("@class", sClass);
                                cm.Parameters.AddWithValue("@pod", pod);
                                cm.Parameters.AddWithValue("@stylestatus", stylestatus);
                                cm.Parameters.AddWithValue("@generateddate", generateddate);
                                cm.Parameters.AddWithValue("@stylesketch", "");
                                cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                                cm.Parameters.AddWithValue("@createordate", sNow);
                                cm.Parameters.AddWithValue("@mdate", sNow);
                                luhid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                                //20231214 比對條件 多加欄位 style為條件，做判斷
                                arrLu_LearnmgrItemDto = arrLu_LearnmgrItemDto.Where(x => x.Style == style).ToList();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        if (sLine.Contains("BOM Comments"))
                            isMaterialColorReport = true;
                        else if (sLine.Contains("@Row: #  | Name  | Criticality"))
                            isMaterialColorReport = false;
                        else if (sLine.Contains("@Row: Request Name"))
                        {
                            countHeader++;//找到樣衣尺寸
                        }


                        if (isMaterialColorReport)
                        {
                            #region MaterialColorReport

                            if (sLine.Contains("BOM Comments"))
                            {
                                if (sLine.Contains("@Row: BOM Name"))
                                {
                                    //@Row: BOM Name BOM : WI22  | @Row: BOM Comments  | @Row:   fabric
                                    var arrRows = sLine.Split(new string[] { "@Row:" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    arrRows.RemoveAt(0);
                                    arrRows.RemoveAt(0);
                                    sLine = string.Join("@Row:", arrRows).Trim();
                                }

                                sLine = sLine.Replace("@Row: BOM Comments  | @Row:", "");
                            }

                            string[] arrRow = sLine.Split(new string[] { "@Row:" }, StringSplitOptions.RemoveEmptyEntries);

                            isLu_BOMGarmentcolor = true;
                            long lubcid = 0;
                            long lubid = 0;

                            List<string> arrBOMGarmentcolorHeaders = new List<string>();
                            for (int i = 0; i < arrRow.Length; i++)
                            {
                                string sRowLine = arrRow[i].Trim().TrimStart('|').TrimEnd('|');

                                if (i == 0)
                                {
                                    type = sRowLine.Replace("|", "").Trim();

                                }
                                else if (i == 1 || sRowLine.Contains("Standard Placement"))
                                {
                                    arrBOMGarmentcolorHeaders = new List<string>();

                                    if (!isLu_BOMGarmentcolor)
                                        continue;

                                    //Header
                                    string[] arrHeader = sRowLine.Split(new string[] { "|" }, StringSplitOptions.None);
                                    //List<string> arrBOMGarmentcolor = new List<string>();
                                    bool isColor = false;
                                    dicColor = new Dictionary<int, string>();//清掉
                                    for (int h = 0; h < arrHeader.Length; h++)
                                    {
                                        if (arrHeader[h].Replace(" ", "") == "StandardPlacement") colStandardPlacement = h;
                                        else if (arrHeader[h].Replace(" ", "") == "Placement") colPlacement = h;
                                        else if (arrHeader[h].Replace(" ", "") == "SupplierArticle#") colSupplierArticle = h;
                                        else if (arrHeader[h].Replace(" ", "") == "Supplier") colSupplier = h;
                                        //else if (Regex.IsMatch(arrHeader[h].Replace(" ", ""), @"^[0-9]{4,}") && Regex.IsMatch(arrHeader[h].Replace(" ", ""), @"[a-zA-Z]{1,}$"))
                                        else if (Regex.IsMatch(arrHeader[h].Replace(" ", ""), @"^[0-9]{4,}"))
                                        {
                                            string colorName = arrHeader[h].Replace(" ", "");
                                            //如果色組只有四碼，前面補0到6碼，只有0001-BLK及0002-WHT維持4碼
                                            if (Regex.IsMatch(colorName, @"^[0-9]{4}[^0-9]") && (colorName != "0001-BLK" && colorName != "0002-WHT"))
                                            {
                                                colorName = "00" + colorName;
                                            }
                                            dicColor.Add(h, colorName);
                                        }
                                    }
                                    //Lu_BOMGarmentcolor

                                    #region Lu_BOMGarmentcolor

                                    sSql = @"insert into PDFTAG.dbo.Lu_BOMGarmentcolor 
(luhid,A" + string.Join(",A", arrColorNames) + @") 
values 
(@luhid,@A" + string.Join(",@A", arrColorNames) + @"); SELECT SCOPE_IDENTITY();";

                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();

                                    cm.Parameters.AddWithValue("@luhid", luhid);

                                    for (int a = 1; a <= 50; a++)
                                    {
                                        if (a > dicColor.Count)
                                            cm.Parameters.AddWithValue("@A" + a, "");
                                        else
                                        {
                                            cm.Parameters.AddWithValue("@A" + a, dicColor.Values.ElementAt(a - 1));

                                            arrBOMGarmentcolorHeaders.Add(dicColor.Values.ElementAt(a - 1));
                                        }
                                    }

                                    lubcid = Convert.ToInt64(cm.ExecuteScalar().ToString());
                                    Response.Write("<!--lubcid=" + lubcid + "-->");
                                    //isLu_BOMGarmentcolor = false;

                                    #endregion

                                }
                                else
                                {
                                    //Response.Write("<!--type="+ type+" " + sRowLine + "-->");
                                    //sbLog.AppendLine("type = " + type + " " + sRowLine);
                                    string[] arrRowValue = sRowLine.Split(new string[] { "|" }, StringSplitOptions.None);

                                    ////@Row: See QA Man ual  |  
                                    ////@Row:  | G A R M E N T  | <--StandardPlacement是空的
                                    //if (arrRow[i].Trim().StartsWith("|"))
                                    //{
                                    //    arrRowValue = arrRow[i].Trim().TrimEnd('|').Split(new string[] { "|" }, StringSplitOptions.None);
                                    //}

                                    if (arrRowValue.Length == 1)
                                    {
                                        iRowid = 1;
                                        type = sRowLine.Replace("|", "").Trim();
                                        continue;
                                    }
                                    if (sRowLine.Contains("Total ="))
                                    {
                                        isMaterialColorReport = false;
                                        continue;
                                    }

                                    //排除異常列
                                    if (arrRowValue.Length < dicColor.Count)
                                        continue;


                                    #region Lu_BOM

                                    string StandardPlacement = arrRowValue[colStandardPlacement].Trim();
                                    string Placement = arrRowValue[colPlacement].Trim();
                                    string SupplierArticle = arrRowValue[colSupplierArticle].Trim().Replace(" ", string.Empty);
                                    string Supplier = arrRowValue[colSupplier].Trim();
                                    int iDataLength = arrRowValue.Length;

                                    if (string.IsNullOrWhiteSpace(StandardPlacement)
                                        && string.IsNullOrWhiteSpace(Placement)
                                        && string.IsNullOrWhiteSpace(SupplierArticle)
                                        && string.IsNullOrWhiteSpace(Supplier))
                                        continue;
                                    if (arrRowValue.Count() <= dicColor.Keys.ElementAt(0)) continue;//
                                    Dictionary<string, string> dictBOM = new Dictionary<string, string>();
                                    for (int countB = 1; countB <= 50; countB++)
                                    {
                                        try
                                        {
                                            if (countB <= dicColor.Count())
                                            {
                                                string matColor = arrRowValue[dicColor.Keys.ElementAt(countB - 1)].Trim();
                                                string matColorNew = string.Empty;
                                                bool spaceOrNot = false;
                                                for (int j = 0; j < matColor.Length; j++)//針對每個字元去做判斷
                                                {
                                                    if (matColor[j].ToString() == " ")//先拿掉空格，要去判斷下一個字是否大寫
                                                    {
                                                        if ((j - 1 > 0 && matColor[j - 1] == '/') || (j + 1 < matColor.Length && matColor[j + 1] == '/')) continue;  //若是斜線前後，直接拿掉空格
                                                        spaceOrNot = true;
                                                        continue;
                                                    }
                                                    if (spaceOrNot && matColor[j] >= 'a' && matColor[j] <= 'z')//若是小寫拿掉空格
                                                    {

                                                    }
                                                    else if (spaceOrNot) matColorNew += " ";    //其餘加回空格
                                                    matColorNew += matColor[j];
                                                    spaceOrNot = false;
                                                }
                                                dictBOM.Add(string.Format("B{0}", countB.ToString()), matColorNew);
                                            }
                                            else
                                            {
                                                dictBOM.Add(string.Format("B{0}", countB.ToString()), "");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            dictBOM.Add(string.Format("B{0}", countB.ToString()), "");
                                            //throw new Exception("arrRowValue.length="+ arrRowValue.Length+ " countB="+ countB+ " dicColor.Key=" + dicColor.Keys.ElementAt(countB - 1) + "="+ JsonConvert.SerializeObject(dicColor)+ " sRowLine="+ sRowLine);
                                        }
                                    }

                                    #region 比對詞彙

                                    //StandardPlacement、Placement、Supplier在原文件直接插入學習後的詞
                                    //SupplierArticle原文件保留，之後UPDATE在編輯的版本，並顯示修改
                                    var res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "StandardPlacement"
                                     && x.Termname_org == StandardPlacement.Trim().Replace(" ", "").ToLower());

                                    if (res != null)
                                        StandardPlacement = res.Termname;

                                    res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Placement"
                                     && x.Termname_org == Placement.Trim().Replace(" ", "").ToLower());

                                    if (res != null)
                                        Placement = res.Termname;

                                    //res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle"
                                    //&& x.Termname_org == SupplierArticle.Trim().Replace(" ", "").ToLower());

                                    //if (res != null)
                                    //	SupplierArticle = res.Termname;

                                    res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Supplier"
                                  && x.Termname_org == Supplier.Trim().Replace(" ", "").ToLower());

                                    if (res != null)
                                        Supplier = res.Termname;

                                    if (false)
                                    {
                                        #region color
                                        //                              bool isUpdateColor = false;
                                        //for (int a = 1; a <= 10; a++)
                                        //{
                                        //	if (a > arrBOMGarmentcolorHeaders.Count)
                                        //		break;

                                        //	//20220803 不會針對0002-WHT做判斷，只會針對White的內容做取代，並顯示 修: PreWhite。trm 也有一個 0002-WHT。點[學習]後，不會把 DTM 變成 PreWhite
                                        //	//string sColName = arrBOMGarmentcolorHeaders[a - 1];
                                        //	string sColName = "GarmentColor";

                                        //	switch (a)
                                        //	{
                                        //		case 1:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B1.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B1 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;

                                        //		case 2:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B2.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B2 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;

                                        //		case 3:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B3.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B3 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;

                                        //		case 4:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B4.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B4 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;
                                        //		case 5:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B5.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B5 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;
                                        //		case 6:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B6.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B6 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;
                                        //		case 7:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B7.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B7 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}

                                        //			break;
                                        //		case 8:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B8.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B8 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;
                                        //		case 9:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B9.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B9 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;
                                        //		case 10:
                                        //			res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == sColName
                                        //&& x.Termname_org == B10.Trim().Replace(" ", "").ToLower());

                                        //			if (res != null)
                                        //			{
                                        //				B10 = res.Termname;
                                        //				isUpdateColor = true;
                                        //			}
                                        //			break;
                                        //	}
                                        //}
                                        #endregion
                                    }


                                    #endregion


                                    bool isEdit = false;


                                    sSql = @"insert into PDFTAG.dbo.Lu_BOM 
(luhid,lubcid,type,rowid,StandardPlacement,Placement,SupplierArticle,Supplier,B" + string.Join(",B", arrColorNames) + @",isEdit) 
values 
(@luhid,@lubcid,@type,@rowid,@StandardPlacement,@Placement,@SupplierArticle,@Supplier,@B" + string.Join(",@B", arrColorNames) + ",@isEdit); SELECT SCOPE_IDENTITY();";

                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();

                                    cm.Parameters.AddWithValue("@luhid", luhid);
                                    cm.Parameters.AddWithValue("@lubcid", lubcid);
                                    cm.Parameters.AddWithValue("@type", type);
                                    cm.Parameters.AddWithValue("@rowid", iRowid);
                                    cm.Parameters.AddWithValue("@StandardPlacement", StandardPlacement);
                                    cm.Parameters.AddWithValue("@Placement", Placement);
                                    cm.Parameters.AddWithValue("@SupplierArticle", SupplierArticle);
                                    cm.Parameters.AddWithValue("@Supplier", Supplier);

                                    for (int colBom = 1; colBom <= 50; colBom++)
                                    {
                                        cm.Parameters.AddWithValue(string.Format("@B{0}", colBom.ToString()), dictBOM[string.Format("B{0}", colBom.ToString())].Trim());
                                    }

                                    cm.Parameters.AddWithValue("@isEdit", isEdit ? 1 : 0);

                                    lubid = Convert.ToInt64(cm.ExecuteScalar().ToString());
                                    iRowid++;

                                    Response.Write("<!--insert lubcid=" + lubcid + "-->");

                                    //先不用，mark起來
                                    //btnlearnNote(arrLu_LearnmgrItemDto, "BOM", StandardPlacement.Trim().Replace(" ", "").ToLower(), lubid.ToString(), "StandardPlacement", sNow);
                                    //btnlearnNote(arrLu_LearnmgrItemDto, "BOM", Placement.Trim().Replace(" ", "").ToLower(), lubid.ToString(), "Placement", sNow);
                                    //btnlearnNote(arrLu_LearnmgrItemDto, "BOM", SupplierArticle.Trim().Replace(" ", "").ToLower(), lubid.ToString(), "SupplierArticle", sNow);
                                    //btnlearnNote(arrLu_LearnmgrItemDto, "BOM", Supplier.Trim().Replace(" ", "").ToLower(), lubid.ToString(), "Supplier", sNow);                                    
                                    //for (int BNum = 1; BNum <= 10; BNum++)
                                    //{
                                    //    btnlearnNote(arrLu_LearnmgrItemDto, "BOM", dictBOM[string.Format("B{0}", BNum.ToString())].Trim().Replace(" ", "").ToLower(), lubid.ToString(), string.Format("B{0}", BNum.ToString()), sNow);
                                    //}

                                    #endregion
                                }

                            }

                            #endregion
                        }
                        else
                        {
                            #region Lu_SizeTable

                            string[] arrRow = sLine.Split(new string[] { "@Row:" }, StringSplitOptions.None);
                            if (sLine.Contains("@Row: #  | Name"))
                            {
                                List<Lu_SizeTableDto> arrLu_SizeTableDtos = new List<Lu_SizeTableDto>();
                                iRowid = 1;
                                long lusthid = 0;
                                int idxOfHTMInstruction = 0;

                                for (int i = 0; i < arrRow.Length; i++)
                                {
                                    string sRowLine = arrRow[i].Trim().TrimStart('|').TrimEnd('|');
                                    if (i == 1)
                                    {
                                        #region Lu_SizeTable_Header

                                        string[] arrRowValue = sRowLine.Split(new string[] { "|" }, StringSplitOptions.None);

                                        sSql = @"insert into PDFTAG.dbo.Lu_SizeTable_Header 
(pipid,SAMPLE,SAMPLESTEP,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15) 
values 
(@pipid,@sample,@sampleStep,@H1,@H2,@H3,@H4,@H5,@H6,@H7,@H8,@H9,@H10,@H11,@H12,@H13,@H14,@H15); SELECT SCOPE_IDENTITY();";

                                        cm.CommandText = sSql;
                                        cm.Parameters.Clear();
                                        cm.Parameters.AddWithValue("@pipid", pipid);

                                        for (int h = 0; h < 20; h++)
                                        {
                                            if (h < arrRowValue.Length)
                                            {
                                                if (arrRowValue[h].Replace(" ", "").Trim().ToLower() == "htminstruction")
                                                {
                                                    idxOfHTMInstruction = h;
                                                }
                                            }
                                        }

                                        //for (int h = (idxOfHTMInstruction == 5 ? 6 : 5); h < 20; h++)
                                        //{
                                        //    int iHIdex = (idxOfHTMInstruction == 5 ? (h - 5) : (h - 4));

                                        //    if (h < arrRowValue.Length)
                                        //    {
                                        //        cm.Parameters.AddWithValue("@H" + iHIdex, arrRowValue[h].ToString().Trim());

                                        //        if (arrRowValue[h].Replace(" ", "").Trim().ToLower() == "htminstruction")
                                        //        {
                                        //            idxOfHTMInstruction = h;
                                        //        }
                                        //    }
                                        //    else
                                        //        cm.Parameters.AddWithValue("@H" + iHIdex, "");
                                        //}
                                        bool flagSample = false;
                                        bool flagHTM = false;
                                        for (int h = 1; h <= 15; h++)
                                        {
                                            int iHIdex = (idxOfHTMInstruction == 5 ? (h + 5) : (h + 4));
                                            string val = "";
                                            if (iHIdex < arrRowValue.Length)
                                            {
                                                val = arrRowValue[iHIdex].ToString().Trim();
                                            }
                                            if (val == "New" && countHeader >= 0 && listSampleStep.Any())
                                            {
                                                flagSample = true;
                                                sample = listSampleStep[countHeader];
                                                int leftIndex = sample.IndexOf("(");
                                                int rightIndex = sample.IndexOf(")");
                                                if (leftIndex > -1 && rightIndex > -1)
                                                {
                                                    sampleStep = sample.Substring(leftIndex + 1, rightIndex - leftIndex - 1).Trim();
                                                    sample = sample.Substring(0, leftIndex).Trim();
                                                }
                                                else
                                                    sampleStep = sample;
                                                val = listSampleSize[countHeader];
                                            }
                                            cm.Parameters.AddWithValue("@H" + h, val);

                                            if (val.Replace(" ", "").Trim().ToLower() == "htminstruction")
                                            {
                                                idxOfHTMInstruction = iHIdex;
                                                flagHTM = true;
                                            }
                                        }
                                        //如果沒有New表示為全段尺寸表，就清空
                                        if (!flagSample) { sample = ""; sampleStep = ""; }
                                        cm.Parameters.AddWithValue("@sample", sample);
                                        cm.Parameters.AddWithValue("@sampleStep", sampleStep);

                                        lusthid = Convert.ToInt64(cm.ExecuteScalar().ToString());


                                        #endregion

                                    }
                                    else if (i >= 2)
                                    {
                                        string[] arrRowValue = sRowLine.Split(new string[] { "|" }, StringSplitOptions.None);

                                        int iLength = arrRowValue.Length;

                                        if (idxOfHTMInstruction == 5)
                                        {
                                            //HTMInstruction 在Tol(-)後
                                            arrLu_SizeTableDtos.Add(new Lu_SizeTableDto()
                                            {
                                                rowid = iRowid,
                                                codeid = arrRowValue[0].Trim(),
                                                Name = arrRowValue[1].Trim(),
                                                Criticality = arrRowValue[2].Trim(),
                                                TolA = arrRowValue[3].Trim(),
                                                TolB = arrRowValue[4].Trim(),
                                                HTMInstruction = arrRowValue[5].Trim(),
                                                A1 = iLength >= 7 ? ConvertToStrDouble(arrRowValue[6].Trim()).ToString() : "",
                                                A2 = iLength >= 8 ? ConvertToStrDouble(arrRowValue[7].Trim()).ToString() : "",
                                                A3 = iLength >= 9 ? ConvertToStrDouble(arrRowValue[8].Trim()).ToString() : "",
                                                A4 = iLength >= 10 ? ConvertToStrDouble(arrRowValue[9].Trim()).ToString() : "",
                                                A5 = iLength >= 11 ? ConvertToStrDouble(arrRowValue[10].Trim()).ToString() : "",
                                                A6 = iLength >= 12 ? ConvertToStrDouble(arrRowValue[11].Trim()).ToString() : "",
                                                A7 = iLength >= 13 ? ConvertToStrDouble(arrRowValue[12].Trim()).ToString() : "",
                                                A8 = iLength >= 14 ? ConvertToStrDouble(arrRowValue[13].Trim()).ToString() : "",
                                                A9 = iLength >= 15 ? ConvertToStrDouble(arrRowValue[14].Trim()).ToString() : "",
                                                A10 = iLength >= 16 ? ConvertToStrDouble(arrRowValue[15].Trim()).ToString() : "",
                                                A11 = iLength >= 17 ? ConvertToStrDouble(arrRowValue[16].Trim()).ToString() : "",
                                                A12 = iLength >= 18 ? ConvertToStrDouble(arrRowValue[17].Trim()).ToString() : "",
                                                A13 = iLength >= 19 ? ConvertToStrDouble(arrRowValue[18].Trim()).ToString() : "",
                                                A14 = iLength >= 20 ? ConvertToStrDouble(arrRowValue[19].Trim()).ToString() : "",
                                                A15 = iLength >= 21 ? ConvertToStrDouble(arrRowValue[20].Trim()).ToString() : "",
                                            });
                                        }
                                        else
                                        {
                                            //HTMInstruction 不在Tol(-)後

                                            arrLu_SizeTableDtos.Add(new Lu_SizeTableDto()
                                            {
                                                rowid = iRowid,
                                                codeid = arrRowValue[0].Trim(),
                                                Name = arrRowValue[1].Trim(),
                                                Criticality = arrRowValue[2].Trim(),
                                                TolA = arrRowValue[3].Trim(),
                                                TolB = arrRowValue[4].Trim(),
                                                HTMInstruction = (idxOfHTMInstruction > 0 ? arrRowValue[idxOfHTMInstruction].Trim() : ""),
                                                A1 = iLength >= 6 ? ConvertToStrDouble(arrRowValue[5].Trim()).ToString() : "",
                                                A2 = iLength >= 7 ? ConvertToStrDouble(arrRowValue[6].Trim()).ToString() : "",
                                                A3 = iLength >= 8 ? ConvertToStrDouble(arrRowValue[7].Trim()).ToString() : "",
                                                A4 = iLength >= 9 ? ConvertToStrDouble(arrRowValue[8].Trim()).ToString() : "",
                                                A5 = iLength >= 10 ? ConvertToStrDouble(arrRowValue[9].Trim()).ToString() : "",
                                                A6 = iLength >= 11 ? ConvertToStrDouble(arrRowValue[10].Trim()).ToString() : "",
                                                A7 = iLength >= 12 ? ConvertToStrDouble(arrRowValue[11].Trim()).ToString() : "",
                                                A8 = iLength >= 13 ? ConvertToStrDouble(arrRowValue[12].Trim()).ToString() : "",
                                                A9 = iLength >= 14 ? ConvertToStrDouble(arrRowValue[13].Trim()).ToString() : "",
                                                A10 = iLength >= 15 ? ConvertToStrDouble(arrRowValue[14].Trim()).ToString() : "",
                                                A11 = iLength >= 16 ? ConvertToStrDouble(arrRowValue[15].Trim()).ToString() : "",
                                                A12 = iLength >= 17 ? ConvertToStrDouble(arrRowValue[16].Trim()).ToString() : "",
                                                A13 = iLength >= 18 ? ConvertToStrDouble(arrRowValue[17].Trim()).ToString() : "",
                                                A14 = iLength >= 19 ? ConvertToStrDouble(arrRowValue[18].Trim()).ToString() : "",
                                                A15 = iLength >= 20 ? ConvertToStrDouble(arrRowValue[19].Trim()).ToString() : "",
                                            });
                                        }


                                        iRowid++;
                                    }
                                }

                                foreach (var item in arrLu_SizeTableDtos)
                                {
                                    #region 比對詞彙

                                    var res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Name"
                                    && x.Termname_org == item.Name.Trim().Replace(" ", "").ToLower());

                                    if (res != null)
                                        item.Name = res.Termname;

                                    res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Criticality"
                                     && x.Termname_org == item.Criticality.Trim().Replace(" ", "").ToLower());

                                    if (res != null)
                                        item.Criticality = res.Termname;

                                    #endregion

                                    sSql = @"insert into PDFTAG.dbo.Lu_SizeTable 
(luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15) 
values 
(@luhid,@rowid,@codeid,@Name,@Criticality,@TolA,@TolB,@HTMInstruction,@lusthid,@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9,@A10,@A11,@A12,@A13,@A14,@A15)  SELECT SCOPE_IDENTITY();";

                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();

                                    cm.Parameters.AddWithValue("@luhid", luhid);
                                    cm.Parameters.AddWithValue("@rowid", item.rowid);
                                    cm.Parameters.AddWithValue("@codeid", item.codeid);
                                    cm.Parameters.AddWithValue("@Name", item.Name);
                                    cm.Parameters.AddWithValue("@Criticality", item.Criticality);
                                    cm.Parameters.AddWithValue("@TolA", item.TolA);
                                    cm.Parameters.AddWithValue("@TolB", item.TolB);
                                    cm.Parameters.AddWithValue("@HTMInstruction", item.HTMInstruction);
                                    cm.Parameters.AddWithValue("@lusthid", lusthid);
                                    cm.Parameters.AddWithValue("@A1", item.A1);
                                    cm.Parameters.AddWithValue("@A2", item.A2);
                                    cm.Parameters.AddWithValue("@A3", item.A3);
                                    cm.Parameters.AddWithValue("@A4", item.A4);
                                    cm.Parameters.AddWithValue("@A5", item.A5);
                                    cm.Parameters.AddWithValue("@A6", item.A6);
                                    cm.Parameters.AddWithValue("@A7", item.A7);
                                    cm.Parameters.AddWithValue("@A8", item.A8);
                                    cm.Parameters.AddWithValue("@A9", item.A9);
                                    cm.Parameters.AddWithValue("@A10", item.A10);
                                    cm.Parameters.AddWithValue("@A11", item.A11);
                                    cm.Parameters.AddWithValue("@A12", item.A12);
                                    cm.Parameters.AddWithValue("@A13", item.A13);
                                    cm.Parameters.AddWithValue("@A14", item.A14);
                                    cm.Parameters.AddWithValue("@A15", item.A15);

                                    long lustid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                                    //先不用，mark起來
                                    //btnlearnNote(arrLu_LearnmgrItemDto, "Size", item.Name.Trim().Replace(" ", "").ToLower(), lustid.ToString(), "Name", sNow);
                                    //btnlearnNote(arrLu_LearnmgrItemDto, "Size", item.Criticality.Trim().Replace(" ", "").ToLower(), lustid.ToString(), "Criticality", sNow);
                                    //Dictionary<string, string> dictBOM = new Dictionary<string, string>();
                                    //for (int countB = 1; countB <= 15; countB++)
                                    //{
                                    //    dictBOM.Add(string.Format("A{0}", countB.ToString()), item."A" + countB.ToString());
                                    //}
                                    //for (int i = 1; i <= 15; i++)
                                    //{
                                    //    btnlearnNote(arrLu_LearnmgrItemDto, "Size", dictBOM[string.Format("A{0}", i.ToString())].Trim().Replace(" ", "").ToLower(), lustid.ToString(), string.Format("A{0}", i.ToString()), sNow);
                                    //}
                                }

                                #region Lu_SizeTable_1

                                sSql = @"select * from PDFTAG.dbo.Lu_SizeTable_Header where lusthid=@lusthid";

                                Response.Write("<!--" + sSql.Replace("@lusthid", lusthid.ToString()) + "-->");
                                cm.CommandText = sSql;
                                cm.Parameters.Clear();
                                cm.Parameters.AddWithValue("@lusthid", lusthid);
                                DataTable dtLu_SizeTable_Header = new DataTable();
                                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                                {
                                    da.Fill(dtLu_SizeTable_Header);
                                }

                                if (dtLu_SizeTable_Header.Rows.Count > 0)
                                {
                                    List<string> arrSizeHeaders = new List<string>(){
                                        "0", "2", "4", "6", "8", "10", "12", "14", "16", "18", "20", "XXXS", "XXS", "XS", "S", "M", "L", "XL", "XXL", "1X", "2X", "3X", "3XL", "4XL", "5XL"};
                                    //"0", "2", "4", "6", "8", "10", "12", "14", "16", "18", "20", "XXXS", "XXS", "XS", "S", "M", "L", "XL", "XXL", "3XL", "4XL", "5XL", "NEW" };

                                    List<Lu_SizeHeaderDto> arrSizeHeaderIdxs = new List<Lu_SizeHeaderDto>();

                                    for (int h = 1; h <= 15; h++)
                                    {
                                        if (arrSizeHeaders.Contains(dtLu_SizeTable_Header.Rows[0]["H" + h].ToString().Trim().ToUpper()))
                                        {
                                            arrSizeHeaderIdxs.Add(new Lu_SizeHeaderDto { Idx = h, Name = dtLu_SizeTable_Header.Rows[0]["H" + h].ToString().Trim() });
                                        }
                                        if (dtLu_SizeTable_Header.Rows[0]["H" + h].ToString().IndexOf(",") > -1)
                                        {
                                            string[] splitSampleSize = dtLu_SizeTable_Header.Rows[0]["H" + h].ToString().Split(',');
                                            for (int i = 0; i < splitSampleSize.Count(); i++)
                                            {
                                                arrSizeHeaderIdxs.Add(new Lu_SizeHeaderDto { Idx = i + 1, Name = splitSampleSize[i].Trim().ToUpper() });
                                            }
                                            break;
                                        }
                                    }



                                    sSql = @"insert into PDFTAG.dbo.Lu_SizeTable_Header_1 
(pipid,SAMPLE,SAMPLESTEP,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15) 
values 
(@pipid,@sample,@sampleStep,@H1,@H2,@H3,@H4,@H5,@H6,@H7,@H8,@H9,@H10,@H11,@H12,@H13,@H14,@H15); SELECT SCOPE_IDENTITY();";

                                    Response.Write("<!--" + sSql + "-->");
                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();
                                    cm.Parameters.AddWithValue("@pipid", pipid);
                                    cm.Parameters.AddWithValue("@sample", sample);
                                    cm.Parameters.AddWithValue("@sampleStep", sampleStep);
                                    for (int h = 0; h <= 14; h++)
                                    {
                                        //var res = arrSizeHeaderIdxs.FirstOrDefault(x => x.Idx == h);

                                        if (h < arrSizeHeaderIdxs.Count)
                                        {
                                            cm.Parameters.AddWithValue("@H" + (h + 1), arrSizeHeaderIdxs[h].Name);
                                        }
                                        else
                                            cm.Parameters.AddWithValue("@H" + (h + 1), "");
                                    }

                                    long lusthid_1 = Convert.ToInt64(cm.ExecuteScalar().ToString());

                                    sSql = @"insert into PDFTAG.dbo.Lu_SizeTable_1(lustid_relation,luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,isEdit) ";

                                    sSql += @" select lustid as lustid_relation,luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction ";
                                    sSql += @"  ,'" + lusthid_1 + "' as lusthid ";

                                    for (int h = 0; h <= 14; h++)
                                    {
                                        if (h < arrSizeHeaderIdxs.Count)
                                            sSql += "  ,A" + arrSizeHeaderIdxs[h].Idx + " as A" + (h + 1);
                                        else
                                            sSql += "  ,null as A" + (h + 1);
                                    }
                                    sSql += @"  ,0 as isEdit ";

                                    sSql += @" from PDFTAG.dbo.Lu_SizeTable ";
                                    sSql += @" where luhid=@luhid and lusthid=@lusthid";


                                    Response.Write("<!--" + sSql.Replace("@luhid", luhid.ToString()).Replace("@lusthid", lusthid.ToString()) + "-->");
                                    sbLog.AppendLine("[Insert into PDFTAG.dbo.Lu_SizeTable_1] pipid=" + pipid + " sSql=" + sSql.Replace("@luhid", luhid.ToString()).Replace("@lusthid", lusthid.ToString()));
                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();
                                    cm.Parameters.AddWithValue("@luhid", luhid);
                                    cm.Parameters.AddWithValue("@lusthid", lusthid);
                                    cm.ExecuteNonQuery();

                                }



                                #endregion
                            }

                            #endregion

                        }
                    }


                    iRow++;
                }
            }

            LogFile.Logger.Log(sbLog.ToString());

            #endregion


            sSql = "select a.*,b.season,b.style,b.generateddate,c.gmname \n";
            sSql += "from PDFTAG.dbo.P_inProcess a              \n";
            sSql += " left join PDFTAG.dbo.Lu_Header b on a.pipid=b.pipid and b.isshow=0             \n";
            sSql += " left join PDFTAG.dbo.GroupManage c on a.gmid=c.gmid and c.isshow=0             \n";
            sSql += " where 1=1 and a.isshow=0 and a.pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            dt = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            string ptitle = dt.Rows[0]["style"].ToString();
            string season2 = dt.Rows[0]["season"].ToString();
            string new_season = "";
            if (titleType == "1")
            {
                #region set ptitle

                string year = season2.Substring(season2.Length - 2, 2);

                if (season2.Contains("Spring"))
                {
                    new_season = year + "SP";
                    ptitle += "-" + new_season;

                }
                else if (season2.Contains("Summer"))
                {
                    new_season = year + "SU";
                    ptitle += "-" + new_season;
                }
                else if (season2.Contains("Fall"))
                {
                    new_season = year + "FA";
                    ptitle += "-" + new_season;

                }
                else if (season2.Contains("Fall & Winter"))
                {
                    new_season = year + "FW";
                    ptitle += "-" + new_season;

                }
                else if (season2.Contains("Winter"))
                {
                    new_season = year + "HO";
                    ptitle += "-" + new_season;

                }
                else if (season2.Contains("Holiday"))
                {
                    new_season = year + "HO";
                    ptitle += "-" + new_season;

                }


                #endregion

            }


            sSql = "update PDFTAG.dbo.P_inProcess    \n";
            sSql += " set mdate=@mdate \n";
            if (titleType == "1")
            {
                sSql += " ,ptitle=@ptitle \n";
            }
            sSql += "where pipid=@pipid \n";
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            cm.Parameters.AddWithValue("@pipid", pipid);
            cm.Parameters.AddWithValue("@mdate", sNow);
            cm.Parameters.AddWithValue("@ptitle", ptitle);
            cm.ExecuteNonQuery();

            if (titleType == "1")
            {
                sSql = "update  PDFTAG.dbo.Lu_Header    \n";
                sSql += " set season=@season \n";
                sSql += "where pipid=@pipid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", pipid);
                cm.Parameters.AddWithValue("@season", new_season);
                cm.ExecuteNonQuery();
            }
            string new_pipid = Add_PDF_Manage_002(pipid);

            LearnLuLu(new_pipid);

            script = "alert('執行完成!')";
        }
    }
    /// <summary>
    /// 點[執行]，自動 於歷程管理 自動新增資料
    /// </summary>
    /// <param name="pipid"></param>
    protected string Add_PDF_Manage_002(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        string new_pipid = "";

        List<string> arrColorNames = new List<string>();
        for (int a = 1; a <= 50; a++)
            arrColorNames.Add(a.ToString());

        try
        {
            hidpipid.Value = pipid;
            DateTime dtNow = DateTime.Now;
            string sFilePath = "";




            sSql = "select count(*)+1 as cnt \n";
            sSql += "from PDFTAG.dbo.HistoryData a              \n";
            sSql += " where pipid=@pipid \n";



            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);

                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string hisversion = dt.Rows[0]["cnt"].ToString();

                if (hisversion.Length == 1)
                    hisversion = "00" + hisversion;
                else if (hisversion.Length == 2)
                    hisversion = "0" + hisversion;

                hisversion = dtNow.ToString("yyyyMMdd") + "-" + hisversion;


                sSql = @"insert into PDFTAG.dbo.HistoryData 
(pipid,hisversion,editdate,creator,isShow) 
values 
(@pipid,@hisversion,@editdate,@creator,@isShow);SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.Parameters.AddWithValue("@hisversion", hisversion);
                cm.Parameters.AddWithValue("@editdate", dtNow);
                cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                cm.Parameters.AddWithValue("@isShow", "0");

                long hdid = Convert.ToInt64(cm.ExecuteScalar().ToString());


                sSql = @"insert into PDFTAG.dbo.P_inProcess 
(ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,hdid,gmid,unit) 
select ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,'" + hdid + @"' as hdid,gmid,unit
 from PDFTAG.dbo.P_inProcess where pipid=@pipid";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();

                sSql = "select pipid \n";
                sSql += "from PDFTAG.dbo.P_inProcess a              \n";
                sSql += " where hdid=@hdid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@hdid", hdid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                new_pipid = dt.Rows[0]["pipid"].ToString();

                sSql = "select luhid \n";
                sSql += "from PDFTAG.dbo.Lu_Header  a              \n";
                sSql += " where pipid=@pipid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string luhid = dt.Rows[0]["luhid"].ToString();


                sSql = @"insert into PDFTAG.dbo.Lu_Header 
(pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate,org_luhid) 
 select '" + new_pipid + @"'as pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate, luhid as org_luhid
from PDFTAG.dbo.Lu_Header where pipid=@pipid; SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                long new_luhid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                sSql = @"insert into PDFTAG.dbo.Lu_BOMGarmentcolor 
(luhid,A" + string.Join(",A", arrColorNames) + @") 
 select '" + new_luhid + @"' as luhid,A" + string.Join(",A", arrColorNames) + @"
   from PDFTAG.dbo.Lu_BOMGarmentcolor where luhid=@luhid;

insert into PDFTAG.dbo.Lu_BOM 
(luhid,type,rowid,StandardPlacement,Placement,SupplierArticle,Supplier,B" + string.Join(",B", arrColorNames) + @",org_lubid,lubcid) 
 select '" + new_luhid + @"' as luhid,type,rowid,StandardPlacement,Placement,SupplierArticle,Supplier,B" + string.Join(",B", arrColorNames) + @",lubid as org_lubid,lubcid
  from PDFTAG.dbo.Lu_BOM where luhid=@luhid;

insert into PDFTAG.dbo.Lu_SizeTable 
(luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,org_lustid) 
 select '" + new_luhid + @"' as luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15, lustid as org_lustid
   from PDFTAG.dbo.Lu_SizeTable where luhid=@luhid;
";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                cm.ExecuteNonQuery();
            }


        }
        catch (Exception err)
        {
            Response.Write("btnAdd_Click:" + err.ToString());
        }


        return new_pipid;
    }
    protected void LearnLuLu(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        try
        {
            DateTime dtNow = DateTime.Now;
            string sFilePath = "";

            List<Lu_LearnmgrItemDto> arrLu_LearnmgrItemDto = new List<Lu_LearnmgrItemDto>();

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_Header a              \n";
                sSql += " where 1=1 and a.isshow=0   \n";
                sSql += " and a.pipid = '" + pipid + "'   \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtLu_Header = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_Header);
                }

                string style = dtLu_Header.Rows[0]["style"].ToString();


                using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
                {
                    sSql = "select * from PDftag.dbo.Lu_LearnmgrItem where style=@style  \n";

                    arrLu_LearnmgrItemDto = cn.Query<Lu_LearnmgrItemDto>(sSql, new { style = style }).ToList();
                }

                #region Lu_BOM

                sSql = "select a.*,b.* \n";
                sSql += "from PDFTAG.dbo.Lu_BOM a              \n";
                sSql += " join PDFTAG.dbo.Lu_BOMGarmentcolor b on a.lubcid=b.lubcid              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid=(select luhid from PDFTAG.dbo.Lu_Header where isshow=0 and pipid='" + pipid + "')   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtLu_BOM = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_BOM);
                }

                foreach (DataRow drBom in dtLu_BOM.Rows)
                {
                    string lubid = drBom["lubid"].ToString();
                    string StandardPlacement = drBom["StandardPlacement"].ToString();
                    string Placement = drBom["Placement"].ToString();
                    string SupplierArticle = drBom["SupplierArticle"].ToString();
                    string Supplier = drBom["Supplier"].ToString();

                    string B1 = drBom["B1"].ToString();
                    string B2 = drBom["B2"].ToString();
                    string B3 = drBom["B3"].ToString();
                    string B4 = drBom["B4"].ToString();
                    string B5 = drBom["B5"].ToString();
                    string B6 = drBom["B6"].ToString();
                    string B7 = drBom["B7"].ToString();
                    string B8 = drBom["B8"].ToString();
                    string B9 = drBom["B9"].ToString();
                    string B10 = drBom["B10"].ToString();

                    string A1 = drBom["A1"].ToString();
                    string A2 = drBom["A2"].ToString();
                    string A3 = drBom["A3"].ToString();
                    string A4 = drBom["A4"].ToString();
                    string A5 = drBom["A5"].ToString();
                    string A6 = drBom["A6"].ToString();
                    string A7 = drBom["A7"].ToString();
                    string A8 = drBom["A8"].ToString();
                    string A9 = drBom["A9"].ToString();
                    string A10 = drBom["A10"].ToString();

                    bool isUpdate = false;
                    bool IsMappingSupplierArticle = false;

                    #region 比對詞彙

                    //StandardPlacement、Placement、Supplier原文件已被取代這邊不須再更新
                    //SupplierArticle找出學習的詞，並顯示修改
                    //var res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "StandardPlacement"
                    // && x.Termname_org == StandardPlacement.Trim().Replace(" ", "").ToLower());

                    //if (res != null)
                    //{
                    //	StandardPlacement = res.Termname;
                    //	isUpdate = true;
                    //}

                    //res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Placement"
                    // && x.Termname_org == Placement.Trim().Replace(" ", "").ToLower());

                    //if (res != null)
                    //{
                    //	Placement = res.Termname;
                    //	isUpdate = true;
                    //}

                    var res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle"
                && x.Termname_org == SupplierArticle.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        SupplierArticle = res.Termname;
                        isUpdate = true;
                    }
                    else
                    {
                        sSql = @"select distinct MAT_NO,MAT_NAME from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] where UPPER(MAT_NO)='" + SupplierArticle.ToUpper().Trim() + "'";

                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        DataTable dtSAM_MAT_DEF = new DataTable();
                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                        {
                            da.Fill(dtSAM_MAT_DEF);
                        }
                        if (dtSAM_MAT_DEF.Rows.Count > 0)
                        {
                            SupplierArticle = dtSAM_MAT_DEF.Rows[0]["MAT_NO"].ToString();
                            isUpdate = true;
                            IsMappingSupplierArticle = true;
                        }
                    }

                    res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Placement"
                     && x.Termname_org == Placement.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Placement = res.Termname;
                        InsertLuLuNote(cm, "lubid", lubid, "Placement", res.Ctermname);
                        //isUpdate = true;
                    }

                    res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle"
                    && x.Termname_org == SupplierArticle.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        SupplierArticle = res.Termname;
                        InsertLuLuNote(cm, "lubid", lubid, "SupplierArticle", res.Ctermname);
                        isUpdate = true;
                    }

                    #region color
                    bool isUpdateColor = false;
                    for (int a = 1; a <= 10; a++)
                    {
                        //20220803 不會針對0002-WHT做判斷，只會針對White的內容做取代，並顯示 修: PreWhite。trm 也有一個 0002-WHT。點[學習]後，不會把 DTM 變成 PreWhite
                        //A1 = "GarmentColor";
                        //A2 = "GarmentColor";
                        //A3 = "GarmentColor";
                        //A4 = "GarmentColor";
                        //A5 = "GarmentColor";
                        //A6 = "GarmentColor";
                        //A7 = "GarmentColor";
                        //A8 = "GarmentColor";
                        //A9 = "GarmentColor";
                        //A10 = "GarmentColor";

                        switch (a)
                        {
                            case 1:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                && x.ColorName == A1
              && x.Termname_org == B1.Trim().Replace(" ", "").ToLower());

                                //if (B1 == "White")
                                //{
                                //    Response.Write("<!--update PDFTAG.dbo.Lu_BOM B1=" + B1 + " res="+ (res==null?false:true) + "-->");
                                //}
                                if (res != null)
                                {

                                    B1 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B1", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;

                            case 2:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A2
              && x.Termname_org == B2.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B2 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B2", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;

                            case 3:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A3
              && x.Termname_org == B3.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B3 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B3", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;

                            case 4:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A4
              && x.Termname_org == B4.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B4 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B4", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;
                            case 5:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A5
              && x.Termname_org == B5.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B5 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B5", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;
                            case 6:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A6
              && x.Termname_org == B6.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B6 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B6", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;
                            case 7:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A7
              && x.Termname_org == B7.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B7 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B7", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;
                            case 8:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A8
              && x.Termname_org == B8.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B8 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B8", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;
                            case 9:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A9
              && x.Termname_org == B9.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B9 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B9", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;
                            case 10:
                                res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticle == SupplierArticle
                                && x.Placement == Placement
                                 && x.ColorName == A10
              && x.Termname_org == B10.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B10 = res.Termname;
                                    InsertLuLuNote(cm, "lubid", lubid, "B10", res.Ctermname);
                                    isUpdate = true;
                                    isUpdateColor = true;
                                }
                                break;

                        }

                    }
                    #endregion

                    #endregion

                    if (isUpdate)
                    {
                        bool isEdit = false;

                        //if (isUpdateColor)
                        isEdit = true;

                        sSql = "update PDFTAG.dbo.Lu_BOM \n";
                        sSql += "set  StandardPlacement=@StandardPlacement,Placement=@Placement,SupplierArticle=@SupplierArticle,Supplier=@Supplier           \n";
                        sSql += " ,B1=@B1,B2=@B2,B3=@B3,B4=@B4,B5=@B5,B6=@B6,B7=@B7,B8=@B8,B9=@B9,B10=@B10,isEdit=@isEdit,IsMappingSupplierArticle=@IsMappingSupplierArticle           \n";
                        sSql += "where lubid=@lubid\n";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@lubid", lubid);
                        cm.Parameters.AddWithValue("@StandardPlacement", StandardPlacement);
                        cm.Parameters.AddWithValue("@Placement", Placement);
                        cm.Parameters.AddWithValue("@SupplierArticle", SupplierArticle);
                        cm.Parameters.AddWithValue("@Supplier", Supplier);
                        cm.Parameters.AddWithValue("@B1", B1);
                        cm.Parameters.AddWithValue("@B2", B2);
                        cm.Parameters.AddWithValue("@B3", B3);
                        cm.Parameters.AddWithValue("@B4", B4);
                        cm.Parameters.AddWithValue("@B5", B5);
                        cm.Parameters.AddWithValue("@B6", B6);
                        cm.Parameters.AddWithValue("@B7", B7);
                        cm.Parameters.AddWithValue("@B8", B8);
                        cm.Parameters.AddWithValue("@B9", B9);
                        cm.Parameters.AddWithValue("@B10", B10);
                        cm.Parameters.AddWithValue("@IsMappingSupplierArticle", IsMappingSupplierArticle ? 1 : 0);
                        cm.Parameters.AddWithValue("@isEdit", isEdit ? 1 : 0);
                        cm.ExecuteNonQuery();


                    }

                }

                #endregion

                #region Lu_SizeTable

                sSql = "select a.*,b.* \n";
                sSql += "from PDFTAG.dbo.Lu_SizeTable a              \n";
                sSql += " join PDFTAG.dbo.Lu_SizeTable_Header b on a.lusthid=b.lusthid              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid=(select luhid from PDFTAG.dbo.Lu_Header where isshow=0 and pipid='" + pipid + "')   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtLu_SizeTable = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_SizeTable);
                }

                foreach (DataRow drLu_SizeTable in dtLu_SizeTable.Rows)
                {
                    string lustid = drLu_SizeTable["lustid"].ToString();
                    string codeid = drLu_SizeTable["codeid"].ToString();
                    string Name = drLu_SizeTable["Name"].ToString();
                    string Criticality = drLu_SizeTable["Criticality"].ToString();
                    string HTMInstruction = drLu_SizeTable["HTMInstruction"].ToString();

                    string A1 = drLu_SizeTable["A1"].ToString();
                    string A2 = drLu_SizeTable["A2"].ToString();
                    string A3 = drLu_SizeTable["A3"].ToString();
                    string A4 = drLu_SizeTable["A4"].ToString();
                    string A5 = drLu_SizeTable["A5"].ToString();
                    string A6 = drLu_SizeTable["A6"].ToString();
                    string A7 = drLu_SizeTable["A7"].ToString();
                    string A8 = drLu_SizeTable["A8"].ToString();
                    string A9 = drLu_SizeTable["A9"].ToString();
                    string A10 = drLu_SizeTable["A10"].ToString();
                    string A11 = drLu_SizeTable["A11"].ToString();
                    string A12 = drLu_SizeTable["A12"].ToString();
                    string A13 = drLu_SizeTable["A13"].ToString();
                    string A14 = drLu_SizeTable["A14"].ToString();
                    string A15 = drLu_SizeTable["A15"].ToString();

                    string H1 = drLu_SizeTable["H1"].ToString();
                    string H2 = drLu_SizeTable["H2"].ToString();
                    string H3 = drLu_SizeTable["H3"].ToString();
                    string H4 = drLu_SizeTable["H4"].ToString();
                    string H5 = drLu_SizeTable["H5"].ToString();
                    string H6 = drLu_SizeTable["H6"].ToString();
                    string H7 = drLu_SizeTable["H7"].ToString();
                    string H8 = drLu_SizeTable["H8"].ToString();
                    string H9 = drLu_SizeTable["H9"].ToString();
                    string H10 = drLu_SizeTable["H10"].ToString();
                    string H11 = drLu_SizeTable["H11"].ToString();
                    string H12 = drLu_SizeTable["H12"].ToString();
                    string H13 = drLu_SizeTable["H13"].ToString();
                    string H14 = drLu_SizeTable["H14"].ToString();
                    string H15 = drLu_SizeTable["H15"].ToString();

                    bool isUpdate = false;

                    #region 比對詞彙

                    var res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Name"
                    && x.code == codeid
                                  && x.Termname_org == Name.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Name = res.Termname;
                        InsertLuLuNote(cm, "lustid", lustid, "Name", res.Ctermname);
                        isUpdate = true;
                    }

                    //res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Criticality"
                    // && x.Termname_org == Criticality.Trim().Replace(" ", "").ToLower());

                    //if (res != null)
                    //{
                    //    Criticality = res.Termname;
                    //    isUpdate = true;
                    //}

                    res = arrLu_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "HTMInstruction"
                     && x.code == codeid
                     && x.Termname_org == HTMInstruction.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        HTMInstruction = res.Termname;
                        InsertLuLuNote(cm, "lustid", lustid, "HTMInstruction", res.Ctermname);
                        isUpdate = true;
                    }

                    #endregion

                    if (isUpdate)
                    {
                        sSql = "update PDFTAG.dbo.Lu_SizeTable \n";
                        sSql += "set  Name=@Name,HTMInstruction=@HTMInstruction,isEdit=1          \n";
                        sSql += "where lustid=@lustid\n";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@lustid", lustid);
                        cm.Parameters.AddWithValue("@Name", Name);
                        cm.Parameters.AddWithValue("@HTMInstruction", HTMInstruction);
                        cm.ExecuteNonQuery();
                    }
                }

                #endregion

            }

            script = "alert('完成!')";
        }
        catch (Exception err)
        {
            Response.Write("btnAdd_Click:" + err.ToString());
        }
    }
    public void InsertLuLuNote(System.Data.SqlClient.SqlCommand cm,string IdName, string lubid, string ColName, string note)
    {
        if (string.IsNullOrEmpty(note))
            return;
        string sSql = @"IF NOT Exists (select * from PDFTAG.dbo.Lu_Ch_Note where IdName=@IdName and id=@id and ColName=@ColName )
                             begin
                                  insert into PDFTAG.dbo.Lu_Ch_Note
                                   (IdName,id,ColName,note,creator,createordate)
                                    values 
                              (@IdName,@id,@ColName,@note,@creator,@createordate)
                              end 
                             else 
                                begin
                                update PDFTAG.dbo.Lu_Ch_Note
                                set note=@note
                             where IdName=@IdName and id=@id and ColName=@ColName
                             end ";

        cm.CommandText = sSql;
        cm.Parameters.Clear();
        cm.Parameters.AddWithValue("@IdName", IdName);
        cm.Parameters.AddWithValue("@id", lubid);
        cm.Parameters.AddWithValue("@ColName", ColName);
        cm.Parameters.AddWithValue("@note", note);
        cm.Parameters.AddWithValue("@creator", LoginUser.PK);
        cm.Parameters.AddWithValue("@createordate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cm.ExecuteNonQuery();
    }
    #endregion

    #region GAP

    public string PaseGAP_Header(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        StringBuilder sbLog = new StringBuilder();

        string gaptype = "";

        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {

            sSql = "select * from PDFTAG.dbo.P_inProcess where pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            //sSql = "select * from PDftag.dbo.Lu_LearnmgrItem  \n";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //DataTable dtLu_LearnmgrItem = new DataTable();
            //using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            //{
            //    da.Fill(dtLu_LearnmgrItem);
            //}

            gaptype = dt.Rows[0]["Gaptype"].ToString();
            string titleType = dt.Rows[0]["titleType"].ToString();
            string sPDFPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString());
            //string sSaveTxtPath = Server.MapPath("~/PdfToText/" + Path.GetFileNameWithoutExtension(dt.Rows[0]["piuploadfile"].ToString()) + ".txt");
            string sSaveTxtPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", ".txt")); ;
            string sSaveTxtPath3 = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", "_3.txt")); ;

            if (parse == "1" || !System.IO.File.Exists(sSaveTxtPath))
            {
                if (gaptype == "2")
                {

                    //有格線
                    if (!System.IO.File.Exists(sSaveTxtPath))
                        ConvertPDFToText2(sPDFPath, sSaveTxtPath);
                    if (!System.IO.File.Exists(sSaveTxtPath3))
                        ConvertPDFToText3(sPDFPath, sSaveTxtPath3);
                }
                else
                {
                    if (!System.IO.File.Exists(sSaveTxtPath))
                        ConvertPDFToText3(sPDFPath, sSaveTxtPath);
                }

            }

            if (!System.IO.File.Exists(sSaveTxtPath))
            {
                script = "alert('轉換失敗')";
                return "";
            }

            #region Parse Text GAP_Header


            string sLastLine = "";
            string sLine = "";
            string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            bool isHeader = false;
            bool isHeaderDone = false;
            bool isHeaderMCLastMod = false;
            int iRow = 1;
            int iLineRow = 0;
            string[] stringSeparators = new string[] { "  " };
            GAP_HeaderDto gAP_HeaderDto = new GAP_HeaderDto();

            if (gaptype == "2")
            {
                #region 有格線

                using (StreamReader data = new StreamReader(sSaveTxtPath))
                {
                    while (!data.EndOfStream)
                    {

                        sLine = data.ReadLine();
                        if (sLine == null) break;
                        string[] arrHeaders = sLine.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
                        iLineRow++;


                        if (sLine.Contains("Design Number"))
                        {
                            if (arrHeaders.Count() > 2)
                                gAP_HeaderDto.style = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Design Number")) + 1];
                            else
                                gAP_HeaderDto.style = sLine.Replace("Design Number", "").Trim();
                            isHeader = true;
                        }
                        if (sLine.Contains("Legacy Style Numbers"))
                        {
                            if (arrHeaders.Count() > 2)
                                gAP_HeaderDto.style = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Legacy Style Numbers")) + 1] + "_" + gAP_HeaderDto.style;
                            else
                            {
                                if (sLine.Replace("Legacy Style Numbers", "").Trim().Length > 0)
                                    gAP_HeaderDto.style = sLine.Replace("Legacy Style Numbers", "").Trim() + "_" + gAP_HeaderDto.style;
                            }
                        }
                        if (isHeader)
                        {
                            if (sLine.Contains("Season"))
                            {
                                var arrSeason = sLine.Replace("Season", "").Trim().Replace(" ", "@").Split('@');
                                string year = arrSeason[1].Substring(arrSeason[1].Length - 2, 2);
                                string season2 = arrSeason[0];
                                string new_season = "";

                                if (season2.Contains("Spring"))
                                {
                                    new_season = year + "SP";

                                }
                                else if (season2.Contains("Summer"))
                                {
                                    new_season = year + "SU";
                                }
                                else if (season2.Contains("Fall"))
                                {
                                    new_season = year + "FA";

                                }
                                else if (season2.Contains("Fall & Winter"))
                                {
                                    new_season = year + "FW";

                                }
                                else if (season2.Contains("Winter"))
                                {
                                    new_season = year + "HO";

                                }
                                else if (season2.Contains("Holiday"))
                                {
                                    new_season = year + "HO";

                                }

                                gAP_HeaderDto.season = new_season;
                            }
                            else if (sLine.Contains("Description"))
                            {
                                string sDescription = "";
                                if (arrHeaders.Count() > 2)
                                    sDescription = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Description")) + 1];
                                else
                                    sDescription = sLine.Replace("Description", "").Trim();

                                sLine = data.ReadLine();

                                if (!sLine.Contains("BOM Numbern"))
                                    sDescription += sLine.Trim();

                                gAP_HeaderDto.ProductDescription = sDescription;
                            }
                            else if (sLine.Contains("Status"))
                            {
                                if (string.IsNullOrEmpty(gAP_HeaderDto.stylestatus))
                                {
                                    if (arrHeaders.Count() > 2)
                                        gAP_HeaderDto.stylestatus = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Status")) + 1];
                                    else
                                        gAP_HeaderDto.stylestatus = sLine.Replace("Status", "").Trim();
                                }
                            }
                            else if (sLine.Contains("Brand/Division"))
                            {
                                if (arrHeaders.Count() > 2)
                                    gAP_HeaderDto.brand = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Brand/Division")) + 1];
                                else
                                    gAP_HeaderDto.brand = sLine.Replace("Brand/Division", "").Trim();
                            }
                            else if (sLine.Contains("Collection"))
                            {
                                if (arrHeaders.Count() > 2)
                                    gAP_HeaderDto.sclass = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Collection")) + 1];
                                else
                                    gAP_HeaderDto.sclass = sLine.Replace("Collection", "").Trim();
                            }
                            else if (sLine.Contains("Revision Modiﬁed"))
                            {
                                if (arrHeaders.Count() > 2)
                                    gAP_HeaderDto.generateddate = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Revision Modiﬁed")) + 1];
                                else
                                    gAP_HeaderDto.generateddate = sLine.Replace("Revision Modiﬁed", "").Trim();
                                isHeaderDone = true;
                            }
                        }
                        if (isHeaderDone)
                        {
                            gAP_HeaderDto.styledesc = "";


                            sSql = "delete PDFTAG.dbo.GAP_BOM where pipid=@pipid;";
                            sSql += "delete PDFTAG.dbo.GAP_BOMGarmentcolor where pipid=@pipid;";
                            sSql += "delete PDFTAG.dbo.GAP_SizeTable where pipid=@pipid;";
                            //sSql += "delete PDFTAG.dbo.GAP_SizeTable_1 where pipid=@pipid;";
                            sSql += "delete PDFTAG.dbo.GAP_SizeTable_Header where pipid=@pipid;";
                            //sSql += "delete PDFTAG.dbo.GAP_SizeTable_Header_1 where pipid=@pipid;";
                            sSql += "delete PDFTAG.dbo.GAP_Header where pipid=@pipid;";
                            sSql += @"insert into PDFTAG.dbo.GAP_Header 
(pipid, season, style, styledesc,ProductDescription, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate,isEdit) 
values
(@pipid, @season, @style, @styledesc,@ProductDescription, @productsize, @brand, @dividion, @class, @pod, @stylestatus, @generateddate, @stylesketch, @creator, @createordate, @mdate,0); SELECT SCOPE_IDENTITY();";

                            cm.CommandText = sSql;
                            cm.Parameters.Clear();

                            cm.Parameters.AddWithValue("@pipid", pipid);
                            cm.Parameters.AddWithValue("@season", gAP_HeaderDto.season);
                            cm.Parameters.AddWithValue("@style", gAP_HeaderDto.style);
                            cm.Parameters.AddWithValue("@styledesc", gAP_HeaderDto.styledesc);
                            cm.Parameters.AddWithValue("@ProductDescription", gAP_HeaderDto.ProductDescription);
                            cm.Parameters.AddWithValue("@productsize", "");
                            cm.Parameters.AddWithValue("@brand", gAP_HeaderDto.brand);
                            cm.Parameters.AddWithValue("@dividion", "");
                            cm.Parameters.AddWithValue("@class", gAP_HeaderDto.sclass);
                            cm.Parameters.AddWithValue("@pod", "");
                            cm.Parameters.AddWithValue("@stylestatus", gAP_HeaderDto.stylestatus);
                            cm.Parameters.AddWithValue("@generateddate", gAP_HeaderDto.generateddate);
                            cm.Parameters.AddWithValue("@stylesketch", "");
                            cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                            cm.Parameters.AddWithValue("@createordate", sNow);
                            cm.Parameters.AddWithValue("@mdate", sNow);
                            cm.ExecuteNonQuery();

                            break;
                        }


                        iRow++;

                        sLastLine = sLine;
                    }
                }

                #endregion
            }
            else
            {
                #region 無格線
                using (StreamReader data = new StreamReader(sSaveTxtPath))
                {
                    while (!data.EndOfStream)
                    {

                        sLine = data.ReadLine();
                        iLineRow++;


                        if (sLine.Contains("Cover Page %%"))
                        {
                            if (string.IsNullOrEmpty(gAP_HeaderDto.brand))
                                gAP_HeaderDto.brand = sLine.Replace("Cover Page %%", "").Split(new string[] { "%%" }, StringSplitOptions.None)[1].Trim();

                        }

                        if (!isHeaderDone && sLine.Contains("Master Style:") && sLine.Contains("Line Plan:"))
                        {
                            isHeader = true;

                        }

                        if (sLine.Contains("MC Last Mod:"))
                        {
                            isHeaderMCLastMod = true;

                        }

                        if (isHeader)
                        {
                            #region Header

                            //Master Style:548553; Studio JacketLine Plan: Athleta Knit Outerwear / Jackets / VestsPerformance HO22Flow: ProductDescription: 92687 Salutation Jacket_black_L2Product Team: Athleta Knit Outerwear / Jackets / VestsArea:BULLSEYEProduct Status:Final for ProductionProduct Type:JacketTheme:1X - 3XCategory:IPSSSub - Category:PERFORMANCE 3RD PIECEItem Type:JACKET _ L2BOM #000636021Tech Designer:Flippin, JudyDesigner:Lee, Alford %% 

                            sLine = sLine.Replace("@Row:", "");
                            string sTempLine = sLine.Replace("Master Style:", "").Replace("Line Plan:", "@Row");

                            string style = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[0].Split(';')[0].Trim();
                            string styleDesc = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[0].Split(';')[1].Trim();

                            sTempLine = sLine.Replace("Line Plan:", "@Row").Replace("Flow:", "@Row");
                            var arrLinPlans = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Split(' ');
                            string season = arrLinPlans[arrLinPlans.Length - 1];

                            string year = season.Substring(season.Length - 2, 2);
                            season = year + season.Replace(year, "");

                            sTempLine = sLine.Replace("ProductDescription:", "@Row").Replace("Product Team:", "@Row");
                            string productDescription = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Trim();


                            sTempLine = sLine.Replace("Product Status:", "@Row").Replace("Product Type:", "@Row");
                            string productStatus = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Trim();

                            sTempLine = sLine.Replace("Product Type:", "@Row").Replace("Theme:", "@Row");
                            string productType = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Trim();

                            isHeader = false;
                            isHeaderDone = true;

                            gAP_HeaderDto.style = style;
                            gAP_HeaderDto.styledesc = styleDesc;
                            gAP_HeaderDto.season = season;
                            gAP_HeaderDto.ProductDescription = productDescription;
                            gAP_HeaderDto.stylestatus = productStatus;
                            gAP_HeaderDto.sclass = productType;


                            #endregion
                        }
                        else if (isHeaderMCLastMod)
                        {
                            sLine = sLine.Replace("@Row:", "");
                            string sTempLine = sLine.Replace("MC Last Mod:", "@Row");

                            string MCLastMod = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Substring(0, 22).Trim();

                            gAP_HeaderDto.generateddate = MCLastMod;


                            sSql = "delete PDFTAG.dbo.GAP_BOM where pipid=@pipid;";
                            sSql += "delete PDFTAG.dbo.GAP_BOMGarmentcolor where pipid=@pipid;";
                            sSql += "delete PDFTAG.dbo.GAP_SizeTable where pipid=@pipid;";
                            //sSql += "delete PDFTAG.dbo.GAP_SizeTable_1 where pipid=@pipid;";
                            sSql += "delete PDFTAG.dbo.GAP_SizeTable_Header where pipid=@pipid;";
                            //sSql += "delete PDFTAG.dbo.GAP_SizeTable_Header_1 where pipid=@pipid;";
                            sSql += "delete PDFTAG.dbo.GAP_Header where pipid=@pipid;";
                            sSql += @"insert into PDFTAG.dbo.GAP_Header 
(pipid, season, style, styledesc,ProductDescription, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate,isEdit) 
values
(@pipid, @season, @style, @styledesc,@ProductDescription, @productsize, @brand, @dividion, @class, @pod, @stylestatus, @generateddate, @stylesketch, @creator, @createordate, @mdate,0); SELECT SCOPE_IDENTITY();";

                            cm.CommandText = sSql;
                            cm.Parameters.Clear();

                            cm.Parameters.AddWithValue("@pipid", pipid);
                            cm.Parameters.AddWithValue("@season", gAP_HeaderDto.season);
                            cm.Parameters.AddWithValue("@style", gAP_HeaderDto.style);
                            cm.Parameters.AddWithValue("@styledesc", gAP_HeaderDto.styledesc);
                            cm.Parameters.AddWithValue("@ProductDescription", gAP_HeaderDto.ProductDescription);
                            cm.Parameters.AddWithValue("@productsize", "");
                            cm.Parameters.AddWithValue("@brand", gAP_HeaderDto.brand);
                            cm.Parameters.AddWithValue("@dividion", "");
                            cm.Parameters.AddWithValue("@class", gAP_HeaderDto.sclass);
                            cm.Parameters.AddWithValue("@pod", "");
                            cm.Parameters.AddWithValue("@stylestatus", gAP_HeaderDto.stylestatus);
                            cm.Parameters.AddWithValue("@generateddate", gAP_HeaderDto.generateddate);
                            cm.Parameters.AddWithValue("@stylesketch", "");
                            cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                            cm.Parameters.AddWithValue("@createordate", sNow);
                            cm.Parameters.AddWithValue("@mdate", sNow);
                            cm.ExecuteNonQuery();

                            break;
                        }


                        iRow++;

                        sLastLine = sLine;
                    }
                }
                #endregion
            }





            #endregion
        }

        return gaptype;

    }

    private void parsePDF_GAP(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        StringBuilder sbLog = new StringBuilder();

        List<Lu_LearnmgrItemDto> arrLu_LearnmgrItemDto = new List<Lu_LearnmgrItemDto>();


        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {

            sSql = "select * from PDFTAG.dbo.P_inProcess where pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            //sSql = "select * from PDftag.dbo.Lu_LearnmgrItem  \n";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //DataTable dtLu_LearnmgrItem = new DataTable();
            //using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            //{
            //    da.Fill(dtLu_LearnmgrItem);
            //}

            string titleType = dt.Rows[0]["titleType"].ToString();
            string sPDFPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString());
            //string sSaveTxtPath = Server.MapPath("~/PdfToText/" + Path.GetFileNameWithoutExtension(dt.Rows[0]["piuploadfile"].ToString()) + ".txt");
            string sSaveTxtPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", ".txt")); ;

            if (parse == "1" || !System.IO.File.Exists(sSaveTxtPath))
                ConvertPDFToText2(sPDFPath, sSaveTxtPath);

            if (!System.IO.File.Exists(sSaveTxtPath))
            {
                script = "alert('轉換失敗')";
                return;
            }

            //sSql = "delete PDFTAG.dbo.GAP_BOM where luhid=(select luhid from PDFTAG.dbo.GAP_Header where pipid=@pipid);";
            //sSql += "delete PDFTAG.dbo.GAP_BOMGarmentcolor  where luhid=(select luhid from PDFTAG.dbo.GAP_Header where pipid=@pipid);";
            //sSql += "delete PDFTAG.dbo.Lu_SizeTable where luhid=(select luhid from PDFTAG.dbo.Lu_Header where pipid=@pipid);";
            //sSql += "delete PDFTAG.dbo.Lu_SizeTable_1 where luhid=(select luhid from PDFTAG.dbo.Lu_Header where pipid=@pipid);";
            //sSql += "delete PDFTAG.dbo.Lu_SizeTable_Header where pipid=@pipid;";
            //sSql += "delete PDFTAG.dbo.Lu_SizeTable_Header_1 where pipid=@pipid;";
            //sSql += "delete PDFTAG.dbo.Lu_Header where pipid=@pipid;";

            //sSql = "truncate table PDFTAG.dbo.GAP_BOM;";
            //sSql += "truncate table PDFTAG.dbo.GAP_BOMGarmentcolor;";
            //sSql = "truncate table PDFTAG.dbo.GAP_SizeTable;";
            //sSql += "truncate table PDFTAG.dbo.GAP_SizeTable_Header;";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //cm.Parameters.AddWithValue("@pipid", pipid);
            //cm.ExecuteNonQuery();

            //string sSaveTxtPath = Server.MapPath("~/PDFManage/GAP-549128~92755_RSI_Run_With_It_4.5IN_Short_L~000572126.txt");

            #region Parse Text for GAP




            string sLastLine = "";
            string sLine = "";
            string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            bool isHeader = false;
            bool isBom = false;
            bool isBomTypeData = false;
            bool isBomGarmentColor = false;
            string sBomType = "";
            List<int> arrBomGarmentColorStartIdx = new List<int>();
            bool isSizeTable = false;


            int iRow = 1;
            int iLineRow = 0;
            long lubcid = 0;
            string type = "";

            using (StreamReader data = new StreamReader(sSaveTxtPath))
            {
                while (!data.EndOfStream)
                {

                    sLine = data.ReadLine();
                    iLineRow++;

                    if (sLine.Contains("Bill of Materials"))
                    {
                        isHeader = false;
                        isBom = true;
                        isSizeTable = false;
                    }

                    if (sLine.Contains("Measurement Chart"))
                    {
                        isHeader = false;
                        isBom = false;
                        isSizeTable = true;
                    }


                    if (isBom)
                    {
                        #region isBom

                        if (sLine.Contains("(SET A:"))
                        {
                            isBomGarmentColor = true;
                            lubcid = 0;

                            #region BomGarmentColor Header

                            int startColumnIdx = sLine.IndexOf('(') - 1;

                            sLine = data.ReadLine();
                            iLineRow++;

                            List<string> arrCNumbers = new List<string>();

                            string[] arrHeaders = sLine.Substring(startColumnIdx, sLine.Length - startColumnIdx).Trim().Split(' ').Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
                            arrCNumbers.Add(string.Join("@", arrHeaders));

                            int iColumnCount = arrHeaders.Length;
                            int iMaxIdx = (startColumnIdx + iColumnCount * 12);


                            iRow = 1;
                            while (iRow < 10)
                            {
                                sLine = data.ReadLine();
                                iLineRow++;

                                List<string> arrParts = new List<string>();
                                for (int i = 0; i < iColumnCount; i++)
                                {
                                    int iLen = sLine.Length;
                                    int iStart = startColumnIdx + (11 * i);
                                    int iValLen = ((iStart + 10) > iLen ? iLen - iStart : 10);
                                    string sColVal = "";

                                    if (iStart < iLen)
                                        sColVal = sLine.Substring(iStart, iValLen);

                                    arrParts.Add(sColVal);



                                }
                                arrCNumbers.Add(string.Join("@", arrParts));

                                if (sLine.Contains("Usage")) break;

                                iRow++;
                            }

                            arrBomGarmentColorStartIdx = new List<int>();
                            for (int i = 0; i < iColumnCount; i++)
                            {
                                int iStart = startColumnIdx + (11 * i);
                                arrBomGarmentColorStartIdx.Add(iStart);
                            }

                            string[] arrRows = new string[iColumnCount];
                            foreach (var itemlineParts in arrCNumbers)
                            {
                                var arrCol = itemlineParts.Split('@');
                                for (int i = 0; i < arrCol.Length; i++)
                                    arrRows[i] += arrCol[i];
                            }

                            #region GAP_BOMGarmentcolor

                            sSql = @"insert into PDFTAG.dbo.GAP_BOMGarmentcolor  
(pipid,luhid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10) 
values 
(@pipid,@luhid,@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9,@A10); SELECT SCOPE_IDENTITY();";

                            cm.CommandText = sSql;
                            cm.Parameters.Clear();
                            cm.Parameters.AddWithValue("@pipid", pipid);
                            cm.Parameters.AddWithValue("@luhid", 0);

                            for (int a = 1; a <= 10; a++)
                            {
                                if (a > arrRows.Length)
                                    cm.Parameters.AddWithValue("@A" + a, "");
                                else
                                {
                                    cm.Parameters.AddWithValue("@A" + a, arrRows[a - 1]);
                                }
                            }

                            lubcid = Convert.ToInt64(cm.ExecuteScalar().ToString());
                            Response.Write("<!--lubcid=" + lubcid + "-->");
                            //isLu_BOMGarmentcolor = false;

                            #endregion


                            #endregion
                        }

                        if (sLine.StartsWith(" Fabric") || sLine.StartsWith(" Trim") || sLine.StartsWith(" Packaging / Labels"))
                        {
                            sBomType = sLine.Trim();
                            isBomTypeData = true;

                            #region Bom Type Data

                            //Image      Type/Desc/RD#          Quality Details        Article          Stitch/Ends           Size      UOM        Qty    Usage

                            List<string> arrBomHeaders = new List<string>();
                            List<int> arrBomHeadersStartIdx = new List<int>();
                            List<List<string>> arrLineRowDatas = new List<List<string>>();
                            List<string> arrRowDatas = new List<string>();


                            string[] arrHeaders = sLastLine.Trim().Replace("  ", "@").Split('@').Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();

                            foreach (var header in arrHeaders)
                            {
                                arrBomHeaders.Add(header);

                                arrBomHeadersStartIdx.Add(sLastLine.IndexOf(header));

                                if (header.Contains("Usage")) break;
                            }

                            int iHeaderColumn = arrBomHeadersStartIdx.Count() - 1;//去掉第一個欄位

                            foreach (var idx in arrBomGarmentColorStartIdx)
                            {
                                arrBomHeadersStartIdx.Add(idx);
                            }

                            int iBomHeadersStartIdxCount = arrBomHeadersStartIdx.Count;

                            iRow = 1;
                            while (iRow < 100)
                            {
                                sLine = data.ReadLine();
                                iLineRow++;

                                if (sLine.Contains("Generated Date/Time"))
                                {
                                    isBomTypeData = false;
                                    //break;
                                }


                                //if (sLine.StartsWith(" Fabric") || sLine.StartsWith(" Trim") || sLine.StartsWith(" Packaging / Labels"))
                                //{
                                //    //新Type, 相同欄位
                                //    sBomType = sLine.Trim();
                                //    continue;
                                //}

                                string sTempLine = sLine;

                                if (sLine.StartsWith(" Packaging / Labels"))
                                {
                                    //太長
                                    sTempLine = sTempLine.Replace(" Packaging / Labels", "pack");
                                }

                                int iLen = sTempLine.Length;

                                List<string> arrLineParts = new List<string>();
                                for (int i = 1; i < iBomHeadersStartIdxCount; i++)
                                {
                                    int iStartIdx = arrBomHeadersStartIdx[i];
                                    int iColumnLen = 0;
                                    string sColVal = "";

                                    if (iStartIdx < iLen)
                                    {
                                        if ((i + 1) < iBomHeadersStartIdxCount)
                                        {
                                            iColumnLen = (arrBomHeadersStartIdx[i + 1] - iStartIdx) - 1;
                                        }
                                        else
                                        {
                                            iColumnLen = iLen - iStartIdx - 1;

                                            if ((i + 1) == iBomHeadersStartIdxCount)
                                            {
                                                //最後一個欄位
                                                iColumnLen = iLen - iStartIdx;
                                            }
                                        }
                                        if ((iStartIdx + iColumnLen) > iLen)
                                            iColumnLen = iLen - iStartIdx - 1;

                                        sColVal = sTempLine.Substring(iStartIdx, iColumnLen);
                                    }
                                    arrLineParts.Add(sColVal);

                                }


                                int iSpaceCnt = arrLineParts.Where(w => string.IsNullOrWhiteSpace(w)).Count();
                                if (iSpaceCnt == arrLineParts.Count() || !isBomTypeData)
                                {
                                    //Row資料結束
                                    string[] arrRows = new string[iBomHeadersStartIdxCount - 1];
                                    foreach (var ItemlineParts in arrLineRowDatas)
                                    {
                                        for (int i = 0; i < ItemlineParts.Count; i++)
                                            arrRows[i] += ItemlineParts[i];
                                    }


                                    //換頁整行空白
                                    if (arrRows.Any(w => !string.IsNullOrEmpty(w)))
                                    {
                                        arrRowDatas.Add(sBomType + "%%" + string.Join("%%", arrRows));
                                    }
                                    arrLineRowDatas = new List<List<string>>();

                                }
                                else
                                {
                                    arrLineRowDatas.Add(arrLineParts);
                                }

                                if (sLine.StartsWith(" Fabric") || sLine.StartsWith(" Trim") || sLine.StartsWith(" Packaging / Labels"))
                                {
                                    //新Type, 相同欄位
                                    sBomType = sLine.Trim();
                                    continue;
                                }
                                if (!isBomTypeData)
                                    break;

                                iRow++;
                            }

                            iRow = 1;
                            foreach (var itemRow in arrRowDatas)
                            {
                                //var arrColValue = itemRow.Split('@');
                                var arrColValue = itemRow.Split(new string[] { "%%" }, StringSplitOptions.None);
                                string BomType = arrColValue[0];
                                string Usage = arrColValue[8];

                                sSql = @"insert into  PDFTAG.dbo.GAP_BOM
(pipid,luhid,lubcid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,isEdit) 
values 
(@pipid,@luhid,@lubcid,@type,@rowid,@StandardPlacement,@Usage,@SupplierArticle,@Supplier,@B1,@B2,@B3,@B4,@B5,@B6,@B7,@B8,@B9,@B10,@isEdit); SELECT SCOPE_IDENTITY();";

                                cm.CommandText = sSql;
                                cm.Parameters.Clear();
                                cm.Parameters.AddWithValue("@pipid", pipid);
                                cm.Parameters.AddWithValue("@luhid", 0);
                                cm.Parameters.AddWithValue("@lubcid", lubcid);
                                cm.Parameters.AddWithValue("@type", BomType);
                                cm.Parameters.AddWithValue("@rowid", iRow);
                                cm.Parameters.AddWithValue("@StandardPlacement", "");
                                cm.Parameters.AddWithValue("@Usage", Usage);//Usage
                                cm.Parameters.AddWithValue("@SupplierArticle", arrColValue[3]);
                                cm.Parameters.AddWithValue("@Supplier", arrColValue[3]);


                                for (int b = 1; b <= 10; b++)
                                {
                                    int idx = (iHeaderColumn + b);
                                    if (idx >= arrColValue.Length)
                                        cm.Parameters.AddWithValue("@B" + b, "");
                                    else
                                    {
                                        cm.Parameters.AddWithValue("@B" + b, arrColValue[idx]);
                                    }
                                }

                                cm.Parameters.AddWithValue("@isEdit", 0);



                                var lubid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                                iRow++;
                            }




                            #endregion

                        }



                        #endregion
                    }
                    else if (isSizeTable)
                    {
                        #region SizeTable

                        if (sLine.StartsWith(" POM"))
                        {


                            #region SizeTable Data

                            // POM        Description              Add'l     Variation    QC       Tol(-)    Tol(+)

                            GAP_SizeTableHeaderDto resGAP_SizeTableHeaderDto = new GAP_SizeTableHeaderDto();
                            List<string> arrSizeTableHeaders = new List<string>();
                            List<string> arrSizeHeaders = new List<string>();
                            List<int> arrSizeTableHeadersStartIdx = new List<int>();
                            List<List<string>> arrLineRowDatas = new List<List<string>>();
                            List<string> arrRowDatas = new List<string>();
                            int iTolAddIdx = 0;
                            int iTolAddEndIdx = 0;
                            int iSizeColumn = 0;
                            bool isStartSizeColumn = false;
                            long lusthid = 0;

                            string[] arrHeaders = sLine.Trim().Replace("  ", "@").Split('@').Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();


                            foreach (var header in arrHeaders)
                            {
                                arrSizeTableHeaders.Add(header);

                                if (isStartSizeColumn)
                                {
                                    iSizeColumn++;
                                    arrSizeTableHeadersStartIdx.Add(iTolAddIdx + 11 * iSizeColumn);
                                    arrSizeHeaders.Add(header);
                                }
                                else
                                    arrSizeTableHeadersStartIdx.Add(sLine.IndexOf(header));

                                if (header.Contains("Tol(+)"))
                                {
                                    iTolAddIdx = sLine.IndexOf(header);
                                    iTolAddEndIdx = sLine.IndexOf("+)") + 2;
                                    isStartSizeColumn = true;
                                }
                            }

                            int iSizeHeaderLength = arrSizeHeaders.Count;
                            resGAP_SizeTableHeaderDto = new GAP_SizeTableHeaderDto()
                            {
                                HeaderCount = iSizeHeaderLength,
                                H1 = iSizeHeaderLength >= 1 ? arrSizeHeaders[0].Trim() : "",
                                H2 = iSizeHeaderLength >= 2 ? arrSizeHeaders[1].Trim() : "",
                                H3 = iSizeHeaderLength >= 3 ? arrSizeHeaders[2].Trim() : "",
                                H4 = iSizeHeaderLength >= 4 ? arrSizeHeaders[3].Trim() : "",
                                H5 = iSizeHeaderLength >= 5 ? arrSizeHeaders[4].Trim() : "",
                                H6 = iSizeHeaderLength >= 6 ? arrSizeHeaders[5].Trim() : "",
                                H7 = iSizeHeaderLength >= 7 ? arrSizeHeaders[6].Trim() : "",
                                H8 = iSizeHeaderLength >= 8 ? arrSizeHeaders[7].Trim() : "",
                                H9 = iSizeHeaderLength >= 9 ? arrSizeHeaders[8].Trim() : "",
                                H10 = iSizeHeaderLength >= 10 ? arrSizeHeaders[9].Trim() : "",
                                H11 = iSizeHeaderLength >= 11 ? arrSizeHeaders[10].Trim() : "",
                                H12 = iSizeHeaderLength >= 12 ? arrSizeHeaders[11].Trim() : "",
                                H13 = iSizeHeaderLength >= 13 ? arrSizeHeaders[12].Trim() : "",
                                H14 = iSizeHeaderLength >= 14 ? arrSizeHeaders[13].Trim() : "",
                                H15 = iSizeHeaderLength >= 15 ? arrSizeHeaders[14].Trim() : "",

                            };

                            int iHeaderColumn = arrSizeTableHeadersStartIdx.Count();

                            //foreach (var idx in arrBomGarmentColorStartIdx)
                            //{
                            //    arrBomHeadersStartIdx.Add(idx);
                            //}

                            int iSizeTableHeadersStartIdxCount = arrSizeTableHeadersStartIdx.Count;


                            sLine = data.ReadLine();//Header 有2行
                            iLineRow++;

                            iRow = 1;


                            List<GAP_SizeTableDto> arrP_SizeTableDtos = new List<GAP_SizeTableDto>();
                            while (iRow < 100)
                            {
                                sLine = data.ReadLine();
                                iLineRow++;

                                sLastLine = sLine;

                                if (string.IsNullOrEmpty(sLine))
                                    continue;

                                if (sLine.Contains("Generated Date/Time"))
                                {
                                    break;
                                }

                                int iLen = sLine.Length;
                                int iSizeValueIdx = 0;
                                List<string> arrLineParts = new List<string>();
                                for (int i = 0; i < iSizeTableHeadersStartIdxCount; i++)
                                {
                                    int iStartIdx = arrSizeTableHeadersStartIdx[i];
                                    int iColumnLen = 0;
                                    string sColVal = "";

                                    if (iStartIdx > iTolAddIdx)
                                    {
                                        if (sLine.Length > iTolAddEndIdx)
                                        {
                                            //Size欄位
                                            try
                                            {
                                                int iSizeTotalLen = sLine.Length - iTolAddEndIdx;
                                                int iSizeCount = iSizeTotalLen / iSizeColumn;
                                                string sSize = sLine.Substring(iTolAddEndIdx, iSizeTotalLen).TrimStart();
                                                //var arrSizeVals = SplitString(sSize, iSizeCount).ToArray();
                                                string[] arrSizeVals = sSize.Trim().Replace("  ", "@").Split('@').Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
                                                sColVal = arrSizeVals[iSizeValueIdx].Trim();
                                            }
                                            catch (Exception ex) { }
                                            iSizeValueIdx++;
                                        }
                                    }
                                    else
                                    {
                                        if (iStartIdx < iLen)
                                        {
                                            if ((i + 1) < iSizeTableHeadersStartIdxCount)
                                            {
                                                iColumnLen = (arrSizeTableHeadersStartIdx[i + 1] - iStartIdx) - 1;
                                            }
                                            else
                                            {
                                                iColumnLen = iLen - iStartIdx - 1;
                                            }
                                            if ((iStartIdx + iColumnLen) > iLen)
                                                iColumnLen = iLen - iStartIdx - 1;

                                            if (i == (iSizeTableHeadersStartIdxCount - 1))
                                                iColumnLen = iLen - iStartIdx;

                                            sColVal = sLine.Substring(iStartIdx, iColumnLen);
                                        }
                                    }

                                    arrLineParts.Add(sColVal);

                                }

                                string sPOM = arrLineParts[0];

                                if (!string.IsNullOrWhiteSpace(sPOM))
                                {

                                    arrLineRowDatas.Add(arrLineParts);

                                    string[] arrRows = new string[iSizeTableHeadersStartIdxCount];
                                    for (int i = 0; i < arrLineParts.Count; i++)
                                        arrRows[i] += arrLineParts[i];


                                    arrRowDatas.Add(string.Join("@", arrRows));

                                    //如果上一筆POM空的也合併
                                    int iLastIdx = arrRowDatas.Count - 2;
                                    if (iLastIdx >= 0 && string.IsNullOrWhiteSpace(arrRowDatas[iLastIdx].Split('@')[0]))
                                    {
                                        string[] arrLastRowDatas = arrRowDatas[iLastIdx].Split('@');

                                        for (int i = 0; i < arrLastRowDatas.Length; i++)
                                            arrRows[i] += arrLastRowDatas[i];


                                        arrRowDatas[iLastIdx + 1] = string.Join("@", arrRows);
                                        arrRowDatas.RemoveAt(iLastIdx);
                                    }

                                }
                                else
                                {
                                    //Row資料結,合併
                                    string[] arrRows = new string[iSizeTableHeadersStartIdxCount];
                                    int iLastIdx = arrRowDatas.Count - 1;

                                    if (iLastIdx >= 0)
                                    {
                                        string[] arrLastRowDatas = arrRowDatas[iLastIdx].Split('@');

                                        for (int i = 0; i < arrLastRowDatas.Length; i++)
                                            arrRows[i] += arrLastRowDatas[i];


                                        for (int i = 0; i < arrLineParts.Count; i++)
                                            arrRows[i] += arrLineParts[i];


                                        arrRowDatas[iLastIdx] = string.Join("@", arrRows);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < arrLineParts.Count; i++)
                                            arrRows[i] += arrLineParts[i];


                                        arrRowDatas.Add(string.Join("@", arrRows));
                                    }

                                    arrLineRowDatas = new List<List<string>>();
                                }


                                iRow++;
                            }

                            #region Insert PDFTAG.dbo.GAP_SizeTable

                            iRow = 1;
                            foreach (var itemRow in arrRowDatas)
                            {
                                var arrColValue = itemRow.Split('@');
                                int iLength = arrColValue.Length;

                                for (int b = 1; b <= 10; b++)
                                {
                                    int idx = (iHeaderColumn + b);
                                    if (idx > arrColValue.Length)
                                    {
                                        //cm.Parameters.AddWithValue("@B" + b, "");
                                    }
                                    else
                                    {
                                        //cm.Parameters.AddWithValue("@B" + b, arrColValue[idx - 1]);
                                        string v = arrColValue[idx - 1];
                                    }
                                }


                                arrP_SizeTableDtos.Add(new GAP_SizeTableDto()
                                {
                                    rowid = iRow,
                                    POM = arrColValue[0].Trim(),
                                    Description = arrColValue[1].Trim(),
                                    AddlComments = arrColValue[2].Trim(),
                                    Variation = arrColValue[3].Trim(),
                                    QC = arrColValue[4].Trim(),
                                    TolA = arrColValue[5].Trim(),
                                    TolB = arrColValue[6].Trim(),

                                    A1 = iLength >= 8 ? arrColValue[7].Trim() : "",
                                    A2 = iLength >= 9 ? arrColValue[8].Trim() : "",
                                    A3 = iLength >= 10 ? arrColValue[9].Trim() : "",
                                    A4 = iLength >= 11 ? arrColValue[10].Trim() : "",
                                    A5 = iLength >= 12 ? arrColValue[11].Trim() : "",
                                    A6 = iLength >= 13 ? arrColValue[12].Trim() : "",
                                    A7 = iLength >= 14 ? arrColValue[13].Trim() : "",
                                    A8 = iLength >= 15 ? arrColValue[14].Trim() : "",
                                    A9 = iLength >= 16 ? arrColValue[15].Trim() : "",
                                    A10 = iLength >= 17 ? arrColValue[16].Trim() : "",
                                    A11 = iLength >= 11 ? arrColValue[10].Trim() : "",
                                    A12 = iLength >= 12 ? arrColValue[11].Trim() : "",
                                    A13 = iLength >= 13 ? arrColValue[12].Trim() : "",
                                    A14 = iLength >= 14 ? arrColValue[13].Trim() : "",
                                    A15 = iLength >= 15 ? arrColValue[14].Trim() : "",

                                });



                                iRow++;
                            }


                            sSql = @"insert into PDFTAG.dbo.GAP_SizeTable_Header 
(pipid,HeaderCount,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15) 
values 
(@pipid,@HeaderCount,@H1,@H2,@H3,@H4,@H5,@H6,@H7,@H8,@H9,@H10,@H11,@H12,@H13,@H14,@H15); SELECT SCOPE_IDENTITY();";

                            cm.CommandText = sSql;
                            cm.Parameters.Clear();
                            cm.Parameters.AddWithValue("@pipid", pipid);
                            cm.Parameters.AddWithValue("@HeaderCount", resGAP_SizeTableHeaderDto.HeaderCount);
                            cm.Parameters.AddWithValue("@H1", resGAP_SizeTableHeaderDto.H1);
                            cm.Parameters.AddWithValue("@H2", resGAP_SizeTableHeaderDto.H2);
                            cm.Parameters.AddWithValue("@H3", resGAP_SizeTableHeaderDto.H3);
                            cm.Parameters.AddWithValue("@H4", resGAP_SizeTableHeaderDto.H4);
                            cm.Parameters.AddWithValue("@H5", resGAP_SizeTableHeaderDto.H5);
                            cm.Parameters.AddWithValue("@H6", resGAP_SizeTableHeaderDto.H6);
                            cm.Parameters.AddWithValue("@H7", resGAP_SizeTableHeaderDto.H7);
                            cm.Parameters.AddWithValue("@H8", resGAP_SizeTableHeaderDto.H8);
                            cm.Parameters.AddWithValue("@H9", resGAP_SizeTableHeaderDto.H9);
                            cm.Parameters.AddWithValue("@H10", resGAP_SizeTableHeaderDto.H10);
                            cm.Parameters.AddWithValue("@H11", resGAP_SizeTableHeaderDto.H11);
                            cm.Parameters.AddWithValue("@H12", resGAP_SizeTableHeaderDto.H12);
                            cm.Parameters.AddWithValue("@H13", resGAP_SizeTableHeaderDto.H13);
                            cm.Parameters.AddWithValue("@H14", resGAP_SizeTableHeaderDto.H14);
                            cm.Parameters.AddWithValue("@H15", resGAP_SizeTableHeaderDto.H15);


                            lusthid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                            foreach (var item in arrP_SizeTableDtos)
                            {
                                sSql = @"insert into PDFTAG.dbo.GAP_SizeTable 
(pipid,luhid,rowid,POM,Description,AddlComments,Variation,QC,TolA,TolB,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,isEdit) 
values 
(@pipid,@luhid,@rowid,@POM,@Description,@AddlComments,@Variation,@QC,@TolA,@TolB,@lusthid,@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9,@A10,@A11,@A12,@A13,@A14,@A15,@isEdit)  SELECT SCOPE_IDENTITY();";

                                cm.CommandText = sSql;
                                cm.Parameters.Clear();
                                cm.Parameters.AddWithValue("@pipid", pipid);
                                cm.Parameters.AddWithValue("@luhid", 0);
                                cm.Parameters.AddWithValue("@rowid", item.rowid);
                                cm.Parameters.AddWithValue("@POM", item.POM);
                                cm.Parameters.AddWithValue("@Description", item.Description);
                                cm.Parameters.AddWithValue("@AddlComments", item.AddlComments);
                                cm.Parameters.AddWithValue("@Variation", item.Variation);
                                cm.Parameters.AddWithValue("@QC", item.QC);
                                cm.Parameters.AddWithValue("@TolA", item.TolA);
                                cm.Parameters.AddWithValue("@TolB", item.TolB);

                                cm.Parameters.AddWithValue("@lusthid", lusthid);
                                cm.Parameters.AddWithValue("@A1", item.A1);
                                cm.Parameters.AddWithValue("@A2", item.A2);
                                cm.Parameters.AddWithValue("@A3", item.A3);
                                cm.Parameters.AddWithValue("@A4", item.A4);
                                cm.Parameters.AddWithValue("@A5", item.A5);
                                cm.Parameters.AddWithValue("@A6", item.A6);
                                cm.Parameters.AddWithValue("@A7", item.A7);
                                cm.Parameters.AddWithValue("@A8", item.A8);
                                cm.Parameters.AddWithValue("@A9", item.A9);
                                cm.Parameters.AddWithValue("@A10", item.A10);
                                cm.Parameters.AddWithValue("@A11", item.A11);
                                cm.Parameters.AddWithValue("@A12", item.A12);
                                cm.Parameters.AddWithValue("@A13", item.A13);
                                cm.Parameters.AddWithValue("@A14", item.A14);
                                cm.Parameters.AddWithValue("@A15", item.A15);

                                cm.Parameters.AddWithValue("@isEdit", 0);
                                cm.ExecuteNonQuery();

                            }


                            #endregion



                            #endregion

                        }

                        #endregion

                    }


                    iRow++;

                    sLastLine = sLine;
                }


                sSql = "select a.*,b.season,b.style,b.generateddate,c.gmname \n";
                sSql += "from PDFTAG.dbo.P_inProcess a              \n";
                sSql += " left join PDFTAG.dbo.GAP_Header b on a.pipid=b.pipid and b.isshow=0             \n";
                sSql += " left join PDFTAG.dbo.GroupManage c on a.gmid=c.gmid and c.isshow=0             \n";
                sSql += " where 1=1 and a.isshow=0 and a.pipid='" + pipid + "' \n";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string ptitle = dt.Rows[0]["style"].ToString();
                string season2 = dt.Rows[0]["season"].ToString();
                string new_season = "";
                if (titleType == "1")
                {
                    ptitle += "-" + season2;

                }

                sSql = "update PDFTAG.dbo.P_inProcess    \n";
                sSql += " set mdate=@mdate \n";
                if (titleType == "1")
                {
                    sSql += " ,ptitle=@ptitle \n";
                }
                sSql += "where pipid=@pipid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", pipid);
                cm.Parameters.AddWithValue("@mdate", sNow);
                cm.Parameters.AddWithValue("@ptitle", ptitle);
                cm.ExecuteNonQuery();
            }

            #endregion



            script = "alert('執行完成!')";
        }
    }

    private void parsePDF_GAP2(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        StringBuilder sbLog = new StringBuilder();

        List<Lu_LearnmgrItemDto> arrLu_LearnmgrItemDto = new List<Lu_LearnmgrItemDto>();


        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {

            sSql = "select * from PDFTAG.dbo.P_inProcess where pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            //sSql = "select * from PDftag.dbo.Lu_LearnmgrItem  \n";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //DataTable dtLu_LearnmgrItem = new DataTable();
            //using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            //{
            //    da.Fill(dtLu_LearnmgrItem);
            //}

            string titleType = dt.Rows[0]["titleType"].ToString();
            string sPDFPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString());
            //string sSaveTxtPath = Server.MapPath("~/PdfToText/" + Path.GetFileNameWithoutExtension(dt.Rows[0]["piuploadfile"].ToString()) + ".txt");
            string sSaveTxtPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", ".txt"));
            string sSaveTxtPath3 = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", "_3.txt"));

            //if (parse == "1" || !System.IO.File.Exists(sSaveTxtPath))
            //	ConvertPDFToText2(sPDFPath, sSaveTxtPath);

            //if (!System.IO.File.Exists(sSaveTxtPath))
            //{
            //	script = "alert('轉換失敗')";
            //	return;
            //}


            #region Parse Text for GAP

            string sLastLine = "";
            string sLine = "";
            string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            bool isHeader = false;
            bool isBom = false;
            bool isBomTypeData = false;
            bool isBomGarmentColor = false;
            string sBomType = "";
            List<int> arrBomGarmentColorStartIdx = new List<int>();
            bool isSizeTable = false;


            int iRow = 1;
            int iLineRow = 0;
            long lubcid = 0;
            int iSizeTableArea = 0;

            using (StreamReader data = new StreamReader(sSaveTxtPath3))
            {
                while (!data.EndOfStream)
                {

                    sLine = data.ReadLine();
                    iLineRow++;

                    if (string.IsNullOrEmpty(sLine))
                        continue;

                    if (sLine.StartsWith("@Row: Product"))
                    {
                        isBom = true;
                        isSizeTable = false;
                    }
                    else if (sLine.StartsWith("@Row: POM Name %%") || sLine.StartsWith("@Row: POMName %%"))
                    {
                        isBom = false;
                        isSizeTable = true;
                        iSizeTableArea++;

                        //20230117尺寸表會有2份，要抓後面的為主
                        if (iSizeTableArea <= 2)
                            isSizeTable = false;

                        //LogFile.Logger.Log("iSizeTableArea=" + iSizeTableArea + " isSizeTable=" + isSizeTable + " iLineRow=" + iLineRow);
                    }


                    if (isBom)
                    {
                        #region isBom

                        //@Row: Fabric (1) %%
                        //if (sLine.StartsWith("@Row: Fabric") || sLine.StartsWith("@Row: Trim") || sLine.StartsWith("@Row: Embellishment")
                        //        || sLine.StartsWith("@Row: Thread") || sLine.StartsWith("@Row: Packaging and Labels") || sLine.StartsWith("@Row: Hangtag/Packaging"))
                        {

                            //sBomType = sLine.Replace("@Row:", "").Split(new string[] { "(" }, StringSplitOptions.None)[0].Trim();
                            //isBomTypeData = true;

                            #region Bom Type Data



                            bool isHasOnnlyforProductColors = false;
                            List<string> arrBomColorHeaders = new List<string>();
                            List<GAP_BomDto> arrRowDatas = new List<GAP_BomDto>();

                            iRow = 1;
                            //while (iRow < 100)
                            for (int i = 1; i <= 100; i++)
                            {
                                sLastLine = sLine;
                                if (i > 1)
                                    sLine = data.ReadLine();
                                iLineRow++;

                                if (string.IsNullOrEmpty(sLine))
                                    continue;

                                if (sLine.StartsWith("@Row: POM Name %%") || sLine.StartsWith("@Row: POMName %%"))
                                {
                                    //isSizeTable = true;
                                    //20230117尺寸表會有2份，要抓後面的為主
                                    isBom = false;
                                    iSizeTableArea++;
                                    break;
                                }


                                //LogFile.Logger.Log("sLine.StartsWith(@Row: Product)=" + sLine.StartsWith("@Row: Product") + " Any=" + !arrBomColorHeaders.Any() + " sLine=" + sLine);

                                if (sLine.StartsWith("@Row: Product"))
                                {
                                    isHasOnnlyforProductColors = false;
                                    arrBomColorHeaders = new List<string>();
                                    string[] arrHeaders = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None).ToArray();

                                    bool isStartColor = false;
                                    foreach (var header in arrHeaders)
                                    {
                                        if (isStartColor && header.Trim() != "Comment" && header.Trim() != "")
                                        {
                                            arrBomColorHeaders.Add(header);
                                            continue;
                                        }

                                        if (!isStartColor)
                                        {
                                            if (header.Replace(" ", "").ToLower().Contains("onlyforproductcolors"))
                                            {
                                                isHasOnnlyforProductColors = true;
                                                isStartColor = true;
                                            }
                                            else
                                            {
                                                //@Row: Product %% Material Name %% SupplierArticleNumber %% Gauge/Ends %% Stitch %% CommonSize %% CommonQty %% Usage %% NIGHTFALLPURPLE(COLORO 136- 35- 07) - 000002795535 %% Comment %% 

                                                if (!sLine.Replace(" ", "").ToLower().Contains("onlyforproductcolors") && sLine.Contains("%% Comment %%") && header.Replace(" ", "").ToLower() == "usage")
                                                {
                                                    isStartColor = true;
                                                }

                                            }

                                        }
                                    }

                                    #region GAP_BOMGarmentcolor

                                    if (arrBomColorHeaders.Count() == 0) continue;//若第二頁沒有色組，則不新增
                                    sSql = @"insert into PDFTAG.dbo.GAP_BOMGarmentcolor  
(pipid,luhid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10) 
values 
(@pipid,@luhid,@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9,@A10); SELECT SCOPE_IDENTITY();";

                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();
                                    cm.Parameters.AddWithValue("@pipid", pipid);
                                    cm.Parameters.AddWithValue("@luhid", 0);

                                    for (int a = 1; a <= 10; a++)
                                    {
                                        if (a > arrBomColorHeaders.Count())
                                            cm.Parameters.AddWithValue("@A" + a, "");
                                        else
                                        {
                                            cm.Parameters.AddWithValue("@A" + a, arrBomColorHeaders[a - 1]);
                                        }
                                    }

                                    lubcid = Convert.ToInt64(cm.ExecuteScalar().ToString());
                                    Response.Write("<!--lubcid=" + lubcid + "-->");

                                    #endregion

                                    continue;
                                }


                                if (sLine.StartsWith("@Row: Fabric") || sLine.StartsWith("@Row: Trim") || sLine.StartsWith("@Row: Embellishment")
                                    || sLine.StartsWith("@Row: Thread") || sLine.StartsWith("@Row: Packaging and Labels") || sLine.StartsWith("@Row: Hangtag/Packaging"))
                                {
                                    //BomType
                                    sBomType = sLine.Replace("@Row:", "").Split(new string[] { "(" }, StringSplitOptions.None)[0].Trim();
                                    continue;
                                }

                                //@Row: Product %% Material Name %% SupplierArticleNumber %% Gauge/Ends %% Stitch %% CommonSize %% CommonQty %% Usage %% Image %% PrimaryRD %% Quality Details %% Supplier[Allocate] %% UOM %% OnlyforProductColors
                                string[] arrParts = sLine.Trim().Replace("@Row:", "").Split(new string[] { "%%" }, StringSplitOptions.None);

                                string sProduct = arrParts[0].Trim();
                                int iProduct = 0;
                                if (!int.TryParse(sProduct, out iProduct))
                                    continue;

                                int iColorLength = arrBomColorHeaders.Count;
                                if (iColorLength == 0) continue;
                                try
                                {

                                    if (isHasOnnlyforProductColors)
                                    {
                                        arrRowDatas.Add(new GAP_BomDto()
                                        {
                                            Type = sBomType,
                                            rowid = iRow,
                                            StandardPlacement = sProduct,
                                            SupplierArticle = arrParts[2].Trim().Replace(" ", string.Empty),
                                            Usage = arrParts[7].Trim(),
                                            QualityDetails = arrParts[10].Trim(),
                                            Supplier = arrParts[11].Trim(),
                                            lubcid = lubcid,
                                            B1 = iColorLength >= 1 ? arrParts[14].Trim() : "",
                                            B2 = iColorLength >= 2 ? arrParts[15].Trim() : "",
                                            B3 = iColorLength >= 3 ? arrParts[16].Trim() : "",
                                            B4 = iColorLength >= 4 ? arrParts[17].Trim() : "",
                                            B5 = iColorLength >= 5 ? arrParts[18].Trim() : "",
                                            B6 = iColorLength >= 6 ? arrParts[19].Trim() : "",
                                            B7 = iColorLength >= 7 ? arrParts[20].Trim() : "",
                                            B8 = iColorLength >= 8 ? arrParts[21].Trim() : "",
                                            B9 = iColorLength >= 9 ? arrParts[22].Trim() : "",
                                            B10 = iColorLength >= 10 ? arrParts[23].Trim() : "",

                                        });
                                    }
                                    else
                                    {
                                        //太長換Table 找出原對應的Type

                                        string sOrgType = sBomType;
                                        string sQualityDetails = "";
                                        string sSupplier = "";

                                        var resProduct = arrRowDatas.FirstOrDefault(x => x.StandardPlacement == sProduct);
                                        if (resProduct != null)
                                        {
                                            sOrgType = resProduct.Type;
                                            sQualityDetails = resProduct.QualityDetails;
                                            sSupplier = resProduct.Supplier;
                                        }

                                        arrRowDatas.Add(new GAP_BomDto()
                                        {
                                            Type = sOrgType,
                                            rowid = iRow,
                                            StandardPlacement = sProduct,
                                            SupplierArticle = arrParts[2].Trim(),
                                            Usage = arrParts[7].Trim(),
                                            QualityDetails = sQualityDetails,
                                            Supplier = sSupplier,
                                            lubcid = lubcid,
                                            B1 = iColorLength >= 1 ? arrParts[8].Trim() : "",
                                            B2 = iColorLength >= 2 ? arrParts[9].Trim() : "",
                                            B3 = iColorLength >= 3 ? arrParts[10].Trim() : "",
                                            B4 = iColorLength >= 4 ? arrParts[11].Trim() : "",
                                            B5 = iColorLength >= 5 ? arrParts[12].Trim() : "",
                                            B6 = iColorLength >= 6 ? arrParts[13].Trim() : "",
                                            B7 = iColorLength >= 7 ? arrParts[14].Trim() : "",
                                            B8 = iColorLength >= 8 ? arrParts[15].Trim() : "",
                                            B9 = iColorLength >= 9 ? arrParts[16].Trim() : "",
                                            B10 = iColorLength >= 10 ? arrParts[17].Trim() : "",

                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogFile.Logger.LogException(ex, "iLineRow=" + iLineRow + " iColorLength=" + iColorLength + " Line=" + sLine);
                                    throw ex;
                                }

                                iRow++;
                            }



                            iRow = 1;
                            foreach (var item in arrRowDatas)
                            {

                                sSql = @"insert into  PDFTAG.dbo.GAP_BOM
(pipid,luhid,lubcid,type,rowid,StandardPlacement,Usage,QualityDetails,SupplierArticle,Supplier,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,isEdit) 
values 
(@pipid,@luhid,@lubcid,@type,@rowid,@StandardPlacement,@Usage,@QualityDetails,@SupplierArticle,@Supplier,@B1,@B2,@B3,@B4,@B5,@B6,@B7,@B8,@B9,@B10,@isEdit); SELECT SCOPE_IDENTITY();";

                                cm.CommandText = sSql;
                                cm.Parameters.Clear();
                                cm.Parameters.AddWithValue("@pipid", pipid);
                                cm.Parameters.AddWithValue("@luhid", 0);
                                cm.Parameters.AddWithValue("@lubcid", item.lubcid);
                                cm.Parameters.AddWithValue("@type", item.Type);
                                cm.Parameters.AddWithValue("@rowid", iRow);
                                cm.Parameters.AddWithValue("@StandardPlacement", item.StandardPlacement);
                                cm.Parameters.AddWithValue("@SupplierArticle", item.SupplierArticle);
                                cm.Parameters.AddWithValue("@Usage", item.Usage);//Usage
                                cm.Parameters.AddWithValue("@QualityDetails", item.QualityDetails);
                                cm.Parameters.AddWithValue("@Supplier", item.Supplier);


                                cm.Parameters.AddWithValue("@B1", item.B1);
                                cm.Parameters.AddWithValue("@B2", item.B2);
                                cm.Parameters.AddWithValue("@B3", item.B3);
                                cm.Parameters.AddWithValue("@B4", item.B4);
                                cm.Parameters.AddWithValue("@B5", item.B5);
                                cm.Parameters.AddWithValue("@B6", item.B6);
                                cm.Parameters.AddWithValue("@B7", item.B7);
                                cm.Parameters.AddWithValue("@B8", item.B8);
                                cm.Parameters.AddWithValue("@B9", item.B9);
                                cm.Parameters.AddWithValue("@B10", item.B10);
                                cm.Parameters.AddWithValue("@isEdit", 0);
                                var lubid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                                iRow++;
                            }




                            #endregion

                        }



                        #endregion
                    }
                    if (isSizeTable)
                    {
                        #region SizeTable Data

                        #region  SizeTableHeader
                        //@Row: POM Name %% Description %% POM Variation %% QC %% TolFraction(- ) %% TolFraction(+) %% ToleranceMessage %% Grading OverrideMessage %% XXS %% XS %% S %% M %% L %% XL %% 
                        //@Row: POM Name %% Description %% POM Variation %% TolFraction(- ) %% TolFraction(+) %% ToleranceMessage %% Grading OverrideMessage %% XXS %% XS %% S %% M %% L %% XL %%
                        if (string.IsNullOrEmpty(sLine))
                            continue;

                        List<string> arrSizes = new List<string>() { "0", "2", "4", "6", "8", "10", "12", "14", "16", "18", "20", "XXXS", "XXS", "XS", "S", "M", "L", "XL", "XXL", "1X", "2X", "3X", "3XL", "4XL", "5XL" };
                        List<string> arrSizeHeaders = new List<string>();
                        string[] arrHeaders = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None).ToArray();

                        int iStartSizeIdx = 0;
                        //foreach (var header in arrHeaders)
                        for (int i = 0; i < arrHeaders.Length; i++)
                        {
                            if (arrSizes.Contains(arrHeaders[i].Trim().ToUpper()))
                            {
                                arrSizeHeaders.Add(arrHeaders[i]);
                                if (iStartSizeIdx == 0)
                                    iStartSizeIdx = i;
                            }
                        }

                        //20230117 尺寸表..若標題=Evaluation 則不用抓
                        if (sLine.Contains("%% QC %%") || sLine.Contains("Measurement Chart Review %%"))
                        {
                            isSizeTable = false;
                            continue;
                        }

                        int iSizeHeaderLength = arrSizeHeaders.Count;

                        sSql = @"insert into PDFTAG.dbo.GAP_SizeTable_Header 
(pipid,HeaderCount,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15) 
values 
(@pipid,@HeaderCount,@H1,@H2,@H3,@H4,@H5,@H6,@H7,@H8,@H9,@H10,@H11,@H12,@H13,@H14,@H15); SELECT SCOPE_IDENTITY();";

                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@pipid", pipid);
                        cm.Parameters.AddWithValue("@HeaderCount", iSizeHeaderLength);
                        for (int a = 1; a <= 15; a++)
                        {
                            if (a > arrSizeHeaders.Count())
                                cm.Parameters.AddWithValue("@H" + a, "");
                            else
                            {
                                cm.Parameters.AddWithValue("@H" + a, arrSizeHeaders[a - 1]);
                            }
                        }


                        long lusthid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                        #endregion

                        iRow = 1;


                        List<GAP_SizeTableDto> arrP_SizeTableDtos = new List<GAP_SizeTableDto>();
                        //while (iRow < 100)
                        for (int i = 1; i <= 100; i++)
                        {
                            sLine = data.ReadLine();
                            iLineRow++;
                            iRow++;
                            sLastLine = sLine;

                            if (string.IsNullOrEmpty(sLine))
                                continue;

                            //if (sLine.StartsWith("@Row: POM Name %%") || sLine.StartsWith("@Row: POMName %%"))
                            if (sLine.StartsWith("@Row: Displaying"))
                            {
                                isSizeTable = false;
                                break;
                            }

                            string[] arrParts = sLine.Trim().Replace("@Row:", "").Split(new string[] { "%%" }, StringSplitOptions.None);


                            if (arrParts.Length < 5)
                            {
                                if (sLine.StartsWith("@Row: "))
                                {

                                    string sPomNote = sLine.Trim().Replace("@Row:", "").Replace("%%", "").Replace("", "").Trim();
                                    if (arrP_SizeTableDtos.Count == 0)//跨頁第一筆是Note的情況
                                    {
                                        sSql = "update GAP_SizeTable set POMNote = '" + sPomNote + "' where lustid = (select top 1 lustid from GAP_SizeTable order by lustid desc)";
                                    }
                                    else arrP_SizeTableDtos[arrP_SizeTableDtos.Count - 1].POMNote = sPomNote;

                                    //LogFile.Logger.Log(" iLineRow=" + iLineRow + " sPomNote="+ sPomNote + " sLine=" + sLine);
                                }
                                continue;
                            }


                            int iSizeLength = arrSizeHeaders.Count;

                            arrP_SizeTableDtos.Add(new GAP_SizeTableDto()
                            {
                                rowid = iRow,
                                POM = arrParts[0].Trim(),
                                POMNote = "",
                                Description = arrParts[1].Trim(),
                                Variation = arrParts[2].Trim(),
                                //QC = arrParts[3].Trim(),
                                QC = "",
                                TolA = arrParts[3].Trim(),
                                TolB = arrParts[4].Trim(),

                                A1 = iSizeLength >= 1 ? arrParts[iStartSizeIdx].Trim() : "",
                                A2 = iSizeLength >= 2 ? arrParts[iStartSizeIdx + 1].Trim() : "",
                                A3 = iSizeLength >= 3 ? arrParts[iStartSizeIdx + 2].Trim() : "",
                                A4 = iSizeLength >= 4 ? arrParts[iStartSizeIdx + 3].Trim() : "",
                                A5 = iSizeLength >= 5 ? arrParts[iStartSizeIdx + 4].Trim() : "",
                                A6 = iSizeLength >= 6 ? arrParts[iStartSizeIdx + 5].Trim() : "",
                                A7 = iSizeLength >= 7 ? arrParts[iStartSizeIdx + 6].Trim() : "",
                                A8 = iSizeLength >= 8 ? arrParts[iStartSizeIdx + 7].Trim() : "",
                                A9 = iSizeLength >= 9 ? arrParts[iStartSizeIdx + 8].Trim() : "",
                                A10 = iSizeLength >= 10 ? arrParts[iStartSizeIdx + 9].Trim() : "",
                                A11 = iSizeLength >= 11 ? arrParts[iStartSizeIdx + 10].Trim() : "",
                                A12 = iSizeLength >= 12 ? arrParts[iStartSizeIdx + 11].Trim() : "",
                                A13 = iSizeLength >= 13 ? arrParts[iStartSizeIdx + 12].Trim() : "",
                                A14 = iSizeLength >= 14 ? arrParts[iStartSizeIdx + 13].Trim() : "",
                                A15 = iSizeLength >= 15 ? arrParts[iStartSizeIdx + 14].Trim() : "",

                            });



                        }

                        #region Insert PDFTAG.dbo.GAP_SizeTable


                        foreach (var item in arrP_SizeTableDtos)
                        {
                            sSql = @"insert into PDFTAG.dbo.GAP_SizeTable 
(pipid,luhid,rowid,POM,POMNote,Description,AddlComments,Variation,QC,TolA,TolB,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,isEdit) 
values 
(@pipid,@luhid,@rowid,@POM,@POMNote,@Description,@AddlComments,@Variation,@QC,@TolA,@TolB,@lusthid,@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9,@A10,@A11,@A12,@A13,@A14,@A15,@isEdit)  SELECT SCOPE_IDENTITY();";

                            cm.CommandText = sSql;
                            cm.Parameters.Clear();
                            cm.Parameters.AddWithValue("@pipid", pipid);
                            cm.Parameters.AddWithValue("@luhid", 0);
                            cm.Parameters.AddWithValue("@rowid", item.rowid);
                            cm.Parameters.AddWithValue("@POM", item.POM);
                            cm.Parameters.AddWithValue("@POMNote", item.POMNote);
                            cm.Parameters.AddWithValue("@Description", item.Description);
                            cm.Parameters.AddWithValue("@AddlComments", "");
                            cm.Parameters.AddWithValue("@Variation", item.Variation);
                            cm.Parameters.AddWithValue("@QC", item.QC);
                            cm.Parameters.AddWithValue("@TolA", item.TolA);
                            cm.Parameters.AddWithValue("@TolB", item.TolB);

                            cm.Parameters.AddWithValue("@lusthid", lusthid);
                            cm.Parameters.AddWithValue("@A1", item.A1);
                            cm.Parameters.AddWithValue("@A2", item.A2);
                            cm.Parameters.AddWithValue("@A3", item.A3);
                            cm.Parameters.AddWithValue("@A4", item.A4);
                            cm.Parameters.AddWithValue("@A5", item.A5);
                            cm.Parameters.AddWithValue("@A6", item.A6);
                            cm.Parameters.AddWithValue("@A7", item.A7);
                            cm.Parameters.AddWithValue("@A8", item.A8);
                            cm.Parameters.AddWithValue("@A9", item.A9);
                            cm.Parameters.AddWithValue("@A10", item.A10);
                            cm.Parameters.AddWithValue("@A11", item.A11);
                            cm.Parameters.AddWithValue("@A12", item.A12);
                            cm.Parameters.AddWithValue("@A13", item.A13);
                            cm.Parameters.AddWithValue("@A14", item.A14);
                            cm.Parameters.AddWithValue("@A15", item.A15);

                            cm.Parameters.AddWithValue("@isEdit", 0);
                            cm.ExecuteNonQuery();

                        }


                        #endregion



                        #endregion

                    }


                    iRow++;

                    sLastLine = sLine;
                }


                sSql = "select a.*,b.season,b.style,b.generateddate,c.gmname \n";
                sSql += "from PDFTAG.dbo.P_inProcess a              \n";
                sSql += " left join PDFTAG.dbo.GAP_Header b on a.pipid=b.pipid and b.isshow=0             \n";
                sSql += " left join PDFTAG.dbo.GroupManage c on a.gmid=c.gmid and c.isshow=0             \n";
                sSql += " where 1=1 and a.isshow=0 and a.pipid='" + pipid + "' \n";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string ptitle = dt.Rows[0]["style"].ToString();
                string season2 = dt.Rows[0]["season"].ToString();
                string new_season = "";
                if (titleType == "1")
                {
                    if (ptitle.IndexOf("_") > -1)
                        ptitle = ptitle.Insert(ptitle.IndexOf("_"), string.Format("-{0}", season2));
                    else
                        ptitle += "-" + season2;
                }

                sSql = "update PDFTAG.dbo.P_inProcess    \n";
                sSql += " set mdate=@mdate \n";
                if (titleType == "1")
                {
                    sSql += " ,ptitle=@ptitle \n";
                }
                sSql += "where pipid=@pipid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", pipid);
                cm.Parameters.AddWithValue("@mdate", sNow);
                cm.Parameters.AddWithValue("@ptitle", ptitle);
                cm.ExecuteNonQuery();
            }

            #endregion



            script = "alert('執行完成!')";
        }
    }

    protected string Add_PDF_Manage_002_GAP(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        string new_pipid = "";
        try
        {
            hidpipid.Value = pipid;
            DateTime dtNow = DateTime.Now;
            string sFilePath = "";


            sSql = "select count(*)+1 as cnt \n";
            sSql += "from PDFTAG.dbo.HistoryData a              \n";
            sSql += " where pipid=@pipid \n";



            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);

                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string hisversion = dt.Rows[0]["cnt"].ToString();

                if (hisversion.Length == 1)
                    hisversion = "00" + hisversion;
                else if (hisversion.Length == 2)
                    hisversion = "0" + hisversion;

                hisversion = dtNow.ToString("yyyyMMdd") + "-" + hisversion;


                sSql = @"insert into PDFTAG.dbo.HistoryData 
(pipid,hisversion,editdate,creator,isShow) 
values 
(@pipid,@hisversion,@editdate,@creator,@isShow);SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.Parameters.AddWithValue("@hisversion", hisversion);
                cm.Parameters.AddWithValue("@editdate", dtNow);
                cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                cm.Parameters.AddWithValue("@isShow", "0");

                long hdid = Convert.ToInt64(cm.ExecuteScalar().ToString());


                sSql = @"insert into PDFTAG.dbo.P_inProcess 
(ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,hdid,gmid,unit) 
select ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,'" + hdid + @"' as hdid,gmid,unit
 from PDFTAG.dbo.P_inProcess where pipid=@pipid";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();

                sSql = "select pipid \n";
                sSql += "from PDFTAG.dbo.P_inProcess a              \n";
                sSql += " where hdid=@hdid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@hdid", hdid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                new_pipid = dt.Rows[0]["pipid"].ToString();

                sSql = "select luhid \n";
                sSql += "from PDFTAG.dbo.GAP_Header  a              \n";
                sSql += " where pipid=@pipid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string luhid = dt.Rows[0]["luhid"].ToString();


                sSql = @"insert into PDFTAG.dbo.GAP_Header 
(pipid, season, style, styledesc,ProductDescription, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate,org_luhid,isEdit) 
 select '" + new_pipid + @"'as pipid, season, style, styledesc,ProductDescription, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate, luhid as org_luhid,0 as isEdit
from PDFTAG.dbo.GAP_Header where pipid=@pipid; SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                long new_luhid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                sSql = @"insert into PDFTAG.dbo.GAP_BOMGarmentcolor 
(pipid,luhid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10) 
 select '" + new_pipid + @"'as pipid,'" + new_luhid + @"' as luhid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10
   from PDFTAG.dbo.GAP_BOMGarmentcolor where pipid=@pipid;

insert into PDFTAG.dbo.GAP_BOM 
(pipid,luhid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,QualityDetails,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,org_lubid,lubcid,isEdit) 
 select '" + new_pipid + @"'as pipid,'" + new_luhid + @"' as luhid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,QualityDetails,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,lubid as org_lubid,lubcid,0 as isEdit
  from PDFTAG.dbo.GAP_BOM where pipid=@pipid;

insert into PDFTAG.dbo.GAP_SizeTable 
(pipid,luhid,rowid,POM,POMNote,Description,AddlComments,TolA,TolB,Variation,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,org_lustid,isEdit) 
 select '" + new_pipid + @"'as pipid,'" + new_luhid + @"' as luhid,rowid,POM,POMNote,Description,AddlComments,TolA,TolB,Variation,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15, lustid as org_lustid,0 as isEdit
   from PDFTAG.dbo.GAP_SizeTable where pipid=@pipid;

";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();

            }


        }
        catch (Exception err)
        {
            Response.Write("btnAdd_Click:" + err.ToString());
        }


        return new_pipid;
    }

    protected void LearnGAP(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";


        DateTime dtNow = DateTime.Now;
        string sFilePath = "";

        List<GAP_LearnmgrItemDto> arrGAP_LearnmgrItemDto = new List<GAP_LearnmgrItemDto>();



        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            sSql = "select * \n";
            sSql += "from PDFTAG.dbo.GAP_Header a              \n";
            sSql += " where 1=1 and a.isshow=0   \n";
            sSql += " and a.pipid = '" + pipid + "'   \n";
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            DataTable dtLu_Header = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtLu_Header);
            }

            string style = dtLu_Header.Rows[0]["style"].ToString();


            using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
            {
                sSql = "select * from PDftag.dbo.GAP_LearnmgrItem where style=@style  \n";

                arrGAP_LearnmgrItemDto = cn.Query<GAP_LearnmgrItemDto>(sSql, new { style = style }).ToList();
            }

            #region GAP_BOM

            sSql = "select a.*,b.* \n";
            sSql += "from PDFTAG.dbo.GAP_BOM a              \n";
            sSql += " join PDFTAG.dbo.GAP_BOMGarmentcolor b on a.lubcid=b.lubcid              \n";
            sSql += " where 1=1   \n";
            sSql += " and a.luhid=(select luhid from PDFTAG.dbo.GAP_Header where isshow=0 and pipid='" + pipid + "')   \n";
            Response.Write("<!--" + sSql + "-->");
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            DataTable dtGAP_BOM = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtGAP_BOM);
            }

            foreach (DataRow drBom in dtGAP_BOM.Rows)
            {
                string lubid = drBom["lubid"].ToString();
                string StandardPlacement = drBom["StandardPlacement"].ToString();
                string Usage = drBom["Usage"].ToString();
                string SupplierArticle = drBom["SupplierArticle"].ToString();
                string Supplier = drBom["Supplier"].ToString();

                string B1 = drBom["B1"].ToString();
                string B2 = drBom["B2"].ToString();
                string B3 = drBom["B3"].ToString();
                string B4 = drBom["B4"].ToString();
                string B5 = drBom["B5"].ToString();
                string B6 = drBom["B6"].ToString();
                string B7 = drBom["B7"].ToString();
                string B8 = drBom["B8"].ToString();
                string B9 = drBom["B9"].ToString();
                string B10 = drBom["B10"].ToString();

                string A1 = drBom["A1"].ToString();
                string A2 = drBom["A2"].ToString();
                string A3 = drBom["A3"].ToString();
                string A4 = drBom["A4"].ToString();
                string A5 = drBom["A5"].ToString();
                string A6 = drBom["A6"].ToString();
                string A7 = drBom["A7"].ToString();
                string A8 = drBom["A8"].ToString();
                string A9 = drBom["A9"].ToString();
                string A10 = drBom["A10"].ToString();

                bool isUpdate = false;

                #region 比對詞彙

                var res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "StandardPlacement"
                 && x.Termname_org == StandardPlacement.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    StandardPlacement = res.Termname;
                    InsertGapNote(cm, "lubid", lubid, "StandardPlacement", res.Ctermname);
                    isUpdate = true;
                }

                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Usage"
                 && x.Termname_org == Usage.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    Usage = res.Termname;
                    InsertGapNote(cm, "lubid", lubid, "Usage", res.Ctermname);
                    isUpdate = true;
                }

                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle"
                && x.Termname_org == SupplierArticle.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    SupplierArticle = res.Termname;
                    InsertGapNote(cm, "lubid", lubid, "SupplierArticle", res.Ctermname);
                    isUpdate = true;
                }

                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Supplier"
              && x.Termname_org == Supplier.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    Supplier = res.Termname;
                    InsertGapNote(cm, "lubid", lubid, "Supplier", res.Ctermname);
                    isUpdate = true;
                }

                #region Color
                for (int a = 1; a <= 10; a++)
                {

                    switch (a)
                    {
                        case 1:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                            && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                            && x.ColorName == A1
          && x.Termname_org == B1.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B1 = res.Termname;
                                //Response.Write("<!--btnLearn_Click: lubid=" + lubid + "  " + res.Ctermname + "-->");
                                InsertGapNote(cm, "lubid", lubid, "B1", res.Ctermname);
                                isUpdate = true;
                            }
                            break;

                        case 2:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                             && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                            && x.ColorName == A2
          && x.Termname_org == B2.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B2 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B2", res.Ctermname);
                                isUpdate = true;
                            }
                            break;

                        case 3:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                            && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                            && x.ColorName == A3
          && x.Termname_org == B3.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B3 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B3", res.Ctermname);
                                isUpdate = true;
                            }
                            break;

                        case 4:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                             && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                             && x.ColorName == A4
          && x.Termname_org == B4.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B4 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B4", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 5:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                             && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                             && x.ColorName == A5
          && x.Termname_org == B5.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B5 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B5", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 6:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                             && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                             && x.ColorName == A6
          && x.Termname_org == B6.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B6 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B6", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 7:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                             && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                             && x.ColorName == A7
          && x.Termname_org == B7.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B7 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B7", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 8:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                             && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                             && x.ColorName == A8
          && x.Termname_org == B8.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B8 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B8", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 9:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                             && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                             && x.ColorName == A9
          && x.Termname_org == B9.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B9 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B9", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 10:
                            res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                            && x.ColName == "GarmentColor"
                             && x.SupplierArticleNumber == SupplierArticle
                            && x.Usage == Usage
                             && x.ColorName == A10
          && x.Termname_org == B10.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B10 = res.Termname;
                                InsertGapNote(cm, "lubid", lubid, "B10", res.Ctermname);
                                isUpdate = true;
                            }
                            break;

                    }

                }
                #endregion

                #endregion

                if (isUpdate)
                {
                    sSql = "update PDFTAG.dbo.GAP_BOM \n";
                    sSql += "set  StandardPlacement=@StandardPlacement,Usage=@Usage,SupplierArticle=@SupplierArticle,Supplier=@Supplier           \n";
                    sSql += " ,B1=@B1,B2=@B2,B3=@B3,B4=@B4,B5=@B5,B6=@B6,B7=@B7,B8=@B8,B9=@B9,B10=@B10,isEdit=1           \n";
                    sSql += "where lubid=@lubid\n";
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@lubid", lubid);
                    cm.Parameters.AddWithValue("@StandardPlacement", StandardPlacement);
                    cm.Parameters.AddWithValue("@Usage", Usage);
                    cm.Parameters.AddWithValue("@SupplierArticle", SupplierArticle);
                    cm.Parameters.AddWithValue("@Supplier", Supplier);
                    cm.Parameters.AddWithValue("@B1", B1);
                    cm.Parameters.AddWithValue("@B2", B2);
                    cm.Parameters.AddWithValue("@B3", B3);
                    cm.Parameters.AddWithValue("@B4", B4);
                    cm.Parameters.AddWithValue("@B5", B5);
                    cm.Parameters.AddWithValue("@B6", B6);
                    cm.Parameters.AddWithValue("@B7", B7);
                    cm.Parameters.AddWithValue("@B8", B8);
                    cm.Parameters.AddWithValue("@B9", B9);
                    cm.Parameters.AddWithValue("@B10", B10);
                    cm.ExecuteNonQuery();
                }
            }

            #endregion

            #region GAP_SizeTable

            sSql = "select a.*,b.* \n";
            sSql += "from PDFTAG.dbo.GAP_SizeTable a              \n";
            sSql += " join PDFTAG.dbo.GAP_SizeTable_Header b on a.lusthid=b.lusthid              \n";
            sSql += " where 1=1   \n";
            sSql += " and a.luhid=(select luhid from PDFTAG.dbo.GAP_Header where isshow=0 and pipid='" + pipid + "')   \n";
            Response.Write("<!--" + sSql + "-->");
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            DataTable dtGAP_SizeTable = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtGAP_SizeTable);
            }

            foreach (DataRow drGAP_SizeTable in dtGAP_SizeTable.Rows)
            {
                string lustid = drGAP_SizeTable["lustid"].ToString();
                string POM = drGAP_SizeTable["POM"].ToString();
                string Description = drGAP_SizeTable["Description"].ToString();

                string A1 = drGAP_SizeTable["A1"].ToString();
                string A2 = drGAP_SizeTable["A2"].ToString();
                string A3 = drGAP_SizeTable["A3"].ToString();
                string A4 = drGAP_SizeTable["A4"].ToString();
                string A5 = drGAP_SizeTable["A5"].ToString();
                string A6 = drGAP_SizeTable["A6"].ToString();
                string A7 = drGAP_SizeTable["A7"].ToString();
                string A8 = drGAP_SizeTable["A8"].ToString();
                string A9 = drGAP_SizeTable["A9"].ToString();
                string A10 = drGAP_SizeTable["A10"].ToString();
                string A11 = drGAP_SizeTable["A11"].ToString();
                string A12 = drGAP_SizeTable["A12"].ToString();
                string A13 = drGAP_SizeTable["A13"].ToString();
                string A14 = drGAP_SizeTable["A14"].ToString();
                string A15 = drGAP_SizeTable["A15"].ToString();

                string H1 = drGAP_SizeTable["H1"].ToString();
                string H2 = drGAP_SizeTable["H2"].ToString();
                string H3 = drGAP_SizeTable["H3"].ToString();
                string H4 = drGAP_SizeTable["H4"].ToString();
                string H5 = drGAP_SizeTable["H5"].ToString();
                string H6 = drGAP_SizeTable["H6"].ToString();
                string H7 = drGAP_SizeTable["H7"].ToString();
                string H8 = drGAP_SizeTable["H8"].ToString();
                string H9 = drGAP_SizeTable["H9"].ToString();
                string H10 = drGAP_SizeTable["H10"].ToString();
                string H11 = drGAP_SizeTable["H11"].ToString();
                string H12 = drGAP_SizeTable["H12"].ToString();
                string H13 = drGAP_SizeTable["H13"].ToString();
                string H14 = drGAP_SizeTable["H14"].ToString();
                string H15 = drGAP_SizeTable["H15"].ToString();

                bool isUpdate = false;

                #region 比對詞彙

                var res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "POM"
                              && x.Pom == POM
                              && x.Termname_org == POM.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    POM = res.Termname;
                    InsertGapNote(cm, "lustid", lustid, "POM", res.Ctermname);
                    isUpdate = true;
                }

                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Description"
                 && x.Pom == POM
                 && x.Termname_org == Description.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    Description = res.Termname;
                    InsertGapNote(cm, "lustid", lustid, "Description", res.Ctermname);
                    isUpdate = true;
                }

                #endregion

                if (isUpdate)
                {
                    sSql = "update PDFTAG.dbo.GAP_SizeTable \n";
                    sSql += "set  POM=@POM,Description=@Description,isEdit=1          \n";
                    sSql += "where lustid=@lustid\n";
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@lustid", lustid);
                    cm.Parameters.AddWithValue("@POM", POM);
                    cm.Parameters.AddWithValue("@Description", Description);
                    cm.ExecuteNonQuery();
                }
            }

            #endregion
        }
    }

    public void InsertGapNote(System.Data.SqlClient.SqlCommand cm, string IdName, string lubid, string ColName, string note)
    {
        if (string.IsNullOrEmpty(note))
            return;
        string sSql = @"IF NOT Exists (select * from PDFTAG.dbo.GAP_Ch_Note where IdName=@IdName and id=@id and ColName=@ColName )
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

        cm.CommandText = sSql;
        cm.Parameters.Clear();
        cm.Parameters.AddWithValue("@IdName", IdName);
        cm.Parameters.AddWithValue("@id", lubid);
        cm.Parameters.AddWithValue("@ColName", ColName);
        cm.Parameters.AddWithValue("@note", note);
        cm.Parameters.AddWithValue("@creator", LoginUser.PK);
        cm.Parameters.AddWithValue("@createordate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cm.ExecuteNonQuery();
    }

    #endregion

    #region UA
    protected string parsePDF_UA_BOM_TagData(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dtBOM = new DataTable();
        string sSql = "";
        string new_pipid = "";

        hidpipid.Value = pipid;
        DateTime dtNow = DateTime.Now;
        string sFilePath = "";


        sSql = "select * \n";
        sSql += "from PDFTAG.dbo.UA_BOM a              \n";
        sSql += " where pipid=@pipid \n";



        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            cm.Parameters.AddWithValue("@pipid", pipid);

            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtBOM);
            }

            //sSql = @"truncate table PDFTAG.dbo.UA_TagData ";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //cm.ExecuteNonQuery();

            foreach (DataRow drBom in dtBOM.Rows)
            {

                string lubid = drBom["lubid"].ToString();
                string type = drBom["type"].ToString();
                string supplierArticle = drBom["SupplierArticle"].ToString();
                Dictionary<string, string> ws = new Dictionary<string, string>();

                try
                {
                    if (type.Contains("Fabric"))
                    {
                        //  Fabric描述 / 物料描述 / 廠商 / 廠商料號 / 物料狀態 / 成份
                        string[] arrData = supplierArticle.Split(new string[] { " /" }, StringSplitOptions.None);

                        for (int i = 0; i < arrData.Count(); i++)
                        {
                            if (i == 5 && i < arrData.Length) { ws.Add(string.Format("w{0}", (i + 1)), arrData[i] + " / "); }
                            else if (i > 5 && i < arrData.Length) { ws["w6"] = ws["w6"] + arrData[i] + " / "; }
                            else
                                ws.Add(string.Format("w{0}", (i + 1)), arrData[i].Trim());
                        }
                        ws["w6"] = ws["w6"].Trim().TrimEnd('/');
                    }
                    else
                    {
                        string[] arrData = supplierArticle.Split(new string[] { " /" }, StringSplitOptions.None);
                        for (int i = 0; i < arrData.Count(); i++)
                        {
                            ws.Add(string.Format("w{0}", (i + 1)), arrData[i].Trim());
                            if (i + 1 == arrData.Count()) { ws[string.Format("w{0}", (i + 1))] = ws[string.Format("w{0}", (i + 1))].Replace("UOM:", ""); }
                        }
                    }

                    if (!string.IsNullOrEmpty(ws["w1"]))
                    {
                        sSql = @"insert into PDFTAG.dbo.UA_TagData 
(hdid,type,lubid,tagnum,W1,W2,W3,W4,W5,W6,W7,W8,W9,W10,creator,creatordate) 
values 
(@hdid,@type,@lubid,@tagnum,@W1,@W2,@W3,@W4,@W5,@W6,@W7,@W8,@W9,@W10,@creator,@creatordate) ";

                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@hdid", 0);
                        cm.Parameters.AddWithValue("@type", type);
                        cm.Parameters.AddWithValue("@lubid", lubid);
                        cm.Parameters.AddWithValue("@tagnum", 0);
                        for (int i = 1; i <= 10; i++)
                        {
                            if (i <= ws.Count())
                                cm.Parameters.AddWithValue(string.Format("@W{0}", i), ws[string.Format("w{0}", i)]);
                            else
                                cm.Parameters.AddWithValue(string.Format("@W{0}", i), "");
                        }
                        cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                        cm.Parameters.AddWithValue("@creatordate", dtNow.ToString("yyyy/MM/dd HH:mm:ss"));
                        cm.ExecuteNonQuery();
                    }
                }
                catch (Exception err)
                {
                    Response.Write("<!--[parsePDF_UA_BOM_TagData] lubid=" + lubid + "  " + err.ToString() + "-->");
                }
            }
        }
        return new_pipid;
    }
    public void PaseUA_Header(string pipid)
    {

        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        StringBuilder sbLog = new StringBuilder();


        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {

            sSql = "select * from PDFTAG.dbo.P_inProcess where pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            //sSql = "select * from PDftag.dbo.Lu_LearnmgrItem  \n";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //DataTable dtLu_LearnmgrItem = new DataTable();
            //using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            //{
            //    da.Fill(dtLu_LearnmgrItem);
            //}

            string titleType = dt.Rows[0]["titleType"].ToString();
            string sPDFPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString());
            //string sSaveTxtPath = Server.MapPath("~/PdfToText/" + Path.GetFileNameWithoutExtension(dt.Rows[0]["piuploadfile"].ToString()) + ".txt");
            string sSaveTxtPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", ".txt"));
            string sSaveTxtPath2 = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", "_v2.txt"));

            if (parse == "1" || !System.IO.File.Exists(sSaveTxtPath))
                ConvertPDFToText3_UA(sPDFPath, sSaveTxtPath);

            //為了找BOM Date
            if (parse == "1" || !System.IO.File.Exists(sSaveTxtPath2))
                ConvertPDFToText2(sPDFPath, sSaveTxtPath2);

            if (!System.IO.File.Exists(sSaveTxtPath))
            {
                script = "alert('轉換失敗')";
                return;
            }

            #region Parse Text UA_Header


            UA_HeaderDto ua_HeaderDto = new UA_HeaderDto();


            string sLastLine = "";
            string sLine = "";
            string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            bool isHeader = false;
            bool isHeaderDone = false;
            bool isHeaderMCLastMod = false;
            int iRow = 1;
            int iLineRow = 0;

            string sGenerateddate = "";
            string sHeaderStyleDesc = "";
            using (StreamReader data = new StreamReader(sSaveTxtPath2))
            {
                while (!data.EndOfStream)
                {
                    sLine = data.ReadLine();

                    if (string.IsNullOrEmpty(sHeaderStyleDesc) && sLine.Contains("Lifecycle:"))
                    {

                        string desc = sLine.Split(':')[1].Trim().Replace("Regional Fit", "");
                        sHeaderStyleDesc = desc.Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[1];
                    }
                    else if (sLine.Contains("Copyright") && sLine.Contains("All Rights Reserved"))
                    {
                        sGenerateddate = sLastLine.Split(' ')[0];
                        break;
                    }
                    sLastLine = sLine;
                }
            }

            sLastLine = "";
            using (StreamReader data = new StreamReader(sSaveTxtPath))
            {
                while (!data.EndOfStream)
                {

                    sLine = data.ReadLine();
                    iLineRow++;


                    if (sLine.StartsWith("@Row:") && sLine.Contains("Season:"))
                    {
                        if (string.IsNullOrEmpty(ua_HeaderDto.season))
                        {
                            //@Row: Status: Prototype1363621 - BaseSeason: FW22Lifecycle: CarryoverUA Tech 6in Novelty 2 PackRegional Fit: US %% 
                            string sTempLine = sLine.Replace("@Row:", "").Replace("Season:", "@Row").Replace("Lifecycle:", "@Row");
                            string season = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Trim();
                            if (season == "")
                            {
                                season = dt.Rows[0]["ptitle"].ToString();
                                season = season.Substring(season.Length - 4, 4);
                            }
                            else
                            {
                                string year = season.Substring(season.Length - 2, 2);
                                season = year + season.Replace(year, "");
                            }
                            ua_HeaderDto.season = season;
                        }
                    }
                    if (sLine.StartsWith("@Row: Status:"))
                    {
                        if (string.IsNullOrEmpty(ua_HeaderDto.style))
                        {
                            //@Row: Status: Prototype1363621 - BaseSeason: FW22Lifecycle: CarryoverUA Tech 6in Novelty 2 PackRegional Fit: US %% 
                            //@Row: Status: Pre-Production1361426 - MCSSeason: SS22Lifecycle: CarryoverUA Training Vent 2.0 SSRegional Fit: US %%
                            //@Row: Status: Production1282508Season: FW23Lifecycle: CarryoverO-Series 6in Boxerjock 2pkRegional Fit: US %% 
                            string sTempLine = sLine.Replace("@Row:", "").Replace("Status: Prototype", "@Row").Replace("Status: Pre-Production", "@Row").Replace("Status: Production", "@Row").Replace("Status: Virtual", "@Row").Replace("Status: Development", "@Row").Replace("Season:", "@Row");
                            string style = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Split('-')[0].Trim();
                            if (sTempLine.Contains("Regional Fit: Asia")) isChina = true;

                            sTempLine = sLine.Replace("@Row:", "").Replace("Lifecycle:", "@Row").Replace("Carryover", "").Replace("Regional Fit:", "@Row");
                            string styleDesc = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Trim();

                            ua_HeaderDto.style = style;
                            //ua_HeaderDto.styledesc = styleDesc;
                            ua_HeaderDto.styledesc = sHeaderStyleDesc;
                        }

                    }
                    if (sLine.StartsWith("@Row:") && sLine.Contains("@Row: Product Family"))
                    {
                        if (string.IsNullOrEmpty(ua_HeaderDto.sclass))
                        {
                            //@Row: Product Family %% Other %% Factory/Country
                            string sclass = sLine.Replace("@Row:", "").Split(new string[] { "%%" }, StringSplitOptions.None)[1].Trim();

                            ua_HeaderDto.sclass = sclass;
                        }

                        continue;
                    }
                    if (sLine.StartsWith("@Row:") && sLine.Contains("@Row: Fit Type"))
                    {
                        if (string.IsNullOrEmpty(ua_HeaderDto.FitType))
                        {
                            //@Row: Fit Type / Reference %% Fitted / 1327417 %% Sample Status %% Fit Approved, Hold Sampling %% 
                            string FitType = sLine.Replace("@Row:", "").Split(new string[] { "%%" }, StringSplitOptions.None)[1].Trim().Split('/')[0].Trim();

                            ua_HeaderDto.FitType = FitType;
                        }
                    }
                    if (sLine.StartsWith("@Row:") && (sLine.Contains("%% Sample Status %%") || sLine.Contains("Sample Status %%")))
                    {
                        if (string.IsNullOrEmpty(ua_HeaderDto.stylestatus))
                        {
                            //@Row: Fit Type / Reference %% Fitted / 1327417 %% Sample Status %% Fit Approved, Hold Sampling %% 
                            string sTempLine = sLine.Replace("@Row:", "").Replace("Sample Status", "@Row").Replace("%%", "");
                            string stylestatus = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Trim();

                            ua_HeaderDto.stylestatus = stylestatus;
                        }

                        continue;
                    }
                    if (sLine.StartsWith("@Row:") && sLine.Contains("%% Gear Line %%"))
                    {
                        if (string.IsNullOrEmpty(ua_HeaderDto.GearLine))
                        {
                            //@Row: Calendar %% MFO %% Gear Line %% N/A %% End Use %% Train %% 
                            string GearLine = sLine.Replace("@Row:", "").Split(new string[] { "%%" }, StringSplitOptions.None)[3].Trim();

                            ua_HeaderDto.GearLine = GearLine;
                        }

                        continue;
                    }
                    if (sLine.StartsWith("@Row:") && sLine.Contains("%% Size Scale %%"))
                    {
                        if (string.IsNullOrEmpty(ua_HeaderDto.productsize))
                        {
                            //@Row: Theme %%  %% Size Scale %% XS-5XL %% Sourcing Manager %%  %% 
                            string sTempLine = sLine.Replace("@Row:", "").Replace("Size Scale", "@Row").Replace("Sourcing Manager", "@Row").Replace("%%", "");
                            string productsize = sTempLine.Split(new string[] { "@Row" }, StringSplitOptions.None)[1].Trim();
                            ua_HeaderDto.productsize = productsize;
                        }


                        sSql = "delete PDFTAG.dbo.UA_BOM where pipid=@pipid;";
                        sSql += "delete PDFTAG.dbo.UA_BOMGarmentcolor where pipid=@pipid;";
                        sSql += "delete PDFTAG.dbo.UA_SizeTable where pipid=@pipid;";
                        //sSql += "delete PDFTAG.dbo.UA_SizeTable_1 where pipid=@pipid;";
                        sSql += "delete PDFTAG.dbo.UA_SizeTable_Header where pipid=@pipid;";
                        //sSql += "delete PDFTAG.dbo.UA_SizeTable_Header_1 where pipid=@pipid;";
                        sSql += "delete PDFTAG.dbo.UA_Header where pipid=@pipid;";
                        sSql += @"insert into PDFTAG.dbo.UA_Header 
(pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,FitType,GearLine,creator,createordate,mdate,isEdit) 
values
(@pipid, @season, @style, @styledesc, @productsize, @brand, @dividion, @class, @pod, @stylestatus, @generateddate, @stylesketch,@FitType,@GearLine, @creator, @createordate, @mdate,0); SELECT SCOPE_IDENTITY();";


                        Response.Write("<!--[PaseUA_Header] pipid=" + pipid + " sSql=" + sSql + "-->");
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();

                        cm.Parameters.AddWithValue("@pipid", pipid);
                        cm.Parameters.AddWithValue("@season", ua_HeaderDto.season);
                        cm.Parameters.AddWithValue("@style", ua_HeaderDto.style);
                        cm.Parameters.AddWithValue("@styledesc", ua_HeaderDto.styledesc);
                        cm.Parameters.AddWithValue("@productsize", ua_HeaderDto.productsize);
                        cm.Parameters.AddWithValue("@brand", "");
                        cm.Parameters.AddWithValue("@dividion", "");
                        cm.Parameters.AddWithValue("@class", ua_HeaderDto.sclass);
                        cm.Parameters.AddWithValue("@pod", "");
                        cm.Parameters.AddWithValue("@stylestatus", ua_HeaderDto.stylestatus);
                        cm.Parameters.AddWithValue("@generateddate", sGenerateddate);
                        cm.Parameters.AddWithValue("@stylesketch", "");
                        cm.Parameters.AddWithValue("@FitType", ua_HeaderDto.FitType);
                        cm.Parameters.AddWithValue("@GearLine", ua_HeaderDto.GearLine);
                        cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                        cm.Parameters.AddWithValue("@createordate", sNow);
                        cm.Parameters.AddWithValue("@mdate", sNow);
                        cm.ExecuteNonQuery();
                        break;
                    }



                    iRow++;

                    sLastLine = sLine;
                }
            }



            #endregion
        }
    }
    private void parsePDF_UA(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        StringBuilder sbLog = new StringBuilder();

        List<Lu_LearnmgrItemDto> arrLu_LearnmgrItemDto = new List<Lu_LearnmgrItemDto>();


        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {

            sSql = "select * from PDFTAG.dbo.P_inProcess where pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            //sSql = "select * from PDftag.dbo.Lu_LearnmgrItem  \n";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //DataTable dtLu_LearnmgrItem = new DataTable();
            //using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            //{
            //    da.Fill(dtLu_LearnmgrItem);
            //}

            string titleType = dt.Rows[0]["titleType"].ToString();
            string sPDFPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString());
            //string sSaveTxtPath = Server.MapPath("~/PdfToText/" + Path.GetFileNameWithoutExtension(dt.Rows[0]["piuploadfile"].ToString()) + ".txt");
            string sSaveTxtPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", ".txt")); ;

            if (parse == "1" || !System.IO.File.Exists(sSaveTxtPath))
                ConvertPDFToText3(sPDFPath, sSaveTxtPath);

            if (!System.IO.File.Exists(sSaveTxtPath))
            {
                script = "alert('轉換失敗')";
                return;
            }

            //sSql = "delete PDFTAG.dbo.GAP_BOM where luhid=(select luhid from PDFTAG.dbo.GAP_Header where pipid=@pipid);";
            //sSql += "delete PDFTAG.dbo.GAP_BOMGarmentcolor  where luhid=(select luhid from PDFTAG.dbo.GAP_Header where pipid=@pipid);";
            //sSql += "delete PDFTAG.dbo.Lu_SizeTable where luhid=(select luhid from PDFTAG.dbo.Lu_Header where pipid=@pipid);";
            //sSql += "delete PDFTAG.dbo.Lu_SizeTable_1 where luhid=(select luhid from PDFTAG.dbo.Lu_Header where pipid=@pipid);";
            //sSql += "delete PDFTAG.dbo.Lu_SizeTable_Header where pipid=@pipid;";
            //sSql += "delete PDFTAG.dbo.Lu_SizeTable_Header_1 where pipid=@pipid;";
            //sSql += "delete PDFTAG.dbo.Lu_Header where pipid=@pipid;";

            //sSql = "truncate table PDFTAG.dbo.UA_BOM;";
            //sSql += "truncate table PDFTAG.dbo.UA_BOMGarmentcolor;";
            //sSql = "truncate table PDFTAG.dbo.UA_SizeTable;";
            //sSql += "truncate table PDFTAG.dbo.UA_SizeTable_Header;";

            //cm.CommandText = sSql;
            //cm.Parameters.Clear();
            //cm.Parameters.AddWithValue("@pipid", pipid);
            //cm.ExecuteNonQuery();

            //string sSaveTxtPath = Server.MapPath("~/PDFManage/GAP-549128~92755_RSI_Run_With_It_4.5IN_Short_L~000572126.txt");

            #region Parse Text for UA




            string sLastLine = "";
            string sLine = "";
            string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            bool isBom = false;
            bool isBomTypeData = false;
            bool isBomGarmentColor = false;
            string sBomType = "";
            List<int> arrBomGarmentColorStartIdx = new List<int>();
            bool isSizeTable = false;

            string sample = "";
            int iRow = 1;
            int iLineRow = 0;
            long lubcid = 0;
            string type = "";

            using (StreamReader data = new StreamReader(sSaveTxtPath))
            {
                while (!data.EndOfStream)
                {

                    sLine = data.ReadLine();
                    iLineRow++;

                    if (sLine.Contains("@Row:  %% (SET A:") || sLine.Contains("@Row:  %% (SET B:") || sLine.Contains("@Row:  %% (SET C:"))
                    {
                        isBom = true;
                        isSizeTable = false;
                    }

                    if (sLine.Contains("@Row: Code %%"))
                    {
                        isBom = false;
                        isSizeTable = true;
                    }

                    //抓樣衣尺寸表                        
                    if (sLine.StartsWith("@Row:  %%"))
                    {
                        if (sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None).Where(w => !string.IsNullOrWhiteSpace(w) && !w.Contains("CW#COLORWAY")).ToArray().Count() > 1)
                            sample = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None).Where(w => !string.IsNullOrWhiteSpace(w) && !w.Contains("CW#COLORWAY")).ToArray()[1].ToString().Trim();
                    }

                    if (isBom)
                    {
                        #region isBom

                        sample = "";//清掉

                        if (sLine.Contains("@Row:  %% (SET A:") || sLine.Contains("@Row:  %% (SET B:") || sLine.Contains("@Row:  %% (SET C:"))
                        {
                            string colorSet = sLine.Replace("@Row:  %%", "").Replace("%%", "").Trim();

                            isBomGarmentColor = true;

                            #region BomGarmentColor Header


                            sLine = data.ReadLine();
                            iLineRow++;

                            List<string> arrGarmentColor = new List<string>();
                            List<string> arrDescs = new List<string>();
                            List<string> arrCNumbers = new List<string>();

                            arrDescs = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None).Where(w => !string.IsNullOrWhiteSpace(w) && !w.Contains("@Row: CONTENT")).ToList();


                            sLine = data.ReadLine();
                            iLineRow++;

                            #endregion


                            if (sLine.StartsWith("@Row: Fabric") || sLine.StartsWith("@Row: Trim") || sLine.StartsWith("@Row: Embellishment")
                                || sLine.StartsWith("@Row: Thread") || sLine.StartsWith("@Row: Label") || sLine.StartsWith("@Row: Hangtag/Packaging"))
                            {

                                isBomTypeData = true;

                                #region Bom Type Data

                                //Fabric      Loc       Usage          Qty

                                List<string> arrBomHeaders = new List<string>();
                                List<int> arrBomHeadersStartIdx = new List<int>();
                                List<List<string>> arrLineRowDatas = new List<List<string>>();
                                List<UA_BomDto> arrUA_BomDto = new List<UA_BomDto>();


                                string[] arrHeaders = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();

                                sBomType = arrHeaders[0].Replace("@Row:", "").Trim();

                                string sFirstCN = arrHeaders[4].Trim();

                                bool isStartColor = false;
                                foreach (var header in arrHeaders)
                                {

                                    if (header.Contains(sFirstCN))
                                    {
                                        isStartColor = true;
                                    }
                                    if (isStartColor)
                                        arrCNumbers.Add(header);
                                }



                                int iBomHeadersCount = arrHeaders.Length;


                                iRow = 1;
                                while (iRow < 100)
                                {
                                    sLastLine = sLine;
                                    sLine = data.ReadLine();
                                    iLineRow++;


                                    if (sLine.StartsWith("@Row: " + sFirstCN))
                                    {
                                        arrGarmentColor.AddRange(sLine.Replace("@Row:", "").Trim().Split(new string[] { "%%" }, StringSplitOptions.None).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray());
                                        isBomTypeData = false;
                                        break;
                                    }


                                    if (sLine.StartsWith("@Row: Fabric") || sLine.StartsWith("@Row: Trim") || sLine.StartsWith("@Row: Embellishment")
                                        || sLine.StartsWith("@Row: Thread") || sLine.StartsWith("@Row: Label") || sLine.StartsWith("@Row: Hangtag/Packaging"))
                                    {
                                        //新Type, 相同欄位
                                        sBomType = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray()[0].Replace("@Row:", "").Trim();
                                        continue;
                                    }

                                    string[] arrParts = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None);
                                    int iLength = arrParts.Length;

                                    arrUA_BomDto.Add(new UA_BomDto()
                                    {
                                        Type = sBomType,
                                        rowid = iRow,
                                        SupplierArticle = arrParts[0].Replace("@Row:", "").Trim(),
                                        Usage = arrParts[2].Replace("@Row:", "").Trim(),
                                        QTY = arrParts[3].Replace("@Row:", "").Trim(),
                                        colorSet = colorSet,

                                        B1 = iLength >= 5 ? arrParts[4].Trim() : "",
                                        B2 = iLength >= 6 ? arrParts[5].Trim() : "",
                                        B3 = iLength >= 7 ? arrParts[6].Trim() : "",
                                        B4 = iLength >= 8 ? arrParts[7].Trim() : "",
                                        B5 = iLength >= 9 ? arrParts[8].Trim() : "",
                                        B6 = iLength >= 10 ? arrParts[9].Trim() : "",
                                        B7 = iLength >= 11 ? arrParts[10].Trim() : "",
                                        B8 = iLength >= 12 ? arrParts[11].Trim() : "",
                                        B9 = iLength >= 13 ? arrParts[12].Trim() : "",
                                        B10 = iLength >= 14 ? arrParts[13].Trim() : "",

                                    });

                                    iRow++;
                                }

                                #region Insert PDFTAG.dbo.UA_BOM

                                sSql = @"insert into PDFTAG.dbo.UA_BOMGarmentcolor  
                                (pipid,luhid,HeaderCount,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A1Desc,A2Desc,A3Desc,A4Desc,A5Desc,A6Desc,A7Desc,A8Desc,A9Desc,A10Desc,A1Number,A2Number,A3Number,A4Number,A5Number,A6Number,A7Number,A8Number,A9Number,A10Number) 
                                values 
                                (@pipid,@luhid,@HeaderCount,@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9,@A10,@A1Desc,@A2Desc,@A3Desc,@A4Desc,@A5Desc,@A6Desc,@A7Desc,@A8Desc,@A9Desc,@A10Desc,@A1Number,@A2Number,@A3Number,@A4Number,@A5Number,@A6Number,@A7Number,@A8Number,@A9Number,@A10Number); SELECT SCOPE_IDENTITY();";

                                int iGarmentColor = arrGarmentColor.Count;
                                int iGarmentColorDescs = arrDescs.Count;
                                int iGarmentColorNumbers = arrCNumbers.Count;

                                cm.CommandText = sSql;
                                cm.Parameters.Clear();
                                cm.Parameters.AddWithValue("@pipid", pipid);
                                cm.Parameters.AddWithValue("@luhid", 0);
                                cm.Parameters.AddWithValue("@HeaderCount", iGarmentColor);
                                cm.Parameters.AddWithValue("@A1", iGarmentColor >= 1 ? arrGarmentColor[0].Trim() : "");
                                cm.Parameters.AddWithValue("@A2", iGarmentColor >= 2 ? arrGarmentColor[1].Trim() : "");
                                cm.Parameters.AddWithValue("@A3", iGarmentColor >= 3 ? arrGarmentColor[2].Trim() : "");
                                cm.Parameters.AddWithValue("@A4", iGarmentColor >= 4 ? arrGarmentColor[3].Trim() : "");
                                cm.Parameters.AddWithValue("@A5", iGarmentColor >= 5 ? arrGarmentColor[4].Trim() : "");
                                cm.Parameters.AddWithValue("@A6", iGarmentColor >= 6 ? arrGarmentColor[5].Trim() : "");
                                cm.Parameters.AddWithValue("@A7", iGarmentColor >= 7 ? arrGarmentColor[6].Trim() : "");
                                cm.Parameters.AddWithValue("@A8", iGarmentColor >= 8 ? arrGarmentColor[7].Trim() : "");
                                cm.Parameters.AddWithValue("@A9", iGarmentColor >= 9 ? arrGarmentColor[8].Trim() : "");
                                cm.Parameters.AddWithValue("@A10", iGarmentColor >= 10 ? arrGarmentColor[9].Trim() : "");

                                cm.Parameters.AddWithValue("@A1Desc", iGarmentColorDescs >= 1 ? arrDescs[0].Trim() : "");
                                cm.Parameters.AddWithValue("@A2Desc", iGarmentColorDescs >= 2 ? arrDescs[1].Trim() : "");
                                cm.Parameters.AddWithValue("@A3Desc", iGarmentColorDescs >= 3 ? arrDescs[2].Trim() : "");
                                cm.Parameters.AddWithValue("@A4Desc", iGarmentColorDescs >= 4 ? arrDescs[3].Trim() : "");
                                cm.Parameters.AddWithValue("@A5Desc", iGarmentColorDescs >= 5 ? arrDescs[4].Trim() : "");
                                cm.Parameters.AddWithValue("@A6Desc", iGarmentColorDescs >= 6 ? arrDescs[5].Trim() : "");
                                cm.Parameters.AddWithValue("@A7Desc", iGarmentColorDescs >= 7 ? arrDescs[6].Trim() : "");
                                cm.Parameters.AddWithValue("@A8Desc", iGarmentColorDescs >= 8 ? arrDescs[7].Trim() : "");
                                cm.Parameters.AddWithValue("@A9Desc", iGarmentColorDescs >= 9 ? arrDescs[8].Trim() : "");
                                cm.Parameters.AddWithValue("@A10Desc", iGarmentColorDescs >= 10 ? arrDescs[9].Trim() : "");

                                cm.Parameters.AddWithValue("@A1Number", iGarmentColorNumbers >= 1 ? arrCNumbers[0].Trim() : "");
                                cm.Parameters.AddWithValue("@A2Number", iGarmentColorNumbers >= 2 ? arrCNumbers[1].Trim() : "");
                                cm.Parameters.AddWithValue("@A3Number", iGarmentColorNumbers >= 3 ? arrCNumbers[2].Trim() : "");
                                cm.Parameters.AddWithValue("@A4Number", iGarmentColorNumbers >= 4 ? arrCNumbers[3].Trim() : "");
                                cm.Parameters.AddWithValue("@A5Number", iGarmentColorNumbers >= 5 ? arrCNumbers[4].Trim() : "");
                                cm.Parameters.AddWithValue("@A6Number", iGarmentColorNumbers >= 6 ? arrCNumbers[5].Trim() : "");
                                cm.Parameters.AddWithValue("@A7Number", iGarmentColorNumbers >= 7 ? arrCNumbers[6].Trim() : "");
                                cm.Parameters.AddWithValue("@A8Number", iGarmentColorNumbers >= 8 ? arrCNumbers[7].Trim() : "");
                                cm.Parameters.AddWithValue("@A9Number", iGarmentColorNumbers >= 9 ? arrCNumbers[8].Trim() : "");
                                cm.Parameters.AddWithValue("@A10Number", iGarmentColorNumbers >= 10 ? arrCNumbers[9].Trim() : "");


                                lubcid = Convert.ToInt64(cm.ExecuteScalar().ToString());


                                foreach (var item in arrUA_BomDto)
                                {

                                    sSql = @"insert into  PDFTAG.dbo.UA_BOM
                                                                (pipid,luhid,lubcid,type,rowid,Usage,SupplierArticle,Supplier,QTY,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,isEdit,COLOR_SET) 
                                                                values 
                                                                (@pipid,@luhid,@lubcid,@type,@rowid,@Usage,@SupplierArticle,@Supplier,@QTY,@B1,@B2,@B3,@B4,@B5,@B6,@B7,@B8,@B9,@B10,@isEdit,@COLOR_SET);";

                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();
                                    cm.Parameters.AddWithValue("@pipid", pipid);
                                    cm.Parameters.AddWithValue("@luhid", pipid);
                                    cm.Parameters.AddWithValue("@lubcid", lubcid);
                                    cm.Parameters.AddWithValue("@type", item.Type);
                                    cm.Parameters.AddWithValue("@rowid", item.rowid);
                                    cm.Parameters.AddWithValue("@SupplierArticle", item.SupplierArticle);
                                    cm.Parameters.AddWithValue("@Usage", item.Usage);
                                    cm.Parameters.AddWithValue("@Supplier", "");
                                    cm.Parameters.AddWithValue("@QTY", item.QTY);


                                    cm.Parameters.AddWithValue("@B1", item.B1);
                                    cm.Parameters.AddWithValue("@B2", item.B2);
                                    cm.Parameters.AddWithValue("@B3", item.B3);
                                    cm.Parameters.AddWithValue("@B4", item.B4);
                                    cm.Parameters.AddWithValue("@B5", item.B5);
                                    cm.Parameters.AddWithValue("@B6", item.B6);
                                    cm.Parameters.AddWithValue("@B7", item.B7);
                                    cm.Parameters.AddWithValue("@B8", item.B8);
                                    cm.Parameters.AddWithValue("@B9", item.B9);
                                    cm.Parameters.AddWithValue("@B10", item.B10);

                                    cm.Parameters.AddWithValue("@isEdit", 0);
                                    cm.Parameters.AddWithValue("@COLOR_SET", item.colorSet);
                                    cm.ExecuteNonQuery();
                                }

                                #endregion



                                #endregion

                            }

                        }

                        #endregion
                    }
                    else if (isSizeTable)
                    {
                        #region SizeTable

                        if (sLine.StartsWith("@Row: Code %%"))
                        {
                            #region SizeTable Data

                            // POM        Description              Add'l     Variation    QC       Tol(-)    Tol(+)

                            UA_SizeTableHeaderDto resUA_SizeTableHeaderDto = new UA_SizeTableHeaderDto();
                            List<string> arrSizeTableHeaders = new List<string>();
                            List<string> arrSizeHeaders = new List<string>();
                            List<int> arrSizeTableHeadersStartIdx = new List<int>();
                            List<List<string>> arrLineRowDatas = new List<List<string>>();
                            List<string> arrRowDatas = new List<string>();
                            int iTolAddIdx = 0;
                            long lusthid = 0;
                            bool isStartSizeColumn = false;

                            string[] arrHeaders = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();


                            foreach (var header in arrHeaders)
                            {
                                if (isStartSizeColumn)
                                {
                                    arrSizeHeaders.Add(header);
                                }


                                if (header.Contains("Tol(+)") || (sample != "" && header == arrHeaders[3]))
                                {
                                    iTolAddIdx = sLine.IndexOf(header);
                                    isStartSizeColumn = true;
                                }
                            }


                            //sLine = data.ReadLine();//Header 有2行
                            string sHeaderDesc = "";
                            //if (sLine.Split(new string[] { "%%" }, StringSplitOptions.None).Length < 3)
                            //	sHeaderDesc = sLine.Replace("@Row:", "").Replace("%%", "").Trim();

                            int iSizeHeaderLength = arrSizeHeaders.Count;
                            resUA_SizeTableHeaderDto = new UA_SizeTableHeaderDto()
                            {
                                HeaderCount = iSizeHeaderLength,
                                HeaderDesc = sHeaderDesc,
                                SAMPLE = sample,
                                H1 = iSizeHeaderLength >= 1 ? arrSizeHeaders[0].Trim() : "",
                                H2 = iSizeHeaderLength >= 2 ? arrSizeHeaders[1].Trim() : "",
                                H3 = iSizeHeaderLength >= 3 ? arrSizeHeaders[2].Trim() : "",
                                H4 = iSizeHeaderLength >= 4 ? arrSizeHeaders[3].Trim() : "",
                                H5 = iSizeHeaderLength >= 5 ? arrSizeHeaders[4].Trim() : "",
                                H6 = iSizeHeaderLength >= 6 ? arrSizeHeaders[5].Trim() : "",
                                H7 = iSizeHeaderLength >= 7 ? arrSizeHeaders[6].Trim() : "",
                                H8 = iSizeHeaderLength >= 8 ? arrSizeHeaders[7].Trim() : "",
                                H9 = iSizeHeaderLength >= 9 ? arrSizeHeaders[8].Trim() : "",
                                H10 = iSizeHeaderLength >= 10 ? arrSizeHeaders[9].Trim() : "",
                                H11 = iSizeHeaderLength >= 11 ? arrSizeHeaders[10].Trim() : "",
                                H12 = iSizeHeaderLength >= 12 ? arrSizeHeaders[11].Trim() : "",
                                H13 = iSizeHeaderLength >= 13 ? arrSizeHeaders[12].Trim() : "",
                                H14 = iSizeHeaderLength >= 14 ? arrSizeHeaders[13].Trim() : "",
                                H15 = iSizeHeaderLength >= 15 ? arrSizeHeaders[14].Trim() : "",

                            };
                            sample = "";//清掉

                            //sLine = data.ReadLine();//Header 有2行
                            iLineRow++;

                            iRow = 1;


                            List<UA_SizeTableDto> arrUA_SizeTableDtos = new List<UA_SizeTableDto>();
                            while (iRow < 100)
                            {
                                sLine = data.ReadLine();
                                iLineRow++;

                                sLastLine = sLine;

                                if (string.IsNullOrEmpty(sLine) || sLine.Contains("@Row: Status:"))
                                {
                                    break;
                                }

                                string[] arrParts = sLine.Trim().Split(new string[] { "%%" }, StringSplitOptions.None);
                                int iLength = arrParts.Length;


                                if (arrParts.Length < 3)
                                {
                                    sHeaderDesc = sLine.Replace("@Row:", "").Replace("%%", "").Trim();
                                    resUA_SizeTableHeaderDto.HeaderDesc = sHeaderDesc;
                                    continue;
                                }

                                arrUA_SizeTableDtos.Add(new UA_SizeTableDto()
                                {
                                    rowid = iRow,
                                    Code = arrParts[0].Trim().Replace("@Row:", ""),
                                    Description = arrParts[1].Trim(),
                                    TolA = arrParts[2].Trim(),
                                    TolB = arrParts[3].Trim(),

                                    A1 = iLength >= 5 ? arrParts[4].Trim() : "",
                                    A2 = iLength >= 6 ? arrParts[5].Trim() : "",
                                    A3 = iLength >= 7 ? arrParts[6].Trim() : "",
                                    A4 = iLength >= 8 ? arrParts[7].Trim() : "",
                                    A5 = iLength >= 9 ? arrParts[8].Trim() : "",
                                    A6 = iLength >= 10 ? arrParts[9].Trim() : "",
                                    A7 = iLength >= 11 ? arrParts[10].Trim() : "",
                                    A8 = iLength >= 12 ? arrParts[11].Trim() : "",
                                    A9 = iLength >= 13 ? arrParts[12].Trim() : "",
                                    A10 = iLength >= 14 ? arrParts[13].Trim() : "",
                                    A11 = iLength >= 15 ? arrParts[14].Trim() : "",
                                    A12 = iLength >= 16 ? arrParts[15].Trim() : "",
                                    A13 = iLength >= 17 ? arrParts[16].Trim() : "",
                                    A14 = iLength >= 18 ? arrParts[17].Trim() : "",
                                    A15 = iLength >= 19 ? arrParts[18].Trim() : "",

                                });

                                iRow++;
                            }

                            #region Insert PDFTAG.dbo.UA_SizeTable


                            sSql = @"insert into PDFTAG.dbo.UA_SizeTable_Header 
(pipid,HeaderCount,HeaderDesc,SAMPLE,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15) 
values 
(@pipid,@HeaderCount,@HeaderDesc,@SAMPLE,@H1,@H2,@H3,@H4,@H5,@H6,@H7,@H8,@H9,@H10,@H11,@H12,@H13,@H14,@H15); SELECT SCOPE_IDENTITY();";

                            cm.CommandText = sSql;
                            cm.Parameters.Clear();
                            cm.Parameters.AddWithValue("@pipid", pipid);
                            cm.Parameters.AddWithValue("@HeaderCount", resUA_SizeTableHeaderDto.HeaderCount);
                            cm.Parameters.AddWithValue("@HeaderDesc", resUA_SizeTableHeaderDto.HeaderDesc);
                            cm.Parameters.AddWithValue("@SAMPLE", resUA_SizeTableHeaderDto.SAMPLE);
                            cm.Parameters.AddWithValue("@H1", resUA_SizeTableHeaderDto.H1);
                            cm.Parameters.AddWithValue("@H2", resUA_SizeTableHeaderDto.H2);
                            cm.Parameters.AddWithValue("@H3", resUA_SizeTableHeaderDto.H3);
                            cm.Parameters.AddWithValue("@H4", resUA_SizeTableHeaderDto.H4);
                            cm.Parameters.AddWithValue("@H5", resUA_SizeTableHeaderDto.H5);
                            cm.Parameters.AddWithValue("@H6", resUA_SizeTableHeaderDto.H6);
                            cm.Parameters.AddWithValue("@H7", resUA_SizeTableHeaderDto.H7);
                            cm.Parameters.AddWithValue("@H8", resUA_SizeTableHeaderDto.H8);
                            cm.Parameters.AddWithValue("@H9", resUA_SizeTableHeaderDto.H9);
                            cm.Parameters.AddWithValue("@H10", resUA_SizeTableHeaderDto.H10);
                            cm.Parameters.AddWithValue("@H11", resUA_SizeTableHeaderDto.H11);
                            cm.Parameters.AddWithValue("@H12", resUA_SizeTableHeaderDto.H12);
                            cm.Parameters.AddWithValue("@H13", resUA_SizeTableHeaderDto.H13);
                            cm.Parameters.AddWithValue("@H14", resUA_SizeTableHeaderDto.H14);
                            cm.Parameters.AddWithValue("@H15", resUA_SizeTableHeaderDto.H15);


                            lusthid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                            foreach (var item in arrUA_SizeTableDtos)
                            {
                                sSql = @"insert into PDFTAG.dbo.UA_SizeTable 
(pipid,luhid,rowid,Code,Description,TolA,TolB,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,isEdit) 
values 
(@pipid,@luhid,@rowid,@Code,@Description,@TolA,@TolB,@lusthid,@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9,@A10,@A11,@A12,@A13,@A14,@A15,@isEdit)  SELECT SCOPE_IDENTITY();";

                                cm.CommandText = sSql;
                                cm.Parameters.Clear();
                                cm.Parameters.AddWithValue("@pipid", pipid);
                                cm.Parameters.AddWithValue("@luhid", pipid);
                                cm.Parameters.AddWithValue("@rowid", item.rowid);
                                cm.Parameters.AddWithValue("@Code", item.Code);
                                cm.Parameters.AddWithValue("@Description", item.Description);
                                cm.Parameters.AddWithValue("@TolA", item.TolA);
                                cm.Parameters.AddWithValue("@TolB", item.TolB);

                                cm.Parameters.AddWithValue("@lusthid", lusthid);
                                cm.Parameters.AddWithValue("@A1", item.A1);
                                cm.Parameters.AddWithValue("@A2", item.A2);
                                cm.Parameters.AddWithValue("@A3", item.A3);
                                cm.Parameters.AddWithValue("@A4", item.A4);
                                cm.Parameters.AddWithValue("@A5", item.A5);
                                cm.Parameters.AddWithValue("@A6", item.A6);
                                cm.Parameters.AddWithValue("@A7", item.A7);
                                cm.Parameters.AddWithValue("@A8", item.A8);
                                cm.Parameters.AddWithValue("@A9", item.A9);
                                cm.Parameters.AddWithValue("@A10", item.A10);
                                cm.Parameters.AddWithValue("@A11", item.A11);
                                cm.Parameters.AddWithValue("@A12", item.A12);
                                cm.Parameters.AddWithValue("@A13", item.A13);
                                cm.Parameters.AddWithValue("@A14", item.A14);
                                cm.Parameters.AddWithValue("@A15", item.A15);

                                cm.Parameters.AddWithValue("@isEdit", 0);
                                cm.ExecuteNonQuery();

                            }


                            #endregion



                            #endregion
                        }

                        #endregion

                    }


                    iRow++;

                    sLastLine = sLine;
                }


                sSql = "select a.*,b.season,b.style,b.generateddate,c.gmname \n";
                sSql += "from PDFTAG.dbo.P_inProcess a              \n";
                sSql += " left join PDFTAG.dbo.UA_Header b on a.pipid=b.pipid and b.isshow=0             \n";
                sSql += " left join PDFTAG.dbo.GroupManage c on a.gmid=c.gmid and c.isshow=0             \n";
                sSql += " where 1=1 and a.isshow=0 and a.pipid='" + pipid + "' \n";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string ptitle = dt.Rows[0]["style"].ToString();
                string season2 = dt.Rows[0]["season"].ToString();
                string new_season = "";
                if (titleType == "1")
                {
                    ptitle += "-" + season2;
                    if (isChina) ptitle += "(CN)";
                }

                sSql = "update PDFTAG.dbo.P_inProcess    \n";
                sSql += " set mdate=@mdate \n";
                if (titleType == "1")
                {
                    sSql += " ,ptitle=@ptitle \n";
                }
                sSql += "where pipid=@pipid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", pipid);
                cm.Parameters.AddWithValue("@mdate", sNow);
                cm.Parameters.AddWithValue("@ptitle", ptitle);
                cm.ExecuteNonQuery();
            }

            #endregion



            script = "alert('執行完成!')";
        }
    }

    protected string Add_PDF_Manage_002_UA(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        string new_pipid = "";
        try
        {
            hidpipid.Value = pipid;
            DateTime dtNow = DateTime.Now;
            string sFilePath = "";


            sSql = "select count(*)+1 as cnt \n";
            sSql += "from PDFTAG.dbo.HistoryData a              \n";
            sSql += " where pipid=@pipid \n";



            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);

                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string hisversion = dt.Rows[0]["cnt"].ToString();

                if (hisversion.Length == 1)
                    hisversion = "00" + hisversion;
                else if (hisversion.Length == 2)
                    hisversion = "0" + hisversion;

                hisversion = dtNow.ToString("yyyyMMdd") + "-" + hisversion;


                sSql = @"insert into PDFTAG.dbo.HistoryData 
(pipid,hisversion,editdate,creator,isShow) 
values 
(@pipid,@hisversion,@editdate,@creator,@isShow);SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.Parameters.AddWithValue("@hisversion", hisversion);
                cm.Parameters.AddWithValue("@editdate", dtNow);
                cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                cm.Parameters.AddWithValue("@isShow", "0");

                long hdid = Convert.ToInt64(cm.ExecuteScalar().ToString());


                sSql = @"insert into PDFTAG.dbo.P_inProcess 
(ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,hdid,gmid,unit) 
select ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,'" + hdid + @"' as hdid,gmid,unit
 from PDFTAG.dbo.P_inProcess where pipid=@pipid";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();

                sSql = "select pipid \n";
                sSql += "from PDFTAG.dbo.P_inProcess a              \n";
                sSql += " where hdid=@hdid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@hdid", hdid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                new_pipid = dt.Rows[0]["pipid"].ToString();

                sSql = "select luhid \n";
                sSql += "from PDFTAG.dbo.UA_Header  a              \n";
                sSql += " where pipid=@pipid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string luhid = dt.Rows[0]["luhid"].ToString();


                sSql = @"insert into PDFTAG.dbo.UA_Header 
(pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,FitType,GearLine,creator,createordate,mdate,org_luhid,isEdit) 
 select '" + new_pipid + @"'as pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,FitType,GearLine,creator,createordate,mdate, luhid as org_luhid,0 as isEdit
from PDFTAG.dbo.UA_Header where pipid=@pipid; SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                long new_luhid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                sSql = @"insert into PDFTAG.dbo.UA_BOMGarmentcolor 
(pipid,luhid,HeaderCount,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10) 
 select '" + new_pipid + @"'as pipid,'" + new_luhid + @"' as luhid,HeaderCount,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10
   from PDFTAG.dbo.UA_BOMGarmentcolor where pipid=@pipid;

insert into PDFTAG.dbo.UA_BOM 
(pipid,luhid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,QTY,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,org_lubid,lubcid,isEdit,COLOR_SET) 
 select '" + new_pipid + @"'as pipid,'" + new_luhid + @"' as luhid,type,rowid,StandardPlacement,Usage,SupplierArticle,Supplier,QTY,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,lubid as org_lubid,lubcid,0 as isEdit,COLOR_SET
  from PDFTAG.dbo.UA_BOM where pipid=@pipid;

insert into PDFTAG.dbo.UA_SizeTable 
(pipid,luhid,rowid,Code,Description,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,org_lustid,isEdit) 
 select '" + new_pipid + @"'as pipid,'" + new_luhid + @"' as luhid,rowid,Code,Description,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15, lustid as org_lustid,0 as isEdit
   from PDFTAG.dbo.UA_SizeTable where pipid=@pipid;

";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();

                sSql = @"
insert into PDFTAG.dbo.UA_TagData 
(hdid,type,lubid,tagnum,W1,W2,W3,W4,W5,W6,W7,W8,W9,W10,creator,creatordate) 
 select a.hdid,a.type,b.lubid,a.tagnum,W1,W2,W3,W4,W5,W6,W7,W8,W9,W10,a.creator,a.creatordate
  from PDFTAG.dbo.UA_TagData a
  join PDFTAG.dbo.UA_BOM b on a.lubid=b.org_lubid
where b.pipid=@pipid;
;";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", new_pipid);
                cm.ExecuteNonQuery();

            }


        }
        catch (Exception err)
        {
            Response.Write("btnAdd_Click:" + err.ToString());
        }


        return new_pipid;
    }

    protected void LearnUA(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";


        DateTime dtNow = DateTime.Now;
        string sFilePath = "";

        List<UA_LearnmgrItemDto> arrUA_LearnmgrItemDto = new List<UA_LearnmgrItemDto>();

        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            sSql = "select * \n";
            sSql += "from PDFTAG.dbo.UA_Header a              \n";
            sSql += " where 1=1 and a.isshow=0   \n";
            sSql += " and a.pipid = '" + pipid + "'   \n";
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            DataTable dtLu_Header = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtLu_Header);
            }

            string style = dtLu_Header.Rows[0]["style"].ToString();

            using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
            {
                sSql = "select * from PDftag.dbo.UA_LearnmgrItem where style=@style  \n";

                arrUA_LearnmgrItemDto = cn.Query<UA_LearnmgrItemDto>(sSql, new { style = style }).ToList();
            }

            #region UA_TagData

            sSql = "select c.* \n";
            sSql += "from PDFTAG.dbo.UA_BOM a              \n";
            sSql += " join PDFTAG.dbo.UA_BOMGarmentcolor b on a.lubcid=b.lubcid              \n";
            sSql += " join PDFTAG.dbo.UA_TagData c on a.lubid=c.lubid               \n";
            sSql += " where 1=1   \n";
            sSql += " and a.luhid=(select luhid from PDFTAG.dbo.UA_Header where isshow=0 and pipid='" + pipid + "')   \n";
            Response.Write("<!--" + sSql + "-->");
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            DataTable dtUA_TagData = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtUA_TagData);
            }

            foreach (DataRow drUA_TagData in dtUA_TagData.Rows)
            {
                string lubid = drUA_TagData["lubid"].ToString();
                string utdid = drUA_TagData["utdid"].ToString();

                string sEW = "";


                for (int i = 1; i <= 8; i++)
                {
                    string SubColName = "W" + i;
                    string sW = drUA_TagData[SubColName].ToString();

                    #region 比對詞彙

                    var res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle" && x.SubColName == SubColName
                 && x.Termname_org.Trim() == sW.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        sEW = res.Termname;
                        string sEW_note = res.Ctermname;

                        sSql = "update PDFTAG.dbo.UA_TagData  set E" + SubColName + "=@E" + SubColName + " ";
                        if (!string.IsNullOrEmpty(sEW_note))
                            sSql += " ,E" + SubColName + "_note=@note ";
                        sSql += " where utdid=@utdid";

                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@utdid", utdid);
                        cm.Parameters.AddWithValue("@E" + SubColName, sEW);
                        cm.Parameters.AddWithValue("@note", sEW_note);
                        cm.ExecuteNonQuery();

                        sSql = "update PDFTAG.dbo.UA_BOM \n";
                        sSql += "set  isEdit=1           \n";
                        sSql += "where lubid=@lubid\n";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@lubid", lubid);
                        cm.ExecuteNonQuery();
                    }
                }



                #endregion
            }


            #endregion


            #region UA_BOM

            sSql = "select a.*,b.* \n";
            sSql += ",c.W1              \n";
            sSql += ",c.W2              \n";
            sSql += ",c.W3              \n";
            sSql += ",c.W4              \n";
            sSql += ",c.W5              \n";
            sSql += ",c.W6              \n";
            sSql += ",c.W7              \n";
            sSql += ",c.W8              \n";
            sSql += "from PDFTAG.dbo.UA_BOM a              \n";
            sSql += " join PDFTAG.dbo.UA_BOMGarmentcolor b on a.lubcid=b.lubcid              \n";
            sSql += " left join PDFTAG.dbo.UA_TagData c on a.lubid=c.lubid               \n";
            sSql += " where 1=1   \n";
            sSql += " and a.luhid=(select luhid from PDFTAG.dbo.UA_Header where isshow=0 and pipid='" + pipid + "')   \n";
            Response.Write("<!--" + sSql + "-->");
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            DataTable dtUA_BOM = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtUA_BOM);
            }

            foreach (DataRow drBom in dtUA_BOM.Rows)
            {
                string lubid = drBom["lubid"].ToString();
                string StandardPlacement = drBom["StandardPlacement"].ToString();
                string Usage = drBom["Usage"].ToString();
                string SupplierArticle = drBom["SupplierArticle"].ToString();
                string Supplier = drBom["Supplier"].ToString();
                string W1 = drBom["W1"].ToString();


                string B1 = drBom["B1"].ToString();
                string B2 = drBom["B2"].ToString();
                string B3 = drBom["B3"].ToString();
                string B4 = drBom["B4"].ToString();
                string B5 = drBom["B5"].ToString();
                string B6 = drBom["B6"].ToString();
                string B7 = drBom["B7"].ToString();
                string B8 = drBom["B8"].ToString();
                string B9 = drBom["B9"].ToString();
                string B10 = drBom["B10"].ToString();

                string A1 = drBom["A1"].ToString();
                string A2 = drBom["A2"].ToString();
                string A3 = drBom["A3"].ToString();
                string A4 = drBom["A4"].ToString();
                string A5 = drBom["A5"].ToString();
                string A6 = drBom["A6"].ToString();
                string A7 = drBom["A7"].ToString();
                string A8 = drBom["A8"].ToString();
                string A9 = drBom["A9"].ToString();
                string A10 = drBom["A10"].ToString();

                bool isUpdate = false;

                #region 比對詞彙

                var res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "StandardPlacement"
                 && x.Termname_org == StandardPlacement.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    StandardPlacement = res.Termname;
                    InsertUANote(cm, "lubid", lubid, "StandardPlacement", res.Ctermname);
                    isUpdate = true;
                }

                //res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Placement"
                // && x.Termname_org == Placement.Trim().Replace(" ", "").ToLower());

                //if (res != null)
                //{
                //    Placement = res.Termname;
                //    isUpdate = true;
                //}

                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle"
                && x.Termname_org == SupplierArticle.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    SupplierArticle = res.Termname;
                    InsertUANote(cm, "lubid", lubid, "SupplierArticle", res.Ctermname);
                    isUpdate = true;
                }

                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Supplier"
              && x.Termname_org == Supplier.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    Supplier = res.Termname;
                    InsertUANote(cm, "lubid", lubid, "Supplier", res.Ctermname);
                    isUpdate = true;
                }


                #region Color

                for (int a = 1; a <= 10; a++)
                {
                    switch (a)
                    {
                        case 1:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A1
          && x.Termname_org == B1.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B1 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B1", res.Ctermname);
                                isUpdate = true;
                            }
                            break;

                        case 2:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A2
          && x.Termname_org == B2.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B2 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B2", res.Ctermname);
                                isUpdate = true;
                            }
                            break;

                        case 3:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A3
          && x.Termname_org == B3.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B3 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B3", res.Ctermname);
                                isUpdate = true;
                            }
                            break;

                        case 4:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A4
          && x.Termname_org == B4.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B4 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B4", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 5:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A5
          && x.Termname_org == B5.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B5 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B5", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 6:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A6
          && x.Termname_org == B6.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B6 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B6", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 7:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A7
          && x.Termname_org == B7.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B7 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B7", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 8:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A8
          && x.Termname_org == B8.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B8 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B8", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 9:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A9
          && x.Termname_org == B9.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B9 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B9", res.Ctermname);
                                isUpdate = true;
                            }
                            break;
                        case 10:
                            res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                             && x.Usage == Usage
                        && x.W1 == W1
                        && x.ColorName == A10
          && x.Termname_org == B10.Trim().Replace(" ", "").ToLower());

                            if (res != null)
                            {
                                B10 = res.Termname;
                                InsertUANote(cm, "lubid", lubid, "B10", res.Ctermname);
                                isUpdate = true;
                            }
                            break;

                    }

                }
                #endregion

                #endregion

                if (isUpdate)
                {
                    sSql = "update PDFTAG.dbo.UA_BOM \n";
                    sSql += "set  StandardPlacement=@StandardPlacement,SupplierArticle=@SupplierArticle,Supplier=@Supplier           \n";
                    sSql += " ,B1=@B1,B2=@B2,B3=@B3,B4=@B4,B5=@B5,B6=@B6,B7=@B7,B8=@B8,B9=@B9,B10=@B10,isEdit=1           \n";
                    sSql += "where lubid=@lubid\n";
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@lubid", lubid);
                    cm.Parameters.AddWithValue("@StandardPlacement", StandardPlacement);
                    //cm.Parameters.AddWithValue("@Placement", Placement);
                    cm.Parameters.AddWithValue("@SupplierArticle", SupplierArticle);
                    cm.Parameters.AddWithValue("@Supplier", Supplier);
                    cm.Parameters.AddWithValue("@B1", B1);
                    cm.Parameters.AddWithValue("@B2", B2);
                    cm.Parameters.AddWithValue("@B3", B3);
                    cm.Parameters.AddWithValue("@B4", B4);
                    cm.Parameters.AddWithValue("@B5", B5);
                    cm.Parameters.AddWithValue("@B6", B6);
                    cm.Parameters.AddWithValue("@B7", B7);
                    cm.Parameters.AddWithValue("@B8", B8);
                    cm.Parameters.AddWithValue("@B9", B9);
                    cm.Parameters.AddWithValue("@B10", B10);
                    cm.ExecuteNonQuery();
                }
            }

            #endregion

            #region UA_SizeTable

            sSql = "select a.*,b.* \n";
            sSql += "from PDFTAG.dbo.UA_SizeTable a              \n";
            sSql += " join PDFTAG.dbo.UA_SizeTable_Header b on a.lusthid=b.lusthid              \n";
            sSql += " where 1=1   \n";
            sSql += " and a.luhid=(select luhid from PDFTAG.dbo.UA_Header where isshow=0 and pipid='" + pipid + "')   \n";
            Response.Write("<!--" + sSql + "-->");
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            DataTable dtUA_SizeTable = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dtUA_SizeTable);
            }

            foreach (DataRow drUA_SizeTable in dtUA_SizeTable.Rows)
            {
                string lustid = drUA_SizeTable["lustid"].ToString();
                string Code = drUA_SizeTable["Code"].ToString();
                string Description = drUA_SizeTable["Description"].ToString();
                string Criticality = drUA_SizeTable["Criticality"].ToString();

                string A1 = drUA_SizeTable["A1"].ToString();
                string A2 = drUA_SizeTable["A2"].ToString();
                string A3 = drUA_SizeTable["A3"].ToString();
                string A4 = drUA_SizeTable["A4"].ToString();
                string A5 = drUA_SizeTable["A5"].ToString();
                string A6 = drUA_SizeTable["A6"].ToString();
                string A7 = drUA_SizeTable["A7"].ToString();
                string A8 = drUA_SizeTable["A8"].ToString();
                string A9 = drUA_SizeTable["A9"].ToString();
                string A10 = drUA_SizeTable["A10"].ToString();
                string A11 = drUA_SizeTable["A11"].ToString();
                string A12 = drUA_SizeTable["A12"].ToString();
                string A13 = drUA_SizeTable["A13"].ToString();
                string A14 = drUA_SizeTable["A14"].ToString();
                string A15 = drUA_SizeTable["A15"].ToString();

                string H1 = drUA_SizeTable["H1"].ToString();
                string H2 = drUA_SizeTable["H2"].ToString();
                string H3 = drUA_SizeTable["H3"].ToString();
                string H4 = drUA_SizeTable["H4"].ToString();
                string H5 = drUA_SizeTable["H5"].ToString();
                string H6 = drUA_SizeTable["H6"].ToString();
                string H7 = drUA_SizeTable["H7"].ToString();
                string H8 = drUA_SizeTable["H8"].ToString();
                string H9 = drUA_SizeTable["H9"].ToString();
                string H10 = drUA_SizeTable["H10"].ToString();
                string H11 = drUA_SizeTable["H11"].ToString();
                string H12 = drUA_SizeTable["H12"].ToString();
                string H13 = drUA_SizeTable["H13"].ToString();
                string H14 = drUA_SizeTable["H14"].ToString();
                string H15 = drUA_SizeTable["H15"].ToString();

                bool isUpdate = false;

                #region 比對詞彙

                var res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Description"
                  && x.Code == Code
                              && x.Termname_org == Description.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    Description = res.Termname;
                    InsertUANote(cm, "lustid", lustid, "Description", res.Ctermname);
                    isUpdate = true;
                }

                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Criticality"
                && x.Code == Code
                 && x.Termname_org == Criticality.Trim().Replace(" ", "").ToLower());

                if (res != null)
                {
                    Criticality = res.Termname;
                    isUpdate = true;
                }

                #endregion

                if (isUpdate)
                {
                    sSql = "update PDFTAG.dbo.UA_SizeTable \n";
                    sSql += "set  Description=@Description,Criticality=@Criticality,isEdit=1          \n";
                    sSql += "where lustid=@lustid\n";
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@lustid", lustid);
                    cm.Parameters.AddWithValue("@Description", Description);
                    cm.Parameters.AddWithValue("@Criticality", Criticality);
                    cm.ExecuteNonQuery();
                }
            }

            #endregion
        }

        script = "alert('完成!')";

    }

    public void InsertUANote(System.Data.SqlClient.SqlCommand cm, string IdName, string lubid, string ColName, string note)
    {
        if (string.IsNullOrEmpty(note))
            return;
        string sSql = @"IF NOT Exists (select * from PDFTAG.dbo.UA_Ch_Note where IdName=@IdName and id=@id and ColName=@ColName )
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

        cm.CommandText = sSql;
        cm.Parameters.Clear();
        cm.Parameters.AddWithValue("@IdName", IdName);
        cm.Parameters.AddWithValue("@id", lubid);
        cm.Parameters.AddWithValue("@ColName", ColName);
        cm.Parameters.AddWithValue("@note", note);
        cm.Parameters.AddWithValue("@creator", LoginUser.PK);
        cm.Parameters.AddWithValue("@createordate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cm.ExecuteNonQuery();
    }

    #endregion

    #region PDF_ALL

    private void ParsePDF_All(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        string realStyle = "";
        StringBuilder sbLog = new StringBuilder();

        //List<Lu_LearnmgrItemDto> arrLu_LearnmgrItemDto = new List<Lu_LearnmgrItemDto>();

        //using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        //{
        //    sSql = "select * from PDftag.dbo.Lu_LearnmgrItem  \n";

        //    arrLu_LearnmgrItemDto = cn.Query<Lu_LearnmgrItemDto>(sSql, new { }).ToList();
        //}

        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {

            sSql = "select * from PDFTAG.dbo.P_inProcess where pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            string titleType = dt.Rows[0]["titleType"].ToString();
            string sPDFPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString());
            string sSaveTxtPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString().Replace(".pdf", ".txt")); ;

            #region Parse Text for ON RUNNING
            //            if (dlVersion.SelectedItem.Value == "4")
            //            {
            //                if (parse == "1" || !System.IO.File.Exists(sSaveTxtPath))
            //                ConvertPDFToText3(sPDFPath, sSaveTxtPath);

            //                if (!System.IO.File.Exists(sSaveTxtPath))
            //                {
            //                    script = "alert('轉換失敗')";
            //                    return;
            //                }

            //                string sLine = "";
            //                bool isMaterialColorReport = false;
            //                //bool isLu_BOMGarmentcolor = true;
            //                bool isSizeTable = false;
            //                int iRow = 1;
            //                //int iRowid = 1;
            //                int sRow = 1;
            //                int sizeTablePage = 0;
            //                long onHid = 0;
            //                long onSMid = 0;
            //                long onSHid = 0;
            //                long onSSid = 0;
            //                //string type = "";
            //                string sample = "";
            //                //string sampleStep = "";
            //                //int countHeader = -1;
            //                //int colStandardPlacement = -1;
            //                //int colPlacement = -1;
            //                //int colSupplierArticle = -1;
            //                //int colSupplier = -1;
            //                //List<string> sizeList = new List<string>();
            //                Dictionary<int, string> dicColor = new Dictionary<int, string>();
            //                Dictionary<int, string> dicHeader = new Dictionary<int, string>();
            //                Dictionary<int, string> dicSizeHeader = new Dictionary<int, string>();
            //                Dictionary<long, string> dicSizeID = new Dictionary<long, string>();
            //                //List<string> arrColorNames = new List<string>();
            //                //for (int a = 1; a <= 50; a++)
            //                //    arrColorNames.Add(a.ToString());

            //                #region Header
            //                string sSeason = "";
            //                string sStyle = "";
            //                string sStyleName = "";
            //                string sVerticals = "";
            //                string sGeneratedDate = "";
            //                Dictionary<string, string> dicHeadD = new Dictionary<string, string>();
            //                // Initialize license object
            //                Aspose.Pdf.License lic = new Aspose.Pdf.License();
            //                try
            //                {
            //                    // Set license
            //                    lic.SetLicense(Server.MapPath("~/PDFManage/Aspose.Pdf.lic"));
            //                }
            //                catch (Exception)
            //                {
            //                    // something went wrong
            //                    throw;
            //                }

            //                Document pdf = new Document(sPDFPath);
            //                String extractedText = "";
            //                bool getNextText = false, isGotSeason = false, isGotStyleName = false, isGotStyleNumber = false, isGotVerticals = false;

            //                TextAbsorber textAbsorber = new TextAbsorber();
            //                pdf.Pages.Accept(textAbsorber);
            //                extractedText = textAbsorber.Text;

            //                StringReader strReader = new StringReader(extractedText);
            //                string[] stringSeparators = new string[] { "  " };
            //                List<string> arrCNumbers = new List<string>();
            //                while (true)
            //                {
            //                    if (isGotSeason && isGotStyleName && isGotStyleNumber && isGotVerticals) break;
            //                    sLine = strReader.ReadLine();
            //                    if (sLine.Contains("Season"))
            //                    {
            //                        string[] arrHeaders = sLine.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
            //                        sSeason = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Season")) + 1].Substring(2, 4);
            //                        isGotSeason = true;
            //                    }
            //                    if (sLine.Contains("Style Name"))
            //                    {
            //                        string[] arrHeaders = sLine.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
            //                        sStyleName = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Style Name")) + 1];
            //                        dicHeadD.Add("Style Name", sStyleName);
            //                        isGotStyleName = true;
            //                    }
            //                    if (sLine.Contains("Style Number"))
            //                    {
            //                        string[] arrHeaders = sLine.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
            //                        sStyle = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Style Number")) + 1];
            //                        isGotStyleNumber = true;
            //                    }
            //                    if (sLine.Contains("Verticals"))
            //                    {
            //                        string[] arrHeaders = sLine.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
            //                        sVerticals = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Verticals")) + 1];
            //                        dicHeadD.Add("Verticals", sVerticals);
            //                        isGotVerticals = true;
            //                    }
            //                    if (sLine.Contains("Greenwich Mean Time"))
            //                    {
            //                        string[] arrHeaders = sLine.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
            //                        sGeneratedDate = Array.Find(arrHeaders, x => x.Contains("Greenwich Mean Time"));
            //                        sGeneratedDate = sGeneratedDate.Substring(0, sGeneratedDate.IndexOf("at") - 1);
            //                    }
            //                }

            //                //HEADER_H
            //                sSql = @"INSERT INTO HEADER_H
            //(PIPID,SEASON,STYLE,GENERATEDDATE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
            //VALUES
            //(@PIPID,@SEASON,@STYLE,@GENERATEDDATE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);SELECT SCOPE_IDENTITY();";

            //                cm.CommandText = sSql;
            //                cm.Parameters.Clear();

            //                cm.Parameters.AddWithValue("@PIPID", pipid);
            //                cm.Parameters.AddWithValue("@SEASON", sSeason);
            //                cm.Parameters.AddWithValue("@STYLE", sStyle);
            //                cm.Parameters.AddWithValue("@GENERATEDDATE", sGeneratedDate);
            //                cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
            //                cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
            //                cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
            //                cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
            //                onHid = Convert.ToInt64(cm.ExecuteScalar().ToString());
            //                //cm.ExecuteNonQuery();

            //                //HEADER_D  
            //                foreach (KeyValuePair<string, string> dicItem in dicHeadD)
            //                {
            //                    sSql = @"INSERT INTO HEADER_D
            //(PIPID,H_ID,COLNAME,VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
            //VALUES
            //(@PIPID,@H_ID,@COLNAME,@VALUE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);";

            //                    cm.CommandText = sSql;
            //                    cm.Parameters.Clear();

            //                    cm.Parameters.AddWithValue("@PIPID", pipid);
            //                    cm.Parameters.AddWithValue("@H_ID", onHid);
            //                    cm.Parameters.AddWithValue("@COLNAME", dicItem.Key);
            //                    cm.Parameters.AddWithValue("@VALUE", dicItem.Value);
            //                    cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
            //                    cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
            //                    cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
            //                    cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
            //                    cm.ExecuteNonQuery();
            //                }

            //                //SIZETABLE_M
            //                sSql = @"INSERT INTO SIZETABLE_M
            //(PIPID,UNIT,CREATOR,CREATEDDATE)
            //VALUES
            //(@PIPID,@UNIT,@CREATOR,@CREATEDDATE);SELECT SCOPE_IDENTITY();";
            //                cm.CommandText = sSql;
            //                cm.Parameters.Clear();

            //                cm.Parameters.AddWithValue("@PIPID", pipid);
            //                cm.Parameters.AddWithValue("@UNIT", dlunit.SelectedItem.Value);
            //                cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
            //                cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
            //                onSMid = Convert.ToInt64(cm.ExecuteScalar().ToString());
            //                #endregion


            //                using (StreamReader data = new StreamReader(sSaveTxtPath))
            //                {
            //                    while (!data.EndOfStream)
            //                    {
            //                        sLine = data.ReadLine();

            //                        if (iRow == 1)
            //                        {

            //                        }
            //                        else
            //                        {
            //                            if (sLine.Contains("@Row: Image | Product"))
            //                                isMaterialColorReport = true;
            //                            else if (sLine.Contains("@Row: Dim %% Description"))
            //                            {
            //                                isSizeTable = true;
            //                                sRow = 1;
            //                                sizeTablePage++;
            //                            }
            //                            //else if (sLine.Contains("@Row: Dim %% Linked"))//或許之後會抓樣衣尺寸表，可能可用這判斷
            //                            //{
            //                            //    countHeader++;//找到樣衣尺寸
            //                            //}
            //                            if (sLine.Contains("@Row: Displaying"))
            //                            {
            //                                isMaterialColorReport = false;
            //                                isSizeTable = false;
            //                            }


            //                            if (isMaterialColorReport)
            //                            {
            //                                #region MaterialColorReport



            //                                #endregion
            //                            }
            //                            else if (isSizeTable)
            //                            {
            //                                #region SizeTable

            //                                string[] arrRow = sLine.Split(new string[] { "@Row: " }, StringSplitOptions.None);
            //                                string[] rowItem = arrRow[1].Split(new string[] { " %% " }, StringSplitOptions.None);

            //                                if (sRow == 1 && sizeTablePage == 1)//抓表頭及成衣尺寸
            //                                {

            //                                    for (int i = 0; i < rowItem.Count(); i++)
            //                                    {
            //                                        switch (rowItem[i].ToString().Replace(" ", "").ToUpper())
            //                                        {
            //                                            case "DIM":
            //                                                dicHeader.Add(i, "DIM");
            //                                                break;
            //                                            case "DESCRIPTION":
            //                                                dicHeader.Add(i, "DESCRIPTION");
            //                                                break;
            //                                            case "TOL(-)":
            //                                                dicHeader.Add(i, "TOL(-)");
            //                                                break;
            //                                            case "TOL(+)":
            //                                                dicHeader.Add(i, "TOL(+)");
            //                                                break;
            //                                            case "COMMENT":
            //                                                dicHeader.Add(i, "COMMENT");
            //                                                break;
            //                                            case "UNLINKPOM?":
            //                                                break;
            //                                            default:
            //                                                if (rowItem[i].ToString().Contains("EU"))
            //                                                {
            //                                                    dicSizeHeader.Add(i, rowItem[i].ToString().Substring(2).Trim());
            //                                                }
            //                                                break;
            //                                        }
            //                                    }
            //                                    foreach (KeyValuePair<int, string> dicItem in dicSizeHeader)
            //                                    {
            //                                        sSql = @"INSERT INTO SIZETABLE_S
            //(PIPID,M_ID,SAMPLE,SIZE_,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
            //VALUES
            //(@PIPID,@M_ID,@SAMPLE,@SIZE_,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);SELECT SCOPE_IDENTITY();";
            //                                        cm.CommandText = sSql;
            //                                        cm.Parameters.Clear();

            //                                        cm.Parameters.AddWithValue("@PIPID", pipid);
            //                                        cm.Parameters.AddWithValue("@M_ID", onSMid);
            //                                        cm.Parameters.AddWithValue("@SAMPLE", sample);
            //                                        cm.Parameters.AddWithValue("@SIZE_", dicItem.Value);
            //                                        cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
            //                                        cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
            //                                        cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
            //                                        cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
            //                                        onSSid = Convert.ToInt64(cm.ExecuteScalar().ToString());
            //                                        dicSizeID.Add(onSSid, dicItem.Value);
            //                                    }

            //                                }
            //                                else//各部位長度
            //                                {
            //                                    if (sLine.Contains("@Row: Dim %% Description"))//排除跨頁的表頭
            //                                    {
            //                                        continue;
            //                                    }
            //                                    if (rowItem.Count() > 2)
            //                                    {
            //                                        //insert ON_SIZETABLE_H
            //                                        sSql = @"INSERT INTO SIZETABLE_H
            //(PIPID,M_ID,CODE,DESCRIPTION,TOL_NEGTIVE,TOL_PLUS,COL1,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
            //VALUES
            //(@PIPID,@M_ID,@CODE,@DESCRIPTION,@TOL_NEGTIVE,@TOL_PLUS,@COL1,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);SELECT SCOPE_IDENTITY();";
            //                                        cm.CommandText = sSql;
            //                                        cm.Parameters.Clear();
            //                                        foreach (KeyValuePair<int, string> dicItem in dicHeader)
            //                                        {
            //                                            switch (dicItem.Value)
            //                                            {
            //                                                case "DIM":
            //                                                    cm.Parameters.AddWithValue("@CODE", rowItem[dicItem.Key]);
            //                                                    break;
            //                                                case "DESCRIPTION":
            //                                                    cm.Parameters.AddWithValue("@DESCRIPTION", rowItem[dicItem.Key]);
            //                                                    break;
            //                                                case "TOL(-)":
            //                                                    cm.Parameters.AddWithValue("@TOL_NEGTIVE", rowItem[dicItem.Key]);
            //                                                    break;
            //                                                case "TOL(+)":
            //                                                    cm.Parameters.AddWithValue("@TOL_PLUS", rowItem[dicItem.Key]);
            //                                                    break;
            //                                                case "COMMENT":
            //                                                    cm.Parameters.AddWithValue("@COL1", rowItem[dicItem.Key]);
            //                                                    break;
            //                                                case "UNLINKPOM?":
            //                                                default:
            //                                                    break;
            //                                            }
            //                                        }
            //                                        cm.Parameters.AddWithValue("@PIPID", pipid);
            //                                        cm.Parameters.AddWithValue("@M_ID", onSMid);
            //                                        cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
            //                                        cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
            //                                        cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
            //                                        cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
            //                                        onSHid = Convert.ToInt64(cm.ExecuteScalar().ToString());

            //                                        //insert ON_SIZETABLE_D
            //                                        foreach (KeyValuePair<long, string> dicItem in dicSizeID)
            //                                        {
            //                                            sSql = @"INSERT INTO SIZETABLE_D
            //(PIPID,M_ID,H_ID,S_ID,VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
            //VALUES
            //(@PIPID,@M_ID,@H_ID,@S_ID,@VALUE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE)";
            //                                            cm.CommandText = sSql;
            //                                            cm.Parameters.Clear();
            //                                            //Console.WriteLine(rowItem[dicItem.Key].ToString().Trim());
            //                                            cm.Parameters.AddWithValue("@S_ID", dicItem.Key);
            //                                            cm.Parameters.AddWithValue("@VALUE", rowItem[dicSizeHeader.FirstOrDefault(x => x.Value == dicItem.Value).Key]);
            //                                            cm.Parameters.AddWithValue("@PIPID", pipid);
            //                                            cm.Parameters.AddWithValue("@M_ID", onSMid);
            //                                            cm.Parameters.AddWithValue("@H_ID", onSHid);
            //                                            cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
            //                                            cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
            //                                            cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
            //                                            cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
            //                                            cm.ExecuteNonQuery();
            //                                        }
            //                                    }
            //                                }
            //                                sRow++;
            //                                #endregion
            //                            }
            //                        }
            //                        iRow++;
            //                    }
            //                }

            //                //LogFile.Logger.Log(sbLog.ToString());
            //            }
            #endregion

            #region Parse Text
            //dlVersion.SelectedItem.Value == "4"  -> ON RUNNING
            //dlVersion.SelectedItem.Value == "9"  -> ARC
            //dlVersion.SelectedItem.Value == "10"  -> ARITZIA
            bool isOnRunning = dlVersion.SelectedItem.Value == "4";
            bool isArc = dlVersion.SelectedItem.Value == "9";
            bool isAritzia = dlVersion.SelectedItem.Value == "10";
            bool is511 = dlVersion.SelectedItem.Value == "8";
            //if (dlVersion.SelectedItem.Value == "10")
            {
                bool isMaterialColorReport = false;
                bool isSizeTable = false;
                string sLine = "";
                int sRow = 1;
                int sizeTablePage = 0;
                long hid = 0;
                long sMid = 0;
                long sHid = 0;
                long sSid = 0;
                string sample = "";
                Dictionary<int, string> dicColor = new Dictionary<int, string>();
                Dictionary<int, string> dicHeader = new Dictionary<int, string>();
                Dictionary<int, string> dicSizeHeader = new Dictionary<int, string>();
                Dictionary<long, string> dicSizeID = new Dictionary<long, string>();

                // Initialize license object
                Aspose.Pdf.License license = new Aspose.Pdf.License();
                try
                {
                    // Set license
                    license.SetLicense(Server.MapPath("~/PDFManage/Aspose.Pdf.lic"));
                }
                catch (Exception)
                {
                    // something went wrong
                    throw;
                }

                Document pdfDocument = new Document(sPDFPath);

                StringBuilder sbFinal = new StringBuilder();
                StringBuilder sb = new StringBuilder();
                foreach (var page in pdfDocument.Pages)
                {
                    isMaterialColorReport = false;
                    isSizeTable = false;
                    Aspose.Pdf.Text.TableAbsorber absorber = new Aspose.Pdf.Text.TableAbsorber();
                    absorber.Visit(page);

                    var textAbsorberPure = new TextAbsorber
                    {
                        ExtractionOptions = { FormattingMode = TextExtractionOptions.TextFormattingMode.Pure }
                    };
                    page.Accept(textAbsorberPure);
                    var ext = textAbsorberPure.Text;

                    if (page.Number == 1)
                    {
                        #region Header

                        string sSeason = "";
                        string sStyle = "";
                        string sStyleName = "";
                        string sVerticals = "";
                        string sBrand = "";
                        string sGeneratedDate = "";
                        Dictionary<string, string> dicHeadD = new Dictionary<string, string>();

                        bool isGotSeason = false, isGotStyleName = false, isGotStyleNumber = false, isGotVerticals = false, isGotBrand = false;

                        TextAbsorber textAbsorber = new TextAbsorber();

                        StringReader strReader = new StringReader(ext);
                        string[] stringSeparators = new string[] { "  " };
                        List<string> arrCNumbers = new List<string>();
                        //while (true)
                        while ((isOnRunning && !(isGotSeason && isGotStyleName && isGotStyleNumber && isGotVerticals))
                            || (isAritzia && !(isGotSeason && isGotStyleName && isGotStyleNumber && isGotBrand))
                            || (isArc && !(isGotSeason && isGotStyleName && isGotStyleNumber))
                            || (is511 && !(isGotSeason && isGotStyleName && isGotStyleNumber)))
                        {
                            sLine = strReader.ReadLine();
                            if (sLine == null) break;
                            string[] arrHeaders = sLine.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
                            #region ON RUNNING
                            if (isOnRunning)
                            {
                                if (isGotSeason && isGotStyleName && isGotStyleNumber && isGotVerticals) break;
                                if (sLine.Contains("Season"))
                                {
                                    sSeason = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Season")) + 1].Substring(2, 4);
                                    switch (sSeason.Substring(2))
                                    {
                                        case "SP":
                                            sSeason = sSeason.Replace("SP", "SS");
                                            break;
                                        case "FA":
                                            sSeason = sSeason.Replace("FA", "FW");
                                            break;
                                        default:
                                            break;
                                    }
                                    isGotSeason = true;
                                }
                                if (sLine.Contains("Style Name"))
                                {
                                    sStyleName = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Style Name")) + 1];
                                    dicHeadD.Add("Style Name", sStyleName);
                                    isGotStyleName = true;
                                }
                                if (sLine.Contains("Style Number"))
                                {
                                    sStyle = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Style Number")) + 1];
                                    isGotStyleNumber = true;
                                }
                                if (sLine.Contains("Verticals"))
                                {
                                    sVerticals = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Verticals")) + 1];
                                    dicHeadD.Add("Verticals", sVerticals);
                                    isGotVerticals = true;
                                }
                                if (sLine.Contains("Greenwich Mean Time") || sLine.Contains("Paciﬁc Daylight Time"))
                                {
                                    sGeneratedDate = Array.Find(arrHeaders, x => x.Contains("Greenwich Mean Time") || x.Contains("Paciﬁc Daylight Time"));
                                    sGeneratedDate = sGeneratedDate.Substring(0, sGeneratedDate.IndexOf("at") - 1);
                                }
                            }
                            #endregion
                            #region ARITZIA
                            else if (isAritzia)
                            {
                                if (isGotSeason && isGotStyleName && isGotStyleNumber && isGotBrand) break;
                                if (sLine.Contains("Style Name"))
                                {
                                    sLine = sLine.Replace("Style Name", "").Trim();
                                    sStyleName = sLine;
                                    dicHeadD.Add("Style Name", sStyleName);
                                    isGotStyleName = true;
                                }
                                if (sLine.Contains("Style #"))
                                {
                                    sLine = sLine.Replace("Style #", "").Replace(" ", "");
                                    realStyle = sLine;
                                    sStyle = sLine.Substring(0, sLine.Length - 5);
                                    isGotStyleNumber = true;
                                    sSeason = sLine.Substring(sLine.Length - 4);
                                    string year = sSeason.Substring(2, 2);
                                    string season = sSeason.Substring(0, 2);
                                    sSeason = year + season;
                                    isGotSeason = true;
                                }
                                if (sLine.Contains("Brand"))
                                {
                                    sBrand = sLine.Replace("Brand", "").Trim();
                                    dicHeadD.Add("Brand", sBrand);
                                    isGotBrand = true;
                                }
                                if (sLine.Contains("Greenwich Mean Time") || sLine.Contains("Paciﬁc Daylight Time"))
                                {
                                    sGeneratedDate = Array.Find(arrHeaders, x => x.Contains("Paciﬁc Daylight Time") || x.Contains("Paciﬁc Daylight Time"));
                                    sGeneratedDate = sGeneratedDate.Substring(0, sGeneratedDate.IndexOf("at") - 1);
                                }
                            }
                            #endregion
                            #region  ARC
                            else if (isArc)
                            {
                                if (sLine.Contains("Season"))
                                {
                                    sSeason = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Season")) + 1];
                                    string year = sSeason.Substring(2, 2);
                                    string season = sSeason.Substring(0, 2);
                                    sSeason = year + season;
                                    isGotSeason = true;
                                }
                                if (new Regex("X[0-9]{9}").IsMatch(sLine))
                                {
                                    sStyle = new Regex("X[0-9]{9}").Match(sLine).ToString();
                                    //sStyle = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains(" / X"))];
                                    //sStyle = sStyle.Substring(sStyle.IndexOf(" / X") + 3, 10);
                                    isGotStyleNumber = true;
                                }
                                if (sLine.Contains("Description"))
                                {
                                    sStyleName = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Description")) + 1];
                                    dicHeadD.Add("Description", sStyleName);
                                    isGotStyleName = true;
                                }
                                if (sLine.Contains("Greenwich Mean Time") || sLine.Contains("Paciﬁc Daylight Time"))
                                {
                                    sGeneratedDate = Array.Find(arrHeaders, x => x.Contains("Greenwich Mean Time") || x.Contains("Paciﬁc Daylight Time"));
                                    sGeneratedDate = sGeneratedDate.Substring(0, sGeneratedDate.IndexOf("at") - 1);
                                }
                            }
                            #endregion
                            #region  511
                            else if (is511)
                            {
                                if (sLine.Contains("Season"))
                                {
                                    sSeason = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Season")) + 1];
                                    string year = sSeason.Substring(2, 2);
                                    string season = sSeason.Substring(0, 2);
                                    switch (season)
                                    {
                                        case "SP":
                                            season = season.Replace("SP", "SS");
                                            break;
                                        case "FL":
                                            season = season.Replace("FL", "FW");
                                            break;
                                        default:
                                            break;
                                    }
                                    sSeason = year + season;
                                    isGotSeason = true;
                                }
                                if (sLine.Contains("Product Code"))
                                {
                                    sStyle = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("Product Code")) + 1];
                                    isGotStyleNumber = true;
                                }
                                if (sLine.Contains("System Product Name"))
                                {
                                    sStyleName = arrHeaders[Array.FindIndex(arrHeaders, x => x.Contains("System Product Name")) + 1];
                                    dicHeadD.Add("System Product Name", sStyleName);
                                    isGotStyleName = true;
                                }
                                if (sLine.Contains("Greenwich Mean Time") || sLine.Contains("Paciﬁc Daylight Time"))
                                {
                                    sGeneratedDate = Array.Find(arrHeaders, x => x.Contains("Greenwich Mean Time") || x.Contains("Paciﬁc Daylight Time"));
                                    sGeneratedDate = sGeneratedDate.Substring(0, sGeneratedDate.IndexOf("at") - 1);
                                }
                            }
                            #endregion
                        }

                        //HEADER_H
                        sSql = @"INSERT INTO HEADER_H
(PIPID,SEASON,STYLE,GENERATEDDATE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
VALUES
(@PIPID,@SEASON,@STYLE,@GENERATEDDATE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);SELECT SCOPE_IDENTITY();";

                        cm.CommandText = sSql;
                        cm.Parameters.Clear();

                        cm.Parameters.AddWithValue("@PIPID", pipid);
                        cm.Parameters.AddWithValue("@SEASON", sSeason);
                        cm.Parameters.AddWithValue("@STYLE", sStyle);
                        cm.Parameters.AddWithValue("@GENERATEDDATE", sGeneratedDate);
                        cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                        cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                        cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                        cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                        hid = Convert.ToInt64(cm.ExecuteScalar().ToString());
                        //cm.ExecuteNonQuery();

                        //HEADER_D  
                        foreach (KeyValuePair<string, string> dicItem in dicHeadD)
                        {
                            sSql = @"INSERT INTO HEADER_D
(PIPID,H_ID,COLNAME,VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
VALUES
(@PIPID,@H_ID,@COLNAME,@VALUE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);";

                            cm.CommandText = sSql;
                            cm.Parameters.Clear();

                            cm.Parameters.AddWithValue("@PIPID", pipid);
                            cm.Parameters.AddWithValue("@H_ID", hid);
                            cm.Parameters.AddWithValue("@COLNAME", dicItem.Key);
                            cm.Parameters.AddWithValue("@VALUE", dicItem.Value);
                            cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                            cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                            cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                            cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                            cm.ExecuteNonQuery();
                        }

                        //SIZETABLE_M
                        sSql = @"INSERT INTO SIZETABLE_M
(PIPID,UNIT,CREATOR,CREATEDDATE)
VALUES
(@PIPID,@UNIT,@CREATOR,@CREATEDDATE);SELECT SCOPE_IDENTITY();";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();

                        cm.Parameters.AddWithValue("@PIPID", pipid);
                        cm.Parameters.AddWithValue("@UNIT", dlunit.SelectedItem.Value);
                        cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                        cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                        sMid = Convert.ToInt64(cm.ExecuteScalar().ToString());
                        #endregion
                    }

                    if (isOnRunning)
                    {
                        if (ext.Contains("Placements"))
                        {
                            isMaterialColorReport = true;
                        }
                        if (ext.Contains("Dimensions"))
                        {
                            isSizeTable = true;
                        }
                    }
                    else if (isAritzia)
                    {
                        if (ext.Contains("Multi- level Placements") || ext.Contains("Style BOM"))
                        {
                            isMaterialColorReport = true;
                        }
                        if (ext.Contains("POMs") && !(ext.Contains("Size Chart Review") || ext.Contains("POM Comments") || ext.Contains("Target") || ext.Contains("Meas.")))
                        {
                            isSizeTable = true;
                        }
                    }
                    else if (isArc)
                    {
                        if (ext.Contains("Multi- level Placements"))
                        {
                            isMaterialColorReport = true;
                        }
                        if (ext.Contains("Points of Measure") && !ext.Contains("Size Chart Review"))
                        {
                            isSizeTable = true;
                        }
                    }
                    else if (is511)
                    {
                        if (ext.Contains("Multi- level Placements"))
                        {
                            isMaterialColorReport = true;
                        }
                        if (ext.Contains("POM's") && !ext.Contains("Size Chart Review"))
                        {
                            isSizeTable = true;
                        }
                    }

                    if (!isSizeTable && !isMaterialColorReport) continue;

                    //只將BOM及尺寸表寫入TXT檔
                    foreach (AbsorbedTable table in absorber.TableList)
                    {
                        Console.WriteLine("Table");
                        foreach (AbsorbedRow row in table.RowList)
                        {
                            sb = new StringBuilder();
                            sb.Append("@Row: ");

                            foreach (AbsorbedCell cell in row.CellList)
                            {
                                string cellText = "";
                                foreach (TextFragment fragment in cell.TextFragments)
                                {
                                    foreach (TextSegment seg in fragment.Segments)
                                    {
                                        cellText += seg.Text;
                                    }
                                }
                                sb.Append(cellText + " %% ");
                            }
                            sbFinal.AppendLine(sb.ToString());
                        }
                        sb.AppendLine("----");
                    }
                }

                System.IO.File.WriteAllText(sSaveTxtPath, sbFinal.ToString());

                //read text
                using (StreamReader data = new StreamReader(sSaveTxtPath))
                {
                    while (!data.EndOfStream)
                    {
                        sLine = data.ReadLine();

                        if (isOnRunning)
                        {
                            if (sLine.Contains("@Row: Image %% Product"))
                                isMaterialColorReport = true;
                            else if (sLine.Contains("@Row: Dim %% Description"))
                            {
                                isSizeTable = true;
                                sRow = 1;
                                sizeTablePage++;
                            }
                        }
                        else if (isAritzia)
                        {
                            if (sLine.Contains("@Row: Product %% Placement"))
                                isMaterialColorReport = true;
                            else if (sLine.Contains("@Row: POM Code %% POM Description"))
                            {
                                isSizeTable = true;
                                sRow = 1;
                                sizeTablePage++;
                            }
                        }
                        else if (isArc)
                        {
                            if (sLine.Contains("@Row: Placement %% Description"))
                                isMaterialColorReport = true;
                            else if (sLine.Contains("@Row: POM %% Description"))
                            {
                                isSizeTable = true;
                                sRow = 1;
                                sizeTablePage++;
                            }
                        }
                        else if (is511)
                        {
                            if (sLine.Contains("@Row: Product %% Image"))
                                isMaterialColorReport = true;
                            else if (sLine.Contains("@Row: POMCode %% Dim %% Description"))
                            {
                                isSizeTable = true;
                                sRow = 1;
                                sizeTablePage++;
                            }
                        }
                        //else if (sLine.Contains("@Row: Dim %% Linked"))//或許之後會抓樣衣尺寸表，可能可用這判斷
                        //{
                        //    countHeader++;//找到樣衣尺寸
                        //}
                        if (sLine.Contains("@Row: Displaying"))
                        {
                            isMaterialColorReport = false;
                            isSizeTable = false;
                        }


                        if (isMaterialColorReport)
                        {
                            #region MaterialColorReport



                            #endregion
                        }
                        else if (isSizeTable)
                        {
                            #region SizeTable

                            string[] arrRow = sLine.Split(new string[] { "@Row: " }, StringSplitOptions.None);
                            string[] rowItem = arrRow[1].Split(new string[] { " %% " }, StringSplitOptions.None);

                            if (sRow == 1 && sizeTablePage == 1)//抓表頭及成衣尺寸
                            {
                                for (int i = 0; i < rowItem.Count(); i++)
                                {
                                    switch (rowItem[i].ToString().Replace(" ", "").ToUpper())
                                    {
                                        case "DIM":
                                            if (is511)
                                                dicHeader.Add(i, "COL2");
                                            else
                                                dicHeader.Add(i, "CODE");
                                            break;
                                        case "POMCODE":
                                        case "POM":
                                            dicHeader.Add(i, "CODE");
                                            break;
                                        case "DESCRIPTION":
                                        case "POMDESCRIPTION":
                                            dicHeader.Add(i, "DESCRIPTION");
                                            break;
                                        case "TOL(-)":
                                            dicHeader.Add(i, "TOL(-)");
                                            break;
                                        case "TOL(+)":
                                            dicHeader.Add(i, "TOL(+)");
                                            break;
                                        case "COMMENT":
                                        case "LENGTH":
                                            dicHeader.Add(i, "COL1");
                                            break;
                                        case "PATTERNM’MENT":
                                            dicHeader.Add(i, "COL2");
                                            break;
                                        case "UNLINKPOM?":
                                            break;
                                        default:
                                            if (isOnRunning && rowItem[i].ToString().Contains("EU"))
                                            {
                                                dicSizeHeader.Add(i, rowItem[i].ToString().Substring(2).Trim());
                                            }
                                            else if (!string.IsNullOrEmpty(rowItem[i].ToString()) && new Regex("^[0-9XSML]*$").IsMatch(rowItem[i].ToString()))//isAritzia、isArc、is511
                                            {
                                                dicSizeHeader.Add(i, rowItem[i].ToString().Trim());
                                            }
                                            break;
                                    }
                                }
                                foreach (KeyValuePair<int, string> dicItem in dicSizeHeader)
                                {
                                    sSql = @"INSERT INTO SIZETABLE_S
(PIPID,M_ID,SAMPLE,SIZE_,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
VALUES
(@PIPID,@M_ID,@SAMPLE,@SIZE_,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);SELECT SCOPE_IDENTITY();";
                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();

                                    cm.Parameters.AddWithValue("@PIPID", pipid);
                                    cm.Parameters.AddWithValue("@M_ID", sMid);
                                    cm.Parameters.AddWithValue("@SAMPLE", sample);
                                    cm.Parameters.AddWithValue("@SIZE_", dicItem.Value);
                                    cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                                    cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                                    cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                                    cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                                    sSid = Convert.ToInt64(cm.ExecuteScalar().ToString());
                                    dicSizeID.Add(sSid, dicItem.Value);
                                }

                            }
                            else//各部位長度
                            {
                                if (isOnRunning && sLine.Contains("@Row: Dim %% Description"))//排除跨頁的表頭
                                {
                                    continue;
                                }
                                else if (isAritzia && sLine.Contains("@Row: POM Code %% POM Description"))//排除跨頁的表頭
                                {
                                    continue;
                                }
                                else if (isArc && sLine.Contains("@Row: POM %% Description"))//排除跨頁的表頭
                                {
                                    continue;
                                }
                                else if (is511 && sLine.Contains("@Row: POMCode %% Dim %% Description"))//排除跨頁的表頭
                                {
                                    continue;
                                }
                                if (rowItem.Count() > 2)
                                {
                                    //insert SIZETABLE_H
                                    sSql = @"INSERT INTO SIZETABLE_H
(PIPID,M_ID,CODE,DESCRIPTION,TOL_NEGTIVE,TOL_PLUS";
                                    if (isOnRunning) sSql += ",COL1";
                                    else if (isArc || is511) sSql += ",COL1,COL2";
                                    sSql += @",CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
VALUES
(@PIPID,@M_ID,@CODE,@DESCRIPTION,@TOL_NEGTIVE,@TOL_PLUS";
                                    if (isOnRunning) sSql += ",@COL1";
                                    else if (isArc || is511) sSql += ",@COL1,@COL2";
                                    sSql += ",@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);SELECT SCOPE_IDENTITY();";
                                    cm.CommandText = sSql;
                                    cm.Parameters.Clear();
                                    foreach (KeyValuePair<int, string> dicItem in dicHeader)
                                    {
                                        switch (dicItem.Value)
                                        {
                                            case "CODE":
                                                cm.Parameters.AddWithValue("@CODE", rowItem[dicItem.Key]);
                                                break;
                                            case "DESCRIPTION":
                                                cm.Parameters.AddWithValue("@DESCRIPTION", rowItem[dicItem.Key]);
                                                break;
                                            case "TOL(-)":
                                                cm.Parameters.AddWithValue("@TOL_NEGTIVE", rowItem[dicItem.Key]);
                                                break;
                                            case "TOL(+)":
                                                cm.Parameters.AddWithValue("@TOL_PLUS", rowItem[dicItem.Key]);
                                                break;
                                            case "COL1":
                                                cm.Parameters.AddWithValue("@COL1", rowItem[dicItem.Key]);
                                                break;
                                            case "COL2":
                                                cm.Parameters.AddWithValue("@COL2", rowItem[dicItem.Key]);
                                                break;
                                            case "COL3":
                                                cm.Parameters.AddWithValue("@COL3", rowItem[dicItem.Key]);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    cm.Parameters.AddWithValue("@PIPID", pipid);
                                    cm.Parameters.AddWithValue("@M_ID", sMid);
                                    cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                                    cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                                    cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                                    cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                                    sHid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                                    //insert SIZETABLE_D
                                    foreach (KeyValuePair<long, string> dicItem in dicSizeID)
                                    {
                                        sSql = @"INSERT INTO SIZETABLE_D
(PIPID,M_ID,H_ID,S_ID,VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
VALUES
(@PIPID,@M_ID,@H_ID,@S_ID,@VALUE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE)";
                                        cm.CommandText = sSql;
                                        cm.Parameters.Clear();
                                        cm.Parameters.AddWithValue("@S_ID", dicItem.Key);
                                        cm.Parameters.AddWithValue("@VALUE", rowItem[dicSizeHeader.FirstOrDefault(x => x.Value == dicItem.Value).Key]);
                                        cm.Parameters.AddWithValue("@PIPID", pipid);
                                        cm.Parameters.AddWithValue("@M_ID", sMid);
                                        cm.Parameters.AddWithValue("@H_ID", sHid);
                                        cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                                        cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                                        cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                                        cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                                        cm.ExecuteNonQuery();
                                    }
                                }
                            }
                            sRow++;
                            #endregion
                        }

                    }
                }

            }
            #endregion

            #region set ptitle
            sSql = "select a.*,b.season,b.style,b.generateddate,c.gmname \n";
            sSql += "from PDFTAG.dbo.P_inProcess a              \n";
            sSql += " left join PDFTAG.dbo.Header_H b on a.pipid=b.pipid and b.isshow=0             \n";
            sSql += " left join PDFTAG.dbo.GroupManage c on a.gmid=c.gmid and c.isshow=0             \n";
            sSql += " where 1=1 and a.isshow=0 and a.pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            dt = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            string ptitle = dt.Rows[0]["style"].ToString();
            string season2 = dt.Rows[0]["season"].ToString();
            if (isAritzia) ptitle = realStyle;
            else ptitle = ptitle + "-" + season2;

            sSql = "update PDFTAG.dbo.P_inProcess    \n";
            sSql += " set mdate=@mdate \n";
            if (titleType == "1")
            {
                sSql += " ,ptitle=@ptitle \n";
            }
            sSql += "where pipid=@pipid \n";
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            cm.Parameters.AddWithValue("@pipid", pipid);
            cm.Parameters.AddWithValue("@mdate", sNow);
            cm.Parameters.AddWithValue("@ptitle", ptitle);
            cm.ExecuteNonQuery();

            #endregion

            string new_pipid = Add_PDF_Manage(pipid);

            script = "alert('執行完成!')";
        }
    }

    /// <summary>
    /// 點[執行]後，原始資料塞進DB後，產生編輯的文件
    /// </summary>
    /// <param name="pipid"></param>
    /// <param name="_cust"></param>
    protected string Add_PDF_Manage(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        string new_pipid = "";

        List<string> arrColorNames = new List<string>();

        try
        {
            hidpipid.Value = pipid;
            DateTime dtNow = DateTime.Now;
            string sFilePath = "";

            sSql = "select count(*)+1 as cnt \n";
            sSql += "from PDFTAG.dbo.HistoryData a              \n";
            sSql += " where pipid=@pipid \n";

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);

                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string hisversion = dt.Rows[0]["cnt"].ToString();

                if (hisversion.Length == 1)
                    hisversion = "00" + hisversion;
                else if (hisversion.Length == 2)
                    hisversion = "0" + hisversion;

                hisversion = dtNow.ToString("yyyyMMdd") + "-" + hisversion;


                sSql = @"insert into PDFTAG.dbo.HistoryData 
(pipid,hisversion,editdate,creator,isShow) 
values 
(@pipid,@hisversion,@editdate,@creator,@isShow);SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.Parameters.AddWithValue("@hisversion", hisversion);
                cm.Parameters.AddWithValue("@editdate", dtNow);
                cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                cm.Parameters.AddWithValue("@isShow", "0");

                long hdid = Convert.ToInt64(cm.ExecuteScalar().ToString());


                sSql = @"insert into PDFTAG.dbo.P_inProcess 
(ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,hdid,gmid,unit) 
select ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,'" + hdid + @"' as hdid,gmid,unit
 from PDFTAG.dbo.P_inProcess where pipid=@pipid";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();

                sSql = "select pipid \n";
                sSql += "from PDFTAG.dbo.P_inProcess a              \n";
                sSql += " where hdid=@hdid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@hdid", hdid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                new_pipid = dt.Rows[0]["pipid"].ToString();

                #region ONRUNNING
                sSql = "select ID from PDFTAG.dbo.HEADER_H a where pipid=@pipid ";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string onhid = dt.Rows[0]["ID"].ToString();


                sSql = @"insert into PDFTAG.dbo.HEADER_H 
(PIPID,SEASON,STYLE,GENERATEDDATE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE,ORGID)
    select '" + new_pipid + @"'as PIPID, SEASON, STYLE, GENERATEDDATE, CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE, ID as ORGID
from PDFTAG.dbo.HEADER_H where pipid=@pipid; SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                long new_hid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                sSql = @"insert into PDFTAG.dbo.HEADER_D
(PIPID,H_ID,COLNAME,VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE,ORGID)
    select '" + new_pipid + "'as PIPID, '" + new_hid + @"' as H_ID, COLNAME, VALUE, CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE, ID as ORGID
from PDFTAG.dbo.HEADER_D where pipid=@pipid; ";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();
                //long new_Did = Convert.ToInt64(cm.ExecuteScalar().ToString());

                sSql = @"INSERT INTO SIZETABLE_M
(PIPID,UNIT,CREATOR,CREATEDDATE,ORGID)
    select '" + new_pipid + @"' as PIPID,UNIT,CREATOR,CREATEDDATE, ID as ORGID
    from PDFTAG.dbo.SIZETABLE_M where pipid=@pipid;SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                long new_SMid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                sSql = @"INSERT INTO SIZETABLE_S
(PIPID,M_ID,SAMPLE,SIZE_,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE,ORGID)
select '" + new_pipid + "' as PIPID, '" + new_SMid + @"' as M_ID,SAMPLE,SIZE_,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE, ID as ORGID
    from PDFTAG.dbo.SIZETABLE_S where pipid=@pipid;

INSERT INTO SIZETABLE_H
(PIPID,M_ID,CODE,DESCRIPTION,TOL_NEGTIVE,TOL_PLUS,COL1,COL2,COL3,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE,ORGID,EnableFlag)
select  '" + new_pipid + "' as PIPID, '" + new_SMid + @"' as M_ID,CODE,DESCRIPTION,TOL_NEGTIVE,TOL_PLUS,COL1,COL2,COL3,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE, ID as ORGID,EnableFlag
from PDFTAG.dbo.SIZETABLE_H where pipid=@pipid;
";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();

                sSql = @"INSERT INTO SIZETABLE_D
(PIPID,M_ID,H_ID,S_ID,VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE,ORGID)
select  '" + new_pipid + @"' as PIPID,'" + new_SMid + @"' as M_ID,
(select ID from SIZETABLE_H where ORGID = SIZETABLE_D.H_ID) as H_ID,
(select ID from SIZETABLE_S where ORGID = SIZETABLE_D.S_ID) as S_ID,
VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE, ID as ORGID
from SIZETABLE_D where pipid=@pipid";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.ExecuteNonQuery();
                #endregion

            }


        }
        catch (Exception err)
        {
            Response.Write("btnAdd_Click:" + err.ToString());
        }


        return new_pipid;
    }

    #endregion

    public string ConvertToStrDouble(string sVal)
    {
        return sVal;
        if (string.IsNullOrWhiteSpace(sVal) || sVal.Contains("--"))
            return "";

        string res = "";
        double dVal = 0;
        string[] arrValue = sVal.Split(' ');

        if (arrValue.Length == 1)
        {
            if (arrValue[0].Contains("/"))
            {
                string[] arr = arrValue[0].Split('/');
                double dVal1 = 0;
                double dVal2 = 0;
                double dVal3 = 0;
                double.TryParse(arr[0], out dVal1);
                double.TryParse(arr[1], out dVal2);
                if (dVal2 > 0)
                    dVal3 = dVal1 / dVal2;

                double.TryParse(arrValue[0], out dVal);

                dVal = dVal + dVal3;

            }
            else
                double.TryParse(arrValue[0], out dVal);

        }
        else if (arrValue.Length == 2)
        {
            string s = arrValue[0];
            string[] arr = arrValue[1].Split('/');
            double dVal1 = 0;
            double dVal2 = 0;
            double dVal3 = 0;
            double.TryParse(arr[0], out dVal1);
            double.TryParse(arr[1], out dVal2);
            if (dVal2 > 0)
                dVal3 = dVal1 / dVal2;

            double.TryParse(arrValue[0], out dVal);

            dVal = dVal + dVal3;
        }


        return dVal.ToString();
    }

    public class Lu_SizeTableDto
    {
        public int rowid { get; set; }
        public string codeid { get; set; }
        public string Name { get; set; }
        public string Criticality { get; set; }
        public string TolA { get; set; }
        public string TolB { get; set; }
        public string HTMInstruction { get; set; }
        public string A1 { get; set; }
        public string A2 { get; set; }
        public string A3 { get; set; }
        public string A4 { get; set; }
        public string A5 { get; set; }
        public string A6 { get; set; }
        public string A7 { get; set; }
        public string A8 { get; set; }
        public string A9 { get; set; }
        public string A10 { get; set; }
        public string A11 { get; set; }
        public string A12 { get; set; }
        public string A13 { get; set; }
        public string A14 { get; set; }
        public string A15 { get; set; }

    }

    public class Lu_SizeHeaderDto
    {
        public int Idx { get; set; }
        public string Name { get; set; }
    }
    public void ConvertPDFToText(string sPdfPath, string sTxtPath)
    {
        // Initialize license object
        Aspose.Pdf.License license = new Aspose.Pdf.License();
        try
        {
            // Set license
            license.SetLicense(Server.MapPath("~/PDFManage/Aspose.Pdf.lic"));
        }
        catch (Exception)
        {
            // something went wrong
            throw;
        }

        // Open document
        Document pdfDocument = new Document(sPdfPath);

        StringBuilder sb = new StringBuilder();
        int iPage = 0;
        foreach (var page in pdfDocument.Pages)
        {
            try
            {
                iPage++;
                Aspose.Pdf.Text.TableAbsorber absorber = new Aspose.Pdf.Text.TableAbsorber();
                absorber.Visit(page);
                foreach (AbsorbedTable table in absorber.TableList)
                {
                    Console.WriteLine("Table");
                    foreach (AbsorbedRow row in table.RowList)
                    {
                        sb.Append("@Row: ");

                        bool isHeader = false;
                        if (row.CellList.Any(a => a.TextFragments.Any(t => t.Text.Contains("Season:"))))
                            isHeader = true;

                        foreach (AbsorbedCell cell in row.CellList)
                        {
                            string cellText = "";
                            bool getSamplStep = false;
                            bool getSamplSize = false;
                            foreach (TextFragment fragment in cell.TextFragments)
                            {
                                if (fragment.BaselinePosition.YIndent <= 11)
                                {
                                    //Remove "Modified Date:Aug 11, 2020.."
                                    continue;
                                }

                                foreach (TextSegment seg in fragment.Segments)
                                {
                                    //if(seg.Text== "Standard")
                                    //{

                                    //}

                                    cellText += (isHeader ? (seg.Text.EndsWith(":") ? "&" : "") : "") + seg.Text + " ";
                                    if (getSamplStep) { listSampleStep.Add(seg.Text); getSamplStep = false; }
                                    else if (getSamplSize) { listSampleSize.Add(seg.Text); getSamplSize = false; }
                                    if (seg.Text == "Sample") { getSamplStep = true; }
                                    else if (seg.Text == "Sample Size") { getSamplSize = true; }
                                }
                                //sb.Append(seg.Text + " | ");
                                //Console.Write($"{sb.ToString()}|");                              
                            }
                            //if (cellText.StartsWith("Modified D")) cellText = "";
                            sb.Append(cellText + " | ");
                        }
                        Console.WriteLine();
                    }
                    sb.AppendLine("----");
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                LogFile.Logger.Log("[ConvertPDFToText] sPdfPath=" + sPdfPath + " iPage=" + iPage + " Exception=" + ex.ToString());
                Response.Write("<!--[ConvertPDFToText] iPage=" + iPage + " Exception=" + ex.ToString() + "-->");
            }
        }

        System.IO.File.WriteAllText(sTxtPath, sb.ToString());
    }

    public void ConvertPDFToText2(string sPdfPath, string sTxtPath)
    {
        LogFile.Logger.Log("[ConvertPDFToText2] sTxtPath=" + sTxtPath);

        // Initialize license object
        Aspose.Pdf.License license = new Aspose.Pdf.License();
        try
        {
            // Set license
            license.SetLicense(Server.MapPath("~/PDFManage/Aspose.Pdf.lic"));
        }
        catch (Exception)
        {
            // something went wrong
            throw;
        }

        Document pdfDocument = new Document(sPdfPath);

        // Create TextAbsorber object to extract text
        TextAbsorber textAbsorber = new TextAbsorber();
        // Accept the absorber for all the pages
        pdfDocument.Pages.Accept(textAbsorber);
        // Get the extracted text
        string extractedText = textAbsorber.Text;
        // Create a writer and open the file
        TextWriter tw = new StreamWriter(sTxtPath);
        // Write a line of text to the file
        tw.WriteLine(extractedText);
        // Close the stream
        tw.Close();
    }

    public void ConvertPDFToText3(string sPdfPath, string sTxtPath)
    {
        // Initialize license object
        Aspose.Pdf.License license = new Aspose.Pdf.License();
        try
        {
            // Set license
            license.SetLicense(Server.MapPath("~/PDFManage/Aspose.Pdf.lic"));
        }
        catch (Exception)
        {
            // something went wrong
            throw;
        }

        Document pdfDocument = new Document(sPdfPath);

        StringBuilder sbFinal = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        foreach (var page in pdfDocument.Pages)
        {
            Aspose.Pdf.Text.TableAbsorber absorber = new Aspose.Pdf.Text.TableAbsorber();
            absorber.Visit(page);
            foreach (AbsorbedTable table in absorber.TableList)
            {
                Console.WriteLine("Table");
                foreach (AbsorbedRow row in table.RowList)
                {
                    sb = new StringBuilder();
                    sb.Append("@Row: ");

                    bool isHeader = false;

                    foreach (AbsorbedCell cell in row.CellList)
                    {
                        string cellText = "";
                        foreach (TextFragment fragment in cell.TextFragments)
                        {
                            //if (fragment.BaselinePosition.YIndent <= 11)
                            //{
                            //    //Remove "Modified Date:Aug 11, 2020.."
                            //    continue;
                            //}

                            foreach (TextSegment seg in fragment.Segments)
                            {
                                //if(seg.Text== "Standard")
                                //{

                                //}

                                cellText += seg.Text;
                            }
                            //sb.Append(seg.Text + " | ");
                            //Console.Write($"{sb.ToString()}|");                              
                        }

                        sb.Append(cellText + " %% ");
                    }

                    sbFinal.AppendLine(sb.ToString());
                }

                sb.AppendLine("----");
            }
        }

        System.IO.File.WriteAllText(sTxtPath, sbFinal.ToString());
    }

    public void ConvertPDFToText3_UA(string sPdfPath, string sTxtPath)
    {
        // Initialize license object
        Aspose.Pdf.License license = new Aspose.Pdf.License();
        try
        {
            // Set license
            license.SetLicense(Server.MapPath("~/PDFManage/Aspose.Pdf.lic"));
        }
        catch (Exception)
        {
            // something went wrong
            throw;
        }

        Document pdfDocument = new Document(sPdfPath);

        StringBuilder sbFinal = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        foreach (var page in pdfDocument.Pages)
        {
            bool isParsePage = true;

            //UA- Size table部分，只需要擷取 文件底部有MEASUREMENT CHART文字的size table
            var textAbsorber = new TextAbsorber
            {
                ExtractionOptions = { FormattingMode = TextExtractionOptions.TextFormattingMode.Pure }
            };
            page.Accept(textAbsorber);
            var ext = textAbsorber.Text;

            if (ext.Contains("Tol(+)"))
            {

                if (!ext.Contains("MEASUREMENT CHART"))
                    isParsePage = false;

            }

            if (!isParsePage)
                continue;

            Aspose.Pdf.Text.TableAbsorber absorber = new Aspose.Pdf.Text.TableAbsorber();
            absorber.Visit(page);
            foreach (AbsorbedTable table in absorber.TableList)
            {
                Console.WriteLine("Table");
                foreach (AbsorbedRow row in table.RowList)
                {
                    sb = new StringBuilder();
                    sb.Append("@Row: ");

                    bool isHeader = false;

                    foreach (AbsorbedCell cell in row.CellList)
                    {
                        string cellText = "";
                        foreach (TextFragment fragment in cell.TextFragments)
                        {
                            //if (fragment.BaselinePosition.YIndent <= 11)
                            //{
                            //    //Remove "Modified Date:Aug 11, 2020.."
                            //    continue;
                            //}

                            foreach (TextSegment seg in fragment.Segments)
                            {
                                //if(seg.Text== "Standard")
                                //{

                                //}

                                cellText += seg.Text;
                            }
                            //sb.Append(seg.Text + " | ");
                            //Console.Write($"{sb.ToString()}|");                              
                        }

                        sb.Append(cellText + " %% ");
                    }

                    sbFinal.AppendLine(sb.ToString());
                }

                sb.AppendLine("----");
            }
        }

        System.IO.File.WriteAllText(sTxtPath, sbFinal.ToString());
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        string sDir2 = "upload/PDF";

        try
        {
            DateTime dtNow = DateTime.Now;
            string sFilePath = "";

            if (FileUpload1.HasFile)
            {
                HttpPostedFile postedFile = Request.Files[0];
                if (postedFile.ContentLength > 0)
                {
                    sFilePath = sDir2 + "/" + dtNow.ToString("yyyyMMddHHmmssfff");

                    if (!Directory.Exists(Server.MapPath(sFilePath)))
                        Directory.CreateDirectory(Server.MapPath(sFilePath));

                    sFilePath += "/" + postedFile.FileName;


                    postedFile.SaveAs(Server.MapPath(sFilePath));
                }

            }

            sSql = @"insert into PDFTAG.dbo.P_inProcess 
(ptitle,pidate,piuploadfile,pver,Gaptype,creator,createordate,isShow,gmid,titleType,unit) 
values 
(@ptitle,@pidate,@piuploadfile,@pver,@Gaptype,@creator,@createordate,@isShow,@gmid,@titleType,@unit);SELECT SCOPE_IDENTITY();";

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {

                cm.Parameters.AddWithValue("@titleType", dlTitleType.SelectedItem.Value);
                cm.Parameters.AddWithValue("@ptitle", ptitle.Text);
                cm.Parameters.AddWithValue("@pidate", dtNow);
                cm.Parameters.AddWithValue("@piuploadfile", sFilePath);
                cm.Parameters.AddWithValue("@pver", ddlpver.SelectedItem.Value);
                cm.Parameters.AddWithValue("@Gaptype", ddlpver.SelectedItem.Value == "3" ? dlGaptype.SelectedItem.Value : "0");
                cm.Parameters.AddWithValue("@unit", dlunit.SelectedItem.Value);
                cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                cm.Parameters.AddWithValue("@createordate", dtNow);
                cm.Parameters.AddWithValue("@isShow", "0");
                //cm.Parameters.AddWithValue("@gmid", ddlGroup2.SelectedItem.Value);
                cm.Parameters.AddWithValue("@gmid", hidgmid.Value);


                long result = Convert.ToInt64(cm.ExecuteScalar().ToString());


            }


        }
        catch (Exception err)
        {
            Response.Write("btnAdd_Click:" + err.ToString());
        }


        DataBind();
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        string sDir2 = "upload/PDF";
        try
        {
            DateTime dtNow = DateTime.Now;
            string sFilePath = "";

            //編輯不看檔案
            //if (FileUpload1.HasFile)
            //{
            //    HttpPostedFile postedFile = Request.Files[0];
            //    if (postedFile.ContentLength > 0)
            //    {
            //        sFilePath = sDir2 + "/" + dtNow.ToString("yyyyMMddHHmmssfff");

            //        if (!Directory.Exists(Server.MapPath(sFilePath)))
            //            Directory.CreateDirectory(Server.MapPath(sFilePath));

            //        sFilePath += "/" + postedFile.FileName;
            //        postedFile.SaveAs(Server.MapPath(sFilePath));
            //    }

            //}

            sSql = "update PDFTAG.dbo.P_inProcess    \n";
            sSql += " set titleType=@titleType,ptitle=@ptitle,pver=@pver,Gaptype=@Gaptype,gmid=@gmid,unit=@unit \n";
            if (!string.IsNullOrEmpty(sFilePath))
            {
                sSql += " ,piuploadfile=@piuploadfile \n";
            }


            sSql += "where pipid=@pipid                          \n";


            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                cm.Parameters.AddWithValue("@titleType", dlTitleType.SelectedItem.Value);
                cm.Parameters.AddWithValue("@ptitle", ptitle.Text);
                cm.Parameters.AddWithValue("@pidate", dtNow);
                if (!string.IsNullOrEmpty(sFilePath))
                    cm.Parameters.AddWithValue("@piuploadfile", sFilePath);
                cm.Parameters.AddWithValue("@pver", ddlpver.SelectedItem.Value);
                cm.Parameters.AddWithValue("@Gaptype", ddlpver.SelectedItem.Value == "3" ? dlGaptype.SelectedItem.Value : "0");
                cm.Parameters.AddWithValue("@unit", dlunit.SelectedItem.Value);
                //cm.Parameters.AddWithValue("@gmid", ddlGroup2.SelectedItem.Value);
                cm.Parameters.AddWithValue("@gmid", hidgmid.Value);
                cm.ExecuteNonQuery();




            }


        }
        catch (Exception err)
        {
            Response.Write("btnEdit_Click:" + err.ToString());
        }


        DataBind();
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DataBind();
    }

    public IEnumerable<string> SplitString(string s, int length)
    {
        var buf = new char[length];
        using (var rdr = new StringReader(s))
        {
            int l;
            l = rdr.ReadBlock(buf, 0, length);
            while (l > 0)
            {
                yield return (new string(buf, 0, l));
                l = rdr.ReadBlock(buf, 0, length);
            }
        }
    }

    protected void dlVersion_SelectedIndexChanged(object sender, EventArgs e)
    {
        var sql = new SQLHelper();
        var dt = new DataTable();

        string sSql = "";

        string version = dlVersion.SelectedItem.Value;

        sSql = "select gmid,gmname \n";
        sSql += "from PDFTAG.dbo.GroupManage a              \n";
        sSql += " where 1=1 and a.isshow=0  \n";

        sSql += " and a.gmcate in ('" + version + "') \n";
        sSql += "order by a.gmname \n";

        Response.Write("<!--" + sSql + "-->");
        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            ddlGroup.DataTextField = "gmname";
            ddlGroup.DataValueField = "gmid";
            ddlGroup.DataSource = dt;
            ddlGroup.DataBind();
            ddlGroup.Items.Insert(0, new System.Web.UI.WebControls.ListItem("全部", ""));


        }
        btnQuery_Click(sender, e);

    }

    /// <summary>
    /// 找出資料庫是否有備註，若有再確認該PDF是否已有備註，若無則依學習新增一筆備註
    /// </summary>
    /// <param name="_learnList"></param>
    /// <param name="_colSoruce"></param>
    /// <param name="_termname"></param>
    /// <param name="_id"></param>
    /// <param name="_colName"></param>
    /// <param name="_dtNow"></param>
    private void btnlearnNote(List<Lu_LearnmgrItemDto> _learnList, string _colSoruce, string _termname, string _id, string _colName, string _dtNow)
    {
        SQLHelper sql = new SQLHelper();
        string sSql = "";
        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            //找學習
            var resNote = _learnList.FirstOrDefault(x => x.ColSource == _colSoruce && x.ColName == "chNote" && x.Termname_org == _termname);
            if (resNote != null)
            {
                //確認是否已有備註
                string sqlNote = "";
                if (_colSoruce == "BOM")
                    sqlNote = "select * from Lu_Ch_Note a where a.IdName='lubid' and a.ColName = '" + _colName + "' and a.Id = " + _id;
                else if (_colSoruce == "Size")
                    sqlNote = "select * from Lu_Ch_Note a where a.IdName='lustid' and a.ColName = '" + _colName + "' and a.Id = " + _id;
                cm.CommandText = sqlNote;
                cm.Parameters.Clear();
                DataTable dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }
                if (dtNote != null)
                {
                    string newNoteSql = @" insert into PDFTAG.dbo.Lu_Ch_Note
                                (IdName,id,ColName,note,creator,createordate)
                                values 
                                (@IdName,@id,@ColName,@note,@creator,@createordate) ";
                    cm.CommandText = newNoteSql;
                    cm.Parameters.Clear();
                    if (_colSoruce == "BOM")
                        cm.Parameters.AddWithValue("@IdName", "lubid");
                    else if (_colSoruce == "Size")
                        cm.Parameters.AddWithValue("@IdName", "lustid");
                    cm.Parameters.AddWithValue("@id", _id);
                    cm.Parameters.AddWithValue("@ColName", _colName);
                    cm.Parameters.AddWithValue("@note", resNote.Termname);
                    cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                    cm.Parameters.AddWithValue("@createordate", _dtNow);
                    cm.ExecuteNonQuery();
                }
            }
        }
    }


    private void ParseExcel_All(string pipid)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        script = "";

        using (SqlCommand cm = new SqlCommand(sSql, sql.getDbcn()))
        {
            sSql = "select * from PDFTAG.dbo.P_inProcess where pipid='" + pipid + "' \n";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }
        }

        string titleType = dt.Rows[0]["titleType"].ToString();
        string sExcelPath = Server.MapPath("~/PDFManage/" + dt.Rows[0]["piuploadfile"].ToString());
        string pverStr = dt.Rows[0]["pver"].ToString();

        List<DataConfigTable> getConfigSetting = _comFunction.getDataConfigTableByPverDataType(pverStr, "ParseExcel_All");
        //取尺碼表的Sheet 關鍵字
        string sizeSheetNameKeyWord = getConfigSetting.Where(x => x.DataName == "sizeSheetNameKeyWord").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "sizeSheetNameKeyWord").FirstOrDefault().DataValue : "";

        //當紀錄上一個pipid資料, 就要清空
        if (HiddenField1.Value != pipid)
        {
            txtStyle.Text = ""; txtSeason.Text = ""; txtBOMDate.Text = "";
        }
        #region 檢查必要欄位 : Style, Season, BOMDate
        string sStyle = txtStyle.Text, sSeason = txtSeason.Text, sGeneratedDate = txtBOMDate.Text;
        if (string.IsNullOrEmpty(sStyle) || string.IsNullOrEmpty(sSeason) || string.IsNullOrEmpty(sGeneratedDate) || sGeneratedDate == DateTime.MinValue.ToString("yyyy/MM/dd"))
        {
            //取Style, Season, BOMDate值
            Dictionary<string, string> vaildFieldDict = getVaildFieldExcel(sExcelPath, pverStr, sizeSheetNameKeyWord);
            if (!string.IsNullOrEmpty(script))
                return;

            foreach (var item in vaildFieldDict)
            {
                switch (item.Key)
                {
                    case "style":
                        sStyle = item.Value;
                        break;
                    case "season":
                        string seasonStr = item.Value;
                        string seasonNum = Regex.Replace(seasonStr, @"[^0-9]+", String.Empty);
                        string seasonLetter = Regex.Replace(seasonStr, @"[^a-zA-Z]+", String.Empty);
                        sSeason = seasonNum + seasonLetter;
                        //針對英文字母，轉換季節代碼  ex:S23 => 23SS , F23 => 23FW
                        List<DataConfigTable> getSeasonReplaceConfigSetting = _comFunction.getDataConfigTableByPverDataType(pverStr, "seasonReplaceWord");
                        if (getSeasonReplaceConfigSetting.Count > 0)
                        {
                            if (sSeason.Length < 4)
                            {
                                foreach (DataConfigTable seasonReplaceWordItem in getSeasonReplaceConfigSetting)
                                {
                                    if (seasonReplaceWordItem.DataName.ToLower() == seasonLetter.ToLower())
                                        sSeason = seasonNum + seasonReplaceWordItem.DataValue;
                                }
                            }
                        }
                        break;
                    case "BOMDate":
                        sGeneratedDate = item.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        //當Season, Style, GeneratedDate 有一個沒有,就要跳出來
        if (string.IsNullOrEmpty(sStyle) || string.IsNullOrEmpty(sSeason) || string.IsNullOrEmpty(sGeneratedDate) || sGeneratedDate == DateTime.MinValue.ToString("yyyy/MM/dd"))
        {
            txtStyle.Enabled = string.IsNullOrEmpty(sStyle);
            txtSeason.Enabled = string.IsNullOrEmpty(sSeason);
            txtBOMDate.Enabled = (string.IsNullOrEmpty(sGeneratedDate) || sGeneratedDate == DateTime.MinValue.ToString("yyyy/MM/dd"));

            txtStyle.Text = sStyle; txtSeason.Text = sSeason; txtBOMDate.Text = sGeneratedDate; HiddenField1.Value = pipid;
            ScriptManager.RegisterStartupScript(this, GetType(), "Javascript", "javascript:showInput(); ", true);
            return;
        }
        #endregion

        //將資料轉換成DataTable
        DataTable dtReadExcel = LoadExcelAsDataTable(sExcelPath, pverStr, sStyle, sSeason, sGeneratedDate, sizeSheetNameKeyWord);
        if (!string.IsNullOrEmpty(script))
            return;
        //沒有資料
        if (dtReadExcel.Rows.Count <= 0 || dtReadExcel.Columns.Count == 3)
        {
            script = "alert(沒有" + sizeSheetNameKeyWord + "資料!!!')";
            return;
        }
        //insert Table 
        DataTableInsertSql(pipid, titleType, dtReadExcel, pverStr);

    }

    /// <summary>
    /// DataTable 塞入 HEADER_H, HEADER_D, SIZETABLE_M, SIZETABLE_S, SIZETABLE_H, SIZETABLE_D 
    /// 再update ptitle 
    /// </summary>
    /// <param name="pipid"></param>
    /// <param name="titleType"></param>
    /// <param name="dtReadExcel"></param>
    /// <param name="_pver"></param>
    public void DataTableInsertSql(string pipid, string titleType, DataTable dtReadExcel, string _pver)
    {
        #region 參數設定 BY 客戶 
        List<DataConfigTable> getConfigSetting = _comFunction.getDataConfigTableByPverDataType(_pver, "DataTableInsertSql");

        //Size結束的表頭欄位名稱
        string endSizeWord = getConfigSetting.Where(x => x.DataName == "endSizeWord").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "endSizeWord").FirstOrDefault().DataValue : "";
        //insert SIZETABLE_H. COL1 的表頭欄位名稱
        string COL1Word = getConfigSetting.Where(x => x.DataName == "COL1Word").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "COL1Word").FirstOrDefault().DataValue : "";
        //略過insert至 SIZETABLE_H
        string skipRowWordList = getConfigSetting.Where(x => x.DataName == "skipRowWordList").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "skipRowWordList").FirstOrDefault().DataValue : "";
        List<string> skipRowWord = new List<string>();
        foreach (var item in skipRowWordList.Split(';'))
        {
            skipRowWord.Add(item);
        }

        //insert SIZETABLE_H.TOL_NEGTIVE &&  SIZETABLE_H.TOL_PLUS 的表頭欄位名稱
        string TOLWordList = getConfigSetting.Where(x => x.DataName == "TOLWordList").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "TOLWordList").FirstOrDefault().DataValue : "";
        string TOLWord = "";
        foreach (DataColumn item in dtReadExcel.Columns)
        {
            if (TOLWordList.Split(';').Contains(item.ColumnName))
                TOLWord = item.ColumnName;
        }

        //表格內，尺碼開始索引
        string startSizeIndexStr = getConfigSetting.Where(x => x.DataName == "startSizeIndex").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "startSizeIndex").FirstOrDefault().DataValue : "";
        int startSizeIndex = string.IsNullOrEmpty(startSizeIndexStr) ? 0 : Convert.ToInt32(startSizeIndexStr);

        //insert SIZETABLE_H.CODE  的表頭欄位名稱
        string codeIndexStr = getConfigSetting.Where(x => x.DataName == "codeIndex").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "codeIndex").FirstOrDefault().DataValue : "";
        int codeIndex = string.IsNullOrEmpty(codeIndexStr) ? 0 : Convert.ToInt32(codeIndexStr);

        //insert SIZETABLE_H.DESCRIPTION 的表頭欄位名稱
        string descriptIndexStr = getConfigSetting.Where(x => x.DataName == "descriptIndex").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "descriptIndex").FirstOrDefault().DataValue : "";
        int descriptIndex = string.IsNullOrEmpty(descriptIndexStr) ? 0 : Convert.ToInt32(descriptIndexStr); ;


        //因Cotopaxi有兩個版本, 其中一版,只有描述, 沒有Code =>所以將Code跟Description都塞一樣
        if (dtReadExcel.Columns[codeIndex].ColumnName == "Point of Measure")
            descriptIndex = 0;
        #endregion

        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        string seasonStr = dtReadExcel.Rows[0]["season"].ToString();
        string seasonNum = Regex.Replace(seasonStr, @"[^0-9]+", String.Empty);
        string seasonLetter = Regex.Replace(seasonStr, @"[^a-zA-Z]+", String.Empty);
        string sSeason = seasonNum + seasonLetter;

        string sStyle = dtReadExcel.Rows[0]["style"].ToString();
        string sDate = dtReadExcel.Rows[0]["generateddate"].ToString();
        string sGeneratedDate = sDate != "nonDate" ? sDate : "";
        string sNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        long onHid = 0; long onSMid = 0; long onSSid = 0; long onSHid = 0;
        string sample = "";  //20231109 目前未用到 ,直接塞空
        List<long> disable_SizeH = new List<long>();
        #region insert Table => HEADER_H, HEADER_D, SIZETABLE_M, SIZETABLE_S, SIZETABLE_H, SIZETABLE_D =>  set ptitle
        using (SqlConnection connection = new SqlConnection(sql._Conn))
        {
            connection.Open();
            SqlCommand cm = connection.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = connection.BeginTransaction();
            cm.Connection = connection;
            cm.Transaction = transaction;

            try
            {
                //HEADER_H
                sSql = @"INSERT INTO HEADER_H
(PIPID,SEASON,STYLE,GENERATEDDATE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
VALUES
(@PIPID,@SEASON,@STYLE,@GENERATEDDATE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE) ;SELECT SCOPE_IDENTITY();";

                cm.CommandText = sSql;
                cm.Parameters.Clear();


                cm.Parameters.AddWithValue("@PIPID", pipid);
                cm.Parameters.AddWithValue("@SEASON", sSeason);
                cm.Parameters.AddWithValue("@STYLE", sStyle);
                cm.Parameters.AddWithValue("@GENERATEDDATE", sGeneratedDate);
                cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                onHid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                //HEADER_D=> GmyShark未用到
                //            //HEADER_D  
                //            foreach (KeyValuePair<string, string> dicItem in dicHeadD)
                //            {
                //                sSql = @"INSERT INTO HEADER_D
                //(PIPID,H_ID,COLNAME,VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
                //VALUES
                //(@PIPID,@H_ID,@COLNAME,@VALUE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);";

                //                cm.CommandText = sSql;
                //                cm.Parameters.Clear();

                //                cm.Parameters.AddWithValue("@PIPID", pipid);
                //                cm.Parameters.AddWithValue("@H_ID", onHid);
                //                cm.Parameters.AddWithValue("@COLNAME", dicItem.Key);
                //                cm.Parameters.AddWithValue("@VALUE", dicItem.Value);
                //                cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                //                cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                //                cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                //                cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                //                cm.ExecuteNonQuery();
                //            }


                //SIZETABLE_M
                sSql = @"INSERT INTO SIZETABLE_M
(PIPID,UNIT,CREATOR,CREATEDDATE)
VALUES
(@PIPID,@UNIT,@CREATOR,@CREATEDDATE);SELECT SCOPE_IDENTITY();";
                cm.CommandText = sSql;
                cm.Parameters.Clear();

                cm.Parameters.AddWithValue("@PIPID", pipid);
                cm.Parameters.AddWithValue("@UNIT", dlunit.SelectedItem.Value);
                cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                onSMid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                Dictionary<long, string> dicSizeID = new Dictionary<long, string>();
                //開始抓Size
                for (int i = startSizeIndex; i < dtReadExcel.Columns.Count; i++)
                {
                    //結束size
                    if (dtReadExcel.Columns[i].ColumnName == endSizeWord || dtReadExcel.Columns[i].ColumnName.Contains("Column"))
                        break;

                    string sizeTxt = dtReadExcel.Columns[i].ColumnName;
                    sSql = @"INSERT INTO SIZETABLE_S
(PIPID,M_ID,SAMPLE,SIZE_,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
VALUES
(@PIPID,@M_ID,@SAMPLE,@SIZE_,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);SELECT SCOPE_IDENTITY();";
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();

                    cm.Parameters.AddWithValue("@PIPID", pipid);
                    cm.Parameters.AddWithValue("@M_ID", onSMid);
                    cm.Parameters.AddWithValue("@SAMPLE", sample);
                    cm.Parameters.AddWithValue("@SIZE_", sizeTxt);
                    cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                    cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                    cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                    cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                    onSSid = Convert.ToInt64(cm.ExecuteScalar().ToString());
                    dicSizeID.Add(onSSid, sizeTxt);
                }

                foreach (DataRow rowItem in dtReadExcel.Rows)
                {
                    //排除Code, Description不符合條件  不塞入Table
                    if ((rowItem[codeIndex].ToString() == "0" && rowItem[descriptIndex].ToString() == "0") || string.IsNullOrEmpty(rowItem[descriptIndex].ToString()))
                        continue;
                    if (skipRowWord.Contains(rowItem[codeIndex].ToString().Replace(" ", "").ToLower()) || skipRowWord.Contains(rowItem[descriptIndex].ToString().Replace(" ", "").ToLower()))
                        continue;

                    //insert ON_SIZETABLE_H
                    sSql = @"INSERT INTO SIZETABLE_H
(PIPID,M_ID,CODE,DESCRIPTION,TOL_NEGTIVE,TOL_PLUS,";
                    if (!string.IsNullOrEmpty(COL1Word))
                        sSql += "COL1,";
                    sSql += @"CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE) VALUES
(@PIPID,@M_ID,@CODE,@DESCRIPTION,@TOL_NEGTIVE,@TOL_PLUS,";
                    if (!string.IsNullOrEmpty(COL1Word))
                        sSql += "@COL1,";
                    sSql += @"@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE);SELECT SCOPE_IDENTITY();";
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();

                    cm.Parameters.AddWithValue("@CODE", rowItem[codeIndex]);
                    cm.Parameters.AddWithValue("@DESCRIPTION", rowItem[descriptIndex]);

                    string tolValue = rowItem[TOLWord].ToString();
                    //coptopaxi拆解 => ex:+1/-0.5
                    if (tolValue.Contains("/") && tolValue.Contains("+") && tolValue.Contains("-"))  //ex: +1/-0.5
                    {
                        string plusString = tolValue.Substring(1, tolValue.IndexOf("/") - 1);
                        string negtiveString = tolValue.Substring(tolValue.IndexOf("/") + 1, tolValue.Length - tolValue.IndexOf("/") - 1);
                        cm.Parameters.AddWithValue("@TOL_NEGTIVE", negtiveString);
                        cm.Parameters.AddWithValue("@TOL_PLUS", plusString);
                    }
                    else
                    {
                        //coptopaxi有時值有cm, 將移除cm
                        if (tolValue.ToLower().Contains("cm"))
                            tolValue = tolValue.Replace("cm", "");

                        cm.Parameters.AddWithValue("@TOL_NEGTIVE", string.IsNullOrEmpty(tolValue) ? tolValue : "-" + tolValue);
                        cm.Parameters.AddWithValue("@TOL_PLUS", tolValue);
                    }

                    if (!string.IsNullOrEmpty(COL1Word))
                        cm.Parameters.AddWithValue("@COL1", rowItem[COL1Word]);

                    cm.Parameters.AddWithValue("@PIPID", pipid);
                    cm.Parameters.AddWithValue("@M_ID", onSMid);
                    cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                    cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                    cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                    cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                    onSHid = Convert.ToInt64(cm.ExecuteScalar().ToString());

                    //insert SIZETABLE_D
                    foreach (KeyValuePair<long, string> dicItem in dicSizeID)
                    {
                        string sizeValue = rowItem[dicItem.Value].ToString();
                        sSql = @"INSERT INTO SIZETABLE_D
                (PIPID,M_ID,H_ID,S_ID,VALUE,CREATOR,CREATEDDATE,MODIFIEDBY,MODIFIEDDATE)
                VALUES
                (@PIPID,@M_ID,@H_ID,@S_ID,@VALUE,@CREATOR,@CREATEDDATE,@MODIFIEDBY,@MODIFIEDDATE)";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@S_ID", dicItem.Key);
                        cm.Parameters.AddWithValue("@VALUE", sizeValue);
                        cm.Parameters.AddWithValue("@PIPID", pipid);
                        cm.Parameters.AddWithValue("@M_ID", onSMid);
                        cm.Parameters.AddWithValue("@H_ID", onSHid);
                        cm.Parameters.AddWithValue("@CREATOR", LoginUser.PK);
                        cm.Parameters.AddWithValue("@CREATEDDATE", sNow);
                        cm.Parameters.AddWithValue("@MODIFIEDBY", LoginUser.PK);
                        cm.Parameters.AddWithValue("@MODIFIEDDATE", sNow);
                        cm.ExecuteNonQuery();

                        //當有sizeValue 有一個小於0就要Flag=> enble=0
                        if ((string.IsNullOrEmpty(sizeValue) || Convert.ToDecimal(sizeValue) <= 0) && !disable_SizeH.Contains(onSHid))
                            disable_SizeH.Add(onSHid);
                    }

                }

                //update SizeTable_H=> enble=0
                if (disable_SizeH.Count > 0)
                {
                    string H_IDStr = "";
                    foreach (long disItem in disable_SizeH)
                        H_IDStr += disItem.ToString() + ",";

                    sSql = @"update PDFTAG.dbo.SIZETABLE_H   set enableFlag=0  where ID in (" + H_IDStr.Substring(0, H_IDStr.Length - 1) + ")";
                    cm.CommandText = sSql;
                    cm.ExecuteNonQuery();
                }


                #region set ptitle
                sSql = "select a.*,b.season,b.style,b.generateddate,c.gmname \n";
                sSql += "from PDFTAG.dbo.P_inProcess a              \n";
                sSql += " left join PDFTAG.dbo.Header_H b on a.pipid=b.pipid and b.isshow=0             \n";
                sSql += " left join PDFTAG.dbo.GroupManage c on a.gmid=c.gmid and c.isshow=0             \n";
                sSql += " where 1=1 and a.isshow=0 and a.pipid='" + pipid + "' \n";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string ptitle = dt.Rows[0]["style"].ToString();
                string season2 = dt.Rows[0]["season"].ToString();
                ptitle = ptitle + "-" + season2;

                sSql = "update PDFTAG.dbo.P_inProcess    \n";
                sSql += " set mdate=@mdate \n";
                if (titleType == "1")
                {
                    sSql += " ,ptitle=@ptitle \n";
                }
                sSql += "where pipid=@pipid \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", pipid);
                cm.Parameters.AddWithValue("@mdate", sNow);
                cm.Parameters.AddWithValue("@ptitle", ptitle);
                cm.ExecuteNonQuery();

                #endregion
                transaction.Commit();

                //新增dbo.HistoryData, P_inProcess 並 複製  ,HEADER_H, HEADER_D, SIZETABLE_M, SIZETABLE_S, SIZETABLE_H, SIZETABLE_D
                string new_pipid = Add_PDF_Manage(pipid);
                script = "alert('執行完成!')";
            }
            catch (Exception ex)
            {
                script = "alert('執行失敗!')";
                transaction.Rollback();
                return;
            }
        }
        #endregion


    }

    /// <summary>
    /// 合併儲存格時，取最後Column索引
    /// </summary>
    /// <param name="styleSheet"></param>
    /// <param name="rowInd"></param>
    /// <param name="startColInd"></param>
    /// <param name="lastColInd"></param>
    /// <returns></returns>
    public int IsMergedCellFinanlColumnIndex(ISheet styleSheet, int rowInd, int startColInd, int lastColInd)
    {
        int cellStr = 0;
        for (int i = startColInd; i < lastColInd; i++)
        {
            if (styleSheet.GetRow(rowInd).GetCell(i).CellType != CellType.Blank)
            {
                cellStr = i;
                break;
            }
        }
        return cellStr;
    }
    public string getCellDateFromException(ICell celldate)
    {
        string generatedDateStr = "";
        if (celldate != null)
        {
            try
            {
                generatedDateStr = celldate.DateCellValue.ToString("yyyy/MM/dd");
            }
            catch (NullReferenceException)
            {
                generatedDateStr = DateTime.FromOADate(celldate.NumericCellValue).ToString("yyyy/MM/dd");
            }
        }
        return generatedDateStr;
    }

    /// <summary>
    /// 透過參數設定By Pver，將Excel轉成DataTable
    /// </summary>
    /// <param name="xlsFilename"></param>
    /// <param name="_pver"></param>
    /// <param name="_styleCode"></param>
    /// <param name="_Season"></param>
    /// <param name="_generatedDate"></param>
    /// <param name="sizeSheetNameKeyWord"></param>
    /// <returns></returns>
    public DataTable LoadExcelAsDataTable(String xlsFilename, string _pver, string _styleCode, string _Season, string _generatedDate, string sizeSheetNameKeyWord)
    {
        #region 參數設定 BY 客戶 
        List<DataConfigTable> getConfigSetting = _comFunction.getDataConfigTableByPverDataType(_pver, "LoadExcelAsDataTable");
        //取尺碼起始Row的關鍵字
        string sizeStartRowKeyWordList = getConfigSetting.Where(x => x.DataName == "sizeStartRowKeyWordList").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "sizeStartRowKeyWordList").FirstOrDefault().DataValue : "";
        //取尺碼表結束的關鍵字
        string sizeEndRowKeyWord = getConfigSetting.Where(x => x.DataName == "sizeEndRowKeyWord").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "sizeEndRowKeyWord").FirstOrDefault().DataValue : "";
        #endregion

        // 回傳的Table
        DataTable table = new DataTable();
        FileInfo fi = new FileInfo(xlsFilename);
        IFormulaEvaluator formulaEvaluator;

        //取得Size Mapping表
        List<DataConfigTable> sizeMapping = _comFunction.getDataConfigTableByPverDataType(_pver, "SizeMapping");

        using (FileStream fstream = new FileStream(fi.FullName, FileMode.Open))
        {
            IWorkbook wb;
            wb = new XSSFWorkbook(fstream); // excel2007
            formulaEvaluator = new XSSFFormulaEvaluator(wb); // Important!! 取公式值的時候會用到

            try
            {
                #region 取要的Size Sheet 
                bool sizeEndFlag = false;
                int startRowNumber = 0;
                int sheetCount = wb.NumberOfSheets;
                for (int sheetNumber = 0; sheetNumber < sheetCount; sheetNumber++)
                {
                    ISheet sizeSheet = wb.GetSheetAt(sheetNumber);
                    if (sizeSheet.SheetName.ToLower().Contains(sizeSheetNameKeyWord.ToLower()))
                    {
                        //Row Start 
                        for (int rowNumber = 0; rowNumber < sizeSheet.LastRowNum; rowNumber++)
                        {
                            IRow currentRowNumber = sizeSheet.GetRow(rowNumber);
                            for (int colNumber = 0; colNumber < currentRowNumber.LastCellNum; colNumber++)
                            {
                                ICell cell = currentRowNumber.GetCell(colNumber);
                                if (cell != null)
                                {
                                    //Row Start =>開始抓Size 的RowNum                         
                                    if (cell.CellType == CellType.String)
                                    {
                                        foreach (string sizeStartRowKeyWord in sizeStartRowKeyWordList.Split(';'))
                                        {
                                            if (cell.StringCellValue.Contains(sizeStartRowKeyWord))
                                            {
                                                startRowNumber = rowNumber;
                                                break;
                                            }
                                        }

                                    }
                                }
                                if (startRowNumber > 0)
                                    break;
                            }
                            if (startRowNumber > 0)
                                break;
                        }
                        // 取值做Header
                        IRow headerRow = sizeSheet.GetRow(startRowNumber);
                        int cellCount = headerRow.LastCellNum; // 取欄位數
                        for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                        {
                            string headerStr = headerRow.GetCell(i).StringCellValue;
                            if (sizeMapping.Where(x => x.DataName == headerStr).ToList().Count > 0)
                            {
                                string sizeMappingValue = sizeMapping.Where(x => x.DataName == headerStr).FirstOrDefault().DataValue;
                                table.Columns.Add(new DataColumn(!string.IsNullOrEmpty(sizeMappingValue) ? sizeMappingValue : headerStr));
                            }
                            else
                                table.Columns.Add(new DataColumn(headerStr));

                        }
                        table.Columns.Add(new DataColumn("style"));
                        table.Columns.Add(new DataColumn("season"));
                        table.Columns.Add(new DataColumn("generateddate"));
                        // 取值塞入Detail
                        for (int rowNum = (startRowNumber + 1); rowNum <= sizeSheet.LastRowNum; rowNum++)
                        {
                            IRow row = sizeSheet.GetRow(rowNum);
                            if (row == null) continue;
                            DataRow dataRow = table.NewRow();
                            dataRow["style"] = _styleCode;
                            dataRow["season"] = _Season;
                            dataRow["generateddate"] = _generatedDate;
                            int dataColNum = 0;
                            //依先前取得的欄位數逐一設定欄位內容
                            for (int colNum = row.FirstCellNum; colNum < cellCount; colNum++)
                            {
                                ICell cell = row.GetCell(colNum);
                                if (cell != null)
                                {
                                    dataRow[dataColNum] = readCellValue(cell, formulaEvaluator);

                                    //如果有結束KeyWord就跳出
                                    if (!string.IsNullOrEmpty(sizeEndRowKeyWord))
                                    {
                                        if (dataRow[dataColNum].ToString().Contains(sizeEndRowKeyWord))
                                        {
                                            sizeEndFlag = true;
                                            break;
                                        }
                                    }
                                    //break;
                                    dataColNum++;
                                }
                            }
                            table.Rows.Add(dataRow);
                            if (sizeEndFlag)
                                break;
                        }
                        break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                script = "alert('執行失敗!!!  " + System.Reflection.MethodBase.GetCurrentMethod() + ex.Message + "')";
                return table;
                //throw;
            }
        }

        return table;
    }

    /// <summary>
    /// 檢查必要欄位 Style, Season, 日期值
    /// </summary>
    /// <param name="xlsFilename"></param>
    /// <param name="pver"></param>
    /// <param name="sizeSheetNameKeyWord"></param>
    /// <returns></returns>
    private Dictionary<string, string> getVaildFieldExcel(String xlsFilename, string pver, string sizeSheetNameKeyWord)
    {
        #region 參數設定By Pver
        List<DataConfigTable> getConfigSetting = _comFunction.getDataConfigTableByPverDataType(pver, "vaildFieldExcel");
        string styleSheetNameKeyWord = getConfigSetting.Where(x => x.DataName == "styleSheetNameKeyWord").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "styleSheetNameKeyWord").FirstOrDefault().DataValue : "";
        string styleWordList = getConfigSetting.Where(x => x.DataName == "styleWordList").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "styleWordList").FirstOrDefault().DataValue : "";
        string seasonWord = getConfigSetting.Where(x => x.DataName == "seasonWord").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "seasonWord").FirstOrDefault().DataValue : "";
        string generateDateWord = getConfigSetting.Where(x => x.DataName == "generateDateWord").ToList().Count > 0 ? getConfigSetting.Where(x => x.DataName == "generateDateWord").FirstOrDefault().DataValue : "";
        #endregion
        Dictionary<string, string> result = new Dictionary<string, string>();
        string styleCodeStr = "", SeasonStr = "", generatedDateStr = "";
        FileInfo fi = new FileInfo(xlsFilename);
        IFormulaEvaluator formulaEvaluator;

        List<string> sheetKeyWordList = new List<string>();
        sheetKeyWordList.Add(sizeSheetNameKeyWord);   //先從Size Sheet找
        sheetKeyWordList.Add(styleSheetNameKeyWord);  //沒有再從StyleSheet找

        using (FileStream fstream = new FileStream(fi.FullName, FileMode.Open))
        {
            IWorkbook wb;
            wb = new XSSFWorkbook(fstream); // excel2007
            formulaEvaluator = new XSSFFormulaEvaluator(wb); // Important!! 取公式值的時候會用到

            try
            {
                int sheetCount = wb.NumberOfSheets;
                foreach (string keyWord in sheetKeyWordList)
                {
                    if (string.IsNullOrEmpty(styleCodeStr) || string.IsNullOrEmpty(SeasonStr) || string.IsNullOrEmpty(generatedDateStr) || generatedDateStr == DateTime.MinValue.ToString("yyyy/MM/dd"))
                    {
                        #region  取要的 Sheet 
                        for (int sheetNumber = 0; sheetNumber < sheetCount; sheetNumber++)
                        {
                            ISheet getSheet = wb.GetSheetAt(sheetNumber);
                            if (getSheet.SheetName.ToLower().Contains(keyWord.ToLower()))
                            {
                                for (int rowNumber = 0; rowNumber < getSheet.LastRowNum; rowNumber++)
                                {
                                    IRow currentRowNumber = getSheet.GetRow(rowNumber);
                                    if (currentRowNumber == null)
                                        continue;
                                    for (int colNumber = 0; colNumber < currentRowNumber.LastCellNum; colNumber++)
                                    {
                                        ICell cell = currentRowNumber.GetCell(colNumber);
                                        if (cell != null && cell.CellType == CellType.String)
                                        {
                                            //取得日期值
                                            if ((string.IsNullOrEmpty(generatedDateStr) || generatedDateStr == DateTime.MinValue.ToString("yyyy/MM/dd")) && cell.StringCellValue.ToLower().Replace(" ", "").Replace(":", "").Contains(generateDateWord))
                                            {
                                                generatedDateStr = getKeyWordValue(getSheet, cell, formulaEvaluator, currentRowNumber.LastCellNum, "date");
                                                break;
                                            }

                                            //取得Style值
                                            if (string.IsNullOrEmpty(styleCodeStr) && styleWordList.Contains(cell.StringCellValue.ToLower().Replace(" ", "").Replace(":", "")))
                                            {
                                                string cellStr = "";
                                                cellStr = getKeyWordValue(getSheet, cell, formulaEvaluator, currentRowNumber.LastCellNum, "style");
                                                styleCodeStr = cellStr.Contains(" ") ? cellStr.Substring(0, cellStr.IndexOf(" ")) : cellStr;
                                                //當尺寸小於3碼 ,無效, 重取
                                                if (styleCodeStr.Length <= 3)
                                                    styleCodeStr = "";
                                            }
                                            //取得Season值
                                            if (string.IsNullOrEmpty(SeasonStr) && cell.StringCellValue.ToLower().Replace(" ", "").Replace(":", "") == seasonWord)
                                            {
                                                SeasonStr = getKeyWordValue(getSheet, cell, formulaEvaluator, currentRowNumber.LastCellNum, "season");
                                            }
                                        }

                                        //已取得就跳出迴圈 column
                                        if (!string.IsNullOrEmpty(styleCodeStr) && !string.IsNullOrEmpty(SeasonStr) && !string.IsNullOrEmpty(generatedDateStr) && generatedDateStr != DateTime.MinValue.ToString("yyyy/MM/dd"))
                                            break;
                                    }
                                    //已取得就跳出迴圈 row
                                    if (!string.IsNullOrEmpty(styleCodeStr) && !string.IsNullOrEmpty(SeasonStr) && !string.IsNullOrEmpty(generatedDateStr) && generatedDateStr != DateTime.MinValue.ToString("yyyy/MM/dd"))
                                        break;
                                }
                                break;
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                script = "alert('執行失敗!!!  " + System.Reflection.MethodBase.GetCurrentMethod() + ex.Message + "')";
                return result;
            }
        }

        result.Add("style", styleCodeStr);
        result.Add("season", SeasonStr);
        result.Add("BOMDate", generatedDateStr);
        return result;
    }

    private string getKeyWordValue(ISheet sheet, ICell cell, IFormulaEvaluator formulaEvaluator, int lastCellNum, string keyWord)
    {
        string resultValue = "";
        ICell resultCell = sheet.GetRow(cell.RowIndex).GetCell(cell.ColumnIndex + 1);
        //是否有合併儲存格
        if (cell.IsMergedCell)
        {
            int nextColInd = IsMergedCellFinanlColumnIndex(sheet, cell.RowIndex, cell.ColumnIndex + 1, lastCellNum);
            if (nextColInd > 0)
            {
                resultCell = sheet.GetRow(cell.RowIndex).GetCell(nextColInd);
            }
        }

        switch (keyWord)
        {
            case "date":
                resultValue = getCellDateFromException(resultCell);
                break;
            case "style":
            case "season":
                resultValue = readCellValue(resultCell, formulaEvaluator);
                break;
            default:
                break;
        }

        return resultValue;
    }


    private string readCellValue(ICell cell, IFormulaEvaluator formulaEvaluator)
    {
        string resultValue = "";
        switch (cell.CellType)
        {
            case CellType.Numeric:  // 數值格式
                if (DateUtil.IsCellDateFormatted(cell))
                {   // 日期格式
                    resultValue = cell.DateCellValue.ToString();
                }
                else
                {   // 數值格式
                    resultValue = cell.NumericCellValue.ToString();
                }
                break;
            case CellType.String:   // 字串格式
                resultValue = cell.StringCellValue;
                break;
            case CellType.Formula:  // 公式格式
                var formulaValue = formulaEvaluator.Evaluate(cell);
                if (formulaValue.CellType == CellType.String) resultValue = formulaValue.StringValue.ToString();          // 執行公式後的值為字串型態
                else if (formulaValue.CellType == CellType.Numeric) resultValue = formulaValue.NumberValue.ToString();    // 執行公式後的值為數字型態
                break;
            default:
                break;
        }
        return resultValue;
    }
}