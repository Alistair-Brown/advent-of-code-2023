using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTwenty
{
    internal class CommunicationNetwork
    {
        Dictionary<string, CommunicationModule> modules;
        BroadcastModule broadcastModule;

        Dictionary<string, ulong> stepsForEachPartTwoConjunctionModuleToGoHigh;

        public CommunicationNetwork(string[] inputLines)
        {
            modules = new Dictionary<string, CommunicationModule> ();

            foreach (string line in inputLines)
            {
                string[] nameAndConnections = line.Split(new string[] { " -> " }, StringSplitOptions.None);
                string[] connections = nameAndConnections[1].Split(new string[] { ", " }, StringSplitOptions.None);

                if (nameAndConnections[0][0] == '&')
                {
                    modules.Add(nameAndConnections[0].Substring(1), new ConjunctionModule(connections));
                }
                else if (nameAndConnections[0][0] == '%')
                {
                    modules.Add(nameAndConnections[0].Substring(1), new FlipFlopModule(connections));
                }
                else
                {
                    broadcastModule = new BroadcastModule(connections);
                }
            }

            // Conjunction modules need to hold information on the modules that input into them as well as those
            // they output to.
            string inputToRx = "";
            foreach (KeyValuePair<string, CommunicationModule> entry in modules)
            {
                foreach (string outputMod in entry.Value.ConnectedModules)
                {
                    if (modules.ContainsKey(outputMod))
                    {
                        modules[outputMod].UpdateInputs(entry.Key);
                    }

                    if (outputMod == "rx")
                    {
                        Debug.Assert(inputToRx.Length == 0, "Found multiple inputs to rx");
                        inputToRx = entry.Key;
                    }
                }
            }

            // Inspection of the input shows that rx has exactly one module that inputs to it, and that that
            // module is a conjunction module with multiple other conjunction modules as its inputs. Save off the
            // the names of those multiple inputs, we'll need them to solve part 2.
            stepsForEachPartTwoConjunctionModuleToGoHigh = new Dictionary<string, ulong>();
            Debug.Assert(inputToRx.Length > 0, "Failed to find an input to Rx");
            foreach (KeyValuePair<string, CommunicationModule> entry in modules)
            {
                foreach (string outputMod in entry.Value.ConnectedModules)
                {
                    if (outputMod == inputToRx)
                    {
                        stepsForEachPartTwoConjunctionModuleToGoHigh.Add(entry.Key, 0);
                    }
                }
            }
            Debug.Assert(stepsForEachPartTwoConjunctionModuleToGoHigh.Count > 0, "No input conjunction modules found");
        }

        // Returns the product of the sum of high and low pulses
        public ulong PressButtonNTimes(ulong timesToPress)
        {
            ulong lowPulses = 0;
            ulong highPulses = 0;

            for (ulong ii = 0; ii < timesToPress; ++ii)
            {
                PressButton(ii + 1, ref lowPulses, ref highPulses);
            }

            return lowPulses * highPulses;
        }

        // Inspection of the puzzle input shows that rx has a single conjuction module leading into it, with multiple
        // other modules leading into that one. For rx to receive a low pulse, we need all of the modules leading into
        // to that conjunction module to send high pulses. Looking at some debug output showed that each of those modules
        // sends a high pulse every n button presses, where n is different for each of the modules, and always prime.
        // So to find the cycle when they all send a high pulse, we need the product of those different n's. This is
        // the cycle number on which rx will receive a low pulse.
        public ulong PressesUntilRxReceivesLowPulse()
        {
            // We don't use these for part two, but PressButton takes them as ref params, so they still need to exist.
            ulong lowPulses = 0;
            ulong highPulses = 0;

            ulong pressNumber = 1;             

            while (true)
            {
                PressButton(pressNumber, ref lowPulses, ref highPulses);

                bool allConjunctionsHigh = true;
                foreach (ulong pressesUntilConjunctionHigh in stepsForEachPartTwoConjunctionModuleToGoHigh.Values)
                {
                    if (pressesUntilConjunctionHigh == 0)
                    {
                        allConjunctionsHigh = false;
                        break;
                    }
                }

                if (allConjunctionsHigh)
                {
                    break;
                }
                else
                {
                    ++pressNumber;
                }
            }

            ulong totalPresses = 1;
            foreach (ulong pressesUntilConjunctionHigh in stepsForEachPartTwoConjunctionModuleToGoHigh.Values)
            {
                totalPresses *= pressesUntilConjunctionHigh;
            }

            return totalPresses;
        }

        private void PressButton(ulong pressNumber, ref ulong lowPulses, ref ulong highPulses)
        {
            Queue<(string receivingModule, string sendingModule, bool highPulse)> queuedPulses = new Queue<(string, string, bool)>();

            List<(string, bool)> pulsesFromBroadcast = broadcastModule.ReceivePulse(false, "");
            ++lowPulses;
            foreach ((string receivingModule, bool highPulse) nextPulse in pulsesFromBroadcast)
            {
                queuedPulses.Enqueue((nextPulse.receivingModule, "broadcast", nextPulse.highPulse));
            }

            while (queuedPulses.Count > 0)
            {
                (string receivingModule, string sendingModule, bool highPulse) currentPulse = queuedPulses.Dequeue();

                if (currentPulse.highPulse)
                {
                    ++highPulses;
                }
                else
                {
                    ++lowPulses;
                }

                List<(string, bool)> pulsesGenerated =
                    modules[currentPulse.receivingModule].ReceivePulse(currentPulse.highPulse, currentPulse.sendingModule);

                foreach ((string receivingModule, bool highPulse) generatedPulse in pulsesGenerated)
                {
                    if (generatedPulse.highPulse && stepsForEachPartTwoConjunctionModuleToGoHigh.Keys.Contains(currentPulse.receivingModule)) 
                    {
                        // Careful not to increase the pressNumber for that module if we've already found it.
                        if (stepsForEachPartTwoConjunctionModuleToGoHigh[currentPulse.receivingModule] == 0)
                        {
                            stepsForEachPartTwoConjunctionModuleToGoHigh[currentPulse.receivingModule] = pressNumber;
                        }
                    }

                    if (modules.ContainsKey(generatedPulse.receivingModule))
                    {
                        queuedPulses.Enqueue((generatedPulse.receivingModule, currentPulse.receivingModule, generatedPulse.highPulse));
                    }
                    else
                    {
                        if (generatedPulse.highPulse)
                        {
                            ++highPulses;
                        }
                        else
                        {
                            ++lowPulses;
                        }
                    }    
                }
            }
        }
    }
}
