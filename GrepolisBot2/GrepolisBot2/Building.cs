using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class Building
    {
        private string m_LocalName = "";
        private string m_DevName = "";
        private int m_Level = 0;//Includes the buildings in the ingame queue
        private int m_NextLevel = 0;
        private int m_TearDownLevel = 0;
        private int m_MaxLevel = 0;
        private bool m_IsMaxLevel = false;
        private int m_MaxLevelHero = 0;
        private int m_TargetLevel = 0;
        private bool m_Upgradable = false;
        private bool m_Teardownable = false;
        private double m_PopBase = 0;
        private double m_PopFactor = 0;

//-->Constructors

        public Building()
        {

        }

        public Building(string p_DevName, int p_MaxLevel, int p_MaxLevelHero, double p_PopBase, double p_PopFactor)
        {
            m_DevName = p_DevName;
            m_MaxLevel = p_MaxLevel;
            m_MaxLevelHero = p_MaxLevelHero;
            m_PopBase = p_PopBase;
            m_PopFactor = p_PopFactor;
        }

//-->Attributes

        public string LocalName
        {
            get 
            {
                if (m_LocalName.Length > 0)
                    return m_LocalName;
                else
                    return m_DevName;
            }
            set { m_LocalName = value; }
        }

        public string DevName
        {
            get { return m_DevName; }
            set { m_DevName = value; }
        }

        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public int NextLevel
        {
            get { return m_NextLevel; }
            set { m_NextLevel = value; }
        }

        public int TearDownLevel
        {
            get { return m_TearDownLevel; }
            set { m_TearDownLevel = value; }
        }

        public int MaxLevel
        {
            get { return m_MaxLevel; }
            set { m_MaxLevel = value; }
        }

        public bool IsMaxLevel
        {
            get { return m_IsMaxLevel; }
            set { m_IsMaxLevel = value; }
        }

        public int MaxLevelHero
        {
            get { return m_MaxLevelHero; }
            set { m_MaxLevelHero = value; }
        }

        public int TargetLevel
        {
            get { return m_TargetLevel; }
            set { m_TargetLevel = value; }
        }

        public bool Upgradable
        {
            get { return m_Upgradable; }
            set { m_Upgradable = value; }
        }

        public bool Teardownable
        {
            get { return m_Teardownable; }
            set { m_Teardownable = value; }
        }

        public double PopBase
        {
            get { return m_PopBase; }
            set { m_PopBase = value; }
        }

        public double PopFactor
        {
            get { return m_PopFactor; }
            set { m_PopFactor = value; }
        }

//-->Methods

        public override string ToString()
        {
            return m_LocalName + " " + m_Level.ToString();
        }

        public int getPopulationAtTargetLevel()
        {
            double l_PopExact = m_PopBase * Math.Pow(m_TargetLevel, m_PopFactor);
            int l_Pop = (int)(l_PopExact + 0.5);
            return l_Pop;
        }

        public int getMaximumLevel(bool p_IsHeroMode)
        {
            int l_Level = 0;

            if (p_IsHeroMode)
            {
                l_Level = m_MaxLevelHero;
            }
            else
            {
                l_Level = m_MaxLevel;
            }

            return l_Level;
        }
    }
}
