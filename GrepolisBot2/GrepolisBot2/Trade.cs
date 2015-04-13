using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class Trade
    {
        private string m_Origin_town_id = "";
        //private string m_origin_town_name = "";
        //private string m_origin_ix = "";
        //private string m_origin_iy = "";
        //private string m_origin_town_player_id = "";
        //private string m_origin_farm_town_name = "";
        private string m_Destination_town_id = "";
        //private string m_destination_town_name = "";
        //private string m_destination_ix = "";
        //private string m_destination_iy = "";
        //private string m_destination_town_player_id = "";
        //private string m_destination_farm_town_name = "";
        //private string m_wonder_type = "";
        //private string m_wonder_ix = "";
        //private string m_wonder_iy = "";
        private string m_Id = "";
        private string m_Wood = "0";
        private string m_Stone = "0";
        private string m_Iron = "0";
        //private string m_started_at = "";
        //private string m_arrival_at = "";
        //private string m_origin_town_type = "";
        //private string m_destination_town_type = "";
        private string m_In_exchange = "";
        //private string m_arrival_seconds_left = "";
        //private string m_cancelable = "";
        //private string m_origin_town_link = "";
        //private string m_destination_town_link = "";

//-->Constructors

        public Trade()
        {

        }

        public Trade(string p_Id)
        {
            m_Id = p_Id;
        }

//-->Attributes

        public string Origin_town_id
        {
            get { return m_Origin_town_id; }
            set { m_Origin_town_id = value; }
        }

        public string Destination_town_id
        {
            get { return m_Destination_town_id; }
            set { m_Destination_town_id = value; }
        }

        public string Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public string Wood
        {
            get { return m_Wood; }
            set { m_Wood = value; }
        }

        public string Stone
        {
            get { return m_Stone; }
            set { m_Stone = value; }
        }

        public string Iron
        {
            get { return m_Iron; }
            set { m_Iron = value; }
        }

        public string In_exchange
        {
            get { return m_In_exchange; }
            set { m_In_exchange = value; }
        }
    }
}
