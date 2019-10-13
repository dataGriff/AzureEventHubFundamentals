using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;
using WiredBrainCoffee.EventHub.Model;

namespace WiredBrainCoffee.FunctionApp
{
  public static class AlertFunction
  {
    [FunctionName("AlertFunction")]
    public static void Run([EventHubTrigger("wiredbraincoffeeeh",
          Connection = "InputEventHubConnectionString",
          ConsumerGroup = "wired_brain_coffee_alert_function")]
      string[] inputMessages,
        [EventHub("wiredbraincoffeealerteh",
          Connection ="OutputEventHubConnectionString")]
      ICollector<string> outputMessages,
      TraceWriter log)
    {
      foreach (var inputMessage in inputMessages)
      {
        var data = JsonConvert.DeserializeObject<CoffeeMachineData>(inputMessage);
        log.Info($"{data}");

        if (data.SensorType == "CounterCappuccino"
         && data.SensorValue % 10 == 0)
        {
          outputMessages.Add(inputMessage);
        }
      }
    }
  }
}
