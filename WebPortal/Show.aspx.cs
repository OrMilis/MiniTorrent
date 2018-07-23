using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClassLibrary;
using Newtonsoft.Json;
namespace WebPortal
{
    public partial class Show : System.Web.UI.Page
    {
        DataTable dtFiles;
        ServerService.MediationServerClient server = new ServerService.MediationServerClient();
        protected void Page_Load(object sender, EventArgs e)
        {
            lblActiveUsers.Text = showActiveUssers();
            lblTotalUsers.Text = showTotalUsers();
            listboxFiles.DataSource = showListOfFiles();
            listboxFiles.DataTextField = "files"; // - need to show the files
            listboxFiles.DataBind();
        }
        protected string showActiveUssers()
        {
            string activeUsers = (JsonConvert.DeserializeObject<List<User>>(server.getAllActiveUsers())).Count().ToString();
            return activeUsers;

        }
        protected string showTotalUsers()
        {
            string Users = (JsonConvert.DeserializeObject<List<User>>(server.GetAllUsers())).Count().ToString();
            return Users;
        }
        protected DataTable showListOfFiles()
        {
            DataTable dtFiles = new DataTable();
            dtFiles.Columns.Add("files");
            foreach (User user in JsonConvert.DeserializeObject<List<User>>(server.getAllActiveUsers()))
            {

                List<String> listOfFiles = new List<String>();
                for (int i = 0; i < dtFiles.Rows.Count; i++)
                {
                    String file = dtFiles.Rows[i]["files"].ToString();
                    listOfFiles.Add(file);
                }

                foreach (string s in user.userFiles.Keys)
                {
                    if (!listOfFiles.Contains(s))// if the file existed - add it to dtfiles
                    {
                        DataRow newRow = dtFiles.NewRow();
                        newRow["files"] = s;
                        dtFiles.Rows.Add(newRow);
                    }
                }
            }

            return dtFiles;
        }



        protected void iBtnSerach_Click(object sender, ImageClickEventArgs e)
        {
            Dictionary<string, HashSet<User>> files = JsonConvert.DeserializeObject<Dictionary<string, HashSet<User>>>(server.getAllFiles());
            HashSet<User> users = new HashSet<User>();
            if (files.ContainsKey(txtFilename.Text))
            {
                DataTable dtFiles = new DataTable();
                dtFiles.Columns.Add("names"); 
                users = files[txtFilename.Text];

                foreach (User user in users)
                {
                    DataRow newRow = dtFiles.NewRow();
                    newRow["names"] = user.name.ToString();
                    dtFiles.Rows.Add(newRow);
                    listSerach.DataSource = dtFiles;
                    listSerach.DataTextField = "names"; // - need to show the users
                    listSerach.DataBind();
                    listSerach.Visible = true;
                }
            }
        }
    }
}