// See https://aka.ms/new-console-template for more information

// TODO: figure out how to parse original label & extract necessary info such as barcodes, sender and recipient adress. 
// TODO: design new label with appropriate dimensions and layout. Libraries: iTextSharp, BarcodeLib, MigraDoc, PdfSharp -> PNG or pdf output?
// TODO: generate new label with data from step 1 and layout from step 2. create new file with imagemagick
using ShippingLabelConverter;



string test = "/Users/pingopengo/Datasets/dhl_label.pdf";

PdfReader pdfReader = new PdfReader();
pdfReader.handlePdf(test, "/Users/pingopengo/Datasets/dhl_label.png");