


using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Formats.Asn1;
using System.IO.Compression;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml;
using System.Xml.XPath;


internal class Program
{
    const char UP = '^';
    const char DOWN = 'v';
    const char RIGHT = '>';
    const char LEFT = '<';
    const char ROBOT = '@';
    const char WALL = '#';
    const char BOX = 'O';
    const char EMPTY = '.';
    const char NEWBOX1 = '[';
    const char NEWBOX2 = ']';
    static List<char> Moves = new List<char>();
    static List<List<char>> Map = new List<List<char>>();

    static bool SimpleBox = false;

    private static void Main(string[] args)
    {
        var file = "C:\\Users\\steve\\Documents\\projects\\advent\\final.txt";
        string data = "";
        using (var reader = new StreamReader(file))
        {
            data = reader.ReadToEnd();
        }

        ParseMapData(data);
        OutputMap();
        var robot = FindRobot();
        var moveCount = 0;
        char lastmove = '-';
        foreach (var move in Moves)
        {
            robot = MoveRobot(robot, move);
            lastmove = move;
            moveCount++;
            if (moveCount == 19998)
            {
                OutputMap();
            }
            //Console.WriteLine("MoveCount: " + moveCount++);
            //OutputMap();
            //Thread.Sleep(100);
        }
        Console.WriteLine("LastMove: " + lastmove);
        OutputMap();
        int boxCount = 0;
        for (int y = 0; y < Map.Count; y++)
        {
            var line = Map[y];
            for (int x = 0; x < line.Count; x++)
            {
                char pos = line[x];
                if (pos == NEWBOX2)
                {
                    boxCount++;
                }
            }
        }
        Console.WriteLine("Box Count: " + boxCount);
        var answer = CalculateResult();
        Console.WriteLine("Answer: " + answer);
    }

    private static object CalculateResult()
    {
        var result = 0;
        var boxCount = 0;
        for (int y = 0; y < Map.Count; y++)
        {
            var line = Map[y];
            var width = Map[y].Count();
            for (int x = 0; x < line.Count; x++)
            {
                char pos = line[x];
                if (pos == BOX || pos == NEWBOX1)
                {
                    result += (100 * y) + x;
                    boxCount++;
                }
            }
        }
        Console.WriteLine("BoxCount in Score: " + boxCount);
        return result;
    }

    private static Point FindRobot()
    {
        for (int y = 0; y < Map.Count; y++)
        {
            var line = Map[y];
            for (int x = 0; x < line.Count; x++)
            {
                char pos = line[x];
                if (pos == ROBOT)
                {
                    return new Point(x, y);
                }
            }
        }
        return new Point();
    }

    private static Point MoveRobot(Point robot, char move)
    {
        var newPos = robot;
        newPos = Move(move, newPos);
        if (CheckMove(newPos, move, false))
        {
            CheckMove(newPos, move, true);
            Map[newPos.Y][newPos.X] = '@';
            Map[robot.Y][robot.X] = '.';
            return newPos;
        }
        return robot;
    }

    private static Point Move(char move, Point currentPos)
    {
        var newPos = currentPos;
        switch (move)
        {
            case UP:
                newPos.Y -= 1;
                break;
            case DOWN:
                newPos.Y += 1;
                break;
            case LEFT:
                newPos.X -= 1;
                break;
            case RIGHT:
                newPos.X += 1;
                break;
        }

        return newPos;
    }

    private static bool CheckMove(Point newPos, char direction, bool doMove)
    {
        switch (Map[newPos.Y][newPos.X])
        {
            case WALL:
                return false;
            case EMPTY:
                return true;
            case BOX:
            case NEWBOX1:
            case NEWBOX2:
                return PushBox(newPos, direction, doMove) ? true : false;
        }
        return false;
    }

    private static bool PushBox(Point newPos, char direction, bool doMove)
    {
        if (SimpleBox)
            return PushBoxHorizontal(newPos, direction, doMove);

        if (direction == UP || direction == DOWN)
        {
            return PushBoxVertical(newPos, direction, doMove);
        }
        return PushBoxHorizontal(newPos, direction, doMove);
    }

    private static bool PushBoxHorizontal(Point newPos, char direction, bool doMove)
    {
        var newBox = Move(direction, newPos);
        if (CheckMove(newBox, direction, doMove))
        {
            if (doMove)
            {
                Map[newBox.Y][newBox.X] = Map[newPos.Y][newPos.X];
            }
            return true;
        }
        return false;
    }

    private static bool PushBoxVertical(Point newPos, char direction, bool doMove)
    {
        var newBox = Move(direction, newPos);
        Point boxLeft, boxRight;
        if (Map[newPos.Y][newPos.X] == ']')
        {
            boxRight = newBox;
            boxLeft = new Point(newBox.X - 1, newBox.Y);
        }
        else
        {
            boxRight = new Point(newBox.X + 1, newBox.Y);
            boxLeft = newBox;
        }

        if (CheckMove(boxRight, direction, doMove) && CheckMove(boxLeft, direction, doMove))
        {
            if (doMove)
            {
                if (Map[newPos.Y][newPos.X] == ']')
                {
                    Map[boxLeft.Y][boxLeft.X] = Map[newPos.Y][newPos.X - 1];
                    Map[boxRight.Y][boxRight.X] = Map[newPos.Y][newPos.X];
                    Map[newPos.Y][newPos.X - 1] = '.';
                    Map[newPos.Y][newPos.X] = '.';
                }
                else
                {
                    Map[boxLeft.Y][boxLeft.X] = Map[newPos.Y][newPos.X];
                    Map[boxRight.Y][boxRight.X] = Map[newPos.Y][newPos.X + 1];
                    Map[newPos.Y][newPos.X] = '.';
                    Map[newPos.Y][newPos.X + 1] = '.';
                }
            }
            return true;
        }
        return false;
    }

    private static void OutputMap()
    {
        foreach (var line in Map)
        {
            foreach (var pos in line)
            {
                Console.Write(pos);
            }
            Console.Write("\n");
        }
    }

    private static void ParseMapData(string data)
    {
        Console.WriteLine("Box Count: " + data.Where(c => c == 'O').Count());
        bool readMap = true;
        foreach (var line in data.Split("\n"))
        {
            if (string.IsNullOrEmpty(line.Trim()))
            {
                readMap = false;
                continue;
            }

            if (readMap)
            {
                var newLine = new List<char>();
                foreach (var c in line.ToCharArray())
                {
                    if (SimpleBox)
                    {
                        newLine.AddRange([c]);
                    }
                    else
                    {
                        newLine.AddRange(TranslateChars(c));
                    }

                }
                Map.Add(newLine);
            }
            else
            {
                Moves.AddRange(line.Trim().ToCharArray());
            }
        }
        int boxCount = 0;
        for (int y = 0; y < Map.Count; y++)
        {
            var line = Map[y];
            for (int x = 0; x < line.Count; x++)
            {
                char pos = line[x];
                if (pos == NEWBOX1)
                {
                    boxCount++;
                }
            }
        }
        Console.WriteLine("Box Count: " + boxCount);
    }

    private static List<char> TranslateChars(char c)
    {
        switch (c)
        {
            case WALL:
                return ['#', '#'];
            case BOX:
                return ['[', ']'];
            case EMPTY:
                return ['.', '.'];
            case ROBOT:
                return ['@', '.'];
        }
        return ['.'];
    }
}

