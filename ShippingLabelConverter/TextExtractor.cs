using Tesseract;

namespace ShippingLabelConverter;

public class TextExtractor
{
    public static string ExtractText(string imagePath)
    {
        using (var engine = new TesseractEngine("/opt/homebrew/share/tessdata", "deu", EngineMode.Default))
        {
            using (var img = Pix.LoadFromFile(imagePath))
            {
                using (var page = engine.Process(img))
                {
                    Console.WriteLine(page.GetText());
                    var text = page.GetText();
                    return text;
                }
            }
        }
    }
}