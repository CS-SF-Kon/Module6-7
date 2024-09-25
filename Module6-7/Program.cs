namespace Module6_7
{
    struct Position
    {
        public string Type;
        public string Model;
        public float Cost;
        public byte Count;
        public float TotalCost;

        public Position(string Type, string Model, float Cost, byte Count)
        {
            this.Type = Type;
            this.Model = Model;
            this.Cost = Cost;
            this.Count = Count;
            TotalCost = (float)Cost * Count;
        }
    }

    abstract class Delivery
    {
        public string Address;
        public abstract string TypeOFDel();
    }

    class HomeDelivery : Delivery
    {
        public override string TypeOFDel()
        {
            return "Delivery to user's home";
        }
    }
    
    class PickPiontDelivery : Delivery
    {
        public override string TypeOFDel()
        {
            return "Delivery to nearest Pick-point";
        }
    }

    class ShopDelivery : Delivery
    {
        public override string TypeOFDel()
        {
            return "Delivery to nearest shop";
        }
    }

    class Order<TDelivery, TPosition>
        where TDelivery : Delivery
        where TPosition : struct
    {
        public TDelivery Delivery;
        public byte PosCount;
        public TPosition[] Positions;
        protected Guid number;
        public Guid Number
        {
            get
            {
                return number;
            }
            protected set
            {
                number = value;
            }
        }
        
        public string Description;

        public Order(byte PosCount)
        {
            Number = Guid.NewGuid();
            this.PosCount = PosCount;
            Positions = new TPosition[this.PosCount];
        }
    }

    internal class Program
    {
        static Order<Delivery, Position> OrderMaker()
        {
            var order = new Order<Delivery, Position>(PosCount:2);

            order.Delivery = new HomeDelivery();
            order.Delivery.Address = "Moscow-City";
            for (int i = 0; i < order.PosCount; i++)
            {
                order.Positions[i] = new Position("PC","Asus",499.9F,2);
            }
            order.Description = $"Order {order.Number} to {order.Delivery.Address}. Type of delivery - {order.Delivery.TypeOFDel()}";
            return order;
        }
        static void Main(string[] args)
        {
            var ord1 = OrderMaker();
            foreach (Position position in ord1.Positions)
            {
                Console.WriteLine(position.Model);
            }
            
            Console.WriteLine(ord1.Description);
        }
    }
}
