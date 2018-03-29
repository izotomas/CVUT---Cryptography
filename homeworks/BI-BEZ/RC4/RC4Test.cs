using System.Collections.Generic;
using NUnit.Framework;

namespace RC4
{
     /*
     * (1) Prozkoumejte vliv inicializačního vektoru na šifrový text. Zdůvodněte zjištění. 
     * (2) Modifikujte program tak, aby zašifroval dvě různé zprávy.
     *      Zvolte si tajný klíč (např. 16 znakový řetězec)
     *      První zpráva bude tajná, 30 znaků dlouhá, její text neprozrazujte
     *      Druhá zpráva bude veřejně známá (prozrazená nebo vynucená) „abcdefghijklmnopqrstuvwxyz0123“
     *      Šifrové texty obou zpráv (v hexadecimální podobě) pošlete svému kolegovi elektronickou poštou,
     *          Váš kolega by měl poslat svoji dvojici zpráv Vám. Nezapomeňte ŠT označit, aby bylo zřejmé,
     *          který odpovídá veřejně známé zprávě.
     *      Víte následující skutečnosti:
     *              - Váš kolega byl neopatrný a zašifroval dvě různé zprávy stejným klíčem.
     *              - Navíc u jedné ze zpráv znáte otevřený text.
     *              - Vaším úkolem je vyluštit i druhou z obou zpráv. (Postup je triviální.)
     *
     * Váš program musí přijímat hexadecimální podobu šifrových textů obou zpráv na svém standardním vstupu.
     * Vyluštěnou zprávu vypíše v textové podobě.
     *
     * Příklad šifrových textů dvou zpráv v hexadecimální podobě, které byly zašifrovány stejným klíčem,
     * z nichž jedna je zašifrovaná známá zpráva „abcdefghijklmnopqrstuvwxyz0123“, a druhou máte vyluštit:
     *      06fb7405eba8d9e94fb1f28f0dd21fdec55fd54750ee84d95ecccf2b1b48
     *      33f6630eaea4dba152baf38d019c04cbc759c94544fb9a815dc68d7b5f1a
     */
    [TestFixture]
    public class RC4Test
    {
        [TestCase("poit9283nv", "40982jfvnme", "AHOJ")]
        public void InfluenceOfIVTest(string key, string iv, string plaintext)
        {
            // IV acts like randomizator, i.e. if it is used, encrypting two PTs with same key will return two different CTs
            // following test might fail at some times, but with reasonably low probability (due to IV randomness)
            for (var i = 0; i < 100; i++)
            {
                var ciphers = new Dictionary<string, IRC4>
                {
                    ["NoIV1"] = new RC4(key),
                    ["NoIV2"] = new RC4(key),
                    ["IV1"] = new RC4(key, enableIV: true),
                    ["IV2"] = new RC4(key, enableIV: true),
                    ["ConstIV1"] = new RC4(key, iv),
                    ["ConstIV2"] = new RC4(key, iv)
                };

                var cipherTexts = new Dictionary<string, string>
                {
                    ["NoIV1"] = ciphers["NoIV1"].EncryptToStr(plaintext),
                    ["NoIV2"] = ciphers["NoIV2"].EncryptToStr(plaintext),
                    ["IV1"] = ciphers["IV1"].EncryptToStr(plaintext),
                    ["IV2"] = ciphers["IV2"].EncryptToStr(plaintext),
                    ["ConstIV1"] = ciphers["ConstIV1"].EncryptToStr(plaintext),
                    ["ConstIV2"] = ciphers["ConstIV2"].EncryptToStr(plaintext),
                };

                
                Assert.AreEqual(cipherTexts["NoIV1"], cipherTexts["NoIV2"]);
                // Using the same IV twice on same PT with same key => not a good idea
                Assert.AreEqual(cipherTexts["ConstIV1"], cipherTexts["ConstIV2"]);
                Assert.AreNotEqual(cipherTexts["IV1"], cipherTexts["IV2"]);
                Assert.AreNotEqual(cipherTexts["IV1"], cipherTexts["NoIV1"]);
                Assert.AreNotEqual(cipherTexts["IV1"], cipherTexts["NoIV2"]);
                Assert.AreNotEqual(cipherTexts["NoIV1"], cipherTexts["ConstIV2"]);
            }
        }

        [TestCase("flkjlnae", "lnlfnlnwj1'.aj")]
        [TestCase("NFAf;k", "lmmflnapejp3nlvnlkjlfjsasmflmen")]
        public void EncryptionDecryptionTest(string key, string message)
        {
            var encrypter = (IRC4) new RC4(key: key);
            var decrypter = (IRC4) new RC4(key: key);

            var ct = encrypter.EncryptToStr(message);
            var pt = decrypter.DecryptToStr(ct);
            Assert.AreEqual(message, pt);
        }
    }
}
