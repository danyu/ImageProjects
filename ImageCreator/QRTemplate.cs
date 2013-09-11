using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;

namespace XMLLayoutClassTest
{
    [Serializable]
    [XmlRoot(ElementName = "document")]
    public class QRTemplateDocument
    {
        public List<Label> Labels;

        public QRTemplateDocument()
        {
            Labels = new List<Label>();
        }
    }


    [XmlInclude(typeof(Item))]
    [XmlInclude(typeof(StaticText))]
    [XmlInclude(typeof(DynamicText))]
    [XmlInclude(typeof(Image))]
    [XmlInclude(typeof(QRCode))]
    public class Label : Item
    {

        public List<Item> Items;

        public Label()
        {
            Items = new List<Item>();
        }

    }

    public class StaticText : Item
    {
        [XmlAttribute("inputKey")]
        public string _Text;

        public StaticText() { }

        public StaticText(int X, int Y, int Z, int Height, int Width, double Rotation, string Text)
        {
            _X = X;
            _Y = Y;
            _Z = Z;
            _Height = Height;
            _Width = Width;
            _Rotation = Rotation;
            _Text = Text;
        }
    }

    public class DynamicText : Item
    {
        [XmlAttribute("inputKey")]
        public string _InputKey;

        public DynamicText() { }

        public DynamicText(int X, int Y, int Z, int Height, int Width, double Rotation, string InputKey)
        {
            _X = X;
            _Y = Y;
            _Z = Z;
            _Height = Height;
            _Width = Width;
            _Rotation = Rotation;
            _InputKey = InputKey;
        }
    }

    public class Image : Item
    {
        [XmlAttribute("inputKey")]
        public string _InputKey;

        [XmlAttribute("Alpha")]
        public string _Alpha;

        public Image() { }

        public Image(int X, int Y, int Z, int Height, int Width, double Rotation, string InputKey, string Alpha)
        {
            _X = X;
            _Y = Y;
            _Z = Z;
            _Height = Height;
            _Width = Width;
            _Rotation = Rotation;
            _InputKey = InputKey;
            _Alpha = Alpha;
        }
    }

    public class QRCode : Item
    {
        [XmlAttribute("inputKey")]
        public string _InputKey;

        [XmlAttribute("Alpha")]
        public string _Alpha;

        public QRCode() { }

        public QRCode(int X, int Y, int Z, int Height, int Width, double Rotation, string InputKey, string Alpha)
        {
            _X = X;
            _Y = Y;
            _Z = Z;
            _Height = Height;
            _Width = Width;
            _Rotation = Rotation;
            _InputKey = InputKey;
            _Alpha = Alpha;
        }
    }

    public class Item
    {
        protected int _X;
        protected int _Y;
        protected int _Z;
        protected int _Height;
        protected int _Width;
        protected double _Rotation;

        [XmlAttribute("X")]
        public int X
        {
            get { return _X; }
            set { _X = value; }
        }

        [XmlAttribute("Y")]
        public int Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        [XmlAttribute("Z")]
        public int Z
        {
            get { return _X; }
            set { _X = value; }
        }

        [XmlAttribute("Height")]
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        [XmlAttribute("Width")]
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        [XmlAttribute("Rotation")]
        public double Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }
    }
}
