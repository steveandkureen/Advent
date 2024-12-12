


using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Formats.Asn1;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata;
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
    public const int Blinks = 75;

    public class Plot
    {
        public string Plant { get; set; } = string.Empty;
        public int Count { get; set; } = 1;
        public List<Point> Plots { get; set; } = new List<Point>();

        public void BuildMap(List<List<string>> map)
        {

        }

        internal bool IsAjacent(Point pos)
        {
            if (Plots.Contains(new Point(pos.X - 1, pos.Y)) || Plots.Contains(new Point(pos.X, pos.Y - 1)))
            {
                return true;
            }
            return false;
        }

        internal void CalculateParimeter()
        {
            Plots.OrderBy(a => a.Y).ThenBy(a => a.X).ToList();
        }
    }
    private static void Main(string[] args)
    {

        string data = "";
        using (var reader = new StreamReader("C:\\Users\\steve\\Documents\\projects\\advent\\input.txt"))
        {
            data = reader.ReadToEnd();
        }
        var map = ParseMap(data);

        var plots = new Dictionary<string, List<Plot>>();

        for (var y = 0; y < map.Count; y++)
        {
            for (var x = 0; x < map[y].Count; x++)
            {
                var pos = new Point(x, y);
                GetPlot(pos, plots, map);
            }
        }

        CalculateParimeter(plots);

        CheckPlots(plots, map);

        Console.WriteLine("Answer: " + OutputPlots(plots));
    }

    private static void CalculateParimeter(Dictionary<string, List<Plot>> plots)
    {
        foreach (var plant in plots.Keys)
        {
            foreach (var plot in plots[plant])
            {
                plot.CalculateParimeter();
            }
        }
    }

    private static void CheckPlots(Dictionary<string, List<Plot>> plots, List<List<string>> map)
    {
        foreach (var plant in plots.Keys)
        {
            foreach (var plot in plots[plant])
            {
                foreach (var p in plot.Plots)
                {
                    if (map[p.Y][p.X] != plant)
                    {
                        Console.WriteLine($"{plant}!={map[p.Y][p.X]},pos: {p.Y},{p.X}");
                    }
                }
                if (plot.Plots.Distinct().Count() != plot.Plots.Count)
                {
                    Console.WriteLine($"{plant} has duplicate");
                    foreach (var p in plot.Plots)
                    {
                        Console.WriteLine($"{plant} pos: {p.Y},{p.X}");
                    }
                }
            }
        }
    }

    private static string OutputPlots(Dictionary<string, List<Plot>> plots)
    {
        var build = new StringBuilder();
        foreach (var plants in plots.Keys)
        {
            build.Append($"{plants}: ");
            foreach (var plot in plots[plants])
            {
                build.Append($"{plot.Plots.Count}, ");
            }
            build.Append("\n");
        }
        return build.ToString();
    }

    private static Plot GetPlot(Point pos, Dictionary<string, List<Plot>> plots, List<List<string>> map)
    {
        var plant = map[pos.Y][pos.X];
        Plot? plot = null;
        if (plots.ContainsKey(plant))
        {
            foreach (var p in plots[plant])
            {
                if (p.IsAjacent(pos))
                {
                    plot = p;
                    p.Plots.Add(pos);
                    break;
                }
            }

            if (plot == null)
            {
                plot = new Plot() { Plant = plant };
                plot.Plots.Add(pos);
                plots[plant].Add(plot);
                return plot;
            }

            foreach (var p in plots[plant])
            {
                if (p == plot)
                    continue;

                if (p.Plots.Any(pl => plot.IsAjacent(pl)))
                {
                    p.Plots.ForEach(pl => plot.Plots.Add(pl));
                    plots[plant].Remove(p);
                    return plot;
                }
            }

            return plot;
        }

        plot = new Plot() { Plant = plant };
        plots.Add(plant, new List<Plot>([plot]));
        plot.Plots.Add(pos);

        return plot;
    }

    private static List<List<string>> ParseMap(string data)
    {
        var map = new List<List<string>>();
        var y = 0;
        foreach (var line in data.Split("\n"))
        {
            map.Add(new List<string>());
            foreach (var c in line.Trim().ToCharArray())
            {
                map[y].Add(c.ToString());
            }
            y++;
        }
        return map;
    }
}

