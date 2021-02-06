using System;
using System.Collections.Generic;

namespace Advent_of_Code_2020
{
    class Day15
    {
        private static Dictionary<int, int> memory = new Dictionary<int, int>();
        private static Tuple<int, int> lastValues = new Tuple<int, int>(0, 0);
        public static void Solve() 
        {            
            int[] Input = new int[] { 2, 15, 0, 9, 1, 20 };
            Tuple <int,int> seedValues = AddSeedToMemory(Input);
            lastValues = IsCurrentValueInMemory(seedValues, Input.Length+1);
            while (lastValues.Item2 < 30000000)
            {
                if (lastValues.Item2 == 2020)
                {
                    Console.WriteLine($"Part1: {lastValues.Item1}");  
                }
                int currentTurn = lastValues.Item2 + 1;
                lastValues = IsCurrentValueInMemory(lastValues, currentTurn);
            }
            Console.WriteLine($"Part2: {lastValues.Item1}");
        }

        private static Tuple<int, int> AddSeedToMemory(int[] seed) 
        {            
            for (int i = 0; i < seed.Length -1; i++)
            {
                memory.Add(seed[i], i + 1);
            }
            int lastNumber = seed[seed.Length - 1];
            int lastTurn = seed.Length;
            Tuple<int, int> lastValues = new Tuple<int, int>(lastNumber, lastTurn);
            return lastValues;            
        }
        private static Tuple<int, int> IsCurrentValueInMemory(Tuple<int,int> aLastValues, int aCurrentTurn) 
        {
            if (memory.ContainsKey(aLastValues.Item1))
            {
                int newVal = aLastValues.Item2 - memory[aLastValues.Item1];
                Tuple<int, int> nextEntry = new Tuple<int, int>(newVal, aCurrentTurn);
                memory[aLastValues.Item1] = aLastValues.Item2;
                return nextEntry;
            }
            else
            {
                memory.Add(aLastValues.Item1,aLastValues.Item2);
                Tuple<int, int> nextEntry = new Tuple<int, int>(0, aCurrentTurn);
                return nextEntry;
            }
        }
    }
}
