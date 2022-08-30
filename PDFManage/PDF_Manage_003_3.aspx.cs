using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;
using OfficeOpenXml;


public partial class Passport_Passport_A000 : System.Web.UI.Page
{
    protected string script;
    protected string hdid;

    protected void Page_Load(object sender, EventArgs e)
    {
        hdid = Request["hdid"];
        if (!IsPostBack)
        {
            setDl();
            DataBind();
        }
    }

    private void setDl()
    {
        var sql = new SQLHelper();
        var dt = new DataTable();

        string sSql = "";


        sSql += "select c.pipid,b.pipid as copy_pipid,b.ptitle,b.pidate,b.piuploadfile,b.pver,b.creator,a.hdid,a.hisversion,a.editdate \n";
        sSql += "from PDFTAG.dbo.HistoryData a              \n";
        sSql += " join PDFTAG.dbo.P_inProcess b on a.hdid=b.hdid            \n";//找出複製的主檔
        sSql += " join PDFTAG.dbo.P_inProcess c on a.pipid=c.pipid            \n";//找出群組
        sSql += " where 1=1 and a.isshow=0  \n";
        sSql += " and a.hdid = '" + hdid + "'   \n";


        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            lblInfo.Text = dt.Rows[0]["pipid"].ToString() + "-" + dt.Rows[0]["ptitle"].ToString() + "-" + dt.Rows[0]["hisversion"].ToString() + "-" +
                (dt.Rows[0]["pver"].ToString() == "1" ? "Lulu" : dt.Rows[0]["pver"].ToString() == "2" ? "UA" : dt.Rows[0]["pver"].ToString() == "3" ? "GAP" : "");

            hidpipid.Value = dt.Rows[0]["copy_pipid"].ToString();//複製的主檔pipid
        }

    }



    private void DataBind()
    {
        bool _AllowPaging = true;
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        StringBuilder sb = new StringBuilder();

        try
        {
            sSql = "select * \n";
            sSql += "from PDFTAG.dbo.Lu_Header a              \n";
            sSql += " where 1=1 and a.isshow=0   \n";
            sSql += " and a.pipid = '" + hidpipid.Value + "'   \n";
            Response.Write("<!--" + sSql + "-->");

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string luhid = dt.Rows[0]["luhid"].ToString();
                string org_luhid = dt.Rows[0]["org_luhid"].ToString();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_Header a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid in (select org_luhid as luhid from  PDFTAG.dbo.Lu_Header where pipid = '" + hidpipid.Value + "' )   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                DataTable dtLu_Header_Org = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_Header_Org);
                }


                List<Lu_Ch_Note> arrNotes = new List<Lu_Ch_Note>();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='luhid'  and a.Id in (select luhid as id from PDFTAG.dbo.Lu_Header where isshow=0 and pipid = @pipid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                DataTable dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                if (false)
                {
                    #region Lu_Header

                    string season = dt.Rows[0]["Season"].ToString();
                    string style = dt.Rows[0]["style"].ToString();
                    string styledesc = dt.Rows[0]["styledesc"].ToString();
                    string productsize = dt.Rows[0]["productsize"].ToString();
                    string brand = dt.Rows[0]["brand"].ToString();
                    string dividion = dt.Rows[0]["dividion"].ToString();
                    string pod = dt.Rows[0]["pod"].ToString();
                    string stylestatus = dt.Rows[0]["stylestatus"].ToString();
                    string generateddate = dt.Rows[0]["generateddate"].ToString();
                    bool isEditHeader = (dt.Rows[0]["isEdit"].ToString().ToLower() == "true" ? true : false);

                    DataRow[] drOrgHeades = dtLu_Header_Org.Select("luhid='" + org_luhid + "'");

                    if (isEditHeader)
                    {
                        Response.Write("<!--isEditHeader=" + isEditHeader + "-->");

                        if (drOrgHeades.Length > 0)
                        {
                            DataRow drOrgHeader = drOrgHeades[0];

                            season = Compare(drOrgHeader["season"].ToString(), season, FilterNote(arrNotes, luhid, "season"));
                            style = Compare(drOrgHeader["style"].ToString(), style, FilterNote(arrNotes, luhid, "style"));
                            styledesc = Compare(drOrgHeader["styledesc"].ToString(), styledesc, FilterNote(arrNotes, luhid, "styledesc"));
                            productsize = Compare(drOrgHeader["productsize"].ToString(), productsize, FilterNote(arrNotes, luhid, "productsize"));
                            brand = Compare(drOrgHeader["brand"].ToString(), brand, FilterNote(arrNotes, luhid, "brand"));
                            dividion = Compare(drOrgHeader["dividion"].ToString(), dividion, FilterNote(arrNotes, luhid, "dividion"));
                            pod = Compare(drOrgHeader["pod"].ToString(), pod, FilterNote(arrNotes, luhid, "pod"));
                            stylestatus = Compare(drOrgHeader["stylestatus"].ToString(), stylestatus, FilterNote(arrNotes, luhid, "stylestatus"));
                            generateddate = Compare(drOrgHeader["generateddate"].ToString(), generateddate, FilterNote(arrNotes, luhid, "generateddate"));
                        }

                    }

                    sb.Append("<table class='table table-hover'>");
                    sb.Append("<tr>");
                    sb.Append(" <th scope='col'>Season</th>");
                    sb.Append(" <th scope='col'>Style</th>");
                    sb.Append(" <th scope='col'>Style Desc</th>");
                    sb.Append(" <th scope='col'>Product Size</th>");
                    sb.Append(" <th scope='col'>Brand</th>");
                    sb.Append(" <th scope='col'>Dividion</th>");
                    sb.Append(" <th scope='col'>Pod</th>");
                    sb.Append(" <th scope='col'>Style Status</th>");
                    sb.Append(" <th scope='col'>Generated Date</th>");
                    sb.Append("</tr>");
                    sb.Append("<tr>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='Season' onclick='editHeader(this)'>" + season + "</td>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='style' onclick='editHeader(this)'>" + style + "</td>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='styledesc' onclick='editHeader(this)'>" + styledesc + "</td>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='productsize' onclick='editHeader(this)'>" + productsize + "</td>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='brand' onclick='editHeader(this)'>" + brand + "</td>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='dividion' onclick='editHeader(this)'>" + dividion + "</td>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='pod' onclick='editHeader(this)'>" + pod + "</td>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='stylestatus' onclick='editHeader(this)'>" + stylestatus + "</td>");
                    sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='generateddate' onclick='editHeader(this)'>" + generateddate + "</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");

                    #endregion
                }

                if (false)
                {
                    #region Lu_BOM

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_Ch_Note  a             \n";
                    sSql += "where 1=1 \n";
                    sSql += " and a.IdName='lubid'  and a.Id in (select lubid as id from PDFTAG.dbo.Lu_BOM where luhid = @luhid ) \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@luhid", luhid);
                    dtNote = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtNote);
                    }

                    arrNotes = dtNote.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();


                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_BOM a              \n";
                    sSql += " where 1=1   \n";
                    sSql += " and a.luhid = '" + luhid + "'   \n";
                    sSql += " order by lubid,rowid asc    \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    dt = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dt);
                    }

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_BOM a              \n";
                    sSql += " where 1=1   \n";
                    sSql += " and a.lubid in (select org_lubid as lubid from  PDFTAG.dbo.Lu_BOM where luhid = '" + luhid + "' )   \n";
                    sSql += " order by lubid,rowid asc    \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    DataTable dtLu_BOM_Org = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtLu_BOM_Org);
                    }

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_BOMGarmentcolor a              \n";
                    sSql += " where a.luhid = '" + luhid + "'   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    DataTable dtLu_BOMGarmentcolor = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtLu_BOMGarmentcolor);
                    }

                    var arrTypes = dt.AsEnumerable().Select(s => s.Field<string>("type")).Distinct().ToList();

                    foreach (var type in arrTypes)
                    {
                        sb.Append("<h4>" + type + "</h4>");
                        sb.Append("<table class='table table-hover'>");
                        sb.Append("<tr>");
                        sb.Append(" <th scope='col'>Standard Placement</th>");
                        sb.Append(" <th scope='col'>Placement</th>");
                        sb.Append(" <th scope='col'>Supplier Article</th>");
                        sb.Append(" <th scope='col'>Supplier</th>");

                        int iColorCnt = 0;
                        if (dtLu_BOMGarmentcolor.Rows.Count > 0)
                        {
                            for (int i = 1; i <= 10; i++)
                            {
                                string sA = dtLu_BOMGarmentcolor.Rows[0]["A" + i].ToString();
                                if (!string.IsNullOrEmpty(sA))
                                {
                                    sb.Append(" <th scope='col'>" + sA + "</th>");
                                    iColorCnt++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        sb.Append("</tr>");

                        DataRow[] drBoms = dt.Select("type='" + type + "'", "rowid asc");

                        foreach (DataRow drBom in drBoms)
                        {
                            string lubid = drBom["lubid"].ToString();
                            string org_lubid = drBom["org_lubid"].ToString();
                            bool isEdit = (drBom["isEdit"].ToString().ToLower() == "true" ? true : false);

                            string standardPlacement = drBom["StandardPlacement"].ToString();
                            string placement = drBom["Placement"].ToString();
                            string supplierArticle = drBom["SupplierArticle"].ToString();
                            string supplier = drBom["Supplier"].ToString();

                            DataRow[] drOrgBoms = dtLu_BOM_Org.Select("lubid='" + org_lubid + "'");

                            if (isEdit)
                            {
                                Response.Write("<!--isEdit=" + isEdit + "-->");

                                if (drOrgBoms.Length > 0)
                                {
                                    DataRow drOrgBom = drOrgBoms[0];

                                    standardPlacement = Compare(drOrgBom["StandardPlacement"].ToString(), standardPlacement, FilterNote(arrNotes, lubid, "StandardPlacement"));
                                    placement = Compare(drOrgBom["Placement"].ToString(), placement, FilterNote(arrNotes, lubid, "placement"));
                                    supplierArticle = Compare(drOrgBom["supplierArticle"].ToString(), supplierArticle, FilterNote(arrNotes, lubid, "supplierArticle"));
                                    supplier = Compare(drOrgBom["supplier"].ToString(), supplier, FilterNote(arrNotes, lubid, "supplier"));
                                }

                            }


                            sb.Append("<tr data-rowid='" + drBom["rowid"].ToString() + "'>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='StandardPlacement' onclick='editBom(this)'>" + standardPlacement + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Placement' onclick='editBom(this)'>" + placement + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='SupplierArticle' onclick='editBom(this)'>" + supplierArticle + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Supplier' onclick='editBom(this)'>" + supplier + "</td>");

                            for (int i = 1; i <= iColorCnt; i++)
                            {
                                string color = drBom["B" + i].ToString();
                                if (isEdit)
                                {
                                    if (drOrgBoms.Length > 0)
                                    {
                                        DataRow drOrgBom = drOrgBoms[0];
                                        color = Compare(drOrgBom["B" + i].ToString(), color, FilterNote(arrNotes, lubid, "B" + i));
                                    }
                                }
                                sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='B" + i + "' onclick='editBom(this)'>" + color + "</td>");
                            }
                            sb.Append("</tr>");


                        }


                        sb.Append("</table>");

                    }



                    #endregion
                }



                #region Lu_SizeTable

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='lustid'  and a.Id in (select lustid as id from PDFTAG.dbo.Lu_SizeTable where luhid = @luhid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_SizeTable a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid = '" + luhid + "'   \n";
                sSql += " order by lusthid,rowid asc    \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_SizeTable a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.lustid in (select org_lustid as lustid from  PDFTAG.dbo.Lu_SizeTable where luhid = '" + luhid + "' )   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                DataTable dtSizeTable_Org = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtSizeTable_Org);
                }

                var arr_lusthids = dt.AsEnumerable().Select(s => s.Field<long>("lusthid")).Distinct().ToList();

                foreach (var lusthid in arr_lusthids)
                {
                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_SizeTable_Header a              \n";
                    sSql += " where a.lusthid in ('" + lusthid + "')   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    DataTable dtLu_SizeTable_Header = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtLu_SizeTable_Header);
                    }

                    sb.Append("<h4>Size Table</h4>");
                    sb.Append("<table class='table table-hover'>");
                    sb.Append("<tr>");
                    sb.Append(" <th scope='col'>#</th>");
                    sb.Append(" <th scope='col'>Name</th>");
                    sb.Append(" <th scope='col'>Criticality</th>");
                    sb.Append(" <th scope='col'>Tol(-)</th>");
                    sb.Append(" <th scope='col'>Tol(+)</th>");
                    sb.Append(" <th scope='col'>HTM Instruction</th>");

                    int iOtherCnt = 0;
                    if (dtLu_SizeTable_Header.Rows.Count > 0)
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            string sH = dtLu_SizeTable_Header.Rows[0]["H" + i].ToString();
                            if (!string.IsNullOrEmpty(sH))
                            {
                                sb.Append(" <th scope='col'>" + sH + "</th>");
                                iOtherCnt++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    sb.Append("</tr>");

                    DataRow[] drSizeTables = dt.Select("lusthid='" + lusthid + "'", "rowid asc");

                    foreach (DataRow drSizeTable in drSizeTables)
                    {
                        string lustid = drSizeTable["lustid"].ToString();
                        string org_lustid = drSizeTable["org_lustid"].ToString();
                        bool isEdit = (drSizeTable["isEdit"].ToString().ToLower() == "true" ? true : false);

                        string codeid = drSizeTable["codeid"].ToString();
                        string name = drSizeTable["Name"].ToString();
                        string criticality = drSizeTable["Criticality"].ToString();
                        string tolA = drSizeTable["TolA"].ToString();
                        string tolB = drSizeTable["TolB"].ToString();
                        string hTMInstruction = drSizeTable["HTMInstruction"].ToString();

                        DataRow[] drOrgSizeTables = dtSizeTable_Org.Select("lustid='" + org_lustid + "'");

                        if (isEdit)
                        {
                            Response.Write("<!--isEdit=" + isEdit + "-->");

                            if (drOrgSizeTables.Length > 0)
                            {
                                DataRow drOrgSizeTable = drOrgSizeTables[0];

                                codeid = Compare(drOrgSizeTable["codeid"].ToString(), codeid, FilterNote(arrNotes, lustid, "codeid"));
                                name = Compare(drOrgSizeTable["name"].ToString(), name, FilterNote(arrNotes, lustid, "name"));
                                criticality = Compare(drOrgSizeTable["criticality"].ToString(), criticality, FilterNote(arrNotes, lustid, "criticality"));
                                tolA = Compare(drOrgSizeTable["tolA"].ToString(), tolA, FilterNote(arrNotes, lustid, "tolA"));
                                tolB = Compare(drOrgSizeTable["tolB"].ToString(), tolB, FilterNote(arrNotes, lustid, "tolB"));
                                hTMInstruction = Compare(drOrgSizeTable["hTMInstruction"].ToString(), hTMInstruction, FilterNote(arrNotes, lustid, "hTMInstruction"));
                                ;
                            }

                        }


                        sb.Append("<tr data-rowid='" + drSizeTable["rowid"].ToString() + "'>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='codeid' onclick='editSizeTable(this)'>" + codeid + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Name' onclick='editSizeTable(this)'>" + name + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Criticality' onclick='editSizeTable(this)'>" + criticality + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' onclick='editSizeTable(this)'>" + tolA + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' onclick='editSizeTable(this)'>" + tolB + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='HTMInstruction' onclick='editSizeTable(this)'>" + hTMInstruction + "</td>");

                        for (int i = 1; i <= iOtherCnt; i++)
                        {
                            string other = drSizeTable["A" + i].ToString();
                            if (isEdit)
                            {
                                if (drOrgSizeTables.Length > 0)
                                {
                                    DataRow drOrgSizeTable = drOrgSizeTables[0];
                                    other = Compare(drOrgSizeTable["A" + i].ToString(), other, FilterNote(arrNotes, lustid, "A" + i));
                                }
                            }
                            sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='A" + i + "' onclick='editSizeTable(this)'>" + other + "</td>");
                        }
                        sb.Append("</tr>");
                    }


                    sb.Append("</table>");

                }



                #endregion


            }

            divHeader.InnerHtml = sb.ToString();

        }
        catch (Exception err)
        {
            Response.Write("<!--" + err.ToString() + "-->");
        }
    }
    public class Lu_Ch_Note
    {
        public long Id { get; set; }
        public string ColName { get; set; }

        public string Note { get; set; }
    }
    public string FilterNote(List<Lu_Ch_Note> arrData, string id, string colName)
    {
        var res = arrData.FirstOrDefault(w => w.Id == long.Parse(id) && w.ColName.ToLower() == colName.ToLower());
        if (res == null) return "";

        return res.Note;
    }
    public string Compare(string org, string newText, string note)
    {
        if (org == newText)
            return org;
        else
            return "<font>原:" + org + "</font><br><font color='red'>修:" + newText + "</font><br><font color='blue'>中:" + note + "</font>";
    }


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

    }

    private void Delete(string hdid)
    {
        SQLHelper sql = new SQLHelper();
        string sSql = "";

        sSql += "update PDFTAG.dbo.HistoryData set isshow=1    \n";
        sSql += " where hdid=@hdid     \n";



        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            cm.Parameters.AddWithValue("@hdid", hdid);

            cm.ExecuteNonQuery();
        }
    }



    protected void btnAdd_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        try
        {
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



            sSql = "update PDFTAG.dbo.P_inProcess    \n";
            sSql += " set ptitle=@ptitle,pver=@pver \n";
            if (!string.IsNullOrEmpty(sFilePath))
            {
                sSql += " ,piuploadfile=@piuploadfile \n";
            }


            sSql += "where pipid=@pipid                          \n";


            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                //cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                //cm.Parameters.AddWithValue("@ptitle", ptitle.Text);
                //cm.Parameters.AddWithValue("@pidate", dtNow);
                //if (!string.IsNullOrEmpty(sFilePath))
                //    cm.Parameters.AddWithValue("@piuploadfile", sFilePath);
                //cm.Parameters.AddWithValue("@pver", ddlpver.SelectedItem.Value);
                //cm.ExecuteNonQuery();




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


}