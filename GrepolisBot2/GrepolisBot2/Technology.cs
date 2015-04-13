using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class Technology
    {
        string m_Id = "";
        bool m_Researched = false;

//-->Constructor

        public Technology(string p_Id)
        {
            m_Id = p_Id;
        }

//-->Attributes

        public string Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public bool Researched
        {
            get { return m_Researched; }
            set { m_Researched = value; }
        }
    }
}
