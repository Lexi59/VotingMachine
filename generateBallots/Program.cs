using System;
using System.Collections.Generic;

namespace generateBallots
{
    class Program
    {
        static void Main(string[] args)
        {
            //Id Party Gender State v v v v v v v

            string[] lines = new string[50000];
            int numberOfVoters = 50000;
            int numberOfCandidates = 7;
            List<string> stateAbbreviations = new List<string> { "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA", "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ", "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY" };
            for (int i = 0; i < numberOfVoters; i++)
            {
                string line = "";
                
                //voter id
                line += (i + 1).ToString();

                //generate party
                var partyNum = new Random().Next(0, 3);
                if (partyNum == 0)
                    line += " R ";
                else if (partyNum == 1)
                    line += " D ";
                else
                    line += " I ";
                
                //generate gender
                if (new Random().Next(0, 2) == 0)
                    line += "M ";
                else
                    line += "F ";

                //generate state
                line += stateAbbreviations[new Random().Next(0, stateAbbreviations.Count)] + " ";

                //generate vote
                int vote = new Random().Next(0, numberOfCandidates);
                for(int j = 0; j < numberOfCandidates; j++)
                {
                    if (j == vote)
                        line += "1 ";
                    else
                        line += "0 ";
                }

                lines[i] = line;
            }

            System.IO.File.WriteAllLines(@"..\..\..\..\VotingMachine\Resources\ballots.txt", lines);
        }
    }
}
