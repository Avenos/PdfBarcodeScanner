
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp;
using ZXing;

namespace PDF_BarcodeScanner
{
    class Barcode
    {
        /// <summary>Attempts to locate a barcode in a file</summary>
        /// <returns>The rectangle containing the barcode or null</returns>
        public Bitmap FindBarcode(FileInfo file)
        {
            //load the image and convert it to grayscale
            Mat img = new Mat(file.FullName, ImreadModes.Grayscale);

            //compute the Scharr gradient magnitude representation of the images
            Mat gradX = new Mat(); Mat gradY = new Mat();
            Cv2.Sobel(img, gradX, MatType.CV_32F, 1, 0, -1);
            Cv2.Sobel(img, gradY, MatType.CV_32F, 0, 1, -1);

            //subtract the y-gradient from the x-gradient
            Mat gradient = new Mat();
            Cv2.Subtract(gradX, gradY, gradient);
            Cv2.ConvertScaleAbs(gradient, gradient);

            //blur and threshold the image
            Mat blurred = new Mat();
            OpenCvSharp.Size size1 = new OpenCvSharp.Size(9, 9);
            Cv2.Blur(gradient, blurred, size1);
            Mat thresh = new Mat();
            Cv2.Threshold(blurred, thresh, 225, 255, ThresholdTypes.Binary);

            //construct a closing kernel and apply it to the thresholded image
            OpenCvSharp.Size size2 = new OpenCvSharp.Size(21, 7);
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, size2);
            Mat closed = new Mat();
            Cv2.MorphologyEx(thresh, closed, MorphTypes.Close, kernel);

            //perform a series of erosions and dilations
            Mat element = new Mat();
            Cv2.Erode(closed, closed, element, null, 4);
            Cv2.Dilate(closed, closed, element, null, 4);

            //find the contours in the thresholded image, then sort the contours
            //by their area, keeping only the largest one
            OpenCvSharp.Point[][] contours = new OpenCvSharp.Point[9999][];
            HierarchyIndex[] h = new HierarchyIndex[9999];
            Cv2.FindContours(closed.Clone(), out contours, out h, 0, ContourApproximationModes.ApproxSimple);

            OpenCvSharp.Point[] biggestContour = getBiggestContour(contours);
            //compute the rotated bounding box of the largest two contours
            RotatedRect rect = Cv2.MinAreaRect(biggestContour);
            //RotatedRect rect2 = Cv2.MinAreaRect(secondBiggestContour);

            //convert from RotatedRect to Rect and double size
            System.Drawing.Size size = new System.Drawing.Size((int)(rect.Size.Width * 2), (int)(rect.Size.Height * 2));
            System.Drawing.Point upperLeft = new System.Drawing.Point((int)(rect.Center.X - rect.Size.Width), (int)(rect.Center.Y - rect.Size.Height));
            Rectangle cropRect1 = new Rectangle(upperLeft, size);

            Bitmap bitmap = Image.FromFile(file.FullName) as Bitmap;
            Bitmap cropped = cropAtRect(bitmap, cropRect1);
            cropped.Save(@"C:\Users\htthomas\Desktop\Labs\cropped\" + file.Name, ImageFormat.Png); //for testing

            return cropped;
        }

        /// <summary>Gets the biggest contour in a set</summary>
        /// <returns>The biggest contour</returns>
        public OpenCvSharp.Point[] getBiggestContour(OpenCvSharp.Point[][] contours)
        {
            double biggestContourArea = -99999;
            OpenCvSharp.Point[] biggestContour = null;
            foreach (OpenCvSharp.Point[] contour in contours)
            {
                if (Cv2.ContourArea(contour) > biggestContourArea)
                {
                    biggestContourArea = Cv2.ContourArea(contour);
                    biggestContour = contour;
                }
            }
            return biggestContour;
        }

        /// <summary>Crops a bitmap to a rectangle</summary>
        /// <returns>The cropped bitmap</returns>
        public Bitmap cropAtRect(Bitmap bitmap, Rectangle rect)
        {
            Bitmap cropped = new Bitmap(rect.Width, rect.Height);
            Graphics graphic = Graphics.FromImage(cropped);
            graphic.DrawImage(bitmap, -rect.X, -rect.Y);
            return cropped;
        }

        /// <summary>Attempts to read a barcode from a bitmap</summary>
        /// <returns>The data represented by the barcode, or null</returns>
        public Result ScanBarcode(Bitmap barcodeBitmap)
        {
            IBarcodeReader reader = new BarcodeReader();
            Result result = reader.Decode(barcodeBitmap); //multi decode?
            return result;
        }
    }
}
