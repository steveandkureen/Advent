


using System.Collections.Immutable;
using System.ComponentModel;
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
    public class Machine
    {
        public DoublePoint A { get; set; } = new DoublePoint();
        public DoublePoint B { get; set; } = new DoublePoint();
        public DoublePoint Prize { get; set; } = new DoublePoint();
    }

    public class DoublePoint
    {
        public DoublePoint()
        {

        }

        public DoublePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public bool IsEqual(DoublePoint b)
        {
            return (X == b.X && Y == b.Y);
        }
    }


    const long Max_Press = 100;

    private static void Main(string[] args)
    {
        var file = "C:\\Users\\steve\\Documents\\projects\\advent\\final.txt";
        //SolveWithBruteForce(file);
        SolveWithMath(file);
    }

    private static void SolveWithBruteForce(string file)
    {
        string data = "";
        using (var reader = new StreamReader(file))
        {
            data = reader.ReadToEnd();
        }
        var machines = ParseMap(data);

        long totalCost = 0;
        var machineCount = 0;
        foreach (var machine in machines)
        {
            var attempt = new List<long>();
            for (long a = 0; a < Max_Press; a++)
            {
                for (long b = 0; b < Max_Press; b++)
                {
                    var x = a * machine.A.X;
                    var y = a * machine.A.Y;

                    x += b * machine.B.X;
                    y += b * machine.B.Y;
                    if (x == machine.Prize.X && y == machine.Prize.Y)
                    {
                        attempt.Add((a * 3) + b);
                        Console.WriteLine($"Machine{machineCount}=a:{a},b:{b}={machine.Prize.X},{machine.Prize.Y}={a * machine.A.X + b * machine.B.X}, {a * machine.A.Y + a * machine.B.Y}");
                    }
                }
            }
            if (attempt.Count > 0)
            {
                totalCost += attempt.Min();
            }
            machineCount++;
        }
        Console.WriteLine("Answer: " + totalCost);
    }

    private static void SolveWithMath(string file)
    {
        string data = "";
        using (var reader = new StreamReader(file))
        {
            data = reader.ReadToEnd();
        }
        var machines = ParseMap(data);

        double totalCost = 0;
        long machineCount = 0;
        foreach (var machine in machines)
        {
            var detM = Determinant(machine.A.X, machine.B.X, machine.A.Y, machine.B.Y);
            if (detM == 0)
            {
                continue;
            }

            var detA = Determinant(machine.Prize.X, machine.B.X, machine.Prize.Y, machine.B.Y);
            var detB = Determinant(machine.A.X, machine.Prize.X, machine.A.Y, machine.Prize.Y);

            double a = detA / detM;
            double b = detB / detM;

            if (CalculateMatch(double.Round(a), double.Round(b), machine, machineCount))
            {
                totalCost += (a * 3) + b;
            }

            machineCount++;
        }
        Console.WriteLine("Answer: " + totalCost);
    }

    private static void OutputMachine(long machineCount, Machine machine, double a, double b)
    {
        Console.WriteLine($"Machine{machineCount}=a:{a},b:{b}={machine.Prize.X},{machine.Prize.Y}={a * machine.A.X + b * machine.B.X}, {a * machine.A.Y + a * machine.B.Y}");
    }

    private static bool CalculateMatch(double a, double b, Machine machine, long machineCount)
    {
        double x = a * machine.A.X + b * machine.B.X;
        double y = a * machine.A.Y + b * machine.B.Y;
        var newVector = new DoublePoint(x, y);
        if (newVector.IsEqual(machine.Prize))
        {
            OutputMachine(machineCount, machine, double.Round(a), double.Round(b));
            return true;
        }
        else
            return false;
    }

    private static double Determinant(double a, double b, double c, double d)
    {
        return a * d - b * c;
    }

    private static List<Machine> ParseMap(string data)
    {
        var machines = new List<Machine>();
        var lines = data.Split("\n");
        for (var i = 0; i < lines.Length; i++)
        {
            var buttonA = lines[i++];
            var buttonB = lines[i++];
            var prize = lines[i++];
            var machine = new Machine();

            machine.A = ParseButton(buttonA.Trim());
            machine.B = ParseButton(buttonB.Trim());
            machine.Prize = ParsePrize(prize);
            machines.Add(machine);
        }
        return machines;
    }

    private static DoublePoint ParsePrize(string prize)
    {                                                                                                            //3345000000 
        var x = long.Parse(prize.Substring(prize.IndexOf('=') + 1, prize.IndexOf(',') - prize.IndexOf('=') - 1)) + 10000000000000;
        prize = prize.Substring(prize.IndexOf(',') + 1, prize.Length - prize.IndexOf(',') - 1);
        var y = long.Parse(prize.Substring(prize.IndexOf('=') + 1, prize.Length - prize.IndexOf('=') - 1)) + 10000000000000;
        return new DoublePoint(x, y);
    }

    private static DoublePoint ParseButton(string button)
    {
        var x = int.Parse(button.Substring(button.IndexOf('+') + 1, button.IndexOf(',') - button.IndexOf('+') - 1));
        button = button.Substring(button.IndexOf(',') + 1, button.Length - button.IndexOf(',') - 1);
        var y = int.Parse(button.Substring(button.IndexOf('+') + 1, button.Length - button.IndexOf('+') - 1));
        return new DoublePoint(x, y);
    }
}

