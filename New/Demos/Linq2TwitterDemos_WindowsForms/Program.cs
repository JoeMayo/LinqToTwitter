using System;
using System.Linq;
using System.Windows.Forms;

namespace Linq2TwitterDemos_WindowsForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TwitterForm());
        }
    }
}
