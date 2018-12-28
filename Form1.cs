using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace test_arduinomini
{
    public partial class Form1 : Form
    {
        public delegate void AddDataDelegate(String myString);// Delegate for richtextbox
        public AddDataDelegate myDelegate;// An instance of AddDataDelegate
        public delegate void AddDataDelegate_button(String myString);// Delegate for button
        public AddDataDelegate_button myDelegate_button;// An instance of AddDataDelegate_button
        bool status=false;// LED status
        SoundPlayer missSound = new SoundPlayer(@"C:\Users\n\Desktop\fart_z.wav");
        SoundPlayer hitSound = new SoundPlayer(@"C:\Users\n\Desktop\fanfare_x.wav");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.Open();//Opening the serial port
            this.myDelegate = new AddDataDelegate(AddDataMethod);//Assigning  "the function that changes richtextbox text" to the delegate 
            this.myDelegate_button = new AddDataDelegate_button(AddDataMethod_button);//Assigning "the function that changes button text" to the delegate 
            //serialPort1.WriteLine("STATE");
            
        }
        public void AddDataMethod_button(String myString)
        {
            button1.Text = myString;//changes button text
        }
        public void AddDataMethod(String myString)
        {
            richTextBox1.Text = myString + Environment.NewLine;//changes richtextbox text
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //MessageBox.Show("serial port receive");
            string s = serialPort1.ReadExisting();//reads the serialport buffer
            if (s.Contains("Hit"))
            {
                hitSound.Play();
            }
            else { missSound.Play(); }
            
            /*
            if (s.Contains("state="))//checks if it is status
            {
                s=s.Trim();
                string new_s = s.Replace("state=", "");
                if (new_s.Contains("0"))
                {
                    status = false;
                    button1.Invoke(this.myDelegate_button, new Object[] { "ON" });//sets button text to on
                }
                else
                {
                    status = true;
                    button1.Invoke(this.myDelegate_button, new Object[] { "OFF" });//sets button text to off
                }
            }
            else
            {
                richTextBox1.Invoke(this.myDelegate, new Object[] { s });////adds the recieved bytes to the richtextbox
            }
            */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text.Contains("ON"))
            {
                serialPort1.WriteLine("2,100,0,0");
                missSound.Play();
                button1.Text = "OFF";
                status = false;

                
            }
            else
            {
                hitSound.Play();
                serialPort1.WriteLine("2,0,0,100");
                button1.Text = "ON";
                status = true;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
