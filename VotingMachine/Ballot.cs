using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingMachine
{
    class Ballot
    {
        public int id;
        public bool male = false;
        public bool female = false;
        public bool democrat = false;
        public bool republican = false;
        public bool independent = false;
        public string state;
        public int candidateVote;
        static public int numberOfCandidates;
        public Ballot(string line)
        {
            var splitLines = line.Split(' ');
            numberOfCandidates = splitLines.Length - 5;
            id = Int32.Parse(splitLines[0]);
            if (splitLines[1] == "D")
                democrat = true;
            else if (splitLines[1] == "R")
                republican = true;
            else
                independent = true;
            if (splitLines[2] == "M")
                male = true;
            else
                female = true;
            state = splitLines[3];
            for(int i = 4; i < splitLines.Length-1; i++)
            {
                if(splitLines[i] == "1")
                {
                    candidateVote = i - 3;
                }
            }
        }
    }
}
