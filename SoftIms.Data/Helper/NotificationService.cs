using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;


namespace SoftIms.Data.Helper
{
    
    public class NotificationService
    {        
        #region email
        public string SendEmail(Smtp smtp, string to, string subject, string body, List<Attachment> attachetments = null)
        {
            string result = string.Empty;
            try
            {
                string footer = "This e-mail is confidential and it is intended only for the addressees. Any review, dissemination, distribution, or copying of this message by persons or entities other than the intended recipient is prohibited. If you have received this e-mail in error, kindly notify us immediately by telephone or e-mail and delete the message from your system. The sender does not accept liability for any errors or omissions in the contents of this message which may arise as a result of the e-mail transmission.";
                string note = "Please do not reply to this email address.";
                string htmlBody = $"<html><body><div style='border:30px solid ccc; border-radius:14px; padding:10px;'><p style='margin:0; font-size:22px;'>{subject}</p><p>{body}</p><hr style='margin:0;'/><p style='color:#148ecd;font-size:14px;'>{note}</p><p style='font-size:10px;'>{footer}</p></div></body></html>";
                AlternateView view = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

                using (var db = new UnitOfWork())
                {
                    var fromAddress = new MailAddress(smtp.SMTPSender, smtp.SMTPSenderDisplayName);
                    var toAddress = new MailAddress(to, to);
                    MailMessage mailMessage = new MailMessage(fromAddress, toAddress);

                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.High;
                    mailMessage.Sender = fromAddress;
                    mailMessage.To.Add(toAddress);
                    if (attachetments != null && attachetments.Any())
                    {
                        foreach (var attachment in attachetments)
                        {
                            mailMessage.Attachments.Add(attachment);
                        }
                    }

                    mailMessage.Subject = subject;
                    mailMessage.AlternateViews.Add(view);

                    Task.Factory.StartNew(() =>{
                        SmtpClient smtpClient = new SmtpClient(smtp.SMTP, smtp.SMTPPort);
                        smtpClient.UseDefaultCredentials = false;
                        var credential = new NetworkCredential(smtp.SMTPSender, smtp.SMTPPassword);
                        smtpClient.Credentials = credential;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mailMessage);
                        smtpClient.Dispose();
                        mailMessage.Dispose();
                        return "success";
                    });
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return   "Failed to send mail due to <br/>" + ex;
            }
        }
        #endregion
    }
}
