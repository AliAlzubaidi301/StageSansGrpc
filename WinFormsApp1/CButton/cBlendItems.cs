using System.ComponentModel;

namespace StageCode.Other
{
    public class cBlendItems
    {
        private Color[] _iColor;

        private float[] _iPoint;

        [Description("The Color for the Point")]
        [Category("Appearance")]
        public Color[] iColor
        {
            get
            {
                return _iColor;
            }
            set
            {
                _iColor = value;
            }
        }

        [Description("The Color for the Point")]
        [Category("Appearance")]
        public float[] iPoint
        {
            get
            {
                return _iPoint;
            }
            set
            {
                _iPoint = value;
            }
        }

        public cBlendItems()
        {
        }

        public cBlendItems(Color[] Color, float[] Pt)
        {
            iColor = Color;
            iPoint = Pt;
        }

        public override string ToString()
        {
            return "BlendItems";
        }
    }

}