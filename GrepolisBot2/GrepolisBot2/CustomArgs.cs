using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class CustomArgs : System.EventArgs
    {
        private string m_Message;

        //-->Constructor

        public CustomArgs(string p_Message)
        {
            m_Message = p_Message;
        }

        //-->Methods

        public string getMessage()
        {
            return m_Message;
        }
    }
}
