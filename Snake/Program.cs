using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;


namespace Snake
{
    public struct Point
    {
        public int x;
        public int y;

        public char ch;

        public Point(int x, int y, char ch)
        {
            this.x = x;
            this.y = y;
            this.ch = ch;   
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return ((p1.x != p2.x) || (p1.y != p2.y));
        }

        public static bool operator==(Point p1, Point p2)
        {
            return ((p1.x == p2.x) && (p1.y == p2.y));
        }

        public void DrawPoint(char ch)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(ch);
        }

        public void Draw()
        {
            DrawPoint(ch);
        }

        public void Clear()
        {
            DrawPoint(' ');
        }
    }

    public enum Direction
    { 
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public class Snake
    {

        public List<Point> snake;

        public Direction direction;

        public Point head;
        public Point tail;

        bool rotate = true;

        public Snake(int x, int y, int length)
        {
            direction = Direction.RIGHT;

            snake = new List<Point>();

            for (int i = x - length; i < x; i++)
            {
                Point p = new Point(i, y, '*');
                snake.Add(p);
                p.Draw();
            }
        }

        public bool IsFood()
        {
            return false;
        }

        public void Move()
        {
            head = GetNextPoint();
            snake.Add(head);

            tail = snake[0];
            snake.Remove(tail);

            head.Draw();
            tail.Clear();

            rotate = true;
        }

        public Point GetNextPoint()
        {
            int ind = snake.Count - 1;
            Point next = snake[ind];
          
            switch (direction)
            {
                case Direction.UP:
                    next.y--;
                    break;
                case Direction.RIGHT:
                    next.x++;
                    break;
                case Direction.DOWN:
                    next.y++;
                    break;
                case Direction.LEFT:
                    next.x--;
                    break;

            }
            return next;
        }

        public void Rotation(ConsoleKey key)
        {
            if (rotate)
            {
                switch (direction)
                {
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        if (key == ConsoleKey.S || key == ConsoleKey.DownArrow)
                            direction = Direction.DOWN;
                        else if (key == ConsoleKey.W || key == ConsoleKey.UpArrow)
                            direction = Direction.UP;
                        break;


                    case Direction.UP:
                    case Direction.DOWN:
                        if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow)
                            direction = Direction.LEFT;
                        else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow)
                            direction = Direction.RIGHT;
                        break;
                }
                rotate = false;
            }
        }

        public bool Eat(Point p)
        {
            head = GetNextPoint();
            if (head == p)
            {
                snake.Add(head);
                head.Draw();
                return true;
            }
            return false;
        }

        public bool IsHit(Point p)
        {
            for (int i = snake.Count - 2; i > 0; i--)
            {
                if (snake[i] == p)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class FoodFactory
    {
        int x;
        int y;
        char ch;
        public Point food { get; private set; }

        Random random = new Random();

        public FoodFactory(int x, int y, char ch)
        {
            this.x = x;
            this.y = y;
            this.ch = ch;
        }

        public void CreateFood(Snake snake)
        {
            // чтоб еда не создалась в змее
            bool iss_snake = false;
            do
            {
                food = new Point(random.Next(2, x - 2), random.Next(2, y - 2), ch);
                foreach (var sn_p in snake.snake)
                {
                    if (sn_p == food)
                    {
                        iss_snake = true;
                        break;
                    }
                }
            } while (iss_snake);
           
            food.Draw();
        }
    }

    class Walls
    {
        private char ch;
        private List<Point> wall = new List<Point>();

        public Walls(int x, int y, char ch)
        {
            this.ch = ch;

            DrawHorizontal(x, 0);
            DrawHorizontal(x, y);
            DrawVertical(0, y);
            DrawVertical(x, y);
        }
        
        private void DrawHorizontal(int x, int y)
        {
            for (int i = 0; i < x; i++)
            {
                Point p = new Point(i, y, ch);
                p.Draw();
                wall.Add(p);
            }
        }

        private void DrawVertical(int x, int y)
        {
            for (int i = 0; i < y; i++)
            {
                Point p = new Point(x, i, ch);
                p.Draw();
                wall.Add(p);
            }
        }

        public bool IsHit(Point p)
        {
            foreach (var w in wall)
            {
                if (p == w)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class Game
    {
        static readonly int x = Console.WindowWidth;
        static readonly int y = Console.WindowHeight;

        static Snake snake;

        static FoodFactory foodFactory;

        static Walls walls;

        static Timer time;

        static bool game_over = false;

        static int speed = 100;
        static void Main()
        {
            Console.SetWindowSize(x + 1, y + 1);
            Console.SetBufferSize(x + 1, y + 1);
            Console.CursorVisible = false;

           
            snake = new Snake(x / 2, y / 2, 3);
            walls = new Walls(x, y, '#');
            foodFactory = new FoodFactory(x, y, '@');
            foodFactory.CreateFood(snake);

            time = new Timer(Loop, null, 0, speed);

            while (true)
            {
                if (game_over)
                    break;
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    snake.Rotation(key.Key);
                }
            }
            Console.ReadLine();
        }

        public static void Loop(Object o)
        {
            if (walls.IsHit(snake.snake[snake.snake.Count-1]) || snake.IsHit(snake.snake[snake.snake.Count - 1]))
            {
                time.Change(0, Timeout.Infinite);
                game_over = true;
            }
            else if (snake.Eat(foodFactory.food))
            {
                foodFactory.CreateFood(snake);
                //speed = (int)speed - (int)(speed * 0.08);
                time.Change(0, speed);
            }
            else
                snake.Move();
        }
    }
}
