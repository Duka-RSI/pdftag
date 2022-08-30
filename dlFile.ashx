<%@ WebHandler Language="C#" Class="dlFile" %>

using System;
using System.Web;

public class dlFile : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        String fileID = context.Request.QueryString["id"];
        String type = "";

        File userpic = null;
        try
        {
            userpic = FilesDao.getFile(int.Parse(fileID));
        }
        catch (Exception ex)
        {
            context.Response.End();
            return;
        }
        
        if (userpic == null)
        {
            context.Response.End();
            return;
        }

        string path = System.Web.Configuration.WebConfigurationManager.AppSettings["UploadFilePath"];

        context.Response.ClearContent();
        string contentDisposition = String.Format("{0}; filename=\"{1}\"", "attachment", userpic.filename);
        context.Response.AddHeader("Content-Disposition", contentDisposition);
        //context.Response.ContentType = "application/download";
        type = userpic.filename.Substring(userpic.filename.LastIndexOf('.')).ToLower();
        //Console.WriteLine(type);
        if (".jpg".Equals(type))
        {
            context.Response.ContentType = "image/jpeg";
        }
        else if (".jpeg".Equals(type))
        {
            context.Response.ContentType = "image/jpeg";
        }
        else if (".gif".Equals(type))
        {
            context.Response.ContentType = "image/gif";
        }
        else if (".pdf".Equals(type))
        {
            context.Response.ContentType = "application/pdf";
        }
        else
        {
            context.Response.ContentType = "application/octet-stream";
        }
        string filePath = path + fileID;
        System.IO.FileInfo f = new System.IO.FileInfo(filePath);
        context.Response.AddHeader("Content-Length", f.Length.ToString());
        context.Response.Flush();
        context.Response.WriteFile(filePath, false);
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
