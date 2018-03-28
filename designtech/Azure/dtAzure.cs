using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace dtAzure
{
    class Storage
    {
        internal Storage()
        {

        }

        #region GetAccount
        /// <summary>
        /// Returns the cloud storage account on Azure based on the account name and account key supplied.
        /// <param name="accountName">The account name</param>
        /// <param name="accountKey">The account key</param>
        /// <returns name="storageAccount">Returns the cloud storage account</returns>
        /// <search>microsoft,azure,cloud,storage,link,account,name,key</search>
        public static CloudStorageAccount GetAccount(string accountName, string accountKey)
        {
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(creds, true);
            return storageAccount;
        }
        #endregion

        #region GetContainer
        /// <summary>
        /// Returns a reference to the CloudBlobContainer object in the given account. 
        /// <param name="storageAccount">The account</param>
        /// <param name="containerName">The name of the container</param>
        /// <returns name="container">Returns the CloudBlobContainer</returns>
        /// <search>microsoft,azure,cloud,storage,link,account,cloud,blob,container</search>
        public static CloudBlobContainer GetContainer(CloudStorageAccount storageAccount, string containerName)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
            return blobContainer;
        }
        #endregion

        #region ListBlobs
        /// <summary>
        /// Returns a reference to the CloudBlobContainer object in the given account. 
        /// <param name="container">The account</param>
        /// <returns name="blobs">Returns the CloudBlobContainer</returns>
        /// <search>microsoft,azure,cloud,storage,link,account,cloud,blob,container</search>
        public static List<string> ListBlobs(CloudBlobContainer container)
        {
            IEnumerable<IListBlobItem> blobs = container.ListBlobs();
            List<string> blobList = new List<string>();
            foreach (var item in blobs)
            {
                blobList.Add(item.ToString());
            }
            return blobList;
        }
        #endregion

        #region ListContainers
        /// <summary>
        /// Returns a reference to the CloudBlobContainer object in the given account. 
        /// <param name="storageAccount">The account</param>
        /// <returns name="container">Returns the CloudBlobContainer</returns>
        /// <search>microsoft,azure,cloud,storage,link,account,cloud,blob,container</search>
        public static List<string> ListContainers(CloudStorageAccount storageAccount)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            var containers = blobClient.ListContainers();
            List<string> containerList = new List<string>();
            foreach (var item in containers)
            {
                containerList.Add(item.Name);
            }
            return containerList;
        }
        #endregion
    }
}
