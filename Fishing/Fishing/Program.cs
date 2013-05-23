using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fishing
{
    class Program
    {
        const int SCORE1 = 3;
        const int SCORE2 = 8;
        const string PARAM = "Mobility (Middle Game)";
        const int LOWER_BOUND = 0;
        const int UPPER_BOUND = 200;
        const int STEP = 10;

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

            int minP = -1;
            int minSum = int.MaxValue ;

            for (int i = LOWER_BOUND; i < UPPER_BOUND; i += STEP)
            {
                Console.WriteLine("" + PARAM + " = " + i + ":");
                    int sum = 0;
                    foreach (String fen in System.IO.File.ReadLines("positions.txt"))
                    {
                        p.StandardInput.WriteLine("setoption name Hash value 1");
                        p.StandardInput.WriteLine("setoption name Clear Hash");
                        p.StandardInput.WriteLine("setoption name " + PARAM + " value " + i);
                        p.StandardInput.WriteLine("position fen " + fen);
                        p.StandardInput.WriteLine("go depth " + (SCORE2 + 2));
                        p.StandardInput.Flush();

                        var output = new List<string>();

                        string l = "";
                        while (l == null || !l.StartsWith("bestmove"))
                        {
                            l = p.StandardOutput.ReadLine();
                            output.Add(l);
                        }

                        int score1 = 0, score2 = 0;
                        foreach (string item in output)
                        {
                            if (item == null)
                                continue;
                            var split = item.Split(' ').ToList();

                            int index = split.FindIndex(A => A == "depth");
                            if (index == -1)
                                continue;
                            int depth = int.Parse(split[index + 1]);

                            index = split.FindIndex(A => A == "cp");
                            if (index == -1)
                                continue;
                            int scores = int.Parse(split[index + 1]);

                            if (depth == SCORE1)
                                score1 = scores;
                            else if (depth == SCORE2)
                                score2 = scores;
                        }

                        sum += (score1 - score2) * (score1 - score2);
                    }
                    Console.Write("\tTotal sum of squares:\t");
                    Console.WriteLine(sum);

                    if (sum < minSum)
                    {
                        minSum = sum;
                        minP = i;
                    }
                
            }
            Console.WriteLine();
            Console.WriteLine("Minimal param:" + minP);
            Console.WriteLine("Minimal sum of sqares:" + minSum);

            Console.ReadKey();
        }
    }
}
