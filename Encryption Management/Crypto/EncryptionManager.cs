using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto
{
    class EncryptionManager
    {
        private byte[] saltValue = null;

        /// <summary>
        /// Prepare the salt value for use in either decrypting or encrypting of data
        /// using AES encryption can be used for a string or file and decypted again
        /// using the methods within this class.
        /// </summary>
        /// <param name="salt"></param>
        public void Crypto(string salt)
        {
            // Set the Salt value for this object
            if (!String.IsNullOrEmpty(salt) || !string.IsNullOrWhiteSpace(salt))
            {
                // Salt contains a valid value to use for encryption
                saltValue = Encoding.ASCII.GetBytes(salt);
            }
            else
            {
                // Salt is not a value and can't be used
                throw new ArgumentNullException("Invalid salt value");
            }
        }

        /// <summary>
        /// Encrypts a string value passed in to it returning the 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string EncryptValue(string value)
        {
            return "To Do!";
        }
    }
}