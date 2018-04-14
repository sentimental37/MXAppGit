using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MXApp.MobileService.Helpers
{
    public class EmailSender
    {
        public static void SendEmail(WMS_Explorer settings, List<string> addresses, List<string> attachments, string subject = "", int? RefNum = 0)
        {
            try
            {
                NetworkCredential cred = new NetworkCredential(settings.WMS_Mail_UserEmail, settings.WMS_Mail_Password);
                SmtpClient client = new SmtpClient(settings.WMS_Mail_HostName);
                client.Port = settings.WMS_Mail_Port.GetValueOrDefault();
                client.EnableSsl = settings.WMS_Mail_EnableSSL.GetValueOrDefault();
                client.Credentials = cred;
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(settings.WMS_Mail_UserEmail);
                foreach (var item in addresses)
                {
                    msg.To.Add(item);
                }
                foreach (var item in attachments)
                {
                    msg.Attachments.Add(new Attachment(item));
                }
                msg.Subject = subject;
                msg.Body = EmailBodyFormatter.GetAllocationReportEmailBody();
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;
                client.Send(msg);
            }
            catch (Exception ex)
            {
            }
        }

        public static void SendReportEmail(WMS_Explorer settings, List<string> To, string attachment, string subject)
        {
            try
            {
                NetworkCredential cred = new NetworkCredential(settings.WMS_Mail_UserEmail, settings.WMS_Mail_Password);
                SmtpClient client = new SmtpClient(settings.WMS_Mail_HostName);
                client.Port = settings.WMS_Mail_Port.GetValueOrDefault();
                client.EnableSsl = settings.WMS_Mail_EnableSSL.GetValueOrDefault();
                client.Credentials = cred;
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(settings.WMS_Mail_UserEmail);
                foreach (var item in To)
                {
                    msg.To.Add(item);
                }
                msg.Attachments.Add(new Attachment(attachment));
                msg.Subject = subject;
                msg.Body = "Please find the attached " + subject;
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;
                client.Send(msg);
            }
            catch (Exception ex)
            {
            }
        }

        public static void SendWorkOrderReportEmail(WMS_Explorer settings, List<string> To, string attachment, string subject)
        {
            try
            {
                NetworkCredential cred = new NetworkCredential(settings.WMS_Mail_UserEmail, settings.WMS_Mail_Password);
                SmtpClient client = new SmtpClient(settings.WMS_Mail_HostName);
                client.Port = settings.WMS_Mail_Port.GetValueOrDefault();
                client.EnableSsl = settings.WMS_Mail_EnableSSL.GetValueOrDefault();
                client.Credentials = cred;
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(settings.WMS_Mail_UserEmail);
                foreach (var item in To)
                {
                    msg.To.Add(item);
                }
                msg.Attachments.Add(new Attachment(attachment));
                msg.Subject = subject;
                msg.Body = "Please find the attached " + subject;
                msg.IsBodyHtml = true;
                msg.Priority = MailPriority.High;
                client.Send(msg);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
