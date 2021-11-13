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

            //GenWalls();
        }

        public void GenWalls()
        {
            Random rn = new Random();

            #region Horizonatal
            int y = rn.Next() % (Console.WindowHeight-1);
            int len = rn.Next(10, 30);
            int begin = rn.Next(10, Console.WindowWidth-1) % (Console.WindowWidth-1);
            for (int i = begin; i < begin+len; i++)
            {
                Point p = new Point(i, y, ch);
                p.Draw();
                wall.Add(p);
            }
            #endregion

            #region Vertical
            int x = rn.Next(2, Console.WindowHeight-1) % (Console.WindowHeight-1);
            len = rn.Next(10, 30);
            begin = rn.Next(10, Console.WindowHeight-1) % Console.WindowHeight-1;
            for (int i = begin; i < begin+len; i++)
            {
                Point p = new Point(x, i, ch);
                p.Draw();
                wall.Add(p);
            }
            #endregion

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
    }// class Walls

    public class Bot
    {
        public void Move(Snake snake, FoodFactory foodFactory)
        {
            // основное действие - осуществление поворота
            // 1. проверка на необходимость поворота (либо движение к еде, либо уход от столкновения)
            // 2. выбор направления поворота

            int dx = snake.head.x - foodFactory.food.x;
            int dy = snake.head.y - foodFactory.food.y;

            if (dx == 0)
            {
                // x-ы равны
                // свожу y                       
                if (dy < 0 && snake.direction != Direction.DOWN)
                {

                    if (snake.direction != Direction.UP && snake.direction != Direction.DOWN)
                        snake.Rotation(ConsoleKey.DownArrow);
                    else
                    {
                        if (dx < 0)
                            snake.Rotation(ConsoleKey.RightArrow);
                        else
                            snake.Rotation(ConsoleKey.LeftArrow);
                    }
                }
                else if (dy > 0 && snake.direction != Direction.UP)
                {
                    if (snake.direction != Direction.UP && snake.direction != Direction.DOWN)
                        snake.Rotation(ConsoleKey.UpArrow);
                    else
                    {
                        if (dx < 0)
                            snake.Rotation(ConsoleKey.RightArrow);
                        else
                            snake.Rotation(ConsoleKey.LeftArrow);
                    }
                }
            }
            else
            {
                if (dx > 0 && snake.direction != Direction.LEFT)
                {
                    if (snake.direction != Direction.RIGHT && snake.direction != Direction.LEFT)
                        snake.Rotation(ConsoleKey.LeftArrow);
                    else
                    {
                        if (dy < 0)
                            snake.Rotation(ConsoleKey.DownArrow);
                        else
                            snake.Rotation(ConsoleKey.UpArrow);
                    }

                }
                else if (dx < 0 && snake.direction != Direction.RIGHT)
                {
                    if (snake.direction != Direction.RIGHT && snake.direction != Direction.LEFT)
                        snake.Rotation(ConsoleKey.RightArrow);
                    else
                    {
                        if (dy > 0)
                            snake.Rotation(ConsoleKey.DownArrow);
                        else
                            snake.Rotation(ConsoleKey.UpArrow);
                    }
                }
            }

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
        static Bot bot;
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
            bot = new Bot();

            time = new Timer(Loop, null, 0, speed);

            while (true)
            {
                //bot.Move(snake, foodFactory);
                if (game_over)
                    break;
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    snake.Rotation(key.Key);
                }

            }
            Console.SetCursorPosition(30, 30);
            Console.Write("████─████─█───█─███   ████─█─█─███─████");
            Console.SetCursorPosition(30, 31);
            Console.Write("█────█──█─██─██─█     █──█─█─█─█───█──█");
            Console.SetCursorPosition(30, 32);
            Console.Write("█─██─████─█─█─█─███   █──█─█─█─███─████");
            Console.SetCursorPosition(30, 33);
            Console.Write("█──█─█──█─█───█─█     █──█─███─█───█─█");
            Console.SetCursorPosition(30, 34);
            Console.Write("████─█──█─█───█─███   ████──█──███─█─█");
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
            {
                snake.Move();
            }
        }
    }
}
