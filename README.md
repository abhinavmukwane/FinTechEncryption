# 🔐 FintechEncryption

🚀 **Enterprise Hybrid Encryption Library for Secure FinTech Application/Api**

`FintechEncryption` is a lightweight .NET library that provides **secure data encryption and decryption using Hybrid Cryptography**.

It combines:

* 🔑 **AES-256-GCM** for fast symmetric encryption
* 🔐 **RSA-2048 OAEP** for secure key exchange
* ✍️ **SHA256 Digital Signature** for message integrity

This approach is widely used in **banking systems, payment gateways, and fintech APIs**.

---

# ✨ Features

✔ Hybrid Encryption (AES + RSA)
✔ AES-256-GCM authenticated encryption
✔ RSA-2048 OAEP key exchange
✔ SHA256 Digital Signature verification
✔ Secure random key generation
✔ Lightweight and dependency minimal
✔ Multi-framework support
✔ Easy integration into existing APIs

---

# 📦 Supported Frameworks

This library supports multiple .NET frameworks:

| Framework            | Supported 	|
| -------------------- | --------  	|
| .NET Framework 4.8   | ✅			|
| .NET Standard 2.0    | ✅       	|
| .NET 6 / 7 / 8       | ✅       	|
| ASP.NET MVC          | ✅       	|
| ASP.NET Core         | ✅       	|
| Console Applications | ✅       	|

---

# 📥 Installation

Install via **NuGet Package Manager**

```
Install-Package FintechEncryption
```

or using **.NET CLI**

```
dotnet add package FintechEncryption
```

---

# 🔑 Hybrid Encryption Architecture
# 🔐 Security Design

The library follows **industry-standard cryptographic practices**:

| Component         | Algorithm     |
| ----------------- | ------------- |
| Data Encryption   | AES-256-GCM   |
| Key Exchange      | RSA-2048 OAEP |
| Signature         | SHA256withRSA |
| Random Generation | SecureRandom  |


---

# 🔁 Secure Communication Use Case

A common real-world scenario is **secure communication between two systems**, such as:

* 🏦 Bank ↔ Payment Gateway
* 🏢 FinTech Platform ↔ Partner API
* 🛒 Merchant System ↔ Payment Processor

Each system generates its **own RSA key pair**, then **exchanges public keys** to enable secure encrypted communication.

---

## 🔑 Step 1: Generate Key Pairs

Each system generates its own RSA key pair.

### System A (Sender)

```csharp
var systemAKeys = KeyGenerator.GenerateRSAKeys();

string systemAPublicKey = systemAKeys.PublicKey;
string systemAPrivateKey = systemAKeys.PrivateKey;
```

### System B (Receiver)

```csharp
var systemBKeys = KeyGenerator.GenerateRSAKeys();

string systemBPublicKey = systemBKeys.PublicKey;
string systemBPrivateKey = systemBKeys.PrivateKey;
```

---

## 🔄 Step 2: Exchange Public Keys

Both systems securely exchange **public keys**.

| System   | Shares     | Keeps Secret |
| -------- | ---------- | ------------ |
| System A | Public Key | Private Key  |
| System B | Public Key | Private Key  |

Example:

```id="keyexchange"
System A → sends PublicKeyA to System B  
System B → sends PublicKeyB to System A
```

Private keys **must never be shared**.

---

## 🔒 Step 3: System A Encrypts Data

System A encrypts the payload using:

* its **private key** (for signing)
* System B's **public key** (for encrypting AES key)

```csharp
var encryptionService = new EncryptionService();

string encryptedPayload = encryptionService.EncryptData(
    plainTextData,
    systemAPrivateKey,
    systemBPublicKey
);
```

Encrypted data can now be sent over:

* REST APIs
* Message queues
* Webhooks
* Secure channels

---

## 🔓 Step 4: System B Decrypts Data

System B decrypts the message using:

* its **private key** (to decrypt AES key)
* System A's **public key** (to verify signature)

```csharp
var decryptionService = new DecryptionService();

string decryptedPayload = decryptionService.DecryptData(
    encryptedData,
    systemBPrivateKey,
    systemAPublicKey
);
```

System B can now safely process the data.

---

# 🔐 Security Guarantees

This hybrid encryption model provides:

| Security Property      | Description                                 |
| ---------------------- | ------------------------------------------- |
| 🔒 Confidentiality     | Only the receiver can decrypt the message   |
| ✍️ Integrity           | Message cannot be altered without detection |
| 👤 Authentication      | Confirms the sender's identity              |
| 🔑 Secure Key Exchange | AES key protected by RSA                    |

---

# 📡 Communication Flow

```id="flowdiagram"
System A                           System B
---------                         ---------

Generate KeyPair A                Generate KeyPair B

PublicKeyA  -------------------->  PublicKeyA received
PublicKeyB  <--------------------  PublicKeyB shared

Encrypt using:
PrivateKeyA + PublicKeyB

Encrypted Payload  ------------->  Receive Payload

                                   Decrypt using:
                                   PrivateKeyB + PublicKeyA
```

---

The payload is encrypted before transmission to ensure **high-level API security**.

---

## ⚠️ Exception Handling

`FinTechEncryption` uses a custom exception **`EncryptionException`** for all encryption and decryption related errors.  
Applications using the library should catch this exception to safely handle failures.

### Example Usage

```csharp
using FinTechEncryption;

try
{
    var encryptionService = new EncryptionService();

    string encrypted = encryptionService.EncryptData(
        jsonPayload,
        senderPrivateKey,
        receiverPublicKey
    );

    var decryptionService = new DecryptionService();

    string decrypted = decryptionService.DecryptData(
        encrypted,
        receiverPrivateKey,
        senderPublicKey
    );

    Console.WriteLine("Decrypted Data: " + decrypted);
}
catch (EncryptionException ex)
{
    Console.WriteLine("Encryption operation failed: " + ex.Message);
}
```

---

# 🚀 When To Use This Library

Use `FintechEncryption` when building:

* FinTech APIs
* Payment gateway integrations
* Secure partner APIs
* Banking systems
* Sensitive data exchange services
* Enterprise API platforms

---


# 🤝 Contributing

Contributions are welcome!

Steps:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Open a Pull Request

---

# 📜 License

This project is licensed under the **MIT License**.

---

# ⭐ Support

If you find this project useful:

⭐ Star the repository
🐛 Report issues
💡 Suggest improvements

---

# 🔗 Keywords

```
dotnet encryption
hybrid encryption
aes rsa encryption
fintech security
secure api encryption
api payload encryption
```

---




