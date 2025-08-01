using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;

namespace FarmHealthReport_ScheduleJob.DTOs
{
    public class MailKitReader
    {
        public string? EmailFrom { get; set; }
        public string? EmailSubject { get; set; }
        public DateTime? EmailReceivedTime { get; set; }
        public string? EmailBody { get; set; }

        public static List<MailKitReader> ReadOutlookEmails()
        {
            var emails = new List<MimeMessage>();
            List<MailKitReader> listEmailDetails = new List<MailKitReader>();

            using (var client = new ImapClient())
            {
                // Connect to Office365 IMAP
                client.Connect("outlook.office365.com", 993, SecureSocketOptions.SslOnConnect);
                client.Authenticate("Kok_Yeow@jabil.com", "");

                // Open the Inbox
                var inbox = client.Inbox;
                inbox.Open(MailKit.FolderAccess.ReadWrite);

                // Search for unread messages with specific subject
                //var uids = inbox.Search(SearchQuery.NotSeen.And(SearchQuery.SubjectContains("PEN7-2 RDS Health report - Asia")));

                int maxEmails = 2;
                int start = inbox.Count - 1;
                int end = Math.Max(0, inbox.Count - maxEmails);

                for (int i = start; i >= end; i--)
                {
                    var message = inbox.GetMessage(i);
                    emails.Add(message);

                    var emailDetails = new MailKitReader
                    {
                        EmailFrom = message.From.ToString(),
                        EmailSubject = message.Subject,
                        EmailReceivedTime = message.Date.DateTime,
                        EmailBody = message.TextBody ?? message.HtmlBody
                    };
                    listEmailDetails.Add(emailDetails);
                }

                //foreach (var uid in uids)
                //{
                //    var message = inbox.GetMessage(uid);
                //    emails.Add(message);

                //    var emailDetails = new MailKitReader
                //    {
                //        EmailFrom = message.From.ToString(),
                //        EmailSubject = message.Subject,
                //        EmailReceivedTime = message.Date.DateTime,
                //        EmailBody = message.TextBody ?? message.HtmlBody
                //    };
                //    listEmailDetails.Add(emailDetails);

                //    // Mark as read
                //    inbox.AddFlags(uid, MessageFlags.Seen, true);
                //}

                client.Disconnect(true);
            }

            return listEmailDetails;
        }
    }
}