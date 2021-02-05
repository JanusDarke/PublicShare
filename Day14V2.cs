using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UtilityTools;
using System.IO;

namespace Advent_of_Code_2020
{
    class Day14V2
    {
        static private List<string> input = new List<string>();
        static private Dictionary<double, double> memory = new Dictionary<double, double>();
        static private int inputPosition = 0;
        static private char[] currentMask = new char[36];
        static private char[] currentMemoryAddress = new char[36];
        static private double currentMemoryValue = 0;

        public static void Solve()
        {
            PopulateInput("input14");
            ExecuteNextCommand();
        }

        static void PopulateInput(string aInput)
        {
            //input = Tools.ParseStringToStringList($"{aInput}.txt");
            input = ParseStringToStringList($"{aInput}.txt");            
        }

        static void ExecuteNextCommand()
        {
            double result = 0;
            if (inputPosition < input.Count)
            {
                string currentCommand = input[inputPosition];
                if (currentCommand.Contains("mask"))
                {
                    currentMask = currentCommand[7..].ToCharArray();
                    inputPosition++;
                    ExecuteNextCommand();
                }
                else
                {
                    int currentMemoryAddressInt = int.Parse(currentCommand[4..currentCommand.IndexOf(']')]);
                    currentMemoryAddress = Convert.ToString(currentMemoryAddressInt, 2).PadLeft(36, '0').ToCharArray();
                    currentMemoryValue = Convert.ToDouble(currentCommand[(currentCommand.IndexOf("= ") + 2)..]);
                    inputPosition++;
                    ResolveCurrentMemory();
                }
            }
            else
            {
                foreach (var KeyValuePair in memory)
                {
                    result += KeyValuePair.Value;
                }
                Console.WriteLine($"Finished, result is: {result}");
                Console.WriteLine($"Result should be   : 3705162613854");
            }
        }

        static void ResolveCurrentMemory()
        {
            List<int> floatingValues = new List<int>();
            for (int i = 0; i < currentMask.Length; i++)
            {
                if (currentMask[i] == 'X')
                {
                    floatingValues.Add(35-i);
                    currentMemoryAddress[i] = '0';
                }
                else if (currentMask[i] == '1')
                {
                    currentMemoryAddress[i] = '1';
                }
            }
            floatingValues.Reverse();
            FindMemoryChanges(floatingValues);
        }

        static void FindMemoryChanges(List<int> aFloatingValues)
        {
            List<double> memoryChanges = new List<double>();
            List<double> memoryBuffer = new List<double>();
            //memoryChanges.Add(Tools.ConvertBinaryToDouble(currentMemoryAddress));
            memoryChanges.Add(ConvertBinaryToDouble(currentMemoryAddress));

            for (int i = 0; i < aFloatingValues.Count; i++)
            {
                double modifier = Math.Pow(2, aFloatingValues[i]);
                foreach (double memoryItem in memoryChanges)
                {
                    memoryBuffer.Add(memoryItem + modifier);
                }
                memoryChanges.AddRange(memoryBuffer);
                memoryBuffer.Clear();
            }
            UpdateMemory(memoryChanges);
        }

        static void UpdateMemory(List<double> memoryChanges)
        {
            foreach (var memoryEntry in memoryChanges)
            {
                if (memory.ContainsKey(memoryEntry))
                {
                    memory[memoryEntry] = currentMemoryValue;
                }
                else
                {
                    memory.Add(memoryEntry, currentMemoryValue);
                }
            }
            ExecuteNextCommand();
        }

        //from tool class:
        public static double ConvertBinaryToDouble(char[] input)
        {
            char[] aBinary = input.Reverse().ToArray();
            double memSum = 0;
            for (int i = 0; i < aBinary.Length; i++)
            {

                if (aBinary[i] == '1')
                {
                    memSum += 1 * Math.Pow(2, i);
                }
            }
            return memSum;
        }
        public static List<string> ParseStringToStringList(string input)
        {
            try
            {
                List<string> parsedInput = File.ReadAllText($".//Inputs//{input}").Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace(Environment.NewLine, "").Trim()).ToList();
                return parsedInput;
            }
            catch { Console.WriteLine("invalid input"); return null; }
        }
    }
}
