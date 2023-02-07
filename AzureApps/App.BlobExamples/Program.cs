

using App.BlobExamples;

var storageExample = new CloudBlobBasicOperations();
//await storageExample.CheckBlobExistist();
//await storageExample.DownloadBlobTolocalPath();
//await storageExample.DownloadBlobToStream();
await storageExample.ReadFromBlobStream();