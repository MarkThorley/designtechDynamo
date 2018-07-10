using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace dtBlockchain
{
    public class Block
    {
        internal Block()
        {

        }

        //Fields
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
        public SHA256 ThisHash { get; set; }
        public SHA256 PreviousHash { get; set; }

        #region Create
        /// <summary>
        /// Creates a block for the block chain
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="timestamp">timestamp</param>
        /// <param name="data">data</param>
        /// <param name="hash">hash</param>
        /// <param name="previousHash">previousHash</param>
        /// <param name="block">output</param>
        /// <search>
        /// create, block, hash, sha256, blockchain
        /// </search>
        public static Block Create(int index, DateTime timestamp, string data, SHA256 hash, SHA256 previousHash)
        {
            Block block = new Block();

            block.Index = index;
            block.Timestamp = timestamp;
            block.Data = data;
            block.ThisHash = hash;
            block.PreviousHash = previousHash;

            return block;
        }
        #endregion

        #region CreateGenesisBlock
        /// <summary>
        /// Creates the initial genesis block used in a blockchain
        /// </summary>
        /// <param name="block">output</param>
        /// <search>
        /// create, block, hash, sha256, blockchain, genesis, start
        /// </search>
        public static Block CreateGenesisBlock()
        {
            int index = 0;
            DateTime timestamp = DateTime.Now;
            string data = "Genesis Block";
            string str = index.ToString() + timestamp.ToString() + data.ToString();
            SHA256 hash = SHA256.Create();
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(str)))
            {
                hash.ComputeHash(ms);
            }
            SHA256 previousHash = SHA256.Create();
            previousHash.Clear();
            Block blk = dtBlockchain.Block.Create(index, timestamp, data, hash, previousHash);
            return blk;
        }
        #endregion

    }



    public class Blockchain
    {

        internal Blockchain()
        {

        }

        #region Create
        /// <summary>
        /// Creates the initial genesis block used in a blockchain
        /// </summary>
        /// <param name="count">count</param>
        /// <search>
        /// blockchain, create
        /// </search>
        public static List<Block> Create(int count)
        {
            List<Block> blockchain = new List<Block>();
            Block genesisBlock = dtBlockchain.Block.CreateGenesisBlock();

            blockchain.Add(genesisBlock);

            for (int i = 0; i < count - 1; i++)
            {
                Block nextBlock = dtBlockchain.Blockchain.NextBlock(blockchain[i]);
                blockchain.Add(nextBlock);
            }
            return blockchain;

        }
        #endregion

        #region NextBlock
        /// <summary>
        /// Creates the next block in the blockchain
        /// </summary>
        /// <param name="lastBlock">index</param>
        /// <param name="nextBlock">output</param>
        /// <search>
        /// create, block, hash, sha256, blockchain, next
        /// </search>
        public static Block NextBlock(Block lastBlock)
        {
            int thisIndex = lastBlock.Index + 1;
            DateTime thisTimestamp = DateTime.Now;
            string thisData = "block" + thisIndex.ToString() + "data";
            string hashString = thisIndex.ToString() + thisTimestamp.ToString() + thisData.ToString();
            SHA256 thisHash = SHA256.Create();
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(hashString)))
            {
                thisHash.ComputeHash(ms);
            }
            SHA256 thisPreviousHash = lastBlock.ThisHash;
            var blk = dtBlockchain.Block.Create(thisIndex, thisTimestamp, thisData, thisHash, thisPreviousHash);
            return blk;
        }
        #endregion

    }

    public class Hash
    {

        internal Hash()
        {

        }

        #region Data
        /// <summary>
        /// Retrieves the hash data from the hash class
        /// </summary>
        /// <param name="hash">index</param>
        /// <param name="str">output</param>
        /// <search>
        /// data, hash, sha256, block, blockchain
        /// </search>
        public static String Data(SHA256 hash)
        {
            try
            {
                byte[] byt = hash.Hash;
                string hashString = string.Empty;
                foreach (byte x in byt)
                {
                    hashString += String.Format("{0:x2}", x);
                }
                return hashString;
            }

            catch (Exception)
            {
                return "no data";
                throw;
            }
        }
        #endregion

        #region Size
        /// <summary>
        /// Retrieves the size of the hash
        /// </summary>
        /// <param name="hash">index</param>
        /// <param name="int">output</param>
        /// <search>
        /// size, length, hash, sha256, block, blockchain
        /// </search>
        public static int Size(SHA256 hash)
        {
            try
            {
                int i = hash.HashSize;
                return i;
            }

            catch (Exception)
            {
                return 0;
                throw;
            }

        }
        #endregion

    }
}


