using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Net.Mail;

/// <summary>
/// MailUtil2 的摘要描述
/// </summary>
public class MailUtil2
{
    public MailUtil2()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    public void sendMailAction(string fromEmail, string password, string fromName, string subject, List<string> toMails, string template, Dictionary<string, string> data, List<string> ccMails = null)
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
        if (ccMails != null && ccMails.Any())
        {
            for (int i = 1; i < ccMails.Count; i++)
            {
                if (ccMails[i].Trim().Length > 0)
                    mail.CC.Add(new MailAddress(ccMails[i]));
            }
        }


        mail.Subject = subject;
        mail.SubjectEncoding = Encoding.UTF8;
        string body = template;
        foreach (string key in data.Keys)
        {
            body = body.Replace("${" + key + "}", data[key]);
        }

        //mail.Bcc.Add(new MailAddress("cgustlog@gmail.com"));

        mail.Body = body;
        mail.BodyEncoding = Encoding.UTF8;
        mail.IsBodyHtml = true;
        mail.Priority = MailPriority.High;

        using (SmtpClient client = new SmtpClient())
        {
            client.Host = "smtp.gmail.com";
            client.Port = 25;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(fromEmail, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = 60 * 1000;
            // Send Mail
            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                LogFile.Logger.LogException(e);
            }
        }
    }

}