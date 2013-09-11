using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using XMLLayoutClassTest;
using System.IO;
using System.Configuration;

namespace QRPrinting
{
    class QRPageCreator
    {
        //App settings
        public static String Zebra_Page_Template_BMP = "";
        public static String Desktop_Page_Template_BMP = "";
        public static String QR_Label_template_BMP = "";
        public static String JNJLogo_GIF = "";

        public QRLabel[] qrLabels;
        public QRPageLayoutTemplate pageLayout;
        public QRPrintInputData[] qrPrintData;
        public QRTemplateDocument template;


        public QRPageCreator()
        {
          
        }

        public QRPageCreator(QRPrintInputData[] qrpd, QRTemplateDocument template)
        {
            
            this.qrPrintData = qrpd;
            this.template = template;
        }

        public static void LoadAppSettings(){
            Zebra_Page_Template_BMP = ConfigurationManager.AppSettings.Get("zebra_printer_QRPage_template");
            Desktop_Page_Template_BMP = ConfigurationManager.AppSettings.Get("desktop_printer_QRPage_template");
            QR_Label_template_BMP = ConfigurationManager.AppSettings.Get("QRLabel_template");
            JNJLogo_GIF = ConfigurationManager.AppSettings.Get("logo_image");
        }

        //Construct QRLabels by combine print data and template 
        public QRLabel[] parseData()
        {
            qrLabels = new QRLabel[qrPrintData.Length];
            QRLabel qrLabel = null;
            List<QRInnerLabel> qInnerLs = null;
            String curValue = "";
            MemoryStream memStream = null;
            int tempIndex = 0;
            //Collection information label by label
            for (int i = 0; i < qrPrintData.Length; i++)
            {
                qrLabel = new QRLabel();
                tempIndex = tempIndex >= template.Labels.Count ? tempIndex - template.Labels.Count : tempIndex;
                //Collection template information for each QRLabel 
                Label labelTempl = template.Labels[tempIndex];

                 qInnerLs = new List<QRInnerLabel>();
                foreach (Item item in labelTempl.Items)
                {
                    if (typeof(DynamicText).IsInstanceOfType(item))
                    {
                        if (qrPrintData[i].labels.TryGetValue(((DynamicText)item)._InputKey, out curValue))
                            qInnerLs.Add(new QRInnerLabel(curValue, new Point(item.X, item.Y), item.Width, item.Height,(float)item.Rotation));
                    }
                    else if (typeof(StaticText).IsInstanceOfType(item))
                    {
                        if (qrPrintData[i].labels.TryGetValue(((StaticText)item)._Text, out curValue))
                            qInnerLs.Add(new QRInnerLabel(curValue, new Point(item.X, item.Y), item.Width, item.Height,(float)item.Rotation) );

                    }
                    else if (typeof(XMLLayoutClassTest.Image).IsInstanceOfType(item))
                    {
                        memStream = null;
                        if (qrPrintData[i].imgs.TryGetValue(((XMLLayoutClassTest.Image)item)._InputKey, out memStream))
                            qrLabel.logoImg = new QRImage(new Point(item.X, item.Y), item.Width, item.Height, (Bitmap)System.Drawing.Image.FromStream(memStream));
                    }
                    else if (typeof(XMLLayoutClassTest.QRCode).IsInstanceOfType(item))
                    {
                        memStream = null;
                        if (qrPrintData[i].imgs.TryGetValue(((XMLLayoutClassTest.QRCode)item)._InputKey, out memStream))
                            qrLabel.qrImg = new QRImage(new Point(item.X, item.Y), item.Width, item.Height, (Bitmap)System.Drawing.Image.FromStream(memStream));

                    }
                    else
                    {
                        //Unknown type
                    }

                }
                qrLabel.innerLabels = qInnerLs.ToArray();
                qrLabel.loc = new Point(labelTempl.X,labelTempl.Y);
                qrLabels[i] = qrLabel;
                tempIndex++;
            }
            return qrLabels;
        }

