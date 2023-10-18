using Steam4NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteamGameBooster
{
    public partial class Form1 : Form
    {
        private static ISteamClient012 _steamClient012;
        private static ISteamApps001 _steamApps001;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<GameList> items = new List<GameList>();
            items.Add(new GameList() { Name = "Counter-Strike 2", ID = "730" });
            items.Add(new GameList() { Name = "Dota 2", ID = "570" }); 
            items.Add(new GameList() { Name = "Naraka: Bladepoint", ID = "1203220" });
            items.Add(new GameList() { Name = "Battlefield: Bad Company™ 2", ID = "24960" });

            comboBox1.DataSource = items;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "ID";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Stop")
            {
                Environment.Exit(1);
            }
            var get_gameid = comboBox1.SelectedValue.ToString();
            Environment.SetEnvironmentVariable("SteamAppId", get_gameid,EnvironmentVariableTarget.Process);
            if (ConnectToSteam())
            {
                SetWindowName(Convert.ToUInt32(get_gameid));
                lb_status.Text = "Game is running..";
                button1.Text = "Stop";
            }
        }

        private static void SetWindowName(uint appId)
        {
            var sb = new StringBuilder(60);
            _steamApps001.GetAppData(appId, "name", sb);

            string gameName = sb.ToString().Trim();
            Form1 frm1 = new Form1();
            frm1.Text = string.IsNullOrWhiteSpace(gameName) ? "Unknown game" : GetUnicodeString(gameName);
        }

        private static string GetUnicodeString(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Encoding.UTF8.GetString(bytes);
        }

        private static bool ConnectToSteam()
        {
            if (!Steamworks.Load(true))
            {
                MessageBox.Show("Steamworks failed to load.");
                return false;
            }

            _steamClient012 = Steamworks.CreateInterface<ISteamClient012>();
            if (_steamClient012 == null)
            {
                MessageBox.Show("Failed to create Steam Client inferface.");
                return false;
            }

            int pipe = _steamClient012.CreateSteamPipe();
            if (pipe == 0)
            {
                MessageBox.Show("Failed to create Steam pipe.");
                return false;
            }

            int user = _steamClient012.ConnectToGlobalUser(pipe);
            if (user == 0)
            {
                MessageBox.Show("Failed to connect to Steam user. (No game in this Account!)");
                return false;
            }

            _steamApps001 = _steamClient012.GetISteamApps<ISteamApps001>(user, pipe);
            if (_steamApps001 == null)
            {
                MessageBox.Show("Failed to create Steam Apps inferface.");
                return false;
            }

            return true;
        }
    }

    public class GameList
    {
        public GameList() { }

        public string ID { set; get; }
        public string Name { set; get; }
    }
}
