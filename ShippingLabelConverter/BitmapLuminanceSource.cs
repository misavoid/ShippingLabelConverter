namespace ShippingLabelConverter;
using ZXing;
using ZXing.Common;
using SkiaSharp; 

public class SkiaLuminanceSource : LuminanceSource
{
    private readonly byte[] _luminanceMatrix;

    public SkiaLuminanceSource(SKBitmap bitmap) : base(bitmap.Width, bitmap.Height)
    {
        _luminanceMatrix = new byte[bitmap.Width * bitmap.Height];

        // Convert the SKBitmap to a luminance matrix (grayscale)
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                // Get the pixel
                SKColor color = bitmap.GetPixel(x, y);

                // Calculate luminance using RGB
                byte luminance = (byte)((color.Red + color.Green + color.Blue) / 3);
                _luminanceMatrix[y * bitmap.Width + x] = luminance;
            }
        }
    }

    public override byte[] Matrix => _luminanceMatrix;

    public override byte[] getRow(int y, byte[] row)
    {
        if (row == null || row.Length < Width)
        {
            row = new byte[Width];
        }
        Array.Copy(_luminanceMatrix, y * Width, row, 0, Width);
        return row;
    }
    
    
    // Implement the crop method to support cropping
    public override LuminanceSource crop(int left, int top, int width, int height)
    {
        // Create a cropped SKBitmap from the original image
        using (var croppedBitmap = new SKBitmap(width, height))
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Copy pixel from the original bitmap (at the offset of `left`, `top`)
                    int sourceX = left + x;
                    int sourceY = top + y;
                    byte luminance = _luminanceMatrix[sourceY * Width + sourceX];

                    // Set the pixel in the cropped bitmap
                    croppedBitmap.SetPixel(x, y, new SKColor(luminance, luminance, luminance));
                }
            }

            // Return a new SkiaLuminanceSource with the cropped data
            return new SkiaLuminanceSource(croppedBitmap);
        }
    }

    public override bool CropSupported => true; // Indicate that cropping is supported
    
}