        public List<QRPage> generateLabelPages ()//;QRLabel[] infos,QRPageLayoutTemplate template)
        {
            String pageTemplateBMPPath = Desktop_Page_Template_BMP;
            Bitmap qrPageBMP=null;// = new Bitmap(Zebra_Page_Template_BMP);
            Bitmap qrLabelBMP = null;//=new Bitmap(QR_Label_template_BMP);

            List<QRPage> pages =new List<QRPage>();

            int numOfLabelsInCurrentPage=0;
            int pageIndex = 0;
            //Iterate all labels
            for (int i = 0; i < qrLabels.Length; i++)
            {
                //Create a new bitmap for QR page
                if (numOfLabelsInCurrentPage == 0)   
                    qrPageBMP = new Bitmap(Environment.CurrentDirectory + pageTemplateBMPPath);
                   
                //Create a new bitmap for QR label 
                qrLabelBMP = new Bitmap(Environment.CurrentDirectory+QR_Label_template_BMP);
                
                //Draw content for the current QR label based on current qr infomation
                qrLabelBMP = createQRLabel(qrLabelBMP, qrLabels[i]);

                //!!!Comment following line of codes when release
                //Save to dir to validation the content
                //qrLabelBMP.Save(Environment.CurrentDirectory + "/../../qr_pages/" + pageIndex + "-" + numOfLabelsInCurrentPage + ".bmp");
                
                //Add current QR label to current QR Page
                qrPageBMP = appendQRLabelToPage(qrLabelBMP, qrPageBMP, qrLabels[i].loc);
                numOfLabelsInCurrentPage++;

                //Append current QR page, which is full of QR labels, into QRPage collection
                if (numOfLabelsInCurrentPage >= template.Labels.Count)
                { 
                    //Attache page number
                    appendPageNumber(qrPageBMP, pageIndex);
                    pages.Add(new QRPage(qrPageBMP));

                    //Reset for next page
                    numOfLabelsInCurrentPage = 0;

                    //Update page number
                    pageIndex++;
                }           

            }

           //Add QRPage, which is not full, to collection 
            if (numOfLabelsInCurrentPage > 0 && numOfLabelsInCurrentPage < template.Labels.Count)
            {
                appendPageNumber(qrPageBMP, pageIndex);
                pages.Add(new QRPage(qrPageBMP));
            }

            return pages;
        }

        //Create a single QRLabel
        public  Bitmap createQRLabel(Bitmap qrBitmap, QRLabel qrInfo)
        {

            Graphics graphics = Graphics.FromImage(qrBitmap);

            //Draw QR code image
            graphics.DrawImageUnscaled((System.Drawing.Image)qrInfo.qrImg.bmp, qrInfo.qrImg.loc.X, qrInfo.qrImg.loc.Y);

            //Draw logo image
            graphics.DrawImage((System.Drawing.Image)qrInfo.logoImg.bmp, qrInfo.logoImg.loc.X, qrInfo.logoImg.loc.Y, qrInfo.logoImg.width, qrInfo.logoImg.height);

            //Draw inner labels in loop
            for (int i = 0; i < qrInfo.innerLabels.Length; i++)
            {
                
                Font font = qrInfo.innerLabels[i].font; //new Font("Times New Roman",  fontSize, FontStyle.Regular);
                SolidBrush brush = new SolidBrush(Color.Black);
                String text = qrInfo.innerLabels[i].txt;

                //Set size and location of string 
                SizeF reqdSize = graphics.MeasureString(text, font, qrInfo.innerLabels[i].width);
                reqdSize.Height = (float)Math.Round(reqdSize.Height);

                //Calculate the rectangle holding the string
                RectangleF strRect = new RectangleF(
                    qrInfo.innerLabels[i].loc.X,
                    qrInfo.innerLabels[i].loc.Y, 
                    reqdSize.Width + 2, 
                    reqdSize.Height);
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;

                //graphics.TranslateTransform(-20, -20);
                graphics.RotateTransform(qrInfo.innerLabels[i].rotation);

                //Draw rectangle with withe background that holds string
                graphics.FillRectangle(new SolidBrush(Color.White), strRect);
                
                //Draw String
                graphics.DrawString(text, font, brush, strRect, format);                

                graphics.RotateTransform(qrInfo.innerLabels[i].rotation*(-1));
                //graphics.TranslateTransform(20, 20);
            }

            return qrBitmap;
        }

