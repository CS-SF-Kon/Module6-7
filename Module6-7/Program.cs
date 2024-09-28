namespace Module6_7
{
    /// <summary>
    /// Абстрактрый класс, описывающий пользователя
    /// </summary>
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
    /// <summary>
    /// Наследуемый от "пользователя" класс, описывающий частное лицо
    /// </summary>
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
        /// <summary>
        /// Конструктор, принимающий на вход имя частного пользователя, а также задающий поля родительского класса 
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <param name="address"></param>
        /// <param name="type"></param>
        public PrivateUser(string CustomerName, string address, string type = "Private user") : base(type, address)
        {
            this.CustomerName = CustomerName;
        }

        public override string CustomerInfo() // переопределение абстрактного метода
        {
            return string.Concat("Type: ", Type, "; ID: ", ID, ", Name: ", CustomerName, ", Address: ", Address);
        }
    }
    /// <summary>
    /// Наследуемый класс, описывающий компанию как пользователя
    /// </summary>
    class Company : Customer // свой класс 3
    {
        protected string CompanyName { get; set; }
        public Company(string CompanyName, string address, string type = "Company") : base(type, address)
        {
            this.CompanyName = CompanyName;
        }
        public override string CustomerInfo()
        {
            return string.Concat("Type: ", Type, "; ID: ", ID, ", Company name: ", CompanyName, ", Address: ", Address);
        }
    }
    /// <summary>
    /// Наследуемый класс, предназначенный для госзаказчиков
    /// </summary>
    class State : Customer // свой класс 4
    {
        public State(string address, string type = "State") : base(type, address)
        {

        }
        public override string CustomerInfo()
        {
            return string.Concat("Type: ", Type, "; ID: ", ID, ", It is alloved to see name and address of this type of customers only for special level employees"); // конечно же никаких "спешиал лэвел эмплойес" тут не предусмотрено...
        }
    }
    /// <summary>
    /// Класс для описания одной позиции заказа
    /// </summary>
    class Position // свой класс 5
    {
        public string Type;
        public string Model;
        public float Cost;
        public byte Count; // если в заказ добавятся несколько товаров одного вида, они будут считаться одной позицией
        public float TotalCost;

        public Position(string Type, string Model, float Cost, byte Count)
        {
            this.Type = Type;
            this.Model = Model;
            this.Cost = Cost;
            this.Count = Count;
            TotalCost = (float)Cost * Count; // а вот суммарная стоимость такой позиции будет подсчитана исходя их количества этих однотипных товаров
        }

        public static float operator + (Position a, Position b) // перегрузка оператора "+", который будет считать суммарную стоимость двух позиций
        {
            return a.TotalCost + b.TotalCost;
        }
    }
    /// <summary>
    /// Асбтрактрый класс, отвечающий за описание доставки
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
    /// <summary>
    /// Класс, описывающий заказ
    /// </summary>
    /// <typeparam name="TDelivery"></typeparam>
    /// <typeparam name="TPosition"></typeparam>
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

        public Position this[int index] // индексатор курильщика
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

        static Order<Delivery<string>, Position> OrderMaker(Customer customer) // сбор заказа решил сделать отдельным методом
        {
            var order = new Order<Delivery<string>, Position>(PosCount:3, customer: customer);

            Console.Write("What type of delivery do you prefer (home delivery (print \"home\"), nearest shop (print \"shop\") or pickpoint (print \"pp\"): ");
            string type = Console.ReadLine();
            switch (type)
            {
                case "home":
                    order.Delivery = new HomeDelivery { Address = customer.Address };
                    break;
                case "shop":
                    order.Delivery = new ShopDelivery { Address = customer.Address };
                    break;
                case "pp":
                    order.Delivery = new PickPointDelivery { Address = customer.Address };
                    break;
                default: 
                    Console.WriteLine("wrong text"); // знаю, что надо зациклить, пока не будет введено верное значение, иначе крашится в рантайме
                    break;
            }

            order.Positions = new Position[] {new Position("Laptop", "Asus", 499.0F, 1), new Position("PC", "Dell", 599.9F, 2)}; // да, пока захардкожено

            order.Description = $"Order {order.Number} to {order.Delivery.Address}. Type of delivery - {order.Delivery.TypeOfDel()}"; // наверное, неадекватно делать так, типа "вот эту часть экземлляра соберу в конструкторе, а вот эту - вне конструктора"
            return order;
        }

        static void Main(string[] args)
        {
            Customer new_customer = new PrivateUser(CustomerName: "Dmitry", address: "Moscow"); // создаём первого пользователя
            Console.Write("First user created. Info about him: ");
            Console.WriteLine(new_customer.CustomerInfo()); // посмотрим информацию о нём

            Console.WriteLine();
            Console.Write("Second user created. Info about him: ");
            Customer second_customer = new Company(CompanyName: "EVRAZ", address: "Tagil"); // создаём второго пользователя
            Console.WriteLine(second_customer.CustomerInfo());

            Console.WriteLine();
            Console.Write("Third user created. Info about him: ");
            Customer third_customer = new State(address: "Secret"); // создаём третьего пользователя
            Console.WriteLine(third_customer.CustomerInfo());

            Console.WriteLine();
            Console.WriteLine("First order for first user added");
            var ord1 = OrderMaker(new_customer); // создаём первый заказ для первого пользователя, нас попросят выбрать тип доставки

            Console.WriteLine();
            Console.WriteLine("");
            foreach (Position position in ord1.Positions) // выведем данные по всем позициям первого заказа
            {
                Console.WriteLine($"Type: {position.Type}");
                Console.WriteLine($"Model: {position.Model}");
                Console.WriteLine($"Cost: {position.Cost}");
                Console.WriteLine($"Count: {position.Count}");
                Console.WriteLine($"Total cost: {position.TotalCost}");
            }

            Console.WriteLine();
            Position pos = ord1[1]; // вызов индексатора, который возвращает определённую позицию заказа
            Console.WriteLine("Let's see elements of second position of first order");
            Console.WriteLine(pos.Count + " units of " + pos.Type + ", model - " + pos.Model); // вторым мы заказали два компа Dell (первой позицией был один ноут Asus)

            Console.WriteLine();
            Customer fourth_customer = new PrivateUser(CustomerName: "", address: "Unknown"); // проверка срабатывания логики в сеттере при создании 
            Console.WriteLine("Private user with empty name");
            Console.WriteLine(fourth_customer.CustomerInfo()); // пустота в поле имени сменилась на NoName
            
            Console.WriteLine();
            Console.WriteLine("Description of first order");
            Console.WriteLine(ord1.Description);

            Console.WriteLine();
            Func<Order<Delivery<string>, Position>>(ref ord1); // вызов обобщённого метода (почему параметр внутри "<>" тусклый, как-будто не обязательный?)

            Console.WriteLine();
            Console.WriteLine("second order for second user added");
            var ord2 = OrderMaker(second_customer); // создаём второй заказ для второго пользователя

            Console.WriteLine();
            Console.Write("Total count of orders in our shop: ");
            Console.WriteLine(Order<Delivery<string>, Position>.TotalOrders); // вызов статического метода

            Console.WriteLine();
            Console.Write("Let's see total cost of first order: ");
            Console.WriteLine(ord2.Positions[1] + ord2.Positions[0]); // суммарная стоимость первого заказа

            Console.WriteLine();
            Convert.ToString(ord1.Number).ShortNum(); // вызов метода расширения, возвращающий укороченный номер заказа
        }
    }
}
