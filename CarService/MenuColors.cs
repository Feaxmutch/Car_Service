using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarService
{
    public struct MenuColors
    {
        public MenuColors(ConsoleColor command, ConsoleColor money, ConsoleColor good, ConsoleColor bad)
        {
            Command = command;
            Money = money;
            Good = good;
            Bad = bad;
        }

        public ConsoleColor Command { get; }
        public ConsoleColor Money { get; }
        public ConsoleColor Good { get; }
        public ConsoleColor Bad { get; }
    }
}
