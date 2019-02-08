
using System;
using System.IO;

namespace PDF_BarcodeScanner
{
    class Logger
    {
        public void Log(string line, string outputFile)
        {
            using (StreamWriter file = new StreamWriter(outputFile, true))
            {
                file.WriteLine(line);
            }
        }
    }
}
