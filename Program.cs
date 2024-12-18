


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
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml;
using System.Xml.XPath;


internal class Program
{

    public enum OPCODES
    {
        ADV = 0,
        BXL = 1,
        BST = 2,
        JNZ = 3,
        BXC = 4,
        OUT = 5,
        BDV = 6,
        CDV = 7
    }

    static long RegA = 0;
    static long RegB = 0;
    static long RegC = 0;

    static bool found = false;


    static char[] Instructions = new char[0];


    private static void Main(string[] args)
    {
        var file = "C:\\Users\\steve\\Documents\\projects\\advent\\final.txt";
        string data = "";
        using (var reader = new StreamReader(file))
        {
            data = reader.ReadToEnd();
        }

        ReadInitData(data);
        OutputComputer();

        var tasks = new List<Task>();
        var threadCount = 10;
        var chunk = (281474900000000 - 35185000000000) / threadCount;
        for (var i = 0; i < threadCount; i++)
        {
            long start = 0;
            start = 35185000000000 + (chunk * i);
            Console.WriteLine("Chunck: " + i + " " + start);
            tasks.Add(CreateTask(start, start, RegB, RegC, start + chunk, i));
            //new Task(() => { RunBatch(start, start, RegB, RegC, start + chunk, i); }));
        }
        tasks.ForEach(t => t.Start());
        Task.WaitAll(tasks.ToArray());
        //3372036854775807 - min
        // bool done = false;
        // long chunk = 1;
        // long regA = RegA;
        // long regB = RegA;
        // long regC = RegC;
        // long stop = long.MaxValue;
        // long thread = 0;
        // long currentTry = 10000000000000;
        //10000000000000;
        //1000000000000000
        //min 35185000000000
        //max 281474900000000
        // Console.WriteLine($"Thread Config: {thread}, {currentTry}, {regA}, {regB}, {regC}, {stop}");
        // var output = new List<long>();
        // while (currentTry < stop && !found)
        // {
        //     output.Clear();
        //     RunComputer(currentTry, regB, regC, output);
        //     //Console.WriteLine("Length: " + currentTry + " " + output.Count);
        //     if (output.Count < Instructions.Length)
        //     {
        //         currentTry += 1000000000;
        //         // return;
        //     }
        //     else if (output.Count > Instructions.Length)
        //     {
        //         currentTry -= 100000000;
        //         //Console.WriteLine("Length: " + currentTry);

        //     }
        //     else
        //     {
        //         return;
        //     }
        //     // if (CompareOutput(output))
        //     // {
        //     //     Console.WriteLine(currentTry);
        //     //     return;
        //     // }
        //     //currentTry++;
        //     // if (currentTry % 1000000 == 0)
        //     // {
        //     //     Console.WriteLine("CurrentTry: " + currentTry + " " + thread);
        //     // }
        // }


    }

    private static Task CreateTask(long currentTry, long start, long regB, long regC, long chunk, int i)
    {
        return new Task(() => { RunBatch(currentTry, start, RegB, RegC, start + chunk, i); });
    }

    private static bool RunBatch(Int64 currentTry, long regA, long regB, long regC, long stop, long thread)
    {
        Console.WriteLine($"Thread Config: {thread}, {currentTry}, {regA}, {regB}, {regC}, {stop}");
        var output = new List<long>();
        while (currentTry < stop && !found)
        {
            output.Clear();
            RunComputer(currentTry, regB, regC, output);
            if (CompareOutput(output))
            {
                Console.WriteLine(currentTry);
                return true;
            }
            currentTry++;
            if (currentTry % 1000000 == 0)
            {
                Console.WriteLine("CurrentTry: " + currentTry + " " + thread);
            }
        }
        return false;
    }

    private static bool CompareOutput(List<long> output)
    {
        if (output.Count != Instructions.Length)
            return false;

        for (int i = 0; i < Instructions.Length; i++)
        {
            if (long.Parse([Instructions[i]]) != output[i])
                return false;
        }
        Console.WriteLine("Insructions: " + string.Join(",", Instructions));
        Console.WriteLine("Output: " + string.Join(",", output));
        found = true;
        return true;
    }

    private static void RunComputer(long regA, long regB, long regC, List<long> output)
    {
        long currentInstruction = 0;
        while (currentInstruction < Instructions.Length)
        {
            var operand = long.Parse([Instructions[currentInstruction + 1]]);
            var instruction = (OPCODES)long.Parse([Instructions[currentInstruction]]);
            switch (instruction)
            {
                case OPCODES.ADV:
                    regA /= (long)Math.Pow(2, GetComboOperand(operand, regA, regB, regC));
                    break;
                case OPCODES.BXL:
                    regB = regB ^ operand;
                    break;
                case OPCODES.BST:
                    regB = GetComboOperand(operand, regA, regB, regC) % 8;
                    break;
                case OPCODES.JNZ:
                    if (regA != 0)
                    {
                        currentInstruction = operand;
                        continue;
                    }
                    break;
                case OPCODES.BXC:
                    regB = regB ^ regC;
                    break;
                case OPCODES.OUT:
                    output.Add(GetComboOperand(operand, regA, regB, regC) % 8);
                    break;
                case OPCODES.BDV:
                    regB = regA / (long)Math.Pow(2, GetComboOperand(operand, regA, regB, regC));
                    break;
                case OPCODES.CDV:
                    regC = regA / (long)Math.Pow(2, GetComboOperand(operand, regA, regB, regC));
                    break;
            }
            currentInstruction += 2;
        }
    }

    private static long GetComboOperand(long operand, long regA, long regB, long regC)
    {
        switch (operand)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                return operand;
            case 4:
                return regA;
            case 5:
                return regB;
            case 6:
                return regC;
            case 7:
            default:
                throw new InvalidOperationException("Operand 7 not allowed");
        }
    }

    private static void OutputComputer()
    {
        Console.WriteLine("Register A: " + RegA);
        Console.WriteLine("Register B: " + RegB);
        Console.WriteLine("Register C: " + RegC);

        Console.WriteLine("Program: " + string.Join(',', Instructions));
    }

    private static void ReadInitData(string data)
    {
        var lines = data.Split("\n");
        RegA = ReadRegister(lines[0].Trim());
        RegB = ReadRegister(lines[1].Trim());
        RegC = ReadRegister(lines[2].Trim());

        var instructions = lines[4].Trim();
        instructions = instructions.Substring(instructions.IndexOf(":") + 1).Trim();
        Instructions = instructions.Split(",").Select(c => char.Parse(c)).ToArray();
    }

    private static long ReadRegister(string line)
    {
        long colonIndex = line.IndexOf(":");
        return long.Parse(line.Substring((int)colonIndex + 1, (int)(line.Length - colonIndex - 1)));
    }
}

