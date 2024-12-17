


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
    const char WALL = '#';
    const char EMPTY = '.';
    const char START = 'S';
    const char END = 'E';


    const int NORTH = 0;
    const int SOUTH = 1;
    const int EAST = 2;
    const int WEST = 3;

    static List<List<char>> Map = new List<List<char>>();
    static List<Answer> Answers = new List<Answer>();

    public class WalkerPosition
    {
        public Point Position { get; set; }
        public int Direction { get; set; }

    }

    private static void Main(string[] args)
    {
        var file = "C:\\Users\\steve\\Documents\\projects\\advent\\final.txt";
        string data = "";
        using (var reader = new StreamReader(file))
        {
            data = reader.ReadToEnd();
        }

        Map = ParseMapData(data);
        OutputMap(Map);
        var start = FindPosition(START);
        var end = FindPosition(END);
        var bestAnswer = AStar(new WalkerPosition { Position = start, Direction = EAST }, Map, end);
        if (bestAnswer == null)
            return;

        var answers = new List<Point>(bestAnswer.Path);
        var answer = FindNewPath(bestAnswer, data, bestAnswer, start, end, answers);

        Console.WriteLine("Answer: " + (answers.Count + 1));
    }

    private static int ShowFinalAnswer(List<Point> points)
    {
        for (int y = 0; y < Map.Count; y++)
        {
            List<char>? line = Map[y];
            for (int x = 0; x < line.Count; x++)
            {
                char pos = line[x];
                var p = new Point(x, y);
                if (points.Contains(p) || pos == START)
                {
                    Console.Write('O');
                }
                else
                {
                    Console.Write(pos);
                }

            }
            Console.Write("\n");
        }
        return points.Count;
    }

    private static int FindNewPath(Answer currentAnswer, string mapData, Answer bestAnswer, Point start, Point end, List<Point> totalList)
    {
        var points = new Stack<Point>(currentAnswer.Path);
        currentAnswer.Path.ForEach(p => points.Push(p));

        while (points.Count > 0)
        {
            var pos = points.Pop();
            if (pos == start || pos == end)
                continue;
            var currentMap = ParseMapData(mapData);
            currentMap[pos.Y][pos.X] = WALL;
            var newAnswer = AStar(new WalkerPosition { Position = start, Direction = EAST }, currentMap, end);
            //OutputMap(currentMap);
            if (newAnswer != null && newAnswer.Cost == bestAnswer.Cost)
            {
                //OutputMap(currentMap);
                var newMap = SerializeMap(currentMap);
                var added = false;
                newAnswer.Path.ForEach(p =>
                {
                    if (!totalList.Contains(p))
                    {
                        added = true;
                        totalList.Add(p);
                    }
                });
                if (added)
                {
                    FindNewPath(newAnswer, newMap, bestAnswer, start, end, totalList);
                }
            }
        }
        ShowFinalAnswer(totalList);
        return totalList.Count + 1;
    }

    private static string SerializeMap(List<List<char>> map)
    {
        var builder = new StringBuilder();
        foreach (var line in map)
        {
            foreach (var c in line)
            {
                builder.Append(c);
            }
            builder.Append('\n');
        }
        return builder.ToString();
    }

    private static int Heuristic(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private static Answer? AStar(WalkerPosition start, List<List<char>> map, Point end)
    {
        var frontier = new PriorityQueue<WalkerPosition, int>();
        frontier.Enqueue(start, 0);
        var cameFrom = new Dictionary<Point, Point?>();
        var costSoFar = new Dictionary<Point, int>();
        cameFrom[start.Position] = null;
        costSoFar[start.Position] = 0;

        while (frontier.Count != 0)
        {
            var current = frontier.Dequeue();

            if (current.Position == end)
            {
                return CalculateAnswer(cameFrom, start, end, costSoFar[end]);
            }

            foreach (var next in GetNeighbors(current, map))
            {
                var newCost = costSoFar[current.Position] + CalculateCost(current, next);

                if (!costSoFar.ContainsKey(next.Position) || newCost < costSoFar[next.Position])
                {
                    costSoFar[next.Position] = newCost;
                    var priority = newCost + Heuristic(end, next.Position);
                    frontier.Enqueue(next, priority);
                    cameFrom[next.Position] = current.Position;
                }
            }
        }
        return null;
    }

    public class Answer
    {
        public List<Point> Path { get; set; } = new List<Point>();
        public int Cost { get; set; }
    }

    private static Answer CalculateAnswer(Dictionary<Point, Point?> cameFrom, WalkerPosition start, Point end, int cost)
    {
        var answer = new Answer();
        Point? current = end;
        while (current != null && cameFrom[current.Value] != null)
        {
            answer.Path.Add(current.Value);
            current = cameFrom[current.Value];
        }
        answer.Cost = cost;
        return answer;
    }

    private static int CalculateCost(WalkerPosition current, WalkerPosition next)
    {
        var cost = 1;
        if (current.Direction == next.Direction)
            return cost;

        if (current.Direction == NORTH || current.Direction == SOUTH)
        {
            if (next.Direction == EAST || next.Direction == WEST)
            {
                cost += 1000;
            }
            else
            {
                cost += 2000;
            }
        }
        else if (current.Direction == WEST || current.Direction == EAST)
        {
            if (next.Direction == NORTH || next.Direction == SOUTH)
            {
                cost += 1000;
            }
            else
            {
                cost += 2000;
            }
        }
        return cost;
    }

    private static List<WalkerPosition> GetNeighbors(WalkerPosition current, List<List<char>> map)
    {
        var pos = current.Position;
        var neighbors = new List<WalkerPosition>();
        CheckNeighbor(current, new Point(pos.X + 1, pos.Y), map, neighbors);
        CheckNeighbor(current, new Point(pos.X - 1, pos.Y), map, neighbors);
        CheckNeighbor(current, new Point(pos.X, pos.Y + 1), map, neighbors);
        CheckNeighbor(current, new Point(pos.X, pos.Y - 1), map, neighbors);
        return neighbors;
    }

    private static void CheckNeighbor(WalkerPosition current, Point point, List<List<char>> map, List<WalkerPosition> neighbors)
    {
        if (map[point.Y][point.X] == EMPTY || map[point.Y][point.X] == END)
        {
            var direction = NORTH;
            if (current.Position.X > point.X)
            {
                direction = WEST;
            }
            else if (current.Position.X < point.X)
            {
                direction = EAST;
            }
            else if (current.Position.Y > point.Y)
            {
                direction = SOUTH;
            }
            else if (current.Position.Y < point.Y)
            {
                direction = NORTH;
            }
            neighbors.Add(new WalkerPosition { Position = point, Direction = direction });
        }
    }

    private static Point FindPosition(char c)
    {
        for (int y = 0; y < Map.Count; y++)
        {
            List<char>? line = Map[y];
            for (int x = 0; x < line.Count; x++)
            {
                if (Map[y][x] == c)
                    return new Point(x, y);
            }
        }
        return new Point();
    }

    private static void OutputMap(List<List<char>> map)
    {
        for (int y = 0; y < map.Count; y++)
        {
            List<char>? line = map[y];
            for (int x = 0; x < line.Count; x++)
            {
                char pos = line[x];
                {
                    Console.Write(pos);
                }

            }
            Console.Write("\n");
        }
    }

    private static List<List<char>> ParseMapData(string data)
    {
        var map = new List<List<char>>();
        foreach (var line in data.Split("\n"))
        {
            var newLine = new List<char>();
            foreach (var c in line.ToCharArray())
            {
                newLine.AddRange([c]);

            }
            map.Add(newLine);
        }
        return map;
    }

}

