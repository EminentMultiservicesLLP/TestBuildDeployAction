using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace CGHSBilling.Common
{
    public class CommonMethods
    {
        public static string ToXML(object oObject)
        {
            if (oObject == null)
                return string.Empty;

            try
            {
                var xmlDoc = new XmlDocument();
                var xmlSerializer = new XmlSerializer(oObject.GetType());

                using (var xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, oObject);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    return xmlDoc.InnerXml;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}