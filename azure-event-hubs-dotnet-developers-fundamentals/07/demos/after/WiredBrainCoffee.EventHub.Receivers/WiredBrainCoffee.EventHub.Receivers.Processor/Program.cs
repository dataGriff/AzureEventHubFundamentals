using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;

namespace WiredBrainCoffee.EventHub.Receivers.Processor
{
  class Program
  {
	// TODO: Initialize the five constants below that are required 
	//       for the EventProcessorHost constructor
    const string eventHubPath = "";
    const string consumerGroupName = "";
    const string eventHubConnectionString = "";
    const string storageConnectionString = "";
    const string leaseContainerName = "";

    static void Main(string[] args)
    {
      MainAsync().Wait();
    }

    private static async Task MainAsync()
    {
      Console.WriteLine($"Register the {nameof(WiredBrainCoffeeEventProcessor)}");

      var eventProcessorHost = new EventProcessorHost(
        eventHubPath,
        consumerGroupName,
        eventHubConnectionString,
        storageConnectionString,
        leaseContainerName);

      await eventProcessorHost.RegisterEventProcessorAsync<WiredBrainCoffeeEventProcessor>();

      Console.WriteLine("Waiting for incoming events...");
      Console.WriteLine("Press any key to shutdown");
      Console.ReadLine();

      await eventProcessorHost.UnregisterEventProcessorAsync();
    }
  }
}
