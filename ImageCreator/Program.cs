using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using XMLLayoutClassTest;
using System.Drawing.Printing;

namespace QRPrinting
{
    class Tester
    {
        //Mock up template data
        public QRTemplateDocument buildMockUpTemplate()
        {
            
            QRTemplateDocument template = new QRTemplateDocument();
            Label qrLabel_1 = new Label();
            Label qrLabel_2 = new Label();
            Label qrLabel_3 = new Label();

            qrLabel_1.Width = 288;
            qrLabel_1.Height = 192;
            qrLabel_1.X = 20;
            qrLabel_1.Y = 20;

            qrLabel_1.Items.Add(new StaticText(5, -20, 1, 15, 124,90, "QR Title"));
            qrLabel_1.Items.Add(new DynamicText(200, 50, 2, 15, 124, 0, "QRNumber"));
            qrLabel_1.Items.Add(new XMLLayoutClassTest.Image(160, 100, 2, 46, 100, 0.0, "JnJLogo.png", "ffffff"));
            qrLabel_1.Items.Add(new QRCode(20, 10, 4, 15, 124, 0.0, "QRImage", "ffffff"));

            qrLabel_2.Width = 288;
            qrLabel_2.Height = 192;
            qrLabel_2.X = 20;
            qrLabel_2.Y = 240;

            qrLabel_2.Items.Add(new StaticText(160, 10, 1, 15, 124, 0.0, "QR Title"));
            qrLabel_2.Items.Add(new DynamicText(200, 50, 2, 15, 124, 0, "QRNumber"));
            qrLabel_2.Items.Add(new XMLLayoutClassTest.Image(160, 100, 3, 46, 100, 0.0, "JnJLogo.png", "ffffff"));
            qrLabel_2.Items.Add(new QRCode(20, 10, 4, 15, 124, 0.0, "QRImage", "ffffff"));

            qrLabel_3.Width = 288;
            qrLabel_3.Height = 192;
            qrLabel_3.X = 20;
            qrLabel_3.Y =460;

            qrLabel_3.Items.Add(new StaticText(180, 10, 1, 15, 124, 0.0, "QR Title"));
            qrLabel_3.Items.Add(new DynamicText(200, 70, 2, 15, 124, 0, "QRNumber"));
            qrLabel_3.Items.Add(new XMLLayoutClassTest.Image(160, 100, 3, 46, 100, 0.0, "JnJLogo.png", "ffffff"));
            qrLabel_3.Items.Add(new QRCode(20, 10, 4, 15, 124, 0.0, "QRImage", "ffffff"));
           
            template.Labels.Add(qrLabel_1);
            template.Labels.Add(qrLabel_2);
            template.Labels.Add(qrLabel_3);
            return template;
        }

        //Mock up print data  
        public QRPrintInputData[] buildMockUpQRLabelData()
        {
            List<QRPrintInputData> data = new List<QRPrintInputData>();

                      
            Dictionary<String, String> labels = new Dictionary<String, String>();
            
            //Add first label
            labels.Add("QR Title", "AREA-001");
            labels.Add("QRNumber", "POI-001");
            Dictionary<String, MemoryStream> imgs = new Dictionary<String, MemoryStream>();
            byte[] img = new byte[0];
            MemoryStream memStream = new MemoryStream();
            System.Drawing.Image.FromFile(Environment.CurrentDirectory + @"\..\..\imgs\26.gif").Save(memStream, System.Drawing.Imaging.ImageFormat.Gif);
            imgs.Add("QRImage", memStream);

            memStream = new MemoryStream();
            System.Drawing.Image.FromFile(Environment.CurrentDirectory + @"\..\..\imgs\Vistakon_logo.gif").Save(memStream, System.Drawing.Imaging.ImageFormat.Gif);
            imgs.Add("JnJLogo.png", memStream);

            //Add some labels
            data.Add(new QRPrintInputData(imgs, labels));
            data.Add(new QRPrintInputData(imgs, labels));
            data.Add(new QRPrintInputData(imgs, labels));
            data.Add(new QRPrintInputData(imgs, labels));
            data.Add(new QRPrintInputData(imgs, labels));

            return data.ToArray();
        }

