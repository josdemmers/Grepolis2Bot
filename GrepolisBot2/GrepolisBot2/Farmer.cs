using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class Farmer
    {
        private string m_ID = "";
        private string m_Name = "";
        private int m_ExpansionState = 0;
        private int m_Mood = 0;
        private string m_IslandX = "000";
        private string m_IslandY = "000";
        private bool m_RelationStatus = false;
        private string m_LootTimer = "";//When you are able to loot again displayed in seconds.
        private string m_LootTimerHuman = "";//Shows in understandable words when you're able to loot again.
        private string m_Limit = "0/0";
        private string m_Looted = "0";//When m_Limit ends with /0 replace m_Limit with m_Looted.
        private bool m_FarmersLimitReached = false;
        private bool m_Enabled = false;

//-->Constructors

        public Farmer()
        {

        }

        public Farmer(string p_ID, string p_Name, string p_ExpansionState, string p_IslandX, string p_IslandY, string p_Mood, string p_RelationStatus, string p_LootTimer, string p_LootTimerHuman)
        {
            m_ID = p_ID;
            m_Name = p_Name;
            m_ExpansionState = int.Parse(p_ExpansionState);
            m_IslandX = p_IslandX;
            m_IslandY = p_IslandY;
            m_Mood = int.Parse(p_Mood);
            m_RelationStatus = p_RelationStatus.Equals("1");
            m_LootTimer = p_LootTimer;
            m_LootTimerHuman = p_LootTimerHuman;
        }

//-->Attributes

        public String ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public int ExpansionState
        {
            get { return m_ExpansionState; }
            set { m_ExpansionState = value; }
        }

        public int Mood
        {
            get { return m_Mood; }
            set { m_Mood = value; }
        }

        public string IslandX
        {
            get { return m_IslandX; }
            set { m_IslandX = value; }
        }

        public string IslandY
        {
            get { return m_IslandY; }
            set { m_IslandY = value; }
        }

        public bool RelationStatus
        {
            get { return m_RelationStatus; }
            set { m_RelationStatus = value; }
        }

        public string LootTimer
        {
            get { return m_LootTimer; }
            set { m_LootTimer = value; }
        }

        public string LootTimerHuman
        {
            get { return m_LootTimerHuman; }
            set { m_LootTimerHuman = value; }
        }

        public string Limit
        {
            get
            {
                if (m_Limit.EndsWith("/0"))
                    return m_Looted;
                else
                    return m_Limit;
            }
            set { m_Limit = value; }
        }

        public string Looted
        {
            get { return m_Looted; }
            set { m_Looted = value; }
        }

        public bool FarmersLimitReached
        {
            get { return m_FarmersLimitReached; }
            set { m_FarmersLimitReached = value; }
        }

        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

//-->Methods

        public bool isLootable(string p_ServerTime)
        {
            Settings l_Settings = Settings.Instance;

            bool l_Lootable = false;
            long l_TimeRemaining = 1;

            l_TimeRemaining = long.Parse(m_LootTimer) - long.Parse(p_ServerTime);

            //Added an extra 60 seconds to handle server lag
            if (m_RelationStatus && l_TimeRemaining <= (0-l_Settings.AdvFarmerLootLag) && long.Parse(m_LootTimer) != 0)
                l_Lootable = true;

            return l_Lootable;
        }
    }
}
