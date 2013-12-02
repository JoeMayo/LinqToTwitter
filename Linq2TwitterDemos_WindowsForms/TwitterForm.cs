using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Linq2TwitterDemos_WindowsForms
{
    public partial class TwitterForm : Form
    {
        public TwitterForm()
        {
            InitializeComponent();
        }

        void MenuItem_Click(object sender, EventArgs e)
        {
            var type = Type.GetType((sender as ToolStripMenuItem).Tag as string);
            Form formInst = (Form)Activator.CreateInstance(type);

            formInst.Show();
        }

        private void TwitterForm_Load(object sender, EventArgs e)
        {
            if (SharedState.Authorizer == null)
                new OAuthForm().ShowDialog();
        }
    }
}
