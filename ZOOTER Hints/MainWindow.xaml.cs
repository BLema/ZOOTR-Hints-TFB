using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace ZOOTR_Hints
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dir = System.AppDomain.CurrentDomain.BaseDirectory;
        private Regex rgx = new Regex("[^a-zA-Z0-9 ()-]");
        private List<String> locations;
        private List<String> items;
        private List<String> checks;
        private List<String> songs;
        private List<String> songChecks;
        private List<String> dungeons;
        private List<String> prizes;
        private List<String> rewards;
        private List<String> exclude;
        private string but = "";
        private Dictionary<string, List<String>> TriItems = new Dictionary<string, List<String>>();

        public List<String> Locations
        {
            set { locations = value; }
            get { return locations; }
        }
        public List<String> Items
        {
            set { items = value; }
            get { return items; }
        }
        public List<String> Checks
        {
            set { checks = value; }
            get { return checks; }
        }
        public List<String> Songs
        {
            set { songs = value; }
            get { return songs; }
        }
        public List<String> SongChecks
        {
            get { return songChecks; }
            set { songChecks = value; }
        }
        public List<String> Dungeons
        {
            set { dungeons = value; }
            get { return dungeons; }
        }
        public List<String> Prizes
        {
            set { prizes = value; }
            get { return prizes; }
        }
        public List<String> Rewards
        {
            get
            {
                rewards = new List<string>();
                rewards.AddRange(Items);
                rewards.AddRange(Songs);
                rewards.RemoveAll(t => exclude.Contains(t));
                return rewards;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            dungeons = new List<String>{ "Dodongos", "Deku Tree", "Jabu-Jabu", "Forest", "Fire",
                "Water", "Spirit", "Shadow"};

            prizes = new List<String> { "Light", "Fire", "Water", "Shadow", "Spirit", "Stone" };

            songChecks = new List<String> { "Minuet", "Prelude", "Crater", "Ice Cavern", 
                "Royal Tomb Song", "Requiem", "OoT" };

            TriItems.Add("Wisdom", new List<String>());
            TriItems.Add("Power", new List<String>());
            TriItems.Add("Courage", new List<String>());

            locations = GetList("Locations");
            items = GetList("Items");
            songs = GetList("Songs");
            checks = GetList("Checks");
            exclude = GetList("Exclude");

            //checkView.Items.Add(new Check());
            //checkView.SelectedIndex = 0;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            TriItems["Wisdom"].Clear();
            TriItems["Power"].Clear();
            TriItems["Courage"].Clear();
            
            foreach (object ctl in g.Children)
            {
                if (ctl.GetType() == typeof(CheckBox))
                    ((CheckBox)ctl).IsChecked = false;

                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = String.Empty;
                    
                if (ctl.GetType() == typeof(ListBox) || ctl.GetType() == typeof(ListView))
                    ((ListBox)ctl).Items.Clear();
            }

            updateStepCount("Reset");
        }

        private void WOTH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;


            if (locations.Contains(WOTH.Text, StringComparer.InvariantCultureIgnoreCase))
            {
                woth_listBox.Items.Add(WOTH.Text);
                WOTH.Clear();
            } else if (locations.Contains(Foolish.Text, StringComparer.InvariantCultureIgnoreCase))
            {
                fool_listBox.Items.Add(Foolish.Text);
                Foolish.Clear();
            }
        }

        private void Found_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            TextBox s = (TextBox)sender;
            string t = "";
            if (s.Name.Equals("Found"))
            {
                t = Found.Text;
                if (rewards.Contains(t, StringComparer.InvariantCultureIgnoreCase))
                {
                    found_listbox.Items.Add(t);
                    ScrollDown(found_listbox);
                    Found.Clear();
                }
            } 
            else if (s.Name.Equals("Found_Path"))
            {
                t = Found_Path.Text;
                if (rewards.Contains(t, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (pathView.SelectedIndex != -1)
                    {
                        Path path = (Path)pathView.Items.GetItemAt(pathView.SelectedIndex);

                        path.items.Add(t);
                        TriItems[path.triforce].Add(t);
                        found_listbox_path.Items.Add(t);
                        updateStepCount(path.triforce);

                        foreach (Path p in pathView.Items)
                        {
                            if (p.region.Equals(path.region) && !p.Equals(path))
                            {
                                p.items.Add(t);
                                TriItems[p.triforce].Add(t);
                                updateStepCount(p.triforce);
                            }
                        }

                        ScrollDown(found_listbox_path);
                        Found_Path.Clear();
                    }
                    else if (but.Equals("Power") || but.Equals("Wisdom") || but.Equals("Courage"))
                    {
                        TriItems[but].Add(t);
                        found_listbox_path.Items.Add(t);
                        updateStepCount(but);

                        ScrollDown(found_listbox_path);
                        Found_Path.Clear();
                    }
                }
            }
        }

        private void ScrollDown(ListBox found)
        {
            if (VisualTreeHelper.GetChildrenCount(found) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(found, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }
        private void All_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(sender is TextBox)
            {
                TextBox temp = (TextBox)sender;
                temp.Text = rgx.Replace(temp.Text, "");
                if (temp.Text.Length == 1)
                {
                    temp.Text = temp.Text.ToUpper();
                    temp.SelectionStart = 1;
                }
            }
        }

        private void Delete_Item(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;
            else if (sender is ListBox)
            {
                ListBox temp = (ListBox)sender;
                if (temp.SelectedIndex == -1) return;
                else
                {
                    if (temp.Name.Equals("found_listbox_path"))
                    {
                        if (pathView.SelectedIndex != -1)
                        {
                            Path path = (Path)pathView.Items.GetItemAt(pathView.SelectedIndex);
                            string item = (string)temp.Items.GetItemAt(temp.SelectedIndex);
                            path.items.Remove(item);
                            TriItems[path.triforce].Remove(item);
                            updateStepCount(path.triforce);
                        }
                        else if (but.Equals("Power") || but.Equals("Wisdom") || but.Equals("Courage"))
                        {
                            string item = (string)temp.Items.GetItemAt(temp.SelectedIndex);
                            //foreach (Path p in pathView.Items)
                            //{
                            //    if (p.triforce.Equals(but) && p.items.Contains(item))
                            //    {
                            //        p.items.Remove(item);
                            //        break;
                            //    }
                            //}
                            TriItems[but].Remove(item);
                            updateStepCount(but);
                        }
                    }

                    if (temp.Name.Equals("pathView"))
                    {
                        Path path = (Path)temp.Items.GetItemAt(temp.SelectedIndex);
                        List<string> found = path.items;
                        TriItems[path.triforce].RemoveAll(t => found.Contains(t));
                        found.Clear();
                        updateStepCount(path.triforce);
                    }

                    temp.Items.RemoveAt(temp.SelectedIndex);
                }
            }
        }

        private void Highlight(TextBox sender)
        {
            sender.SelectAll();
        }

        private void KeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                Highlight((TextBox)sender);
            }
        }

        private new void MouseEnter(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox)
            {
                Highlight((TextBox)sender);
            }
        }

        private void Check_Down(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                TextBox s = e.Source as TextBox;
                if(s != null)
                {
                    s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

                e.Handled = true;
            }
        }

        private List<String> GetList(string name)
        {
            string path = dir + "\\" + name + ".txt";
            List<String> gotten = new List<string>();
            if (File.Exists(path))
            {
                StreamReader sr = File.OpenText(path);
                
                while (sr.Peek() != -1)
                {
                    gotten.Add(sr.ReadLine());   
                }
                
                sr.Close();
            }
            else
            {
                File.CreateText(path);
                System.Diagnostics.Process.Start(dir);
            }
            return gotten;
        }

        private void Lists_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(dir);
        }

        private void Triforce_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            
            switch (b.Content)
            {
                case ("PWR"):
                    but = "Power";
                    break;
                case ("WIS"):
                    but = "Wisdom";
                    break;
                case ("CRG"):
                    but = "Courage";
                    break;
                default:
                    break;
            }

            if (locations.Contains(Area.Text, StringComparer.InvariantCultureIgnoreCase))
            {
                Path path = new Path(Area.Text, but);

                if (pathView.Items.Contains(path))
                    return;

                foreach (Path p in pathView.Items)
                {
                    if (p.region.Equals(Area.Text))
                    {
                        path.items.AddRange(p.items);
                        TriItems[but].AddRange(p.items);
                        updateStepCount(but);
                    }
                }

                pathView.Items.Add(path);
                Area.Clear();

                pathView.SelectedIndex = pathView.Items.Count - 1;

                Found_Path.Focus();

            } 
            else if (Area.Text.Equals(""))
            {
                if (found_listbox_path.SelectedIndex != -1)
                {
                    string item = (string)found_listbox_path.Items.GetItemAt(found_listbox_path.SelectedIndex);
                    TriItems[but].Add(item);
                    found_listbox_path.SelectedIndex = -1;
                    Triforce_Click(sender, e);
                }
                else
                {
                    pathView.SelectedIndex = -1;
                    found_listbox_path.Items.Clear();
                    for (int i = 0; i < TriItems[but].Count; i++)
                    {
                        found_listbox_path.Items.Add(TriItems[but][i]);
                    }
                }
            }
            
        }

        private void Check_In(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox s = sender as TextBox;
                if (s.Name.Equals("has"))
                {
                    if (has.Text.Trim() != "" &&
                       checks.Contains(checkLoc.Text, StringComparer.InvariantCultureIgnoreCase))
                    {
                        checkView.Items.Add(new Check(checkLoc.Text, has.Text));
                        checkLoc.Clear();
                        has.Clear();

                        s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    }
                }
                else if (s.Name.Equals("Song"))
                {
                    if (songChecks.Contains(songLoc.Text, StringComparer.InvariantCultureIgnoreCase) &&
                       songs.Contains(Song.Text, StringComparer.InvariantCultureIgnoreCase))
                    {
                        checkView.Items.Add(new Check(songLoc.Text, Song.Text));
                        songLoc.Clear();
                        Song.Clear();

                        s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    }
                }
            }
        }

        private void PathView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            found_listbox_path.Items.Clear();
            if (pathView.SelectedIndex == -1) return;

            but = "";
            Path path = (Path)pathView.Items.GetItemAt(pathView.SelectedIndex);
            for (int i = 0; i < path.items.Count; i++)
            {
                found_listbox_path.Items.Add(path.items[i]);
            }

        }

        private void NumTest(object sender, TextChangedEventArgs e)
        {
            Regex nums = new Regex(@"[^\d]");
            TextBox temp = (TextBox)sender;
            temp.Text = nums.Replace(temp.Text, "");
        }

        private void updateStepCount(string triforce)
        {
            switch (triforce)
            {
                case ("Power"):
                    pwrCount.Text = TriItems[triforce].Count().ToString();
                    break;
                case ("Wisdom"):
                    wisCount.Text = TriItems[triforce].Count().ToString();
                    break;
                case ("Courage"):
                    crgCount.Text = TriItems[triforce].Count().ToString();
                    break;
                default:
                    pwrCount.Text = "0";
                    wisCount.Text = "0";
                    crgCount.Text = "0";
                    break;
            }
        }
    }
}
