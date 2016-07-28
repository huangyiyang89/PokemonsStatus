using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Login;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace PokemonsStatus
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static Client _client;
        static Settings _clientSettings = new Settings();
        static Inventory _inventory;
        static DataTable t;
        
        public MainWindow()
        {
            InitializeComponent();
            t = new DataTable();
            t.Columns.Add("ID", typeof(int));
            t.Columns.Add("CNName", typeof(string));
            t.Columns.Add("Pokemon", typeof(string));
            t.Columns.Add("CreationTime", typeof(DateTime));
            t.Columns.Add("LV", typeof(double));
            t.Columns.Add("CP", typeof(int));
            t.Columns.Add("MaxCP", typeof(int));
            t.Columns.Add("ATK", typeof(int));
            t.Columns.Add("DEF", typeof(int));
            t.Columns.Add("STA", typeof(int));
            t.Columns.Add("CPPerfection", typeof(double));
            t.Columns.Add("IVPerfection", typeof(double));
            
           
            textbox_ptcusername.Text = Properties.Settings.Default.username;
            textBox_ptcpass.Text = Properties.Settings.Default.password;
            
        }

        private async void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            btnSignOut.IsEnabled = false;
            btnGoogleLogin.IsEnabled = false;
            btnPtcLogin.IsEnabled = false;
            try
            {
                _clientSettings.AuthType = AuthType.Ptc;
                _clientSettings.PtcUsername = textbox_ptcusername.Text;
                _clientSettings.PtcPassword = textBox_ptcpass.Text;
                _clientSettings.DefaultLatitude = 37.808586;
                _clientSettings.DefaultLongitude = -122.409836;
                _client = new Client(_clientSettings);
                await _client.DoPtcLogin(_clientSettings.PtcUsername, _clientSettings.PtcPassword);
                await _client.SetServer();
                _inventory = new Inventory(_client);
                Bind();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnGoogleLogin.IsEnabled =true;
            btnPtcLogin.IsEnabled = true;
            btnSignOut.IsEnabled = true;

        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            btnSignOut.IsEnabled = false;
            btnGoogleLogin.IsEnabled = false;
            btnPtcLogin.IsEnabled = false;
            try
            {
                _clientSettings.AuthType = AuthType.Google;
                _clientSettings.PtcUsername = textbox_ptcusername.Text;
                _clientSettings.PtcPassword = textBox_ptcpass.Text;
                
                _clientSettings.DefaultLatitude = 37.808586;
                _clientSettings.DefaultLongitude = -122.409836;

                
                _client = new Client(_clientSettings);
                await _client.DoGoogleLogin();
                await _client.SetServer();
                _inventory = new Inventory(_client);
                Bind();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnGoogleLogin.IsEnabled = true;
            btnPtcLogin.IsEnabled = true;
            btnSignOut.IsEnabled = true;
        }

        private async void Bind()
        {
            t.Clear();
            var Pokemons = await _inventory.GetPokemons();
            foreach (var Pokemon in Pokemons)
            {
                DataRow newrow = t.NewRow();
                newrow["ID"] = (int)Pokemon.PokemonId;
                newrow["CNName"] = (PokemonNameCN)Pokemon.PokemonId;
                newrow["Pokemon"] = Pokemon.PokemonId.ToString();
                DateTime time = DateTime.MinValue;
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
                newrow["CreationTime"] = startTime.AddMilliseconds(Pokemon.CreationTimeMs);
                newrow["LV"] = PokemonInfo.GetLevel(Pokemon);
                newrow["CP"] = Pokemon.Cp;
                newrow["MaxCP"] = PokemonInfo.CalculateMaxCP(Pokemon);
                newrow["ATK"] = Pokemon.IndividualAttack;
                newrow["DEF"] = Pokemon.IndividualDefense;
                newrow["STA"] = Pokemon.IndividualStamina;
                newrow["CPPerfection"] = Math.Round(PokemonInfo.CalculatePokemonPerfection(Pokemon),2);
                newrow["IVPerfection"] = Math.Round(PokemonInfo.CalculatePokemonPerfection2(Pokemon),2);
                t.Rows.Add(newrow);
                
            }
            
            dataGrid.ItemsSource = t.AsDataView();
           


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
            Properties.Settings.Default.username = textbox_ptcusername.Text;
            Properties.Settings.Default.password = textBox_ptcpass.Text;
            Properties.Settings.Default.Save();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Configs\\GoogleAuth.ini", "");
            
        }
    }
}
