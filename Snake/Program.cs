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

        public void DrawPoint(char ch)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(ch);
        }

        public void Draw()
        {
            DrawPoint('*');
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
            Point next = snake[snake.Count - 1];
          
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
                        if (key == ConsoleKey.S)
                            direction = Direction.DOWN;
                        else if (key == ConsoleKey.W)
                            direction = Direction.UP;
                        break;


                    case Direction.UP:
                    case Direction.DOWN:
                        if (key == ConsoleKey.A)
                            direction = Direction.LEFT;
                        else if (key == ConsoleKey.D)
                            direction = Direction.RIGHT;
                        break;
                }
                rotate = false;
            }
        }
    }

    public class Game
    {
        static readonly int x = Console.WindowWidth;
        static readonly int y = Console.WindowHeight;

        static Snake snake;
   
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            snake = new Snake(x / 2, y / 2, 3);

            var time = new Timer( (object o) => snake.Move(), null, 0, 200);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    snake.Rotation(key.Key);
                }
            }

        }
    }
}
