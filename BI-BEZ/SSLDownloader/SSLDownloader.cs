using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SSLDownloader
{
    public static class SSLDownloader
    {
        public const string SERVER_NAME = "http://fit.cvut.cz";

        public static void Download(string serverName = SERVER_NAME)
        {
            //Do webrequest to get info on secure site
            var request = (HttpWebRequest)WebRequest.Create(serverName);
            var response = (HttpWebResponse)request.GetResponse();
            response.Close();

            //retrieve the ssl cert and assign it to an X509Certificate object
            var cert = request.ServicePoint.Certificate;

            //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
            var cert2 = new X509Certificate2(cert);

            //write the X509Certificate to a file
            var certBytes = cert2.Export(X509ContentType.Cert);
            using (var fout = new FileStream("cert.pem", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fout.Write(certBytes, 0, certBytes.Length);
            }
        }
        
    }
}