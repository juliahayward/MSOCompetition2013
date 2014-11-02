using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> palindromes = new Dictionary<string, int>();
            var sr = new StreamReader(new FileStream("Data\\minidictionary.txt", FileMode.Open, FileAccess.Read));
            while (!sr.EndOfStream)
            {
                var word = sr.ReadLine().ToLower();
                label1.Text = word;
                Application.DoEvents();
                if (IsPalindrome(word))
                {
                    if (!palindromes.ContainsKey(word))
                        palindromes.Add(word, 0);
                    palindromes[word]++;
                }
            }
            label1.Text = "";
            sr.Close();
            listBox1.Items.Clear();
            foreach (var word in palindromes.Keys)
            {
                listBox1.Items.Add(word + " (" + palindromes[word] + " occurrences)" + Environment.NewLine);
            }
            Application.DoEvents();
        }

        private string Reverse(string word)
        {
            return string.Join("", word.ToArray().Reverse());
        }

        private bool IsPalindrome(string word)
        {
            var reverse = string.Join("", word.ToArray().Reverse());
            return reverse.Equals(word);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var numberRep = numBox.Text;
            var nbase = int.Parse(baseBox.Text);
            var newstring = "";
            for (int i = 0; i < 10; i++)
            {
                var number = stringToInt(numberRep, nbase);
                var reverse = stringToInt(Reverse(numberRep), nbase);
                newstring = intToString(number + reverse, nbase);

                if (IsPalindrome(newstring))
                {
                    MessageBox.Show(newstring);
                    return;
                }

                numberRep = newstring;
            }
            
            MessageBox.Show("NONE," + newstring);
        }
        
        private long stringToInt(string n, int b)
        {
            long i = 0;
            long mult = 1;
            foreach (var digit in n.Reverse().ToArray())
            {
                i += int.Parse("" + digit) * mult;
                mult *= b;
            }
            return i;

        }

        private string intToString(long l, int b)
        {
            string s = "";
            while (l > 0)
            {
                var rem = l % b;
                s += rem;
                l = (l - rem) / b;
            }
            return string.Join("", s.Reverse());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            Dictionary<string, int> individuals = new Dictionary<string,int>();
            Dictionary<string, List<double>> markers = new Dictionary<string, List<double>>();
            var sr = new StreamReader(new FileStream("Data\\trainingGenes.csv", FileMode.Open, FileAccess.Read));
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var parts = line.Split(',');
                if (string.IsNullOrEmpty(parts[0]))
                    continue;
                if (parts[0] == "\"eth\"")
                    continue;
                
                if (!individuals.ContainsKey(parts[0]))
                {
                    individuals.Add(parts[0], 1);
                    var list = new List<double>();
                    for (int i = 1; i < parts.Length; i++)
                    {
                        if (parts[i] == "\"?\"")
                            list.Add(0.5);
                        else list.Add(int.Parse(parts[i]));
                    }
                    markers.Add(parts[0], list);
                }
                else
                {
                    individuals[parts[0]]++;
                    var list = new List<double>();
                    for (int i = 1; i < parts.Length; i++)
                    {
                        if (parts[i] == "\"?\"")
                            list.Add(0.5);
                        else list.Add(int.Parse(parts[i]));
                    }
                    var oldList = markers[parts[0]];
                    var sumList = new List<double>();
                    for (int i = 0; i < oldList.Count; i++)
                    {
                        sumList.Add(oldList.ElementAt(i) + list.ElementAt(i));
                    }
                    markers[parts[0]] = sumList;
                }

            }
            sr.Close();

            sr = new StreamReader(new FileStream("Data\\testcases.csv", FileMode.Open, FileAccess.Read));
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var parts = line.Split(',');
                var person = parts[0];
                if (string.IsNullOrEmpty(person))
                    continue;
                if (parts[0] == "\"eth\"")
                    continue;
                // Person's array
                var list = new List<double>();
                for (int i = 1; i < parts.Length; i++)
                {
                    if (parts[i] == "\"?\"")
                        list.Add(0.5);
                    else list.Add(int.Parse(parts[i]));
                }
                
                Dictionary<string, double> scores = new Dictionary<string,double>();
                foreach (var eth in markers.Keys)
                {
                    double simScore = 0;
                    for (int j = 0; j < list.Count; j++)
                    {
                        simScore += Math.Abs(list.ElementAt(j) 
                            - (markers[eth].ElementAt(j) / individuals[eth])); 
                    }
                    scores.Add(eth, simScore);
                }

                var best = scores.OrderBy(x => x.Value).First().Key;
                listBox2.Items.Add("Person " + person.Replace("\"", "") + " is " + best.Replace("\"", ""));
                foreach (var option in scores.OrderBy(x => x.Value))
                    listBox2.Items.Add("     " + option.Key + " scores " + Math.Round(option.Value, 2));
            }
            sr.Close();
        }

        private List<int> squares = new List<int>();
        private List<int> players = new List<int>();
        private List<int> mine = new List<int>();
        string result = "";

        private void button4_Click(object sender, EventArgs e)
        {
            squares.Clear();
            for (int i = 1; i < 10; i++) squares.Add(i);
            players.Clear();
            mine.Clear();
            UpdateUI();
            result = "";
        }


        private void UpdateUI()
        {
            label5.Text = "Remaining: " + string.Join(",", squares.ToArray());
            label7.Text = "Your numbers " + string.Join(",", players.ToArray());
            label8.Text = "My numbers " + string.Join(",", mine.ToArray());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                var playerMove = int.Parse(textBox1.Text);
                if (playerMove == 0)
                {
                    if (squares.Count != 9)
                    {
                        MessageBox.Show("Game already started");
                        return;
                    }
                }
                else
                {
                    if (!squares.Contains(playerMove))
                    {
                        MessageBox.Show("Illegal move");
                        return;
                    }
                    squares.Remove(playerMove);
                    players.Add(playerMove);
                }

                if (PlayerHasWon())
                {
                    MessageBox.Show("You win");
                    result = "Player win";
                }
                else
                {
                    MakeMachineMove();
                    if (MachineHasWon())
                    {     MessageBox.Show("I win");
                    result = "Machine win";

                    }
                }
                if (!squares.Any() && result == "")
                {
                    MessageBox.Show("Draw");
                    result = "draw";
                }


                UpdateUI();
            }
            catch (Exception) { MessageBox.Show("Huh?"); }
        }

        private void MakeMachineMove()
        {
            if (squares.Any())
            {
                var move = EvaluateBest();
                squares.Remove(move.Square);
                mine.Add(move.Square);
                MessageBox.Show("I take " + move.Square);
            }
        }

        private bool PlayerHasWon()
        {
            return HasWon(players) ;
        }

        private bool MachineHasWon()
        {
            return HasWon(mine);
        }

        private Move EvaluateBest()
        {
            // Just to make sure evaluation doesn't take too long - play centre if your go first.
            if (squares.Count == 9)
                return new Move { Square = 5, Value = 0 };

            var moves = new Dictionary<int, int>();
            foreach (var move in squares.ToArray())
            {
                squares.Remove(move);
                mine.Add(move);
                moves.Add(move, Evaluate(false));
                mine.Remove(move);
                squares.Add(move);
            }
            var bestMove = moves.OrderBy(x => x.Value).Last().Key;
            return new Move { Square = bestMove, Value = moves[bestMove] };
        }

        private Move EvaluateWorst()
        {
            var moves = new Dictionary<int, int>();
            foreach (var move in squares.ToArray())
            {
                squares.Remove(move);
                players.Add(move);
                moves.Add(move, Evaluate(true));
                players.Remove(move);
                squares.Add(move);
            }
            var bestMove = moves.OrderBy(x => x.Value).First().Key;
            return new Move() { Square = bestMove , Value = moves[bestMove] };
        }

        private int Evaluate(bool isMachineMove)
        {
            if (MachineHasWon())
                return 1;
            if (PlayerHasWon())
                return -1;
            if (!squares.Any())
                return 0;
            if (isMachineMove)
            {
                return EvaluateBest().Value;
            }
            else
            {
                return EvaluateWorst().Value;
            }
        }


        private bool HasWon(List<int> s)
        {
            // Not the most attractive, but as the game is isomorphic to tictactoe we know there
            // are only eight winning combinations
            if (s.Contains(4) && s.Contains(3) && s.Contains(8)) return true;
            if (s.Contains(9) && s.Contains(5) && s.Contains(1)) return true;
            if (s.Contains(2) && s.Contains(7) && s.Contains(6)) return true;
            if (s.Contains(4) && s.Contains(9) && s.Contains(2)) return true;
            if (s.Contains(3) && s.Contains(5) && s.Contains(7)) return true;
            if (s.Contains(8) && s.Contains(1) && s.Contains(6)) return true;
            if (s.Contains(4) && s.Contains(5) && s.Contains(6)) return true;
            if (s.Contains(8) && s.Contains(5) && s.Contains(2)) return true;
            return false;
        }
    }

    public class Move { public int Square { get; set; } public int Value { get; set; } }
}
