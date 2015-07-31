using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Crypto
{
    class EncryptionManager
    {
        private byte[] publicKey;

        private static readonly int blockBitSize = 128;
        private static readonly int keyBitSize = 256;

        private static readonly int SaltBitSize = 64;
        private static readonly int Iterations = 10000;
        private static readonly int MinPasswordLength = 12;

        private static readonly RandomNumberGenerator randomNG = RandomNumberGenerator.Create();

        /// <summary>
        /// Prepare the salt value for use in either decrypting or encrypting of data
        /// using AES encryption can be used for a string or file and decypted again
        /// using the methods within this class.
        /// </summary>
        /// <param name="pKey"></param>
        public void Crypto(string pKey)
        {
            // Set the Salt value for this object
            if (!string.IsNullOrEmpty(pKey) || !string.IsNullOrWhiteSpace(pKey))
            {
                // Public key contains a valid value to use for encryption
                publicKey = Encoding.ASCII.GetBytes(pKey);
            }
            else
            {
                // Public key is not a value and can't be used
                throw new ArgumentNullException("Invalid public key value.");
            }
        }

        /// <summary>
        /// Helper class that returns a random key each time it is called
        /// </summary>
        /// <returns>byte array</returns>
        private static byte[] NewKey()
        {
            var key = new byte[keyBitSize / 8];
            randomNG.GetBytes(key);
            return key;
        }

        /// <summary>
        /// Simple AES encryption then authentication for UTF8 message.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cryptKey"></param>
        /// <param name="authKey"></param>
        /// <param name="publicKey"></param>
        /// <returns>Encrypted string message</returns>
        public static string EncryptValue(string value, byte[] cryptKey, byte[] authKey, byte[] publicKey = null)
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("Nothing to Encrypt! - You must provide a value.");
            }

            var plainText = Encoding.UTF8.GetBytes(value);
            var cipherText = EncryptValue(plainText, cryptKey, authKey, publicKey);

            return Convert.ToBase64String(cipherText);
        }

        /// <summary>
        /// Main Encryption process, does validation checks on all aspects then 
        /// performs the encryption of the passed in value, finally authenticates
        /// the encryption value and adds that to the final product
        /// </summary>
        /// <param name="value">The message/value to encrypt</param>
        /// <param name="cryptKey">The Encryption key</param>
        /// <param name="authKey">The authentication key</param>
        /// <param name="publicKey">optional public key</param>
        /// <returns>Byte arrray</returns>
        private static byte[] EncryptValue(byte[] value, byte[] cryptKey, byte[] authKey, byte[] publicKey = null)
        {
            // Check the cryptKey is valid and usable
            if(cryptKey == null || cryptKey.Length != keyBitSize / 8)
            {
                throw new ArgumentException(string.Format("Key needs to be {0} bit.", keyBitSize), "Crypt Key");
            }

            // Check the authKey is valid and usable
            if(authKey == null || authKey.Length != keyBitSize / 8)
            {
                throw new ArgumentException(string.Format("Key needs to be {0} bit.", keyBitSize), "Auth Key");
            }

            // Check there is something to encrypt
            if(value == null || value.Length < 1)
            {
                throw new ArgumentException(string.Format("There is nothing to encrypt, please supply a message/value", "Value"));
            }

            // Check the publicKey value exists if not set it up as an empty byte array as it's optional anyway
            publicKey = publicKey ?? new byte[] { };

            byte[] cipherText;
            byte[] iv;

            using (var aes = new AesManaged
            {
                KeySize = keyBitSize,
                BlockSize = blockBitSize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                // Use random IV
                aes.GenerateIV();
                iv = aes.IV;

                using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
                using (var cipherStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cryptoStream))
                    {
                        // Encrypt data
                        binaryWriter.Write(value);
                    }

                    cipherText = cipherStream.ToArray();
                }
            }

            // Assemble encrypted message and add authentication to it
            using (var hmac = new HMACSHA256(authKey))
            using (var encryptedStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(encryptedStream))
                {
                    // Pre-pend public key (if any)
                    binaryWriter.Write(publicKey);

                    // Pre-pend IV
                    binaryWriter.Write(iv);

                    // Write cipherText
                    binaryWriter.Write(cipherText);
                    binaryWriter.Flush();

                    // Authenticate all data
                    var authed = hmac.ComputeHash(encryptedStream.ToArray());

                    // Post-pend authentication
                    binaryWriter.Write(authed);
                }

                return encryptedStream.ToArray();
            }
        }
    }
}