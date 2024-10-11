using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    // Setup the path to file that should be split, and call the SplitMarkdownIntoSections method
    // adjust the regext for the header to match the number of # you want to split on
    static void Main(string[] args)
    {
        string filePath = @"file.md";
        var sections = SplitMarkdownIntoSections(filePath);

        int sectionNum = 1;
        var sourceFile = Path.GetFileNameWithoutExtension(filePath);
        foreach (var section in sections)
        {
            // write section to file
            string header = section.Key;
            string content = section.Value;

            // clean header to use in filename
            header = header.Replace("#", "").Trim();

            // remove invalid characters from header according to filesystem
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                header = header.Replace(c.ToString(), "");
            }
            header = header.Replace(" ", "").Trim();

            // prefix the filename with the name of the original file and a number for uniqueness
            var fileName = $"{sourceFile}_{sectionNum}_{header}.md";

            File.WriteAllText(fileName, content);

            sectionNum++;
        }
    }

    static Dictionary<string, string> SplitMarkdownIntoSections(string filePath)
    {
        var sections = new Dictionary<string, string>();
        var lines = File.ReadAllLines(filePath);
        string currentHeader = null;
        var contentBuilder = new List<string>();

        foreach (var line in lines)
        {
            if (Regex.IsMatch(line, @"^#{2,3} "))
            {
                if (currentHeader != null)
                {
                    contentBuilder.Insert(0, currentHeader);
                    sections[currentHeader] = string.Join(Environment.NewLine, contentBuilder);
                    contentBuilder.Clear();
                }
                currentHeader = line;
            }
            else
            {
                contentBuilder.Add(line);
            }
        }

        if (currentHeader != null)
        {
            sections[currentHeader] = string.Join(Environment.NewLine, contentBuilder);
        }

        return sections;
    }
}