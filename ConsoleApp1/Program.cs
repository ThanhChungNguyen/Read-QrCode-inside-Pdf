using System;
using Bytescout.BarCodeReader;
using Com.AugustCellars.COSE;
using Ionic.Zlib;
using PeterO.Cbor;
using DgcReader;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Reader reader = new Reader("demo", "demo"))
            {
                reader.BarcodeTypesToFind.QRCode = true;
                FoundBarcode[] barcodes = reader.ReadFrom(@"C:\image.pdf");

                foreach (FoundBarcode code in barcodes)
                {
                    Console.WriteLine("Found barcode with type '{0}' and value '{1}'", code.Type, code.Value);
                    Console.WriteLine(code.ToString());
                    var message = Program.Decode(code.Value);
                    Console.WriteLine(message);
                }
            }  
            
            Console.ReadLine();
        }

        public static string Decode(string hc1Text)
        {
            var base45 = new Base45();
            var qrmessage = hc1Text.Substring(4);//remove first 4 chars
            byte[] decodedBase45 = base45.Decode(qrmessage);//using base45 lib
            var cose = ZlibStream.UncompressBuffer(decodedBase45);//using zlib or similar

            var decrypted = Message.DecodeFromBytes(cose).GetContent(); //using COSE
            CBORObject cbor = CBORObject.DecodeFromBytes(decrypted);    //using Peter.O.. CBOR

            var jsonDecoded = cbor.ToJSONString(); //or deserialize it to custom class

            return jsonDecoded;
        }
    }
}
