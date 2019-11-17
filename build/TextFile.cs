using System.IO;
using System.Text;

class TextFile
{
    public static void ReplaceValues(string fileName, params (string searchTerm, string replacement)[] values)
    {
        string content = File.ReadAllText(fileName);

        foreach (var value in values)
        {
            content = content.Replace(value.searchTerm, value.replacement);
        }

        File.WriteAllText(fileName, content, new UTF8Encoding(false));
    }
}