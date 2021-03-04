using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;
namespace PCTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set version and print copywrite
            string version = "0.1.0";
            Console.WriteLine("PC Testing Script v" + version + ". Made by Lew :) 2021");
            Console.WriteLine("Please choose an option:");
            bool again = true;
            //Basic menu and option select
            Console.WriteLine("Menu:");
            Console.WriteLine("1. CPU Stress Test");
            Console.WriteLine("2. GPU Stress Test");
            Console.WriteLine("3. Combined Stress Test");
            Console.WriteLine("4. Memtest64 Memory Test");
            while (again)
            {
                again = false;
                switch (Console.ReadLine())
                {
                case "1":
                    Console.WriteLine("Starting CPU Stress Test");
                        stressCPU();
                break;
                case "2":
                    Console.WriteLine("Starting Furmark GPU Test");
                        stressGPU();
                break;
                case "3":
                    Console.WriteLine("Starting combined CPU/GPU Test");
                        startBoth();
                break;
                case "4":
                    Console.WriteLine("Starting Memtest64 Memory Test");
                break;

                default:
                    Console.WriteLine("Incorrect option, try again :)");
                again = true;
                break;
                 }
            }
        }
        public static void stressCPU()
        {
            //Check for cpu stress test, download if not, then run with -console option.
            Console.Clear();
            if (!File.Exists("cpustress.exe"))
            {
                download("cpustress.exe");
                Console.WriteLine("File downloaded!");
            }
            else
            {
                Console.WriteLine("File already exists, continuing!");
            }
            //Start CPU Stress test, pass args to PressureService to run in console mode. Press q to quit!
            ProcessStartInfo startInfo = new ProcessStartInfo("cpustress.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = "-console";
            Process.Start(startInfo);
        }

        public static void stressGPU()
        {
            //find if FurMark exists in either Program files or in testing tools directory
            string path = null;
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToString() + "\\Geeks3D\\Benchmarks\\FurMark\\FurMark.exe") && (!File.Exists("C:\\AWD Testing Tools\\FurMark\\FurMark.exe")))
            {
                download("furmark.exe");
                try
                {
                    //run installer, some of this is unneeded but idk what
                    Console.WriteLine("Starting to install application");
                    Process process = new Process();
                    process.StartInfo.FileName = "furmark.exe";
                    process.StartInfo.Arguments = string.Format(" /qb /i \"{0}\" ALLUSERS=1", "furmark.exe");
                    process.StartInfo.Arguments = "/quiet";

                    process.Start();
                    process.WaitForExit();
                    Console.WriteLine("Application installed successfully!");
                }
                catch
                {
                    Console.WriteLine("There was a problem installing the application!");
                }
            }
            else
            {
                // find which one exists, set path. probs cleaner way of doing this
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToString() + "\\Geeks3D\\Benchmarks\\FurMark\\FurMark.exe")){
                    path = (Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToString() + "\\Geeks3D\\Benchmarks\\FurMark\\FurMark.exe");
                }
                else if (File.Exists("C:\\AWD Testing Tools\\FurMark\\FurMark.exe"))
                {
                    path = "C:\\AWD Testing Tools\\FurMark\\FurMark.exe";
                }
            }
            //start furmark with args
            Console.WriteLine("File already exists, continuing!");
            ProcessStartInfo startInfo = new ProcessStartInfo(path);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = "/nogui /width 1280 /height 720 /run_mode 2 /app_process_priority 50";
            Process.Start(startInfo);

            //set to high priority so it doesn't totally fall over when cpu bench is running
            Process[] processes = Process.GetProcessesByName("FurMark");
            foreach (Process process in processes)
            {
                process.PriorityClass = ProcessPriorityClass.High;
            }
        }


        

        public static void startBoth()
        {
            //start both, does what it says on the tin
            Thread CPU = new Thread(new ThreadStart(stressCPU));
            Thread GPU = new Thread(new ThreadStart(stressGPU));
            GPU.Start();
            Thread.Sleep(5000);
            CPU.Start();
            GPU.Join();
            CPU.Join();
        }

        public static void download(string name)
        {
            // Kudzayi woz here

            //download file from my website. boo hoo hardcoded variables
            WebClient downloader = new WebClient();
            string uri = "https://d-bk.uk/files/", file;
            file = uri + name;
            Console.WriteLine("Downloading " + file);
            downloader.DownloadFile(file, name);
            Console.WriteLine("Downloaded " + file + " from " + uri);

            
        }
    }
}

