using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinLoading
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            loadingMetro1.Start();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            loadingMetro1.Start();
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            loadingMetro1.Stop();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            loadingMetroHorizontal1.State = true;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            loadingMetroHorizontal1.State = false;
        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            loadingMetroHorizontal1.EndStop = checkBox1.Checked;
        }

        private void button5_Click(object sender, System.EventArgs e)
        {
            loadingMaterial1.State = true;
        }

        private void button6_Click(object sender, System.EventArgs e)
        {
            loadingMaterial1.State = false;
        }

        private void button7_Click(object sender, System.EventArgs e)
        {
            loadingMaterialHorizontal1.State = true;
        }

        private void button8_Click(object sender, System.EventArgs e)
        {
            loadingMaterialHorizontal1.State = false;
        }

        private void checkBox2_CheckedChanged(object sender, System.EventArgs e)
        {
            loadingMaterialHorizontal1.EndStop = checkBox2.Checked;
        }

        private void button9_Click(object sender, System.EventArgs e)
        {
            BackColor = Color.White;
            panel4.ForeColor = panel3.ForeColor = panel6.ForeColor = panel8.ForeColor = Color.DimGray;
        }

        private void button10_Click(object sender, System.EventArgs e)
        {
            BackColor = Color.Black;
            panel4.ForeColor = panel3.ForeColor = panel6.ForeColor = panel8.ForeColor = Color.WhiteSmoke;
        }

        private void button12_Click(object sender, System.EventArgs e)
        {
            loadingMetroHorizontal1.Value = loadingMaterial1.Value = loadingMaterial21.Value = loadingMaterialHorizontal1.Value = 0;
        }

        private void button11_Click(object sender, System.EventArgs e)
        {
            loadingMetroHorizontal1.Value += 5;
            loadingMaterial1.Value += 5;
            loadingMaterial21.Value += 5;
            loadingMaterialHorizontal1.Value += 5;
        }

        private void button13_Click(object sender, System.EventArgs e)
        {
            Task.Run(() =>
            {
                while (loadingMetroHorizontal1.MaxValue > loadingMetroHorizontal1.Value ||
                loadingMaterial1.MaxValue > loadingMaterial1.Value ||
                loadingMaterial21.MaxValue > loadingMaterial21.Value ||
                loadingMaterialHorizontal1.MaxValue > loadingMaterialHorizontal1.Value)
                {
                    loadingMetroHorizontal1.Value += 0.1;
                    loadingMaterial1.Value += 0.1;
                    loadingMaterial21.Value += 0.1;
                    loadingMaterialHorizontal1.Value += 0.1;
                    System.Threading.Thread.Sleep(10);
                }
            });
        }
    }
}