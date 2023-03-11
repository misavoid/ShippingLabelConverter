using Ghostscript.NET.Processor;
using Tesseract;

namespace ShippingLabelConverter;
using ImageMagick;

public class PdfReader
{
    public void handlePdf(string pdfFilePath, string outputFilePath)
    {
        ConvertPdfToImage(pdfFilePath, outputFilePath);
        ExtractText(outputFilePath);
        
    }
    
    
    // convert pdf to png
    private static void ConvertPdfToImage(string pdfFilePath, string outputFilePath)
    {
        
        var images = new MagickImageCollection();
        {
            var settings = new MagickReadSettings();
            {
                settings.Density = new Density(300, 300);
                settings.FrameIndex = 0;
                settings.FrameCount = 1;
            }
            // Add all the pages of the PDF to the collection
            images.Read(pdfFilePath, settings);

            
            // Select the first page of the PDF
            var firstPage = images[0];

            using (var horizontal = images)
            {
                // save file as horizontal png, overwrite if exists
                var fileStream = new FileStream(outputFilePath, FileMode.Create);
                {
                    horizontal.Write(outputFilePath);
                }
            }
            
        }
    }
    
    // extract text from png with OCR (tesseract)
    private static void ExtractText(string outputFilePath)
    {
        var engine = new TesseractEngine(@"./tessdata", "deu", EngineMode.Default);
        using var img = Pix.LoadFromFile(outputFilePath);
        using var page = engine.Process(img);
        string text = page.GetText();
        Console.WriteLine(text);
    }
}