using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO.Ports;

namespace test_arduinomini
{

    public class Target
    {
        public int idNo;
        public bool hit;
        public bool miss;
        public bool active;
        private bool flashTracker;
        public int brightness = 200;
        public int red;
        public int green;
        public int blue;
        public int teamNumber;
        private int redSave;
        private int greenSave;
        private int blueSave;
        private System.Timers.Timer durationTimer;
        private System.Timers.Timer intervalTimer;

        public Target(int id)
        {
            idNo = id;
            flashTracker = true;
            intervalTimer = new System.Timers.Timer();
            durationTimer = new System.Timers.Timer();
            intervalTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerEventProcessorFlipFlop);
            durationTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerEventProcessorStartStop);
            this.teamNumber = -1; //-1 to represent not having been set - long term this code path will be removed
        }

        //function overloads allow you to call the same function name with different number of parameters. Here I'm just using it
        //so that it's optional to pass in teamNumber
        public Target(int id, int teamNumber)
        {
            idNo = id;
            this.teamNumber = teamNumber;
            flashTracker = true;
            intervalTimer = new System.Timers.Timer();
            durationTimer = new System.Timers.Timer();
            intervalTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerEventProcessorFlipFlop);
            durationTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerEventProcessorStartStop);
        }

        private void TimerEventProcessorFlipFlop(Object myObject, EventArgs myEventArgs)
        {
            if(flashTracker)
            {
                red = 0;
                green = 0;
                blue = 0;
                flashTracker = false;
            } else
            {
                red = redSave;
                green = greenSave;
                blue = blueSave;
                flashTracker = true;
            }
        }

        private void TimerEventProcessorStartStop(Object myObject, EventArgs myEventArgs)
        {
            red = redSave;
            green = greenSave;
            blue = blueSave;
            intervalTimer.Stop();
            durationTimer.Stop();
            flashTracker = true;
            active = true;
        }

        public void SetLED(int r, int g, int b)
        {
            red = r;
            green = g;
            blue = b;
        }

        public void SetRed()
        {
            red = brightness;
            green = 0;
            blue = 0;
        }

        public void SetBlue()
        {
            red = 0;
            green = 0;
            blue = brightness;
        }

        public void onHit()
        {
            this.hit = true;
        }

        public void onMiss()
        {
            Flash(500, 5000, true);
            miss = true;
            //active = false;
        }

        public string SendLED()
        {
            string s = idNo.ToString() + "," + red.ToString() + "," + green.ToString() + "," + blue.ToString();
            return s;
        }

        //refactored so that it matches its name and actually sends the signal
        public void SendLED(SerialPort serialPort)
        {
            string s = idNo.ToString() + "," + red.ToString() + "," + green.ToString() + "," + blue.ToString();
            serialPort.WriteLine(s);
        }

        string ping()
        {
            string s = idNo + ",ping";
            return s;
        }

        public bool SetHit()
        {
            if (active)
            {
                hit = true;
                return true;
            } else
            {
                return false;
            }
        }

        public void Flash(int interval, int duration, bool disabled)
        {
            redSave = red;
            greenSave = green;
            blueSave = blue;
            active = false;
            intervalTimer.Interval = interval;
            durationTimer.Interval = duration;
            durationTimer.Start();
            intervalTimer.Start();
        }

        public int getTeamNumber()
        {
            return this.teamNumber;
        }
        /*public void Flash(int in terval, int duration, bool start)
        {
            string s = idNo.ToString() + ",flash," + interval.ToString();
            if(start)
            {
                myTimer.Interval = duration;
                myTimer.Start();
            }
            return s;
        } */
    }
}

