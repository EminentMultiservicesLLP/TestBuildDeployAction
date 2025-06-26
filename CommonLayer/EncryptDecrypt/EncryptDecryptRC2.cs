using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.EncryptDecrypt
{
    public class EncryptDecryptRC2
    {

        static RC2CryptoServiceProvider _rc2CryptoServiceProvider;
        static EncryptDecryptRC2()
        {
            _rc2CryptoServiceProvider = new RC2CryptoServiceProvider();

            //Ensure that you create your own key and IV.
            //key and IV provided to ensure that sample works

            int intVal = 0;
            TryCatch.Run(() =>
            {
                intVal = _rc2CryptoServiceProvider.KeySize;

                _rc2CryptoServiceProvider.Key = new byte[] { 111, 222, 121, 82, 172, 21, 185, 152, 228, 182, 72, 132, 123, 123, 131, 12 };
                _rc2CryptoServiceProvider.IV = new byte[] { 172, 223, 13, 42, 252, 102, 81, 211 };

            });
           
        }

        public static string EncryptString(string AString)
        {
            if (AString == string.Empty)
            {
                return AString;
            }
            else
            {
                byte[] encryptedData = null; string result = string.Empty;

                ICryptoTransform encryptor = default(ICryptoTransform);
                encryptor = _rc2CryptoServiceProvider.CreateEncryptor();

                TryCatch.Run(() =>
                {
                    using (MemoryStream dataStream = new MemoryStream())
                    {
                        //Create the encrypted stream
                        using (CryptoStream encryptedStream = new CryptoStream(dataStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter theWriter = new StreamWriter(encryptedStream))
                            {
                                //Write the string to the memory stream
                                theWriter.Write(AString);
                                //End the writing
                                theWriter.Flush();
                                encryptedStream.FlushFinalBlock();
                                //Position back at start
                                dataStream.Position = 0;

                                //Create area for data
                                encryptedData = new byte[dataStream.Length + 1];

                                //Read data from memory
                                dataStream.Read(encryptedData, 0, Convert.ToInt32(dataStream.Length));

                                //Convert to String
                                result = Convert.ToBase64String(encryptedData);
                            }
                        }
                    }
                });
                return result;
            }
               
        }

        public static string DecryptString(string AString)
        {
            if ((AString == String.Empty))
            {
                return AString;
            }
            else
            {
                byte[] encryptedData; string result = string.Empty; int strLen = 0;
                encryptedData = Convert.FromBase64String(AString);
                TryCatch.Run(() =>
                {
                    using(MemoryStream dataStream = new MemoryStream())
                    {
                        // Create decryptor and stream
                        ICryptoTransform decryptor = _rc2CryptoServiceProvider.CreateDecryptor();
                        using (CryptoStream encryptedStream = new CryptoStream(dataStream, decryptor, CryptoStreamMode.Write))
                        {
                            // Write the decrypted data to the memory stream
                            encryptedStream.Write(encryptedData, 0, (encryptedData.Length - 1));
                            encryptedStream.FlushFinalBlock();
                            
                            // Position back at start
                            dataStream.Position = 0;

                            // Determine length of decrypted string
                            strLen = Convert.ToInt32(dataStream.Length);

                            // Create area for data
                            encryptedData = new byte[strLen-1];

                            dataStream.Read(encryptedData, 0, strLen);
                            
                            StringBuilder retStr = new StringBuilder();
                            int i;
                            for (i = 0; (i <= (strLen - 1)); i++)
                            {
                                retStr.Append(((char)(encryptedData[i])));
                            }

                            // Return result
                            result = retStr.ToString();
                        }
                    }
                });
                return result;
            }

        }



    }
}
