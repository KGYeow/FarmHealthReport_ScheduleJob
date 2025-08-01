using Microsoft.Office.Interop.Outlook;
using System.Reflection;

namespace FarmHealthReport_ScheduleJob.DTOs
{
    public class OutlookEmails
    {
        public string? EmailFrom { get; set; }
        public string? EmailSubject { get; set; }
        public DateTime? EmailReceivedTime { get; set; }
        public string? EmailBody { get; set; }

        public static List<OutlookEmails> ReadMailItems(string readStatus) 
        {
            Application? outlookApplication = null;
            NameSpace? outlookNamespace = null;
            MAPIFolder? inboxFolder = null;

            Items? mailItems = null;
            List<OutlookEmails> listEmailDetails = [];
            OutlookEmails emailDetails;

            try
            {
                outlookApplication = new Application();
                outlookNamespace = outlookApplication.GetNamespace("MAPI");
                outlookNamespace.Logon("Outlook", Missing.Value, false, true);
                //inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
                inboxFolder = outlookNamespace.Folders[1].Folders["Farm Health Report"];

                if (readStatus == "Read")
                    mailItems = inboxFolder.Items.Restrict("[Unread]=false");
                else if (readStatus == "Unread")
                    mailItems = inboxFolder.Items.Restrict("[Unread]=true");
                else
                    mailItems = inboxFolder.Items;

                // Retrieve and freeze all the email before processing
                List<MailItem> fixedMailItems = [];
                foreach (MailItem item in mailItems)
                {
                    if (item.Subject == "PEN7-2 RDS Health report - Asia")
                        fixedMailItems.Add(item);
                }   

                // Now process after emails are frozen
                foreach (MailItem item in fixedMailItems)
                {
                    emailDetails = new OutlookEmails
                    {
                        EmailFrom = item.SenderEmailAddress,
                        EmailSubject = item.Subject,
                        EmailReceivedTime = item.ReceivedTime,
                        EmailBody = item.Body
                    };
                    listEmailDetails.Add(emailDetails);

                    item.UnRead = false;
                    item.Save();
                    ReleaseComObject(item);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                ReleaseComObject(mailItems);
                ReleaseComObject(inboxFolder);
                ReleaseComObject(outlookNamespace);
                ReleaseComObject(outlookApplication);
            }
            return listEmailDetails;
        }

        private static void ReleaseComObject(object? obj) 
        {
            if (obj != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
        }
    }
}