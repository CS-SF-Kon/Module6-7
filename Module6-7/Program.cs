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
        private string customerName;
        protected string CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                customerName = string.IsNullOrEmpty(value) ? "NoName" : value; // логика в сеттере - пустое поле имени при создании заказчика превратится в "NoName"
            }
        }
        public PrivateUser(string CustomerName, string address, string type = "Private user") : base(type, address)
        {
            this.CustomerName = CustomerName;
        }

        public override string CustomerInfo() // переопределение абстрактного метода
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

    class Position
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

    abstract class Delivery<T>
    {
        public T Address; // наследование обобщения
        public abstract string TypeOfDel(); // абстрактный метод
    }

    class HomeDelivery : Delivery<string> // наследование обобщения - бессмысленное (в данном случае) и беспощадное 
    {
        public override string TypeOfDel() // переопределение метода
        {
            return "Delivery to user's home";
        }
    }
    
    class PickPointDelivery : Delivery<string>
    {
            public override string TypeOfDel()
        {
            return "Delivery to nearest Pick-point";
        }
    }

    class ShopDelivery : Delivery<string>
    {
        public override string TypeOfDel()
        {
            return "Delivery to nearest shop";
        }
    }

    class Order<TDelivery, TPosition> // обобщения
        where TDelivery : Delivery<string>
        where TPosition : Position
    {
        public static int TotalOrders; // статическое поле - будет отслеживать общее количество заказов всех видов и всех пользователей
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
            this.customer = customer; // агрегация - заказчик может иметь несколько заказов, а, значит, должен существовать независимо от заказа
            TotalOrders++;
        }

        public Position this[int index] // индексктор курильщика
        {
            get
            {
                return Positions[index];
            }
        }
    }

    static class OrderShortNum
    {
        public static void ShortNum(this string Num) // метод расширения, выводит укороченный номер заказа
        {
            Console.Write(Num.Substring(0, 8));
        }
    }

    internal class Program
    {
        public static void Func<T>(ref T ord) // обобщённый метод, тоже бессмысленный
        {
            Console.WriteLine(ord.GetType());
        }

        static Order<Delivery<string>, Position> OrderMaker(Customer customer)
        {
            var order = new Order<Delivery<string>, Position>(PosCount:3, customer: customer);

            order.Delivery = new HomeDelivery
            {
                Address = customer.Address
            };

            order.Positions = new Position[] {new Position("Laptop", "Asus", 499.0F, 1), new Position("PC", "Dell", 599.9F, 2)}; // да, 

            //for (int i = 0; i < order.PosCount; i++)
            //{
            //    order.Positions[i] = new Position("PC","Asus",499.9F,2);
            //}
            

            order.Description = $"Order {order.Number} to {order.Delivery.Address}. Type of delivery - {order.Delivery.TypeOfDel()}"; // насколько адекватно делать так, типа "вот эту часть экземлляра соберу в конструкторе, а вот эту - вне конструктора"? наверное, не очень...
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

            Position pos = ord1[1]; // вызов индексатора
            Console.WriteLine(pos.Model + " " + pos.Count);

            Customer fourth_customer = new PrivateUser(CustomerName: "", address: "Unknown");
            Console.WriteLine(fourth_customer.CustomerInfo());
            
            Console.WriteLine(ord1.Description);

            Func<Order<Delivery<string>, Position>>(ref ord1); // вызов обобщённого метода (почему параметр внутри "<>" тусклый, как-будто не обязательный?)

            var ord2 = OrderMaker(second_customer);
            Console.WriteLine(Order<Delivery<string>, Position>.TotalOrders); // вызов статического метода

            string shortnum = Convert.ToString(ord1.Number);
            shortnum.ShortNum(); // вызов метода расширения
        }
    }
}
