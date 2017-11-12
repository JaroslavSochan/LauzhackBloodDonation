using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppDoctor
{
    public partial class AnalyserForm : Form
    {
        public AnalyserForm()
        {
            InitializeComponent();

            trackBar1.Maximum = 50;
            trackBar2.Maximum = 50;
            trackBar3.Maximum = 50;
            trackBar4.Maximum = 50;
            trackBar5.Maximum = 50;
            trackBar5.Maximum = 50;
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            double[] data = new double[6]
            {
                (double)trackBar1.Value / 100,
                (double)trackBar2.Value / 100,
                (double)trackBar3.Value / 100,
                (double)trackBar4.Value / 100,
                (double)trackBar5.Value / 100,
                (double)trackBar6.Value / 100
            };

            await DatabaseAdapter.SetNewBloodPicture(DatabaseAdapter.PersonLogged, data);
        }
    }
}
