using OrthoDesigner.LIB;
using StageCode.LIB;
using StageCode;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace OrthoDesigner.Other
{
    [Serializable]
    public class SerializableControl
    {
        public string TypeName { get; set; } = "";
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Text { get; set; }
    }
}