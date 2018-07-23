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
    public partial class Administrator : System.Web.UI.Page
    {
        DataTable dtUsers;
        ServerService.MediationServerClient server = new ServerService.MediationServerClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            setEnableOnUserUpdate(false);
            if (!this.IsPostBack)
            {
                dtUsers = new DataTable();
            }
        }

        private void Search()
        {
            string listData = server.SearchUser(txtUser.Text);
            List<User> theUsers = JsonConvert.DeserializeObject<List<User>>(listData);
            DataTable dtUsers = new DataTable();
            dtUsers.Columns.Add("name", typeof(string));
            dtUsers.Columns.Add("password", typeof(string));
            dtUsers.Columns.Add("mail", typeof(string));
            dtUsers.Columns.Add("active", typeof(string));
            foreach (User user in theUsers)
            {
                DataRow row = dtUsers.NewRow();
                row["name"] = user.name;
                row["password"] = user.password;
                row["mail"] = user.mail;
                row["active"] = user.isActive;
                dtUsers.Rows.Add(row);
            }

            Session.Add("UsersDB", dtUsers);
            listBoxUsers.DataSource = dtUsers;
            listBoxUsers.DataTextField = "mail";
            listBoxUsers.DataBind();
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Search();
        }

        protected void listBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            setEnableOnUserUpdate(true);
            dtUsers = (DataTable)Session["UsersDB"];
            string username = listBoxUsers.SelectedItem.Text;
            DataRow row = dtUsers.Rows[listBoxUsers.SelectedIndex];
            User user = new User
            {
                name = row["name"].ToString(),
                mail = row["mail"].ToString(),
                password = row["password"].ToString(),
                isActive = Convert.ToBoolean(row["active"].ToString())
            };
            txtUsername.Text = user.name;
            txtPassword.Text = user.password;
            txtEmail.Text = user.mail;
            if (user.isActive == false)
                chkLock.Checked = true;
            else
                chkLock.Checked = false;
            Session.Add("UpdateUser", user);
        }

        private void setEnableOnUserUpdate(bool flag)
        {
            lblUsername.Visible = flag;
            lblUserLock.Visible = flag;
            lblPassword.Visible = flag;
            lblEmail.Visible = flag;
            txtEmail.Visible = flag;
            txtPassword.Visible = flag;
            txtUsername.Visible = flag;
            chkLock.Visible = flag;
            iBtnUpdate.Visible = flag;
            iBtnDelete.Visible = flag;
        }

        protected void iBtnUpdate_Click(object sender, ImageClickEventArgs e)
        {
            User updateUser = (User)Session["UpdateUser"];
            updateUser.name = txtUsername.Text;
            updateUser.mail = txtEmail.Text;
            updateUser.password = txtPassword.Text;
            User user = new User(txtUsername.Text, txtPassword.Text, txtEmail.Text);
            if (chkLock.Checked)
            {
                updateUser.isActive = false;
                //server.SignOut(JsonConvert.SerializeObject(user, Formatting.Indented));
            }
            else
            {
                updateUser.isActive = true;
                //server.SignIn(JsonConvert.SerializeObject(updateUser, Formatting.Indented));
            }
            server.UpdateUser(updateUser);
            Search();
            setEnableOnUserUpdate(false);
        }

        protected void iBtnDelete_Click(object sender, ImageClickEventArgs e)
        {

            User updateUser = (User)Session["UpdateUser"];
            Console.WriteLine(server.deleteUser(JsonConvert.SerializeObject(updateUser, Formatting.Indented)));
            //need to add here the delette from dtUsers

            Search();
            setEnableOnUserUpdate(false);
        }
    }
}