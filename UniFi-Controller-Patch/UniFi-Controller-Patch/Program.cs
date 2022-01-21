using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceProcess;

namespace UniFiControllerPatch
{
    internal class Program
    {
        public static string unifi = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) , "Ubiquiti UniFi\\lib\\");
        public static string unifi_jar = Path.Combine(unifi, "ace.jar");
        static void Main(string[] args)
        {
            Console.WriteLine("[i] Log4j Patcher for UniFi Network Controller [Version: 1.0.0]");
            Console.WriteLine("[i] by valnoxy (https://valnoxy.dev)");
            Console.WriteLine("\n[i] This tool is open source! See: https://github.com/valnoxy/UniFi-Controller-Patch");

            string service = CheckSys();

            RunService(service, false);

            string log4jver = String.Empty;
            if (service == "svc")
            {
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.16.0.jar")))
                   log4jver = "2.16.0";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.15.0.jar")))
                   log4jver = "2.15.0";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.14.1.jar")))
                   log4jver = "2.14.1";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.14.0.jar")))
                   log4jver = "2.14.0";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.13.3.jar")))
                   log4jver = "2.13.3";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.13.2.jar")))
                   log4jver = "2.13.2";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.13.1.jar")))
                   log4jver = "2.13.1";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.13.0.jar")))
                   log4jver = "2.13.0";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.12.4.jar")))
                   log4jver = "2.12.4";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.12.3.jar")))
                   log4jver = "2.12.3";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.12.2.jar")))
                   log4jver = "2.12.2";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.12.1.jar")))
                   log4jver = "2.12.1";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.12.0.jar")))
                   log4jver = "2.12.0";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.11.2.jar")))
                   log4jver = "2.11.2";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.11.1.jar")))
                   log4jver = "2.11.1";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.11.0.jar")))
                   log4jver = "2.11.0";
                if (File.Exists(Path.Combine(unifi, "log4j-core-2.10.0.jar")))
                   log4jver = "2.10.0";
            }

            if (String.IsNullOrEmpty(log4jver))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] This Version of UniFi Network Controller is too old. Please update it before using this patch.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[!] Restart service ...");
                RunService(service, true);
                
                Console.WriteLine("[!] Terminating in 10 sec ...");
                System.Threading.Thread.Sleep(10000);
                Environment.Exit(-1);
            }

            Console.WriteLine("[i] Updating log4j classes ...");
            UpdateClass(unifi, "https://dl.exploitox.de/other/vuln/log4j/v2.17.1/log4j-api-2.17.1.jar", log4jver, "api");
            UpdateClass(unifi, "https://dl.exploitox.de/other/vuln/log4j/v2.17.1/log4j-core-2.17.1.jar", log4jver, "core");
            UpdateClass(unifi, "https://dl.exploitox.de/other/vuln/log4j/v2.17.1/log4j-slf4j-impl-2.17.1.jar", log4jver, "slf4j-impl");
            RunService(service, true);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[i] PowerChute was successfully patched! Closing ...");
            Console.ForegroundColor = ConsoleColor.White;
            System.Threading.Thread.Sleep(5000);
        }

        static string CheckSys()
        {
            Console.WriteLine("[i] Searching for UniFi Network Controller ...");
            if (Directory.Exists(unifi))
            {
                Console.WriteLine("[i] UniFi Network Controller found!");
                return "svc";
            }
            else
            {
                Console.WriteLine("[!] Error: Cannot find UniFi Network Controller.");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(-1);
                return "";
            }
        }

        private static void RunService(string servicename, bool v)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";

            if (v == true)
            {
                p.StartInfo.Arguments = $"/c java -jar {unifi_jar} startsvc";
            }

            if (v == false)
            {
                p.StartInfo.Arguments = $"/c java -jar {unifi_jar} stopsvc";
            }

            p.Start();
            p.WaitForExit();
        }
        
        private static void UpdateClass(string path, string url, string log4jver, string log4jmodule)
        {
            using (var client = new WebClient())
            {
                // =================================
                // =      Downloading Class        =
                // =================================
                Console.Write($"[i] Downloading log4j-{log4jmodule}-2.17.1.jar ... ");
                try 
                {
                    client.DownloadFile(url, path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[!] An error has occurred while downloading: {ex}");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(-1);
                }
                Console.WriteLine("OK!");

                // =================================
                // =      Removing old Class       =
                // =================================
                try
                {
                    Console.WriteLine($"[i] Removing log4j-{log4jmodule}-{log4jver}.jar ... ");
                    File.Delete(Path.Combine(path, $"log4j-{log4jmodule}-{log4jver}.jar"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[!] An error has occurred while removing: {ex}");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(-1);
                }
                Console.WriteLine("OK!");

                // =================================
                // =       Linking new Class       =
                // =================================
                try
                {
                    Console.WriteLine($"[i] Linking to log4j-{log4jmodule}-2.17.1.jar ... ");
                    SymbolLink.SymbolicLink.CreateSymbolicLink(Path.Combine(path, $"log4j-{log4jmodule}-2.17.1.jar"), Path.Combine(path, $"log4j-{log4jmodule}-{log4jver}.jar"), true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[!] An error has occurred while linking: {ex}");
                    System.Threading.Thread.Sleep(5000);
                    Environment.Exit(-1);
                }
                Console.WriteLine("OK!");
            }
        }
    }
}
