namespace CarService
{
    public class CarServiceMenu
    {
        private const string CommandEndRepair = "end";
        
        private readonly CarService _carService;
        private readonly MenuColors _colors;

        public CarServiceMenu(CarService carService, MenuColors colors)
        {
            ArgumentNullException.ThrowIfNull(carService);

            _carService = carService;
            _colors = colors;
        }

        public void ServeCars()
        {
            while (_carService.CarsCount > 0)
            {
                Console.Clear();
                ShowCurrentStatus();
                Console.Write("\nВаш ввод: ");
                HandleInput();
            }
        }

        private void ShowCurrentStatus()
        {
            Car car = _carService.CurrentCar;
            Console.Write($"Деньги: ");
            UserUtilits.WriteWithColor($"{_carService.Money}\n", _colors.Money);
            Console.WriteLine();
            ShowCarDetails(car);
            Console.WriteLine();
            ShowInputInfo();
            Console.WriteLine();
            ShowPriceInfo();
            Console.WriteLine();
            ShowEndRepairInfo(car);
        }

        private void ShowCarDetails(Car car)
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

                if (_carService.HaveDetail(i))
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
            UserUtilits.WriteWithColor($"{CommandEndRepair}\n", _colors.Command);
        }

        public void ShowPriceInfo()
        {
            Console.Write("За замену сломаной детали вы ");
            UserUtilits.WriteWithColor($"получите оплату ", _colors.Good);
            Console.Write("в размере: ");
            UserUtilits.WriteWithColor($"цена детали + {_carService.WorkPrice}\n", _colors.Money);
        }

        private void ShowEndRepairInfo(Car car)
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

                if (car.IsChanged)
                {
                    UserUtilits.WriteWithColor("штраф ", _colors.Bad);
                    UserUtilits.WriteWithColor($"{percent}% ", _colors.Money);
                    UserUtilits.WriteWithColor("от цены каждой не починеной детали ", _colors.Bad);
                    penality = _carService.GetDynamicPenality();
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

        private void HandleInput()
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = _colors.Command;
            string command = Console.ReadLine();
            Console.ForegroundColor = defaultColor;

            if (command == CommandEndRepair)
            {
                _carService.EndRepair();
                return;
            }

            if (command == string.Empty)
            {
                Console.WriteLine("Вы ничего не ввели");
                Console.ReadKey();
                return;
            }

            Console.ForegroundColor = defaultColor;
            bool isCorrectCommand = int.TryParse(command, out int index);
            bool isCorrectIndex = isCorrectCommand && (index >= 0 && index < _carService.CurrentCar.Details.Count);
            bool isHaveDetail = isCorrectCommand && isCorrectIndex && _carService.HaveDetail(index);

            if (isCorrectCommand == false)
            {
                UserUtilits.WriteWithColor(command, _colors.Command);
                Console.WriteLine(" не является коректным числом или командой");
                Console.ReadKey();
                return;
            }

            if (isCorrectIndex == false)
            {
                Console.Write($"у машины нет детали под номером ");
                UserUtilits.WriteWithColor($"{index}", _colors.Command);
                Console.ReadKey();
                return;
            }

            if (isHaveDetail == false)
            {
                Console.Write($"На складе нет детали под номером ");
                UserUtilits.WriteWithColor($"{index}", _colors.Command);
                Console.ReadKey();
                return;
            }

            _carService.ReplaceDetail(index);
        }
    }
}
