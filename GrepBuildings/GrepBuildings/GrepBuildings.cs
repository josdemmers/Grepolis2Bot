using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrepBuildings
{
    public partial class GrepBuildings : UserControl
    {
        private string m_Index = "0";
        private string m_TownID = "0";
        private string m_TownName = "No town selected";

        private int m_SizeOfQueueBot = 0;
        private int m_SizeOfQueueGame = 0;

        private Color m_LevelTargetColor = Color.GreenYellow;

        private bool m_TooltipsEnabled = false;
        private ToolTip m_Tooltip = new ToolTip();
        private ImageZoom m_ImageZoom = new ImageZoom();

//-->Constructor        

        public GrepBuildings()
        {
            InitializeComponent();
        }

        public GrepBuildings(string p_Index, string p_TownID, string p_TownName, bool p_Tooltips)
        {
            InitializeComponent();
            m_Index = p_Index;
            m_TownID = p_TownID;
            m_TownName = p_TownName;
            groupBoxTown.Text = m_TownName;
            m_TooltipsEnabled = p_Tooltips;
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
            set { m_TownName = value; }
        }

        public Color LevelTargetColor
        {
            get { return m_LevelTargetColor; }
            set { m_LevelTargetColor = value; }
        }

        public bool TooltipsEnabled
        {
            get { return m_TooltipsEnabled; }
            set {m_TooltipsEnabled = value; }
        }

//-->Methods

        private void setZoomImageByName(string p_Building)
        {
            try
            {
                m_ImageZoom.setImageByName(p_Building);
                //Set tooltip position
                int l_X = MousePosition.X;
                int l_Y = MousePosition.Y;
                m_ImageZoom.setLocation(l_X - 10, l_Y - 45);

                //Show tooltip
                if (!m_ImageZoom.Visible && m_TooltipsEnabled)
                {
                    m_ImageZoom.Show();
                    m_ImageZoom.Refresh();
                }
                else if(!m_TooltipsEnabled)
                {
                    m_ImageZoom.Hide();
                }
            }
            catch (Exception)
            {

            }
        }

        public void setHeroMode(bool p_IsActive)
        {
            if (p_IsActive)
            {
                numericUpDownStorage.Maximum = 35;
                numericUpDownFarm.Maximum = 45;
                numericUpDownAcademy.Maximum = 36;
                numericUpDownTemple.Maximum = 30;
            }
            else
            {
                numericUpDownStorage.Maximum = 30;
                numericUpDownFarm.Maximum = 40;
                numericUpDownAcademy.Maximum = 30;
                numericUpDownTemple.Maximum = 25;
            }
        }

        public void setTownName(string p_TownName)
        {
            m_TownName = p_TownName;
            groupBoxTown.Text = m_TownName;
        }

        public string getBuildingQueue()
        {
            string l_Queue = "";
            for (int i = 0; i < m_SizeOfQueueBot; i++)
            {
                l_Queue += ((Label)(flowLayoutPanelQueueBot.Controls[i])).Tag.ToString() + ";";
            }
            return l_Queue;
        }

        public void setBuildingLevels(string p_BuildingLevels)
        {
            string[] l_Buildings = new string[] { "", "" };//dynamic array
            l_Buildings = p_BuildingLevels.Split(';');

            for (int i = 0; i < l_Buildings.Length; i++)
            {
                if (i < flowLayoutPanelBuildings.Controls.Count / 2)
                    ((Label)(flowLayoutPanelBuildings.Controls[i * 2])).Text = l_Buildings[i];
            }

            setBuildingLevelColors();
        }

        private void setBuildingLevelColors()
        {
            if (numericUpDownMain.Value.ToString().Equals(labelMainImg.Text))
                labelMainImg.ForeColor = m_LevelTargetColor;
            else
                labelMainImg.ForeColor = Color.White;
            if (numericUpDownHide.Value.ToString().Equals(labelHideImg.Text))
                labelHideImg.ForeColor = m_LevelTargetColor;
            else
                labelHideImg.ForeColor = Color.White;
            if (numericUpDownStorage.Value.ToString().Equals(labelStorageImg.Text))
                labelStorageImg.ForeColor = m_LevelTargetColor;
            else
                labelStorageImg.ForeColor = Color.White;
            if (numericUpDownFarm.Value.ToString().Equals(labelFarmImg.Text))
                labelFarmImg.ForeColor = m_LevelTargetColor;
            else
                labelFarmImg.ForeColor = Color.White;
            if (numericUpDownLumber.Value.ToString().Equals(labelLumberImg.Text))
                labelLumberImg.ForeColor = m_LevelTargetColor;
            else
                labelLumberImg.ForeColor = Color.White;
            if (numericUpDownStoner.Value.ToString().Equals(labelStonerImg.Text))
                labelStonerImg.ForeColor = m_LevelTargetColor;
            else
                labelStonerImg.ForeColor = Color.White;
            if (numericUpDownIroner.Value.ToString().Equals(labelIronerImg.Text))
                labelIronerImg.ForeColor = m_LevelTargetColor;
            else
                labelIronerImg.ForeColor = Color.White;
            if (numericUpDownMarket.Value.ToString().Equals(labelMarketImg.Text))
                labelMarketImg.ForeColor = m_LevelTargetColor;
            else
                labelMarketImg.ForeColor = Color.White;
            if (numericUpDownDocks.Value.ToString().Equals(labelDocksImg.Text))
                labelDocksImg.ForeColor = m_LevelTargetColor;
            else
                labelDocksImg.ForeColor = Color.White;
            if (numericUpDownBarracks.Value.ToString().Equals(labelBarracksImg.Text))
                labelBarracksImg.ForeColor = m_LevelTargetColor;
            else
                labelBarracksImg.ForeColor = Color.White;
            if (numericUpDownWall.Value.ToString().Equals(labelWallImg.Text))
                labelWallImg.ForeColor = m_LevelTargetColor;
            else
                labelWallImg.ForeColor = Color.White;
            if (numericUpDownAcademy.Value.ToString().Equals(labelAcademyImg.Text))
                labelAcademyImg.ForeColor = m_LevelTargetColor;
            else
                labelAcademyImg.ForeColor = Color.White;
            if (numericUpDownTemple.Value.ToString().Equals(labelTempleImg.Text))
                labelTempleImg.ForeColor = m_LevelTargetColor;
            else
                labelTempleImg.ForeColor = Color.White;
            if (numericUpDownTheater.Value.ToString().Equals(labelTheaterImg.Text))
                labelTheaterImg.ForeColor = m_LevelTargetColor;
            else
                labelTheaterImg.ForeColor = Color.White;
            if (numericUpDownThermal.Value.ToString().Equals(labelThermalImg.Text))
                labelThermalImg.ForeColor = m_LevelTargetColor;
            else
                labelThermalImg.ForeColor = Color.White;
            if (numericUpDownLibrary.Value.ToString().Equals(labelLibraryImg.Text))
                labelLibraryImg.ForeColor = m_LevelTargetColor;
            else
                labelLibraryImg.ForeColor = Color.White;
            if (numericUpDownLighthouse.Value.ToString().Equals(labelLighthouseImg.Text))
                labelLighthouseImg.ForeColor = m_LevelTargetColor;
            else
                labelLighthouseImg.ForeColor = Color.White;
            if (numericUpDownTower.Value.ToString().Equals(labelTowerImg.Text))
                labelTowerImg.ForeColor = m_LevelTargetColor;
            else
                labelTowerImg.ForeColor = Color.White;
            if (numericUpDownStatue.Value.ToString().Equals(labelStatueImg.Text))
                labelStatueImg.ForeColor = m_LevelTargetColor;
            else
                labelStatueImg.ForeColor = Color.White;
            if (numericUpDownOracle.Value.ToString().Equals(labelOracleImg.Text))
                labelOracleImg.ForeColor = m_LevelTargetColor;
            else
                labelOracleImg.ForeColor = Color.White;
            if (numericUpDownTrade_office.Value.ToString().Equals(labelTrade_officeImg.Text))
                labelTrade_officeImg.ForeColor = m_LevelTargetColor;
            else
                labelTrade_officeImg.ForeColor = Color.White;
        }

        public void setBuildingNames(string p_BuildingNames)
        {
            string[] l_Buildings = new string[] { "", "" };//dynamic array
            l_Buildings = p_BuildingNames.Split(';');

            for (int i = 0; i < l_Buildings.Length; i++)
            {
                if (i < flowLayoutPanelBuildings.Controls.Count / 2)
                {
                    m_Tooltip.SetToolTip(((Label)(flowLayoutPanelBuildings.Controls[(i * 2) + 1])), l_Buildings[i]);
                }
            }
        }

        public void setBuildingLevelsTarget(string p_BuildingLevelsTarget)
        {
            string[] l_Target = new string[] { "", "" };//dynamic array
            l_Target = p_BuildingLevelsTarget.Split(';');

            for (int i = 0; i < l_Target.Length; i++)
            {
                if (i < flowLayoutPanelTarget.Controls.Count)
                    ((NumericUpDown)(flowLayoutPanelTarget.Controls[i])).Value = int.Parse(l_Target[i]);
            }
        }

        public string getBuildingLevelsTarget()
        {
            string l_BuildingLevelsTarget = "";

            for (int i = 0; i < flowLayoutPanelTarget.Controls.Count; i++)
            {
                l_BuildingLevelsTarget += ((NumericUpDown)(flowLayoutPanelTarget.Controls[i])).Value.ToString() + ";";
            }

            return l_BuildingLevelsTarget;
        }

        public void setBuildingQueueBot(string p_BuildingQueue)
        {
            String[] l_Buildings = new String[] { "", "" };//dynamic array
            l_Buildings = p_BuildingQueue.Split(';');

            clearBuildingQueueBot();

            for (int i = 0; i < l_Buildings.Length; i++)
            {
                addBuildingToQueueBot(l_Buildings[i]);
            }
        }

        public void setBuildingQueueGame(string p_BuildingQueue)
        {
            String[] l_Buildings = new String[] { "", "" };//dynamic array
            l_Buildings = p_BuildingQueue.Split(';');

            clearBuildingQueueGame();

            for (int i = 0; i < l_Buildings.Length; i++)
            {
                addBuildingToQueueGame(l_Buildings[i]);
            }
        }

        private void clearBuildingQueueBot()
        {
            for (int i = 0; i < flowLayoutPanelQueueBot.Controls.Count; i++)
            {
                ((Label)(flowLayoutPanelQueueBot.Controls[i])).Image = global::GrepBuildings.Properties.Resources.emptyx20;
                ((Label)(flowLayoutPanelQueueBot.Controls[i])).Tag = "empty";
            }
            m_SizeOfQueueBot = 0;
        }

        private void clearBuildingQueueGame()
        {
            for (int i = 0; i < flowLayoutPanelQueueGame.Controls.Count; i++)
            {
                ((Label)(flowLayoutPanelQueueGame.Controls[i])).Image = global::GrepBuildings.Properties.Resources.emptyx20;
                ((Label)(flowLayoutPanelQueueGame.Controls[i])).Tag = "empty";
            }
            m_SizeOfQueueGame = 0;
        }

        public void removeItemFromQueueBotByIndex(int p_Index)
        {
            if (!((Label)(flowLayoutPanelQueueBot.Controls[p_Index - 1])).Tag.Equals("empty"))
            {
                for (int i = p_Index - 1; i < flowLayoutPanelQueueBot.Controls.Count - 1; i++)
                {
                    ((Label)(flowLayoutPanelQueueBot.Controls[i])).Image = ((Label)(flowLayoutPanelQueueBot.Controls[i + 1])).Image;
                    ((Label)(flowLayoutPanelQueueBot.Controls[i])).Tag = ((Label)(flowLayoutPanelQueueBot.Controls[i + 1])).Tag;
                }
                ((Label)(flowLayoutPanelQueueBot.Controls[flowLayoutPanelQueueBot.Controls.Count - 1])).Image = global::GrepBuildings.Properties.Resources.emptyx20;
                ((Label)(flowLayoutPanelQueueBot.Controls[flowLayoutPanelQueueBot.Controls.Count - 1])).Tag = "empty";

                m_SizeOfQueueBot -= 1;
            }
            else
            {
                //Queue is empty at this spot.
            }
        }

        private void addBuildingToQueueBot(string p_Building)
        {
            bool l_Added = false;
            if (m_SizeOfQueueBot != 42)
            {
                switch (p_Building)
                {
                    case "main"://0
                        if (getTotalBuildingLevel(0) < 25)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.mainx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "main";
                            l_Added = true;
                        }
                        break;
                    case "hide"://1
                        if (getTotalBuildingLevel(0) >= 10 && getTotalBuildingLevel(7) >= 4 && getTotalBuildingLevel(2) >= 7 && getTotalBuildingLevel(1) < 10)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.hidex20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "hide";
                            l_Added = true;
                        }
                        break;
                    case "storage"://2
                        if (getTotalBuildingLevel(2) < numericUpDownStorage.Maximum)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.storagex20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "storage";
                            l_Added = true;
                        }
                        break;
                    case "farm"://3
                        if (getTotalBuildingLevel(3) < numericUpDownFarm.Maximum)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.farmx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "farm";
                            l_Added = true;
                        }
                        break;
                    case "lumber"://4
                        if (getTotalBuildingLevel(4) < 40)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.lumberx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "lumber";
                            l_Added = true;
                        }
                        break;
                    case "stoner"://5
                        if (getTotalBuildingLevel(5) < 40)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.stonerx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "stoner";
                            l_Added = true;
                        }
                        break;
                    case "ironer"://6
                        if (getTotalBuildingLevel(4) >= 1 && getTotalBuildingLevel(6) < 40)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.ironerx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "ironer";
                            l_Added = true;
                        }
                        break;
                    case "market"://7
                        if (getTotalBuildingLevel(0) >= 3 && getTotalBuildingLevel(2) >= 5 && getTotalBuildingLevel(7) < 30)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.marketx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "market";
                            l_Added = true;
                        }
                        break;
                    case "docks"://8
                        if (getTotalBuildingLevel(0) >= 14 && getTotalBuildingLevel(4) >= 15 && getTotalBuildingLevel(6) >= 10 && getTotalBuildingLevel(8) < 30)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.docksx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "docks";
                            l_Added = true;
                        }
                        break;
                    case "barracks"://9
                        if (getTotalBuildingLevel(0) >= 2 && getTotalBuildingLevel(3) >= 3 && getTotalBuildingLevel(6) >= 1 && getTotalBuildingLevel(9) < 30)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.barracksx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "barracks";
                            l_Added = true;
                        }
                        break;
                    case "wall"://10
                        if (getTotalBuildingLevel(0) >= 5 && getTotalBuildingLevel(12) >= 3 && getTotalBuildingLevel(10) < 25)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.wallx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "wall";
                            l_Added = true;
                        }
                        break;
                    case "academy"://11
                        if (getTotalBuildingLevel(0) >= 8 && getTotalBuildingLevel(3) >= 6 && getTotalBuildingLevel(9) >= 5 && getTotalBuildingLevel(11) < numericUpDownAcademy.Maximum)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.academyx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "academy";
                            l_Added = true;
                        }
                        break;
                    case "temple"://12
                        if (getTotalBuildingLevel(5) >= 1 && getTotalBuildingLevel(12) < numericUpDownTemple.Maximum)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.templex20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "temple";
                            l_Added = true;
                        }
                        break;
                    case "theater"://13
                        if (getTotalBuildingLevel(0) >= 24 && getTotalBuildingLevel(4) >= 35 && getTotalBuildingLevel(6) >= 32 && getTotalBuildingLevel(8) >= 5 && getTotalBuildingLevel(11) >= 5 && getTotalBuildingLevel(13) < 1 && getTotalBuildingLevel(14) < 1 && getTotalBuildingLevel(15) < 1 && getTotalBuildingLevel(16) < 1)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.theaterx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "theater";
                            l_Added = true;
                        }
                        break;
                    case "thermal"://14
                        if (getTotalBuildingLevel(0) >= 24 && getTotalBuildingLevel(3) >= 35 && getTotalBuildingLevel(8) >= 5 && getTotalBuildingLevel(11) >= 5 && getTotalBuildingLevel(13) < 1 && getTotalBuildingLevel(14) < 1 && getTotalBuildingLevel(15) < 1 && getTotalBuildingLevel(16) < 1)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.thermalx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "thermal";
                            l_Added = true;
                        }
                        break;
                    case "library"://15
                        if (getTotalBuildingLevel(0) >= 24 && getTotalBuildingLevel(8) >= 5 && getTotalBuildingLevel(11) >= 20 && getTotalBuildingLevel(13) < 1 && getTotalBuildingLevel(14) < 1 && getTotalBuildingLevel(15) < 1 && getTotalBuildingLevel(16) < 1)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.libraryx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "library";
                            l_Added = true;
                        }
                        break;
                    case "lighthouse"://16
                        if (getTotalBuildingLevel(0) >= 24 && getTotalBuildingLevel(8) >= 20 && getTotalBuildingLevel(11) >= 5 && getTotalBuildingLevel(13) < 1 && getTotalBuildingLevel(14) < 1 && getTotalBuildingLevel(15) < 1 && getTotalBuildingLevel(16) < 1)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.lighthousex20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "lighthouse";
                            l_Added = true;
                        }
                        break;
                    case "tower"://17
                        if (getTotalBuildingLevel(0) >= 21 && getTotalBuildingLevel(10) >= 20 && getTotalBuildingLevel(12) >= 5 && getTotalBuildingLevel(7) >= 5 && getTotalBuildingLevel(17) < 1 && getTotalBuildingLevel(18) < 1 && getTotalBuildingLevel(19) < 1 && getTotalBuildingLevel(20) < 1)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.towerx20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "tower";
                            l_Added = true;
                        }
                        break;
                    case "statue"://18
                        if (getTotalBuildingLevel(0) >= 21 && getTotalBuildingLevel(12) >= 12 && getTotalBuildingLevel(7) >= 5 && getTotalBuildingLevel(17) < 1 && getTotalBuildingLevel(18) < 1 && getTotalBuildingLevel(19) < 1 && getTotalBuildingLevel(20) < 1)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.statuex20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "statue";
                            l_Added = true;
                        }
                        break;
                    case "oracle"://19
                        if (getTotalBuildingLevel(0) >= 21 && getTotalBuildingLevel(1) >= 10 && getTotalBuildingLevel(12) >= 5 && getTotalBuildingLevel(7) >= 5 && getTotalBuildingLevel(17) < 1 && getTotalBuildingLevel(18) < 1 && getTotalBuildingLevel(19) < 1 && getTotalBuildingLevel(20) < 1)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.oraclex20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "oracle";
                            l_Added = true;
                        }
                        break;
                    case "trade_office"://20
                        if (getTotalBuildingLevel(0) >= 21 && getTotalBuildingLevel(12) >= 5 && getTotalBuildingLevel(7) >= 15 && getTotalBuildingLevel(17) < 1 && getTotalBuildingLevel(18) < 1 && getTotalBuildingLevel(19) < 1 && getTotalBuildingLevel(20) < 1)
                        {
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Image = global::GrepBuildings.Properties.Resources.trade_officex20;
                            ((Label)(flowLayoutPanelQueueBot.Controls[m_SizeOfQueueBot])).Tag = "trade_office";
                            l_Added = true;
                        }
                        break;
                }
                if(l_Added)
                    m_SizeOfQueueBot += 1;
            }
            else
            {
                //MessageBox.Show("The building queue is full.");
            }
            
        }

        private void addBuildingToQueueGame(string p_Building)
        {
            if (m_SizeOfQueueGame != 7)
            {
                switch (p_Building)
                {
                    case "main":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.mainx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "main";
                        break;
                    case "hide":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.hidex20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "hide";
                        break;
                    case "storage":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.storagex20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "storage";
                        break;
                    case "farm":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.farmx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "farm";
                        break;
                    case "lumber":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.lumberx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "lumber";
                        break;
                    case "stoner":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.stonerx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "stoner";
                        break;
                    case "ironer":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.ironerx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "ironer";
                        break;
                    case "market":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.marketx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "market";
                        break;
                    case "docks":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.docksx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "docks";
                        break;
                    case "barracks":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.barracksx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "barracks";
                        break;
                    case "wall":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.wallx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "wall";
                        break;
                    case "academy":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.academyx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "academy";
                        break;
                    case "temple":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.templex20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "temple";
                        break;
                    case "theater":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.theaterx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "theater";
                        break;
                    case "thermal":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.thermalx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "thermal";
                        break;
                    case "library":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.libraryx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "library";
                        break;
                    case "lighthouse":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.lighthousex20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "lighthouse";
                        break;
                    case "tower":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.towerx20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "tower";
                        break;
                    case "statue":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.statuex20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "statue";
                        break;
                    case "oracle":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.oraclex20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "oracle";
                        break;
                    case "trade_office":
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Image = global::GrepBuildings.Properties.Resources.trade_officex20;
                        ((Label)(flowLayoutPanelQueueGame.Controls[m_SizeOfQueueGame])).Tag = "trade_office";
                        break;
                }
                m_SizeOfQueueGame += 1;
            }
            else
            {
                //MessageBox.Show("The building queue is full.");
            }
        }

        private int getTotalBuildingLevel(int p_Index)
        {
            int l_IngameQueueLevel = 0;// getGameQueueLevel(p_Index);
            int l_BotQueueLevel = getBotQueueLevel(p_Index);
            int l_BuildingLevel = getBuildingLevel(p_Index);
            int l_TotalBuildingLevel = l_IngameQueueLevel + l_BotQueueLevel + l_BuildingLevel;

            return l_TotalBuildingLevel;
        }

        private int getBuildingLevel(int p_Index)
        {
            int l_BuildingLevel = 0;
            if (p_Index < flowLayoutPanelBuildings.Controls.Count)
                    l_BuildingLevel = int.Parse(((Label)(flowLayoutPanelBuildings.Controls[p_Index * 2])).Text);
            return l_BuildingLevel;
        }

        private int getBotQueueLevel(int p_Index)
        {
            int l_BotQueueLevel = 0;
            string l_BuildingName = ((Label)(flowLayoutPanelBuildings.Controls[p_Index * 2])).Tag.ToString();

            for (int i = 0; i < m_SizeOfQueueBot; i++)
            {
                if(l_BuildingName.Equals(((Label)(flowLayoutPanelQueueBot.Controls[i])).Tag))
                    l_BotQueueLevel++;
            }
            return l_BotQueueLevel;
        }

        private int getGameQueueLevel(int p_Index)
        {
            int l_IngameQueueLevel = 0;
            string l_BuildingName = ((Label)(flowLayoutPanelBuildings.Controls[p_Index * 2])).Tag.ToString();

            for (int i = 0; i < m_SizeOfQueueGame; i++)
            {
                if (l_BuildingName.Equals(((Label)(flowLayoutPanelQueueGame.Controls[i])).Tag))
                    l_IngameQueueLevel++;
            }
            return l_IngameQueueLevel;
        }

