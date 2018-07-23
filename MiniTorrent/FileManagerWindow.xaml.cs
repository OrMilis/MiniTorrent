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
using System.Windows.Shapes;
using Newtonsoft.Json;
using ClassLibrary;
using System.Threading;

namespace MiniTorrent
{
    public partial class FileManagerWindow : Window
    {

        User user;
        List<User> userWithFile = new List<User>();
        int selectedIndex = -1;
        long downloadFileSize;

        private Peer peer;
        private string uploadPath;

        public FileManagerWindow(User user, string uploadPath, string downloadPath)
        {
            InitializeComponent();
            this.user = user;

            peer = new Peer(uploadPath, downloadPath);

            this.uploadPath = uploadPath;

            Task.Factory.StartNew(() => peer.HandleIncomingFile(user.port));
            Task.Factory.StartNew(() => peer.HandleIncomingRequest(8003));

            initDataGrids(Files_DataGrid);
            initDataGrids(Search_DataGrid);

            Files_DataGrid.ItemsSource = user.userFiles;
        }

        private void Search_Btn_Click(object sender, RoutedEventArgs e)
        {
            selectedIndex = -1;
            downloadFileSize = 0;

            ServerService.MediationServerClient server = new ServerService.MediationServerClient();

            string fileName = Search_TextBox.Text.ToString();
            string listData;

            try
            {
                if (fileName.Length > 0)
                {
                    listData = server.RequestFiles(fileName);
                    userWithFile = JsonConvert.DeserializeObject<List<User>>(listData);
                    if (userWithFile.Count > 0)
                    {
                        userWithFile.RemoveAll(u => u.ip == this.user.ip);
                        Search_DataGrid.ItemsSource = userWithFile;

                        long size;
                        userWithFile.ElementAt<User>(0).userFiles.TryGetValue(fileName, out size);

                        if (size > 1000)
                            SizeNum_Lable.Content = size / 1000 + " Kb";
                        else if (size > 1000000)
                            SizeNum_Lable.Content = size / 1000000 + " Mb";
                        else
                            SizeNum_Lable.Content = size  + " b";



                    }
                }
            }
            catch { }

        }

        private void initDataGrids(DataGrid dataGrid)
        {
            dataGrid.AutoGenerateColumns = false;
            dataGrid.IsReadOnly = true;
            dataGrid.CanUserAddRows = false;
            dataGrid.CanUserDeleteRows = false;
            dataGrid.CanUserReorderColumns = false;
            dataGrid.CanUserResizeColumns = false;
            dataGrid.CanUserResizeRows = false;
            dataGrid.CanUserSortColumns = false;

        }

        private void Download_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndex != -1)
            {
                Download_ProgressBar.Value = 0;
                User user = userWithFile.ElementAt<User>(selectedIndex);
                string filename = Search_TextBox.Text;
                user.userFiles.TryGetValue(filename, out downloadFileSize);
                Task.Factory.StartNew(() => peer.SendRequest(user.ip, 8003, filename));
                peer.DataReceived += new Peer.FileRecievedEventHandler(dataRecieved);
            }
        }

        private void Search_DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            selectedIndex = Search_DataGrid.Items.IndexOf(Search_DataGrid.CurrentItem);
        }

        private void dataRecieved(object source, double received)
        {
            Dispatcher.BeginInvoke(new ThreadStart(() => updateProgress(received)));
        }

        private void updateProgress(double received)
        {
            if (received == -1)
            {
                Download_ProgressBar.Value = 100;
                ServerService.MediationServerClient server = new ServerService.MediationServerClient();
                user.loadFiles(uploadPath);
                Files_DataGrid.ItemsSource = user.userFiles;
                server.SignOut(JsonConvert.SerializeObject(user));
                server.SignIn(JsonConvert.SerializeObject(user));
            }
            else
            {
                Download_ProgressBar.Value = (double)(received / downloadFileSize) * 100;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            ServerService.MediationServerClient server = new ServerService.MediationServerClient();
            server.SignOut(JsonConvert.SerializeObject(this.user));
            base.OnClosed(e);

        }

        private void ReadDll_Btn_Click(object sender, RoutedEventArgs e)
        {
            ReadDll readDll = new ReadDll();
            readDll.ShowDialog();
        }
    }
}
