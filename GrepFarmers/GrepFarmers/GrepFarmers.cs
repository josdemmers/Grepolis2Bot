using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrepFarmers
{
    public partial class GrepFarmers : UserControl
    {
        private string m_Index = "0";
        private string m_TownID = "0";
        private string m_TownName = "No town selected";

        private ToolTip m_Tooltip = new ToolTip();

//-->Constructor

        public GrepFarmers()
        {
            InitializeComponent();
        }

//-->Attributes

        public string Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        public string TownID
        {
            get { return m_TownID; }
            set { m_TownID = value; }
        }

        public string TownName
        {
            get { return m_TownName; }
            set
            { 
                m_TownName = value;
                groupBoxTown.Text = m_TownName;
            }
        }

//-->Methods

        public void setFarmersStatus(string p_Relations, string p_Limits)
        {
            string[] l_Relations = new string[] { "", "" };//dynamic array
            string[] l_Limits = new string[] { "", "" };//dynamic array
            l_Relations = p_Relations.Split(';');
            //Input parameter is true when limit is reached. Need to invert boolean here!
            l_Limits = p_Limits.Split(';');

            for (int i = 0; i < flowLayoutPanelFarmersA.Controls.Count; i++)
            {
                if (l_Relations[i].Equals("False") && l_Limits[i].Equals("True"))
                    ((Label)(flowLayoutPanelFarmersA.Controls[i])).Image = global::GrepFarmers.Properties.Resources.farm_town_1_0_0;
                else if(l_Relations[i].Equals("False") && l_Limits[i].Equals("False"))
                    ((Label)(flowLayoutPanelFarmersA.Controls[i])).Image = global::GrepFarmers.Properties.Resources.farm_town_1_0_1;
                else if (l_Relations[i].Equals("True") && l_Limits[i].Equals("True"))
                    ((Label)(flowLayoutPanelFarmersA.Controls[i])).Image = global::GrepFarmers.Properties.Resources.farm_town_1_1_0;
                else
                    ((Label)(flowLayoutPanelFarmersA.Controls[i])).Image = global::GrepFarmers.Properties.Resources.farm_town_1_1_1;
            }

            for (int i = 0; i < flowLayoutPanelFarmersB.Controls.Count; i++)
            {
                if (l_Relations[i+4].Equals("False") && l_Limits[i+4].Equals("True"))
                    ((Label)(flowLayoutPanelFarmersB.Controls[i])).Image = global::GrepFarmers.Properties.Resources.farm_town_1_0_0;
                else if (l_Relations[i+4].Equals("False") && l_Limits[i+4].Equals("False"))
                    ((Label)(flowLayoutPanelFarmersB.Controls[i])).Image = global::GrepFarmers.Properties.Resources.farm_town_1_0_1;
                else if (l_Relations[i+4].Equals("True") && l_Limits[i+4].Equals("True"))
                    ((Label)(flowLayoutPanelFarmersB.Controls[i])).Image = global::GrepFarmers.Properties.Resources.farm_town_1_1_0;
                else
                    ((Label)(flowLayoutPanelFarmersB.Controls[i])).Image = global::GrepFarmers.Properties.Resources.farm_town_1_1_1;
            }

        }

        public void setFarmersName(string p_Names)
        {
            string[] l_Names = new string[] { "", "" };//dynamic array
            l_Names = p_Names.Split(';');

            for (int i = 0; i < flowLayoutPanelFarmersA.Controls.Count; i++)
            {
                m_Tooltip.SetToolTip(((Label)(flowLayoutPanelFarmersA.Controls[i])), l_Names[i]);
            }

            for (int i = 0; i < flowLayoutPanelFarmersB.Controls.Count; i++)
            {
                m_Tooltip.SetToolTip(((Label)(flowLayoutPanelFarmersB.Controls[i])), l_Names[i+4]);
            }
        }

        public void setFarmersLootTime(string p_LootTimes)
        {
            string[] l_LootTimes = new string[] { "", "" };//dynamic array
            l_LootTimes = p_LootTimes.Split(';');

            labelTimerFarmer1.Text = l_LootTimes[0];
            labelTimerFarmer2.Text = l_LootTimes[1];
            labelTimerFarmer3.Text = l_LootTimes[2];
            labelTimerFarmer4.Text = l_LootTimes[3];
            labelTimerFarmer5.Text = l_LootTimes[4];
            labelTimerFarmer6.Text = l_LootTimes[5];
            labelTimerFarmer7.Text = l_LootTimes[6];
            labelTimerFarmer8.Text = l_LootTimes[7];
        }

        public void setFarmersMood(string p_Mood)
        {
            string[] l_Mood = new string[] { "", "" };//dynamic array
            l_Mood = p_Mood.Split(';');

            labelMoodFarmer1.Text = l_Mood[0];
            labelMoodFarmer2.Text = l_Mood[1];
            labelMoodFarmer3.Text = l_Mood[2];
            labelMoodFarmer4.Text = l_Mood[3];
            labelMoodFarmer5.Text = l_Mood[4];
            labelMoodFarmer6.Text = l_Mood[5];
            labelMoodFarmer7.Text = l_Mood[6];
            labelMoodFarmer8.Text = l_Mood[7];
        }

        public void setFarmersLimit(string p_Limit)
        {
            string[] l_Limit = new string[] { "", "" };//dynamic array
            l_Limit = p_Limit.Split(';');

            labelLimitFarmer1.Text = l_Limit[0];
            labelLimitFarmer2.Text = l_Limit[1];
            labelLimitFarmer3.Text = l_Limit[2];
            labelLimitFarmer4.Text = l_Limit[3];
            labelLimitFarmer5.Text = l_Limit[4];
            labelLimitFarmer6.Text = l_Limit[5];
            labelLimitFarmer7.Text = l_Limit[6];
            labelLimitFarmer8.Text = l_Limit[7];
        }

        public string getSelectedFarmers()
        {
            string l_Selected = checkBoxFarmer1.Checked.ToString() + ";" + checkBoxFarmer2.Checked.ToString() + ";" + checkBoxFarmer3.Checked.ToString() + ";" +
                checkBoxFarmer4.Checked.ToString() + ";" + checkBoxFarmer5.Checked.ToString() + ";" + checkBoxFarmer6.Checked.ToString() + ";" +
                checkBoxFarmer7.Checked.ToString() + ";" + checkBoxFarmer8.Checked.ToString() + ";";
            return l_Selected;
        }

        public void setSelectedFarmers(string p_SelectedFarmers)
        {
            try
            {
                string[] l_SelectedFarmers = new string[] { "", "" };//dynamic array
                l_SelectedFarmers = p_SelectedFarmers.Split(';');
                checkBoxFarmer1.Checked = l_SelectedFarmers[0].Equals("True");
                checkBoxFarmer2.Checked = l_SelectedFarmers[1].Equals("True");
                checkBoxFarmer3.Checked = l_SelectedFarmers[2].Equals("True");
                checkBoxFarmer4.Checked = l_SelectedFarmers[3].Equals("True");
                checkBoxFarmer5.Checked = l_SelectedFarmers[4].Equals("True");
                checkBoxFarmer6.Checked = l_SelectedFarmers[5].Equals("True");
                checkBoxFarmer7.Checked = l_SelectedFarmers[6].Equals("True");
                checkBoxFarmer8.Checked = l_SelectedFarmers[7].Equals("True");
            }
            catch(Exception)
            {
            }
        }

        public void selectAll()
        {
            checkBoxFarmer1.Checked = true;
            checkBoxFarmer2.Checked = true;
            checkBoxFarmer3.Checked = true;
            checkBoxFarmer4.Checked = true;
            checkBoxFarmer5.Checked = true;
            checkBoxFarmer6.Checked = true;
            checkBoxFarmer7.Checked = true;
            checkBoxFarmer8.Checked = true;
        }

        public void deselectAll()
        {
            checkBoxFarmer1.Checked = false;
            checkBoxFarmer2.Checked = false;
            checkBoxFarmer3.Checked = false;
            checkBoxFarmer4.Checked = false;
            checkBoxFarmer5.Checked = false;
            checkBoxFarmer6.Checked = false;
            checkBoxFarmer7.Checked = false;
            checkBoxFarmer8.Checked = false;
        }

        public void hideFarmers()
        {
            flowLayoutPanelFarmersLayout.Visible = false;
        }

        public void showFarmers()
        {
            flowLayoutPanelFarmersLayout.Visible = true;
        }
    }
}
