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
            /*
Stopwatch s1 = new Stopwatch();
Stopwatch s2 = new Stopwatch();

s1.Start();
foreach (String line in lines)
foreach(State state in states)
if (line.Split(' ')[3] == state.stateAbbrv)
{
state.ballots.Enqueue(new Ballot(line));
break;
}
s1.Stop();
Console.WriteLine("Ballot Processing ... Seq - " + Convert.ToString(s1.ElapsedTicks));

s2.Start();                                                                                                     */
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
            });                                                                                                             /*
            s2.Stop();
            Console.WriteLine("Ballot Processing ... Par - " + Convert.ToString(s2.ElapsedTicks));                          */
        }

        private void goButtonClick(object sender, EventArgs e)
        {
            int state = statesDropdown.SelectedIndex;

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

                Stopwatch s1 = new Stopwatch();
                Stopwatch s2 = new Stopwatch();
                //    /*
                //sequential version of counting votes by gender                                                                            
                s1.Start();
                if (state == 0)
                {
                    foreach (State tempState in states)
                    {
                        List<Ballot> stateBallots = tempState.ballots.ToList();
                        foreach (Ballot ballot in stateBallots)
                        {
                            if (ballot.male)
                                votesByGender[ballot.candidateVote - 1, 0]++;
                            else
                                votesByGender[ballot.candidateVote - 1, 1]++;
                        }
                    }
                }
                else
                {
                    state--;
                    List<Ballot> stateBallots = states[state].ballots.ToList();
                    foreach (Ballot ballot in stateBallots)
                    {
                        if (ballot.male)
                            votesByGender[ballot.candidateVote - 1, 0]++;
                        else
                            votesByGender[ballot.candidateVote - 1, 1]++;
                    }
                }
                s1.Stop();
                Console.WriteLine("\nGender ... Seq - " + Convert.ToString(s1.ElapsedTicks));

                //parallel version of counting votes by gender  
                s2.Start();                                                                                                                                      //   */
                if (state == 0)
                {
                    Parallel.ForEach(states, tempState =>
                    {
                        List<Ballot> stateBallots = tempState.ballots.ToList();
                        int[,] tempVotesByGender = new int[Ballot.numberOfCandidates, 2];
                        for (int i = 0; i < Ballot.numberOfCandidates; i++)
                        {
                            tempVotesByGender[i, 0] = 0;
                            tempVotesByGender[i, 1] = 0;
                        }

                        Parallel.ForEach(stateBallots, ballot =>
                        {
                            if (ballot.male)
                                Interlocked.Increment(ref tempVotesByGender[ballot.candidateVote - 1, 0]);
                            else
                                Interlocked.Increment(ref tempVotesByGender[ballot.candidateVote - 1, 1]);
                        });

                        for (int i = 0; i < Ballot.numberOfCandidates; i++)
                        {
                            Interlocked.Add(ref votesByGender[i, 0], tempVotesByGender[i, 0]);
                            Interlocked.Add(ref votesByGender[i, 1], tempVotesByGender[i, 1]);
                        }
                    });
                }
                else
                {
                    state--;
                    List<Ballot> stateBallots = states[state].ballots.ToList();
                    Parallel.ForEach(stateBallots, ballot =>
                    {
                        Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                        if (ballot.male)
                            Interlocked.Increment(ref votesByGender[ballot.candidateVote - 1, 0]);
                        else
                            Interlocked.Increment(ref votesByGender[ballot.candidateVote - 1, 1]);
                    });
                }                                                                                                                                                //   /*
                s2.Stop();
                Console.WriteLine("Gender ... Par - " + Convert.ToString(s2.ElapsedTicks));                                                                        // */




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

                Action<Ballot> calcVotesByParty = (ballot) => {
                    if (ballot.democrat)
                        Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 0]);
                    else if (ballot.republican)
                        Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 1]);
                    else
                        Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 2]);
                };

                //           /*
                Stopwatch s1 = new Stopwatch();
                Stopwatch s2 = new Stopwatch();

                //sequential version of counting votes by party
                s1.Start();
                if (state == 0)
                {
                    foreach (State tempState in states)
                    {
                        List<Ballot> stateBallots = tempState.ballots.ToList();
                        foreach (Ballot ballot in stateBallots)
                        {
                            if (ballot.democrat)
                                Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 0]);
                            else if (ballot.republican)
                                Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 1]);
                            else
                                Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 2]);
                        }
                    }
                }
                else
                {
                    state--;
                    List<Ballot> stateBallots = states[state].ballots.ToList();
                    foreach (Ballot ballot in stateBallots)
                    {
                        if (ballot.democrat)
                            Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 0]);
                        else if (ballot.republican)
                            Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 1]);
                        else
                            Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 2]);
                    }
                }
                s1.Stop();
                Console.WriteLine("\nParty ... Seq - " + Convert.ToString(s1.ElapsedTicks));

                //parallel version of counting votes by party   
                s2.Start();                                                                                                                                        //         */    
                if (state == 0)
                {
                    Parallel.ForEach(states, tempState =>
                    {
                        List<Ballot> stateBallots = tempState.ballots.ToList();
                        int[,] tempVotesByParty = new int[Ballot.numberOfCandidates, 3];
                        for (int i = 0; i < Ballot.numberOfCandidates; i++)
                        {
                            tempVotesByParty[i, 0] = 0;
                            tempVotesByParty[i, 1] = 0;
                            tempVotesByParty[i, 2] = 0;
                        }

                        Parallel.ForEach(stateBallots, ballot =>
                        {
                            if (ballot.democrat)
                                Interlocked.Increment(ref tempVotesByParty[ballot.candidateVote - 1, 0]);
                            else if (ballot.republican)
                                Interlocked.Increment(ref tempVotesByParty[ballot.candidateVote - 1, 1]);
                            else
                                Interlocked.Increment(ref tempVotesByParty[ballot.candidateVote - 1, 2]);
                        });

                        for (int i = 0; i < Ballot.numberOfCandidates; i++)
                        {
                            Interlocked.Add(ref votesByParty[i, 0], tempVotesByParty[i, 0]);
                            Interlocked.Add(ref votesByParty[i, 1], tempVotesByParty[i, 1]);
                            Interlocked.Add(ref votesByParty[i, 2], tempVotesByParty[i, 2]);
                        }
                    });
                }
                else
                {
                    state--;
                    List<Ballot> stateBallots = states[state].ballots.ToList();
                    Parallel.ForEach(stateBallots, ballot =>
                    {
                        if (ballot.democrat)
                            Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 0]);
                        else if (ballot.republican)
                            Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 1]);
                        else
                            Interlocked.Increment(ref votesByParty[ballot.candidateVote - 1, 2]);
                    });
                }                                                                                                                                                   //        /*
                s2.Stop();
                Console.WriteLine("Party ... Par - " + Convert.ToString(s2.ElapsedTicks));                                                                           //       */


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
