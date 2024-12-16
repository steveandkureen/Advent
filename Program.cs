


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
    public class Robot
    {
        public Vector2 Pos { get; set; }
        public Vector2 Vel { get; set; }
    }

    const int Map_Width = 101;
    const int Map_Height = 103;
    const int Move_Seconds = 100000;

    static List<string> Maps = new List<string>();

    private static void Main(string[] args)
    {
        var file = "C:\\Users\\steve\\Documents\\projects\\advent\\final.txt";
        string data = "";
        using (var reader = new StreamReader(file))
        {
            data = reader.ReadToEnd();
        }
        var robots = ParseRobots(data);
        OutputMap(robots);
        MoveRobots(robots);
        Console.WriteLine("-------------Complete--------------");
        OutputMap(robots);
        CalculteAnswer(robots);
    }

    private static void CalculteAnswer(List<Robot> robots)
    {
        var quads = new int[4];
        foreach (var robot in robots)
        {
            if (robot.Pos.X < Map_Width / 2 && robot.Pos.Y < Map_Height / 2)
            {
                quads[0]++;
            }
            else if (robot.Pos.X > Map_Width / 2 && robot.Pos.Y < Map_Height / 2)
            {
                quads[1]++;
            }
            else if (robot.Pos.X < Map_Width / 2 && robot.Pos.Y > Map_Height / 2)
            {
                quads[2]++;
            }
            else if (robot.Pos.X > Map_Width / 2 && robot.Pos.Y > Map_Height / 2)
            {
                quads[3]++;
            }
        }
        Console.WriteLine($"Answer={quads[0] * quads[1] * quads[2] * quads[3]}");
    }

    private static void MoveRobots(List<Robot> robots)
    {
        for (var s = 0; s < Move_Seconds; s++)
        {
            for (int i = 0; i < robots.Count; i++)
            {
                var robot = robots[i];
                var newPos = new Vector2(robot.Pos.X + robot.Vel.X, robot.Pos.Y + robot.Vel.Y);
                newPos.X = CheckBoundry(newPos.X, Map_Width);
                newPos.Y = CheckBoundry(newPos.Y, Map_Height);
                robot.Pos = newPos;
            }
            if (CheckForTree(robots.OrderBy(r => r.Pos.X).ThenBy(r => r.Pos.Y).ToList()))
            {
                OutputMap(robots);
                Console.WriteLine($"---seconds:{s}----");
                return;
            }
            if (s % 1000 == 0)
            {
                Console.WriteLine($"---seconds:{s}----");
            }

        }
    }

    private static bool CheckForTree(List<Robot> robots)
    {
        for (int i = 0; i < robots.Count; i++)
        {
            var robot = robots[i];
            if (IsTree(robot, robots, 0))
                return true;
        }
        return false;
    }

    private static bool IsTree(Robot robot, List<Robot> robots, int depth)
    {
        var r1 = robots.Where(r => r.Pos.X == robot.Pos.X - depth && r.Pos.Y == robot.Pos.Y + depth);
        var r2 = robots.Where(r => r.Pos.X == robot.Pos.X + depth && r.Pos.Y == robot.Pos.Y + depth);
        if (r1.Count() > 0 && r1.Count() > 0)
        {
            if (depth > 5)
            {
                return true;
            }
            else
            {
                return IsTree(robot, robots, depth + 1);
            }
        }
        return false;
    }

    private static float CheckBoundry(float p, int max)
    {
        if (p >= max)
        {
            p -= max;
        }
        else if (p < 0)
        {
            p += max;
        }
        return p;
    }

    private static void OutputMap(List<Robot> robots)
    {
        var builder = new StringBuilder();
        for (var y = 0; y < Map_Height; y++)
        {
            for (var x = 0; x < Map_Width; x++)
            {
                var r = robots.Where(r => r.Pos == new Vector2(x, y));
                if (r.Count() == 0)
                {
                    builder.Append(".");
                    //Console.Write(".");
                }
                else
                {
                    builder.Append(r.Count());
                    //Console.Write(r.Count());
                }
            }
            builder.Append("\n");
            //Console.Write("\n");
        }
        Console.Write(builder.ToString());
        //Maps.Add(builder.ToString());
    }

    private static List<Robot> ParseRobots(string data)
    {
        var robots = new List<Robot>();
        foreach (var line in data.Split("\n"))
        {
            var robot = new Robot();
            var parts = line.Trim().Split(" ");
            var pos = parts[0].Replace("p=", "").Split(",");
            robot.Pos = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
            var vel = parts[1].Replace("v=", "").Split(",");
            robot.Vel = new Vector2(float.Parse(vel[0]), float.Parse(vel[1]));
            robots.Add(robot);
        }
        return robots;
    }
}

