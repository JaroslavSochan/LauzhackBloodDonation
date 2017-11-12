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
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            Credentials credentials = new Credentials();
            credentials.User = textBox1.Text;
            credentials.Pass = textBox2.Text;

            Person person = new Person();

            
            

            //await DatabaseAdapter.LogIn(credentials, person);

            //if (person.CustomerId != -1)
            //{
                AnalyserForm form = new AnalyserForm();
                form.Show();
            //}
        }
    }
}
