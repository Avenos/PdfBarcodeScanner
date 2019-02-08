
PDF BARCODE SCANNER

Henry Thomas, Analyst/Programmer II
State of Alaska
Department of Health and Social Services
henry.thomas@alaska.gov

Description:
This program is designed to read barcodes from PDF documents.  It
does this by first converting the PDFs to bitmaps.  Then, it finds
where the barcode is in the bitmap and crops it out.  It can then
be easily scanned by ZXing (the barcode reading library).

Notes:
This program has roughly a xx% accuracy rate at finding and scanning
barcodes.  If it can't find a barcode in a PDF, it will move the PDF
to another folder for it to be manually scanned by a person.  Fortunately,
false positives are very rare; the program typically reads the barcodes
correctly or not at all.

Libraries:
Ghostscript.NET for PDF to bitmap conversion
OpenCvSharp for barcode locating
ZXing.NET for barcode scanning

THE GHOSTSCRIPT 32-BIT RASTERIZER NEEDS TO BE INSTALLED ON YOUR MACHINE FOR THIS PROGRAM TO WORK
https://github.com/ArtifexSoftware/ghostpdl-downloads/releases/download/gs926/gs926aw32.exe

Helpful Resources:
https://www.pyimagesearch.com/2014/11/24/detecting-barcodes-images-python-opencv/
(used to construct the FindBarcode method)
