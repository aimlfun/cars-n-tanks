using System.ComponentModel;
using System.Drawing.Drawing2D;
using CarsAndTanks.Utilities;

namespace CarsAndTanks.UX.Controls;

/// <summary>
/// 
/// </summary>
public class RoundedPanel : Panel
{
    /// <summary>
    /// 
    /// </summary>
    private int _radius = 10;

    /// <summary>
    /// 
    /// </summary>
    private int _panelSplit = 0;

    /// <summary>
    /// 
    /// </summary>
    private Color _panelColor = Color.FromArgb(243, 243, 243);

    /// <summary>
    /// 
    /// </summary>
    private Color _panelBottomColor = Color.FromArgb(232, 232, 232);

    /// <summary>
    /// 
    /// </summary>
    private Color _panelBorderColor = Color.FromArgb(204, 204, 204);

    /// <summary>
    /// 
    /// </summary>
    [Browsable(true)]
    [Category("Custom")]
    public Color PanelColor
    {
        get
        {
            return _panelColor;
        }

        set
        {
            if (_panelColor == value) return;

            _panelColor = value;
            Invalidate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Browsable(true)]
    [Category("Custom")]
    public Color PanelBottomColor
    {
        get
        {
            return _panelBottomColor;
        }

        set
        {
            if (_panelBottomColor == value) return;

            _panelBottomColor = value;
            Invalidate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Browsable(true)]
    [Category("Custom")]
    public Color PanelBorderColor
    {
        get
        {
            return _panelBorderColor;
        }

        set
        {
            if (_panelBorderColor == value) return;

            _panelBorderColor = value;
            Invalidate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Browsable(true)]
    [Category("Custom")]
    [Description("Number of pixels from bottom that will be in a different color.")]
    public int PanelSplit
    {
        get
        {
            return _panelSplit;
        }

        set
        {
            _panelSplit = value;
            Invalidate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Browsable(true)]
    [Category("Custom")]
    [Description("Radius for rounded corners.")]
    public int Radius
    {
        get
        {
            return _radius;
        }

        set
        {
            _radius = value;
            Invalidate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public RoundedPanel()
    {
        // we want double buffering, user painting etc.
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        UpdateStyles();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        using (SolidBrush brush2 = new(Color.FromArgb(249, 249, 249)))
        {
            g.FillRectangle(brush2, 0, 0, Width, Height);
        }

        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using (SolidBrush brush = new(_panelColor))
        {
            g.FillRoundedRectangle(brush, 0, 0, Width, Height, _radius);
        }

        using (SolidBrush brush2 = new(_panelBottomColor))
        {
            if (_panelSplit > 0) g.FillBottomHalfRoundedRectangle(brush2, 0, 0, Width - 1, Height - 1, _panelSplit, _radius);
        }

        using Pen pen = new(_panelBorderColor);
        g.DrawRoundedRectangle(pen, 0, 0, Width - 1, Height - 1, _radius);
        g.DrawLine(pen, 0, Height - _panelSplit, Width - 1, Height - _panelSplit);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    public void PaintBorder(Color borderColor)
    {
        using Graphics g = CreateGraphics();
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using Pen pen = new(borderColor);
        g.DrawRoundedRectangle(pen, 0, 0, Width - 1, Height - 1, _radius);
    }
}
