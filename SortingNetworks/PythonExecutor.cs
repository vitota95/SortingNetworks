using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingNetworks
{
    public static class PythonExecutor
    {
        public static string ExecuteFile(string path)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "\"C:\\Users\\javig\\OneDrive\\Escritorio\\master\\master thesis\\perfect_matching\\interpreter\\Scripts\\python.exe\"",
                //Arguments = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)} {5} {10}",
                Arguments = "\"" + path + "\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                LoadUserProfile = true
            };

            var result = "";
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    Console.WriteLine("From System Diagnostics");
                    Console.WriteLine("5 + 10 = {0}", result);
                    Console.WriteLine($"the path: {path}");

                }
            }

            return result;
        }
    }
}
