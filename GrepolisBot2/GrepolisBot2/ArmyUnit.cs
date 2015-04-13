using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class ArmyUnit
    {
        private string m_Name = "";
        private string m_LocalName = "";
        private int m_Population = 0;
        private int m_Capacity = 0;
        private string m_God = "none";
        private bool m_IsFromBarracks = true;
        private int m_CurrentAmount = 0;//Currently in town
        private int m_TotalAmount = 0;//Total units (Includes those on a mission)
        private int m_MaxBuild = 0;//How many you can build with the current resources
        private int m_QueueBot = 0;//Target training amount
        private int m_QueueGame = 0;//Current amount that is being trained ingame
        private bool m_IsResearched = false;
        private int m_Wood = 0;
        private int m_Stone = 0;
        private int m_Iron = 0;
        private int m_Favor = 0;
        private int m_TempleLvlReq = 0;
        private int m_BarracksLvlReq = 0;
        private int m_DocksLvlReq = 0;

//-->Constructors

        public ArmyUnit()
        {

        }

        public ArmyUnit(string p_Name, int p_Population, int p_Capacity, string p_God, bool p_IsFromBarracks,
            bool p_IsResearched, int p_Wood, int p_Stone, int p_Iron, int p_Favor, int p_TempleLvlReq, int p_BarracksLvlReq, int p_DocksLvlReq)
        {
            m_Name = p_Name;
            m_Population = p_Population;
            m_Capacity = p_Capacity;
            m_God = p_God;
            m_IsFromBarracks = p_IsFromBarracks;
            m_IsResearched = p_IsResearched;
            m_Wood = p_Wood;
            m_Stone = p_Stone;
            m_Iron = p_Iron;
            m_Favor = p_Favor;
            m_TempleLvlReq = p_TempleLvlReq;
            m_BarracksLvlReq = p_BarracksLvlReq;
            m_DocksLvlReq = p_DocksLvlReq;
        }

//-->Attributes

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public string LocalName
        {
            get
            {
                //Forces LocalName to return dev name of unit when translation is not available.
                if (m_LocalName.Length > 0)
                    return m_LocalName;
                else
                    return m_Name;
            }
            set { m_LocalName = value; }
        }

        public int Population
        {
            get { return m_Population; }
            set { m_Population = value; }
        }

        public int Capacity
        {
            get { return m_Capacity; }
            set { m_Capacity = value; }
        }

        public string God
        {
            get { return m_God; }
            set { m_God = value; }
        }

        public bool IsFromBarracks
        {
            get { return m_IsFromBarracks; }
            set { m_IsFromBarracks = value; }
        }

        public int CurrentAmount
        {
            get { return m_CurrentAmount; }
            set { m_CurrentAmount = value; }
        }

        public int TotalAmount
        {
            get { return m_TotalAmount; }
            set { m_TotalAmount = value; }
        }

        public int QueueBot
        {
            get { return m_QueueBot; }
            set { m_QueueBot = value; }
        }

        public int QueueGame
        {
            get { return m_QueueGame; }
            set { m_QueueGame = value; }
        }

        public int MaxBuild
        {
            get { return m_MaxBuild; }
            set { m_MaxBuild = value; }
        }

        public bool IsResearched
        {
            get { return m_IsResearched; }
            set { m_IsResearched = value; }
        }

        public int Wood
        {
            get { return m_Wood; }
            set { m_Wood = value; }
        }

        public int Stone
        {
            get { return m_Stone; }
            set { m_Stone = value; }
        }

        public int Iron
        {
            get { return m_Iron; }
            set { m_Iron = value; }
        }

        public int Favor
        {
            get { return m_Favor; }
            set { m_Favor = value; }
        }

        public int TempleLvlReq
        {
            get { return m_TempleLvlReq; }
            set { m_TempleLvlReq = value; }
        }

        public int BarracksLvlReq
        {
            get { return m_BarracksLvlReq; }
            set { m_BarracksLvlReq = value; }
        }

        public int DocksLvlReq
        {
            get { return m_DocksLvlReq; }
            set { m_DocksLvlReq = value; }
        }
    }
}
