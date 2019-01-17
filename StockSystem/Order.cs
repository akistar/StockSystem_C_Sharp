using System;

namespace StockSystem
{
    class Order
    {
        private String orderId;
        private String userId;
        private int amount;
        private double price;
        private String action;
        private long create;
        private String status;


        public Order(String orderId, String userId, int amount, double price, String action, long create, String status)
        {
            this.orderId = orderId;
            this.userId = userId;
            this.amount = amount;
            this.price = price;
            this.action = action;
            this.create = create;
            this.status = status;
        }


        public int Amount { get => amount; set => amount = value; }
        public double Price { get => price; set => price = value; }
        public string OrderId { get => orderId; set => orderId = value; }
        public string UserId { get => userId; set => userId = value; }
        public string Action { get => action; set => action = value; }
        public long Create { get => create; set => create = value; }
        public string Status { get => status; set => status = value; }
    }


    class CompleteOrder: Order {
        private String orderRef;

        public CompleteOrder(String orderId, String userId, int amount, double price, String action, long create, String status, String orderRef):
        base(orderId,userId,amount,price,action,create,status){
            this.orderRef = orderRef;
        }

        public string OrderRef { get => orderRef; set => orderRef = value; }
    }
}

