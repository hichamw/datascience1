using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace DataScience1Assigment1
{
    class Program
    {
        static void Main(string[] args)
        {
            string userItemName = "userItem.data";
            string parentFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string userItemPath = Path.Combine(parentFolder, userItemName);
            string outputFile = Path.Combine(parentFolder, "write.txt");
            Array text = File.ReadAllLines(userItemPath);

            Dictionary<string, Dictionary<string, string>> userPreferences =
                new Dictionary<string, Dictionary<string, string>>();

            foreach (string line in text)
            {
                var splitLine = line.Split(',');
                string user = splitLine[0];
                string item = splitLine[1];
                string rating = splitLine[2];

                if (userPreferences.ContainsKey(user))
                {
                    if (!userPreferences[user].ContainsKey(item))
                    {
                        userPreferences[user].Add(item, rating);
                    }
                }
                else
                {
                    userPreferences.Add(user, new Dictionary<string, string>());
                    userPreferences[user].Add(item, rating);
                }
            }
            
            Console.WriteLine(CalculateSimilarity("pearson", userPreferences["3"], userPreferences["4"]));

            foreach (var user in userPreferences)
            {
                using (StreamWriter writetext = new StreamWriter(outputFile, true))
                {
                    writetext.WriteLine("User {0} gave these ratings:", user.Key);
                    foreach (var rating in user.Value)
                    {
                        writetext.WriteLine("{0}:{1}", rating.Key, rating.Value);
                    }
                }
            }
        }

        private static double CalculateSimilarity(string strategy, Dictionary<string, string> user1,
            Dictionary<string, string> user2)
        {
            double answer = 0;
            switch (strategy)
            {
                case "euclidean":
                    answer = EuclideanSimilarity(user1, user2);
                    break;
                case "pearson":
                    answer = PearsonSimilarity(user1, user2);
                    break;
                case "cosine":
                    answer = CosineSimilarity(user1, user2);
                    break;
            }

            return answer;
        }

        private static double EuclideanSimilarity(Dictionary<string, string> user1, Dictionary<string, string> user2)
        {
            double answer = 0.0;

            var ratingsSum = 0.0;
            foreach (var rating in user1)
            {
                if (user2.ContainsKey(rating.Key))
                {
                    ratingsSum += Math.Pow(Convert.ToDouble(rating.Value) - Convert.ToDouble(user2[rating.Key]),
                        2.0);
                }
            }

            var distance = Math.Sqrt(ratingsSum);

            answer = 1 / (distance + 1);

            return answer;
        }

        private static double PearsonSimilarity(Dictionary<string, string> user1, Dictionary<string, string> user2)
        {
            double sumUser1 = 0;
            double sumUser2 = 0;
            double sumPowUser1 = 0;
            double sumPowUser2 = 0;
            double topLeft = 0;

            int count = 0;

            foreach (var rating in user1)
            {
                if (user2.ContainsKey(rating.Key))
                {
                    count++;
                    topLeft += Convert.ToDouble(rating.Value) * Convert.ToDouble(user2[rating.Key]);
                    sumUser1 += Convert.ToDouble(rating.Value);
                    sumUser2 += Convert.ToDouble(user2[rating.Key]);
                    sumPowUser1 += Math.Pow(Convert.ToDouble(rating.Value), 2.0);
                    sumPowUser2 += Math.Pow(Convert.ToDouble(user2[rating.Key]), 2.0);
                }
            }

            double topRight = (sumUser1 * sumUser2) / count;

            double bottomLeft = Math.Sqrt(sumPowUser1 - Math.Pow(sumUser1, 2.0) / count);
            double bottomRight = Math.Sqrt(sumPowUser2 - Math.Pow(sumUser2, 2.0) / count);
            
            var answer = (topLeft - topRight) / (bottomLeft * bottomRight);

            return answer;
        }

        private static double CosineSimilarity(Dictionary<string, string> user1, Dictionary<string, string> user2)
        {
            double top = 0;
            double bottomLeft = 0;
            double bottomRight = 0;

            foreach (var rating in user1)
            {
                if (user2.ContainsKey(rating.Key))
                {
                    double rating1 = Convert.ToDouble(rating.Value);
                    double rating2 = Convert.ToDouble(user2[rating.Key]);
                    
                    top += (rating1 * rating2);
                    bottomLeft += Math.Pow(rating1, 2.0);
                    bottomRight += Math.Pow(rating2, 2.0);
                }
            }
            bottomLeft = Math.Sqrt(bottomLeft);
            bottomRight = Math.Sqrt(bottomRight);
            double bottom = bottomLeft * bottomRight;

            double answer = top / bottom;
            
            return answer;
        }
    }
}