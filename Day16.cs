using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Advent_of_Code_2020
{
    class Day16
    {
        public static void Solve()
        {
            List<string> inputList = ParseInput("input16");

            Dictionary<string, HashSet<int>> ticketPolicy = GetTicketPolicy(inputList);

            int[] myTicket = GetMyTicket(inputList);

            List<int[]> unfilteredNearbyTickets = GetAllNearbyTickets(inputList);

            List<int[]> filteredNearbyTickets = FilterOutInvalidNearbyTickets(ticketPolicy, unfilteredNearbyTickets);

            Dictionary<string, HashSet<int>> possibleTicketFieldContents = GetAllPossibleTicketFieldContents(ticketPolicy, filteredNearbyTickets);

            List<Tuple<string, int>> ticketFieldContents = SieveOutFieldContents(possibleTicketFieldContents, new List<Tuple<string, int>>());

            CalculatePart2(myTicket, ticketFieldContents);

            Console.ReadLine();
        }

        private static List<string> ParseInput(string aInput)
        {
            List<string> input = File.ReadAllLines(@$"./Inputs/{aInput}.txt").ToList();
            return input;
        }

        private static Dictionary<string, HashSet<int>> GetTicketPolicy(List<string> aInput)
        {
            Dictionary<string, HashSet<int>> ticketPolicyStorage = new Dictionary<string, HashSet<int>>();
            List<string[]> rawTicketPolicy = aInput.Where(x => x.Contains(" or ")).Select(x => x.Replace(" or ", ": ").Split(": ")).ToList();
            foreach (var rawPolicy in rawTicketPolicy)
            {
                ticketPolicyStorage.Add(rawPolicy[0], GetCurrentPolicyNumberRange(rawPolicy));
            }
            return ticketPolicyStorage;
        }

        private static int[] GetMyTicket(List<string> aInput)
        {
            int ticketPosition = aInput.FindIndex(x => x.Contains("your ticket"));
            int[] ticketNumbers = aInput[ticketPosition + 1].Split(',').Select(x => int.Parse(x)).ToArray();
            return ticketNumbers;
        }

        private static List<int[]> GetAllNearbyTickets(List<string> aInput)
        {
            List<int[]> nearbyTickets = new List<int[]>();
            int nerbyTicketPositions = aInput.FindIndex(x => x.Contains("nearby tickets"));
            for (int i = nerbyTicketPositions + 1; i < aInput.Count; i++)
            {
                nearbyTickets.Add(aInput[i].Split(',').Select(x => int.Parse(x)).ToArray());
            }
            return nearbyTickets;
        }

        private static List<int[]> FilterOutInvalidNearbyTickets(Dictionary<string, HashSet<int>> aTicketPolicy, List<int[]> aUnfilteredNearbyTickets)
        {
            HashSet<int> validNumbers = GetAllValidNumbers(aTicketPolicy);

            List<int[]> validNearbyTickets = new List<int[]>();
            List<int> invalidNumbers = new List<int>();
            foreach (var ticket in aUnfilteredNearbyTickets)
            {
                int[] values = ticket.Where(x => !validNumbers.Contains(x)).ToArray();
                if (values.Length == 0)
                {
                    validNearbyTickets.Add(ticket);
                }
                invalidNumbers.AddRange(values);
            }
            int part1Result = invalidNumbers.Sum();
            Console.WriteLine($"Part 1 Result: {part1Result}");
            return validNearbyTickets;

        }

        private static Dictionary<string, HashSet<int>> GetAllPossibleTicketFieldContents(Dictionary<string, HashSet<int>> aTicketPolicy, List<int[]> aValidNearbyTickets)
        {
            Dictionary<string, HashSet<int>> possibleTicketFieldContents = new Dictionary<string, HashSet<int>>();

            foreach (var validFieldNumberRange in aTicketPolicy)
            {
                for (int i = 0; i < aValidNearbyTickets[0].Length; i++)
                {
                    if (aValidNearbyTickets.Where(x => validFieldNumberRange.Value.Contains(x[i])).Select(x => x).Count() == aValidNearbyTickets.Count())
                    {
                        if (possibleTicketFieldContents.ContainsKey(validFieldNumberRange.Key))
                        {
                            possibleTicketFieldContents[validFieldNumberRange.Key].Add(i);
                        }
                        else
                        {
                            possibleTicketFieldContents.Add(validFieldNumberRange.Key, new HashSet<int>());
                            possibleTicketFieldContents[validFieldNumberRange.Key].Add(i);
                        }

                    }
                }
            }
            return possibleTicketFieldContents;
        }

        private static List<Tuple<string, int>> SieveOutFieldContents(Dictionary<string, HashSet<int>> aPossibleTicketFields, List<Tuple<string, int>> aTicketFieldContents)
        {
            if (!(aPossibleTicketFields.Select(y => y.Value.Count()).ToList().Sum() == 0))
            {
                foreach (var kvp in aPossibleTicketFields)
                {
                    foreach (var item in kvp.Value)
                    {
                        //search for unique indexes in each field property, add them to the result list and remove them from all other properties.
                        if (aPossibleTicketFields.Where(x => x.Value.Contains(item)).Select(y => y).Count() == 1)
                        {
                            aTicketFieldContents.Add(new Tuple<string, int>(kvp.Key, item));
                            foreach (var kvp3 in aPossibleTicketFields)
                            {
                                kvp3.Value.Remove(item);
                            }
                            break;
                        }
                    }
                    //search for field properties that only have one possible index. Add that index to result list, remove it from all other properties.
                    if (kvp.Value.Count == 1)
                    {
                        aTicketFieldContents.Add(new Tuple<string, int>(kvp.Key, kvp.Value.FirstOrDefault()));
                        foreach (var kvp2 in aPossibleTicketFields)
                        {
                            kvp2.Value.Remove(aTicketFieldContents.Last().Item2);
                        }
                    }
                }
                SieveOutFieldContents(aPossibleTicketFields, aTicketFieldContents);
            }
            return aTicketFieldContents;
        }

        private static void CalculatePart2(int[] myTicket, List<Tuple<string, int>> aTicketIndexes)
        {
            double part2Result = 0;
            foreach (var item in aTicketIndexes.Where(x => x.Item1.Contains("departure")).Select(y => y.Item2).ToList())
            {
                if (part2Result == 0)
                {
                    part2Result = myTicket[item];
                }
                else
                {
                    part2Result *= myTicket[item];
                }
            }
            Console.WriteLine($"Part 2 Result: {part2Result}");
        }

        private static HashSet<int> GetCurrentPolicyNumberRange(string[] aRawPolicy)
        {
            HashSet<int> validPolicyNumbers = new HashSet<int>();
            List<string[]> validNumberRanges = aRawPolicy.Where(x => x.Contains("-")).Select(x => x.Split("-")).ToList();
            foreach (string[] validNumberRange in validNumberRanges)
            {
                int.TryParse(validNumberRange[0], out int rangeStartNumber);
                int.TryParse(validNumberRange[1], out int rangeEndNumber);
                int rangeLength = rangeEndNumber - rangeStartNumber + 1;
                foreach (int validPolicyNumber in Enumerable.Range(rangeStartNumber, rangeLength))
                {
                    validPolicyNumbers.Add(validPolicyNumber);
                }
            }
            return validPolicyNumbers;
        }

        private static HashSet<int> GetAllValidNumbers(Dictionary<string, HashSet<int>> aValidTickets)
        {
            HashSet<int> validNumbers = new HashSet<int>();
            foreach (KeyValuePair<string, HashSet<int>> kvp in aValidTickets)
            {
                validNumbers.UnionWith(kvp.Value);
            }
            return validNumbers;
        }
    }
}

