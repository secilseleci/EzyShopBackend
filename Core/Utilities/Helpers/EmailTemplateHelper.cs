namespace Core.Utilities.Helpers
{
    public static class EmailTemplateHelper
    {
        public static async Task<string> GetTemplateContentAsync(string templateName, Dictionary<string, string> replacements)
        {
 

            try
            {
                var templatePath = Path.Combine(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName,
                    "Core", "Resources", "EmailTemplates", templateName
                );
                if (!File.Exists(templatePath))
                { 
                    throw new FileNotFoundException($"⚠️ Email template not found: {templatePath}");
                }
 
                var content = await File.ReadAllTextAsync(templatePath);

                foreach (var replacement in replacements)
                { 
                    content = content.Replace($"{{{replacement.Key}}}", replacement.Value);
                }

                return content;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}

