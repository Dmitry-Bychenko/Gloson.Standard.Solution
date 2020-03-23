using System.Drawing;

namespace Gloson.Text.Formats {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Horizontal Alignment
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum TextHorizontalAlignment {
    /// <summary>
    /// Left
    /// </summary>
    Left = 0,
    /// <summary>
    /// Right
    /// </summary>
    Right = 1,
    /// <summary>
    /// Center
    /// </summary>
    Center = 2,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Vertical Alignment
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum TextVerticalAlignment {
    /// <summary>
    /// Top
    /// </summary>
    Top = 0,
    /// <summary>
    /// Bottom
    /// </summary>
    Bottom = 1,
    /// <summary>
    /// Center
    /// </summary>
    Center = 2,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Font Modifiers
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum FontModifiers {
    None = 0,
    Regular = 0,
    Bold = 1,
    Italic = 2,
    Underline = 4,
    Crossed = 8,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Font Descriptor
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class FontDescriptor {
    #region Public

    /// <summary>
    /// Font Name (null in case of default)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Size
    /// </summary>
    public float Size { get; set; } = 10.0f;

    /// <summary>
    /// Modifiers
    /// </summary>
    public FontModifiers Modifiers { get; set; }

    /// <summary>
    /// ForeColor
    /// </summary>
    public Color ForeColor { get; set; } = Color.Empty;

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Text Descriptor
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class TextDescriptor {
    #region Public

    /// <summary>
    /// Horizontal Alignment
    /// </summary>
    public TextHorizontalAlignment HorizontalAlignment { get; set; }

    /// <summary>
    /// Vertical Alignment
    /// </summary>
    public TextVerticalAlignment VerticalAlignment { get; set; } = TextVerticalAlignment.Center;

    /// <summary>
    /// Font Descriptor
    /// </summary>
    public FontDescriptor Font { get; } = new FontDescriptor();

    /// <summary>
    /// Back Color
    /// </summary>
    public Color BackColor { get; set; } = Color.Empty;

    #endregion Public
  }

}
