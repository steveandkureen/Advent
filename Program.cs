


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
        public LongPoint A { get; set; } = new LongPoint();
        public LongPoint B { get; set; } = new LongPoint();
        public LongPoint Prize { get; set; } = new LongPoint();
    }

    public class LongPoint
    {
        public LongPoint()
        {

        }
        public LongPoint(long x, long y)
        {
            X = x;
            Y = y;
        }
        public long X { get; private set; }
        public long Y { get; private set; }
    }

    const long Max_Press = 100;

    private static void Main(string[] args)
    {

        string data = "";
        using (var reader = new StreamReader("C:\\Users\\steve\\Documents\\projects\\advent\\input.txt"))
        {
            data = reader.ReadToEnd();
        }
        var machines = ParseMap(data);

        long totalCost = 0;
        foreach (var machine in machines)
        {
            var attempt = new List<long>();
            for (long a = 0; a < Max_Press; a++)
            {
                for (long b = 0; b < Max_Press; b++)
                {
                    var x = a * machine.A.X;
                    var y = a * machine.A.Y;
                    if (a == 90 && b == 40)
                        a = 90;
                    x += b * machine.B.X;
                    y += b * machine.B.Y;
                    if (x == machine.Prize.X && y == machine.Prize.Y)
                    {
                        attempt.Add((a * 3) + b);
                    }
                }
            }
            if (attempt.Count > 0)
            {
                totalCost += attempt.Min();
            }
        }
        Console.WriteLine("Answer: " + totalCost);
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

    private static LongPoint ParsePrize(string prize)
    {
        var x = long.Parse(prize.Substring(prize.IndexOf('=') + 1, prize.IndexOf(',') - prize.IndexOf('=') - 1));// + 10000000000000;
        prize = prize.Substring(prize.IndexOf(',') + 1, prize.Length - prize.IndexOf(',') - 1);
        var y = long.Parse(prize.Substring(prize.IndexOf('=') + 1, prize.Length - prize.IndexOf('=') - 1));// + 10000000000000;
        return new LongPoint(x, y);
    }

    private static LongPoint ParseButton(string button)
    {
        var x = int.Parse(button.Substring(button.IndexOf('+') + 1, button.IndexOf(',') - button.IndexOf('+') - 1));
        button = button.Substring(button.IndexOf(',') + 1, button.Length - button.IndexOf(',') - 1);
        var y = int.Parse(button.Substring(button.IndexOf('+') + 1, button.Length - button.IndexOf('+') - 1));
        return new LongPoint(x, y);
    }
}

