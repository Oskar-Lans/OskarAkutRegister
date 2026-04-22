using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OskarAkutRegister
{
    public partial class Form1 : Form
    {
        public List<Patient> Patients = new List<Patient>();
        public List<Patient> sortedPatients = new List<Patient>();

        public List<Patient> behandlade = new List<Patient>();

        public int väntatFörlänge = 0;

        public int varningar = 0;

        public string sparadeBehandledePos = "";

        public List<string> Sympton = new List<string>()
        {
            "Andningssvårigheter",
            "Blödning",
            "Bröstsmärta",
            "Förvirring",
            "Hög feber",
            "Medvetslöshet",
            "Svårt att andas",
            "Svår smärta"
        };
        public List<string> Prio = new List<string>()
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"
        };


        int förlängetid = 10 * 60;
        int maxvänt = 40 * 60;
        string dag = "";
        public List<ListViewItem> lvil = new List<ListViewItem>();
        public Form1()
        {
            InitializeComponent();
            SymptonLista.DataSource = Sympton;
            Prioritering.DataSource = Prio;
            listView1.GridLines = true;
            listView1.View = View.Details;
            listView1.Columns.Add("             Namn             ", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Ålder", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("             Sympton               ", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Prio", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Tid", -2, HorizontalAlignment.Left);

            listView2.GridLines = true;
            listView2.View = View.Details;
            listView2.Columns.Add("             Namn             ", -2, HorizontalAlignment.Left);
            listView2.Columns.Add("Ålder", -2, HorizontalAlignment.Left);
            listView2.Columns.Add("             Sympton               ", -2, HorizontalAlignment.Left);
            listView2.Columns.Add("Prio", -2, HorizontalAlignment.Left);
            listView2.Columns.Add("Tid", -2, HorizontalAlignment.Left);
            if(DateTime.Now.Hour >= 12)
                dag = DateTime.Today.Month + "_" + DateTime.Today.Day.ToString() + "_" + DateTime.Today.DayOfWeek.ToString() + "_Förmiddag_" + DateTime.Now.Hour;
            else
                dag = DateTime.Today.Month + "_" + DateTime.Today.Day.ToString()+ "_" + DateTime.Today.DayOfWeek.ToString() + "_Eftermiddag_" + DateTime.Now.Hour;
            textBox1.Text = dag;
            

            try
            {
                Behandlade();
            }
            catch
            {

            }

        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool succes = false;
            try
            {
                int test = Int32.Parse(Prioritering.Text.ToString());
                test = Int32.Parse(textBox3.Text.ToString());
                succes = true;
            }
            catch
            {
                succes = false;
                MessageBox.Show("Du får inte ha bokstäver i prioreterings listan eller åldern!","Patient",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            if(succes == true)
            {
                Patient patient = new Patient();
                patient.namn = textBox4.Text.ToString();
                patient.ålder = textBox3.Text.ToString();
                patient.sympton = SymptonLista.Text.ToString();
                patient.prio = Prioritering.Text.ToString();
                Patients.Add(patient);
                sortedPatients.Add(patient);
                listView1.Items.Clear();
                writeKölista(Patients);
                textBox4.Text = "";
                textBox3.Text = "";
                SymptonLista.Text = "Andningssvårigheter";
                Prioritering.Text = "1";
            }
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            NamnSortering(Patients);
            

        }
        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            ÅlderSortering(Patients);
        }

        void NamnSortering(List<Patient> patients)
        {
            patients.Sort((x, y) => x.namn.CompareTo(y.namn));
            listView1.Items.Clear();
            writeKölista(patients);
        }
        void ÅlderSortering(List<Patient> patients)
        {
            for(int i = 0; i < patients.Count; i++)
            {
                while (patients[i].ålder.Length < 3)
                {
                    patients[i].ålder = "0" + patients[i].ålder;
                }
            }
            patients.Sort((x, y) => y.ålder.CompareTo(x.ålder));
            for (int i = 0; i < patients.Count; i++)
            {
                if(patients.Count != 1)
                {
                    patients[i].ålder = patients[i].ålder.TrimStart('0');
                }
                else
                {
                    patients[i].ålder = patients[i].ålder.Remove(0, 2);
                }
                
            }

            listView1.Items.Clear();
            writeKölista(patients);
        }
        void prioSortering(List<Patient> patients)
        {
            patients.Sort((x, y) => y.prio.CompareTo(x.prio));

            listView1.Items.Clear();
            writeKölista(patients);
        }

        void tidSortering(List<Patient> patients)
        {
            patients.Sort((x, y) => y.väntetid.CompareTo(x.väntetid));

            listView1.Items.Clear();
            writeKölista(patients);
        }
        void SymponSortering(List<Patient> patients)
        {
            patients.Sort((x, y) => x.sympton.CompareTo(y.sympton));

            listView1.Items.Clear();
            writeKölista(patients);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            SymponSortering(Patients);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            prioSortering(Patients);
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            väntatFörlänge = 0;
            int väntetid = 0;
            if(Patients.Count < sortedPatients.Count)
            {
                for(int i = 0; i < sortedPatients.Count; i++)
                {
                    Patients[i] = sortedPatients[i];
                }
            }
            foreach (Patient patient in Patients)
            {
                patient.väntetid++;
            }
            for (int i = 0; i < listView1.Items.Count; i++)
            {
               listView1.BeginUpdate();
                int merPrio = 0;
                int personligMaxvänt = maxvänt / (Int32.Parse((Patients[i].ålder)) / 10);
                int persongligFörlänge = förlängetid / (Int32.Parse((Patients[i].ålder)) / 10);
                if (Patients[i].väntetid > personligMaxvänt)
                {
                    listView1.Items[i].BackColor = Color.Red;
                    väntatFörlänge++;
                    merPrio += 2;
                }
                else if (Patients[i].väntetid > persongligFörlänge)
                {
                    listView1.Items[i].BackColor = Color.Orange;
                    väntatFörlänge++;
                    merPrio++;
                }
                else
                {
                    listView1.Items[i].BackColor = Color.White;
                }
                väntetid += Patients[i].väntetid;
                int timmar = Patients[i].väntetid / 3600;
                int min = (Patients[i].väntetid - timmar * 3600) / 60;
                int sekund = (Patients[i].väntetid - timmar * 3600) - min * 60;
                int nuvarandePrio = Int32.Parse(Patients[i].prio) + merPrio;
                if(timmar > 0)
                    listView1.Items[i].SubItems[4].Text = (timmar + "h  " + min + "min  " + sekund + "s");
                else if (min > 0)
                    listView1.Items[i].SubItems[4].Text = (min + "min  " + sekund + "s");
                else
                    listView1.Items[i].SubItems[4].Text = (sekund + "s");
                listView1.Items[i].SubItems[3].Text = nuvarandePrio.ToString();
                listView1.EndUpdate();
                
            }
            if(listView1.Items.Count > 0)
            {
                väntetid = väntetid / listView1.Items.Count;
                
                int h = väntetid / 3600;
                int minut = (väntetid - h * 3600) / 60;
                int s = (väntetid - h * 3600 - minut * 60);
                if (h > 0)
                    label11.Text = (h + "h  " + minut + "min  " + s + "s");
                else if (minut > 0)
                    label11.Text = (minut + "min  " + s + "s");
                else
                    label11.Text = s + "s";

            }
            else
            {
                label11.Text = "0 min";
            }
            label12.Text = väntatFörlänge.ToString();


        }
        public void Behandlade()
        {
            try
            {
                listView2.Items.Clear();
                string person = File.ReadAllText("Behandlade.txt".ToString());
                string[] indPerson = person.Split('\n','\n');
                

                    for (int i = 0; i < indPerson.GetLength(0); i++)
                    {
                        string[] delPers = indPerson[i].Split('^');
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = delPers[0].ToString();
                        lvi.SubItems.Add(delPers[1].ToString());
                        lvi.SubItems.Add(delPers[2].ToString());
                        lvi.SubItems.Add(delPers[3].ToString());
                        int tid = Int32.Parse(delPers[4]);
                        int timmar = tid / 3600;
                        int min = (tid - timmar * 3600) / 60;
                        int sekund = (tid - timmar * 3600) - min * 60;
                        if (timmar > 0)
                            lvi.SubItems.Add(timmar + "h  " + min + "min  " + sekund + "s");
                        else if (min > 0)
                            lvi.SubItems.Add(min + "min  " + sekund + "s");
                        else
                            lvi.SubItems.Add(sekund + "s");
                        if (Int32.Parse(delPers[4]) > maxvänt)
                            lvi.BackColor = Color.Red;
                        else if (Int32.Parse(delPers[4]) > förlängetid)
                            lvi.BackColor = Color.Orange;


                        listView2.Items.Add(lvi);

                    }

            }
            catch
            {

            }
        }


        
        
        
        public void writeKölista(List<Patient> patients)
        {
            sortedPatients.Clear();
            for (int i = 0; i < patients.Count; i++)
            {
                sortedPatients.Add(patients[i]);   
                ListViewItem lvi = new ListViewItem();
                int merPrio = 0;
                int personligMaxvänt = maxvänt / (Int32.Parse((patients[i].ålder))/10);
                int persongligFörlänge = förlängetid / (Int32.Parse((patients[i].ålder))/10);
                if (patients[i].väntetid > personligMaxvänt)
                {
                    lvi.BackColor = Color.Red;
                    väntatFörlänge++;
                    merPrio += 2;
                }
                else if (patients[i].väntetid > persongligFörlänge)
                {
                    lvi.BackColor = Color.Orange;
                    merPrio++;
                    väntatFörlänge++;
                }
                int nuvarandePrio = Int32.Parse(patients[i].prio) + merPrio;

                lvi.Text = patients[i].namn;
                lvi.SubItems.Add(patients[i].ålder);
                lvi.SubItems.Add(patients[i].sympton);
                lvi.SubItems.Add(nuvarandePrio.ToString());
                int timmar = Patients[i].väntetid / 3600;
                int min = (Patients[i].väntetid - timmar * 3600) / 60;
                int sekund = (Patients[i].väntetid - timmar * 3600) - min * 60;
                if (timmar > 0)
                    lvi.SubItems.Add((timmar + "h  " + min + "min  " + sekund + "s"));
                else if (min > 0)
                    lvi.SubItems.Add((min + "min  " + sekund + "s"));
                else
                    lvi.SubItems.Add((sekund + "s"));

                listView1.Items.Add(lvi);
            }
            for (int i = 0; i < sortedPatients.Count; i++)
            {
                Patients[i] = sortedPatients[i];
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            tidSortering(Patients);
        }

        private void button7_Click(object sender, EventArgs e)
        {

            
            for (int i = 0; i < Patients.Count; i++)
                {
                Debug.WriteLine("WWWW " + sortedPatients.Count + "  " + Patients.Count + "   " + i);
                    if (Patients[i] == sortedPatients[listView1.SelectedItems[0].Index])
                    {
                    string behandladinfo = sortedPatients[listView1.SelectedItems[0].Index].namn + "^" + sortedPatients[listView1.SelectedItems[0].Index].ålder + "^" + sortedPatients[listView1.SelectedItems[0].Index].sympton + "^" + sortedPatients[listView1.SelectedItems[0].Index].prio + "^" + sortedPatients[listView1.SelectedItems[0].Index].väntetid + "^";
                    File.AppendAllText("Behandlade.txt", behandladinfo + Environment.NewLine);
                    listView1.SelectedItems[0].Remove();
                    Behandlade();
                    
                    sortedPatients.Remove(Patients[i]);
                    Patients.Remove(Patients[i]);
                    break;
                    }
            }
               
            
            
            

        }

        private void button8_Click(object sender, EventArgs e)
        {
            File.WriteAllText("Behandlade.txt", "");
            Behandlade();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {

            string behandlade = File.ReadAllText("Behandlade.txt");
            string[] rader = behandlade.Split('\n');
            string behandladeInfo = "Namn   Ålder   Sympton  Prio  Väntetid(s) \n";
            
            for(int i = 0; i < rader.GetLength(0); i++)
            {
                string[] allt = rader[i].Split('^');
                rader[i] = "";
                for(int j = 0; j < allt.GetLength(0); j++)
                {
                    rader[i] += "  " + allt[j];
                }
                behandladeInfo += rader[i] + "\n";
            }

            File.WriteAllText(sparadeBehandledePos,behandladeInfo);


            
            MessageBox.Show("Patienterna har exporterats","Exportering",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Loggin log = new Loggin();
            log.Show();
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            sparadeBehandledePos = textBox1.Text.ToString();
            if (sparadeBehandledePos.EndsWith(".txt"))
                return;
            else
                sparadeBehandledePos += ".txt";
        }
    }

    public class Patient
    {
        public string namn;
        public string ålder;
        public string sympton;
        public string prio;
        public int väntetid = 0;
    }

    
}
