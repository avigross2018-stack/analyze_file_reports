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
                if (splitContent.Count == 0)
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
            foreach (string line in fileContent)
            {
                tempLine = [.. line.Split(",")];
                if (tempLine.Count > 5 || tempLine.Count < 5) { continue; }
                tempLine = tempLine.ConvertAll(x => x.ToLower().Trim());
                cleanData.Add(string.Join(",", tempLine));

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
            int lineCount = -1;

            foreach (string line in cleanData)
            {
                lineCount += 1;
                tempLine = [.. line.Split(",")];
                string name = tempLine[0];
                string rep = tempLine[1];
                string stat = tempLine[4];
                if (!int.TryParse(tempLine[2], out int pri))
                {
                    System.Console.WriteLine($"Invalid record (Priority not INT), LINE {lineCount}.");
                    continue;
                }

                if (pri > 5 || pri < 1)
                {
                    System.Console.WriteLine($"Invalid record (Primary not in range 1-5), LINE {lineCount}.");
                    continue;
                }

                if (!double.TryParse(tempLine[3], out double sco))
                {
                    System.Console.WriteLine($"Invalid record (Score not Double), LINE {lineCount}.");
                    continue;
                }

                if (sco > 100.0 || sco < 0.0)
                {
                    System.Console.WriteLine($"Invalid record (Score not in range 0.0-100.0), LINE {lineCount}.");
                    continue;
                }

                if (!Enum.TryParse(rep, true, out ReportType reportResult))
                {
                    System.Console.WriteLine($"Invalid record (Invalid ReportType), LINE {lineCount}.");
                    continue;
                }

                if (!Enum.TryParse(stat, true, out Status statusResult))
                {
                    System.Console.WriteLine($"Invalid record (Invalid Status), LINE {lineCount}.");
                    continue;
                }

                unitName[index] = name;
                reportType[index] = rep.ToLower();
                priority[index] = pri;
                score[index] = sco;
                status[index] = stat.ToLower();

                index++;

            }
            listLen = index;
            System.Console.WriteLine($"{index + 1} processed record Valid.");
        }


        static void CalculateAverage(double[] score,
                                        int listLen)
        {
            double total = 0;
            for (int i = 0; i < listLen; i++)
            {
                total += score[i];
            }
            double avg = total / listLen;
            System.Console.WriteLine($"Average {avg:F2}");
        }


        static void FindMaxScore(double[] score, int listLen)
        {
            double maxScore = 0;
            for (int i = 0; i < listLen; i++)
            {
                if (score[i] > maxScore) { maxScore = score[i]; }
            }
            System.Console.WriteLine($"Max Score {maxScore}");
        }


        static void FindMinScore(double[] score, int listLen)
        {
            double minScore = score[0];
            for (int i = 0; i < listLen; i++)
            {
                if (score[i] < minScore) { minScore = score[i]; }
            }
            System.Console.WriteLine($"Min Score {minScore}");
        }


        static void CountByStatus(string[] status, string statusChoice, int listLen)
        {
            if (!Enum.TryParse(statusChoice, true, out Status statusResult))
            {
                System.Console.WriteLine($"Invalid Status."); return;
            }
            int statusCounter = 0;
            for (int i = 0; i < listLen; i++)
            {
                if (status[i] == null) { continue; }
                if (status[i].Equals(statusChoice.ToLower())) { statusCounter += 1; }
            }
            System.Console.WriteLine($"Count Status {statusChoice} {statusCounter}");
        }

        static void CountByType(string[] reportType, string report, int listLen)
        {
            if (!Enum.TryParse(report, true, out ReportType reportResult))
            {
                System.Console.WriteLine($"Invalid ReportType.");
                return;
            }
            int reportCounter = 0;
            for (int i = 0; i < listLen; i++)
            {
                if (reportType[i] == null) { continue; }
                if (reportType[i].Equals(report.ToLower())) { reportCounter += 1; }
            }
            System.Console.WriteLine($"Count Type {report} {reportCounter}");

        }

        static void DisplayBasicStatistics(double[] score,
                                            int listLen)
        {
            System.Console.WriteLine("== Report Statistics ==");
            System.Console.WriteLine($"Total Reports {listLen}.");
            CalculateAverage(score, listLen);
            FindMaxScore(score, listLen);
            FindMinScore(score, listLen);
        }

        static void DisplayStatusCounts(string[] status, int listLen)
        {
            System.Console.WriteLine("== Status Counts ==");
            CountByStatus(status, "Pending", listLen);
            CountByStatus(status, "Approved", listLen);
            CountByStatus(status, "Rejected", listLen);
        }

        static void DisplayTypeCounts(string[] reportType, int listLen)
        {
            System.Console.WriteLine("== Type Counts ==");
            CountByType(reportType, "Collect", listLen);
            CountByType(reportType, "Analyze", listLen);
            CountByType(reportType, "Recon", listLen);
            CountByType(reportType, "Intel", listLen);
        }


        static void DisplayHighestPriorityApproved(string[] unitName,
                                string[] reportType,
                                int[] priority,
                                double[] score,
                                string[] status,
                                int listLen)
        {
            int pri = 0;
            int reportIndex = 0;
            for (int i = 0; i < listLen; i++)
            {

                if (status[i] == "approved")
                {
                    if (priority[i] > pri)
                    {
                        pri = priority[i];
                        reportIndex = i;
                    }
                }
            }
            System.Console.WriteLine("== Highest Priority Approved ==");
            System.Console.WriteLine($"UNIT: {unitName[reportIndex]}");
            System.Console.WriteLine($"REPORT TYPE: {reportType[reportIndex]}");
            System.Console.WriteLine($"PRIORITY: {priority[reportIndex]}");
            System.Console.WriteLine($"SCORE: {score[reportIndex]}");
            System.Console.WriteLine($"STATUS: {status[reportIndex]}");
        }


        static void DisplayAverageByPriority(int[] priority, double[] score, int listLen)
        {

            double[] scoreTotal = new double[6];
            int[] priorityAmount = new int[6];

            for (int i = 0; i < listLen; i++)
            {
                int p = priority[i];

                scoreTotal[p] += score[i];
                priorityAmount[p] += 1;
            }
            System.Console.WriteLine("== Average By Priority ==");
            for (int i = 1; i < scoreTotal.Length; i++)
            {
                if (priorityAmount[i] == 0)
                {
                    System.Console.WriteLine($"Priority {i} avg 0");
                    continue;
                }
                System.Console.WriteLine($"Priority {i} avg {scoreTotal[i] / priorityAmount[i]:F2}");
            }
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
            if (x == null) { return; }
            ProcessReports(x, unitName, reportType, priority, score, status, out int listLen);
            DisplayBasicStatistics(score, listLen);
            DisplayStatusCounts(status, listLen);
            DisplayTypeCounts(reportType, listLen);
            DisplayHighestPriorityApproved(unitName, reportType, priority, score, status, listLen);
            DisplayAverageByPriority(priority, score, listLen);

        }
    }
}
