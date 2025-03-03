    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Windows.Forms;
    using Microsoft.VisualBasic.CompilerServices;

namespace StageCode.Other
{
    #region assembly CButtonLib, Version=1.1.9092.19744, Culture=neutral, PublicKeyToken=null
    #endregion



    [DesignerGenerated]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(CButton), "CButtonLib.CButton.bmp")]
    [DefaultEvent("ClickButtonArea")]
    public class CButton : Control
    {
        public enum eMouseDrawState
        {
            Over,
            Down,
            Up
        }

        public enum eShape
        {
            Ellipse,
            Rectangle,
            TriangleUp,
            TriangleDown,
            TriangleLeft,
            TriangleRight
        }

        public enum eFillType
        {
            Solid,
            GradientLinear,
            GradientPath
        }

        public delegate void ClickButtonAreaEventHandler(object Sender, EventArgs e);

        private IContainer components;

        private eMouseDrawState MouseDrawState;

        private int PressedOffset;

        private PointF Imagept;

        private RectangleF ButtonArea;

        private RectangleF TextArea;

        private RectangleF ImageArea;

        private Size ImageSizeUse;

        private Color[] _HoverColorBlend;

        private Color[] _ClickColorBlend;

        private Color[] _DisabledBlend;

        private Color _HoverColorSolid;

        private Color _ClickColorSolid;

        private Color _DisabledSolid;

        private CornersProperty _Corners;

        private eShape _Shape;

        private int _DimFactorHover;

        private int _DimFactorClick;

        private Color _BorderColor;

        private bool _BorderShow;

        private eFillType _FillType;

        private LinearGradientMode _FillTypeLinear;

        private Color _ColorFillSolid;

        private cBlendItems _ColorFillBlend;

        private string _Text;

        private ContentAlignment _TextAlign;

        private Padding _TextMargin;

        private TextImageRelation _TextImageRelation;

        private bool _TextShadowShow;

        private Color _TextShadow;

        private Image _Image;

        private ContentAlignment _ImageAlign;

        private Size _ImageSize;

        private ImageList _Imagelist;

        private int _ImageIndex;

        private Image _SideImage;

        private Size _SideImageSize;

        private bool _SideImageBehindText;

        private ContentAlignment _SideImageAlign;

        private cFocalPoints _FocalPoints;

        private DesignerRectTracker _CenterPtTracker;

        private DesignerRectTracker _FocusPtTracker;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Appearance CButton")]
        [Description("Get or Set the Corner Radii")]
        public CornersProperty Corners
        {
            get
            {
                return _Corners;
            }
            set
            {
                _Corners = value;
                Invalidate();
            }
        }

        [Description("The Shape of the Button")]
        [Category("Appearance CButton")]
        public eShape Shape
        {
            get
            {
                return _Shape;
            }
            set
            {
                _Shape = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set how much to dim the color on mouse rollover. Positive to Lighten and negative to Darken")]
        [DefaultValue(50)]
        public int DimFactorHover
        {
            get
            {
                return _DimFactorHover;
            }
            set
            {
                _DimFactorHover = value;
                UpdateDimBlends();
                UpdateDimColors();
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set how much to dim the color on mouse down. Positive to Lighten and negative to Darken")]
        [DefaultValue(-25)]
        public int DimFactorClick
        {
            get
            {
                return _DimFactorClick;
            }
            set
            {
                _DimFactorClick = value;
                UpdateDimBlends();
                UpdateDimColors();
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the Border color")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get
            {
                return _BorderColor;
            }
            set
            {
                _BorderColor = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set to show the Border")]
        [DefaultValue(true)]
        public bool BorderShow
        {
            get
            {
                return _BorderShow;
            }
            set
            {
                _BorderShow = value;
                Invalidate();
            }
        }

        [Description("The Fill Type to apply to the CButton")]
        [Category("Appearance CButton")]
        public eFillType FillType
        {
            get
            {
                return _FillType;
            }
            set
            {
                _FillType = value;
                Invalidate();
            }
        }

        [Description("The Linear Blend type")]
        [Category("Appearance CButton")]
        public LinearGradientMode FillTypeLinear
        {
            get
            {
                return _FillTypeLinear;
            }
            set
            {
                _FillTypeLinear = value;
                Invalidate();
            }
        }

        [Description("The Solid Color to fill the CButton")]
        [Category("Appearance CButton")]
        public Color ColorFillSolid
        {
            get
            {
                return _ColorFillSolid;
            }
            set
            {
                _ColorFillSolid = value;
                UpdateDimColors();
                Invalidate();
            }
        }

        public new Color BackColor
        {
            get
            {
                return ColorFillSolid;
            }
            set
            {
                ColorFillSolid = value;
            }
        }

        [Description("The ColorBlend to fill the CButton")]
        [Category("Appearance CButton")]
        public cBlendItems ColorFillBlend
        {
            get
            {
                return _ColorFillBlend;
            }
            set
            {
                _ColorFillBlend = value;
                UpdateDimBlends();
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the Button Text")]
        [DefaultValue("CButton")]
        public override string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the alignment for the text")]
        [DefaultValue(32)]
        public ContentAlignment TextAlign
        {
            get
            {
                return _TextAlign;
            }
            set
            {
                _TextAlign = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the margion between the text and the button edge")]
        [DefaultValue("2,2,2,2")]
        public Padding TextMargin
        {
            get
            {
                return _TextMargin;
            }
            set
            {
                _TextMargin = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the Relationship of the Text to the Image")]
        [DefaultValue("(None)")]
        public TextImageRelation TextImageRelation
        {
            get
            {
                return _TextImageRelation;
            }
            set
            {
                _TextImageRelation = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set if the Text has a shadow")]
        [DefaultValue(false)]
        public bool TextShadowShow
        {
            get
            {
                return _TextShadowShow;
            }
            set
            {
                _TextShadowShow = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the color of the Shadow Text")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color TextShadow
        {
            get
            {
                return _TextShadow;
            }
            set
            {
                _TextShadow = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the small Image next to text")]
        [DefaultValue("(None)")]
        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the placement of the Image")]
        [DefaultValue("(None)")]
        public ContentAlignment ImageAlign
        {
            get
            {
                return _ImageAlign;
            }
            set
            {
                _ImageAlign = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the Size of the Image")]
        [DefaultValue("16, 16")]
        public Size ImageSize
        {
            get
            {
                return _ImageSize;
            }
            set
            {
                _ImageSize = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the ImageList control")]
        [DefaultValue("(None)")]
        public ImageList Imagelist
        {
            get
            {
                return _Imagelist;
            }
            set
            {
                _Imagelist = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the ImageList control")]
        public int ImageIndex
        {
            get
            {
                return _ImageIndex;
            }
            set
            {
                if (Imagelist.Images.Count > 0 && ((value >= 0) & (value < Imagelist.Images.Count)))
                {
                    _ImageIndex = value;
                    Image = Imagelist.Images[value];
                    Invalidate();
                }
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the Side Image")]
        [DefaultValue("(None)")]
        public Image SideImage
        {
            get
            {
                return _SideImage;
            }
            set
            {
                _SideImage = value;
                if (value != null)
                {
                    _SideImageSize = value.Size;
                }

                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the Size of the Side Image")]
        [DefaultValue("48, 48")]
        public Size SideImageSize
        {
            get
            {
                return _SideImageSize;
            }
            set
            {
                _SideImageSize = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set if the Side Image is in front or behind the Text")]
        [DefaultValue(true)]
        public bool SideImageBehindText
        {
            get
            {
                return _SideImageBehindText;
            }
            set
            {
                _SideImageBehindText = value;
                Invalidate();
            }
        }

        [Category("Appearance CButton")]
        [Description("Get or Set the Side Image Alignment")]
        [DefaultValue(16)]
        public ContentAlignment SideImageAlign
        {
            get
            {
                return _SideImageAlign;
            }
            set
            {
                _SideImageAlign = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The CenterPoint and FocusScales for the ColorBlend")]
        [Category("Appearance CButton")]
        public cFocalPoints FocalPoints
        {
            get
            {
                return _FocalPoints;
            }
            set
            {
                _FocalPoints = value;
                CenterPtTracker.TrackerRectangle = CenterPtTrackerRectangle();
                FocusPtTracker.TrackerRectangle = FocusPtTrackerRectangle();
                Invalidate();
            }
        }

        [Browsable(false)]
        [Category("Shape")]
        public DesignerRectTracker CenterPtTracker
        {
            get
            {
                return _CenterPtTracker;
            }
            set
            {
                _CenterPtTracker = value;
            }
        }

        [Browsable(false)]
        [Category("Shape")]
        public DesignerRectTracker FocusPtTracker
        {
            get
            {
                return _FocusPtTracker;
            }
            set
            {
                _FocusPtTracker = value;
            }
        }

        public event ClickButtonAreaEventHandler ClickButtonArea;

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.Size = new System.Drawing.Size(75, 25);
            base.ResumeLayout(false);
        }

        public CButton()
        {
            base.MouseDown += CButton_MouseDown;
            base.MouseMove += CButton_MouseMove;
            base.MouseLeave += CButton_MouseLeave;
            base.MouseUp += CButton_MouseUp;
            base.Resize += CButton_Resize;
            MouseDrawState = eMouseDrawState.Up;
            PressedOffset = 0;
            _Corners = new CornersProperty(6);
            _Shape = eShape.Rectangle;
            _DimFactorHover = 50;
            _DimFactorClick = -25;
            _BorderColor = Color.Black;
            _BorderShow = true;
            _FillType = eFillType.GradientLinear;
            _FillTypeLinear = LinearGradientMode.Vertical;
            _ColorFillSolid = SystemColors.Control;
            _ColorFillBlend = new cBlendItems(new Color[3]
            {
            Color.AliceBlue,
            Color.RoyalBlue,
            Color.Navy
            }, new float[3] { 0f, 0.5f, 1f });
            _Text = "CButton";
            _TextAlign = ContentAlignment.MiddleCenter;
            _TextImageRelation = TextImageRelation.Overlay;
            _TextShadowShow = false;
            _TextShadow = Color.DimGray;
            _Image = null;
            _ImageAlign = ContentAlignment.MiddleCenter;
            _ImageSize = new Size(16, 16);
            _Imagelist = new ImageList();
            _SideImage = null;
            _SideImageSize = new Size(48, 48);
            _SideImageBehindText = true;
            _SideImageAlign = ContentAlignment.MiddleLeft;
            _FocalPoints = new cFocalPoints(0.5f, 0.5f, 0f, 0f);
            _CenterPtTracker = new DesignerRectTracker();
            _FocusPtTracker = new DesignerRectTracker();
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
            base.Size = new Size(75, 25);
            FillType = eFillType.Solid;
            base.BackColor = Color.Transparent;
        }

        private RectangleF CenterPtTrackerRectangle()
        {
            RectangleF result = new RectangleF(FocalPoints.CenterPoint().X * (float)base.Width - 5f, FocalPoints.CenterPoint().Y * (float)base.Height - 5f, 10f, 10f);
            return result;
        }

        private RectangleF FocusPtTrackerRectangle()
        {
            RectangleF result = new RectangleF(FocalPoints.FocusScales().X * (float)base.Width - 5f, FocalPoints.FocusScales().Y * (float)base.Height - 5f, 10f, 10f);
            return result;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color color;
            Color color2;
            Color color3;
            if (base.Enabled)
            {
                color = _BorderColor;
                color2 = ForeColor;
                color3 = _TextShadow;
            }
            else
            {
                color = GrayTheColor(_BorderColor);
                color2 = GrayTheColor(ForeColor);
                color3 = GrayTheColor(_TextShadow);
            }

            Pen pen = new Pen(color);
            pen.Alignment = PenAlignment.Inset;
            ButtonArea = AdjustRect(checked(new RectangleF(0f, 0f, base.Size.Width - 1, base.Size.Height - 1)), base.Padding);
            GraphicsPath path = GetPath();
            if (BackgroundImage == null)
            {
                switch (FillType)
                {
                    case eFillType.Solid:
                        {
                            using (Brush brush = new SolidBrush(GetFill()))
                            {
                                e.Graphics.FillPath(brush, path);
                            }

                            break;
                        }
                    case eFillType.GradientPath:
                        {
                            using (PathGradientBrush pathGradientBrush = new PathGradientBrush(path))
                            {
                                ColorBlend colorBlend2 = new ColorBlend();
                                colorBlend2.Colors = GetFillBlend();
                                colorBlend2.Positions = ColorFillBlend.iPoint;
                                pathGradientBrush.FocusScales = FocalPoints.FocusScales();
                                pathGradientBrush.CenterPoint = new PointF((float)base.Width * FocalPoints.CenterPoint().X, (float)base.Height * FocalPoints.CenterPoint().Y);
                                pathGradientBrush.InterpolationColors = colorBlend2;
                                e.Graphics.FillPath(pathGradientBrush, path);
                            }

                            break;
                        }
                    case eFillType.GradientLinear:
                        {
                            using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(ButtonArea, Color.White, Color.White, FillTypeLinear))
                            {
                                ColorBlend colorBlend = new ColorBlend();
                                colorBlend.Colors = GetFillBlend();
                                colorBlend.Positions = ColorFillBlend.iPoint;
                                linearGradientBrush.InterpolationColors = colorBlend;
                                e.Graphics.FillPath(linearGradientBrush, path);
                            }

                            break;
                        }
                }
            }

            if (BorderShow)
            {
                e.Graphics.DrawPath(pen, path);
            }

            path.Dispose();
            PointF pointF = ImageLocation(GetStringFormat(SideImageAlign), base.Size, SideImageSize);
            if (SideImageBehindText && SideImage != null)
            {
                e.Graphics.DrawImage(EnableDisableImage(SideImage), pointF.X, pointF.Y, SideImageSize.Width, SideImageSize.Height);
            }

            SetImageAndText(e.Graphics);
            if (Image != null)
            {
                e.Graphics.DrawImage(EnableDisableImage(Image), Imagept.X, Imagept.Y, ImageSize.Width, ImageSize.Height);
            }

            if (TextShadowShow)
            {
                TextArea.Offset(1f, 1f);
                e.Graphics.DrawString(Text, Font, new SolidBrush(color3), TextArea, GetStringFormat(TextAlign));
                TextArea.Offset(-1f, -1f);
            }

            e.Graphics.DrawString(Text, Font, new SolidBrush(color2), TextArea, GetStringFormat(TextAlign));
            if (!SideImageBehindText && SideImage != null)
            {
                e.Graphics.DrawImage(EnableDisableImage(SideImage), pointF.X, pointF.Y, SideImageSize.Width, SideImageSize.Height);
            }

            pen.Dispose();
            base.OnPaint(e);
        }

        private Bitmap EnableDisableImage(Image img)
        {
            if (base.Enabled)
            {
                return (Bitmap)img;
            }

            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            ColorMatrix colorMatrix = new ColorMatrix(new float[5][]
            {
            new float[5] { 0.5f, 0.5f, 0.5f, 0f, 0f },
            new float[5] { 0.5f, 0.5f, 0.5f, 0f, 0f },
            new float[5] { 0.5f, 0.5f, 0.5f, 0f, 0f },
            new float[5] { 0f, 0f, 0f, 1f, 0f },
            new float[5] { 0f, 0f, 0f, 0f, 1f }
            });
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);
            graphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttributes);
            graphics.Dispose();
            return bitmap;
        }

        private GraphicsPath GetPath()
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            switch (_Shape)
            {
                case eShape.Ellipse:
                    graphicsPath.AddEllipse(ButtonArea);
                    break;
                case eShape.Rectangle:
                    graphicsPath = GetRoundedRectPath(ButtonArea);
                    break;
                case eShape.TriangleUp:
                    {
                        PointF[] points4 = new PointF[3]
                        {
                new PointF(ButtonArea.Width / 2f, ButtonArea.Y),
                new PointF(ButtonArea.Width, ButtonArea.Y + ButtonArea.Height),
                new PointF(ButtonArea.X, ButtonArea.Y + ButtonArea.Height)
                        };
                        graphicsPath.AddPolygon(points4);
                        break;
                    }
                case eShape.TriangleDown:
                    {
                        PointF[] points3 = new PointF[3]
                        {
                new PointF(ButtonArea.X, ButtonArea.Y),
                new PointF(ButtonArea.Width / 2f, ButtonArea.Y + ButtonArea.Height),
                new PointF(ButtonArea.X + ButtonArea.Width, ButtonArea.Y)
                        };
                        graphicsPath.AddPolygon(points3);
                        break;
                    }
                case eShape.TriangleLeft:
                    {
                        PointF[] points2 = new PointF[3]
                        {
                new PointF(ButtonArea.X, ButtonArea.Y + ButtonArea.Height / 2f),
                new PointF(ButtonArea.Width, ButtonArea.Y),
                new PointF(ButtonArea.Width, ButtonArea.Y + ButtonArea.Height)
                        };
                        graphicsPath.AddPolygon(points2);
                        break;
                    }
                case eShape.TriangleRight:
                    {
                        PointF[] points = new PointF[3]
                        {
                new PointF(ButtonArea.X, ButtonArea.Y),
                new PointF(ButtonArea.Width, ButtonArea.Y + ButtonArea.Height / 2f),
                new PointF(ButtonArea.X, ButtonArea.Y + ButtonArea.Height)
                        };
                        graphicsPath.AddPolygon(points);
                        break;
                    }
            }

            return graphicsPath;
        }

        private StringFormat GetStringFormat(ContentAlignment ctrlalign)
        {
            StringFormat stringFormat = new StringFormat();
            switch (ctrlalign)
            {
                case ContentAlignment.MiddleCenter:
                    stringFormat.LineAlignment = StringAlignment.Center;
                    stringFormat.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    stringFormat.LineAlignment = StringAlignment.Center;
                    stringFormat.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleRight:
                    stringFormat.LineAlignment = StringAlignment.Center;
                    stringFormat.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.TopCenter:
                    stringFormat.LineAlignment = StringAlignment.Near;
                    stringFormat.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopLeft:
                    stringFormat.LineAlignment = StringAlignment.Near;
                    stringFormat.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    stringFormat.LineAlignment = StringAlignment.Near;
                    stringFormat.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    stringFormat.LineAlignment = StringAlignment.Far;
                    stringFormat.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    stringFormat.LineAlignment = StringAlignment.Far;
                    stringFormat.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                    stringFormat.LineAlignment = StringAlignment.Far;
                    stringFormat.Alignment = StringAlignment.Far;
                    break;
            }

            return stringFormat;
        }

        public void SetImageAndText(Graphics g)
        {
            PressedOffset = 0;
            if (MouseDrawState == eMouseDrawState.Down)
            {
                PressedOffset = 1;
            }

            if (Image != null)
            {
                ImageSizeUse = ImageSize;
            }
            else
            {
                ImageSizeUse = new Size(0, 0);
            }

            checked
            {
                switch (TextImageRelation)
                {
                    case TextImageRelation.Overlay:
                    case TextImageRelation.ImageAboveText:
                    case TextImageRelation.TextAboveImage:
                        TextArea = AdjustRect(ButtonArea, TextMargin);
                        ImageArea = ButtonArea;
                        TextArea.Y += PressedOffset;
                        Imagept = ImageLocation(GetStringFormat(ImageAlign), ButtonArea.Size, ImageSizeUse);
                        Imagept.X += ButtonArea.X;
                        break;
                    case TextImageRelation.ImageBeforeText:
                        {
                            SizeF sizeF2 = g.MeasureString(Text, Font);
                            TextArea = AdjustRect(ButtonArea, TextMargin);
                            TextArea.Width -= ImageSizeUse.Width - 4;
                            TextArea.Y += PressedOffset;
                            ImageArea = new RectangleF(TextArea.X - (float)ImageSizeUse.Width, ButtonArea.Y, ImageSizeUse.Width, ImageSizeUse.Height);
                            Imagept = ImageLocation(GetStringFormat(ImageAlign), ButtonArea.Size, ImageArea.Size);
                            switch (GetStringFormat(TextAlign).Alignment)
                            {
                                case StringAlignment.Center:
                                    Imagept.X = ButtonArea.X + (ButtonArea.Width - sizeF2.Width - (float)ImageSizeUse.Width) / 2f;
                                    TextArea.X = ButtonArea.X + (float)ImageSizeUse.Width;
                                    break;
                                case StringAlignment.Near:
                                    Imagept.X = ButtonArea.X + 4f;
                                    TextArea.X = ButtonArea.X + (float)ImageSizeUse.Width + 4f;
                                    break;
                                case StringAlignment.Far:
                                    Imagept.X = ButtonArea.X + TextArea.Width - sizeF2.Width - 12f;
                                    TextArea.X = ButtonArea.X + (float)ImageSizeUse.Width - 8f;
                                    break;
                            }

                            break;
                        }
                    case TextImageRelation.TextBeforeImage:
                        {
                            SizeF sizeF = g.MeasureString(Text, Font);
                            TextArea = AdjustRect(ButtonArea, TextMargin);
                            TextArea.Width -= ImageSizeUse.Width - 8;
                            TextArea.Y += PressedOffset;
                            ImageArea = new RectangleF(TextArea.X, ButtonArea.Y, ImageSizeUse.Width, ImageSizeUse.Height);
                            Imagept = ImageLocation(GetStringFormat(ImageAlign), ButtonArea.Size, ImageArea.Size);
                            switch (GetStringFormat(TextAlign).Alignment)
                            {
                                case StringAlignment.Center:
                                    Imagept.X = (TextArea.Width - sizeF.Width) / 2f + sizeF.Width;
                                    TextArea.X = -4f;
                                    break;
                                case StringAlignment.Near:
                                    Imagept.X = sizeF.Width + 8f;
                                    TextArea.X = 4f;
                                    break;
                                case StringAlignment.Far:
                                    Imagept.X = TextArea.Width - 12f;
                                    TextArea.X = -16f;
                                    break;
                            }

                            break;
                        }
                }

                Imagept.Y += (float)PressedOffset + ButtonArea.Y;
            }
        }

        public PointF ImageLocation(StringFormat sf, SizeF Area, SizeF ImageArea)
        {
            PointF result = default(PointF);
            switch (sf.Alignment)
            {
                case StringAlignment.Center:
                    result.X = (Area.Width - ImageArea.Width) / 2f;
                    break;
                case StringAlignment.Near:
                    result.X = 2f;
                    break;
                case StringAlignment.Far:
                    result.X = Area.Width - ImageArea.Width - 2f;
                    break;
            }

            switch (sf.LineAlignment)
            {
                case StringAlignment.Center:
                    result.Y = (Area.Height - ImageArea.Height) / 2f;
                    break;
                case StringAlignment.Near:
                    result.Y = 2f;
                    break;
                case StringAlignment.Far:
                    result.Y = Area.Height - ImageArea.Height - 2f;
                    break;
            }

            return result;
        }

        private void UpdateDimBlends()
        {
            _HoverColorBlend = (Color[])_ColorFillBlend.iColor.Clone();
            _ClickColorBlend = (Color[])_ColorFillBlend.iColor.Clone();
            _DisabledBlend = (Color[])_ColorFillBlend.iColor.Clone();
            checked
            {
                int num = _ColorFillBlend.iColor.Length - 1;
                for (int i = 0; i <= num; i++)
                {
                    Color color = _ColorFillBlend.iColor[i];
                    _HoverColorBlend[i] = DimTheColor(color, _DimFactorHover);
                    _ClickColorBlend[i] = DimTheColor(color, _DimFactorClick);
                    _DisabledBlend[i] = GrayTheColor(color);
                }
            }
        }

        private void UpdateDimColors()
        {
            _HoverColorSolid = DimTheColor(_ColorFillSolid, _DimFactorHover);
            _ClickColorSolid = DimTheColor(_ColorFillSolid, _DimFactorClick);
            _DisabledSolid = GrayTheColor(_ColorFillSolid);
        }

        public Color DimTheColor(Color DimColor, int DimDegree)
        {
            if (DimColor == Color.Transparent || DimDegree == 0)
            {
                return DimColor;
            }

            checked
            {
                int num = DimColor.R + DimDegree;
                int num2 = DimColor.G + DimDegree;
                int num3 = DimColor.B + DimDegree;
                if (num > 255)
                {
                    num = 255;
                }

                if (num2 > 255)
                {
                    num2 = 255;
                }

                if (num3 > 255)
                {
                    num3 = 255;
                }

                if (num < 0)
                {
                    num = 0;
                }

                if (num2 < 0)
                {
                    num2 = 0;
                }

                if (num3 < 0)
                {
                    num3 = 0;
                }

                return Color.FromArgb(num, num2, num3);
            }
        }

        public Color GrayTheColor(Color GrayColor)
        {
            checked
            {
                int num = (int)Math.Round(unchecked((double)(int)GrayColor.R * 0.3 + (double)(int)GrayColor.G * 0.59 + (double)(int)GrayColor.B * 0.11));
                return Color.FromArgb(GrayColor.A, num, num, num);
            }
        }

        private Color[] GetFillBlend()
        {
            if (base.Enabled)
            {
                if (MouseDrawState == eMouseDrawState.Over)
                {
                    return _HoverColorBlend;
                }

                if (MouseDrawState == eMouseDrawState.Down)
                {
                    return _ClickColorBlend;
                }

                return _ColorFillBlend.iColor;
            }

            return _DisabledBlend;
        }

        private Color GetFill()
        {
            if (base.Enabled)
            {
                if (MouseDrawState == eMouseDrawState.Over)
                {
                    return _HoverColorSolid;
                }

                if (MouseDrawState == eMouseDrawState.Down)
                {
                    return _ClickColorSolid;
                }

                return _ColorFillSolid;
            }

            return _DisabledSolid;
        }

        public RectangleF AdjustRect(RectangleF BaseRect, Padding Pad)
        {
            BaseRect.Width -= Pad.Horizontal;
            BaseRect.Height -= Pad.Vertical;
            BaseRect.Offset(Pad.Left, Pad.Top);
            return BaseRect;
        }

        public Rectangle AdjustRect(Rectangle BaseRect, Padding Pad)
        {
            checked
            {
                BaseRect.Width -= Pad.Horizontal;
                BaseRect.Height -= Pad.Vertical;
                BaseRect.Offset(Pad.Left, Pad.Top);
                return BaseRect;
            }
        }

        public GraphicsPath GetRoundedRectPath(RectangleF BaseRect)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            checked
            {
                if (Corners.All == -1)
                {
                    GraphicsPath graphicsPath2 = graphicsPath;
                    if (Corners.UpperLeft == 0)
                    {
                        graphicsPath2.AddLine(BaseRect.X, BaseRect.Y, BaseRect.X, BaseRect.Y);
                    }
                    else
                    {
                        RectangleF rect = new RectangleF(BaseRect.Location, new SizeF(Corners.UpperLeft * 2, Corners.UpperLeft * 2));
                        graphicsPath2.AddArc(rect, 180f, 90f);
                    }

                    if (Corners.UpperRight == 0)
                    {
                        graphicsPath2.AddLine(BaseRect.X + (float)Corners.UpperLeft, BaseRect.Y, BaseRect.Right - (float)Corners.UpperRight, BaseRect.Top);
                    }
                    else
                    {
                        RectangleF rect = new RectangleF(BaseRect.Location, new SizeF(Corners.UpperRight * 2, Corners.UpperRight * 2));
                        rect.X = BaseRect.Right - (float)(Corners.UpperRight * 2);
                        graphicsPath2.AddArc(rect, 270f, 90f);
                    }

                    if (Corners.LowerRight == 0)
                    {
                        graphicsPath2.AddLine(BaseRect.Right, BaseRect.Top + (float)Corners.UpperRight, BaseRect.Right, BaseRect.Bottom - (float)Corners.LowerRight);
                    }
                    else
                    {
                        RectangleF rect = new RectangleF(BaseRect.Location, new SizeF(Corners.LowerRight * 2, Corners.LowerRight * 2));
                        rect.Y = BaseRect.Bottom - (float)(Corners.LowerRight * 2);
                        rect.X = BaseRect.Right - (float)(Corners.LowerRight * 2);
                        graphicsPath2.AddArc(rect, 0f, 90f);
                    }

                    if (Corners.LowerLeft == 0)
                    {
                        graphicsPath2.AddLine(BaseRect.Right - (float)Corners.LowerRight, BaseRect.Bottom, BaseRect.X - (float)Corners.LowerLeft, BaseRect.Bottom);
                    }
                    else
                    {
                        RectangleF rect = new RectangleF(BaseRect.Location, new SizeF(Corners.LowerLeft * 2, Corners.LowerLeft * 2));
                        rect.Y = BaseRect.Bottom - (float)(Corners.LowerLeft * 2);
                        graphicsPath2.AddArc(rect, 90f, 90f);
                    }

                    graphicsPath2.CloseFigure();
                    graphicsPath2 = null;
                }
                else
                {
                    GraphicsPath graphicsPath3 = graphicsPath;
                    if (Corners.All == 0)
                    {
                        graphicsPath3.AddRectangle(BaseRect);
                    }
                    else
                    {
                        RectangleF rect = new RectangleF(BaseRect.Location, new SizeF(Corners.All * 2, Corners.All * 2));
                        graphicsPath3.AddArc(rect, 180f, 90f);
                        rect.X = BaseRect.Right - (float)(Corners.All * 2);
                        graphicsPath3.AddArc(rect, 270f, 90f);
                        rect.Y = BaseRect.Bottom - (float)(Corners.All * 2);
                        graphicsPath3.AddArc(rect, 0f, 90f);
                        rect.X = BaseRect.Left;
                        graphicsPath3.AddArc(rect, 90f, 90f);
                    }

                    graphicsPath3.CloseFigure();
                    graphicsPath3 = null;
                }

                return graphicsPath;
            }
        }

        private void CButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (base.DesignMode)
            {
                ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                ArrayList arrayList = new ArrayList();
                arrayList.Clear();
                selectionService.SetSelectedComponents(arrayList, SelectionTypes.Replace);
                arrayList.Add(this);
                selectionService.SetSelectedComponents(arrayList, SelectionTypes.Add);
                if (e.Button == MouseButtons.Right)
                {
                    if (CenterPtTracker.IsActive)
                    {
                        FocalPoints = new cFocalPoints(new PointF(0.5f, 0.5f), FocalPoints.FocusScales());
                        Invalidate();
                    }
                    else if (FocusPtTracker.IsActive)
                    {
                        FocalPoints = new cFocalPoints(FocalPoints.CenterPoint(), new PointF(0f, 0f));
                        Invalidate();
                    }
                }
            }
            else if (base.Enabled && GetPath().IsVisible(e.X, e.Y))
            {
                MouseDrawState = eMouseDrawState.Down;
                Invalidate(Rectangle.Round(ButtonArea));
            }
        }

        private void CButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (base.DesignMode)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (CenterPtTracker.IsActive)
                    {
                        FocalPoints = new cFocalPoints(new PointF((float)((double)e.X / (double)base.Width), (float)((double)e.Y / (double)base.Height)), FocalPoints.FocusScales());
                        Invalidate();
                    }
                    else if (FocusPtTracker.IsActive)
                    {
                        FocalPoints = new cFocalPoints(FocalPoints.CenterPoint(), new PointF((float)((double)e.X / (double)base.Width), (float)((double)e.Y / (double)base.Height)));
                        Invalidate();
                    }
                }
            }
            else if (GetPath().IsVisible(e.X, e.Y))
            {
                if (MouseDrawState != eMouseDrawState.Down)
                {
                    MouseDrawState = eMouseDrawState.Over;
                }

                Invalidate(Rectangle.Round(ButtonArea));
            }
            else if (MouseDrawState != eMouseDrawState.Up)
            {
                MouseDrawState = eMouseDrawState.Up;
                Invalidate(Rectangle.Round(ButtonArea));
            }
        }

        private void CButton_MouseLeave(object sender, EventArgs e)
        {
            if (MouseDrawState != eMouseDrawState.Up)
            {
                MouseDrawState = eMouseDrawState.Up;
                Invalidate(Rectangle.Round(ButtonArea));
            }
        }

        private void CButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseDrawState == eMouseDrawState.Down)
            {
                ClickButtonArea?.Invoke(this, new EventArgs());
            }

            MouseDrawState = eMouseDrawState.Up;
            Invalidate(Rectangle.Round(ButtonArea));
        }

        private void CButton_Resize(object sender, EventArgs e)
        {
            if (base.DesignMode)
            {
                CenterPtTracker.TrackerRectangle = CenterPtTrackerRectangle();
                FocusPtTracker.TrackerRectangle = FocusPtTrackerRectangle();
            }
        }
    }
#if false // Journal de décompilation
'10' éléments dans le cache
------------------
Résoudre : 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\mscorlib.dll'
------------------
Résoudre : 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.dll'
------------------
Résoudre : 'Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Microsoft.VisualBasic.dll'
------------------
Résoudre : 'System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Windows.Forms.dll'
------------------
Résoudre : 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Drawing.dll'
------------------
Résoudre : 'System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Introuvable par le nom : 'System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
#endif

}
