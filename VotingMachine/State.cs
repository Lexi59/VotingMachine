using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingMachine
{
    class State
    {
        public string stateAbbrv;
        public ConcurrentQueue<Ballot> ballots;

        public State (string abbr)
        {
            stateAbbrv = abbr;
            ballots = new ConcurrentQueue<Ballot>();
        }
    }
}
