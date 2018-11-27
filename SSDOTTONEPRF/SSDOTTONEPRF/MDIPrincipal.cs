using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSDOTTONEPRF
{
    public partial class MDIPrincipal : Form
    {
        private int childFormNumber = 0;

        public MDIPrincipal()
        {
            InitializeComponent();
        }
 
        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form1 childForm = new Form1();
            childForm.MdiParent = this;
            childForm.Text = "Porcentaje de Productos Vendidos dentro de un rango de Fechas ";
            childForm.Show();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 childForm = new Form2();
            childForm.MdiParent = this;
            childForm.Text = "Venta por Sucursal";
            childForm.Show();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 childForm = new Form3();
            childForm.MdiParent = this;
            childForm.Text = "Venta Anuales por Zonas del domicilio del Cliente";
            childForm.Show();
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            Form1 childForm = new Form1();
            childForm.MdiParent = this;
            childForm.Text = "Porcentaje de Productos Vendidos dentro de un rango de Fechas ";
            childForm.Show();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form2 childForm = new Form2();
            childForm.MdiParent = this;
            childForm.Text = "Venta por Sucursal";
            childForm.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Form3 childForm = new Form3();
            childForm.MdiParent = this;
            childForm.Text = "Venta Anuales por Zonas del domicilio del Cliente";
            childForm.Show();
        }

        private void MDIPrincipal_Load(object sender, EventArgs e)
        {

        }
    }
}
