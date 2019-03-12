using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommentApi;

namespace ParsnipWebsite.Custom_Controls.Comment_Api
{
    public partial class CommentGroup : System.Web.UI.UserControl
    {
        CommentApi.CommentGroup MyCommentGroup;
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        CommentGroup(Guid pCommentGroupId)
        {
            MyCommentGroup = new CommentApi.CommentGroup(pCommentGroupId);
            MyCommentGroup.Select();
            foreach(Comment comment in MyCommentGroup.GetAllComments())
            {
                DisplayComments.InnerHtml += comment + "<break />";
            }
            
        }
    }
}