using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToTwitter;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var twitterCtx = new TwitterContext();

        var tweets =
            from tweet in twitterCtx.Status
            where tweet.Type == StatusType.Public
            select tweet;

        TwitterListView.DataSource = tweets;
        TwitterListView.DataBind();
    }
}