        //Output pages for validation
        public void outputPages(List<QRPage> pages){
          //Save for validation
            for (int i = 0; i < pages.Count; i++)
            {
                pages[i].content.Save(Environment.CurrentDirectory + "/../../qr_pages/" + "page-" + i + ".bmp");
            }
        }

        public void run()
        {

            int total = 5;
            int txtPerQRLabel = 3;
            int labelPerPage = 4;
            int qrLocIndex = 0;
            int inner_label_left = 170;
            int inner_label_height = 20;
            int inner_label_width = 100;
            int inner_label_vertical_top = 30;

            //----------------------Preparing testing data
            QRPage.PageNumberLoc = new Point(300, 950);
            QRPage.PageNumberFont = new Font("Times New Roman", 14, FontStyle.Bold);

            //Create an array of QR Label information
            QRLabel[] infos = new QRLabel[total];
            for (int i = 0; i < total; i++)
            {

                infos[i] = new QRLabel();

                //Append mockup QR Image
                infos[i].qrImg = new QRImage(new Point(10, 10),
                                (Bitmap)System.Drawing.Image.FromFile(Environment.CurrentDirectory + @"\..\..\imgs\26.gif"));

                //Append logo image
                infos[i].logoImg = new QRImage(new Point(160, 120), 100, 46,
                                (Bitmap)System.Drawing.Image.FromFile(Environment.CurrentDirectory + QRPageCreator.JNJLogo_GIF));

                //Create Labels
                QRInnerLabel[] labels = new QRInnerLabel[txtPerQRLabel];

                labels[0] = new QRInnerLabel(
                        "LOC: XXX",
                        Color.Black, new Font("Times New Roman", 10, FontStyle.Bold),
                        new Point(inner_label_left, 0 * inner_label_height + inner_label_vertical_top), inner_label_width, inner_label_height);
                labels[1] = new QRInnerLabel(
                        System.DateTime.Now.ToLocalTime().ToShortTimeString(),
                        Color.Black, new Font("Times New Roman", 8, FontStyle.Regular),
                        new Point(inner_label_left, 1 * inner_label_height + inner_label_vertical_top), inner_label_width, inner_label_height);
                labels[2] = new QRInnerLabel(
                        "QR CODE: XXX",
                        Color.Black, new Font("Times New Roman", 8, FontStyle.Regular),
                        new Point(inner_label_left, 2 * inner_label_height + inner_label_vertical_top), inner_label_width, inner_label_height);

                infos[i].innerLabels = labels;

                //Define top left of QR Label
                infos[i].loc = new Point(10, qrLocIndex * (192 + 20) + 50);
                if (qrLocIndex >= labelPerPage - 1) qrLocIndex = qrLocIndex - labelPerPage;
                qrLocIndex++;
            }

            //Create a mock up page layout template
            QRPageLayoutTemplate pageLayoutTempl = new QRPageLayoutTemplate();
            pageLayoutTempl.capacity = labelPerPage;

            //----------------------Test Function
            QRPageCreator qrPageGen = new QRPageCreator();
            qrPageGen.qrLabels = infos;
            List<QRPage> pages = qrPageGen.generateLabelPages();
            outputPages(pages);
        }

        public void testTemplate(){

            QRPageCreator.LoadAppSettings();

            //Create QR Page creater
            QRPageCreator qrPageGen = new QRPageCreator(buildMockUpQRLabelData(), buildMockUpTemplate());
            qrPageGen.parseData();
            
            List<QRPage> pages = qrPageGen.generateLabelPages();
            outputPages(pages);

            QRPrintController qrPC = new QRPrintController();
            if (qrPC.findPrinter(QRPrintController.UncontrolledPrinter_Key))
            {

                if (qrPC.print(pages, QRPrintController.UncontrolledPrinter_Key))
                    Console.Out.WriteLine("Printing succeed.");
                else
                    Console.Out.WriteLine("Printing failed.");
            }

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            new Tester().testTemplate();
        }

       

    }
}
