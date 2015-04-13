using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrepCulture
{
    public partial class GrepCulture : UserControl
    {
        private bool m_EnabledParty = false;
        private bool m_EnabledGames = false;
        private bool m_EnabledTriumph = false;
        private bool m_EnabledTheater = false;
        private string m_Index = "0";
        private string m_TownName = "No town selected";
        private string m_TownID = "0";

//-->Constructor        
        public GrepCulture()
        {
            InitializeComponent();
        }

        public GrepCulture(string p_Index, string p_ID, string p_Name, bool p_EnabledGlobal, bool p_EnabledParty, bool p_EnabledGames, bool p_EnabledTriumph, bool p_EnabledTheater)
        {
            InitializeComponent();
            m_Index = p_Index;
            m_TownName = p_Name;
            m_TownID = p_ID;
            groupBoxTown.Text = p_Name;
            m_EnabledParty = p_EnabledParty;
            m_EnabledGames = p_EnabledGames;
            m_EnabledTriumph = p_EnabledTriumph;
            m_EnabledTheater = p_EnabledTheater;
        }

//-->Attributes
        public string Index
        {
          get { return m_Index; }
          set { m_Index = value; }
        }

        public string TownName
        {
            get { return m_TownName; }
            set 
            { 
                m_TownName = value;
                groupBoxTown.Text = value;
            }
        }

        public string TownID
        {
            get { return m_TownID; }
            set { m_TownID = value; }
        }

        public bool EnabledParty
        {
            get { return m_EnabledParty; }
            set 
            { 
                m_EnabledParty = value;
                checkBoxPartyEnabled.Checked = value;
            }
        }

        public bool EnabledGames
        {
            get { return m_EnabledGames; }
            set
            { 
                m_EnabledGames = value;
                checkBoxGamesEnabled.Checked = value;
            }
        }

        public bool EnabledTriumph
        {
            get { return m_EnabledTriumph; }
            set 
            { 
                m_EnabledTriumph = value;
                checkBoxTriumphEnabled.Checked = value;
            }
        }

        public bool EnabledTheater
        {
            get { return m_EnabledTheater; }
            set
            {
                m_EnabledTheater = value;
                checkBoxTheaterEnabled.Checked = value;
            }
        }

//-->Methods

//-->Event handlers
        private void checkBoxPartyEnabled_CheckedChanged(object sender, EventArgs e)
        {
            m_EnabledParty = checkBoxPartyEnabled.Checked;
        }

        private void checkBoxGamesEnabled_CheckedChanged(object sender, EventArgs e)
        {
            m_EnabledGames = checkBoxGamesEnabled.Checked;
        }

        private void checkBoxTriumphEnabled_CheckedChanged(object sender, EventArgs e)
        {
            m_EnabledTriumph = checkBoxTriumphEnabled.Checked;
        }

        private void checkBoxTheaterEnabled_CheckedChanged(object sender, EventArgs e)
        {
            m_EnabledTheater = checkBoxTheaterEnabled.Checked;
        }
    }
}
