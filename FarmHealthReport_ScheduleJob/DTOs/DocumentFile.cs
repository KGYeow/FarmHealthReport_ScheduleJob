using FarmHealthReport_ScheduleJob.Helpers;
using HtmlAgilityPack;

namespace FarmHealthReport_ScheduleJob.DTOs
{
    public class DocumentFile
    {
        public string FileName { get; set; } = string.Empty;
        public string FileContent { get; set; } = string.Empty;
        public long FileSize { get; set; } // in bytes
        public DateTime LastModifiedTime { get; set; }

        // Retrieve the list of document file report text content from synced local OneDrive
        public static List<DocumentFile> ReadDocsFromLocalOneDrive()
        {
            var allDocs = new List<DocumentFile>();

            try
            {
                ConsoleLogger.LogStep("Retrieving document file reports from local folder...");

                // Get all the htm files' path from the folder path
                string folderPath = @"C:\Users\4093094\Jabil\NurulNajihah AbdulRahim - FARM HEALTH DATA";
                ConsoleLogger.LogInfo($"Folder path: {folderPath}");

                var filePaths = Directory.GetFiles(folderPath, "*.htm");
                ConsoleLogger.LogInfo($"{filePaths.Length} document(s) found.");

                foreach (var filePath in filePaths)
                {
                    var fileInfo = new FileInfo(filePath);
                    var doc = new HtmlDocument();
                    doc.Load(filePath);

                    // Extract document content in text string
                    allDocs.Add(new DocumentFile()
                    {
                        FileName = Path.GetFileName(filePath),
                        FileContent = doc.DocumentNode.InnerText,
                        FileSize = fileInfo.Length,
                        LastModifiedTime = fileInfo.LastWriteTime,
                    });
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.LogError("An unexpected error occurred.");
                Console.WriteLine($"Details: {ex.Message}");
            }

            Console.WriteLine();
            return allDocs;
        }
    }
}