//-->Outgoing events

//-->Event handlers

        private void labelQueueBot1_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(1);
        }

        private void labelQueueBot2_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(2);
        }

        private void labelQueueBot3_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(3);
        }

        private void labelQueueBot4_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(4);
        }

        private void labelQueueBot5_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(5);
        }

        private void labelQueueBot6_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(6);
        }

        private void labelQueueBot7_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(7);
        }

        private void labelQueueBot8_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(8);
        }

        private void labelQueueBot9_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(9);
        }

        private void labelQueueBot10_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(10);
        }

        private void labelQueueBot11_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(11);
        }

        private void labelQueueBot12_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(12);
        }

        private void labelQueueBot13_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(13);
        }

        private void labelQueueBot14_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(14);
        }

        private void labelQueueBot15_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(15);
        }

        private void labelQueueBot16_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(16);
        }

        private void labelQueueBot17_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(17);
        }

        private void labelQueueBot18_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(18);
        }

        private void labelQueueBot19_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(19);
        }

        private void labelQueueBot20_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(20);
        }

        private void labelQueueBot21_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(21);
        }

        private void labelQueueBot22_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(22);
        }

        private void labelQueueBot23_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(23);
        }

        private void labelQueueBot24_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(24);
        }

        private void labelQueueBot25_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(25);
        }

        private void labelQueueBot26_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(26);
        }

        private void labelQueueBot27_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(27);
        }

        private void labelQueueBot28_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(28);
        }

        private void labelQueueBot29_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(29);
        }

        private void labelQueueBot30_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(30);
        }

        private void labelQueueBot31_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(31);
        }

        private void labelQueueBot32_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(32);
        }

        private void labelQueueBot33_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(33);
        }

        private void labelQueueBot34_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(34);
        }

        private void labelQueueBot35_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(35);
        }

        private void labelQueueBot36_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(36);
        }

        private void labelQueueBot37_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(37);
        }

        private void labelQueueBot38_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(38);
        }

        private void labelQueueBot39_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(39);
        }

        private void labelQueueBot40_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(40);
        }

        private void labelQueueBot41_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(41);
        }

        private void labelQueueBot42_DoubleClick(object sender, EventArgs e)
        {
            removeItemFromQueueBotByIndex(42);
        }

        private void labelMainAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelMainImg.Tag.ToString());
        }

        private void labelHideAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelHideImg.Tag.ToString());
        }

        private void labelStorageAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelStorageImg.Tag.ToString());
        }

        private void labelFarmAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelFarmImg.Tag.ToString());
        }

        private void labelLumberAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelLumberImg.Tag.ToString());
        }

        private void labelStonerAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelStonerImg.Tag.ToString());
        }

        private void labelIronerAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelIronerImg.Tag.ToString());
        }

        private void labelMarketAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelMarketImg.Tag.ToString());
        }

        private void labelDocksAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelDocksImg.Tag.ToString());
        }

        private void labelBarracksAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelBarracksImg.Tag.ToString());
        }

        private void labelWallAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelWallImg.Tag.ToString());
        }

        private void labelAcademyAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelAcademyImg.Tag.ToString());
        }

        private void labelTempleAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelTempleImg.Tag.ToString());
        }

        private void labelTheaterAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelTheaterImg.Tag.ToString());
        }

        private void labelThermalAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelThermalImg.Tag.ToString());
        }

        private void labelLibraryAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelLibraryImg.Tag.ToString());
        }

        private void labelLighthouseAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelLighthouseImg.Tag.ToString());
        }

        private void labelTowerAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelTowerImg.Tag.ToString());
        }

        private void labelStatueAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelStatueImg.Tag.ToString());
        }

        private void labelOracleAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelOracleImg.Tag.ToString());
        }

        private void labelTrade_officeAdd_Click(object sender, EventArgs e)
        {
            addBuildingToQueueBot(labelTrade_officeImg.Tag.ToString());
        }

        private void labelMainImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("main");
        }

        private void labelHideImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("hide");
        }

        private void labelStorageImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("storage");
        }

        private void labelFarmImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("farm");
        }

        private void labelLumberImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("lumber");
        }

        private void labelStonerImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("stoner");
        }

        private void labelIronerImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("ironer");
        }

        private void labelMarketImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("market");
        }

        private void labelDocksImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("docks");
        }

        private void labelBarracksImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("barracks");
        }

        private void labelWallImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("wall");
        }

        private void labelAcademyImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("academy");
        }

        private void labelTempleImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("temple");
        }

        private void labelTheaterImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("theater");
        }

        private void labelThermalImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("thermal");
        }

        private void labelLibraryImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("library");
        }

        private void labelLighthouseImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("lighthouse");
        }

        private void labelTowerImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("tower");
        }

        private void labelStatueImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("statue");
        }

        private void labelOracleImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("oracle");
        }

        private void labelTrade_officeImg_MouseMove(object sender, MouseEventArgs e)
        {
            setZoomImageByName("trade_office");
        }

        private void labelMainImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelHideImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelStorageImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelFarmImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelLumberImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelStonerImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelIronerImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelMarketImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelDocksImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelBarracksImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelWallImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelAcademyImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelTempleImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelTheaterImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelThermalImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelLibraryImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelLighthouseImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelTowerImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelStatueImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelOracleImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }

        private void labelTrade_officeImg_MouseLeave(object sender, EventArgs e)
        {
            m_ImageZoom.Hide();
        }
    }
}
