using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingMachine
{
    class State
    {
        public string stateAbbrv;
        public List<Ballot> ballots;

        public State (string abbr)
        {
            stateAbbrv = abbr;
            ballots = new List<Ballot>();
        }
    }
}
