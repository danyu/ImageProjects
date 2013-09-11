using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using System.Configuration;

namespace QRPrinting
{
    class QRPrintController
    {
        List<QRPage> pages;
        Font printFont;

        private int printedPage;
        public static string UncontrolledPrinter_Key = "UncontrolledPrinter";
        public static string ControlledPrinter_Key = "ControlledPrinter";

        public QRPrintController()
        {
            printedPage = 0;
        }

        public bool print(List<QRPage> pages, String keyOfPrinterName)
        {
            printedPage = 0;
            this.pages = pages;
            String printerName = ConfigurationManager.AppSettings.Get(keyOfPrinterName);

            try
            {

                //Create print document, which consists of multiple QR pages.
                PrintDocument pd = new PrintDocument();

                //Set printer name
                pd.PrinterSettings.PrinterName = printerName;

                //Create print page event
                pd.PrintPage += new PrintPageEventHandler
                   (this.pd_PrintPage);

                //Start printing
                pd.Print();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {

            ev.Graphics.DrawImage(pages[printedPage].content, 10, 10);
            printedPage++;

            if (printedPage < pages.Count)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        public bool findPrinter(string keyOfPrinterName)
        {
            String printerName = ConfigurationManager.AppSettings.Get(keyOfPrinterName);
            PrinterSettings.StringCollection installedPrinters = PrinterSettings.InstalledPrinters;

            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                if (PrinterSettings.InstalledPrinters[i] == printerName) return true;

            return false;
        }
    }
}