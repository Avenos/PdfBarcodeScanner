
using System;
using System.IO;
using System.Drawing.Imaging;
using Ghostscript.NET.Rasterizer;

namespace PDF_BarcodeScanner
{
    class Converter
    {
        /// <summary>Converts all PDF files in a folder to a PNG's at a certain DPI.</summary>
        /// <returns>Void</returns>
        public void PDFtoPNG(string inputDir, string outputDir, int dpi)
        {
            var dirInfo = new DirectoryInfo(inputDir);
            FileInfo[] PDFs = dirInfo.GetFiles("*.pdf");
            Directory.CreateDirectory(outputDir);
            using (var rasterizer = new GhostscriptRasterizer())
            {
                foreach (FileInfo pdf in PDFs)
                {
                    rasterizer.Open(pdf.FullName);
                    for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                    {
                        var pageFilePath = Path.Combine(outputDir, string.Format("p" + pageNumber + " " + pdf.Name + ".png"));
                        var img = rasterizer.GetPage(dpi, dpi, pageNumber);
                        img.Save(pageFilePath, ImageFormat.Png);
                    }
                }
            }
        }
    }
}
