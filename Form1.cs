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
using System.Diagnostics;


namespace test_arduinomini
{
    public partial class Form1 : Form
    {
        public delegate void AddDataDelegate(String myString);// Delegate for richtextbox
        public AddDataDelegate myDelegate;// An instance of AddDataDelegate
        public delegate void AddDataDelegate_button(String myString);// Delegate for button
        public AddDataDelegate_button myDelegate_button;// An instance of AddDataDelegate_button
        SoundPlayer missSound = new SoundPlayer(@"C:\Users\n\Desktop\fart_z.wav");
        SoundPlayer hitSound = new SoundPlayer(@"C:\Users\n\Desktop\fanfare_x.wav");
        List<Target> targets = new List<Target>();
        static System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer screenRefresh = new System.Windows.Forms.Timer();
        int redScore;
        int blueScore;
        Random random = new Random();
        int brightness = 200;
        int gameDuration = 1200;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.Open();//Opening the serial port
            this.myDelegate = new AddDataDelegate(AddDataMethod);//Assigning  "the function that changes richtextbox text" to the delegate 
            this.myDelegate_button = new AddDataDelegate_button(AddDataMethod_button);//Assigning "the function that changes button text" to the delegate
            button2.Click += new EventHandler(button2_Click);

            targets.Add(new Target(2));
            targets.Add(new Target(3));
            targets.Add(new Target(4));
            targets.Add(new Target(5));
            targets.Add(new Target(6));
            targets.Add(new Target(7));

            gameTimer.Tick += new EventHandler(TimerEventProcessor);
            screenRefresh.Tick += new EventHandler(TimerEventProcessor2);
            screenRefresh.Interval = 100;
            screenRefresh.Start();
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
            Debug.WriteLine(s);
            if (s.Contains("Hit"))
            {
                string[] messages = s.Split(',');
                if (Int32.TryParse(messages[0], out int id))
                {
                    Target target;
                    target = targets.Find(x => x.idNo.Equals(id));
                    if (target.active)
                    {
                        if (target.red == 0)
                        {
                            target.SetLED(brightness, 0, 0);
                        }
                        else
                        {
                            target.SetLED(0, 0, brightness);
                        }
                        target.hit = true;
                    }
                }
                else { MessageBox.Show("no sender id found"); }
            }
            else if (s.Contains("Miss"))
            {
                string[] messages = s.Split(',');
                if (Int32.TryParse(messages[0], out int id))
                {
                    Target target;
                    target = targets.Find(x => x.idNo.Equals(id));
                    //serialPort1.WriteLine(target.Flash(200, 5000, true));
                    target.Flash(500, 5000, true);
                    target.miss = true;
                    target.active = false;
                }
                else { MessageBox.Show("no sender id found"); }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Contains("ON"))
            {
                foreach (Target target in targets)
                {
                    target.SetLED(0, 0, 0);
                }
            }
            else
            {
                foreach (Target target in targets)
                {
                    target.SetLED(0, 0, 0);
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //start game timer
            // on each event check targets and add to total, reset hits and misses as found
            // after x events total up and 
            redScore = 0;
            blueScore = 0;
            foreach (Target target in targets)
            {
                target.SetLED(brightness, 0, 0);
                serialPort1.WriteLine(target.SendLED());
            }
            int half = targets.Count / 2;
            while (half > 0)
            {
                int randomID = random.Next(2, 7);
                Target target = targets.Find(x => x.idNo.Equals(randomID));
                if (target.blue == 0 )
                {
                    target.SetLED(0, 0, brightness);
                    serialPort1.WriteLine(target.SendLED());
                    half--;
                }
            }
            gameTimer.Interval = 250;
            gameTimer.Start();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs) //static keyword removed
        {
            
            //iterate through targets checking hit and miss state
            foreach (Target target in targets)
            {
                if (target.red == 0)
                {
                    blueScore++;
                }
                else
                {
                    redScore++;
                }
            }
            //richTextBox1.AppendText("RED:" + redScore.ToString() + " BLUE:" + blueScore.ToString());

        }

        private void TimerEventProcessor2(Object myObject, EventArgs myEventArgs) //static keyword removed
        {
            foreach (Target target in targets)
            {
                serialPort1.WriteLine(target.SendLED());
            }
         
        }
    }

}
/* Ideas for games
 * 1. Team control the flag. RED vs BLUE team. hit targets to change colour. If team pulls big lead and has most of field show briefly
 * a green or wiping target, when hit this target flips the field. when targets detect a miss they flash and lock out
 * 
 * 2. Random race. Turn based game. Field is blank at the start. a random target lights and the shooter has a chance to hit and lock it in
 * to start earning points. If you hit, another target lights and can be locked in. If miss the field switches and a random light illuminates
 * for the other player to begin his turn. number of hit targets on the field maxes out at 50%. Game plays for X shots, each player scores 
 * per target lit on field. If targets difficulty is high contest is to control large part of field, if low then dominating the field. 
 * Cap aims to limit the lead a player can develop.
 * 
 * 3. zombie hoard
 * 
 * 4. Consecutive hits single player (piano scales recommended by lawrie)
 * 
 * 5. shootout. turn based. plays like skate with random targets lighting.
 * 
 * 6. co-op consecutive hit game, have x.x seconds to hit hit zone after a miss to recover a shot or a random miss on a randomly illuminated to
 * keep the run but lose the point
 * 
 * Ideas for LEDs
 * Full flash
 * Left Right Wipe
 * Battery bar
 * 
 * Operating functions
 * check battery
 * set id
 * set hit/miss mechanics
 * drop out unresponsive targets
 */