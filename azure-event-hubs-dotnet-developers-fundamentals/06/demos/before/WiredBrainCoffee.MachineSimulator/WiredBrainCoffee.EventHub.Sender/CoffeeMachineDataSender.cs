using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiredBrainCoffee.EventHub.Sender.Model;

namespace WiredBrainCoffee.EventHub.Sender
{
  public interface ICoffeeMachineDataSender
  {
    Task SendDataAsync(CoffeeMachineData data);
    Task SendDataAsync(IEnumerable<CoffeeMachineData> datas);
  }
  public class CoffeeMachineDataSender : ICoffeeMachineDataSender
  {
    private EventHubClient _eventHubClient;

    public CoffeeMachineDataSender(string eventHubConnectionString)
    {
      _eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString);
    }
    public async Task SendDataAsync(CoffeeMachineData data)
    {
      EventData eventData = CreateEventData(data);
      await _eventHubClient.SendAsync(eventData);
    }

    public async Task SendDataAsync(IEnumerable<CoffeeMachineData> datas)
    {
      var eventDatas = datas.Select(coffeeMachineData => CreateEventData(coffeeMachineData));

      var eventDataBatch = _eventHubClient.CreateBatch();

      foreach (var eventData in eventDatas)
      {
        if (!eventDataBatch.TryAdd(eventData))
        {
          await _eventHubClient.SendAsync(eventDataBatch.ToEnumerable());
          eventDataBatch = _eventHubClient.CreateBatch();
          eventDataBatch.TryAdd(eventData);
        }
      }

      if (eventDataBatch.Count > 0)
      {
        await _eventHubClient.SendAsync(eventDataBatch.ToEnumerable());
      }
    }

    private static EventData CreateEventData(CoffeeMachineData data)
    {
      var dataAsJson = JsonConvert.SerializeObject(data);
      var eventData = new EventData(Encoding.UTF8.GetBytes(dataAsJson));
      return eventData;
    }
  }
}
