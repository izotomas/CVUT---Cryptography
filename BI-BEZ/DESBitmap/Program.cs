// Autor: Izo Tomas
using System;
using CommandLine;
using DESBitmap.Options;

namespace DESBitmap
{
    /*
     * Vlastnosti operačního módu demonstrujte pomocí zašifrování obrazových dat (datové části obrázku ve formátu BMP)
     *     * Stáhněte si např. tento obrázek ve formátu BMP (rozbalte zip): obrazek.zip. (homer-simpson.zip)
     *     * Napište program, který zkopíruje hlavičku a zašifruje část souboru s obrazovými daty pomocí DES v módu ECB.
     *         Výstupní soubor se bude jmenovat (původní_jméno)_ecb.bmp
     *     * Napište program, který dešifruje obrázek zašifrovaný prvním programem.
     *         Výstupní soubor se bude jmenovat (původní_jméno)_dec.bmp. Porovnejte dešifrovaný soubor s původním.
     *     * Porovnejte původní obrázek a jeho zašifrovanou podobu a vysvětlete svá zjištění
     *     * Změňte pro šifrování i dešifrování použitý operační mód na CBC a vytvořte (původní_jméno)_cbc.bmp
     *         a (původní_jméno)_cbc_dec.bmp(upřesní cvičící)
     *     * Porovnejte původní obrázek a jeho zašifrovanou podobu a vysvětlete svá zjištění
     *     * Na první řádek zdrojáku dejte komentář se jménem autora!
     */
    
    
    public class Program
    {

        public static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<EncryptOptions, DecryptOptions>(args)
                .MapResult(
                    (EncryptOptions opts) =>
                    {
                        Console.WriteLine("Encryption\ninput => {0}\nkey => {1}\nmode => {2}", 
                            opts.InputFile, opts.Key, opts.Mode);
                        return 0;
                    },
                    (DecryptOptions opts) =>
                    {
                        Console.WriteLine("decrypt options");
                        return 0;
                    },
                    errs => 1);
        }
    }
}