using System.ComponentModel;

namespace StageCode.Other
{
    public class cFocalPoints
    {
        private float _CenterPtX;

        private float _CenterPtY;

        private float _FocusPtX;

        private float _FocusPtY;

        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(0.5)]
        public float CenterPtX
        {
            get
            {
                return _CenterPtX;
            }
            set
            {
                if (value < 0f)
                {
                    value = 0f;
                }

                if (value > 1f)
                {
                    value = 1f;
                }

                _CenterPtX = value;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(0.5)]
        public float CenterPtY
        {
            get
            {
                return _CenterPtY;
            }
            set
            {
                if (value < 0f)
                {
                    value = 0f;
                }

                if (value > 1f)
                {
                    value = 1f;
                }

                _CenterPtY = value;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(0)]
        public float FocusPtX
        {
            get
            {
                return _FocusPtX;
            }
            set
            {
                if (value < 0f)
                {
                    value = 0f;
                }

                if (value > 1f)
                {
                    value = 1f;
                }

                _FocusPtX = value;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(0)]
        public float FocusPtY
        {
            get
            {
                return _FocusPtY;
            }
            set
            {
                if (value < 0f)
                {
                    value = 0f;
                }

                if (value > 1f)
                {
                    value = 1f;
                }

                _FocusPtY = value;
            }
        }

        public PointF CenterPoint()
        {
            return new PointF(CenterPtX, CenterPtY);
        }

        public PointF FocusScales()
        {
            return new PointF(FocusPtX, FocusPtY);
        }

        public cFocalPoints()
        {
            _CenterPtX = 0.5f;
            _CenterPtY = 0.5f;
            _FocusPtX = 0f;
            _FocusPtY = 0f;
            CenterPtX = 0.5f;
            CenterPtY = 0.5f;
            FocusPtX = 0f;
            FocusPtY = 0f;
        }

        public cFocalPoints(float Cx, float Cy, float Fx, float Fy)
        {
            _CenterPtX = 0.5f;
            _CenterPtY = 0.5f;
            _FocusPtX = 0f;
            _FocusPtY = 0f;
            CenterPtX = Cx;
            CenterPtY = Cy;
            FocusPtX = Fx;
            FocusPtY = Fy;
        }

        public cFocalPoints(PointF ptC, PointF ptF)
        {
            _CenterPtX = 0.5f;
            _CenterPtY = 0.5f;
            _FocusPtX = 0f;
            _FocusPtY = 0f;
            CenterPtX = ptC.X;
            CenterPtY = ptC.Y;
            FocusPtX = ptF.X;
            FocusPtY = ptF.Y;
        }

        public override string ToString()
        {
            return $"{_CenterPtX}, {_CenterPtY}, {_FocusPtX}, {_FocusPtY}";
        }
    }
}