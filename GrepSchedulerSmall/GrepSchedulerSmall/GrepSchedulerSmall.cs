using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrepSchedulerSmall
{
    public partial class GrepSchedulerSmall : UserControl
    {
        private List<bool> m_Scheduler = new List<bool>();

//-->Constructors

        public GrepSchedulerSmall()
        {
            InitializeComponent();
            initScheduler();
        }

//-->Attributes
//-->Methods
        
        private void initScheduler()
        {
            for (int i = 0; i < 24; i++)
                m_Scheduler.Add(true);
        }

        private void setTimeFrame(string p_TimeFrame)
        {
            labelTimeFrame.Text = p_TimeFrame;
        }

        private void switchSchedulerAt(int p_Index)
        {
            m_Scheduler[p_Index] = !m_Scheduler[p_Index];
            setSchedulerAt(p_Index);
        }

        private void setSchedulerAt(int p_Index)
        {
            Color l_Color = Color.Crimson;
            if (m_Scheduler[p_Index])
                l_Color = Color.DarkGreen;
            switch (p_Index)
            {
                case 0:
                    labelTime0.BackColor = l_Color;
                    break;
                case 1:
                    labelTime1.BackColor = l_Color;
                    break;
                case 2:
                    labelTime2.BackColor = l_Color;
                    break;
                case 3:
                    labelTime3.BackColor = l_Color;
                    break;
                case 4:
                    labelTime4.BackColor = l_Color;
                    break;
                case 5:
                    labelTime5.BackColor = l_Color;
                    break;
                case 6:
                    labelTime6.BackColor = l_Color;
                    break;
                case 7:
                    labelTime7.BackColor = l_Color;
                    break;
                case 8:
                    labelTime8.BackColor = l_Color;
                    break;
                case 9:
                    labelTime9.BackColor = l_Color;
                    break;
                case 10:
                    labelTime10.BackColor = l_Color;
                    break;
                case 11:
                    labelTime11.BackColor = l_Color;
                    break;
                case 12:
                    labelTime12.BackColor = l_Color;
                    break;
                case 13:
                    labelTime13.BackColor = l_Color;
                    break;
                case 14:
                    labelTime14.BackColor = l_Color;
                    break;
                case 15:
                    labelTime15.BackColor = l_Color;
                    break;
                case 16:
                    labelTime16.BackColor = l_Color;
                    break;
                case 17:
                    labelTime17.BackColor = l_Color;
                    break;
                case 18:
                    labelTime18.BackColor = l_Color;
                    break;
                case 19:
                    labelTime19.BackColor = l_Color;
                    break;
                case 20:
                    labelTime20.BackColor = l_Color;
                    break;
                case 21:
                    labelTime21.BackColor = l_Color;
                    break;
                case 22:
                    labelTime22.BackColor = l_Color;
                    break;
                case 23:
                    labelTime23.BackColor = l_Color;
                    break;
            }
        }

        public void setScheduler(string p_SchedulerConfig)
        {
            //Set resources
            string[] l_SchedulerConfig = new string[] { "", "" };//dynamic array
            l_SchedulerConfig = p_SchedulerConfig.Split(';');
            for (int i = 0; i < m_Scheduler.Count; i++)
            {
                m_Scheduler[i] = l_SchedulerConfig[i].Equals("True");
                setSchedulerAt(i);
            }
        }

        public string getScheduler()
        {
            string l_Scheduler = "";
            for (int i = 0; i < m_Scheduler.Count; i++)
            {
                l_Scheduler += m_Scheduler[i].ToString() + ";";
            }
            return l_Scheduler;
        }

//-->Events

        private void labelTime0_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("00:00 - 00:59");
        }

        private void labelTime1_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("01:00 - 01:59");
        }

        private void labelTime2_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("02:00 - 02:59");
        }

        private void labelTime3_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("03:00 - 03:59");
        }

        private void labelTime4_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("04:00 - 04:59");
        }

        private void labelTime5_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("05:00 - 05:59");
        }

        private void labelTime6_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("06:00 - 06:59");
        }

        private void labelTime7_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("07:00 - 07:59");
        }

        private void labelTime8_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("08:00 - 08:59");
        }

        private void labelTime9_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("09:00 - 09:59");
        }

        private void labelTime10_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("10:00 - 10:59");
        }

        private void labelTime11_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("11:00 - 11:59");
        }

        private void labelTime12_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("12:00 - 12:59");
        }

        private void labelTime13_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("13:00 - 13:59");
        }

        private void labelTime14_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("14:00 - 14:59");
        }

        private void labelTime15_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("15:00 - 15:59");
        }

        private void labelTime16_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("16:00 - 16:59");
        }

        private void labelTime17_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("17:00 - 17:59");
        }

        private void labelTime18_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("18:00 - 18:59");
        }

        private void labelTime19_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("19:00 - 19:59");
        }

        private void labelTime20_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("20:00 - 20:59");
        }

        private void labelTime21_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("21:00 - 21:59");
        }

        private void labelTime22_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("22:00 - 22:59");
        }

        private void labelTime23_MouseHover(object sender, EventArgs e)
        {
            setTimeFrame("23:00 - 23:59");
        }

        private void labelTime0_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(0);
        }

        private void labelTime1_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(1);
        }

        private void labelTime2_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(2);
        }

        private void labelTime3_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(3);
        }

        private void labelTime4_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(4);
        }

        private void labelTime5_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(5);
        }

        private void labelTime6_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(6);
        }

        private void labelTime7_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(7);
        }

        private void labelTime8_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(8);
        }

        private void labelTime9_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(9);
        }

        private void labelTime10_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(10);
        }

        private void labelTime11_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(11);
        }

        private void labelTime12_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(12);
        }

        private void labelTime13_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(13);
        }

        private void labelTime14_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(14);
        }

        private void labelTime15_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(15);
        }

        private void labelTime16_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(16);
        }

        private void labelTime17_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(17);
        }

        private void labelTime18_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(18);
        }

        private void labelTime19_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(19);
        }

        private void labelTime20_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(20);
        }

        private void labelTime21_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(21);
        }

        private void labelTime22_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(22);
        }

        private void labelTime23_Click(object sender, EventArgs e)
        {
            switchSchedulerAt(23);
        }
    }
}
