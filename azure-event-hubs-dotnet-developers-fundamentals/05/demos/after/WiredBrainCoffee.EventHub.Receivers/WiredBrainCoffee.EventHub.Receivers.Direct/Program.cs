using Microsoft.Azure.EventHubs;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiredBrainCoffee.EventHub.Receivers.Direct
{
  class Program
  {
	// TODO: Add your event hub connection string here
    const string eventHubConnectionString = ""; 
	
    static void Main(string[] args)
    {
      MainAsync().Wait();
    }

    private static async Task MainAsync()
    {
      Console.WriteLine("Connecting to the Event Hub...");
      var eventHubClient =
        EventHubClient.CreateFromConnectionString(eventHubConnectionString);
      var runtimeInformation = await eventHubClient.GetRuntimeInformationAsync();
      var partitionReceivers = runtimeInformation.PartitionIds.Select(partitionId =>
          eventHubClient.CreateReceiver("wired_brain_coffee_console_direct",
          partitionId, DateTime.Now)).ToList();

      Console.WriteLine("Waiting for incoming events...");

      foreach (var partitionReceiver in partitionReceivers)
      {
        partitionReceiver.SetReceiveHandler(
          new WiredBrainCoffeePartitionReceiveHandler(partitionReceiver.PartitionId));
      }

      Console.WriteLine("Press any key to shutdown");
      Console.ReadLine();
      await eventHubClient.CloseAsync();
    }
  }
}
