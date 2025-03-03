namespace StageCode.Other
{
    public class DesignerRectTracker
    {
        private RectangleF _TrackerRectangle;

        private bool _IsActive;

        public RectangleF TrackerRectangle
        {
            get
            {
                return _TrackerRectangle;
            }
            set
            {
                _TrackerRectangle = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
            }
        }

        public DesignerRectTracker()
        {
            _TrackerRectangle = new RectangleF(0f, 0f, 10f, 10f);
            _IsActive = false;
        }
    }

}