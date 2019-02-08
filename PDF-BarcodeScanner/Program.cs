
/////////////////////////////////////////////////////////////////////////////
//PDF BARCODE SCANNER
//
//Henry Thomas, Analyst/Programmer II
//State of Alaska
//Department of Health and Social Services
//henry.thomas@alaska.gov
//
//Description:
//This program is designed to read barcodes from PDF documents.  It
//does this by first converting the PDFs to bitmaps.  Then, it finds
//where the barcode is in the bitmap and crops it out.  It can then
//be easily scanned by ZXing (the barcode reading library).
//
//Notes:
//This program has roughly a xx% accuracy rate at finding and scanning
//barcodes.  If it can't find a barcode in a PDF, it will move the PDF
//to another folder for it to be manually scanned by a person.  Fortunately,
//false positives are very rare; the program typically reads the barcodes
//correctly or not at all.
//
//Libraries:
//Ghostscript.NET for PDF to bitmap conversion
//OpenCvSharp for barcode locating
//ZXing.NET for barcode scanning
//
//Helpful Resources:
//https://www.pyimagesearch.com/2014/11/24/detecting-barcodes-images-python-opencv/
//(used to construct the FindBarcode method)
//http://www.ghostscript.com/download/gsdnld.html
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Drawing;

namespace PDF_BarcodeScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            var converter = new Converter();
            var barcode = new Barcode();
            var logger = new Logger();

            Console.WriteLine("Enter the directory of PDFs:");
            string directory = Console.ReadLine();
            //directory = "C:\Users\htthomas\Desktop\Labs"
            converter.PDFtoPNG(directory, $"{directory}\\png", 192);

            //barcode locating and scanning
            var pngDirInfo = new DirectoryInfo($"{directory}\\png");
            FileInfo[] pngFileInfo = pngDirInfo.GetFiles("*.png");

            var outputFile = $"{directory}\\output.txt";

            double successes = 0.0; double failures = 0.0;
            foreach (FileInfo png in pngFileInfo)
            {
                Bitmap cropped = barcode.FindBarcode(png);
                ZXing.Result barcodeValue = barcode.ScanBarcode(cropped);

                if (barcodeValue != null)
                {
                    string barcodeString = barcodeValue.ToString();
                    int index = barcodeString.IndexOf("-");
                    if (index > 0)
                        barcodeString = barcodeString.Substring(0, index);

                    string subStr = png.Name.Substring(3, png.Name.Length - 6);
                    System.IO.File.Copy($"{directory}\\{subStr}", $"{directory}\\scanned\\{barcodeString}.pdf");

                    Console.WriteLine(png.Name + " - " + barcodeString);
                    logger.Log(png.Name + " - " + barcodeString, outputFile);
                    successes += 1;
                }
                else
                {
                    Console.WriteLine(png.Name + " - " + "Barcode read failure!");
                    logger.Log(png.Name + " - " + "Barcode read failure!", outputFile);
                    failures += 1;
                }
            }
            Console.WriteLine("Accuracy: " + (100 * (successes / (successes + failures))) + "%\n");
            logger.Log("Accuracy: " + (100 * (successes / (successes + failures))) + "%\n", outputFile);
        }
    }
}
