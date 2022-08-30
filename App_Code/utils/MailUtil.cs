using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;

/// <summary>
/// DbUtil 的摘要描述
/// </summary>
public class MailUtil
{

    public static void sendMail(string fromEmail, string password, string fromName, string subject, List<string> toMails, string template, Dictionary<string, string> data)
    {
        if (toMails == null || toMails.Count <= 0)
        {
            return;
        }
        MailAddress from = new MailAddress(fromEmail, fromName, Encoding.UTF8);
        MailMessage mail = new MailMessage(from, new MailAddress(toMails[0]));
        for (int i = 1; i < toMails.Count; i++)
        {
            if (toMails[i].Trim().Length > 0)
                mail.To.Add(new MailAddress(toMails[i]));
        }
        mail.Subject = subject;
        mail.SubjectEncoding = Encoding.UTF8;
        string body = template;
        foreach (string key in data.Keys)
        {
            body = body.Replace("${" + key + "}", data[key]);
        }

        mail.Body = body;
        mail.BodyEncoding = Encoding.UTF8;
        mail.IsBodyHtml = false;
        mail.Priority = MailPriority.High;
        // SMTP Setting
        //SmtpClient client = new SmtpClient( );
        //client.Host = "smtp.gmail.com";
        //client.Port = 587;
        //client.UseDefaultCredentials = false;
        //client.Credentials = new NetworkCredential(fromEmail, password);
        //client.EnableSsl = true;
        //client.DeliveryMethod = SmtpDeliveryMethod.Network;

        //var client = new SmtpClient("smtp.gmail.com", 587)
        //{
        //    Credentials = new NetworkCredential(fromEmail, password),
        //    EnableSsl = true
        //};

        using (SmtpClient client = new SmtpClient())
        {
            //client.Host = "smtp.gmail.com";
            //client.Port = 25;
            client.Host = "smtp.ntut.edu.tw";
            client.Port = 25;
            client.EnableSsl = false;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(fromEmail, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = 60 * 1000;
            // Send Mail
            client.Send(mail);
        }
    }

}