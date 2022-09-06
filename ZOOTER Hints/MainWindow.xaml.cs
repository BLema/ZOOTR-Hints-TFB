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

            songChecks = new List<String> { "Minuet", "Burning Kak", "Prelude", "Crater",
                "Ice Cavern", "Royal Tomb Song", "Requiem", "OoT" };
        

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
            foreach (object ctl in g.Children)
            {
                if (ctl.GetType() == typeof(CheckBox))
                    ((CheckBox)ctl).IsChecked = false;
                if (ctl.GetType() == typeof(TextBox))
                    ((TextBox)ctl).Text = String.Empty;
                if (ctl.GetType() == typeof(ListBox) || ctl.GetType() == typeof(ListView))
                    ((ListBox)ctl).Items.Clear();
            }
        }

        private void WOTH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            //e.Handled = true;
            if(locations.Contains(WOTH.Text, StringComparer.InvariantCultureIgnoreCase))
            {
                woth_listBox.Items.Add(WOTH.Text);
                WOTH.Clear();
            }
        }

        private void Found_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            //e.Handled = true;
            string t = Found.Text;
            if (rewards.Contains(t, StringComparer.InvariantCultureIgnoreCase))
            {
                found_listbox.Items.Add(t);
                if (VisualTreeHelper.GetChildrenCount(found_listbox) > 0)
                {
                    Border border = (Border)VisualTreeHelper.GetChild(found_listbox, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    scrollViewer.ScrollToBottom();
                }
                Found.Clear();
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

        private void Check_In(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox s = e.Source as TextBox;
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
    }
}
