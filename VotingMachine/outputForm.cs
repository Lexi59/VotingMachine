using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
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
            statesDropdown.Items.Add("Overall");
            foreach (string state in stateAbbreviations)
            {
                statesDropdown.Items.Add(state);
                states.Add(new State(state));
            }
            sortByBox.Items.Add("Gender");
            sortByBox.Items.Add("Party");

            List<string> lines = System.IO.File.ReadAllLines(@"..\..\Resources\ballots.txt").ToList<string>();

            /* foreach (String line in lines)
                      foreach(State state in states)
                          if (line.Split(' ')[3] == state.stateAbbrv)
                          {
                              state.ballots.Enqueue(new Ballot(line));
                              break;
                          }          
                                                                                                        */
            Parallel.ForEach(lines, line =>
            {
                foreach (State state in states)
                {
                    if (line.Split(' ')[3] == state.stateAbbrv)
                    {
                        state.ballots.Enqueue(new Ballot(line));
                        break;
                    }
                }
            });
        }

        private void goButtonClick(object sender, EventArgs e)
        {
            int numStates;
            int state = statesDropdown.SelectedIndex;
            if (statesDropdown.SelectedIndex > 0)
            {
                numStates = 1;
                state--;
            }
            else
            {
                numStates = 50;
            }

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

                Action<Ballot> calcVotesByGender = (ballot) =>
                {
                    if (ballot.male)
                        Interlocked.Increment(ref votesByGender[ballot.candidateVote - 1, 0]);
                    else
                        Interlocked.Increment(ref votesByGender[ballot.candidateVote - 1, 1]);
                };



                //sequential version of counting votes by gender
                /*
                if (state == 0)
                {
                for (int s = 0; s < 50; s++)
                {
                List<Ballot> stateBallots = states[s].ballots.ToList();
                for (int x = 0; x < stateBallots.Count; x++)
                calcVotesByGender(stateBallots[x]);
                }
                }
                else
                {
                state--;
                List<Ballot> stateBallots = states[state].ballots.ToList();
                for (int x = 0; x < stateBallots.Count; x++)
                calcVotesByGender(stateBallots[x]);
                }
                */

                //parallel version of counting votes by gender 
                Parallel.For(0, numStates, s =>
                {
                    List<Ballot> stateBallots = states[s].ballots.ToList();
                    Parallel.For(0, stateBallots.Count, x =>
                    {
                        calcVotesByGender(stateBallots[x]);
                    });
                });


                var totals = new int[2];
                for (var i = 0; i < Ballot.numberOfCandidates; i++)
                {
                    resultsBox.Items.Add(new ListViewItem(new string[] { "Candidate " + (i + 1), votesByGender[i, 0].ToString(), votesByGender[i, 1].ToString(), (votesByGender[i, 0] + votesByGender[i, 1]).ToString() }));
                    totals[0] += votesByGender[i, 0];
                    totals[1] += votesByGender[i, 1];
                    resultsBox.Items[i].UseItemStyleForSubItems = false;
                    resultsBox.Items[i].SubItems[3].BackColor = Color.LightGray;
                }
                resultsBox.Items.Add(new ListViewItem(new string[] { "Totals:", totals[0].ToString(), totals[1].ToString(), (totals[0] + totals[1]).ToString() }));
                resultsBox.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                resultsBox.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Items[resultsBox.Items.Count - 1].BackColor = Color.LightGray;

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

                Action<Ballot> calcVotesByParty = (ballot) =>
                {
                    if (ballot.democrat)
                        Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 0]);
                    else if (ballot.republican)
                        Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 1]);
                    else
                        Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 2]);
                };


                //sequential version of counting votes by party 
                /*
                if (state == 0)
                {
                for (int s = 0; s < 50; s++)
                {
                List<Ballot> stateBallots = states[s].ballots.ToList();
                for (int x = 0; x < stateBallots.Count; x++)
                calcVotesByParty(stateBallots[x]);
                }
                }
                else
                {
                state--;
                List<Ballot> stateBallots = states[state].ballots.ToList();
                for (int x = 0; x < stateBallots.Count; x++)
                calcVotesByParty(stateBallots[x]);
                }
                */

                //parallel version of counting votes by party 
                Parallel.For(0, numStates, s =>
                {
                    List<Ballot> stateBallots = states[s].ballots.ToList();
                    Parallel.For(0, stateBallots.Count, x =>
                    {
                        calcVotesByParty(stateBallots[x]);
                    });
                });

                var totals = new int[3];
                for (var i = 0; i < Ballot.numberOfCandidates; i++)
                {
                    resultsBox.Items.Add(new ListViewItem(new string[] { "Candidate " + (i + 1), votesByParty[i, 0].ToString(), votesByParty[i, 1].ToString(), votesByParty[i, 2].ToString(), (votesByParty[i, 0] + votesByParty[i, 1] + votesByParty[i, 2]).ToString() }));
                    totals[0] += votesByParty[i, 0];
                    totals[1] += votesByParty[i, 1];
                    totals[2] += votesByParty[i, 2];
                    resultsBox.Items[i].UseItemStyleForSubItems = false;
                    resultsBox.Items[i].SubItems[4].BackColor = Color.LightGray;
                }

                resultsBox.Items.Add(new ListViewItem(new string[] { "Totals:", totals[0].ToString(), totals[1].ToString(), totals[2].ToString(), (totals[0] + totals[1] + totals[2]).ToString() }));
                resultsBox.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                resultsBox.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[2].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[3].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Columns[4].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                resultsBox.Items[resultsBox.Items.Count - 1].BackColor = Color.LightGray;
            }
        }
    }
}
