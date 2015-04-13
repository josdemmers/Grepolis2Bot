using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class CulturalEvent
    {
        private string m_Name = "";
        private string m_NameLocal = "";
        private bool m_Ready = false;//Checks if already started and if building requirements are OK
        private bool m_EnoughResources = false;

//-->Constructor
        public CulturalEvent(string p_Name)
        {
            m_Name = p_Name;
        }

//-->Attributes
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public string NameLocal
        {
            get { return m_NameLocal; }
            set { m_NameLocal = value; }
        }

        public bool Ready
        {
            get { return m_Ready; }
            set { m_Ready = value; }
        }

        public bool EnoughResources
        {
            get { return m_EnoughResources; }
            set { m_EnoughResources = value; }
        }

//-->Methods

    }
}
