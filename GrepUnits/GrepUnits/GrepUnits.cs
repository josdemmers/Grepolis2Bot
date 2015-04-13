using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrepUnits
{
    public partial class GrepUnits : UserControl
    {
        private string m_Index = "0";
        private string m_TownID = "0";
        private string m_TownName = "No town selected";
        private ToolTip m_Tooltip = new ToolTip();

//-->Constructor

        public GrepUnits()
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

        public void setUnitNames(string p_UnitNames)
        {
            string[] l_Units = new string[] { "", "" };//dynamic array
            l_Units = p_UnitNames.Split(';');

            for (int i = 0; i < l_Units.Length; i++)
            {
                if (i < flowLayoutPanelUnitsImg.Controls.Count)
                {
                    m_Tooltip.SetToolTip(((Label)(flowLayoutPanelUnitsImg.Controls[i])), l_Units[i]);
                }
            }
        }


        /*public string getUnitQueue()
        {
            string l_Queue = "";
            for (int i = 0; i < flowLayoutPanelUnitsTarget.Controls.Count; i++)
            {
                l_Queue += ((NumericUpDown)(flowLayoutPanelUnitsTarget.Controls[i])).Value.ToString() + ";";
            }
            return l_Queue;
        }*/
        public int getUnitQueue(string p_Name)
        {
            int l_Queue = 0;

            switch (p_Name)
            {
                case "sword":
                    l_Queue = (int)numericUpDownSword.Value;
                    break;
                case "slinger":
                    l_Queue = (int)numericUpDownSlinger.Value;
                    break;
                case "archer":
                    l_Queue = (int)numericUpDownArcher.Value;
                    break;
                case "hoplite":
                    l_Queue = (int)numericUpDownHoplite.Value;
                    break;
                case "rider":
                    l_Queue = (int)numericUpDownRider.Value;
                    break;
                case "chariot":
                    l_Queue = (int)numericUpDownChariot.Value;
                    break;
                case "catapult":
                    l_Queue = (int)numericUpDownCatapult.Value;
                    break;
                case "minotaur":
                    l_Queue = (int)numericUpDownMinotaur.Value;
                    break;
                case "manticore":
                    l_Queue = (int)numericUpDownManticore.Value;
                    break;
                case "centaur":
                    l_Queue = (int)numericUpDownCentaur.Value;
                    break;
                case "pegasus":
                    l_Queue = (int)numericUpDownPegasus.Value;
                    break;
                case "harpy":
                    l_Queue = (int)numericUpDownHarpy.Value;
                    break;
                case "medusa":
                    l_Queue = (int)numericUpDownMedusa.Value;
                    break;
                case "zyklop":
                    l_Queue = (int)numericUpDownZyklop.Value;
                    break;
                case "cerberus":
                    l_Queue = (int)numericUpDownCerberus.Value;
                    break;
                case "fury":
                    l_Queue = (int)numericUpDownFury.Value;
                    break;
                case "griffin":
                    l_Queue = (int)numericUpDownGriffin.Value;
                    break;
                case "calydonian_boar":
                    l_Queue = (int)numericUpDownCalydonianBoar.Value;
                    break;
                case "godsent":
                    l_Queue = (int)numericUpDownGodsent.Value;
                    break;
                case "big_transporter":
                    l_Queue = (int)numericUpDownBig_transporter.Value;
                    break;
                case "bireme":
                    l_Queue = (int)numericUpDownBireme.Value;
                    break;
                case "attack_ship":
                    l_Queue = (int)numericUpDownAttack_ship.Value;
                    break;
                case "demolition_ship":
                    l_Queue = (int)numericUpDownDemolition_ship.Value;
                    break;
                case "small_transporter":
                    l_Queue = (int)numericUpDownSmall_transporter.Value;
                    break;
                case "trireme":
                    l_Queue = (int)numericUpDownTrireme.Value;
                    break;
                case "colonize_ship":
                    l_Queue = (int)numericUpDownColonize_ship.Value;
                    break;
                case "sea_monster":
                    l_Queue = (int)numericUpDownSea_monster.Value;
                    break;
            }

            return l_Queue;
        }

        /*public void setUnitQueue(string p_Queue)
        {
            string[] l_Queue = new string[] { "", "" };//dynamic array
            l_Queue = p_Queue.Split(';');

            for (int i = 0; i < flowLayoutPanelUnitsTarget.Controls.Count; i++)
            {
                ((NumericUpDown)(flowLayoutPanelUnitsTarget.Controls[i])).Value = decimal.Parse(l_Queue[i]);
            }
        }*/

        public void setUnitQueue(string p_Name, int p_Queue)
        {
            switch (p_Name)
            {
                case "sword":
                    numericUpDownSword.Value = p_Queue;
                    break;
                case "slinger":
                    numericUpDownSlinger.Value = p_Queue;
                    break;
                case "archer":
                    numericUpDownArcher.Value = p_Queue;
                    break;
                case "hoplite":
                    numericUpDownHoplite.Value = p_Queue;
                    break;
                case "rider":
                    numericUpDownRider.Value = p_Queue;
                    break;
                case "chariot":
                    numericUpDownChariot.Value = p_Queue;
                    break;
                case "catapult":
                    numericUpDownCatapult.Value = p_Queue;
                    break;
                case "minotaur":
                    numericUpDownMinotaur.Value = p_Queue;
                    break;
                case "manticore":
                    numericUpDownManticore.Value = p_Queue;
                    break;
                case "centaur":
                    numericUpDownCentaur.Value = p_Queue;
                    break;
                case "pegasus":
                    numericUpDownPegasus.Value = p_Queue;
                    break;
                case "harpy":
                    numericUpDownHarpy.Value = p_Queue;
                    break;
                case "medusa":
                    numericUpDownMedusa.Value = p_Queue;
                    break;
                case "zyklop":
                    numericUpDownZyklop.Value = p_Queue;
                    break;
                case "cerberus":
                    numericUpDownCerberus.Value = p_Queue;
                    break;
                case "fury":
                    numericUpDownFury.Value = p_Queue;
                    break;
                case "griffin":
                    numericUpDownGriffin.Value = p_Queue;
                    break;
                case "calydonian_boar":
                    numericUpDownCalydonianBoar.Value = p_Queue;
                    break;
                case "godsent":
                    numericUpDownGodsent.Value = p_Queue;
                    break;
                case "big_transporter":
                    numericUpDownBig_transporter.Value = p_Queue;
                    break;
                case "bireme":
                    numericUpDownBireme.Value = p_Queue;
                    break;
                case "attack_ship":
                    numericUpDownAttack_ship.Value = p_Queue;
                    break;
                case "demolition_ship":
                    numericUpDownDemolition_ship.Value = p_Queue;
                    break;
                case "small_transporter":
                    numericUpDownSmall_transporter.Value = p_Queue;
                    break;
                case "trireme":
                    numericUpDownTrireme.Value = p_Queue;
                    break;
                case "colonize_ship":
                    numericUpDownColonize_ship.Value = p_Queue;
                    break;
                case "sea_monster":
                    numericUpDownSea_monster.Value = p_Queue;
                    break;
            }
        }

        public void setUnitGameQueue(string p_Name, int p_Queue)
        {
            string l_Queue = "";
            if (p_Queue > 0)
                l_Queue = p_Queue.ToString();

            switch (p_Name)
            {
                case "sword":
                    labelSwordImg.Text = l_Queue;
                    break;
                case "slinger":
                    labelSlingerImg.Text = l_Queue;
                    break;
                case "archer":
                    labelArcherImg.Text = l_Queue;
                    break;
                case "hoplite":
                    labelHopliteImg.Text = l_Queue;
                    break;
                case "rider":
                    labelRiderImg.Text = l_Queue;
                    break;
                case "chariot":
                    labelChariotImg.Text = l_Queue;
                    break;
                case "catapult":
                    labelCatapultImg.Text = l_Queue;
                    break;
                case "minotaur":
                    labelMinotaurImg.Text = l_Queue;
                    break;
                case "manticore":
                    labelManticoreImg.Text = l_Queue;
                    break;
                case "centaur":
                    labelCentaurImg.Text = l_Queue;
                    break;
                case "pegasus":
                    labelPegasusImg.Text = l_Queue;
                    break;
                case "harpy":
                    labelHarpyImg.Text = l_Queue;
                    break;
                case "medusa":
                    labelMedusaImg.Text = l_Queue;
                    break;
                case "zyklop":
                    labelZyklopImg.Text = l_Queue;
                    break;
                case "cerberus":
                    labelCerberusImg.Text = l_Queue;
                    break;
                case "fury":
                    labelFuryImg.Text = l_Queue;
                    break;
                case "griffin":
                    labelGriffinImg.Text = l_Queue;
                    break;
                case "calydonian_boar":
                    labelCalydonianBoarImg.Text = l_Queue;
                    break;
                case "godsent":
                    labelGodsentImg.Text = l_Queue;
                    break;
                case "big_transporter":
                    labelBig_transporterImg.Text = l_Queue;
                    break;
                case "bireme":
                    labelBiremeImg.Text = l_Queue;
                    break;
                case "attack_ship":
                    labelAttack_shipImg.Text = l_Queue;
                    break;
                case "demolition_ship":
                    labelDemolition_shipImg.Text = l_Queue;
                    break;
                case "small_transporter":
                    labelSmall_transporterImg.Text = l_Queue;
                    break;
                case "trireme":
                    labelTriremeImg.Text = l_Queue;
                    break;
                case "colonize_ship":
                    labelColonize_shipImg.Text = l_Queue;
                    break;
                case "sea_monster":
                    labelSea_monsterImg.Text = l_Queue;
                    break;
            }
        }

        /*public void setTotalUnits(string p_Units)
        {
            string[] l_Units = new string[] { "", "" };//dynamic array
            l_Units = p_Units.Split(';');

            for (int i = 0; i < flowLayoutPanelUnitsTotal.Controls.Count; i++)
            {
                ((Label)(flowLayoutPanelUnitsTotal.Controls[i])).Text = l_Units[i];
            }
        }*/

        public void setTotalUnits(string p_Name, string p_Total)
        {
            switch (p_Name)
            {
                case "sword":
                    labelCurrentSwordAmount.Text = p_Total;
                    break;
                case "slinger":
                    labelCurrentSlingerAmount.Text = p_Total;
                    break;
                case "archer":
                    labelCurrentArcherAmount.Text = p_Total;
                    break;
                case "hoplite":
                    labelCurrentHopliteAmount.Text = p_Total;
                    break;
                case "rider":
                    labelCurrentRiderAmount.Text = p_Total;
                    break;
                case "chariot":
                    labelCurrentChariotAmount.Text = p_Total;
                    break;
                case "catapult":
                    labelCurrentCatapultAmount.Text = p_Total;
                    break;
                case "minotaur":
                    labelCurrentMinotaurAmount.Text = p_Total;
                    break;
                case "manticore":
                    labelCurrentManticoreAmount.Text = p_Total;
                    break;
                case "centaur":
                    labelCurrentCentaurAmount.Text = p_Total;
                    break;
                case "pegasus":
                    labelCurrentPegasusAmount.Text = p_Total;
                    break;
                case "harpy":
                    labelCurrentHarpyAmount.Text = p_Total;
                    break;
                case "medusa":
                    labelCurrentMedusaAmount.Text = p_Total;
                    break;
                case "zyklop":
                    labelCurrentZyklopAmount.Text = p_Total;
                    break;
                case "cerberus":
                    labelCurrentCerberusAmount.Text = p_Total;
                    break;
                case "fury":
                    labelCurrentFuryAmount.Text = p_Total;
                    break;
                case "griffin":
                    labelCurrentGriffinAmount.Text = p_Total;
                    break;
                case "calydonian_boar":
                    labelCurrentCalydonianBoarAmount.Text = p_Total;
                    break;
                case "godsent":
                    labelCurrentGodsentAmount.Text = p_Total;
                    break;
                case "big_transporter":
                    labelCurrentBig_transporterAmount.Text = p_Total;
                    break;
                case "bireme":
                    labelCurrentBiremeAmount.Text = p_Total;
                    break;
                case "attack_ship":
                    labelCurrentAttack_shipAmount.Text = p_Total;
                    break;
                case "demolition_ship":
                    labelCurrentDemolition_shipAmount.Text = p_Total;
                    break;
                case "small_transporter":
                    labelCurrentSmall_transporterAmount.Text = p_Total;
                    break;
                case "trireme":
                    labelCurrentTriremeAmount.Text = p_Total;
                    break;
                case "colonize_ship":
                    labelCurrentColonize_shipAmount.Text = p_Total;
                    break;
                case "sea_monster":
                    labelCurrentSea_monsterAmount.Text = p_Total;
                    break;
            }
        }
    }
}
