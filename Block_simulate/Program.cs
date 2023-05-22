/*
 * 
 *  [ Blockchain Simulation ]
 * 
 *  This is a example of how mining works and how Data can be encrypted.
 *  Every Data-Block will be encrypted, we will generate a SHA-256 Hash out
 *  of every Block which is the Encryption-Key of the next Datablock.
 *  Since there is technically no first Hash, we will take some random, user-defined Hash.
 *  Please notice, there is a public-Key which represents a Masterpassword of every encrypted Block,
 *  so the Hash is not the only Key needed.
 * 
 * 
 * (c) 2023 by Artur Loewen 
 * 
 * This Code is licensed under the terms of the MIT license.
 * https://opensource.org/license/mit/
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Block_simulate
{
    internal class Program
    {
        private static readonly string PublicKey = "TESTPASS"; // Master-Password

        private static readonly string StartHash = "5d8fb897ac69d26acb4bba9ab58b2e6dd5cc08aa0d78c50bf34f6678dc1a8fc9";


        /// <summary>
        /// Our Data-Structure
        /// </summary>
        struct Dta
        {
            public string key;
            public string value;
        }

        /// <summary>
        /// Random Collection, just for Testing
        /// </summary>
        static List<Dta> DataCollection = new List<Dta>()
        {
            {new Dta { key = "Item0", value = "Value0"} },
            {new Dta { key = "Item1", value = "Value1"} },
            {new Dta { key = "Item2", value = "Value2"} }
        };

        static void Main(string[] args)
        {


            string currentHash = StartHash; // Current Hash, starting with our starting Hash since we have no previous Block at the Beginning
            string blocks = string.Empty; // 

            // Collection of Hashes
            List<string> HashCol = new List<string>();

            // pre-defined Variables for later Purposes
            BigInteger nonce = 0;
            string thash = string.Empty;
            string enc = string.Empty;

            // log the start time
            Console.WriteLine($"START... ({DateTime.Now.ToString("dd.MM.yyyy | HH:mm:ss")})");
            foreach (Dta itm in DataCollection)
            {

                // Nonce / variety by leading Zeros
                while (!thash.StartsWith("000"))
                {
                    // encrypting the Datablock using the last given Hash
                    enc = encodeString(currentHash, $"{itm.key};{itm.value};{nonce}");
                    // Temporary Hash to check the variety
                    thash = ComputeSha256Hash(enc);
                    // increasing the Nonce
                    nonce++;
                }

                // after finding the variety adding the Block to my List
                blocks += $"DATA: {enc}\n";
                // Collecting our Hashes ordered by our Data
                HashCol.Add(currentHash);

                // set current Hash to current temp. hash found by lookup
                currentHash = thash;

                // Reset everything
                thash = "";
                enc = "";
                nonce = 0;

            }
            // log the End Time 
            Console.WriteLine($"END : ({DateTime.Now.ToString("dd.MM.yyyy | HH:mm:ss")})");

            Console.WriteLine("<---------------HASHES----------------->");

            // Add the Last Generated Hash since our Loop ended before 
            HashCol.Add(currentHash);

            // Outputing every collected Hash
            HashCol.ForEach(x => Console.WriteLine($"HASH: {x}"));
            Console.WriteLine("<---------------END-HASH--------------->");

            Console.WriteLine("<-------------DATA-BLOCKS-------------->");
            // Outputing the collected Data
            Console.Write(blocks);
            Console.WriteLine("<-------------END-BLOCKS--------------->");


            // For testing purposes we can test every Block with given Hash by now
            while (true)
            {
                Console.Write("Enter the Crypted Chain : ");
                string ln = Console.ReadLine();
                Console.WriteLine();
                Console.Write("Enter the Hash Key : ");
                string kln = Console.ReadLine();
                Console.WriteLine($"-> {decodeString(kln, ln)}");

            }

        }


        /// <summary>
        /// SHA-256 
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Encrypting Function for AES-256
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string encodeString(string privateKey, string data)
        {
            string answer = "";
            byte[] privateKeyBytes = { };
            privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            byte[] publicKeyBytes = { };
            publicKeyBytes = Encoding.UTF8.GetBytes(PublicKey);
            byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(data);
            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                var memoryStream = new MemoryStream();
                var cryptoStream = new CryptoStream(memoryStream,
                provider.CreateEncryptor(publicKeyBytes, privateKeyBytes),
                CryptoStreamMode.Write);
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();
                answer = Convert.ToBase64String(memoryStream.ToArray());
            }
            return answer;
        }

        /// <summary>
        /// Decrypting Function for AES-256
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string decodeString(string privateKey, String data)
        {
            string answer = "";
            byte[] privateKeyBytes = { };
            privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            byte[] publicKeyBytes = { };
            publicKeyBytes = Encoding.UTF8.GetBytes(PublicKey);
            byte[] inputByteArray = new byte[data.Replace(" ", "+").Length];
            inputByteArray = Convert.FromBase64String(data.Replace(" ", "+"));
            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                var memoryStream = new MemoryStream();
                var cryptoStream = new CryptoStream(memoryStream,
                provider.CreateDecryptor(publicKeyBytes, privateKeyBytes),
                CryptoStreamMode.Write);
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();
                answer = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            return answer;
        }
    }
}
