using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TheParsnipWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BenTextBox_TextChanged(object sender, EventArgs e)
        {
            BenLabel.Text = BenTextBox.Text;
        }

        protected void BenButton_Click(object sender, EventArgs e)
        {
            BenLabel.Text = BenTextBox.Text;
        }
    }
}