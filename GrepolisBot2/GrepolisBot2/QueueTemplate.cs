using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class QueueTemplate : IComparable<QueueTemplate>
    {
        private string m_Name = "";
        private string m_Queue = "";

        private List<ArmyUnit> m_ArmyUnits = new List<ArmyUnit>();
        private List<Building> m_Buildings = new List<Building>();

//-->Constructor

        public QueueTemplate()
        {

        }
        
        public QueueTemplate(string p_Name)
        {
            m_Name = p_Name;
            addBuildings();
            addArmyUnits();
        }

        public QueueTemplate(string p_Name, string p_Queue)
        {
            m_Name = p_Name;
            m_Queue = p_Queue;
            addBuildings();
            addArmyUnits();
        }

//-->Attributes

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public string Queue
        {
            get { return m_Queue; }
            set { m_Queue = value; }
        }

//-->Methods

        private void addBuildings()
        {
            m_Buildings.Add(new Building("main", 25, 25, 1, 1.5));
            m_Buildings.Add(new Building("hide", 10, 10, 3, 0.5));
            m_Buildings.Add(new Building("storage", 30, 35, 0, 1));
            m_Buildings.Add(new Building("farm", 40, 45, 0, 0));
            m_Buildings.Add(new Building("lumber", 40, 40, 1, 1.25));
            m_Buildings.Add(new Building("stoner", 40, 40, 1, 1.25));
            m_Buildings.Add(new Building("ironer", 40, 40, 1, 1.25));
            m_Buildings.Add(new Building("market", 30, 40, 2, 1.1));
            m_Buildings.Add(new Building("docks", 30, 30, 4, 1));
            m_Buildings.Add(new Building("barracks", 30, 30, 1, 1.3));
            m_Buildings.Add(new Building("wall", 25, 25, 2, 1.16));
            m_Buildings.Add(new Building("academy", 30, 36, 3, 1));
            m_Buildings.Add(new Building("temple", 25, 30, 5, 1));
            m_Buildings.Add(new Building("theater", 1, 1, 60, 1));
            m_Buildings.Add(new Building("thermal", 1, 1, 60, 1));
            m_Buildings.Add(new Building("library", 1, 1, 60, 1));
            m_Buildings.Add(new Building("lighthouse", 1, 1, 60, 1));
            m_Buildings.Add(new Building("tower", 1, 1, 60, 1));
            m_Buildings.Add(new Building("statue", 1, 1, 60, 1));
            m_Buildings.Add(new Building("oracle", 1, 1, 60, 1));
            m_Buildings.Add(new Building("trade_office", 1, 1, 60, 1));
        }

        private void addArmyUnits()
        {
            m_ArmyUnits.Add(new ArmyUnit("sword", 1, 0, "none", true, true, 95, 0, 85, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("slinger", 1, 0, "none", true, false, 55, 100, 40, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("archer", 1, 0, "none", true, false, 120, 0, 75, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("hoplite", 1, 0, "none", true, false, 0, 75, 150, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("rider", 3, 0, "none", true, false, 240, 120, 360, 0, 0, 10, 0));
            m_ArmyUnits.Add(new ArmyUnit("chariot", 4, 0, "none", true, false, 200, 440, 320, 0, 0, 15, 0));
            m_ArmyUnits.Add(new ArmyUnit("catapult", 15, 0, "none", true, false, 1200, 1200, 1200, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("minotaur", 30, 0, "zeus", true, true, 1400, 600, 3100, 202, 10, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("manticore", 45, 0, "zeus", true, true, 4400, 3000, 3400, 405, 15, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("centaur", 12, 0, "athena", true, true, 1740, 300, 700, 100, 4, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("pegasus", 20, 0, "athena", true, true, 2800, 360, 80, 180, 12, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("harpy", 14, 0, "hera", true, true, 1600, 400, 1360, 130, 5, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("medusa", 18, 0, "hera", true, true, 1500, 3800, 2200, 210, 10, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("zyklop", 40, 0, "poseidon", true, true, 2000, 4200, 3360, 360, 12, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("cerberus", 30, 0, "hades", true, true, 1250, 1500, 3000, 320, 10, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("fury", 55, 0, "hades", true, true, 2500, 5000, 5000, 480, 16, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("griffin", 38, 0, "artemis", true, true, 3800, 1900, 4800, 250, 15, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("calydonian_boar", 20, 0, "artemis", true, true, 2800, 1400, 1600, 110, 10, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("godsent", 3, 0, "none", true, true, 0, 0, 0, 15, 1, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("big_transporter", 7, 20, "none", false, true, 500, 500, 400, 0, 0, 0, 1));//19
            m_ArmyUnits.Add(new ArmyUnit("bireme", 8, 0, "none", false, false, 800, 700, 180, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("attack_ship", 10, 0, "none", false, false, 1300, 300, 800, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("demolition_ship", 8, 0, "none", false, false, 500, 750, 150, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("small_transporter", 5, 10, "none", false, false, 800, 0, 400, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("trireme", 16, 0, "none", false, false, 2000, 1300, 900, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("colonize_ship", 170, 0, "none", false, false, 10000, 10000, 10000, 0, 0, 0, 20));
            m_ArmyUnits.Add(new ArmyUnit("sea_monster", 50, 0, "poseidon", false, true, 5400, 2800, 3800, 400, 22, 0, 1));
        }

        public override string ToString()
        {
            return m_Name;
        }

        public int CompareTo(QueueTemplate p_Obj)
        {
            return m_Name.CompareTo(p_Obj.ToString());
        }

        public int getUnitTargetAmount(string p_Name)
        {
            int l_Target = 0;

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (m_ArmyUnits[i].Name.Equals(p_Name))
                    l_Target = m_ArmyUnits[i].QueueBot;
            }

            return l_Target;
        }

        public void setUnitTargetAmount(string p_Name, int p_Amount)
        {
            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (m_ArmyUnits[i].Name.Equals(p_Name))
                    m_ArmyUnits[i].QueueBot = p_Amount;
            }
        }
    }
}
