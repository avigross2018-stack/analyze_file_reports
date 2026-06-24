using System;
using System.IO;

namespace MyFirstProject
{

    enum ReportType
    {
        Collect,
        Analyze,
        Recon,
        Intel
    }

    enum Status
    {
        Pending,
        Approved,
        Rejected
    }

    class Program
    {

        static List<string>? LoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] content = File.ReadAllLines(filePath);
                List<string> splitContent = [.. content];
                System.Console.WriteLine(splitContent);
                if(splitContent.Count == 0)
                {
                    System.Console.WriteLine("ERROR: Empty file.");
                    return null;
                }
                return splitContent;
            }
            System.Console.WriteLine($"ERROR: File {Path.GetFileName(filePath)} Not found.");
            return null;
        }


        static List<string> CleanUnFormatLines(List<string> fileContent)
        {
            List<string> cleanData = new List<string>();
            List<string> tempLine = new List<string>();
            foreach(string line in fileContent)
            {
                tempLine = [.. line.Split(",")];
                if(tempLine.Count > 5 || tempLine.Count < 5){continue;}
                tempLine = tempLine.ConvertAll(x => x.ToLower().Trim());
                cleanData.Add(string.Join(", ", tempLine));
                
            }
            return cleanData;
        }


        static void ProcessReports(
                                List<string> fileContent,
                                string[] unitName,
                                string[] reportType,
                                int[] priority,
                                double[] score,
                                string[] status,
                                out int listLen)
        {
            List<string> cleanData = CleanUnFormatLines(fileContent);
            List<string> tempLine = new List<string>();
            int index = 0;
            
            foreach(string line in cleanData)
            {
                tempLine = [.. line.Split(",")];
                string name = tempLine[0];
                string rep = tempLine[1];
                string stat = tempLine[4];
                if(!int.TryParse(tempLine[2], out int pri)){
                    System.Console.WriteLine($"Invalid record (Priority not INT).");
                    continue;}

                if(pri > 5 || pri < 1){
                    System.Console.WriteLine($"Invalid record (Primary not in range 1-5).");
                    continue;}

                if(!double.TryParse(tempLine[3], out double sco)){
                    System.Console.WriteLine($"Invalid record (Score not Double).");
                    continue;}
                    
                if(sco > 100.0 || sco < 0.0){
                    System.Console.WriteLine($"Invalid record (Score not in range 0.0-100.0).");
                    continue;}

                if(!Enum.TryParse(rep, true, out ReportType reportResult)){
                    System.Console.WriteLine($"Invalid record (Invalid ReportType).");
                    continue;}

                if(!Enum.TryParse(stat, true, out Status statusResult)){
                    System.Console.WriteLine($"Invalid record (Invalid Status).");
                    continue;}

                unitName[index] = name;
                reportType[index] = rep.ToLower();
                priority[index] = pri;
                score[index] = sco;
                status[index] = stat.ToLower();

                index ++;
                
            }
            listLen = index;
            System.Console.WriteLine($"{index + 1} processed record Valid.");
        }


        static void CalculateAverage(double[] score,
                                        int listLen)
        {
            double total = 0;
            foreach (var item in score)
            {
                total += item;
            }
            double avg = total / listLen;
            System.Console.WriteLine($"Average {avg:F2}");
        }


        static void FindMaxScore(double[] score)
        {
            double maxScore = 0;
            foreach (var item in score)
            {
                if(item > maxScore){maxScore = item;}
            }
            System.Console.WriteLine($"Max Score {maxScore}");
        }


        static void FindMinScore(double[] score)
        {
            double minScore = score[0];
            foreach (var item in score)
            {
                if(item < minScore){minScore = item;}
            }
            System.Console.WriteLine($"Min Score {minScore}");
        }


        static void Main(string[] args)
        {
            const int MAX_REPORTS = 100;
            const string FILE_PATH = "reports.txt";
            
            string[] unitName = new string[MAX_REPORTS];
            string[] reportType = new string[MAX_REPORTS];
            int[] priority = new int[MAX_REPORTS]; // 1-5
            double[] score = new double[MAX_REPORTS]; // 0.0 - 100.0
            string[] status = new string[MAX_REPORTS];

            List<string>? x = LoadFile(FILE_PATH);
            ProcessReports(x ,unitName, reportType, priority, score, status, out int listLen);
            CalculateAverage(score, listLen);
            FindMaxScore(score);
            FindMinScore(score);
              
        }
    }
}
