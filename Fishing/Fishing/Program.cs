using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fishing
{
    static class helpers
    {
        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
    }

    class Program
    {
        const int SCORE1 = 3;
        const int SCORE2 = 7;
        const string PARAM = "Space";
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

            int maxHits = 0;
            int maxHitsP = -1;

            IEnumerable<string> lines = System.IO.File.ReadLines("positions.txt");

            for (int i = LOWER_BOUND; i < UPPER_BOUND; i += STEP)
            {
                int bmHits = 0;

                Console.WriteLine("" + PARAM + " = " + i + ":");
                    int sum = 0;
                    foreach (String fen in lines)
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
                        System.Diagnostics.Debug.WriteLine("fen = " + fen);
                        string move = l.Split(' ')[1];
                        System.Diagnostics.Debug.WriteLine("move:" + move);
                        string source = move.Substring(0, 2);
                        string dest = move.Substring(2, 2);
                        string piece = PieceAt(fen, source).ToUpper();
                        System.Diagnostics.Debug.WriteLine("our move:" + piece + "-" + dest);

                        var sp = fen.Split(' ').ToList();
                        int ind = sp.IndexOf("bm");
                        string bm = sp[ind + 1];
                        bm = bm.Replace(";", "");
                        bm = bm.Replace("+", "");
                        bm = bm.Replace("x", "");

                        System.Diagnostics.Debug.WriteLine("bm:" + bm);
                        string bmDest = bm.GetLast(2);
                        //System.Diagnostics.Debug.WriteLine("bmdest:" + bmDest);
                        if (dest == bmDest &&
                            (piece.ToCharArray()[0] == bm[0] || piece == "P"))
                        {
                            bmHits++;
                            //System.Diagnostics.Debug.WriteLine("Our move is the best one");
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
                    Console.Write("\tNum of BestMove hits:\t");
                    Console.WriteLine(bmHits);
                    if (sum < minSum)
                    {
                        minSum = sum;
                        minP = i;
                    }

                    if (bmHits > maxHits)
                    {
                        maxHits = bmHits;
                        maxHitsP = i;
                    }
                
            }
            Console.WriteLine();
            Console.WriteLine("Minimal param:" + minP);
            Console.WriteLine("Minimal sum of sqares:" + minSum);
            Console.WriteLine("From BestMove hits:");
            Console.WriteLine("Best param:" + maxHitsP);
            Console.WriteLine("Maximal hits:" + maxHits);

            Console.ReadKey();
        }

        private static string PieceAt(string fen, string loc)
        {
            // row, from up to down (black to white)
            int row = 9 - int.Parse(loc.Substring(1,1));
            //System.Diagnostics.Debug.WriteLine("row is " + row);
            int col = loc[0] - 'a' + 1;
            //System.Diagnostics.Debug.WriteLine("col is " + col);

            string fenrow = fen.Split('/')[row - 1].Split(' ')[0];
            //System.Diagnostics.Debug.WriteLine("fenrow is " + fenrow);
            int c = 1;
            for (int i = 0; c <= 8; i++)
            {
                int res = 0;
                if (int.TryParse(fenrow.Substring(i, 1), out res))
                    c += res;
                else if (c == col)
                    return fenrow.Substring(i, 1);
                else
                    c++;
                
            }
            return null;
        }

    }
}
