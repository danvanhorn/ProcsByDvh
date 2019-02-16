using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ProcsByDvh
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        public enum AllocationProtect : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        static void ReadProcessMemory(string pid)
        {
            if (pid != null && Regex.IsMatch(pid, @"\d"))
            {
                Process proc = Process.GetProcessById(Int32.Parse(pid));

                byte[] buffer = new byte[proc.PagedSystemMemorySize64];
                IntPtr bytesread;

                ReadProcessMemory(proc.Handle, proc.Modules[0].BaseAddress, buffer, (int)proc.PagedSystemMemorySize64, out bytesread);

                Console.WriteLine(BitConverter.ToString(buffer));
            }
            else
            {
                Console.WriteLine("Enter a valid pid.");
            }
        }

        static void ShowExecutablePages(string pid)
        {
            long MaxAddress = 0x7fffffff;
            long address = 0;

            if (pid != null && Regex.IsMatch(pid, @"\d"))
            {
                Process proc = Process.GetProcessById(Int32.Parse(pid));
                try
                {
                    IntPtr hProcess = proc.Handle;
                    do
                    {
                        MEMORY_BASIC_INFORMATION m;
                        int result = VirtualQueryEx(hProcess, (IntPtr)address, out m, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                        if (m.AllocationProtect == (uint)AllocationProtect.PAGE_EXECUTE ||
                           m.AllocationProtect == (uint)AllocationProtect.PAGE_EXECUTE_READ ||
                           m.AllocationProtect == (uint)AllocationProtect.PAGE_EXECUTE_READWRITE ||
                           m.AllocationProtect == (uint)AllocationProtect.PAGE_EXECUTE_WRITECOPY)
                        {
                            Console.WriteLine("Executable page address space: {0}-{1}    {2} bytes", m.BaseAddress, (uint)m.BaseAddress + (uint)m.RegionSize - 1, m.RegionSize);
                        }
                        if (address == (long)m.BaseAddress + (long)m.RegionSize)
                            break;
                        address = (long)m.BaseAddress + (long)m.RegionSize;
                    } while (address <= MaxAddress);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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
                Process proc = Process.GetProcessById(Int32.Parse(pid));
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

        static void ListThreadsInProcess(string pid)
        {
            if (pid != null && Regex.IsMatch(pid, @"\d"))
            {
                Process proc = Process.GetProcessById(Int32.Parse(pid));
                foreach (ProcessThread thread in proc.Threads)
                {
                    Console.WriteLine("Thread Id: {0}", thread.Id);
                }
            }
            else
            {
                Console.WriteLine("Enter a valid pid.");
            }
        }

        static void ListProcesses()
        {
            Process[] procs = Process.GetProcesses(Environment.MachineName);
            foreach (Process proc in procs)
            {
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
            Console.Write("> ");
            line = Console.ReadLine();

            // handle immediate exit
            if (!String.Equals(line, "exit"))
            {
                // handle list processes so user can see available pids
                if (String.Equals(line, "procs"))
                {
                    ListProcesses();
                }

                // handle first pid entered
                if (line != null && Regex.IsMatch(line, @"\d"))
                {
                    pid = line;
                }

                // start main program loop
                while (true)
                {
                    Console.WriteLine("==== Current PID: {0} ====", pid);
                    Console.WriteLine("Enter 'procs' to see a list of running processses.");
                    Console.WriteLine("Enter 'ls' to list on running threads within the process boundary.");
                    Console.WriteLine("Enter 'mods' to enumerate all loaded modules within a process.");
                    Console.WriteLine("Enter 'pages' to show all executable pages within the process.");
                    Console.WriteLine("Enter 'mem' to read process memory.");
                    Console.WriteLine("Or enter a new pid and press enter.");
                    Console.WriteLine("Type 'exit' to exit.");
                    Console.Write("> ");
                    line = Console.ReadLine();

                    // make sure we have a pid
                    if (line != null && Regex.IsMatch(line, @"\d"))
                    {
                        pid = line;
                    }

                    switch (line)
                    {
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

                    if (String.Equals(line, "exit"))
                    {
                        break;
                    }
                    else
                    {
                        line = null;
                    }
                }

            }
        }
    }
}
