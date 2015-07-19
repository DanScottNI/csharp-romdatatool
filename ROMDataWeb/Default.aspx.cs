using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ROMDataLib;

namespace ROMDataWeb
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ROMDataManager romMan = new ROMDataManager();
            Game game= romMan.LoadROMData(Server.MapPath("Games\\SampleGame.xml"));
        }
    }
}
