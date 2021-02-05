using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UtilityTools;
using System.IO;

namespace Advent_of_Code_2020
{
    class Day14V3
    {
        private static Dictionary<double, double> memory = new Dictionary<double, double>();
        private static char[] currentMask = new char[36];
        private static bool Part1Finished = false;

        public static void Solve()
        {
            List<string> inputList = PopulateInput("input14");
            ExecuteNextCommand(inputList);
        }

        private static List<string> PopulateInput(string aInputFileName)
        {
            return Tools.ParseStringToStringList($"{aInputFileName}.txt");         
        }

        private static void ExecuteNextCommand(List<string> commandList, int commandListPosition = -1)
        {
            commandListPosition++;
            if (commandListPosition < commandList.Count)
            {
                string currentCommand = commandList[commandListPosition];                
                if (currentCommand.Contains("mask"))
                {
                    currentMask = currentCommand[7..].ToCharArray();                    
                    ExecuteNextCommand(commandList, commandListPosition);
                }
                else
                {
                    int.TryParse(currentCommand[4..currentCommand.IndexOf(']')], out int currentMemoryAddressInt);
                    char[] currentMemoryAddress = Convert.ToString(currentMemoryAddressInt, 2).PadLeft(36, '0').ToCharArray();
                    char[] currentMemoryValue = Convert.ToString(Convert.ToInt32(currentCommand[(currentCommand.IndexOf("= ") + 2)..]), 2).PadLeft(36, '0').ToCharArray();  
                    if (ResolveCurrentMemory(currentMemoryAddress, currentMemoryValue))
                    {
                        ExecuteNextCommand(commandList, commandListPosition);
                    }                    
                }
            }
            else
            {
                CalculateResult(commandList);
            }
        }

        private static bool ResolveCurrentMemory(char[] aCurrentMemoryAddress, char[] aCurrentMemoryValue)
        {
            List<int> floatingValues = new List<int>();
            for (int i = 0; i < currentMask.Length; i++)
            {
                if (currentMask[i] == 'X')
                {
                    if (Part1Finished)
                    {
                        floatingValues.Add(35 - i);
                        aCurrentMemoryAddress[i] = '0';
                    }
                }
                else if (currentMask[i] == '1')
                {
                    if (Part1Finished)
                    {
                        aCurrentMemoryAddress[i] = '1';
                    }
                    else
                    {
                        aCurrentMemoryValue[i] = '1';
                    }                    
                }
                else if (!Part1Finished && currentMask[i] == '0') 
                {
                    aCurrentMemoryValue[i] = '0';
                }
            }
            floatingValues.Reverse();
            return FindMemoryChanges(floatingValues, aCurrentMemoryAddress, aCurrentMemoryValue);
        }

        private static bool FindMemoryChanges(List<int> aFloatingValues, char[] aCurrentMemoryAddress, char[] aCurrentMemoryValue)
        {
            List<double> memoryChanges = new List<double>();
            List<double> memoryBuffer = new List<double>();
            double currentMemValueDouble = Tools.ConvertBinaryToDouble(aCurrentMemoryValue);
            
            if (Part1Finished)
            {
                memoryChanges.Add(Tools.ConvertBinaryToDouble(aCurrentMemoryAddress));
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
            }
            else 
            {
                memoryChanges.Add(Tools.ConvertBinaryToDouble(aCurrentMemoryAddress));
            }
            return UpdateMemory(memoryChanges, currentMemValueDouble);
        }

        private static bool UpdateMemory(List<double> memoryChanges, double aCurrentMemoryValue)
        {            
            foreach (var memoryEntry in memoryChanges)
            {
                if (memory.ContainsKey(memoryEntry))
                {
                    memory[memoryEntry] = aCurrentMemoryValue;
                }
                else
                {
                    memory.Add(memoryEntry, aCurrentMemoryValue);
                }
            }
            return true;
        }

        private static void CalculateResult(List<string> commandList)
        {
            double result = 0;
            foreach (var KeyValuePair in memory)
            {
                result += KeyValuePair.Value;
            }
            if (Part1Finished)
            {
                Console.WriteLine($"Finished part 2, result is: {result}");
                Console.WriteLine($"Result should be          : 3705162613854");
            }
            else
            {
                Console.WriteLine($"Finished part 1, result is: {result}");
                Console.WriteLine($"Result should be          : 7611244640053");
            }

            if (!Part1Finished)
            {
                Part1Finished = true;
                currentMask = new char[36];
                memory.Clear();
                ExecuteNextCommand(commandList, -1);
            }
        }
    }
}
