
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
//This program has roughly a 50% accuracy rate at finding and scanning
//barcodes.  If it can't find a barcode in a PDF, it will move the PDF
//to another folder for it to be manually scanned by a person.  Fortunately,
//false positives are very rare; the program typically reads the barcodes
//correctly or not at all.
//
//Libraries:
//Ghostscript.NET for PDF to bitmap conversion
//OpenCvSharp for barcode recognition
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
            Converter converter = new Converter();
            Barcode barcode = new Barcode();

            Console.WriteLine("Enter the directory of PDFs:");
            //string directory = Console.ReadLine();
            converter.PDFtoPNG(@"C:\Users\htthomas\Desktop\Labs", @"C:\Users\htthomas\Desktop\Labs\png\", 192);

            //barcode finding and scanning
            DirectoryInfo dirInfo2 = new DirectoryInfo(@"C:\Users\htthomas\Desktop\Labs\png\");
            FileInfo[] PNGs = dirInfo2.GetFiles("*.png");

            Logger logger = new Logger();
            string outputFile = @"C:\Users\htthomas\Desktop\Labs\output.txt";

            double success = 0.0; double fail = 0.0;
            foreach (FileInfo png in PNGs)
            {
                Bitmap cropped = barcode.FindBarcode(png);
                ZXing.Result barcodeValue = barcode.ScanBarcode(cropped);

                if (barcodeValue != null)
                {
                    Console.WriteLine(png.Name + "---" + barcode.ScanBarcode(cropped));
                    logger.Log(png.Name + "---" + barcode.ScanBarcode(cropped), outputFile);
                    success += 1;
                }
                else
                {
                    Console.WriteLine(png.Name + "---" + "Barcode read failure!");
                    logger.Log(png.Name + "---" + "Barcode read failure!", outputFile);
                    fail += 1;
                }
            }
            Console.WriteLine("Accuracy: " + (100 * (success / (success + fail))) + "%");
        }
    }
}
