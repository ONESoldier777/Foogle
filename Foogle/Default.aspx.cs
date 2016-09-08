using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Foogle
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnIndex_Click(object sender, EventArgs e)
        {
            FoogleEngine.Page page;
            FoogleEngine.IndexWorker.TryPageParse("http://stackoverflow.com/questions/16642196/get-html-code-from-website-in-c-sharp", out page);
            //bool work = FoogleEngine.IndexWorker.TryPageIndex("http://stackoverflow.com/questions/16642196/get-html-code-from-website-in-c-sharp", out page);
        }
    }
}