using System;
using System.Windows.Input;

namespace Snake
{
    class Game
    {
        public class Snake
        {

        }

        public Snake sn = new Snake();
        public char[,] field = new char[Console.WindowHeight, Console.WindowWidth];

        public Game()
        {
            for (int i = 0; i < Console.WindowHeight; i++)
                for (int j = 0; j < Console.WindowHeight; j++)
                    this.field[i, j] = ' ';

        }
    
    }
    internal class Program
    {
        

        static void Main(string[] args)
        {

            while(true)
            {
                var key = Console.ReadKey();
            }
            
        }
    }
}
