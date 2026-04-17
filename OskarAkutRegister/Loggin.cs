using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OskarAkutRegister
{
    public partial class Loggin : Form
    {
        bool showPass = false;
        public Loggin()
        {
            InitializeComponent();
            textBox1.UseSystemPasswordChar = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label3.Text = "";
            label4.Text = "";
            if (textBox2.Text.ToString() == "")
            {
                label3.Text = "ex 9807031234";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                string personNummer = textBox2.Text.ToString();
                string lösenord = textBox1.Text.ToString();
                string inlog = (personNummer + "^" + lösenord + " ");

                bool exist = false;
                string[] per = File.ReadAllLines("inlog.txt");
                
                for (int i = 0; i < per.GetLength(0); i++)
                {
                    string[] delat = per[i].Split('^');
                    if (delat[0] == personNummer)
                        exist = true;
                }
                if(!exist)
                {
                    File.AppendAllText("inlog.txt", inlog + Environment.NewLine);

                    textBox1.Text = "";
                    textBox2.Text = "";
                    MessageBox.Show("Ditt konto har registrerats", "Registrering", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("Det finns redan ett konto på det här personnummret, prova att logga in istället eller kontakta vår icke existerande kund support.  :D","Registrering",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
            else
            {
                label4.Text = "DU måste fylla i personnummer och lösenord";

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                string pN = textBox2.Text.ToString();
                string lN = textBox1.Text.ToString();
                string inlog = (pN + "^" + lN);

                string inloog = File.ReadAllText("inlog.txt").ToString();


                string[] inlogs = inloog.Split(' ');

                for (int i = 0; i < inlogs.GetLength(0); i++)
                {
                    inlogs[i] = inlogs[i].TrimStart('\r', '\n');
                }
                bool match = false;

                for (int i = 0; i < inlogs.GetLength(0); i++)
                {
                    if (inlog.ToString() == inlogs[i].ToString())
                    {
                        match = true;
                        break;
                    }
                }

                if (match == true)
                {
                    label3.Text = "logged in!";
                    Debug.WriteLine("Found Inlog with log in");
                    Form1 f1 = new Form1();
                    f1.Enabled = true;
                    f1.BringToFront();
                    this.Hide();
                    f1.Show();
                    match = false;
                }
                else
                {
                    textBox1.Text = "";
                    textBox2.Text = "";
                    label4.Text = "Fel personummer eller lösernord";

                }
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                label4.Text = "Fel personummer eller lösernord";

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label4.Text = "";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(showPass == true)
                showPass = false;
            else if(showPass == false)
                showPass = true;

            textBox1.UseSystemPasswordChar = !showPass;
        }
    }
}
