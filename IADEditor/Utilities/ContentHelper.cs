using System.IO;
using System.Text;

namespace IADEditor.Utilities;

public static class ContentHelper
{
    public static string GetRandomString(int length = 8)
    {
        if (length <= 0) { length = 8; }

        var n = length / 11;
        var sb = new StringBuilder();
        for (int i = 0; i <= n; ++i)
        {
            sb.Append(Path.GetRandomFileName().Replace(".", ""));   
        }

        return sb.ToString(0, length);
    }
}