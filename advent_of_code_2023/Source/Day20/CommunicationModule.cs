using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.DayTwenty
{
    internal abstract class CommunicationModule
    {
        protected List<string> connectedModuleNames;
        public List<string> ConnectedModules {  get { return connectedModuleNames; } }

        public CommunicationModule(string[] connectedModuleNames)
        {
            this.connectedModuleNames = new List<string>();

            foreach (string name in connectedModuleNames)
            {
                this.connectedModuleNames.Add(name);
            }
        }

        public abstract List<(string moduleName, bool highPulse)> ReceivePulse(bool highPulse, string sendingModule);

        public virtual void UpdateInputs(string inputName) { }
    }

    internal class BroadcastModule : CommunicationModule
    {
        public BroadcastModule(string[] connectedModuleNames) : base(connectedModuleNames) { }

        public override List<(string moduleName, bool highPulse)> ReceivePulse(bool highPulse, string sendingModule)
        {
            List<(string moduleName, bool highPulse)> pulsesFired = new List<(string moduleName, bool highPulse)>();

            foreach (string name in connectedModuleNames)
            {
                pulsesFired.Add((name, highPulse));
            }

            return pulsesFired;
        }
    }

    internal class FlipFlopModule : CommunicationModule
    {
        bool isOn;

        public FlipFlopModule(string[] connectedModuleNames) : base(connectedModuleNames)
        {
            isOn = false;
        }

        public override List<(string moduleName, bool highPulse)> ReceivePulse(bool highPulse, string sendingModule)
        {
            List<(string moduleName, bool highPulse)> pulsesFired = new List<(string moduleName, bool highPulse)>();

            if (!highPulse)
            {
                isOn = !isOn;

                foreach (string name in connectedModuleNames)
                {
                    pulsesFired.Add((name, isOn ? true : false));
                }
            }

            return pulsesFired;
        }
    }

    internal class ConjunctionModule : CommunicationModule
    {
        // True if last pulse was high
        private Dictionary<string, bool> rememberedPulses;

        public ConjunctionModule(string[] connectedModuleNames) : base(connectedModuleNames)
        {
            rememberedPulses = new Dictionary<string, bool>();
        }

        public override void UpdateInputs(string inputName)
        {
            rememberedPulses.Add(inputName, false);
        }

        public override List<(string moduleName, bool highPulse)> ReceivePulse(bool highPulse, string sendingModule)
        {
            List<(string moduleName, bool highPulse)> pulsesFired = new List<(string moduleName, bool highPulse)>();

            rememberedPulses[sendingModule] = highPulse;

            bool allHighPulses = true;
            foreach (bool wasHigh in rememberedPulses.Values)
            {
                if (!wasHigh)
                {
                    allHighPulses = false;
                    break;
                }
            }

            foreach (string name in connectedModuleNames)
            {
                pulsesFired.Add((name, allHighPulses ? false : true));
            }

            return pulsesFired;
        }
    }
}
