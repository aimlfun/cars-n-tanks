namespace CarsAndTanks.UX.Controls;

/// <summary>
/// Based on XAML Controls Gallery as a reference.
/// We declare them in one place for changing consistently throughout the app.
/// </summary>
public static class FluentStyle
{
    /// <summary>
    /// Color of navigation panels.
    /// </summary>
    public readonly static Color NavigationBackgroundColor = Color.FromArgb(243, 243, 243);

    /// <summary>
    /// 
    /// </summary>
    public readonly static Color BackgroundColor = Color.FromArgb(249, 249, 249);

    /// <summary>
    /// 
    /// </summary>
    public readonly static SolidBrush BackgroundColorBrush = new(BackgroundColor);

    // === BUTTON ==

    /// <summary>
    /// Buttons should have a 4px radius.
    /// </summary>
    public const float ButtonBorderRadius = 4;

    /// <summary>
    /// 
    /// </summary>
    public readonly static Color ButtonBackgroundColor = Color.FromArgb(251, 251, 251);

    /// <summary>
    /// 
    /// </summary>
    public readonly static SolidBrush ButtonBackgroundBrush = new(ButtonBackgroundColor);

    /// <summary>
    /// 
    /// </summary>
    public readonly static Color ButtonBackgroundHoverColor = Color.FromArgb(246, 246, 246);

    /// <summary>
    /// 
    /// </summary>
    public readonly static SolidBrush ButtonBackgroundBrushHover = new(ButtonBackgroundHoverColor);

    /// <summary>
    /// 
    /// </summary>
    public readonly static Color ButtonBorderColor = Color.FromArgb(229, 229, 229);

    /// <summary>
    /// 
    /// </summary>
    public readonly static Pen ButtonBorderPen = new(ButtonBorderColor);

    /// <summary>
    /// 
    /// </summary>
    public readonly static Color ButtonTextColor = Color.FromArgb(0, 0, 0);

    /// <summary>
    /// 
    /// </summary>
    public readonly static Color ButtonDisabledColor = Color.Gray;

    /// <summary>
    /// 
    /// </summary>
    public readonly static SolidBrush ButtonTextBrush = new(ButtonTextColor);

    /// <summary>
    /// 
    /// </summary>
    public readonly static SolidBrush DisabledButtonTextBrush = new(ButtonDisabledColor);


    // === LISTBOX ==

    /// <summary>
    /// 
    /// </summary>
    public readonly static Color ListBoxBackgroundColor = Color.FromArgb(243, 243, 243);

    /// <summary>
    /// 
    /// </summary>
    public readonly static SolidBrush ListBoxBackgroundBrush = new(ListBoxBackgroundColor);

    /// <summary>
    /// 
    /// </summary>
    public readonly static Color ListBoxBackgroundSelectedColor = Color.FromArgb(170, 208, 246);

    /// <summary>
    /// 
    /// </summary>
    public readonly static SolidBrush ListBoxBackgroundSelectedBrush = new(ListBoxBackgroundSelectedColor);
}
