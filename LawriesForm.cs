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
    public partial class LawriesForm : Form
    {
        SoundPlayer missSound = new SoundPlayer(@"C:\Users\n\Desktop\fart_z.wav");
        SoundPlayer hitSound = new SoundPlayer(@"C:\Users\n\Desktop\fanfare_x.wav");
        SoundPlayer teamDominationStartSound = new SoundPlayer(@"C:\Users\n\Desktop\target sounds\Announcer Sounds\short list\announcer_attack_controlpoints.wav");
        SoundPlayer countdown1Sound = new SoundPlayer(@"C:\Users\n\Desktop\target sounds\Announcer Sounds\short list\announcer_begins_1sec.wav");
        SoundPlayer countdown2Sound = new SoundPlayer(@"C:\Users\n\Desktop\target sounds\Announcer Sounds\short list\announcer_begins_2sec.wav");
        SoundPlayer countdown3Sound = new SoundPlayer(@"C:\Users\n\Desktop\target sounds\Announcer Sounds\short list\announcer_begins_3sec.wav");
        SoundPlayer countdownBeginSound = new SoundPlayer(@"C:\Users\n\Desktop\target sounds\Announcer Sounds\short list\announcer_am_roundstart03.wav");
        SoundPlayer errorSound = new SoundPlayer(@"C:\Users\n\Desktop\target sounds\Announcer Sounds\short list\cramer-09.wav");
        SoundPlayer bellSound = new SoundPlayer(@"C:\Users\n\Desktop\target sounds\Announcer Sounds\short list\chinese-gong-daniel_simon.wav");
        SoundPlayer endSound = new SoundPlayer(@"C:\Users\n\Desktop\target sounds\Announcer Sounds\short list\announcer_victory.wav");
        

        List<Target> targets = new List<Target>();
        static System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer screenRefresh = new System.Windows.Forms.Timer();
        int redScore;
        int blueScore;
        Random random = new Random();
        int gameDuration = 2*60*4;
        int cycles = 0;

        public LawriesForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.Open();
            button2.Click += new EventHandler(button2_Click);  //button 2 is field domination button

            setupTargets(6);

            gameTimer.Tick += new EventHandler(GameEventLoop);
            screenRefresh.Tick += new EventHandler(screenRefreshEventHandler);
            screenRefresh.Interval = 100;
            screenRefresh.Start();
        }

        private void setupTargets(int numberOfTargets)
        {
            for(int i = 0; i < numberOfTargets; i++)
            {
                targets.Add(new Target(i+2));  //+2 because of some id number thing, 0 is server
            }
        }

        public void AddDataMethod_button(String myString)
        {
            button1.Text = myString;//changes button text
        }

        public void AddDataMethod(String myString)
        {
            richTextBox1.Text = myString + Environment.NewLine;//changes richtextbox text
        }

        public Target getTarget(int id)
        {
            //alteratively, could use the find function
            return targets[id - 2];
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string s = serialPort1.ReadExisting();
            Debug.WriteLine(s);
            if (s.Contains("Hit"))
            {
                string[] messages = s.Split(',');
                if (Int32.TryParse(messages[0], out int id))
                {
                    Target target = getTarget(id);
                    if (target.active)
                    {
                        bellSound.Play();
                        if (isTeam1(target))
                        {
                            target.SetRed();
                            target.teamNumber = 2;
                        }
                        else
                        {
                            target.SetBlue();
                            target.teamNumber = 1;
                        }
                        target.onHit();
                    }
                }
                else {  }
            }
            else if (s.Contains("Miss"))
            {
                string[] messages = s.Split(',');
                if (Int32.TryParse(messages[0], out int id))
                {
                    errorSound.Play();
                    Target target = getTarget(id);
                    target.onMiss();
                }
                else {  }
            }
        }

        private void setAllTargetLights(int r, int g, int b)
        {
            foreach (Target target in targets)
            {
                target.SetLED(r, g, b);
            }
        }

        //maybe helpful but currently not called by anything else.
        private void setTeamTargetLights(int team, int r, int g, int b)
        {
            foreach (Target target in targets)
            {
                if (target.getTeamNumber() == team)
                {
                    target.SetLED(r, g, b);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //why are both branches the same function?
            if (button1.Text.Contains("ON"))
            {
                setAllTargetLights(0, 0, 0);
            }
            else
            {
                setAllTargetLights(0, 0, 0);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private bool isTeam1(Target target)
        {
            return target.getTeamNumber() == 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //needs code for when button is clicked while game is in progress.
            teamDominationStartSound.Play();
            //start game timer
            // on each event check targets and add to total, reset hits and misses as found
            // after x events total up and 
            //redScore = 0;
            //blueScore = 0;
            //foreach (Target target in targets)
            //{
            //    target.SetRed();
            //    target.SendLED(serialPort1);
            //}
            //int half = targets.Count / 2;
            //while (half > 0)
            //{
            //    int randomID = random.Next(2, 7);
            //    Target target = getTarget(randomID);
            //    if (isTeam1(target))
            //    {
            //        target.SetBlue();
            //        target.SendLED(serialPort1);
            //        half--;
            //    }
            //}
            gameTimer.Interval = 250;
            gameTimer.Start();
            setAllTargetLights(0, 0, 0);
        }

        //needs refactor so that it's more generic and supports multiple games.
        private void GameEventLoop(Object myObject, EventArgs myEventArgs)
        {
            if (cycles >= 4*3 && cycles < 4*3+1)
            {
                countdown3Sound.Play();
            }

            if (cycles >= 4 * 4 && cycles < 4 * 4 + 1)
            {
                countdown2Sound.Play();
            }

            if (cycles >= 4 * 5 && cycles < 4 * 5 + 1)
            {
                countdown1Sound.Play();
            }

            if (cycles >= 4 * 6 && cycles < 4 * 6 + 1)
            {
                //if ()
                //{
                    countdownBeginSound.Play();
                //}
                redScore = 0;
                blueScore = 0;
                foreach (Target target in targets)
                {
                    target.SetRed();
                    target.active = true;
                }
                int half = targets.Count / 2;
                while (half > 0)
                {
                    int randomID = random.Next(2, 7);
                    Target target = getTarget(randomID);
                    if (isTeam1(target) == false)
                    {
                        target.teamNumber = 1;
                        target.SetBlue();
                        //target.SendLED(serialPort1);
                        half--;
                    }
                }
            }

            if (cycles > 4 * 7)
            {
                foreach (Target target in targets)
                {
                    if (isTeam1(target))  //how does work?
                    {
                        redScore++;
                    }
                    else
                    {
                        blueScore++;
                    }
                }
                richTextBox1.AppendText("RED:" + redScore.ToString() + " BLUE:" + blueScore.ToString());
            }

            if (cycles >= gameDuration && cycles < gameDuration + 1)
            {
                endSound.Play();
                foreach(Target target in targets)
                {
                    if (redScore > blueScore)
                    {
                        target.SetRed();
                    } else
                    {
                        target.SetBlue();
                    }
                    target.Flash(250, 3000, true);
                }
            }

            if (cycles > (gameDuration + 20))
            {
                setAllTargetLights(0, 0, 0);
                gameTimer.Stop();
            }
            cycles++;
        }

        private void screenRefreshEventHandler(Object myObject, EventArgs myEventArgs)
        {
            foreach (Target target in targets)
            {
                target.SendLED(serialPort1);
            }
        }

        private void singlePlayerGame(Object someObject, EventArgs eventArgs) {
            //variables for score
            //array of hit targets
            //
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