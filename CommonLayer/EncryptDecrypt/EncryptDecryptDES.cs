using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.Extensions;

namespace CommonLayer.EncryptDecrypt
{
    public class EncryptDecryptDES
    {
         private static DESCryptoServiceProvider _desCryptoServiceProvider;

         static EncryptDecryptDES()
         {
             _desCryptoServiceProvider = new DESCryptoServiceProvider();

             _desCryptoServiceProvider.Key = new Byte[] { 111, 212, 121, 82, 172, 21, 185, 152 };
             _desCryptoServiceProvider.IV = new Byte[] { 172, 223, 13, 42, 252, 102, 81, 211 };
         }
         public static string EncryptString(string AString)
         {
             if (AString == string.Empty)
             {
                 return AString;
             }
             else
             {
                 AString = AString.ReverseString();
                 
                 byte[] encryptedData = null;
                 MemoryStream dataStream = default(MemoryStream);

                 ICryptoTransform encryptor = default(ICryptoTransform);
                 encryptor = _desCryptoServiceProvider.CreateEncryptor();

                 try
                 {
                     dataStream = new MemoryStream();
                     CryptoStream encryptedStream = default(CryptoStream);
                     try
                     {
                         encryptedStream = new CryptoStream(dataStream, encryptor, CryptoStreamMode.Write);

                         StreamWriter theWriter = default(StreamWriter);
                         try
                         {
                             theWriter = new StreamWriter(encryptedStream);
                             theWriter.Write(AString);

                             theWriter.Flush();
                             encryptedStream.FlushFinalBlock();

                             dataStream.Position = 0;
                             encryptedData = new byte[dataStream.Length + 1];

                             dataStream.Read(encryptedData, 0, Convert.ToInt32(dataStream.Length));
                             return Convert.ToBase64String(encryptedData);
                         }
                         finally
                         {
                             theWriter.Close();
                         }
                     }
                     finally
                     {
                         encryptedStream.Close();
                     }
                 }
                 finally
                 {
                     dataStream.Close();
                 }
             }
         }

         public static string DecryptString(string AString)
         {
             if (AString == string.Empty)
             {
                 return AString;
             }
             else
             {
                 byte[] encryptedData = null;
                 CryptoStream encryptedStream = default(CryptoStream);
                 int strLen = 0;string retStr = null;

                 encryptedData = Convert.FromBase64String(AString);

                 TryCatch.RunThrow(() =>
                 {
                     using (MemoryStream dataStream = new MemoryStream())
                     {
                         //Create decryptor and stream
                         ICryptoTransform decryptor = default(ICryptoTransform);
                         decryptor = _desCryptoServiceProvider.CreateDecryptor();
                         encryptedStream = new CryptoStream(dataStream, decryptor, CryptoStreamMode.Write);


                         //Write the decrypted data to the memory stream
                         encryptedStream.Write(encryptedData, 0, encryptedData.Length - 1);
                         encryptedStream.FlushFinalBlock();
                         //AppendErrorLog("Writing to memory stream" & vbCrLf)

                         //Position back at start
                         dataStream.Position = 0;
                         strLen = Convert.ToInt32(dataStream.Length);

                         encryptedData = new byte[strLen];

                         //Read decrypted data to byte()
                         dataStream.Read(encryptedData, 0, strLen);

                         for (int i = 0; i <= strLen - 1; i++)
                             retStr += Convert.ToChar(encryptedData[i]);

                         retStr = retStr.ReverseString();
                     }
                 });
                 
                 return retStr;
             }
         }
    }
}
