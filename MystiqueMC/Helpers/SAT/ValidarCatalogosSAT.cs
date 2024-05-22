// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.SAT.ValidarCatalogosSAT
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace MystiqueMC.Helpers.SAT
{
  public static class ValidarCatalogosSAT
  {
    public static bool? ValidarCodigoPostal(string catalogo, string codigoPostal)
    {
      using (FileStream fileStream = new FileStream(catalogo, FileMode.Open, FileAccess.Read))
      {
        XDocument xdocument = XDocument.Load((Stream) fileStream);
        XNamespace xnamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
        XElement xelement = xdocument.Element(xnamespace + "schema");
        return xelement != null ? new bool?(xelement.Elements(xnamespace + "simpleType").Where<XElement>((Func<XElement, bool>) (c => c.HasAttributes && c.Attribute((XName) "name")?.Value == "c_CodigoPostal")).Elements<XElement>(xnamespace + "restriction").Elements<XElement>(xnamespace + "enumeration").Where<XElement>((Func<XElement, bool>) (c => c.HasAttributes && c.Attribute((XName) "value") != null)).Select<XElement, string>((Func<XElement, string>) (c => c.Attribute((XName) "value")?.Value)).Any<string>((Func<string, bool>) (c => c == codigoPostal))) : new bool?();
      }
    }
  }
}
