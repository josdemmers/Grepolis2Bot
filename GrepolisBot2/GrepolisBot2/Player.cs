using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GrepolisBot2
{
    class Player
    {
        //General
        string m_PlayerID = "";
        string m_DefaultTownID = "";

        //Gods
        int m_FavorZeus = 0;
        int m_FavorPoseidon = 0;
        int m_FavorHera = 0;
        int m_FavorAthena = 0;
        int m_FavorHades = 0;
        int m_FavorArtemis = 0;

        string m_FavorZeusProduction = "0";
        string m_FavorPoseidonProduction = "0";
        string m_FavorHeraProduction = "0";
        string m_FavorAthenaProduction = "0";
        string m_FavorHadesProduction = "0";
        string m_FavorArtemisProduction = "0";

        //Cultural
        string m_CulturalLevelStr = "";
        string m_CulturalCitiesStr = "";
        string m_CulturalPointsStr = "";
        int m_CulturalPointsCurrent = 0;
        int m_CulturalPointsMax = 0;

        //Premium
        int m_Gold = 0;
        string m_CuratorActive = "0";
        string m_TraderActive = "0";
        string m_PriestActive = "0";
        string m_CommanderActive = "0";
        string m_CaptainActive = "0"; 

        //Attack
        private int m_IncomingAttacks = 0;//Is a variable for all towns

        //Custom classes
        private List<Town> m_Towns = new List<Town>();
        private List<Trade> m_Trades = new List<Trade>();
        private List<Group> m_Groups = new List<Group>();
        private List<Notification> m_Notifications = new List<Notification>();
        private List<QueueTemplate> m_TemplatesUnitQueue = new List<QueueTemplate>();
        private List<QueueTemplate> m_TemplatesBuildingQueue = new List<QueueTemplate>();

//-->Constructor
        
        public Player()
        {

        }

//-->Attributes

        //General
        public string PlayerID
        {
            get { return m_PlayerID; }
            set { m_PlayerID = value; }
        }

        public string DefaultTownID
        {
            get { return m_DefaultTownID; }
            set { m_DefaultTownID = value; }
        }

        //Gods
        public int FavorZeus
        {
            get { return m_FavorZeus; }
            set { m_FavorZeus = value; }
        }

        public int FavorPoseidon
        {
            get { return m_FavorPoseidon; }
            set { m_FavorPoseidon = value; }
        }

        public int FavorHera
        {
            get { return m_FavorHera; }
            set { m_FavorHera = value; }
        }

        public int FavorAthena
        {
            get { return m_FavorAthena; }
            set { m_FavorAthena = value; }
        }

        public int FavorHades
        {
            get { return m_FavorHades; }
            set { m_FavorHades = value; }
        }

        public int FavorArtemis
        {
            get { return m_FavorArtemis; }
            set { m_FavorArtemis = value; }
        }

        public string FavorZeusProduction
        {
            get { return m_FavorZeusProduction; }
            set { m_FavorZeusProduction = value; }
        }

        public string FavorPoseidonProduction
        {
            get { return m_FavorPoseidonProduction; }
            set { m_FavorPoseidonProduction = value; }
        }

        public string FavorHeraProduction
        {
            get { return m_FavorHeraProduction; }
            set { m_FavorHeraProduction = value; }
        }

        public string FavorAthenaProduction
        {
            get { return m_FavorAthenaProduction; }
            set { m_FavorAthenaProduction = value; }
        }

        public string FavorHadesProduction
        {
            get { return m_FavorHadesProduction; }
            set { m_FavorHadesProduction = value; }
        }

        public string FavorArtemisProduction
        {
            get { return m_FavorArtemisProduction; }
            set { m_FavorArtemisProduction = value; }
        }

        //Cultural
        public string CulturalLevelStr
        {
            get { return m_CulturalLevelStr; }
            set { m_CulturalLevelStr = value; }
        }

        public string CulturalCitiesStr
        {
            get { return m_CulturalCitiesStr; }
            set { m_CulturalCitiesStr = value; }
        }

        public string CulturalPointsStr
        {
            get { return m_CulturalPointsStr; }
            set { m_CulturalPointsStr = value; }
        }

        public int CulturalPointsCurrent
        {
            get { return m_CulturalPointsCurrent; }
            set { m_CulturalPointsCurrent = value; }
        }

        public int CulturalPointsMax
        {
            get { return m_CulturalPointsMax; }
            set { m_CulturalPointsMax = value; }
        }

        //Premium
        public int Gold
        {
            get { return m_Gold; }
            set { m_Gold = value; }
        }

        public string CuratorActive
        {
            get { return m_CuratorActive; }
            set { m_CuratorActive = value; }
        }

        public string TraderActive
        {
            get { return m_TraderActive; }
            set { m_TraderActive = value; }
        }

        public string PriestActive
        {
            get { return m_PriestActive; }
            set { m_PriestActive = value; }
        }

        public string CommanderActive
        {
            get { return m_CommanderActive; }
            set { m_CommanderActive = value; }
        }

        public string CaptainActive
        {
            get { return m_CaptainActive; }
            set { m_CaptainActive = value; }
        }

        //Attack
        public int IncomingAttacks
        {
            get { return m_IncomingAttacks; }
            set { m_IncomingAttacks = value; }
        }

        //Custom classes
        public List<Town> Towns
        {
            get { return m_Towns; }
            set { m_Towns = value; }
        }

        public List<Trade> Trades
        {
            get { return m_Trades; }
            set { m_Trades = value; }
        }

        public List<Group> Groups
        {
            get { return m_Groups; }
            set { m_Groups = value; }
        }

        public List<QueueTemplate> TemplatesUnitQueue
        {
            get { return m_TemplatesUnitQueue; }
            set { m_TemplatesUnitQueue = value; }
        }

        public List<QueueTemplate> TemplatesBuildingQueue
        {
            get { return m_TemplatesBuildingQueue; }
            set { m_TemplatesBuildingQueue = value; }
        }

//-->Methods

        public void autoUpdateTownTrade()
        {
            for (int i = 0; i < Towns.Count; i++)
            {
                if (! Towns[i].TradeOmitFromChangeAll)
                     Towns[i].TradeMode = "send";
            }
        }

        public int autoUpdateTownTrade(int receiveAbove)
        {
            int numChanges = 0;

            for (int i = 0; i < Towns.Count; i++)
            {
                if (!Towns[i].TradeOmitFromChangeAll)
                {
                    if (Towns[i].PopulationAvailable > receiveAbove)
                    {
                        Towns[i].TradeMode = "receive";
                        numChanges++;
                    }

                    else
                        Towns[i].TradeMode = "send";
                }
            }
            return numChanges;
        }

        public string getAttackWarning(bool p_IncludeSupport)
        {
            string l_AttackWarning = "";
            string l_SupportWarning = "";
            string l_AttackMovement = "";
            string l_SupportMovement = "";

            for (int i = 0; i < Towns.Count; i++)
            {
                l_AttackMovement = Towns[i].getIncomingAttackMovement();
                l_SupportMovement = Towns[i].getIncomingSupportMovement();
                if (l_AttackMovement.Length > 0)
                {
                    if (l_AttackWarning.Length > 0)
                        l_AttackWarning = l_AttackWarning + Environment.NewLine + l_AttackMovement;
                    else
                        l_AttackWarning = l_AttackMovement;
                }
                if (l_SupportMovement.Length > 0)
                {
                    if (l_SupportWarning.Length > 0)
                        l_SupportWarning = l_SupportWarning + Environment.NewLine + l_SupportMovement;
                    else
                        l_SupportWarning = l_SupportMovement;
                }
            }
            if (l_AttackWarning.Length > 0 && p_IncludeSupport && l_SupportWarning.Length > 0)
                return l_AttackWarning + Environment.NewLine + "----------" + Environment.NewLine + l_SupportWarning;
            else
                return l_AttackWarning;
        }

        public string getAttackWarningForum(bool p_IncludeSupport)
        {
            string l_AttackWarning = "";
            string l_SupportWarning = "";
            string l_AttackMovement = "";
            string l_SupportMovement = "";

            for (int i = 0; i < Towns.Count; i++)
            {
                l_AttackMovement = Towns[i].getIncomingAttackMovementForum();
                l_SupportMovement = Towns[i].getIncomingSupportMovementForum();
                if (l_AttackMovement.Length > 0)
                {
                    if (l_AttackWarning.Length > 0)
                        l_AttackWarning = l_AttackWarning + Environment.NewLine + l_AttackMovement;
                    else
                        l_AttackWarning = l_AttackMovement;
                }
                if (l_SupportMovement.Length > 0)
                {
                    if (l_SupportWarning.Length > 0)
                        l_SupportWarning = l_SupportWarning + Environment.NewLine + l_SupportMovement;
                    else
                        l_SupportWarning = l_SupportMovement;
                }
            }
            if (l_AttackWarning.Length > 0)
            {
                if (p_IncludeSupport && l_SupportWarning.Length > 0)
                    return "Forum message" + Environment.NewLine + l_AttackWarning + Environment.NewLine + "----------" + Environment.NewLine + l_SupportWarning;
                else
                    return "Forum message" + Environment.NewLine + l_AttackWarning;
            }
            else
                return l_AttackWarning;
        }

        public bool isGodAvailable(string p_God)
        {
            bool l_Available = false;

            switch (p_God)
            {
                case "zeus":
                    l_Available = !FavorZeusProduction.Equals("0");
                    break;
                case "poseidon":
                    l_Available = !FavorPoseidonProduction.Equals("0");
                    break;
                case "hera":
                    l_Available = !FavorHeraProduction.Equals("0");
                    break;
                case "athena":
                    l_Available = !FavorAthenaProduction.Equals("0");
                    break;
                case "hades":
                    l_Available = !FavorHadesProduction.Equals("0");
                    break;
            }
            return l_Available;
        }

        public string getFavorByTownIndex(int p_Index)
        {
            if (Towns[p_Index].God.Equals("zeus"))
                return FavorZeus.ToString() + " (+" + FavorZeusProduction + ")";
            else if (Towns[p_Index].God.Equals("poseidon"))
                return FavorPoseidon.ToString() + " (+" + FavorPoseidonProduction + ")";
            else if (Towns[p_Index].God.Equals("hera"))
                return FavorHera.ToString() + " (+" + FavorHeraProduction + ")";
            else if (Towns[p_Index].God.Equals("athena"))
                return FavorAthena.ToString() + " (+" + FavorAthenaProduction + ")";
            else if (Towns[p_Index].God.Equals("hades"))
                return FavorHades.ToString() + " (+" + FavorHadesProduction + ")";
            else if (Towns[p_Index].God.Equals("artemis"))
                return FavorArtemis.ToString() + " (+" + FavorArtemisProduction + ")";
            return "0 (+0)";
        }

        public string getFavorByTownIndexNewLine(int p_Index)
        {
            if (Towns[p_Index].God.Equals("zeus"))
                return FavorZeus.ToString() + Environment.NewLine + "(+" + FavorZeusProduction + ")";
            else if (Towns[p_Index].God.Equals("poseidon"))
                return FavorPoseidon.ToString() + Environment.NewLine + "(+" + FavorPoseidonProduction + ")";
            else if (Towns[p_Index].God.Equals("hera"))
                return FavorHera.ToString() + Environment.NewLine + "(+" + FavorHeraProduction + ")";
            else if (Towns[p_Index].God.Equals("athena"))
                return FavorAthena.ToString() + Environment.NewLine + "(+" + FavorAthenaProduction + ")";
            else if (Towns[p_Index].God.Equals("hades"))
                return FavorHades.ToString() + Environment.NewLine + "(+" + FavorHadesProduction + ")";
            else if (Towns[p_Index].God.Equals("artemis"))
                return FavorArtemis.ToString() + Environment.NewLine + "(+" + FavorArtemisProduction + ")";
            return "0" + Environment.NewLine + "(+0)";
        }

        public int getFavorByTownIndexInt(int p_Index)
        {
            if (Towns[p_Index].God.Equals("zeus"))
                return FavorZeus;
            else if (Towns[p_Index].God.Equals("poseidon"))
                return FavorPoseidon;
            else if (Towns[p_Index].God.Equals("hera"))
                return FavorHera;
            else if (Towns[p_Index].God.Equals("athena"))
                return FavorAthena;
            else if (Towns[p_Index].God.Equals("hades"))
                return FavorHades;
            else if (Towns[p_Index].God.Equals("artemis"))
                return FavorArtemis;
            return 0;
        }

        public string getActivePowerUsageByName(string p_Power)
        {
            string l_Usage = "";
            for (int i = 0; i < m_Towns.Count; i++)
            {
                if (m_Towns[i].CastedPowers.Contains(p_Power))
                    l_Usage += m_Towns[i].Name + ", ";
            }
            return l_Usage;
        }

        /*
         * Checks if group with p_ID is in current Group list
         */
        public bool isUniqueGroup(String p_ID)
        {
            bool l_IsUnique = true;
            for (int i = 0; i < m_Groups.Count; i++)
            {
                if (m_Groups[i].ID == p_ID)
                    l_IsUnique = false;
            }
            return l_IsUnique;
        }

        /*
         * Get group index by ID
         */
        public int getGroupIndexByID(string p_ID)
        {

            for (int i = 0; i < m_Groups.Count; i++)
            {
                if (m_Groups[i].ID == p_ID)
                    return i;
            }
            return -1;
        }

        /*
         * Checks if town with p_ID is in current Town list
         */
        public bool isUniqueTown(String p_ID)
        {
            bool l_IsUnique = true;
            for (int i = 0; i < m_Towns.Count; i++)
            {
                if (m_Towns[i].TownID == p_ID)
                    l_IsUnique = false;
            }
            return l_IsUnique;
        }
    
        /*
         * Get town index by ID
         */
        public int getTownIndexByID(string p_ID)
        {
            
            for (int i = 0; i < m_Towns.Count; i++)
            {
                if (m_Towns[i].TownID == p_ID)
                    return  i;
            }
            return -1;
        }

        public void sortTownsByName()
        {
            int l_CurrentPos;
            int l_MinimumPos;

            for (l_CurrentPos = 0; l_CurrentPos < m_Towns.Count; l_CurrentPos++)
            {
                /* Find the town name with the lowest alphabetic order in the current sub array*/
                l_MinimumPos = l_CurrentPos;
                for (int i = l_CurrentPos + 1; i < m_Towns.Count; i++)
                {
                    if (string.Compare(m_Towns[i].Name, m_Towns[l_MinimumPos].Name) == -1)
                    {
                        l_MinimumPos = i;
                    }
                }
                /* Swap the current town and the town with the lowest alphabetic order */
                if (l_MinimumPos != l_CurrentPos)
                {
                    Town l_Town = m_Towns[l_CurrentPos];
                    m_Towns[l_CurrentPos] = m_Towns[l_MinimumPos];
                    m_Towns[l_MinimumPos] = l_Town;
                }
            }
        }

        public void addNotification(string p_ServerTime, string p_Notify_id, string p_Time, string p_Type, string p_Subject)
        {
            if(isUniqueNotification(p_Notify_id))
            {
                m_Notifications.Add(new Notification(p_ServerTime, p_Notify_id, p_Time, p_Type, p_Subject));
            }
        }

        public bool isUniqueNotification(string p_Notify_id)
        {
            bool l_IsUnique = true;
            for (int i = 0; i < m_Notifications.Count; i++)
            {
                if (m_Notifications[i].Notify_id.Equals(p_Notify_id))
                    l_IsUnique = false;
            }
            return l_IsUnique;
        }

        public string getNotifications()
        {
            Settings l_Settings = Settings.Instance;

            string l_Notifications = "";
            string l_Notification = "";
            int l_Size = 0;

            for (int i = m_Notifications.Count - 1; i >= 0; i--)
            {
                l_Size++;
                if (m_Notifications[i].Subject.Equals(""))
                    l_Notification = m_Notifications[i].getHumanTimeNotification() + " " + m_Notifications[i].Type;
                else
                {
                    if (m_Notifications[i].Type.Equals("incoming_attack"))
                        l_Notification = m_Notifications[i].getHumanTimeCreated() + " / " + m_Notifications[i].getHumanTimeNotification() + " " + m_Notifications[i].Subject;
                    else
                        l_Notification = m_Notifications[i].getHumanTimeNotification() + " " + m_Notifications[i].Subject;
                }

                if(l_Notifications.Equals(""))
                    l_Notifications = l_Notification;
                else
                    l_Notifications = l_Notifications + System.Environment.NewLine + l_Notification;

                if (l_Size > l_Settings.GUINotificationSize)
                    break;
            }

            return l_Notifications;
        }

        public string getLatestNotificationID()
        {   
            string l_NotificationID = "0";
            if (m_Notifications.Count > 0)
                l_NotificationID = m_Notifications[m_Notifications.Count - 1].Notify_id;
            return l_NotificationID;
            //return "0";
        }

        public void loadTownsSettings()
        {
            //Note Modify this when adding new town settings (2/2)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            loadUnitQueueSettings();
            
            try
            {
                XmlTextReader l_TextReader = new XmlTextReader(l_Settings.AdvTownsSettingsFile);
                int l_TownIndex = -1;

                l_TextReader.Read();

                // If the node has value
                while (l_TextReader.Read())
                {
                    // Move to fist element
                    l_TextReader.MoveToElement();

                    if (l_TextReader.Name.Equals("Town"))
                    {
                        l_TownIndex = getTownIndexByID(l_TextReader.GetAttribute("TownID"));
                        if (l_TownIndex != -1)
                        {
                            if (l_TextReader.GetAttribute("BuildingQueueEnabled") != null)
                                m_Towns[l_TownIndex].BuildingQueueEnabled = l_TextReader.GetAttribute("BuildingQueueEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("BuildingLevelsTargetEnabled") != null)
                                m_Towns[l_TownIndex].BuildingLevelsTargetEnabled = l_TextReader.GetAttribute("BuildingLevelsTargetEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("BuildingDowngradeEnabled") != null)
                                m_Towns[l_TownIndex].BuildingDowngradeEnabled = l_TextReader.GetAttribute("BuildingDowngradeEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("BuildingLevelsTarget") != null)
                                m_Towns[l_TownIndex].setBuildingsLevelTarget(l_TextReader.GetAttribute("BuildingLevelsTarget"));
                            if (l_TextReader.GetAttribute("BuildingQueue") != null)
                                m_Towns[l_TownIndex].setBuildingQueue(l_TextReader.GetAttribute("BuildingQueue"));
                            if (l_TextReader.GetAttribute("UnitQueueEnabled") != null)
                                m_Towns[l_TownIndex].UnitQueueEnabled = l_TextReader.GetAttribute("UnitQueueEnabled").Equals("True");
                            //if (l_TextReader.GetAttribute("UnitQueue") != null)
                            //    m_Towns[l_TownIndex].setUnitQueue(l_TextReader.GetAttribute("UnitQueue"));
                            if (l_TextReader.GetAttribute("FarmersLootEnabled") != null)
                                m_Towns[l_TownIndex].FarmersLootEnabled = l_TextReader.GetAttribute("FarmersLootEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("FarmersFriendlyDemandsOnly") != null)
                                m_Towns[l_TownIndex].FarmersFriendlyDemandsOnly = l_TextReader.GetAttribute("FarmersFriendlyDemandsOnly").Equals("True");
                            if (l_TextReader.GetAttribute("FarmersMinMood") != null)
                                m_Towns[l_TownIndex].FarmersMinMood = int.Parse(l_TextReader.GetAttribute("FarmersMinMood"));
                            if (l_TextReader.GetAttribute("FarmersLootInterval") != null)
                                m_Towns[l_TownIndex].FarmersLootInterval = l_TextReader.GetAttribute("FarmersLootInterval");
                            //if (l_TextReader.GetAttribute("FarmersSelected") != null)
                            //    m_Towns[l_TownIndex].setSelectedFarmers(l_TextReader.GetAttribute("FarmersSelected"));
                            if (l_TextReader.GetAttribute("CulturalFestivalsEnabled") != null)
                                m_Towns[l_TownIndex].CulturalFestivalsEnabled = l_TextReader.GetAttribute("CulturalFestivalsEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("CulturalPartyEnabled") != null)
                                m_Towns[l_TownIndex].CulturalPartyEnabled = l_TextReader.GetAttribute("CulturalPartyEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("CulturalGamesEnabled") != null)
                                m_Towns[l_TownIndex].CulturalGamesEnabled = l_TextReader.GetAttribute("CulturalGamesEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("CulturalTriumphEnabled") != null)
                                m_Towns[l_TownIndex].CulturalTriumphEnabled = l_TextReader.GetAttribute("CulturalTriumphEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("CulturalTheaterEnabled") != null)
                                m_Towns[l_TownIndex].CulturalTheaterEnabled = l_TextReader.GetAttribute("CulturalTheaterEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("MilitiaTrigger") != null)
                                m_Towns[l_TownIndex].MilitiaTrigger = int.Parse(l_TextReader.GetAttribute("MilitiaTrigger"));
                            //Trading
                            if (l_TextReader.GetAttribute("TradeEnabled") != null)
                                m_Towns[l_TownIndex].TradeEnabled = l_TextReader.GetAttribute("TradeEnabled").Equals("True");
                            if (l_TextReader.GetAttribute("TradeMode") != null)
                                m_Towns[l_TownIndex].TradeMode = l_TextReader.GetAttribute("TradeMode");
                            if (l_TextReader.GetAttribute("TradeRemainingResources") != null)
                                m_Towns[l_TownIndex].TradeRemainingResources = int.Parse(l_TextReader.GetAttribute("TradeRemainingResources"));
                            if (l_TextReader.GetAttribute("TradeMinSendAmount") != null)
                                m_Towns[l_TownIndex].TradeMinSendAmount = int.Parse(l_TextReader.GetAttribute("TradeMinSendAmount"));
                            if (l_TextReader.GetAttribute("TradePercentageWarehouse") != null)
                                m_Towns[l_TownIndex].TradePercentageWarehouse = int.Parse(l_TextReader.GetAttribute("TradePercentageWarehouse"));
                            if (l_TextReader.GetAttribute("TradeMaxDistance") != null)
                                m_Towns[l_TownIndex].TradeMaxDistance = int.Parse(l_TextReader.GetAttribute("TradeMaxDistance"));
                            if (l_TextReader.GetAttribute("TradeOmitFromChangeAll") != null)
                                m_Towns[l_TownIndex].TradeOmitFromChangeAll = l_TextReader.GetAttribute("TradeOmitFromChangeAll").Equals("True");
                        }
                    }
                }
                l_TextReader.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loadTownsSettings(): " + e.Message);
            }
        }

        public void loadUnitQueueSettings()
        {
            //Note Modify this when adding new units (14/15)

            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextReader l_TextReader = new XmlTextReader(l_Settings.AdvUnitQueueSettingsFile);
                int l_TownIndex = -1;

                l_TextReader.Read();

                // If the node has value
                while (l_TextReader.Read())
                {
                    // Move to fist element
                    l_TextReader.MoveToElement();

                    if (l_TextReader.Name.Equals("Town"))
                    {
                        l_TownIndex = getTownIndexByID(l_TextReader.GetAttribute("TownID"));
                        if (l_TownIndex != -1)
                        {
                            if (l_TextReader.GetAttribute("sword") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("sword", int.Parse(l_TextReader.GetAttribute("sword")));
                            if (l_TextReader.GetAttribute("slinger") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("slinger", int.Parse(l_TextReader.GetAttribute("slinger")));
                            if (l_TextReader.GetAttribute("archer") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("archer", int.Parse(l_TextReader.GetAttribute("archer")));
                            if (l_TextReader.GetAttribute("hoplite") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("hoplite", int.Parse(l_TextReader.GetAttribute("hoplite")));
                            if (l_TextReader.GetAttribute("rider") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("rider", int.Parse(l_TextReader.GetAttribute("rider")));
                            if (l_TextReader.GetAttribute("chariot") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("chariot", int.Parse(l_TextReader.GetAttribute("chariot")));
                            if (l_TextReader.GetAttribute("catapult") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("catapult", int.Parse(l_TextReader.GetAttribute("catapult")));
                            if (l_TextReader.GetAttribute("minotaur") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("minotaur", int.Parse(l_TextReader.GetAttribute("minotaur")));
                            if (l_TextReader.GetAttribute("manticore") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("manticore", int.Parse(l_TextReader.GetAttribute("manticore")));
                            if (l_TextReader.GetAttribute("centaur") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("centaur", int.Parse(l_TextReader.GetAttribute("centaur")));
                            if (l_TextReader.GetAttribute("pegasus") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("pegasus", int.Parse(l_TextReader.GetAttribute("pegasus")));
                            if (l_TextReader.GetAttribute("harpy") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("harpy", int.Parse(l_TextReader.GetAttribute("harpy")));
                            if (l_TextReader.GetAttribute("medusa") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("medusa", int.Parse(l_TextReader.GetAttribute("medusa")));
                            if (l_TextReader.GetAttribute("zyklop") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("zyklop", int.Parse(l_TextReader.GetAttribute("zyklop")));
                            if (l_TextReader.GetAttribute("cerberus") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("cerberus", int.Parse(l_TextReader.GetAttribute("cerberus")));
                            if (l_TextReader.GetAttribute("fury") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("fury", int.Parse(l_TextReader.GetAttribute("fury")));
                            if (l_TextReader.GetAttribute("griffin") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("griffin", int.Parse(l_TextReader.GetAttribute("griffin")));
                            if (l_TextReader.GetAttribute("calydonian_boar") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("calydonian_boar", int.Parse(l_TextReader.GetAttribute("calydonian_boar")));
                            if (l_TextReader.GetAttribute("godsent") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("godsent", int.Parse(l_TextReader.GetAttribute("godsent")));
                            if (l_TextReader.GetAttribute("big_transporter") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("big_transporter", int.Parse(l_TextReader.GetAttribute("big_transporter")));
                            if (l_TextReader.GetAttribute("bireme") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("bireme", int.Parse(l_TextReader.GetAttribute("bireme")));
                            if (l_TextReader.GetAttribute("attack_ship") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("attack_ship", int.Parse(l_TextReader.GetAttribute("attack_ship")));
                            if (l_TextReader.GetAttribute("demolition_ship") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("demolition_ship", int.Parse(l_TextReader.GetAttribute("demolition_ship")));
                            if (l_TextReader.GetAttribute("small_transporter") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("small_transporter", int.Parse(l_TextReader.GetAttribute("small_transporter")));
                            if (l_TextReader.GetAttribute("trireme") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("trireme", int.Parse(l_TextReader.GetAttribute("trireme")));
                            if (l_TextReader.GetAttribute("colonize_ship") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("colonize_ship", int.Parse(l_TextReader.GetAttribute("colonize_ship")));
                            if (l_TextReader.GetAttribute("sea_monster") != null)
                                m_Towns[l_TownIndex].setUnitTargetAmount("sea_monster", int.Parse(l_TextReader.GetAttribute("sea_monster")));
                        }
                    }
                }
                l_TextReader.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loadUnitQueueSettings(): " + e.Message);
            }
        }

        public void loadFarmersSettings()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextReader l_TextReader = new XmlTextReader(l_Settings.AdvFarmersSettingsFile);
                int l_TownIndex = -1;

                l_TextReader.Read();

                // If the node has value
                while (l_TextReader.Read())
                {
                    // Move to fist element
                    l_TextReader.MoveToElement();

                    if (l_TextReader.Name.Equals("Town"))
                    {
                        l_TownIndex = getTownIndexByID(l_TextReader.GetAttribute("TownID"));
                        if (l_TownIndex != -1)
                        {
                            if (l_TextReader.GetAttribute("FarmersSelected") != null)
                                m_Towns[l_TownIndex].setSelectedFarmers(l_TextReader.GetAttribute("FarmersSelected"));
                        }
                    }
                }
                l_TextReader.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loadFarmersSettings(): " + e.Message);
            }
        }

        public void loadFarmersSettings(string p_TownID)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextReader l_TextReader = new XmlTextReader(l_Settings.AdvFarmersSettingsFile);
                int l_TownIndex = -1;

                l_TextReader.Read();

                // If the node has value
                while (l_TextReader.Read())
                {
                    // Move to fist element
                    l_TextReader.MoveToElement();

                    if (l_TextReader.Name.Equals("Town"))
                    {
                        l_TownIndex = getTownIndexByID(l_TextReader.GetAttribute("TownID"));
                        if (l_TownIndex != -1)
                        {
                            if (p_TownID.Equals(l_TextReader.GetAttribute("TownID")))
                            {
                                if (l_TextReader.GetAttribute("FarmersSelected") != null)
                                    m_Towns[l_TownIndex].setSelectedFarmers(l_TextReader.GetAttribute("FarmersSelected"));
                            }
                        }
                    }
                }
                l_TextReader.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loadFarmersSettings(string): " + e.Message);
            }
        }

        public void loadTemplatesUnitQueue()
        {
            //Note Modify this when adding new units (15/15)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextReader l_TextReader = new XmlTextReader(l_Settings.AdvTemplatesUnitQueueSettingsFile);

                l_TextReader.Read();

                // If the node has value
                while (l_TextReader.Read())
                {
                    // Move to fist element
                    l_TextReader.MoveToElement();

                    if (l_TextReader.Name.Equals("Template"))
                    {
                        if (l_TextReader.GetAttribute("Name") != null)
                        {
                            m_TemplatesUnitQueue.Add(new QueueTemplate(l_TextReader.GetAttribute("Name")));
                            if (l_TextReader.GetAttribute("sword") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("sword", int.Parse(l_TextReader.GetAttribute("sword")));
                            if (l_TextReader.GetAttribute("slinger") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("slinger", int.Parse(l_TextReader.GetAttribute("slinger")));
                            if (l_TextReader.GetAttribute("archer") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("archer", int.Parse(l_TextReader.GetAttribute("archer")));
                            if (l_TextReader.GetAttribute("hoplite") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("hoplite", int.Parse(l_TextReader.GetAttribute("hoplite")));
                            if (l_TextReader.GetAttribute("rider") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("rider", int.Parse(l_TextReader.GetAttribute("rider")));
                            if (l_TextReader.GetAttribute("chariot") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("chariot", int.Parse(l_TextReader.GetAttribute("chariot")));
                            if (l_TextReader.GetAttribute("catapult") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("catapult", int.Parse(l_TextReader.GetAttribute("catapult")));
                            if (l_TextReader.GetAttribute("minotaur") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("minotaur", int.Parse(l_TextReader.GetAttribute("minotaur")));
                            if (l_TextReader.GetAttribute("manticore") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("manticore", int.Parse(l_TextReader.GetAttribute("manticore")));
                            if (l_TextReader.GetAttribute("centaur") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("centaur", int.Parse(l_TextReader.GetAttribute("centaur")));
                            if (l_TextReader.GetAttribute("pegasus") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("pegasus", int.Parse(l_TextReader.GetAttribute("pegasus")));
                            if (l_TextReader.GetAttribute("harpy") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("harpy", int.Parse(l_TextReader.GetAttribute("harpy")));
                            if (l_TextReader.GetAttribute("medusa") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("medusa", int.Parse(l_TextReader.GetAttribute("medusa")));
                            if (l_TextReader.GetAttribute("zyklop") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("zyklop", int.Parse(l_TextReader.GetAttribute("zyklop")));
                            if (l_TextReader.GetAttribute("cerberus") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("cerberus", int.Parse(l_TextReader.GetAttribute("cerberus")));
                            if (l_TextReader.GetAttribute("fury") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("fury", int.Parse(l_TextReader.GetAttribute("fury")));
                            if (l_TextReader.GetAttribute("griffin") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("griffin", int.Parse(l_TextReader.GetAttribute("griffin")));
                            if (l_TextReader.GetAttribute("calydonian_boar") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("calydonian_boar", int.Parse(l_TextReader.GetAttribute("calydonian_boar")));
                            if (l_TextReader.GetAttribute("godsent") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("godsent", int.Parse(l_TextReader.GetAttribute("godsent")));
                            if (l_TextReader.GetAttribute("big_transporter") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("big_transporter", int.Parse(l_TextReader.GetAttribute("big_transporter")));
                            if (l_TextReader.GetAttribute("bireme") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("bireme", int.Parse(l_TextReader.GetAttribute("bireme")));
                            if (l_TextReader.GetAttribute("attack_ship") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("attack_ship", int.Parse(l_TextReader.GetAttribute("attack_ship")));
                            if (l_TextReader.GetAttribute("demolition_ship") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("demolition_ship", int.Parse(l_TextReader.GetAttribute("demolition_ship")));
                            if (l_TextReader.GetAttribute("small_transporter") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("small_transporter", int.Parse(l_TextReader.GetAttribute("small_transporter")));
                            if (l_TextReader.GetAttribute("trireme") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("trireme", int.Parse(l_TextReader.GetAttribute("trireme")));
                            if (l_TextReader.GetAttribute("colonize_ship") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("colonize_ship", int.Parse(l_TextReader.GetAttribute("colonize_ship")));
                            if (l_TextReader.GetAttribute("sea_monster") != null)
                                m_TemplatesUnitQueue[m_TemplatesUnitQueue.Count - 1].setUnitTargetAmount("sea_monster", int.Parse(l_TextReader.GetAttribute("sea_monster")));
                        }
                    }
                }
                l_TextReader.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loadTemplatesUnitQueue(): " + e.Message);
            }
        }

        public void loadTemplatesBuildingQueue()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextReader l_TextReader = new XmlTextReader(l_Settings.AdvTemplatesBuildingQueueSettingsFile);

                l_TextReader.Read();

                // If the node has value
                while (l_TextReader.Read())
                {
                    // Move to fist element
                    l_TextReader.MoveToElement();

                    if (l_TextReader.Name.Equals("Template"))
                        m_TemplatesBuildingQueue.Add(new QueueTemplate(l_TextReader.GetAttribute("Name"), l_TextReader.GetAttribute("Queue")));
                }
                l_TextReader.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loadTemplatesBuildingQueue(): " + e.Message);
            }
        }

        public string getTownsSortedByDistance(int p_Index)
        {
            string l_SortedTowns = "";
            int[] l_TownIndexes = new int[m_Towns.Count];

            //Prepare array
            for (int i = 0; i < l_TownIndexes.Length; i++)
            {
                l_TownIndexes[i] = i;
            }

            //Sort array
            for (int i = 0; i < l_TownIndexes.Length; i++)
            { 
                int min = i;
                double minDistance = Math.Sqrt(
                    Math.Pow(Math.Abs((int.Parse(m_Towns[p_Index].IslandX) - int.Parse(m_Towns[l_TownIndexes[i]].IslandX))), 2) +
                    Math.Pow(Math.Abs((int.Parse(m_Towns[p_Index].IslandY) - int.Parse(m_Towns[l_TownIndexes[i]].IslandY))), 2));

                int j;
                for (j = i + 1; j < l_TownIndexes.Length; j++) 
                {
                    double curDistance = Math.Sqrt(
                    Math.Pow(Math.Abs((int.Parse(m_Towns[p_Index].IslandX) - int.Parse(m_Towns[l_TownIndexes[j]].IslandX))), 2) +
                    Math.Pow(Math.Abs((int.Parse(m_Towns[p_Index].IslandY) - int.Parse(m_Towns[l_TownIndexes[j]].IslandY))), 2));

                    if (curDistance < minDistance) 
                    { 
                        min = j;
                        minDistance = curDistance;
                    }
                } 
                int l_TownIndex = l_TownIndexes[min];
                l_TownIndexes[min] = l_TownIndexes[i];
                l_TownIndexes[i] = l_TownIndex;
            }

            //Array to string
            for (int i = 0; i < l_TownIndexes.Length; i++)
            {
                l_SortedTowns += l_TownIndexes[i].ToString() + ";";
            }

            return l_SortedTowns;
        }

        public string getTradeTownsInRange(int p_Index)
        {
            string l_Towns = "";

            for (int i = 0; i < m_Towns.Count; i++)
            {
                int min = i;
                double l_Distance = Math.Sqrt(
                    Math.Pow(Math.Abs((int.Parse(m_Towns[p_Index].IslandX) - int.Parse(m_Towns[i].IslandX))), 2) +
                    Math.Pow(Math.Abs((int.Parse(m_Towns[p_Index].IslandY) - int.Parse(m_Towns[i].IslandY))), 2));

                if (l_Distance < m_Towns[p_Index].TradeMaxDistance)
                    l_Towns += m_Towns[i].Name + System.Environment.NewLine;
            }
            return l_Towns;
         }

        public string getTownsOnIsland(string p_X, string p_Y)
        {
            string l_Towns = "";

            for (int i = 0; i < m_Towns.Count; i++)
            {
                if (p_X.Equals(m_Towns[i].IslandX) && p_Y.Equals(m_Towns[i].IslandY))
                    l_Towns += m_Towns[i].Name + System.Environment.NewLine;
            }
            return l_Towns;
        }

        public int getTotalUnits(string p_Unit)
        {
            int l_UnitCount = 0;

            for (int i = 0; i < m_Towns.Count; i++)
            {
                l_UnitCount += m_Towns[i].ArmyUnits[m_Towns[i].getUnitIndex(p_Unit)].CurrentAmount;
            }

            return l_UnitCount;
        }

        public int getTotalUnitsAll(string p_Unit)
        {
            int l_UnitCount = 0;

            for (int i = 0; i < m_Towns.Count; i++)
            {
                l_UnitCount += m_Towns[i].ArmyUnits[m_Towns[i].getUnitIndex(p_Unit)].TotalAmount;
            }

            return l_UnitCount;
        }

        public void resetCastedPowers()
        {
            for (int i = 0; i < m_Towns.Count; i++)
            {
                m_Towns[i].CastedPowers = "";
            }
        }

        public void resetBuildingQueue()
        {
            for (int i = 0; i < m_Towns.Count; i++)
            {
                m_Towns[i].IngameBuildingQueueParsed = "";
            }
        }

        public void updateBuildingQueue()
        {
            for (int i = 0; i < m_Towns.Count; i++)
            {
                m_Towns[i].setIngameBuildingQueue(m_Towns[i].IngameBuildingQueueParsed);
            }
        }

        public int getTradeById(string p_Id)
        {
            for (int i = 0; i < m_Trades.Count; i++)
            {
                if(m_Trades[i].Id.Equals(p_Id))
                    return i;
            }
            return -1;
        }

        public void resetTrades()
        {
            m_Trades.Clear();
        }

        //Ignores farmers and market trades
        public int getOutgoingTradeCount(string p_TownID)
        {
            int l_OutgoingTrades = 0;
            for (int i = 0; i < m_Trades.Count; i++)
            {
                if (m_Trades[i].Origin_town_id.Equals(p_TownID) && m_Trades[i].In_exchange.Equals("false"))
                    l_OutgoingTrades++;
            }
            return l_OutgoingTrades;
        }

        public void resetUnitQueue()
        {
            for (int i = 0; i < m_Towns.Count; i++)
            {
                for (int j = 0; j < m_Towns[i].ArmyUnits.Count; j++)
                {
                    m_Towns[i].ArmyUnits[j].QueueGame = 0;
                }
                //Reset size of army unit queue
                m_Towns[i].SizeOfLandUnitQueue = 0;
                m_Towns[i].SizeOfNavyUnitQueue = 0;
            }
        }

        public void resetUnits()
        {
            for (int i = 0; i < m_Towns.Count; i++)
            {
                for (int j = 0; j < m_Towns[i].ArmyUnits.Count; j++)
                {
                    m_Towns[i].ArmyUnits[j].TotalAmount = 0;
                    m_Towns[i].ArmyUnits[j].CurrentAmount = 0;
                }
            }
        }

        public void setUnitCount(string p_Unit, string p_Count, string p_HomeTown, string p_CurrentTown)
        {
            if (p_HomeTown.Equals(p_CurrentTown))
            {
                int l_UnitIndex = m_Towns[getTownIndexByID(p_HomeTown)].getUnitIndex(p_Unit);
                if (l_UnitIndex != -1)//All unknown or ignored units give -1, e.g. militia
                {
                    m_Towns[getTownIndexByID(p_HomeTown)].ArmyUnits[l_UnitIndex].CurrentAmount = int.Parse(p_Count);
                    m_Towns[getTownIndexByID(p_HomeTown)].ArmyUnits[l_UnitIndex].TotalAmount += int.Parse(p_Count);
                }
            }
            else
            {
                for (int i = 0; i < Towns.Count; i++)
                {
                    if (Towns[i].TownID.Equals(p_HomeTown))
                    {
                        int l_UnitIndex = m_Towns[getTownIndexByID(p_HomeTown)].getUnitIndex(p_Unit);
                        if (l_UnitIndex != -1)//All unknown or ignored units give -1, e.g. militia
                        {
                            m_Towns[getTownIndexByID(p_HomeTown)].ArmyUnits[l_UnitIndex].TotalAmount += int.Parse(p_Count);
                        }
                        break;
                    }
                }
            }
        }

        public void setLocalBuildingName(string p_Id, string p_LocalName)
        {
            int l_Index = m_Towns[0].getBuildingIndex(p_Id);
            if (l_Index != -1)
            {
                for (int i = 0; i < m_Towns.Count; i++)
                {
                    m_Towns[i].Buildings[l_Index].LocalName = p_LocalName;
                }
            }
        }

        public void setLocalUnitName(string p_Id, string p_LocalName)
        {
            int l_Index = m_Towns[0].getUnitIndex(p_Id);
            if (l_Index != -1)
            {
                for (int i = 0; i < m_Towns.Count; i++)
                {
                    m_Towns[i].ArmyUnits[l_Index].LocalName = p_LocalName;
                }
            }
        }

        public bool setAllTownsLootInterval(string p_LootInterval)
        {
            bool l_MissingResearch = false;

            for (int i = 0; i < m_Towns.Count; i++)
            {
                if (m_Towns[i].Farmers.Count > 0)
                {
                    if ((p_LootInterval.Equals("10") || p_LootInterval.Equals("40") || p_LootInterval.Equals("180") || p_LootInterval.Equals("480")))
                    {
                        if (!m_Towns[i].Research.isResearched("booty"))
                            l_MissingResearch = true;
                        else
                            m_Towns[i].FarmersLootInterval = p_LootInterval;
                    }
                    else
                        m_Towns[i].FarmersLootInterval = p_LootInterval;
                }
            }

            return l_MissingResearch;
        }
    }
}
