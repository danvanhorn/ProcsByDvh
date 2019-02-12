using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProcsByDvh
{
    class Program
    {
        static void ListThreadsInProcess(string pid) {
            List<Process> procs = new List<Process>(Process.GetProcesses(Environment.MachineName));
            Process proc = procs.Find(p => String.Equals(p.Id.ToString(),pid));
            foreach (ProcessThread thread in proc.Threads) {
                Console.WriteLine(thread);
            }
        }

        static void ListProcesses() {
            Process[] procs = Process.GetProcesses(Environment.MachineName);
            foreach (Process proc in procs) {
                Console.WriteLine("pid: {0} name: {1}", proc.Id, proc.ProcessName);
            }
        }
        
        static void Main(string[] args)
        {
            string line = null;
            Console.Write("ProcsByDvh\n");
            Console.Write("Press (1) to see a list of running processses\n");
            Console.Write("Enter a PID and press enter\n");
            Console.Write("Press (2) to list on running threads within the process boundary\n");
            Console.Write("Press (3) to enumerate all loaded modules within a process\n");
            Console.Write("Press (4) to show all executable pages within the process\n");
            Console.Write("Press (5) to read process memory\n");
            Console.Write("Type 'exit' to exit\n");
            while (true) {
                line = Console.ReadLine();
                switch(line) {
                    case "1":
                        ListProcesses();
                        continue;
                    default:
                        break;
                } 
                if (String.Equals(line, "exit")) {
                    break;
                }
            }
        }
    }
}
