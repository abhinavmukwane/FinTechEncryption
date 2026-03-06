using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinTechEncryption.Services
{
    public class DecryptionService
    {
        public string DecryptData(string encryptedText, string receiverPrivateKey, string senderPublicKey)
        {
            string combined = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedText));

            var parts = combined.Split(':');
            if (parts.Length != 3)
                throw new Exception("Invalid encrypted format");

            string encHeader = parts[0];
            string encBody = parts[1];
            string signature = parts[2];

            // 1. Decrypt AES key using receiver PRIVATE key
            string aesKey = DecryptRSA(encHeader, receiverPrivateKey);


            byte[] keyBytes = Encoding.UTF8.GetBytes(aesKey);
            byte[] iv = keyBytes.Take(12).ToArray();

            // 2. Verify signature using sender PUBLIC key
            if (!VerifySignature(encBody, signature, senderPublicKey))
                throw new Exception("Digital signature verification failed");

            // 3. AES GCM decrypt body
            byte[] decryptedBytes = DecryptAESGCM(Convert.FromBase64String(encBody), keyBytes, iv);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private byte[] DecryptAESGCM(byte[] cipherBytes, byte[] key, byte[] iv)
        {
            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(key), 128, iv);
            cipher.Init(false, parameters);

            byte[] output = new byte[cipher.GetOutputSize(cipherBytes.Length)];
            int len = cipher.ProcessBytes(cipherBytes, 0, cipherBytes.Length, output, 0);
            cipher.DoFinal(output, len);

            return output;
        }

        private string DecryptRSA(string encryptedBase64, string privateKeyBase64)
        {
            var keyInfo = PrivateKeyInfo.GetInstance(Convert.FromBase64String(privateKeyBase64));
            var privateKey = PrivateKeyFactory.CreateKey(keyInfo);

            var engine = new OaepEncoding(new RsaEngine());
            engine.Init(false, privateKey);

            byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);
            return Encoding.UTF8.GetString(engine.ProcessBlock(cipherBytes, 0, cipherBytes.Length));
        }

        private bool VerifySignature(string data, string signatureBase64, string publicKeyBase64)
        {
            var publicKey = PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKeyBase64));

            var verifier = SignerUtilities.GetSigner("SHA256withRSA");
            verifier.Init(false, publicKey);

            var bytes = Encoding.UTF8.GetBytes(data);
            verifier.BlockUpdate(bytes, 0, bytes.Length);

            return verifier.VerifySignature(Convert.FromBase64String(signatureBase64));
        }
    }
}
