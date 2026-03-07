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
    public class EncryptionService
    {
        public string EncryptData(string plainText, string senderPrivateKey, string receiverPublicKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plainText))
                    throw new EncryptionException("Plain text cannot be null or empty.");

                if (string.IsNullOrWhiteSpace(senderPrivateKey))
                    throw new EncryptionException("Sender private key is required.");

                if (string.IsNullOrWhiteSpace(receiverPublicKey))
                    throw new EncryptionException("Receiver public key is required.");

                string aesKey = Guid.NewGuid().ToString("N").Substring(0, 32);
                byte[] keyBytes = Encoding.UTF8.GetBytes(aesKey);
                byte[] iv = keyBytes.Take(12).ToArray();

                string encryptedBody = Convert.ToBase64String(EncryptAESGCM(plainText, keyBytes, iv));

                string signature = SignData(encryptedBody, senderPrivateKey);

                string encryptedHeader = EncryptRSA(aesKey, receiverPublicKey);

                string combined = $"{encryptedHeader}:{encryptedBody}:{signature}";
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(combined));
            }
            catch (EncryptionException)
            {
                throw;
            }
            catch (FormatException ex)
            {
                throw new EncryptionException("Invalid key format provided.", ex);
            }
            catch (Exception ex)
            {
                throw new EncryptionException("Encryption failed due to an unexpected error.", ex);
            }
        }

        private byte[] EncryptAESGCM(string data, byte[] key, byte[] iv)
        {
            try
            {
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters = new AeadParameters(new KeyParameter(key), 128, iv);
                cipher.Init(true, parameters);

                byte[] input = Encoding.UTF8.GetBytes(data);
                byte[] output = new byte[cipher.GetOutputSize(input.Length)];

                int len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
                cipher.DoFinal(output, len);

                return output;
            }
            catch (Exception ex)
            {
                throw new EncryptionException("AES encryption failed.", ex);
            }
        }

        private string SignData(string data, string privateKeyBase64)
        {
            var keyInfo = PrivateKeyInfo.GetInstance(Convert.FromBase64String(privateKeyBase64));
            var privateKey = PrivateKeyFactory.CreateKey(keyInfo);

            var signer = SignerUtilities.GetSigner("SHA256withRSA");
            signer.Init(true, privateKey);

            var bytes = Encoding.UTF8.GetBytes(data);
            signer.BlockUpdate(bytes, 0, bytes.Length);

            return Convert.ToBase64String(signer.GenerateSignature());
        }

        private string EncryptRSA(string text, string publicKeyBase64)
        {
            try
            {
                var publicKey = PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKeyBase64));

                var engine = new OaepEncoding(new RsaEngine());
                engine.Init(true, publicKey);

                byte[] input = Encoding.UTF8.GetBytes(text);

                return Convert.ToBase64String(engine.ProcessBlock(input, 0, input.Length));
            }
            catch (Exception ex)
            {
                throw new EncryptionException("RSA encryption failed.", ex);
            }
        }
    }
}