        //Append a QRLabel to a QRPage
        public Bitmap appendQRLabelToPage(Bitmap qrLabel,Bitmap  qrPage, Point loc)
        {
            Graphics graphics = Graphics.FromImage(qrPage);
            graphics.DrawImageUnscaled((System.Drawing.Image)qrLabel, loc.X, loc.Y);
            graphics.DrawRectangle(new Pen(Color.Black), new Rectangle(loc.X , loc.Y , qrLabel.Width , qrLabel.Height));
            
            return qrPage;
        }

        public Bitmap appendPageNumber(Bitmap qrPage, int pageNum)
        {
            Graphics graphics = Graphics.FromImage(qrPage);
            graphics.DrawString(String.Format(QRPage.PageNumberFormat, pageNum), 
                                QRPage.PageNumberFont, 
                                new SolidBrush(Color.Black), 
                                QRPage.PageNumberLoc);

            return qrPage;
        }
    }

    
    
    //The page that is sent to printer.
    class QRPage
    {

        //Page Number
        //public int pageNumber;

        //Page Number location
        public static  Point PageNumberLoc=new Point(300, 950);

        //Page Number font
        public static Font PageNumberFont=new Font("Times New Roman", 14, FontStyle.Bold);

        //Page Number format
        public static String PageNumberFormat="Page - {0}";

        //What the page displays
        public Bitmap content;

        public QRPage(Bitmap pageContent)
        {
            content = pageContent;
        }
    }

    //Super class to all printable items
    public abstract class PrintItem
    {
        //Top-left point of QR code image
        public Point loc;

        //Dimension
        public int width = 0;
        public int height = 0;

        //Rotation
        public float rotation=0;

        //Transparency
        public string alpha="";

    }

    //QRLabel comprises:
    //QR Code Image, inner labels (dynamic and static) amd Logo Image.  
    class QRLabel : PrintItem
    {
        //Top-Left point of QR Label
        public Point loc;

        //QR image in format of Bitmap
        public QRImage qrImg;

        //Labels that explains the QR image
        public QRInnerLabel[] innerLabels;

        //Logo image
        public QRImage logoImg;

        public QRLabel() { }
        public QRLabel(Point loc, QRImage qrImg, QRInnerLabel[] innerLabels, QRImage logoImg)
        {
            this.loc = loc;
            this.qrImg = qrImg;
            this.innerLabels = innerLabels;
            this.logoImg = logoImg;
        }
    }

    //QR Image contains information for drawing QR code on a QR Label
    class QRImage : PrintItem
    {

        //QR code image
        public Bitmap bmp;

        public QRImage(Point loc, Bitmap bmp) { this.loc = loc; this.bmp = bmp; }

        public QRImage(Point loc, int width, int height, Bitmap bmp)
        {
            this.loc = loc;
            this.bmp = bmp;
            this.width = width;
            this.height = height;
        }
    }

    //The label that is located in a QR label
    public class QRInnerLabel:PrintItem

    {
        //text in a innter label
        public String txt;
        //text color
        public Color clr=Color.Black;
        //text font
        public Font font=new Font("Times New Roman", 10, FontStyle.Regular);

        public QRInnerLabel() { }

        public QRInnerLabel(String txt, Point loc, int width, int height, float rotation )
        {
            this.txt = txt;
            this.loc = loc;
            this.rotation = rotation;
        }

        public QRInnerLabel(String txt, Color clr, Font font, Point loc, int width, int height)
        {
            this.txt = txt;
            this.clr = clr;
            this.font = font;
            this.loc = loc;
        }
    }

    //Information for constructing a page that contains QR labels
    public class QRPageLayoutTemplate
    {
        //how many labels in a page, used for paging
        public int capacity;

        public int templateType;

        //page dimension
        public int pageWidth;
        public int pageHeight;     
        
    }


}
