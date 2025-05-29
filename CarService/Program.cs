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
                new("Коробка передач", 120),
            };

            MenuColors menuColors = new(
                ConsoleColor.DarkCyan, 
                ConsoleColor.DarkYellow, 
                ConsoleColor.Green, 
                ConsoleColor.Red);

            CarFactory carFactory = new(detailFactoriys);
            CarServiceFactory serviceFactory = new(detailFactoriys, detailsInStorage);
            CarService carService = serviceFactory.Create(startMoney, fixedPenality, dynamicPenalityMultiplyer, workPrice);
            CarServiceMenu menu = new(carService, menuColors);

            carService.TakeCars(carFactory.Create(carsCount));
            menu.ServeCars();
        }
    }
}
