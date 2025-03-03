using System.ComponentModel;

namespace StageCode.Other
{
    public class CornersProperty
    {
        private short _All;

        private short _UpperLeft;

        private short _UpperRight;

        private short _LowerLeft;

        private short _LowerRight;

        [Description("Set the Radius of the All four Corners the same")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(-1)]
        public short All
        {
            get
            {
                return _All;
            }
            set
            {
                _All = value;
                if (value > -1)
                {
                    _LowerLeft = value;
                    _LowerRight = value;
                    _UpperLeft = value;
                    _UpperRight = value;
                }
            }
        }

        [Description("Set the Radius of the Upper Left Corner")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(0)]
        public short UpperLeft
        {
            get
            {
                return _UpperLeft;
            }
            set
            {
                _UpperLeft = value;
                CheckForAll(value);
            }
        }

        [Description("Set the Radius of the Upper Right Corner")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(0)]
        public short UpperRight
        {
            get
            {
                return _UpperRight;
            }
            set
            {
                _UpperRight = value;
                CheckForAll(value);
            }
        }

        [Description("Set the Radius of the Lower Left Corner")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(0)]
        public short LowerLeft
        {
            get
            {
                return _LowerLeft;
            }
            set
            {
                _LowerLeft = value;
                CheckForAll(value);
            }
        }

        [Description("Set the Radius of the Lower Right Corner")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(0)]
        public short LowerRight
        {
            get
            {
                return _LowerRight;
            }
            set
            {
                _LowerRight = value;
                CheckForAll(value);
            }
        }

        public CornersProperty(short LowerLeft, short LowerRight, short UpperLeft, short UpperRight)
        {
            _All = -1;
            _UpperLeft = 0;
            _UpperRight = 0;
            _LowerLeft = 0;
            _LowerRight = 0;
            this.LowerLeft = LowerLeft;
            this.LowerRight = LowerRight;
            this.UpperLeft = UpperLeft;
            this.UpperRight = UpperRight;
        }

        public CornersProperty(short All)
        {
            _All = -1;
            _UpperLeft = 0;
            _UpperRight = 0;
            _LowerLeft = 0;
            _LowerRight = 0;
            this.All = All;
        }

        public CornersProperty()
        {
            _All = -1;
            _UpperLeft = 0;
            _UpperRight = 0;
            _LowerLeft = 0;
            _LowerRight = 0;
            LowerLeft = 0;
            LowerRight = 0;
            UpperLeft = 0;
            UpperRight = 0;
        }

        private void CheckForAll(short val)
        {
            if (val == LowerLeft && val == LowerRight && val == UpperLeft && val == UpperRight)
            {
                if (_All != val)
                {
                    _All = val;
                }
            }
            else if (All != -1)
            {
                All = -1;
            }
        }
    }
}