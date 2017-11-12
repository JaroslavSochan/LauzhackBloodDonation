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

            trackBar1.Maximum = 100;
            trackBar2.Maximum = 100;
            trackBar3.Maximum = 100;
            trackBar4.Maximum = 100;
            trackBar5.Maximum = 100;
            trackBar6.Maximum = 100;
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

            GeartestContext context = new GeartestContext();
            context.Bloddpictures.Add(new Bloddpicture()
            {
                Erytrocyt = (decimal?)trackBar1.Value / 100,
                Fibrinogen = (decimal?)trackBar2.Value / 100,
                Hemocyt = (decimal?)trackBar3.Value / 100,
                Leukocyt = (decimal?)trackBar4.Value / 100,
                Protrombin = (decimal?)trackBar5.Value / 100,
                Trombocyt = (decimal?)trackBar6.Value / 100,
                IdPerson = 1,
                SawDoctor = 1,
                SawPerson = 0
            });

            context.SaveChanges();
            //await DatabaseAdapter.SetNewBloodPicture(DatabaseAdapter.PersonLogged, data);
        }
    }
}
