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
using System.IO.Ports;






namespace SLM
{
    public partial class Form1 : Form
    {
        String idlog;
        double totaldb,dosis,ratarata,dosistotal=0;
        decimal dosistotals=0;
        int jumlahdata;
        delegate void invok(String m);
       // invok invoker;
        SerialPort port = new SerialPort();
        string comPort, RxString;
       // string[] ArduinoData = null;
        string[] ports = SerialPort.GetPortNames();
        public Form1()
        {
            InitializeComponent();
            foreach (string s in ports) comboBox1.Items.Add(s);
            port.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

        }
        String data;
        private void chart1_Click(object sender, EventArgs e)
        {




        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "TXT files|*.txt";
            openFileDialog1.Title = "Open Data Sensor";
            openFileDialog1.ShowDialog();
            data = openFileDialog1.FileName;

            //String fileName = "datacek.txt";
            StreamReader perintah = File.OpenText(data);           
            StreamReader perintah2 = File.OpenText(@"C:\DATA LOGGER\DATE\" + data.Substring(21,5) + ".txt");
            StreamReader perintah3 = File.OpenText(@"C:\DATA LOGGER\DOSIS\" + data.Substring(21, 5) + ".txt");



            String baca,tgl,dosis;
            string[] datatxt = new string[99999];
            string[] datatxt2 = new string[99999];
            string[] datatxt3 = new string[99999];
            int pjng;
            DateTime time = DateTime.Now;

            if ((baca = perintah.ReadLine()) != null && (tgl = perintah2.ReadLine()) != null && (dosis = perintah3.ReadLine()) != null)
            {
                chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;


                datatxt = baca.Split('#');
                datatxt2 = tgl.Split('#');
                datatxt3 = dosis.Split('#');
                pjng = datatxt.Length - 1;
                
                for (int i = 0; i < pjng; i++)
                {
                    int p = chart1.Series[0].Points.AddXY(datatxt2[i].ToString(), datatxt[i].ToString());
                    int r =  chart1.Series[1].Points.AddXY(datatxt2[i].ToString(), 85);

                    richTextBox1.AppendText(datatxt[i] + " dB(A)" +"  "+ datatxt2[i].ToString() + " "+ datatxt3[i].ToString() + "\n");
                    richTextBox1.ScrollToCaret();
                    label2.Text = datatxt3[i].ToString();

                    //chart1.Series[0].Points[p].Label = datatxt[i] + "";
                }

            }
            else
            {

            }
            perintah.Close();


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comPort = Convert.ToString(comboBox1.SelectedItem); // Set selected COM port
            try
            {
                port.PortName = comPort;
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect :: " + ex.Message, "Sorry, I just couldn't do it...");
            }

            if (port.IsOpen == true)
            {
                comboBox1.Enabled = false;
            }
        }

        private void bunifuToggleSwitch1_OnValuechange(object sender, EventArgs e)
        {
            if (bunifuToggleSwitch1.Value == true)
            {
                label3.Text = "AUTO";
            }
            else {
                label3.Text = "MANUAL";
            }
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            ports = SerialPort.GetPortNames();
            foreach (string s in ports) comboBox1.Items.Add(s);
        }


        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DateTime time = DateTime.Now;

            if (port.IsOpen == true)
            {
                try
                {

                    RxString = port.ReadLine();
                    // ArduinoData = RxString.Split('|');

                    //this.BeginInvoke(invoker, ArduinoData[9] + "|" + ArduinoData[10]);
                    try {
                      
                        richTextBox1.Invoke(new Action(() => richTextBox1.AppendText(RxString.Substring(2, 5) +" dB(A)"+"  "+ time.ToString("h:mm:ss")+" "+dosistotals.ToString()+ "\n")));
                        richTextBox1.Invoke(new Action(() => richTextBox1.ScrollToCaret()));
                        
                        chart1.Invoke(new Action(() => chart1.Series[0].Points.AddXY(time.ToString("h:mm:ss"), RxString.Substring(2, 5))));
                        chart1.Invoke(new Action(() => chart1.Series[1].Points.AddXY (time.ToString("h:mm:ss"),85)));
                        StreamWriter sw = new StreamWriter(@"C:\DATA LOGGER\SOUND\"+idlog+".txt", true);
                        sw.Write(RxString.Substring(2, 5) + "#");
                        sw.Close();
                        StreamWriter swa = new StreamWriter(@"C:\DATA LOGGER\DATE\" + idlog + ".txt", true);
                        swa.Write(time.ToString("h:mm:ss") + "#");
                        swa.Close();

                        StreamWriter swas = new StreamWriter(@"C:\DATA LOGGER\DOSIS\" + idlog + ".txt", true);
                        swas.Write(dosistotals+"#");
                        swas.Close();


                        if (Convert.ToDouble(RxString.Substring(2, 5)) >= 85)
                        {
                            totaldb += Convert.ToDouble(RxString.Substring(2, 5));
                            jumlahdata++;
                            ratarata = totaldb / jumlahdata;

                            if(ratarata>=85 && ratarata < 86){
                                dosis = ratarata / 28800;
                            }
                            else if(ratarata >= 86 && ratarata < 87)
                            {
                                dosis = ratarata / 28800;
                            }
                            else if (ratarata >= 87 && ratarata < 88)
                            {
                                dosis = ratarata / 19200;
                            }
                            else if (ratarata >= 88 && ratarata < 89)
                            {
                                dosis = ratarata / 14400;
                            }
                            else if (ratarata >= 89 && ratarata < 90)
                            {
                                dosis = ratarata / 9600;
                            }
                            else if (ratarata >= 90 && ratarata < 91)
                            {
                                dosis = ratarata / 7200;
                            }
                            else if (ratarata >= 91 && ratarata < 92)
                            {
                                dosis = ratarata / 6000;
                            }
                            else if (ratarata >= 92 && ratarata < 93)
                            {
                                dosis = ratarata / 4800;
                            }
                            else if (ratarata >= 93 && ratarata < 94)
                            {
                                dosis = ratarata / 3600;
                            }
                            else if (ratarata >= 94 && ratarata < 95)
                            {
                                dosis = ratarata / 3000;
                            }
                            else if (ratarata >= 95 && ratarata < 96)
                            {
                                dosis = ratarata / 2400;
                            }
                            else if (ratarata >= 96 && ratarata < 97)
                            {
                                dosis = ratarata / 1800;
                            }
                            else if (ratarata >= 97 && ratarata < 98)
                            {
                                dosis = ratarata / 1500;
                            }
                            else if (ratarata >= 98 && ratarata < 99)
                            {
                                dosis = ratarata / 1200;
                            }
                            else if (ratarata >= 99 && ratarata < 100)
                            {
                                dosis = ratarata / 900;
                            }
                            else if (ratarata >= 100 && ratarata < 101)
                            {
                                dosis = ratarata / 750;
                            }
                            else if (ratarata >= 101 && ratarata < 102)
                            {
                                dosis = ratarata / 600;
                            }
                            else if (ratarata >= 102 && ratarata < 103)
                            {
                                dosis = ratarata / 450;
                            }
                            else if (ratarata >= 103 && ratarata < 104)
                            {
                                dosis = ratarata / 375;
                            }
                            else if (ratarata >= 104 && ratarata < 105)
                            {
                                dosis = ratarata / 300;
                            }
                            else if (ratarata >= 105 && ratarata < 106)
                            {
                                dosis = ratarata / 225;
                            }
                            else if (ratarata >= 106 && ratarata < 107)
                            {
                                dosis = ratarata / 187;
                            }
                            else if (ratarata >= 107 && ratarata < 108)
                            {
                                dosis = ratarata / 150;
                            }
                            else if (ratarata >= 108 && ratarata < 109)
                            {
                                dosis = ratarata / 112;
                            }
                            else if (ratarata >= 109 && ratarata < 110)
                            {
                                dosis = ratarata / 93;
                            }
                            else if (ratarata >= 110 && ratarata < 111)
                            {
                                dosis = ratarata / 75;
                            }
                            else if (ratarata >= 111 && ratarata < 112)
                            {
                                dosis = ratarata / 56;
                            }
                            else if (ratarata >= 112 && ratarata < 113)
                            {
                                dosis = ratarata / 47;
                            }
                            else if (ratarata >= 113 && ratarata < 114)
                            {
                                dosis = ratarata / 37;
                            }
                            else if (ratarata >= 114 && ratarata < 115)
                            {
                                dosis = ratarata / 28;
                            }
                            else if (ratarata >= 115 && ratarata < 116)
                            {
                                dosis = ratarata / 23;
                            }
                            else if (ratarata >= 116 && ratarata < 117)
                            {
                                dosis = ratarata / 18;
                            }
                            else if (ratarata >= 117 && ratarata < 118)
                            {
                                dosis = ratarata / 14;
                            }
                            else if (ratarata >= 118 && ratarata < 119)
                            {
                                dosis = ratarata / 11;
                            }
                            else if (ratarata >= 119 && ratarata < 120)
                            {
                                dosis = ratarata / 9;
                            }
                            else if (ratarata >= 120 && ratarata < 121)
                            {
                                dosis = ratarata / 7;
                            }
                            else if (ratarata >= 121 && ratarata < 122)
                            {
                                dosis = ratarata / 5;
                            }
                            else if (ratarata >= 122 && ratarata < 123)
                            {
                                dosis = ratarata / 4;
                            }
                            else if (ratarata >= 123 && ratarata < 124)
                            {
                                dosis = ratarata / 3;
                            }
                            else if (ratarata >= 124 && ratarata < 125)
                            {
                                dosis = ratarata / 2;
                            }
                            else if (ratarata >= 125 && ratarata < 126)
                            {
                                dosis = ratarata / 1;
                            }
                            else
                            {
                                dosis = ratarata;
                            }

                            dosistotal += dosis;
                            dosistotals = Math.Round(Convert.ToDecimal(dosistotal), 5);
                            label2.Invoke(new Action(() => label2.Text=dosistotals.ToString()));
                            if (dosistotal > 1) {
                                label2.Invoke(new Action(() => label2.ForeColor=Color.Red));
                            }

                        }
                    }
                    catch { }
                    
                   // this.chart1.Series[0].Points.AddXY("tes", 22);
                    //Invalidate();




                }
                catch { }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            port.Close();
            comboBox1.Enabled = true;
            foreach(var series in chart1.Series)
            {
                series.Points.Clear();
            }

            richTextBox1.ResetText();
            Random rnd = new Random();
            idlog = Convert.ToString(rnd.Next(10001, 99999));
        }


        //string es = "A12.00B~", jj;
        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(es.Substring(1, 5));
            chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            Random rnd = new Random();
            idlog = Convert.ToString(rnd.Next(10001,99999));
        }
    }
}
