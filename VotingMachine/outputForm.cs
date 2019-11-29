using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VotingMachine
{
    public partial class outputForm : Form
    {
        private List<State> states;
        public outputForm()
        {
            InitializeComponent();
            states = new List<State>();
            List<string> stateAbbreviations = new List<string> { "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA", "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ", "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY" };
            statesDropdown.Items.Add("Overalll");
            foreach(string state in stateAbbreviations)
            {
                statesDropdown.Items.Add(state);
                states.Add(new State(state));
            }
            sortByBox.Items.Add("Gender");
            sortByBox.Items.Add("Party");

            List<string> lines = System.IO.File.ReadAllLines(@"..\..\Resources\ballots.txt").ToList<string>();
            Parallel.ForEach(lines, line =>
            {
                Parallel.ForEach(states, state =>
                {
                    if (line.Split(' ')[3] == state.stateAbbrv)
                    {
                        state.ballots.Add(new Ballot(line));
                    }
                });
            });
        }

        private void goButtonClick(object sender, EventArgs e)
        {
            State currentState = states[statesDropdown.SelectedIndex - 1];

            resultsBox.Items.Clear();
            resultsBox.Columns.Clear();

            if (sortByBox.SelectedItem == null)
            {
                resultsBox.Items.Add("No sorting option picked");
            }
            else if (sortByBox.SelectedItem.ToString() == "Gender")
            {
                resultsBox.Columns.Add("Candidate");
                resultsBox.Columns.Add("Male");
                resultsBox.Columns.Add("Female");
                resultsBox.Columns.Add("Total");

                int[,] votesByGender = new int[Ballot.numberOfCandidates, 2];
                for (int i = 0; i < Ballot.numberOfCandidates; i++)
                {
                    votesByGender[i, 0] = 0;
                    votesByGender[i, 1] = 0;
                }

                Action<Ballot> calcVotesByGender = (ballot) => {
                    if (ballot.male)
                        votesByGender[ballot.candidateVote - 1, 0]++;
                    else
                        votesByGender[ballot.candidateVote - 1, 1]++;
                };

                var tasks = Enumerable.Range(0, currentState.ballots.Count)
                                      .Select(x => Task.Factory.StartNew(()=>calcVotesByGender(currentState.ballots[x])))
                                      .ToArray();
                Task.WaitAll(tasks);

                for(var i = 0; i < Ballot.numberOfCandidates; i++)
                {
                    resultsBox.Items.Add(new ListViewItem(new string[] {"Candidate " + (i + 1), votesByGender[i, 0].ToString(), votesByGender[i, 1].ToString(), (votesByGender[i,0]+votesByGender[i,1]).ToString() }));
                }
                resultsBox.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                resultsBox.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            else if (sortByBox.SelectedItem.ToString() == "Party")
            {
                resultsBox.Columns.Add("Candidate");
                resultsBox.Columns.Add("Democrat");
                resultsBox.Columns.Add("Republican");
                resultsBox.Columns.Add("Independent");
                resultsBox.Columns.Add("Total");

                int[,] votesByParty = new int[Ballot.numberOfCandidates, 3];
                for (int i = 0; i < Ballot.numberOfCandidates; i++)
                {
                    votesByParty[i, 0] = 0;
                    votesByParty[i, 1] = 0;
                    votesByParty[i, 2] = 0;
                }

                Action<Ballot> calcVotesByGender = (ballot) => {
                    if (ballot.democrat)
                        votesByParty[ballot.candidateVote - 1, 0]++;
                    else if (ballot.republican)
                        votesByParty[ballot.candidateVote - 1, 1]++;
                    else
                        votesByParty[ballot.candidateVote - 1, 2]++;
                };

                var tasks = Enumerable.Range(0, currentState.ballots.Count)
                                      .Select(x => Task.Factory.StartNew(() => calcVotesByGender(currentState.ballots[x])))
                                      .ToArray();
                Task.WaitAll(tasks);

                for (var i = 0; i < Ballot.numberOfCandidates; i++)
                {
                    resultsBox.Items.Add(new ListViewItem(new string[] { "Candidate " + (i + 1), votesByParty[i, 0].ToString(), votesByParty[i, 1].ToString(), votesByParty[i,2].ToString(),(votesByParty[i, 0] + votesByParty[i, 1] + votesByParty[i,2]).ToString() }));
                }
                resultsBox.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                resultsBox.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[4].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }

        }
    }
}
