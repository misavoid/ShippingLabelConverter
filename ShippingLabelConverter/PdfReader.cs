using Ghostscript.NET.Processor;
using IronOcr;

namespace ShippingLabelConverter;
using ImageMagick;

public class PdfReader
{
    
    // convert pdf to png
    public void ConvertPdfToImage(string pdfFilePath, string outputFilePath)
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
}
