using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace TheParsnipWeb
{
    public partial class UserForm : System.Web.UI.UserControl
    {
        Account MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            
        }

        public UserForm()
        {
            MyAccount = new Account();
        }

        public UserForm(Account pAccount)
        {
            MyAccount = pAccount;
            username.Text = pAccount.Username;
            email.Text = pAccount.Email;
            pwd.Text = "CLASSIFIED";
            fname.Text = MyAccount.Fname;
            sname.Text = MyAccount.Sname;
            dob.Text = MyAccount.Dob;
        }

        protected void but_Submit_Click(object sender, EventArgs e)
        {
            MyAccount.Username = username.Text;
        }
    }
}