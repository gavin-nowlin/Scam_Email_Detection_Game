using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System.IO;

public class RSAHandler
{
    private AsymmetricKeyParameter publicKey;

    // Load the public key from a PEM-formatted string
    public RSAHandler(string publicKeyPem)
    {
        using (StringReader reader = new StringReader(publicKeyPem))
        {
            PemReader pemReader = new PemReader(reader);
            publicKey = (AsymmetricKeyParameter)pemReader.ReadObject();
        }
    }

    // Encrypt a message using the public key
    public string Encrypt(string plainText)
    {
        IAsymmetricBlockCipher encryptEngine = new Pkcs1Encoding(new RsaEngine());
        encryptEngine.Init(true, publicKey); // 'true' for encryption

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptEngine.ProcessBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }
}
