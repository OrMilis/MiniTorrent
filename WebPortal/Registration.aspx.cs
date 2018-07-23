using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ClassLibrary;
using Newtonsoft.Json;

namespace WebPortal
{
    public partial class Registration : System.Web.UI.Page
    {
        ServerService.MediationServerClient server = new ServerService.MediationServerClient();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            User user = new User(txtUsername.Text, txtPassword.Text, txtEmail.Text);
            server.SignUp(JsonConvert.SerializeObject(user,Formatting.Indented));
        }
    }
}