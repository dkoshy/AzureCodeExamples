using Azure.Storage.Blobs;

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
            _Connectionstring = @"";
            _client = new BlobServiceClient(_Connectionstring);
        }

        public async Task CheckBlobExistist()
        {
            ContainerName =  "opstechtestbulkupload";
            BlobName=@"saple ss.csv";

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

            ContainerName =  "opstechtestbulkupload";
            BlobName=@"saple ss.csv";
           
            var blobClient = new BlobClient(_Connectionstring, ContainerName, BlobName);
            var path = Path.Combine("blobDownloads", $"sample-{id}.csv");
            await blobClient.DownloadToAsync(path);
        }

        public async Task DownloadBlobToStream()
        {
            var id = Guid.NewGuid();

            ContainerName =  "opstechtestbulkupload";
            BlobName=@"saple ss.csv";

            var blobClient = new BlobClient(_Connectionstring, ContainerName, BlobName);
            var path = Path.Combine("blobDownloads", $"Stream-{id}.csv");
            using  FileStream stream = new FileStream(path, FileMode.Create);

            await blobClient.DownloadToAsync(stream);
        }

        public async Task ReadFromBlobStream()
        {
            ContainerName =  "opstechtestbulkupload";
            BlobName=@"saple ss.csv";
            var blobclient = new BlobClient(_Connectionstring, ContainerName, BlobName);

             var stream = await  blobclient.OpenReadAsync();
            using StreamReader reader = new StreamReader(stream);
            var   data = reader.ReadToEnd();
            Console.WriteLine(data);
        }
    }
}
