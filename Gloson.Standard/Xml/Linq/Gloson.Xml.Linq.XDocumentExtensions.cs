using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Gloson.Xml.Linq {

  //---------------------------------------------------------------------------
  //
  /// <summary>
  /// XDocument extensions
  /// </summary>
  //
  //---------------------------------------------------------------------------

  public static class XDocumentExtensions {
    #region Public

    /// <summary>
    /// Add standard caption like xml version = etc.
    /// </summary>
    /// <param name="document">Document to modify</param>
    /// <param name="comment">Add standard caption</param>
    public static void AddStandardCaption(this XDocument document,
                                          String comment) {
      if (Object.ReferenceEquals(null, document))
        throw new ArgumentNullException(nameof(document));

      XComment xComment = null;

      using (var en = document.Nodes().GetEnumerator()) {
        while (en.MoveNext()) {
          if (en.Current is XElement)
            break;

          if (Object.ReferenceEquals(null, xComment))
            xComment = en.Current as XComment;
        }

        // Creating the declaration
        if (Object.ReferenceEquals(null, document.Declaration))
          document.Declaration = new XDeclaration("1.0", "utf-8", "yes");

        // Changing or creating comment
        if (!String.IsNullOrEmpty(comment)) {
          if (Object.ReferenceEquals(null, xComment)) {
            xComment = new XComment(comment);

            document.AddFirst(xComment);
          }

          xComment.Value = comment;
        }
      }
    }

    /// <summary>
    /// Add standard caption (like xml version = etc.) 
    /// </summary>
    /// <param name="document">Document to modify</param>
    public static void AddStandardCaption(this XDocument document) {
      AddStandardCaption(document, null);
    }

    /// <summary>
    /// Save to string
    /// </summary>
    /// <param name="document">Document to save</param>
    public static String SaveToString(this XDocument document) {
      if (Object.ReferenceEquals(null, document))
        throw new ArgumentNullException(nameof(document));

      StringBuilder Sb = new StringBuilder();

      if (!Object.ReferenceEquals(null, document.Declaration))
        Sb.Append(document.Declaration.ToString());

      if (Sb.Length > 0)
        Sb.AppendLine();

      Sb.Append(document.ToString(SaveOptions.None));

      return Sb.ToString();
    }

    /// <summary>
    /// Load XML document from string
    /// </summary>
    /// <param name="xmlText">XML Text</param>
    public static XDocument LoadFromString(String xmlText) {
      if (String.IsNullOrEmpty(xmlText))
        return new XDocument();

      using (TextReader tr = new StringReader(xmlText)) {
        return XDocument.Load(tr);
      }
    }

    #endregion Public
  }

}
