using System.Collections.Generic;
using CarService;

namespace CarService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int startMoney = 250;
            int fixedPenality = 200;
            float dynamicPenalityMultiplyer = 1.5f;
            int workPrice = 70;
            int carsCount = 6;
            int detailsInStorage = 2;

            List<DetailFactory> detailFactoriys = new()
            {
                new("Корпус", 100),
                new("Двигатель", 140),
                new("Колёса", 80),
                new("Подвеска", 110),
                new("Коробка передач", 120)
            };

            ViewColors menuColors = new(
                ConsoleColor.DarkCyan, 
                ConsoleColor.DarkYellow, 
                ConsoleColor.Green, 
                ConsoleColor.Red);

            CarFactory carFactory = new(detailFactoriys);
            CarServiceFactory serviceFactory = new(detailFactoriys, detailsInStorage);
            CarService carService = serviceFactory.Create(startMoney, fixedPenality, dynamicPenalityMultiplyer, workPrice);
            CarServiceView view = new(carService, menuColors);

            carService.TakeCars(carFactory.Create(carsCount));
            carService.ServiceCars();
        }
    }

    internal class DetailUtilits
    {
        public static bool TryGetDetail(ICollection<IDetail> details, IDetail sample, out IDetail result)
        {
            ArgumentNullException.ThrowIfNull(details);
            ArgumentNullException.ThrowIfNull(sample);

            result = default;

            foreach (var detail in details)
            {
                if (detail.Equals(sample))
                {
                    result = detail;
                    return true;
                }
            }

            return false;
        }
    }

    public static class UserUtilits
    {
        private static Random _random = new();

        public static bool GetRandomBoolean(float trueChance)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(trueChance);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(trueChance, 1);

            return _random.NextSingle() <= trueChance;
        }

        public static T GetRandomObject<T>(List<T> objects)
        {
            ArgumentNullException.ThrowIfNull(objects);

            return objects[_random.Next(0, objects.Count)];
        }

        public static void WriteWithColor(string mesage, ConsoleColor color)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(mesage);
            Console.ForegroundColor = defaultColor;
        }

        public static float ConvertToPercent(float multiplyer)
        {
            return multiplyer * 100;
        }

        public static float Round(float number, int decimals)
        {
            return Convert.ToSingle(Math.Round(Convert.ToDecimal(number), decimals));
        }

        public static void HandleIndexExeptions<T>(IEnumerable<T> collection, int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, collection.Count());
        }
    }

    public static class QueueExtentions
    {
        public static void EnqueueRange<T>(this Queue<T> queue, List<T> items)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }

    public interface ICar
    {
        IReadOnlyList<IDetail> Details { get; }

        bool IsBroken();
    }

    public class Car : ICar
    {
        private readonly List<Detail> _details;

        public Car(List<Detail> details)
        {
            ArgumentNullException.ThrowIfNull(details);

            _details = new HashSet<Detail>(details).ToList();
            TakeRandomDamage();
        }

        public IReadOnlyList<IDetail> Details => _details;

        public bool IsBroken()
        {
            return _details.Any(detail => detail.IsBroken);
        }

        private void TakeRandomDamage()
        {
            float breakChance = 0.4f;

            foreach (var detail in _details)
            {
                if (UserUtilits.GetRandomBoolean(breakChance))
                {
                    detail.Break();
                }
            }

            if (IsBroken() == false)
            {
                UserUtilits.GetRandomObject(_details).Break();
                Console.WriteLine("принудительная поломка");
                Console.ReadLine();
            }
        }

        public void ReplaceDetail(int index, IDetail detail)
        {
            ArgumentNullException.ThrowIfNull(detail);
            UserUtilits.HandleIndexExeptions(Details, index);

            if (_details[index].Equals(detail) == false)
            {
                throw new ArgumentException("incorrect detail for replace");
            }

            _details[index] = (Detail)detail;
        }
    }

    public class CarFactory
    {
        private readonly List<DetailFactory> _factoriys;

        public CarFactory(List<DetailFactory> detailFactorys)
        {
            ArgumentNullException.ThrowIfNull(detailFactorys);

            _factoriys = detailFactorys;
        }

        public Car Create()
        {
            List<Detail> details = new();

            foreach (var factory in _factoriys)
            {
                details.Add(factory.Create());
            }

            return new Car(details);
        }

        public List<Car> Create(int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

            List<Car> cars = new();

            for (int i = 0; i < count; i++)
            {
                cars.Add(Create());
            }

            return cars;
        }
    }

    public interface ICarService
    {
        event Action<ICar, bool> CarServing;
        event Action RequestingCommand;
        event Action<string, ServiceError> CommandFailed;
        public event Action ServiceStarting;
        public event Action ServiceEnded;

        int Money { get; }

        int FixedPenality { get; }

        float DynamicPenalityMultiplyer { get; }

        int WorkPrice { get; }

        string CommandEndRepair { get; }

        bool HaveDetail(int index, ICar car);

        int GetDynamicPenality(ICar car);
    }

    public class CarService : ICarService
    {
        private readonly Queue<Car> _cars;
        private readonly Storage _storage;
        public CarService(Storage storage, int startMoney, int fixedPenality, float dynamicPenalityMultiplyer, int workPrice)
        {
            ArgumentNullException.ThrowIfNull(storage);
            ArgumentOutOfRangeException.ThrowIfNegative(startMoney);
            ArgumentOutOfRangeException.ThrowIfNegative(fixedPenality);
            ArgumentOutOfRangeException.ThrowIfNegative(dynamicPenalityMultiplyer);
            ArgumentOutOfRangeException.ThrowIfNegative(workPrice);

            _storage = storage;
            _cars = new();
            Money = startMoney;
            FixedPenality = fixedPenality;
            DynamicPenalityMultiplyer = dynamicPenalityMultiplyer;
            WorkPrice = workPrice;
        }

        public event Action<ICar, bool> CarServing;
        public event Action RequestingCommand;
        public event Action<string, ServiceError> CommandFailed;
        public event Action ServiceStarting;
        public event Action ServiceEnded;

        public int Money { get; private set; }

        public int FixedPenality { get; }

        public float DynamicPenalityMultiplyer { get; }

        public int WorkPrice { get; }

        public string CommandEndRepair { get; } = "end";

        public void ServiceCars()
        {
            bool isCarChanged;
            CommandResult commandResult;
            ServiceStarting?.Invoke();

            while (_cars.Count > 0)
            {
                commandResult = CommandResult.Error;
                isCarChanged = false;
                Car car = _cars.Dequeue();
                CarServing?.Invoke(car, isCarChanged);

                while (commandResult == CommandResult.EndRepair == false)
                {
                    commandResult = RequestUserCommand(car, isCarChanged);

                    if (commandResult == CommandResult.ReplaceDetail)
                    {
                        isCarChanged = true;
                    }

                    CarServing?.Invoke(car, isCarChanged);
                }
            }

            ServiceEnded?.Invoke();
        }

        public void TakeCars(List<Car> cars)
        {
            ArgumentNullException.ThrowIfNull(cars);

            _cars.EnqueueRange(cars);
        }

        private void ReplaceDetail(int index, Car car)
        {
            UserUtilits.HandleIndexExeptions(car.Details, index);
            IDetail sample = car.Details[index];

            if (HaveDetail(index, car) == false)
            {
                throw new ArgumentException($"Storage is not have detail \"{sample.Name}\"");
            }

            if (sample.IsBroken)
            {
                Money += sample.Price + WorkPrice;
            }

            car.ReplaceDetail(index, _storage.GiveDetail(sample));
        }

        private void EndRepair(ICar car, bool isChanged)
        {
            if (car.IsBroken())
            {
                int penality = default;

                if (isChanged)
                {
                    penality = GetDynamicPenality(car);
                }
                else
                {
                    penality = FixedPenality;
                }

                Money -= penality;
            }
        }

        public bool HaveDetail(int index, ICar car)
        {
            UserUtilits.HandleIndexExeptions(car.Details, index);
            return _storage.HaveDetail(car.Details[index]);
        }

        public int GetDynamicPenality(ICar car)
        {
            int penality = default;

            for (int i = 0; i < car.Details.Count; i++)
            {
                if (car.Details[i].IsBroken == true)
                {
                    penality += (int)(car.Details[i].Price * DynamicPenalityMultiplyer);
                }
            }

            return penality;
        }

        private CommandResult RequestUserCommand(Car car, bool isChanged)
        {
            RequestingCommand?.Invoke();
            string command = Console.ReadLine();

            if (command == CommandEndRepair)
            {
                EndRepair(car, isChanged);
                return CommandResult.EndRepair;
            }

            if (command == string.Empty)
            {
                CommandFailed?.Invoke(command, ServiceError.Empty);
                return CommandResult.Error;
            }

            bool isIndex = int.TryParse(command, out int index);
            bool isCorrectIndex = isIndex && (index >= 0 && index < car.Details.Count);
            bool isHaveDetail = isIndex && isCorrectIndex && HaveDetail(index, car);

            if (isIndex == false)
            {
                CommandFailed?.Invoke(command, ServiceError.UncnownCommand);
                return CommandResult.Error;
            }

            if (isCorrectIndex == false)
            {
                CommandFailed?.Invoke(command, ServiceError.OutOfRange);
                return CommandResult.Error;
            }

            if (isHaveDetail == false)
            {
                CommandFailed?.Invoke(command, ServiceError.CantReplace);
                return CommandResult.Error;
            }

            ReplaceDetail(index, car);
            return CommandResult.ReplaceDetail;
        }

        private enum CommandResult
        {
            Error,
            ReplaceDetail,
            EndRepair
        }
    }

    public class CarServiceFactory
    {
        private readonly StorageFactory _storageFactory;
        private readonly CarFactory _carFactory;
        private readonly int _detailsCount;

        public CarServiceFactory(List<DetailFactory> detailFactorys, int detailsCount)
        {
            ArgumentNullException.ThrowIfNull(detailFactorys);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(detailsCount);

            _carFactory = new(detailFactorys);
            _storageFactory = new(detailFactorys);
            _detailsCount = detailsCount;
        }

        public CarService Create(int startMoney, int fixedPenality, float dynamicPenalityMultiplyer, int workPrice)
        {
            Storage storage = _storageFactory.Create(_detailsCount);
            return new CarService(storage, startMoney, fixedPenality, dynamicPenalityMultiplyer, workPrice);
        }
    }

    public class CarServiceView
    {
        private readonly ICarService _carService;
        private readonly ViewColors _colors;

        public CarServiceView(CarService carService, ViewColors colors)
        {
            ArgumentNullException.ThrowIfNull(carService);

            _carService = carService;
            _colors = colors;
            _carService.ServiceStarting += Subscribe;
            _carService.ServiceEnded += Unsubscribe;
        }

        private void ShowCurrentStatus(ICar car, bool isCarChanged)
        {
            Console.ForegroundColor = _colors.Default;
            Console.Clear();
            Console.Write($"Деньги: ");
            UserUtilits.WriteWithColor($"{_carService.Money}\n", _colors.Money);
            Console.WriteLine();
            ShowCarDetails(car);
            Console.WriteLine();
            ShowInputInfo();
            Console.WriteLine();
            ShowPriceInfo();
            Console.WriteLine();
            ShowEndRepairInfo(car, isCarChanged);
        }

        private void ShowCarDetails(ICar car)
        {
            ArgumentNullException.ThrowIfNull(car);

            string separation = " ";

            for (int i = 0; i < car.Details.Count; i++)
            {
                Console.Write("№" + i + separation);
                Console.Write(car.Details[i].Name + separation);
                UserUtilits.WriteWithColor($"({car.Details[i].Price})", _colors.Money);
                Console.Write(separation);

                if (car.Details[i].IsBroken)
                {
                    UserUtilits.WriteWithColor("Сломана", _colors.Bad);
                }
                else
                {
                    UserUtilits.WriteWithColor("Исправна", _colors.Good);
                }

                Console.Write(separation);

                if (_carService.HaveDetail(i, car))
                {
                    UserUtilits.WriteWithColor("Есть в наличии", _colors.Good);
                }
                else
                {
                    UserUtilits.WriteWithColor("Нет в наличии", _colors.Bad);
                }

                Console.WriteLine();
            }
        }

        private void ShowInputInfo()
        {
            Console.WriteLine("Для ремонта выберите номер детали");
            Console.Write($"Если вы хотите завершить ремонт введите ");
            UserUtilits.WriteWithColor($"{_carService.CommandEndRepair}\n", _colors.Command);
        }

        public void ShowPriceInfo()
        {
            Console.Write("За замену сломаной детали вы ");
            UserUtilits.WriteWithColor($"получите оплату ", _colors.Good);
            Console.Write("в размере: ");
            UserUtilits.WriteWithColor($"цена детали + {_carService.WorkPrice}\n", _colors.Money);
        }

        private void ShowEndRepairInfo(ICar car, bool isCarChanged)
        {
            ArgumentNullException.ThrowIfNull(car);

            int penality = default;
            int percentDecimals = 2;
            float percent = UserUtilits.ConvertToPercent(_carService.DynamicPenalityMultiplyer);
            percent = UserUtilits.Round(percent, percentDecimals);

            if (car.IsBroken())
            {
                Console.Write("Если завершите ремонт сейчас, вы ");
                UserUtilits.WriteWithColor("заплатите ", _colors.Bad);

                if (isCarChanged)
                {
                    UserUtilits.WriteWithColor("штраф ", _colors.Bad);
                    UserUtilits.WriteWithColor($"{percent}% ", _colors.Money);
                    UserUtilits.WriteWithColor("от цены каждой не починеной детали ", _colors.Bad);
                    penality = _carService.GetDynamicPenality(car);
                }
                else
                {
                    UserUtilits.WriteWithColor("фиксированый штраф ", _colors.Bad);
                    penality = _carService.FixedPenality;
                }

                Console.Write($"в размере: ");
                UserUtilits.WriteWithColor($"{penality}", _colors.Money);
            }
            else
            {
                Console.Write("Машина ");
                UserUtilits.WriteWithColor("исправна. ", _colors.Good);
                Console.Write("Вы можете завершить ремонт.");
            }
        }

        public void Subscribe()
        {
            _carService.CarServing += ShowCurrentStatus;
            _carService.RequestingCommand += ShowRequestingMessage;
            _carService.CommandFailed += ShowError;
        }

        public void Unsubscribe()
        {
            _carService.CarServing -= ShowCurrentStatus;
            _carService.RequestingCommand -= ShowRequestingMessage;
            _carService.CommandFailed -= ShowError;
            _carService.ServiceStarting -= Subscribe;
            _carService.ServiceEnded -= Unsubscribe;
        }

        private void ShowError(string command, ServiceError errorType)
        {
            Console.ForegroundColor = _colors.Default;

            if (errorType == ServiceError.Empty)
            {
                Console.WriteLine("Вы ничего не ввели");
                Console.ReadKey();
            }

            if (errorType == ServiceError.UncnownCommand)
            {
                UserUtilits.WriteWithColor(command, _colors.Command);
                UserUtilits.WriteWithColor(" не является коректным числом или командой", _colors.Default);
                Console.ReadKey();
                return;
            }

            if (errorType == ServiceError.OutOfRange)
            {
                UserUtilits.WriteWithColor("у машины нет детали под номером ", _colors.Default);
                UserUtilits.WriteWithColor($"{command}", _colors.Command);
                Console.ReadKey();
                return;
            }

            if (errorType == ServiceError.CantReplace)
            {
                UserUtilits.WriteWithColor("На складе нет детали под номером ", _colors.Default);
                UserUtilits.WriteWithColor($"{command}", _colors.Command);
                Console.ReadKey();
                return;
            }
        }

        private void ShowRequestingMessage()
        {
            Console.WriteLine();
            Console.Write("Ваш ввод: ");
            Console.ForegroundColor = _colors.Command;
        }
    }

    public readonly struct ViewColors
    {
        public ViewColors(ConsoleColor command, ConsoleColor money, ConsoleColor good, ConsoleColor bad)
        {
            Command = command;
            Money = money;
            Good = good;
            Bad = bad;
        }

        public ConsoleColor Default { get; } = ConsoleColor.Gray;
        public ConsoleColor Command { get; }
        public ConsoleColor Money { get; }
        public ConsoleColor Good { get; }
        public ConsoleColor Bad { get; }
    }

    public enum ServiceError
    {
        Empty,
        UncnownCommand,
        OutOfRange,
        CantReplace
    }

    public interface IDetail
    {
        public string Name { get; }

        public int Price { get; }

        public bool IsBroken { get; }

        public bool Equals(IDetail detail);
    }

    public class Detail : IDetail
    {
        public Detail(string name, int price)
        {
            Name = name;
            Price = price;
            IsBroken = false;
        }

        public string Name { get; }

        public int Price { get; }

        public bool IsBroken { get; private set; }

        public void Break()
        {
            IsBroken = true;
        }

        public bool Equals(IDetail detail)
        {
            bool isEquals = (detail == null) == false && detail.Name == Name && detail.Price == Price;
            return isEquals;
        }
    }

    public class DetailFactory
    {
        private readonly string _name;
        private readonly int _price;

        public DetailFactory(string name, int price)
        {
            _name = name;
            _price = price;
        }

        public Detail Create()
        {
            return new Detail(_name, _price);
        }
    }

    public class Storage
    {
        private List<IDetail> _details;

        public Storage(List<IDetail> details)
        {
            ArgumentNullException.ThrowIfNull(details);

            if (details.Any(detail => detail.IsBroken))
            {
                throw new ArgumentException("Storage can't have broken details");
            }

            _details = details;
        }

        public bool HaveDetail(IDetail sample)
        {
            ArgumentNullException.ThrowIfNull(sample);

            return DetailUtilits.TryGetDetail(_details, sample, out IDetail _);
        }

        public IDetail GiveDetail(IDetail sample)
        {
            ArgumentNullException.ThrowIfNull(sample);

            if (DetailUtilits.TryGetDetail(_details, sample, out IDetail detail) == false)
            {
                throw new ArgumentException("Storage not have detail");
            }

            _details.Remove(detail);
            return detail;
        }
    }

    public class StorageFactory
    {
        private readonly List<DetailFactory> _factorys;

        public StorageFactory(List<DetailFactory> detailSamples)
        {
            _factorys = detailSamples;
        }

        public Storage Create(int detailsCount)
        {
            List<IDetail> details = new();

            for (int i = 0; i < detailsCount; i++)
            {
                foreach (var factory in _factorys)
                {
                    details.Add(factory.Create());
                }
            }

            return new Storage(details);
        }
    }
}
