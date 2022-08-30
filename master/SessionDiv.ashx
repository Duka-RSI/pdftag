<%@ WebHandler Language="C#" Class="Session" %>

using System;
using System.Web;
using Newtonsoft.Json;
using  System.Web.SessionState;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

public class SessionDiv : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {

        string sID=context.Request["id"];


        context.Session["Div"] = sID;

        context.Response.Write(sID);

    }
	
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}

public class result
    {
        public string id { get; set; }
    }
