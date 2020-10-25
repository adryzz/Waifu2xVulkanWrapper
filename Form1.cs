using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Waifu2x_Vulkan_Wrapper
{
    public partial class Form1 : Form
    {
        Process p;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var res = saveFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                textBox2.Text = saveFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string path0 = "";
            string path1 = "";
            try
            {
                path0 = Path.GetFullPath(textBox1.Text);
                path1 = Path.GetFullPath(textBox2.Text);
                if (!File.Exists(path0) && !File.Exists(path1))
                {
                    MessageBox.Show(this, "The specified file does not exist", "Waifu2x-Vulkan-Wrapper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, "Exception type " + ex.GetType() + ": " + ex.Message, "Waifu2x-Vulkan-Wrapper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            p = new Process();
            p.StartInfo.FileName = "waifu2x-ncnn-vulkan.exe";
            p.OutputDataReceived += P_OutputDataReceived;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Exited += P_Exited;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            List<string> arguments = new List<string>();

            //arguments.Add("-v");//verbose output

            arguments.Add("-n " + Convert.ToInt32(numericUpDown1.Value));//de-noise

            arguments.Add("-s " + Convert.ToInt32(numericUpDown2.Value));//upscale size

            if (checkBox1.Checked)
            {
                arguments.Add("-x");//TTA mode
            }

            arguments.Add("-i " + path0);//input path

            arguments.Add("-o " + path1);//output path


            string args = "";
            foreach (string s in arguments)
            {
                args += s + " ";
            }
            args.Remove(args.Length - 1);
            p.StartInfo.Arguments = args;
            p.Start();
            p.BeginOutputReadLine();
            progressBar1.Style = ProgressBarStyle.Marquee;
            groupBox1.Enabled = false;
            button3.Enabled = false;
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Invoke(new DoStuff(() => {
                groupBox1.Enabled = true;
                button3.Enabled = true;
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 100;
                MessageBox.Show(this, "Done!", "Waifu2x-Vulkan-Wrapper", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void P_Exited(object sender, EventArgs e)
        {
            /*
            if (InvokeRequired)
            {
                Invoke(new DoStuff(() => {
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 100;
                    MessageBox.Show(this, "Done!", "Waifu2x-Vulkan-Wrapper", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 100;
                MessageBox.Show(this, "Done!", "Waifu2x-Vulkan-Wrapper", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            */
        }
        delegate void DoStuff();
    }
}
