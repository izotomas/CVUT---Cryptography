using System;

namespace SSL
{
    public class Program
    {
        /*
         *    Napište program v jazyce C, který:
         * 
         *        * stáhne stránku předmětu https://fit.cvut.cz/student/odkazy do souboru
         *        * vypíše informace o certifikátu serveru fit.cvut.cz
         *        * uloží ho do souboru ve formátu PEM
         *        * Soubor PEM zobrazte v textové podobě pomocí utility openssl x509
         */
            
        public static void Main(string[] args)
        {
            // download BI-BEZ url
            const string url = "https://fit.cvut.cz/zajemce/bakalar/pocitacove-inzenyrstvi?q=predmety/bi-bez";
            SSL.GetURL(url);
            
            // get cert object and print fit.cvut.cz certificate info
            var res = SSL.GetCertificate();
            Console.WriteLine(res);
            
            // write cert object to file
            SSL.WritePEMCertificate(res);
        }
    }
}