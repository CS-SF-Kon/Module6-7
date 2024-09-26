using System.Net;

namespace Module6_7
{
    abstract class Customer // свой абстрактный класс
    {
        protected int ID;
        protected string Type;
        public string Address;
        protected Customer(string type, string address)
        {
            Type = type;
            var rnd = new Random();
            ID = rnd.Next();
            Address = address;
        }
        public abstract string CustomerInfo(); // абстрактный метод
    }
    
    class PrivateUser : Customer // свой класс 2 + наследование
    {
        protected string CustomerName { get; set; }
        public PrivateUser(string CustomerName, string address, string type = "Private user") : base(type, address)
        {
            this.CustomerName = CustomerName;
        }

        public override string CustomerInfo()
        {
            return string.Concat(Type, " ", ID, " ", CustomerName, " ", Address);
        }
    }

    class Company : Customer // свой класс 3
    {
        protected string CompanyName { get; set; }
        public Company(string CompanyName, string address, string type = "Company") : base(type, address)
        {
            this.CompanyName = CompanyName;
        }
        public override string CustomerInfo()
        {
            return string.Concat(Type, " ", ID, " ", CompanyName, " ", Address);
        }
    }

    class State : Customer // свой класс 4
    {
        public State(string address, string type = "State") : base(type, address)
        {

        }
        public override string CustomerInfo()
        {
            return string.Concat(Type, " ", ID);
        }
    }

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
        public abstract string TypeOFDel(); // абстрактный метод
    }

    class HomeDelivery : Delivery
    {
        public override string TypeOFDel() // переопределение метода
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

    class Order<TDelivery, TPosition> // обобщения
        where TDelivery : Delivery
        where TPosition : struct
    {
        public TDelivery Delivery;
        private Customer customer;
        public byte PosCount;
        public TPosition[] Positions;
        protected Guid number; // инкапсуляция
        public Guid Number // инкапсуляция - свойство
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

        public Order(byte PosCount, Customer customer) // конструктор с параметрами
        {
            Number = Guid.NewGuid();
            this.PosCount = PosCount;
            Positions = new TPosition[this.PosCount]; // композиция (используется, правда, не класс, а структура) - перечень заказываемого не может существовать без заказа в целом
            this.customer = customer; // агрегация - заказчик может иметь несколько заказов, а, занчит, должен существовать не зависимо от заказа
        }
    }

    internal class Program
    {
        static Order<Delivery, Position> OrderMaker(Customer customer)
        {
            var order = new Order<Delivery, Position>(PosCount:2, customer: customer);

            order.Delivery = new HomeDelivery
            {
                Address = customer.Address
            };
            for (int i = 0; i < order.PosCount; i++)
            {
                order.Positions[i] = new Position("PC","Asus",499.9F,2);
            }
            order.Description = $"Order {order.Number} to {order.Delivery.Address}. Type of delivery - {order.Delivery.TypeOFDel()}";
            return order;
        }
        static void Main(string[] args)
        {
            Customer new_customer = new PrivateUser(CustomerName: "Dmitry", address: "Moscow");
            Console.WriteLine(new_customer.CustomerInfo());

            Customer second_customer = new Company(CompanyName: "EVRAZ", address: "Tagil");
            Console.WriteLine(second_customer.CustomerInfo());

            Customer third_customer = new State(address: "Secret");
            Console.WriteLine(third_customer.CustomerInfo());

            var ord1 = OrderMaker(new_customer);
            
            foreach (Position position in ord1.Positions)
            {
                Console.WriteLine(position.Model);
            }
            
            
            Console.WriteLine(ord1.Description);
        }
    }
}
