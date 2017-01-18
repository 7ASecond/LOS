using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOS_Installer
{
    public partial class FrmInstaller : Form
    {
        public FrmInstaller()
        {
            InitializeComponent();

            Image spinnerImg = Image.FromFile(@"Theme\Current\spinner.gif");

            pictureBox1.Image = spinnerImg;
            pictureBox2.Image = spinnerImg;
            pictureBox3.Image = spinnerImg;
            pictureBox4.Image = spinnerImg;
            pictureBox5.Image = spinnerImg;
            pictureBox6.Image = spinnerImg;
            pictureBox7.Image = spinnerImg;
            pictureBox8.Image = spinnerImg;
        }
    }
}
