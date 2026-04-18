namespace MyFirstChatUI.Models
{
    public class CoffeeData
    {
        private readonly IWebHostEnvironment _environment;

        public CoffeeData(IWebHostEnvironment environment)
        {
            _environment = environment;
            DataPath = Path.Combine(_environment.WebRootPath, "data");
        }

        public string DataPath { get; }

        /// <summary>
        /// Reads all markdown files from wwwroot/data directory and returns their contents as an array of strings
        /// </summary>
        /// <returns>Array of strings containing the content of each markdown file</returns>
        public async Task<string[]> ReadMarkdownFilesAsync()
        {
            if (!Directory.Exists(DataPath))
            {
                return Array.Empty<string>();
            }

            var markdownFiles = Directory.GetFiles(DataPath, "*.md");
            var contents = new List<string>();

            foreach (var filePath in markdownFiles)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(filePath);
                    contents.Add(content);
                }
                catch (Exception ex)
                {
                    // Log the error if needed, but continue processing other files
                    Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                }
            }

            return contents.ToArray();
        }

        /// <summary>
        /// Gets the filenames of all markdown files in the wwwroot/data directory
        /// </summary>
        /// <returns>Array of filenames (without full path)</returns>
        public string[] GetMarkdownFileNames()
        {
            if (!Directory.Exists(DataPath))
            {
                return Array.Empty<string>();
            }

            var markdownFiles = Directory.GetFiles(DataPath, "*.md");
            return markdownFiles.Select(Path.GetFileName).Where(name => name != null).ToArray()!;
        }

        /// <summary>
        /// Reads all markdown files and returns them as a dictionary with filename as key and content as value
        /// </summary>
        /// <returns>Dictionary with filename as key and file content as value</returns>
        public async Task<Dictionary<string, string>> ReadMarkdownFilesAsDictionaryAsync()
        {
            var result = new Dictionary<string, string>();
            
            if (!Directory.Exists(DataPath))
            {
                return result;
            }

            var markdownFiles = Directory.GetFiles(DataPath, "*.md");

            foreach (var filePath in markdownFiles)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(filePath);
                    var fileName = Path.GetFileName(filePath);
                    result[fileName] = content;
                }
                catch (Exception ex)
                {
                    // Log the error if needed, but continue processing other files
                    Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                }
            }

            return result;
        }
    }
}
