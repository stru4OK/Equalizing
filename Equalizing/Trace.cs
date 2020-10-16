using System;
using System.IO;

namespace Equalizing
{
    public class Trace
    {
        public static int TraceWrite(string text)
        {
            StreamWriter files = new StreamWriter("Trace.txt", true);

            files.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : \n" + text + "\n");
            files.Flush();
            files.Close();
            return 0;
        }
    }
}
