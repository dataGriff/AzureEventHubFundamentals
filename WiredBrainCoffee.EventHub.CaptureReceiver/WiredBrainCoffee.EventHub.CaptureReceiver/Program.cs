using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiredBrainCoffee.EventHub.Model;

namespace WiredBrainCoffee.EventHub.CaptureReceiver
{
  class Program
  {
        //this app only works with blob storage account and not gen 2 dat laka, must be a way of doing this? 

	// TODO: Enter the connection string of your storage account here
    const string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=datagriffircoffeesab;AccountKey=hNUicFGajbFbz4pp1yB1+aRE4GDrGofYISIVc1Yg4cvjufEsK+lZMx7888NJk5DKnfyyNFSk8VVPIkbDkPQt+A==;EndpointSuffix=core.windows.net";
	
	// TODO: Enter the blob container name here
    const string containerName = "eventhubcapture";
    static void Main(string[] args)
    {
      MainAsync().Wait();
    }

    private static async Task MainAsync()
    {
      var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
      var blobClient = storageAccount.CreateCloudBlobClient();
      var blobContainer = blobClient.GetContainerReference(containerName);

      var resultSegment =
         await blobContainer.ListBlobsSegmentedAsync(null, true, BlobListingDetails.All,
         null, null, null, null);

      foreach (var cloudBlockBlob in resultSegment.Results.OfType<CloudBlockBlob>())
      {
        await ProcessCloudBlockBlobAsync(cloudBlockBlob);
      }

      Console.ReadLine();
    }

    private static async Task ProcessCloudBlockBlobAsync(CloudBlockBlob cloudBlockBlob)
    {
      var avroRecords = await DownloadAvroRecordsAsync(cloudBlockBlob);

      PrintCoffeeMachineDatas(avroRecords);
    }

    private static void PrintCoffeeMachineDatas(List<AvroRecord> avroRecords)
    {
      var coffeeMachineDatas = avroRecords.Select(avroRecord =>
      CreateCoffeeMachineData(avroRecord));

      foreach (var coffeeMachineData in coffeeMachineDatas)
      {
        Console.WriteLine(coffeeMachineData);
      }
    }

    private static async Task<List<AvroRecord>> DownloadAvroRecordsAsync(CloudBlockBlob cloudBlockBlob)
    {
      var memoryStream = new MemoryStream();
      await cloudBlockBlob.DownloadToStreamAsync(memoryStream); //if large files better to use filestream instead
      memoryStream.Seek(0, SeekOrigin.Begin);
      List<AvroRecord> avroRecords;
      using (var reader = AvroContainer.CreateGenericReader(memoryStream))
      {
        using (var sequentialReader = new SequentialReader<object>(reader))
        {
          avroRecords = sequentialReader.Objects.OfType<AvroRecord>().ToList();
        }
      }

      return avroRecords;
    }

    private static CoffeeMachineData CreateCoffeeMachineData(AvroRecord avroRecord)
    {
      var body = avroRecord.GetField<byte[]>("Body");
      var dataAsJson = Encoding.UTF8.GetString(body);
      var coffeeMachineData = JsonConvert.DeserializeObject<CoffeeMachineData>(dataAsJson);
      return coffeeMachineData;
    }
  }
}
