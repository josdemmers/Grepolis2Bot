using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class Movement
    {
        //General
        private string m_Type = "";
        private bool m_Cancelable = false;
        private string m_Started_at = "";
        private string m_Arrival_at = "";
        private string m_Arrival_eta = "";
        //private string m_Arrival_seconds_left = "";
        private string m_Arrived_human = "";
        private string m_Id = "";
        private bool m_Incoming = false;
        private bool m_Incoming_attack = false;//Only when l_MovIncoming is true
        private string m_Command_name = "";
        //Origin movement
        private string m_OriginTownID = "";
        private string m_OriginTownName = "";

//-->Constructors

        public Movement()
        {

        }

        public Movement(string p_Type, bool p_Cancelable, string p_Started_at, string p_Arrival_at, string p_Arrival_eta, string p_Arrived_human, string p_Id, bool p_Incoming, bool p_Incoming_attack, string p_Command_name)
        {
            m_Type = p_Type;
            m_Cancelable = p_Cancelable;
            m_Started_at = p_Started_at;
            m_Arrival_at = p_Arrival_at;
            m_Arrival_eta = p_Arrival_eta;
            //m_Arrival_seconds_left = p_Arrival_seconds_left;
            m_Arrived_human = p_Arrived_human;
            m_Id = p_Id;
            m_Incoming = p_Incoming;
            m_Incoming_attack = p_Incoming_attack;
            m_Command_name = p_Command_name;
        }

//-->Attributes

        //General
        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public bool Cancelable
        {
            get { return m_Cancelable; }
            set { m_Cancelable = value; }
        }

        public string Started_at
        {
            get { return m_Started_at; }
            set { m_Started_at = value; }
        }

        public string Arrival_at
        {
            get { return m_Arrival_at; }
            set { m_Arrival_at = value; }
        }

        public string Arrival_eta
        {
            get { return m_Arrival_eta; }
            set { m_Arrival_eta = value; }
        }

        /*public string Arrival_seconds_left
        {
            get { return m_Arrival_seconds_left; }
            set { m_Arrival_seconds_left = value; }
        }*/

        public string Arrived_human
        {
            get { return m_Arrived_human; }
            set { m_Arrived_human = value; }
        }

        public string Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public bool Incoming
        {
            get { return m_Incoming; }
            set { m_Incoming = value; }
        }

        public bool Incoming_attack
        {
            get { return m_Incoming_attack; }
            set { m_Incoming_attack = value; }
        }

        public string Command_name
        {
            get { return m_Command_name; }
            set { m_Command_name = value; }
        }

        //Origin movement
        public string OriginTownID
        {
            get { return m_OriginTownID; }
            set { m_OriginTownID = value; }
        }

        public string OriginTownName
        {
            get { return m_OriginTownName; }
            set { m_OriginTownName = value; }
        }

//-->Methods

        public void addTOIInfo(string p_Id, string p_Name)
        {
            Parser l_Parser = Parser.Instance;

            m_OriginTownID = p_Id;
            m_OriginTownName = p_Name;
        }
    }
}