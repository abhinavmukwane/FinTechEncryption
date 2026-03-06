using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinTechEncryption.Security
{
    public class KeyGenerator
    {
        public static (string PublicKey, string PrivateKey) GenerateRSAKeys()
        {
            var generator = new RsaKeyPairGenerator();

            generator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));

            AsymmetricCipherKeyPair keyPair = generator.GenerateKeyPair();

            var privateKeyInfo =
                PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);

            var publicKeyInfo =
                SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);

            string privateKey = Convert.ToBase64String(privateKeyInfo.GetEncoded());

            string publicKey = Convert.ToBase64String(publicKeyInfo.GetEncoded());

            return (publicKey, privateKey);
        }
    }
}
