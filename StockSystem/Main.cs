using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace StockSystem
{
    public class MainSystem
    {
        static void Main(string[] args)

        {
            IList<Order> buyPendingOrderList = new List<Order>();
            IList<Order> sellPendingOrderList = new List<Order>();
            IList<Order> buyPendingOrders = new List<Order>();
            IList<Order> sellPendingOrders = new List<Order>();

            double finalPrice = -1;

            IList<String> completeBuyOrdersIds = new List<String>();
            IList<String> completeSellOrdersIds = new List<String>();


            IList<JObject> completeBuyOrders = new List<JObject>();
            IList<JObject> completeSellOrders = new List<JObject>();

            object[] currentBuyInfo = new object[] { "", 0 };
            object[] currentSellInfo = new object[] { "", 0 };


            using (StreamReader r = new StreamReader("buy_pending_order.json"))
            {
                String buyJson = r.ReadToEnd();

                JArray buyOrders = (JArray)JsonConvert.DeserializeObject(buyJson);

                foreach (JObject o in buyOrders)
                {
                    String orderId = (String)o["id"];
                    String userId = (String)o["userID"];
                    int amount = (int)o["amount"];
                    double price = (double)o["price"];
                    String status = (String)o["status"];
                    String action = (String)o["action"];
                    long create = (long)o["create"];
                    buyPendingOrderList.Add(new Order(orderId, userId, amount, price, action, create, status));
                    buyPendingOrders.Add(new Order(orderId, userId, amount, price, action, create, status));

                }

                buyPendingOrderList = (from o in buyPendingOrderList orderby o.Price descending select o).ToList();

                foreach (Order order in buyPendingOrderList)
                {
                    Console.WriteLine(order.OrderId);


                }

            }

            using (StreamReader r = new StreamReader("sell_pending_order.json"))
            {
                String sellJson = r.ReadToEnd();

                JArray sellOrders = (JArray)JsonConvert.DeserializeObject(sellJson);

                foreach (JObject o in sellOrders)
                {
                    String orderId = (String)o["id"];
                    String userId = (String)o["userID"];
                    int amount = (int)o["amount"];
                    double price = (double)o["price"];
                    String status = (String)o["status"];
                    String action = (String)o["action"];
                    long create = (long)o["create"];
                    sellPendingOrderList.Add(new Order(orderId, userId, amount, price, action, create, status));
                    sellPendingOrders.Add(new Order(orderId, userId, amount, price, action, create, status));


                }

                sellPendingOrderList = (from o in sellPendingOrderList orderby o.Price select o).ToList();
                foreach (Order order in sellPendingOrderList)
                {
                    Console.WriteLine(order.OrderId);

                }

            }

            while (sellPendingOrderList[0].Price - buyPendingOrderList[0].Price <= 0)
            {
                if (sellPendingOrderList[0].Amount - buyPendingOrderList[0].Amount == 0)
                {
                    completeBuyOrdersIds.Add(buyPendingOrderList[0].OrderId);
                    completeSellOrdersIds.Add(sellPendingOrderList[0].OrderId);
                    finalPrice = (sellPendingOrderList[0].Price + buyPendingOrderList[0].Price) / 2;
                    currentBuyInfo[0] = "";
                    currentBuyInfo[1] = 0;
                    currentSellInfo[0] = "";
                    currentSellInfo[1] = 0;
                    buyPendingOrderList.RemoveAt(0);
                    sellPendingOrderList.RemoveAt(0);
                }
                else if (sellPendingOrderList[0].Amount - buyPendingOrderList[0].Amount > 0)
                {
                    completeBuyOrdersIds.Add(buyPendingOrderList[0].OrderId);
                    sellPendingOrderList[0].Amount = sellPendingOrderList[0].Amount - buyPendingOrderList[0].Amount;
                    finalPrice = (sellPendingOrderList[0].Price + buyPendingOrderList[0].Price) / 2;
                    currentBuyInfo[0] = "";
                    currentBuyInfo[1] = 0;
                    currentSellInfo[0] = sellPendingOrderList[0].OrderId;
                    currentSellInfo[1] = sellPendingOrders[0].Amount;
                    buyPendingOrderList.RemoveAt(0);
                }
                else
                {
                    completeSellOrdersIds.Add(sellPendingOrderList[0].OrderId);
                    buyPendingOrderList[0].Amount = buyPendingOrderList[0].Amount - sellPendingOrderList[0].Amount;
                    finalPrice = (sellPendingOrderList[0].Price + buyPendingOrderList[0].Price) / 2;
                    currentSellInfo[0] = "";
                    currentSellInfo[1] = 0;
                    currentBuyInfo[0] = buyPendingOrderList[0].OrderId;
                    currentBuyInfo[1] = buyPendingOrders[0].Amount;
                    sellPendingOrderList.RemoveAt(0);

                }
            }


            foreach (Order o in buyPendingOrders)
            {
                if (completeBuyOrdersIds.Contains(o.OrderId))
                {

                    dynamic jsonObject = new JObject();
                    jsonObject.id = Guid.NewGuid().ToString();
                    jsonObject.userId = o.UserId;
                    jsonObject.amount = o.Amount;
                    jsonObject.price = finalPrice;
                    jsonObject.action = o.Action;
                    jsonObject.status = "success";
                    jsonObject.create = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                    jsonObject.order_ref = o.OrderId;
                    completeBuyOrders.Add(jsonObject
                                         );
                }
                else if (o.OrderId.Equals(currentBuyInfo[0]))
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.id = Guid.NewGuid().ToString();
                    jsonObject.userId = o.UserId;
                    jsonObject.amount = o.Amount - (int)currentBuyInfo[1];
                    jsonObject.price = finalPrice;
                    jsonObject.action = o.Action;
                    jsonObject.status = "success";
                    jsonObject.create = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                    jsonObject.order_ref = o.OrderId;
                    completeBuyOrders.Add(jsonObject
                                         );
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.id = Guid.NewGuid().ToString();
                    jsonObject.userId = o.UserId;
                    jsonObject.amount = (int)currentBuyInfo[1];
                    jsonObject.price = o.Price;
                    jsonObject.action = o.Action;
                    jsonObject.status = "fail";
                    jsonObject.create = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                    jsonObject.order_ref = o.OrderId;
                    completeBuyOrders.Add(jsonObject
                                         );

                }
            }

            String completeBuyJson = JsonConvert.SerializeObject(completeBuyOrders.ToArray());
            System.IO.File.WriteAllText("buy_final_order.json", completeBuyJson);


            foreach (Order o in sellPendingOrders)
            {
                if (completeSellOrdersIds.Contains(o.OrderId))
                {

                    dynamic jsonObject = new JObject();
                    jsonObject.id = Guid.NewGuid().ToString();
                    jsonObject.userId = o.UserId;
                    jsonObject.amount = o.Amount;
                    jsonObject.price = finalPrice;
                    jsonObject.action = o.Action;
                    jsonObject.status = "success";
                    jsonObject.create = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                    jsonObject.order_ref = o.OrderId;
                    completeSellOrders.Add(jsonObject
                                         );
                }
                else if (o.OrderId.Equals(currentBuyInfo[0]))
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.id = Guid.NewGuid().ToString();
                    jsonObject.userId = o.UserId;
                    jsonObject.amount = o.Amount - (int)currentSellInfo[1];
                    jsonObject.price = finalPrice;
                    jsonObject.action = o.Action;
                    jsonObject.status = "success";
                    jsonObject.create = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                    jsonObject.order_ref = o.OrderId;
                    completeSellOrders.Add(jsonObject
                                         );
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.id = Guid.NewGuid().ToString();
                    jsonObject.userId = o.UserId;
                    jsonObject.amount = (int)currentSellInfo[1];
                    jsonObject.price = o.Price;
                    jsonObject.action = o.Action;
                    jsonObject.status = "fail";
                    jsonObject.create = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                    jsonObject.order_ref = o.OrderId;
                    completeSellOrders.Add(jsonObject
                                         );

                }
            }

            String completeSellJson = JsonConvert.SerializeObject(completeSellOrders.ToArray());
            System.IO.File.WriteAllText("sell_final_order.json", completeSellJson);

        }
    }
}
