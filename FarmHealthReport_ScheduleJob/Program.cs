using FarmHealthReport_ScheduleJob.DTOs;
using FarmHealthReport_ScheduleJob.Helpers;
using FarmHealthReport_ScheduleJob.Models;
using System.Text.RegularExpressions;

namespace FarmHealthReport_ScheduleJob
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DisplayProgramTitle();

            var docReportContentList = DocumentFile.ReadDocsFromLocalOneDrive();

            if (docReportContentList.Count != 0)
            {
                int i = 1;
                foreach (var docReport in docReportContentList)
                {
                    Console.WriteLine($"Document #{i}");
                    Console.WriteLine($"Name            : {docReport.FileName}");
                    Console.WriteLine($"Size            : {docReport.FileSize} bytes");
                    Console.WriteLine($"Last Modified   : {docReport.LastModifiedTime:d MMM yyyy, h:mm tt}");
                    Console.WriteLine("");

                    // Insert the document report data into the database
                    InsertDocReportDataIntoDatabase(docReport.FileContent);

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();
                    i = i + 1;
                }
            }
            else
            {
                ConsoleLogger.LogInfo("No document report found.");
            }

            Console.WriteLine();
            ConsoleLogger.LogInfo("Program execution completed.");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // Display the program title
        static void DisplayProgramTitle()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================");
            Console.WriteLine("         FARM HEALTH REPORT PROCESSOR             ");
            Console.WriteLine("==================================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        // Extract the data from the document report text body and insert data to database
        static void InsertDocReportDataIntoDatabase(string docText)
        {
            ConsoleLogger.LogStep("Starting database insertion process...");

            // Create a new instance of the database context to interact with the database
            using (var context = new FarmServerMonitoringDbTestContext())
            {
                // Create report data
                var report = InsertReportData(docText, context);

                // Check if the report has already existed in the database
                var isReportExist = context.ServerHealthReports.Where(a => a.Id == report.Id).Any();
                if (isReportExist)
                {
                    ConsoleLogger.LogInfo("Skipped: Duplicate report detected.");
                    return;
                }

                // Create collection table related data
                InsertCollectionTableData(docText, report.Id, context);

                // Create connection broker data
                InsertConnectionBrokerData(docText, report, context);

                context.SaveChanges();
            }

            ConsoleLogger.LogSuccess("Report inserted successfully.");
        }

        // Insert the report to the database
        static ServerHealthReport InsertReportData(string docText, FarmServerMonitoringDbTestContext context)
        {
            try
            {
                // Split the document text content into string array
                var docTextArray = docText.Split(["\r\n", "\n"], StringSplitOptions.None).Select(x => x.Trim()).Skip(1).ToArray();

                // Extract the report information
                var reportName = docTextArray.FirstOrDefault() ?? string.Empty;
                var scriptStartTime = Regex.Match(docText, @"Script Start time:\s*(.+)").Groups[1].Value;
                var scriptEndTime = Regex.Match(docText, @"Script End time:\s*(.+)").Groups[1].Value;

                // Create a farm server health report data
                var report = new ServerHealthReport()
                {
                    Id = reportName.Replace("RDS Health Report", "").Replace(" ", "") + "_" + DateTime.Parse(scriptStartTime).ToString("ddMMyyyy_HHmmss"),
                    ReportName = reportName,
                    ScriptStartTime = DateTime.Parse(scriptStartTime),
                    ScriptEndTime = DateTime.Parse(scriptEndTime),
                };

                context.ServerHealthReports.Add(report);
                return report;
            }
            catch (Exception ex)
            {
                ConsoleLogger.LogError("An unexpected error occurred.");
                Console.WriteLine($"Details: {ex.Message}\n");
            }
            return new();
        }

        // Insert the collection table to the database
        static void InsertCollectionTableData(string docText, string reportId, FarmServerMonitoringDbTestContext context)
        {
            // Split the document text content into string array
            var lines = docText.Split(["\r\n", "\n"], StringSplitOptions.None).Select(x => x.Trim()).Skip(1).ToArray();

            // Get the list of collection tables
            var collectionTables = SplitByCollectionTables(lines.ToList());

            // Action for each collection table
            foreach (var collectionTable in collectionTables)
            {
                // Get the collection name from table header
                var tableHeaderLine = collectionTable.First();
                var collectionName = Regex.Match(tableHeaderLine, @"Collection:\s*(\S+)", RegexOptions.IgnoreCase).Groups[1].Value;

                // Use the collection body to extract records, averages, and total sections
                var body = collectionTable.Skip(1).ToList();
                var (records, averages, totals) = ParseCollectionTableSection(body);

                // Create a collection table data
                var collection = new Collection
                {
                    ReportId = reportId,
                    Name = collectionName,
                    CpuUsageAvg = averages[0],
                    MemoryUsageAvg = averages[1],
                    CdriveFreeSpaceAvg = averages[2],
                    DdriveFreeSpaceAvg = averages[3],
                    SessionsTotalAvg = averages[6],
                    SessionsActiveAvg = averages[7],
                    SessionsDiscAvg = averages[8],
                    SessionsNullAvg = averages[9],
                    SessionsTotalSum = totals[1],
                    SessionsActiveSum = totals[2],
                    SessionsDiscSum = totals[3],
                    SessionsNullSum = totals[4]
                };
                context.Collections.Add(collection);
                context.SaveChanges();

                // Insert for the collection table's records data
                InsertCollectionRecordData(records, collection.Id, context);
            }
        }

        // Split the collection tables from the report text content
        static List<List<string>> SplitByCollectionTables(List<string> docTextArray)
        {
            var result = new List<List<string>>();
            List<string>? currentTable = null;

            foreach (var line in docTextArray)
            {
                if (line.StartsWith("Collection:", StringComparison.OrdinalIgnoreCase))
                {
                    currentTable = new List<string> { line };
                    result.Add(currentTable);
                }
                if (currentTable != null)
                    currentTable.Add(line);
            }
            return result;
        }

        public enum CollectionTableSection { Records, Averages, Totals }

        // Extract the records, averages, and totals sections from a collection table
        static (List<string> records, List<string> averages, List<string> totals) ParseCollectionTableSection(List<string> tableBody)
        {
            var records = new List<string>();
            var averages = new List<string>();
            var totals = new List<string>();

            bool isHeaderSkipped = false;
            var currentSection = CollectionTableSection.Records;

            foreach (var line in tableBody)
            {
                if (!isHeaderSkipped)
                {
                    if (line.Contains("Null"))  // Skip the header until "Null", the final header
                        isHeaderSkipped = true;
                    continue;
                }
                if (line.Contains("Average"))
                {
                    currentSection = CollectionTableSection.Averages;
                    continue;
                }
                if (line.Contains("Total"))
                {
                    currentSection = CollectionTableSection.Totals;
                    continue;
                }

                switch (currentSection)
                {
                    case CollectionTableSection.Records:
                        records.Add(line);
                        break;
                    case CollectionTableSection.Averages:
                        averages.Add(line);
                        break;
                    case CollectionTableSection.Totals:
                        totals.Add(line);
                        break;
                }
            }

            return (records, averages, totals);
        }

        // Insert the collection table's records to the database
        static void InsertCollectionRecordData(List<string> collectionRecords, int collectionId, FarmServerMonitoringDbTestContext context)
        {
            // Initialize the number of rows and columns in a collection table
            var numRow = collectionRecords.Count(x => x.Contains("MYPEN", StringComparison.OrdinalIgnoreCase));
            var numCol = 13; // 12 columns + 1 empty line

            try
            {
                // Loop through all the rows of a collection table
                for (int i = 0; i < numRow; i++)
                {
                    // Get one row of collection data
                    var collectionRow = collectionRecords.Skip(i * numCol).Take(numCol).ToList();

                    // Create a collection record
                    var collectionRecord = new CollectionRecord()
                    {
                        CollectionId = collectionId,
                        ServerName = collectionRow[0],
                        Enabled = collectionRow[1],
                        CpuUsage = collectionRow[2],
                        MemoryUsage = collectionRow[3],
                        CdriveFreeSpace = collectionRow[4],
                        DdriveFreeSpace = collectionRow[5],
                        Uptime = collectionRow[6],
                        PendingReboot = collectionRow[7],
                        SessionsTotal = collectionRow[8],
                        SessionsActive = collectionRow[9],
                        SessionsDisc = collectionRow[10],
                        SessionsNull = collectionRow[11]
                    };
                    context.CollectionRecords.Add(collectionRecord);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                ConsoleLogger.LogError("An unexpected error occurred.");
                Console.WriteLine($"Details: {ex.Message}\n");
            }
        }

        // Insert the connection brokers to the database
        static void InsertConnectionBrokerData(string docText, ServerHealthReport report, FarmServerMonitoringDbTestContext context)
        {
            // Get the connection brokers from the document report text
            var connectionBrokers = Regex.Match(docText, @"ConnectionBrokers:\s*(.+)").Groups[1].Value.Split([", "], StringSplitOptions.None).Select(x => x.Trim()).ToArray();

            foreach (var connectionBroker in connectionBrokers)
            {
                try
                {
                    // Check if the connection broker has already existed in the database
                    var existingConnectionBroker = context.ConnectionBrokers.Where(a => a.Name == connectionBroker).FirstOrDefault();

                    // Create the connection broker if it doesn't exist in database
                    if (existingConnectionBroker == null)
                    {
                        var newConnectionBroker = new ConnectionBroker()
                        {
                            Name = connectionBroker,
                            Reports = [report]
                        };
                        context.ConnectionBrokers.Add(newConnectionBroker);
                    }
                    else
                        existingConnectionBroker.Reports.Add(report);
                }
                catch (Exception ex)
                {
                    ConsoleLogger.LogError("An unexpected error occurred.");
                    Console.WriteLine($"Details: {ex.Message}\n");
                }
            }
        }
    }
}