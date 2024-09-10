namespace ShippingLabelConverter;
using ZXing;
using ZXing.Common;
using SkiaSharp;

public class BarcodeExtractor
{
    public List<LuminanceSource> ExtractBarcodes(string imagePath)
    {
        // var barcodes = new List<string>();
        var croppedLuminanceSources = new List<LuminanceSource>();
        
        // Initialize the BarcodeReaderGeneric to support multi-barcode detection
        var barcodeReader = new BarcodeReaderGeneric
        {
            Options = new DecodingOptions
            {
                TryHarder = true,  // Enable better barcode detection
                ReturnCodabarStartEnd = true,
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.QR_CODE,  // Support for QR codes
                    BarcodeFormat.CODE_128, // Support for CODE_128
                    BarcodeFormat.CODE_39,  // Support for CODE_39
                    BarcodeFormat.EAN_13,   // Support for EAN-13
                    BarcodeFormat.ITF,       // Support for ITF
                    BarcodeFormat.ITF-14    // just to be sure
                },
                PureBarcode = false,
                AssumeCode39CheckDigit = true
            }
        };

        // Load the image into an SKBitmap using SkiaSharp
        using (var barcodeImage = SKBitmap.Decode(imagePath))
        {
            // Create the custom SkiaLuminanceSource
            var luminanceSource = new SkiaLuminanceSource(barcodeImage);

            // Create a BinaryBitmap based on the LuminanceSource - i dont think i need this.
            // var binaryBitmap = new BinaryBitmap(new HybridBinarizer(luminanceSource));

            // Use DecodeMultiple to extract all barcodes in the image
            var results = barcodeReader.DecodeMultiple(luminanceSource);

            if (results != null && results.Length > 0)
            {
                foreach (var result in results)
                {
                    Console.WriteLine($"Detected Barcode: {result.Text}");
                    
                    // log bounding box points for each barcode
                    foreach (var point in result.ResultPoints)
                    {
                        Console.WriteLine($"Point: X = {point.X}, Y = {point.Y}");
                    }
                    
                    // Crop the barcode region from the luminance source
                    int left = result.ResultPoints.Min(p => (int)p.X);
                    int right = result.ResultPoints.Max(p => (int)p.X);
                    int top = result.ResultPoints.Min(p => (int)p.Y);
                    int bottom = result.ResultPoints.Max(p => (int)p.Y);

                    Console.WriteLine($"Bounding box: Left = {left}, Right = {right}, Top = {top}, Bottom = {bottom}");
                    
                    // Crop the area of the barcode
                    LuminanceSource croppedSource = luminanceSource.crop(left, top, right - left, bottom - top);

                    // Store the cropped luminance source for further use
                    croppedLuminanceSources.Add(croppedSource);
                }
            }
            else
            {
                Console.WriteLine("No barcodes detected.");
            }
        }

        return croppedLuminanceSources;
    }
    
    // Helper method to save luminanceSource as an image
    private void SaveLuminanceSourceAsImage(SkiaLuminanceSource luminanceSource, string outputPath)
    {
        // Expand the tilde (~) to the user's home directory
        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string fullOutputPath = outputPath.Replace("~", homePath);

        int width = luminanceSource.Width;
        int height = luminanceSource.Height;

        using (var bitmap = new SKBitmap(width, height))
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get the luminance (grayscale) value
                    byte luminance = luminanceSource.getRow(y, null)[x];

                    // Set the pixel color based on the luminance (R=G=B=luminance)
                    bitmap.SetPixel(x, y, new SKColor(luminance, luminance, luminance));
                }
            }

            // Save the bitmap as a PNG
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(fullOutputPath))
            {
                data.SaveTo(stream);
            }
        }

        Console.WriteLine($"Luminance source image saved as {fullOutputPath}");
    }
}