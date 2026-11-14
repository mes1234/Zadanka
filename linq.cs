using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Calculate sum of orders value placed by each client. Include only orders containing socks.
 */

var customers = new List<Customer>
{
    new (1, "Alice"),
    new (2, "Bob"),
    new (3, "Charlie"),
};

var items = new List<Item>
{
    new (1, "Socks", 3.5m),
    new (2, "Hat", 25m),
    new (3, "Dress", 75m)
};

var orders = new List<Order>
{
    new (101, 1),
    new (102, 1),
    new (103, 1),
    new (201, 2),
};

var itemsInOrders = new List<ItemInOrder>
{
    new (3, 101),
    new (2, 101),
    new (1, 101),
    new (1, 102),
    new (2, 103),
    new (1, 201),
};

var result = GetSumOfAllOrdersContainingSocksByClient();

foreach (var line in result.Select(x => $"{x.CustomerName}\t{x.SumOfOrders}")) Console.WriteLine(line);

IEnumerable<(string CustomerName, decimal SumOfOrders)> GetSumOfAllOrdersContainingSocksByClient()
{
   // Cache socket item for later use
   var socksItem = items.First(x=>x.Name=="Socks");
	
   // Orders with socks
   var ordersIdsWithSocks = itemsInOrders.Where(x=>x.ItemId == socksItem.Id).Select(x=>x.OrderId);
	
   var customersWhoOrderedSocks = orders.Where(x=>ordersIdsWithSocks.Contains(x.Id)).GroupBy(x=>x.CustomerId);
	
   var result = new Dictionary<string, decimal>();
	
   foreach(var customerAccumulation in customersWhoOrderedSocks){
	   var customer = customers.First(x=>x.Id == customerAccumulation.Key);
	   var name = customer.Name;
	   var count = customerAccumulation.Count();
	   
	   result[name] = customerAccumulation.Count() * socksItem.Value;
   }
	
	
  return result.Select(x=>(x.Key,x.Value));
}

record CustomerOrder(int CustomerId, Order Order);


record Order(int Id, int CustomerId);
record Customer(int Id, string Name);
record Item(int Id, string Name, decimal Value);
record ItemInOrder(int ItemId, int OrderId);