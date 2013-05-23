using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fishing
{
    class Program
    {
        static void Main(string[] args)
        {
            Process p = new Process();
            p.StartInfo.FileName = "StockFish.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;

            p.Start();

            Console.WriteLine(p.StandardOutput.ReadLine());

            int sum = 0;

            foreach (String fen in System.IO.File.ReadLines("positions.txt"))
            {
                p.StandardInput.WriteLine("position fen " + fen);
                p.StandardInput.WriteLine("go depth 9");
                p.StandardInput.Flush();

                var output = new List<string>();

                string l = "";
                while (!l.StartsWith("bestmove"))
                {
                    l = p.StandardOutput.ReadLine();
                    output.Add(l);
                }

                int score3 = 0, score7 = 0;
                foreach (string item in output)
                {
                    var split = item.Split(' ').ToList();

                    int index = split.FindIndex(A => A == "depth");
                    if (index == -1)
                        continue;
                    int depth = int.Parse(split[index + 1]);

                    index = split.FindIndex(A => A == "cp");
                    if (index == -1)
                        continue;
                    int scores = int.Parse(split[index + 1]);

                    if (depth == 3)
                        score3 = scores;
                    else if (depth == 7)
                        score7 = scores;
                }
                sum += (score7 - score3) * (score7 - score3);
            }

            Console.WriteLine("Total sum of squares:");
            Console.WriteLine(sum);

            Console.ReadKey();
        }
    }
}
