using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SSL
{
    public static class SSL
    {
        public const string SERVER_NAME = "https://fit.cvut.cz/";
        public const string CERT_PEM_FILENAME = "cert.pem";
        public const string URL_FILENAME = "page.html";

        public static X509Certificate2 GetCertificate(string serverName = SERVER_NAME)
        {
            try
            {
                // make Http request
                var request = (HttpWebRequest) WebRequest.Create(serverName);
                request.GetResponse();

                // get certificate
                var cert = request.ServicePoint.Certificate;
                if (cert == null)
                {
                    throw new ArgumentException("Certificate not found at: [" + serverName + "]");
                }
                // convert to new format
                var cert2 = new X509Certificate2(cert);
                return cert2;
            }
            catch (UriFormatException e)
            {
                Console.WriteLine("Server is not a valid URI: [" + serverName + "]");
                throw e;
            }
            catch (WebException e)
            {
                Console.WriteLine("Server is unable to reach: [" + serverName + "]");
                throw e;
            }
        }

        public static void WritePEMCertificate(X509Certificate2 cert, string fileName = CERT_PEM_FILENAME)
        {

            try
            {
                const string pemPrefix = "-----BEGIN CERTIFICATE-----\r\n";
                const string pemSuffix = "\n-----END CERTIFICATE-----";
                var pem = pemPrefix + Convert.ToBase64String(cert.RawData, Base64FormattingOptions.InsertLineBreaks) + pemSuffix;
                using (var fout = new StreamWriter(fileName))
                {
                    fout.Write(pem);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error writing to [" + fileName + "]");
            }
        }
        

        public static void GetURL(string url = SERVER_NAME)
        {
            try
            {
                var wc = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                wc.DownloadFile(url, URL_FILENAME);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
}