// See https://aka.ms/new-console-template for more information

// TODO: figure out how to parse original label & extract necessary info
// TODO: barcodes
// TODO: sender and recipient address
// TODO: design new label with appropriate dimensions and layout. Libraries: iTextSharp, BarcodeLib, MigraDoc, PdfSharp -> PNG or pdf output?
// TODO: generate new label with data from step 1 and layout from step 2. create new file with imagemagick


using ShippingLabelConverter;



string test = "/Users/misanthrop/Documents/Code/data/DHL-Paketmarke.pdf";


PdfReader pdfReader = new PdfReader();
pdfReader.ConvertPdfToImage(test, "/Users/misanthrop/Documents/Code/data/DHL-Paketmarke_neu.png");

string img = "/Users/misanthrop/Documents/Code/data/DHL-Paketmarke_neu.png";
string extractedText = TextExtractor.ExtractText(img);

Console.WriteLine("extracted text:");
Console.WriteLine(extractedText);

var barcodeExtractor = new BarcodeExtractor();
barcodeExtractor.ExtractBarcodes(img);

Console.WriteLine("barcode extraction complete");



