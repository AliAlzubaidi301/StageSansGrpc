namespace StageCode.Other
{
    public sealed class ContentAlignment_Parser
    {
        public static ContentAlignment Get_Alignment(int Value)
        {
            ContentAlignment contentAlignment = 0;
            switch (Value / 10)
            {
                case 0:
                    contentAlignment = ContentAlignment.MiddleCenter;
                    break;
                case 1:
                    contentAlignment = ContentAlignment.TopCenter;
                    break;
                case 2:
                    contentAlignment = ContentAlignment.BottomCenter;
                    break;
            }

            switch (Value % 10)
            {
                case 0:
                    contentAlignment = (ContentAlignment)checked((int)Math.Round((double)contentAlignment / 2.0));
                    break;
                case 1:
                    contentAlignment = (ContentAlignment)checked(unchecked((int)contentAlignment) * 2);
                    break;
            }

            return contentAlignment;
        }

        public static int Get_ValueToWrite(ContentAlignment Align)
        {
            return Align switch
            {
                ContentAlignment.MiddleLeft => 0,
                ContentAlignment.MiddleRight => 1,
                ContentAlignment.MiddleCenter => 2,
                ContentAlignment.TopLeft => 10,
                ContentAlignment.TopRight => 11,
                ContentAlignment.TopCenter => 12,
                ContentAlignment.BottomLeft => 20,
                ContentAlignment.BottomRight => 21,
                ContentAlignment.BottomCenter => 22,
                _ => 0,
            };
        }
    }
}