using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; 
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;

namespace ConsumeFileShare
{
    class Program
    {
        #region initialization
        /// <summary>
        /// Initialize class variables
        /// _cloudDirRef is the directory that has the file to download
        /// _referenceFile is the file to download
        /// _fileShareName is the name of file share service
        /// _connectionString is define in App.config 
        /// </summary>
        private static readonly string _referenceDirectory = "NameOftTheDirectoryInTheCloud";
        private static readonly string _referenceFile = "NameOftTheFileInTheCloud";
        private static readonly string _localPath = "localPath";
        private static readonly string _fileShareName = "fileShareName";
        private static readonly string _connectionString = "StorageConnectionString";
        #endregion

        public static void SaveFile(string connectionString, string fileShareName, string directoryName, string fileName, string referencePath )
        {
            try
            {
                CloudFileShare share = GetReference(connectionString, fileShareName);

                CloudFileShare fileShareRef = CheckFileShareExistance(share);

                CloudFileDirectory cloudDirRef = CheckDirectoryExistance(fileShareRef, directoryName);

                CloudFile cloudFileRef = CheckFileExistance(cloudDirRef, referencePath, fileName);

                SaveFileFromFileShare(cloudFileRef, referencePath, directoryName, fileName);

            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Opps! Something went wrong.
Error: {ex.Message}");
            }    
        }

        private static CloudFileShare GetReference(string connectionString,string fileShareName)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(connectionString));

            // Create a CloudFileClient object for credentialed access to Azure Files.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            return fileClient.GetShareReference(fileShareName);  
        }

        private static CloudFileShare CheckFileShareExistance(CloudFileShare share)
        {      
                // Ensure that the share exists. 
            return share.Exists()? share: throw new Exception("FileShare not exist");
        }

        public static CloudFileDirectory CheckDirectoryExistance(CloudFileShare share,string referenceDirectory) {

            // Get a reference to the root directory for the share.
            CloudFileDirectory rootDir = share.GetRootDirectoryReference();

            // Get a reference to the directory we created previously.
            CloudFileDirectory sampleDir = rootDir.GetDirectoryReference(referenceDirectory);

            // Ensure that the directory exists.
            return sampleDir.Exists() ? sampleDir : throw new Exception("Directory not exist");
        }

        public static CloudFile CheckFileExistance(CloudFileDirectory cloudDirRef, string referencePath, string fileName)
        {       
          // Get a reference to the file we created previously.
            CloudFile cloudFile = cloudDirRef.GetFileReference(fileName);

            // Ensure that the file exists.
            if (!cloudFile.Exists()) throw new Exception("File not exist");
            
                var directory = new DirectoryInfo(referencePath);
                if (!directory.Exists) directory.Create();
                return cloudFile;
        }

        public static void SaveFileFromFileShare(CloudFile cloudFile, string localPath,string localDirectory,string localFile)
        {
            string path = Path.Combine(localPath, localFile);

            using (var outputFileStream = new FileStream(path, FileMode.Create))
            {
                cloudFile.DownloadToStream(outputFileStream);
                Console.WriteLine($@"File Has been saved at {localPath}
Directory: {localDirectory}
FileName: {localFile} ");
            }


        }

        public static void Main(string[] args)
        {
            SaveFile(_connectionString, _fileShareName, _referenceDirectory, _referenceFile, _localPath); 
        
        }
            
    }

}


    