using System;
using System.IO;

namespace Nintenlord.IO
{
    public static class IOHelpers
    {
        public static string FindFile(string currentFile, string newFile)
        {
            newFile = newFile.Trim('\"');

            if (File.Exists(newFile))
            {
                return newFile;
            }
            else if (!String.IsNullOrEmpty(currentFile))
            {
                var path = Path.GetDirectoryName(currentFile);
                path = Path.Combine(path, newFile);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return string.Empty;
        }

        public static char? ReadCharacter(this TextReader reader)
        {
            var value = reader.Read();
            if (value == -1)
            {
                return null;
            }
            else
            {
                return Convert.ToChar(value);
            }
        }

        public static char? PeekCharacter(this TextReader reader)
        {
            var value = reader.Peek();
            if (value == -1)
            {
                return null;
            }
            else
            {
                return Convert.ToChar(value);
            }
        }
    }
}
