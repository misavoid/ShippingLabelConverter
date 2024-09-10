using System.Diagnostics;
using System.Text.RegularExpressions;
using ImageMagick;



namespace ShippingLabelConverter
{
    public static class TextExtractor
    {
        public static string ExtractText(string imagePath)
        {
            string tesseractPath = "/opt/homebrew/bin/tesseract";  // Path to Tesseract installed via Homebrew
            string outputTextPath = Path.GetTempFileName();  // Temporary file to store OCR output (without extension)
            string outputTextFile = outputTextPath + ".txt";  // Tesseract appends .txt automatically
            string finalText = string.Empty;
            
            // Rotate and preprocess the image
            using (var image = new MagickImage(imagePath))
            {
                image.BackgroundColor = MagickColors.White;
                image.Alpha(AlphaOption.Remove);  // Remove transparency
                
                image.Contrast();  // Enhance contrast
                image.BrightnessContrast(new Percentage(20), new Percentage(50));  // Increase brightness and contrast
                
                image.Rotate(90);  // Rotate if needed
                // Crop the image to the right half (adjust values as needed)
                var cropArea = new MagickGeometry
                {
                    Width = image.Width / 2,  // Half the image width
                    Height = image.Height,  // Full image height
                    X = (int)(image.Width / 2),  // Start from the middle of the image
                    Y = 0  // Top of the image
                };
                image.Crop(cropArea);  // Crop to the right side
                image.Write(imagePath);  // Save the processed image
            }
            
            
            // Set up the process to run Tesseract
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = tesseractPath,
                Arguments = $"{imagePath} {outputTextPath} -l deu",  // Append .txt automatically
                RedirectStandardError = true,  // Capture errors
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            

            // Run Tesseract and capture errors
            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.Start();

                string stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Tesseract failed with error: {stderr}");
                }
            }

            // Read the output text from the Tesseract-generated .txt file
            string extractedText = File.ReadAllText(outputTextFile);

            // Clean up the temporary files
            File.Delete(outputTextFile);

            return extractedText;
        }
        
        // New Method to Extract Barcodes from the Text Output
        public static string ExtractBarcodes(string imagePath)
        {
            // Run Tesseract to extract text, reuse the existing method
            string extractedText = ExtractText(imagePath);
            
            // Define a pattern to capture barcodes (usually numeric or alphanumeric patterns)
            string barcodePattern = @"\b\d{10,20}\b";  // Matches sequences of 10 to 20 digits, adjust as needed

            // Use Regex to find potential barcodes in the extracted text
            var matches = Regex.Matches(extractedText, barcodePattern);

            if (matches.Count > 0)
            {
                // Collect and return all barcodes found in the image
                string barcodes = string.Join("\n", matches);
                return barcodes;
            }

            return "No barcodes detected.";
        }
    }
}
