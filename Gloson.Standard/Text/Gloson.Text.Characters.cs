using System;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Control Characters
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ControlCharacters {
    #region Constants

    /// <summary>
    /// Null constant (0x00)
    /// </summary>
    public const Char NULL = '\0';

    /// <summary>
    /// Start Of Heading (0x01)
    /// </summary>
    public const Char SOH = (Char)0x01;

    /// <summary>
    /// Start of TeXt 
    /// </summary>
    public const Char STX = (Char)0x02;

    /// <summary>
    /// End of TeXt  
    /// </summary>
    public const Char ETX = (Char)0x03;

    /// <summary>
    /// End Of Transmission
    /// </summary>
    public const Char EOT = (Char)0x04;

    /// <summary>
    /// Enquiry 
    /// </summary>
    public const Char ENQ = (Char)0x05;

    /// <summary>
    /// Acknowledge 
    /// </summary>
    public const Char ACK = (Char)0x06;

    /// <summary>
    /// Bell 
    /// </summary>
    public const Char BEL = (Char)0x07;

    /// <summary>
    /// BackSpace 
    /// </summary>
    public const Char BS = (Char)0x08;

    /// <summary>
    /// Horizontal Tabulation 
    /// </summary>
    public const Char HT = (Char)0x09;

    /// <summary>
    /// Line feed
    /// </summary>
    public const Char LF = (Char)0x0A;

    /// <summary>
    /// Vertical tabulation 
    /// </summary>
    public const Char VT = (Char)0x0B;

    /// <summary>
    /// Form Feed
    /// </summary>
    public const Char FF = (Char)0x0C;

    /// <summary>
    /// Carriage Return 
    /// </summary>
    public const Char CR = (Char)0x0D;

    /// <summary>
    /// Shift Out 
    /// </summary>

    public const Char SO = (Char)0x0E;

    /// <summary>
    /// Shift In 
    /// </summary>
    public const Char SI = (Char)0x0F;

    /// <summary>
    /// Data Link Escape 
    /// </summary>
    public const Char DLE = (Char)0x10;

    /// <summary>
    /// Device Control 1 
    /// </summary>
    public const Char DC1 = (Char)0x11;

    /// <summary>
    /// Device Control 2 
    /// </summary>
    public const Char DC2 = (Char)0x12;

    /// <summary>
    /// Device control 3 
    /// </summary>
    public const Char DC3 = (Char)0x13;

    /// <summary>
    /// Device Control 4 
    /// </summary>
    public const Char DC4 = (Char)0x14;

    /// <summary>
    /// Negative AcKnowledge 
    /// </summary>
    public const Char NAK = (Char)0x15;

    /// <summary>
    /// Synchronize 
    /// </summary>
    public const Char SYN = (Char)0x16;

    /// <summary>
    /// End of Transmission Block 
    /// </summary>
    public const Char ETB = (Char)0x17;

    /// <summary>
    /// Cancel 
    /// </summary>
    public const Char CAN = (Char)0x18;

    /// <summary>
    /// End of Medium 
    /// </summary>
    public const Char EM = (Char)0x19;

    /// <summary>
    /// Substitute 
    /// </summary>
    public const Char SUB = (Char)0x1A;

    /// <summary>
    /// Escape 
    /// </summary>
    public const Char ESC = (Char)0x1B;

    /// <summary>
    /// File Separator 
    /// </summary>
    public const Char FS = (Char)0x1C;

    /// <summary>
    /// Group Separator 
    /// </summary>
    public const Char GS = (Char)0x1D;

    /// <summary>
    /// Record Separator 
    /// </summary>
    public const Char RS = (Char)0x1E;

    /// <summary>
    /// Unit Separator 
    /// </summary>
    public const Char US = (Char)0x1F;

    /// <summary>
    /// Space 
    /// </summary>
    public const Char SPC = (Char)0x20;

    #endregion Constants
  }

}
