namespace FamiStudio
{
    public class ParamCheckBox : Control
    {
        // MATTT : What was that again?
        private float bmpScale = Platform.IsMobile ? DpiScaling.Window * 0.25f : 1.0f;

        public delegate void CheckedChangedDelegate(Control sender, bool check);
        public event CheckedChangedDelegate CheckedChanged;

        private bool hover;
        private ParamInfo param;
        private TextureAtlasRef bmpCheckOn;
        private TextureAtlasRef bmpCheckOff;

        public event ControlDelegate ValueChangeStart;
        public event ControlDelegate ValueChangeEnd;

        public ParamCheckBox(ParamInfo p)
        {
            param = p;
            height = 16;
        }

        protected override void OnAddedToContainer()
        {
            bmpCheckOn  = window.Graphics.GetTextureAtlasRef("CheckBoxYes");
            bmpCheckOff = window.Graphics.GetTextureAtlasRef("CheckBoxNo");
            width  = DpiScaling.ScaleCustom(bmpCheckOn.ElementSize.Width,  bmpScale);
            height = DpiScaling.ScaleCustom(bmpCheckOn.ElementSize.Height, bmpScale);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Left && IsParamEnabled())
            {
                ValueChangeStart?.Invoke(this);
                param.SetValue(param.GetValue() == 0 ? 1 : 0);
                ValueChangeEnd?.Invoke(this);
                CheckedChanged?.Invoke(this, param.GetValue() != 0);
                MarkDirty();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            SetAndMarkDirty(ref hover, true);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            SetAndMarkDirty(ref hover, false);
        }

        private bool IsParamEnabled()
        {
            return enabled && (param.IsEnabled == null || param.IsEnabled());
        }

        protected override void OnRender(Graphics g)
        {
            var c = g.GetCommandList();
            var paramEnabled = IsParamEnabled();
            var opacity = paramEnabled ? hover ? 0.6f : 1.0f : 0.25f;
            var color = Color.Black.Transparent(opacity);

            c.DrawRectangle(0, 0, width - 1, height - 1, color); 
            c.DrawTextureAtlas(param.GetValue() != 0 ? bmpCheckOn : bmpCheckOff, 0, 0, 1, color);
        }
    }
}