using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentInspectorApp
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            txtServer.Text = Properties.Settings.Default["server"].ToString();
            txtPort.Text = Properties.Settings.Default["port"].ToString();
            txtName.Text = Properties.Settings.Default["name"].ToString();
            txtSecondname.Text = Properties.Settings.Default["secondname"].ToString();
            txtUsername.Text = Environment.UserName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int port;
            if (int.TryParse(txtPort.Text, out port))
            {
                Properties.Settings.Default["port"] = port;
                Properties.Settings.Default["server"] = txtServer.Text;
                Properties.Settings.Default["name"] = txtName.Text;
                Properties.Settings.Default["secondname"] = txtSecondname.Text;
                Properties.Settings.Default.Save();
                Close();
            }
            else
            {
                MessageBox.Show("Valor de puerto incorrecto. Si lo has cambiado por error, escribe 10256.","Error");
            }
        }
    }
}
