using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class Research
    {
        //Custom classes
        private List<Technology> m_Technologies = new List<Technology>();

//-->Constructor

        public Research()
        {
            initTechnologies();
        }

//-->Attributes

//-->Methods

        private void initTechnologies()
        {
            m_Technologies.Add(new Technology("slinger"));
            m_Technologies.Add(new Technology("archer"));
            m_Technologies.Add(new Technology("hoplite"));
            m_Technologies.Add(new Technology("town_guard"));
            m_Technologies.Add(new Technology("diplomacy"));
            m_Technologies.Add(new Technology("espionage"));
            m_Technologies.Add(new Technology("booty"));
            m_Technologies.Add(new Technology("pottery"));
            m_Technologies.Add(new Technology("rider"));
            m_Technologies.Add(new Technology("architecture"));
            m_Technologies.Add(new Technology("instructor"));
            m_Technologies.Add(new Technology("bireme"));
            m_Technologies.Add(new Technology("building_crane"));
            m_Technologies.Add(new Technology("meteorology"));
            m_Technologies.Add(new Technology("chariot"));
            m_Technologies.Add(new Technology("attack_ship"));
            m_Technologies.Add(new Technology("conscription"));
            m_Technologies.Add(new Technology("shipwright"));
            m_Technologies.Add(new Technology("demolition_ship"));
            m_Technologies.Add(new Technology("catapult"));
            m_Technologies.Add(new Technology("cryptography"));
            m_Technologies.Add(new Technology("colonize_ship"));
            m_Technologies.Add(new Technology("small_transporter"));
            m_Technologies.Add(new Technology("plow"));
            m_Technologies.Add(new Technology("trireme"));
            m_Technologies.Add(new Technology("phalanx"));
            m_Technologies.Add(new Technology("mathematics"));
            m_Technologies.Add(new Technology("ram"));
            m_Technologies.Add(new Technology("cartography"));
            m_Technologies.Add(new Technology("take_over"));
        }

        public bool isResearched(string p_Id)
        {
            bool l_IsResearched = false;
            for (int i = 0; i < m_Technologies.Count; i++)
            {
                if (m_Technologies[i].Id.Equals(p_Id) && m_Technologies[i].Researched)
                {
                    l_IsResearched = true;
                    break;
                }
            }
            return l_IsResearched;
        }

        public void setResearchStatus(string p_Id, bool p_Researched)
        {
            for (int i = 0; i < m_Technologies.Count; i++)
            {
                if (m_Technologies[i].Id.Equals(p_Id))
                {
                    m_Technologies[i].Researched = p_Researched;
                    break;
                }
            }
        }
    }
}
