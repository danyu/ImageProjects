using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QRPrinting
{
    //This class represents input data for printing.
    //It will be parsed by QRPageCreator to generate QR Pages.
    class QRPrintInputData
    {

      public  Dictionary<String, MemoryStream> imgs;
      public  Dictionary<String, String> labels;

      public QRPrintInputData() { }
      public QRPrintInputData(Dictionary<String, MemoryStream> imgs, Dictionary<String, String> labels) {
          this.imgs = imgs;
          this.labels = labels;
      }

    }
}
