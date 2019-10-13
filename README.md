# AzureEventHubFundamentals

## Architectural Considerations

* You can make namespace availability zone redundant at no extra cost! Do this :) 

### Throughput Units at NameSpace Level

* 1 = 1MB/S or 1000 events of ingress.
or
* 1 = 2MB/S or 2000 events of egress. 
* Throughput units cost money. 
* Event hubs share these throughput units within a namespace.
* If senders provide more data than have throughput units then they will be throttled. 
* So would need to increase number of throughput units. 
* You can use auto-inflate enabled at the namespace level, same place you set throughput units.
* **Auto-inflate doesn't auto-decrease!!! So need to downscale manually to save cash!!!**"
* 1 throughput unit also includes 84GN storage per day. 

### Event Hubs

* Maximum of 10 per namespace.

### Partitions

* Senders distributed round robin to partitions.
* Pay for throughput units, not partitions.
* Cant change number of partitions after create event hub. 
* Consumer group per partition is good practice (?). 
* Shouldnt create more partitions than really need as each partition needs its own socket connection, so can hinder performance. 32 events across 32 partitions means lots of connections if only one consumer group. 
* Need to consider partitions carefully, as if want to create more would have to create new hub and change all connections... 

## Dev Considerations

* You can batch together messages using the overloaded async method SendDataAsync, which sends multiple event datas e.g.
```c#
async Task SendDataAsync(IEnumerable<CoffeeMachineData> coffeeMachineDatas)

....
```
* See dispatcher time tick method to see how 2 messages sent as array. SO one message to reduce network bandwidth. 
* How to keep message size limit... be careful when have batch for example. Need to ensure split batch to not exceed message size. There eventhub data batch class to help you stay in the limit. 
* See _eventHubClient.CreateBatch() in CoffeeMachineDataSender.cs. 

## Data Capture Considerations..

* Yes you can use data capture even though says no here... ([Cant use gen 2 data lake yet](https://docs.microsoft.com/en-gb/azure/storage/blobs/data-lake-storage-upgrade?toc=%2fazure%2fstorage%2fblobs%2ftoc.json#azure-ecosystem))


