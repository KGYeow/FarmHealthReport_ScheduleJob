//using Microsoft.Graph;
//using Microsoft.Graph.Models;
//using Microsoft.Identity.Client;
//using System.Net.Http.Headers;

//namespace FarmHealthReport_ScheduleJob.DTOs
//{
//    public class MsGraphEmailReader
//    {
//        public string? EmailFrom { get; set; }
//        public string? EmailSubject { get; set; }
//        public DateTime? EmailReceivedTime { get; set; }
//        public string? EmailBody { get; set; }

//        public static async Task<List<MsGraphEmailReader>> ReadMailItemsAsync(string readStatus)
//        {
//            var listEmailDetails = new List<MsGraphEmailReader>();

//            try
//            {
//                var tenantId = "YOUR_TENANT_ID";
//                var clientId = "YOUR_CLIENT_ID";
//                var clientSecret = "YOUR_CLIENT_SECRET";
//                var userEmail = "Kok_Yeow@jabil.com"; // Replace with the mailbox to read from

//                var confidentialClient = ConfidentialClientApplicationBuilder
//                    .Create(clientId)
//                    .WithTenantId(tenantId)
//                    .WithClientSecret(clientSecret)
//                    .Build();

//                //var authProvider = new ClientCredentialProvider(confidentialClient);
//                //var graphClient = new GraphServiceClient(authProvider);
//                var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(async request =>
//                {
//                    var token = await confidentialClient
//                        .AcquireTokenForClient(["https://graph.microsoft.com/.default"])
//                        .ExecuteAsync();

//                    request.Headers.Authorization =
//                        new AuthenticationHeaderValue("Bearer", token.AccessToken);
//                }));

//                // Get the folder ID for "Farm Health Report"
//                var folders = await graphClient.Users[userEmail].MailFolders.GetAsync();

//                var targetFolder = folders.Value.FirstOrDefault(f => f.DisplayName == "Farm Health Report");
//                if (targetFolder == null)
//                    throw new Exception("Folder 'Farm Health Report' not found.");

//                // Build the filter
//                //string filter = "subject eq 'PEN7-2 RDS Health report - Asia'";
//                //if (readStatus == "true")
//                //    filter += " and isRead eq true";
//                //else if (readStatus == "false")
//                //    filter += " and isRead eq false";

//                // Get messages
//                //var messages = await graphClient.Users[userEmail].MailFolders[targetFolder.Id].Messages
//                //    .Request()
//                //    .Filter(filter)
//                //    .Top(50)
//                //    .Select("sender,subject,receivedDateTime,body,isRead")
//                //    .OrderBy("receivedDateTime desc")
//                //    .GetAsync();
//                var messages = await graphClient
//                    .Users[userEmail]
//                    .MailFolders[targetFolder.Id]
//                    .Messages
//                    .GetAsync(requestConfig =>
//                    {
//                        var qp = requestConfig.QueryParameters;
//                        qp.Filter = "subject eq 'PEN7-2 RDS Health report - Asia'" + (
//                            readStatus == "read" ? " and isRead eq true"
//                            : readStatus == "unread" ? " and isRead eq false"
//                            : ""
//                        );
//                        qp.Top = 50;
//                        qp.Select = ["sender", "subject", "receivedDateTime", "body", "isRead"];
//                        qp.Orderby = ["receivedDateTime desc"];
//                    });

//                foreach (var message in messages.Value)
//                {
//                    var emailDetails = new MsGraphEmailReader
//                    {
//                        EmailFrom = message.Sender?.EmailAddress?.Address,
//                        EmailSubject = message.Subject,
//                        EmailReceivedTime = message.ReceivedDateTime?.DateTime ?? DateTime.MinValue,
//                        EmailBody = message.Body?.Content
//                    };

//                    listEmailDetails.Add(emailDetails);

//                    // Mark as read if needed
//                    if (message.IsRead == false)
//                    {
//                        await graphClient.Users[userEmail].Messages[message.Id].PatchAsync(new Message { IsRead = true });
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error: " + ex.Message);
//            }

//            return listEmailDetails;
//        }
//    }
//}