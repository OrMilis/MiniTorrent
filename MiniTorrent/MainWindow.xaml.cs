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
using Newtonsoft.Json;
using ClassLibrary;
using System.IO;

namespace MiniTorrent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ConfigData configData = new ConfigData();

        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void SignIn_Btn_Click(object sender, RoutedEventArgs e)
        {

            ServerService.MediationServerClient client = new ServerService.MediationServerClient();
            User user = new User(configData);
            string userData = JsonConvert.SerializeObject(user, Formatting.Indented);

            if (client.SignIn(userData))
            {
                //MessageBox.Show("Hello " + user.name);
            }
            else
            {
                MessageBox.Show("Error in sign in.");
            }

            FileManagerWindow fileManagerWindow = new FileManagerWindow(user, configData.UploadPath, configData.DownloadPath);
            fileManagerWindow.ShowDialog();

        }

        private void Settings_Btn_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settings = new SettingWindow(configData);
            settings.ShowDialog();
            settings.Closed += new EventHandler(loadConfigEventHandler);
        }

        private void loadConfigEventHandler(object sender, EventArgs args)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                StreamReader reader = new StreamReader("D:\\config.json");
                string jsonFile = reader.ReadToEnd();
                configData = JsonConvert.DeserializeObject<ConfigData>(jsonFile);
                reader.Close();
            }
            catch (Exception e) { }
        }
    }
}
