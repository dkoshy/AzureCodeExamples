using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.ComponentModel;

namespace App.BlobExamples
{
    public class CloudBlobBasicOperations
    {
        private readonly string _Connectionstring; 
        private BlobServiceClient _client;
        public string ContainerName { get; set; }
        public string  BlobName { get; set; }

        public CloudBlobBasicOperations()
        {
            _Connectionstring = @"DefaultEndpointsProtocol=https;AccountName=azstoragedk104;AccountKey=HKD7M9gEdDxVn3LaR/J6jvSGN7nDpam3/+JywJ1KHMkPudVYMRZombvROe/lLx0Ngq2GlLJjZlzm+AStW5/sDg==;EndpointSuffix=core.windows.net";
            _client = new BlobServiceClient(_Connectionstring);
        }

        public async Task CheckBlobExistist()
        {
            ContainerName =  "sampledata";
            BlobName=@"sample.txt";

            var blobContainerClient = _client.GetBlobContainerClient(ContainerName);
            var blobClient = blobContainerClient.GetBlobClient(BlobName);

            var exsists = await blobClient.ExistsAsync();

            if(exsists)
            {
                Console.WriteLine($"blob {BlobName} Exists");
            }
            else
            {
                Console.WriteLine("Blob is missing ");
            }

        }

        public async Task DownloadBlobTolocalPath()
        {
            var id = Guid.NewGuid();

            ContainerName =  "sampledata";
            BlobName=@"sample.txt";
           
            var blobClient = new BlobClient(_Connectionstring, ContainerName, BlobName);
            var path = Path.Combine("blobDownloads", $"sample-{id}.csv");
            await blobClient.DownloadToAsync(path);
        }

        public async Task DownloadBlobToStream()
        {
            var id = Guid.NewGuid();

            ContainerName =  "sampledata";
            BlobName=@"sample.txt";

            var blobClient = new BlobClient(_Connectionstring, ContainerName, BlobName);
            var path = Path.Combine("blobDownloads", $"Stream-{id}.csv");
            using  FileStream stream = new FileStream(path, FileMode.Create);

            await blobClient.DownloadToAsync(stream);
        }


        public async Task ReadFromBlobStream()
        {
            ContainerName =  "sampledata";
            BlobName=@"sample.txt";
            var blobclient = new BlobClient(_Connectionstring, ContainerName, BlobName);

             var stream = await  blobclient.OpenReadAsync();
            using StreamReader reader = new StreamReader(stream);
            var   data = reader.ReadToEnd();
            Console.WriteLine(data);
        }

        public async Task ListBlobs()
        {
            ContainerName =  "sampledata";
            var containerClient = new BlobContainerClient(_Connectionstring, ContainerName);
            var blobsData =  containerClient.GetBlobsAsync();

            await foreach(BlobItem blobData in  blobsData)
            {
                Console.WriteLine($"File Name : - {blobData.Name}");
               
            }
        }

        public async Task DownloadFromStream()
        {
            ContainerName =  "sampledata";
            BlobName=@"sample.txt";
            var localfile = "sampledata.txt";
            var path = Path.Combine("blobDownloads", localfile);
            var blobClient = new BlobClient(_Connectionstring, ContainerName, BlobName);

            if(await blobClient.ExistsAsync())
            {
                using var blobstream = await blobClient.OpenReadAsync();
                using FileStream filestream = File.OpenWrite(path);
                await blobstream.CopyToAsync(filestream);
            }
        }

        public async Task DownloadToText()
        {
            ContainerName =  "sampledata";
            BlobName=@"sample.txt";
            var localfile = "sampledata.txt";
            var path = Path.Combine("blobDownloads", localfile);
            var blobClient = new BlobClient(_Connectionstring, ContainerName, BlobName);

            var downloadResult = await  blobClient.DownloadContentAsync();
            string bolobcontent =  downloadResult.Value.Content.ToString();
            Console.WriteLine(bolobcontent);
        }

        public async Task ListContaners()
        {
            var containers =  _client.GetBlobContainersAsync();

            await foreach(var container in containers)
            {
                Console.WriteLine($"Name -> {container.Name}");
                Console.WriteLine($"Public Access -> {container.Properties.PublicAccess}");
                Console.WriteLine($"Version Id -> {container.VersionId}");
            }
        }

        public async Task UploadBlobAsync()
        {
            ContainerName =  "sampledata";
            var localFilePath = Path.Combine("blobDownloads", "samplepic.jpg");

            var blobclient = new BlobClient(_Connectionstring, ContainerName, Path.GetFileName(localFilePath));

            var blobcontentinfo = await blobclient.UploadAsync(localFilePath,true);
            Console.WriteLine($"Uploaded blob information is given below");
            Console.WriteLine($"{blobcontentinfo.Value.BlobSequenceNumber}");
        }

        public async Task uploadBlobWithStream()
        {
            ContainerName =  "sampledata";
            
            var localFilePath = Path.Combine("blobDownloads", "samplepic.jpg");
            var blobclient = new BlobClient(_Connectionstring, ContainerName, $"{Guid.NewGuid()}.jpg");
            var stream = File.OpenRead(localFilePath);

            var blobcontentinfo = await blobclient.UploadAsync(stream);
            Console.WriteLine($"Uploaded blob information is given below");
            Console.WriteLine($"{blobcontentinfo.Value.BlobSequenceNumber}");
        }

        public async Task DeleteBlobAsync()
        {
            ContainerName =  "sampledata";
            var blobclient = new BlobClient(_Connectionstring, ContainerName, "sample.txt");
            var isDeleted = await blobclient.DeleteIfExistsAsync(DeleteSnapshotsOption.None);
            Console.WriteLine($"Blob is deleted {isDeleted.Value}");
        }

        public async Task UploadandSetPropertiesOfBlob()
        {
            ContainerName =  "sampledata";
            var localFilePath = Path.Combine("blobDownloads", "samplepic.jpg");
            var blobClient = new BlobClient(_Connectionstring, ContainerName, $"{Guid.NewGuid()}.jpg");

            await blobClient.UploadAsync(localFilePath);

            //setting headers
            var blobheaders = (await blobClient.GetPropertiesAsync())?.Value;
           
            var newHeaders = new BlobHttpHeaders
            {
                ContentType = "image/jpeg",
                ContentLanguage = "en-us",
                //copy the default settings of the blob
                CacheControl = blobheaders.CacheControl,
                ContentDisposition = blobheaders.ContentDisposition,
                ContentEncoding = blobheaders.ContentEncoding,
                ContentHash = blobheaders.ContentHash,
            };

            await blobClient.SetHttpHeadersAsync(newHeaders);

            //set  metadata

            var bolmetainfo = new Dictionary<string, string>()
           {
               {"cetgory" , "SampleTest" },
               {"image_about" , "samplecode" }
           };

           await  blobClient.SetMetadataAsync(bolmetainfo);
          
        }
    }
}
