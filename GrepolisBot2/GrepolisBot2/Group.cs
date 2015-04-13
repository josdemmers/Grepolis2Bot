using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class Group
    {
        private string m_ID = "";
        private string m_Name = "";
        private bool m_IsAdministrator = false;
        private List<string> m_Towns = new List<string>();

//-->Constructor

        public Group()
        {

        }

        public Group(string p_ID, string p_Name, bool p_IsAdministrator)
        {
            m_ID = p_ID;
            m_Name = p_Name;
            m_IsAdministrator = p_IsAdministrator;
        }

//-->Attributes

        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public List<string> Towns
        {
            get { return m_Towns; }
            set { m_Towns = value; }
        }

//-->Methods


    }
}
