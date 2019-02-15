using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProcsByDvh
{
    class Program
    {
        static void ReadProcessMemory(string pid)
        {
            if (pid != null && Regex.IsMatch(pid, @"\d"))
            {
                Console.WriteLine("TODO: Read process memory");
            }
            else
            {
                Console.WriteLine("Enter a valid pid.");
            }
        }

        static void ShowExecutablePages(string pid)
        {
            if (pid != null && Regex.IsMatch(pid, @"\d"))
            {
                //List<Process> procs = new List<Process>(Process.GetProcesses(Environment.MachineName));
                //Process proc = procs.Find(p => String.Equals(p.Id.ToString(), pid));
                //foreach (ProcessModule module in proc.)
                //{
                //    Console.WriteLine(string.Format("Module: {0}", module.FileName));
                //}
            }
            else
            {
                Console.WriteLine("Enter a valid pid.");
            }
        }

        static void EnumerateModules(string pid)
        {
            if (pid != null && Regex.IsMatch(pid, @"\d"))
            {
                List<Process> procs = new List<Process>(Process.GetProcesses(Environment.MachineName));
                Process proc = procs.Find(p => String.Equals(p.Id.ToString(), pid));
                foreach (ProcessModule module in proc.Modules)
                {
                    Console.WriteLine(string.Format("Module: {0}", module.FileName));
                }
            }
            else
            {
                Console.WriteLine("Enter a valid pid.");
            }
        }

        static void ListThreadsInProcess(string pid) {
            if(pid != null && Regex.IsMatch(pid, @"\d"))
            {
                List<Process> procs = new List<Process>(Process.GetProcesses(Environment.MachineName));
                Process proc = procs.Find(p => String.Equals(p.Id.ToString(),pid));
                foreach (ProcessThread thread in proc.Threads) {
                    Console.WriteLine("Thread Id: {0}", thread.Id);
                }
            } else
            {
                Console.WriteLine("Enter a valid pid.");
            }
        }

        static void ListProcesses() {
            Process[] procs = Process.GetProcesses(Environment.MachineName);
            foreach (Process proc in procs) {
                Console.WriteLine("Process Id: {0} Name: {1}", proc.Id, proc.ProcessName);
            }
        }
        
        static void Main(string[] args)
        {
            string line = null;
            string pid = null;

            Console.WriteLine("==== ProcsByDvh ====");
            Console.WriteLine("Enter 'procs' to see a list of running processses.");
            Console.WriteLine("Enter a PID and press enter to see additional options.");
            Console.WriteLine("Type 'exit' to exit.");
            line = Console.ReadLine();

            // handle immediate exit
            if(!String.Equals(line, "exit"))
            {
                // handle list processes so user can see available pids
                if(String.Equals(line, "procs"))
                {
                    ListProcesses();
                }

                // handle first pid entered
                if (line != null && Regex.IsMatch(line, @"\d"))
                {
                    pid = line;
                }

                // start main program loop
                while (true) {
                    Console.WriteLine("==== Current PID: {0} ====", pid);
                    Console.WriteLine("Enter 'procs' to see a list of running processses.");
                    Console.WriteLine("Enter 'ls' to list on running threads within the process boundary.");
                    Console.WriteLine("Enter 'mods' to enumerate all loaded modules within a process.");
                    Console.WriteLine("Enter 'pages' to show all executable pages within the process.");
                    Console.WriteLine("Enter 'mem' to read process memory.");
                    Console.WriteLine("Or enter a new pid and press enter.");
                    Console.WriteLine("Type 'exit' to exit\n");
                    line = Console.ReadLine();

                    // make sure we have a pid
                    if (line != null && Regex.IsMatch(line, @"\d"))
                    {
                        pid = line;
                    }

                    switch (line) {
                        case "procs":
                            ListProcesses();
                            continue;
                        case "ls":
                            ListThreadsInProcess(pid);
                            continue;
                        case "mods":
                            EnumerateModules(pid);
                            continue;
                        case "pages":
                            ShowExecutablePages(pid);
                            continue;
                        case "mem":
                            ReadProcessMemory(pid);
                            continue;
                        default:
                            break;
                    } 

                    if (String.Equals(line, "exit")) {
                        break;
                    } else
                    {
                        line = null;
                    }
                }

            }
        }
    }
}
