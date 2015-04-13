using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GrepolisBot2
{
    class Town
    {
        //General
        private string m_TownID = "";
        private string m_Name = "";
        private string m_God = "null";//none, zeus, poseidon, hera, athena, hades, artemis. 
        private string m_IslandX = "000";
        private string m_IslandY = "000";
        private string m_Points = "0";
        private int m_PopulationMax = 0;
        private int m_PopulationAvailable = 0;
        private int m_PopulationExtra = 0;
        private string m_HasConqueror = "false";
        private bool m_BuildingsCanBeTearedDown = false;
        private string m_ServerTime = "";//Same variable as in controller class, added here as well to make resources calculations easier.

        //Building Queue
        private string[] m_IngameBuildingQueue = { "", "", "", "", "", "", "" };
        private string m_IngameBuildingQueueParsed = "";
        private bool m_IsBuildingOrderQueueFull = false;//Boolean from buildingBuildData request
        private Queue<string> m_BuildingQueue = new Queue<string>();
        private bool m_BuildingQueueEnabled = true;
        private bool m_BuildingLevelsTargetEnabled = false;
        private bool m_BuildingDowngradeEnabled = false;

        //Unit Queue
        private bool m_UnitQueueEnabled = true;
        private int m_SizeOfLandUnitQueue = 0;
        private int m_SizeOfNavyUnitQueue = 0;

        //Farmers
        private bool m_FarmersLootEnabled = false;
        private bool m_FarmersFriendlyDemandsOnly = false;
        private int m_FarmersMinMood = 86;
        private string m_FarmersLootInterval = "5";
        private ArrayList m_FarmersSequence = new ArrayList { 0, 1, 2, 3, 4, 5, 6, 7 };

        //Resources
        private int m_Wood = 0;
        private int m_Stone = 0;
        private int m_Iron = 0;
        private int m_Storage = 0;
        private int m_WoodProduction = 0;
        private int m_StoneProduction = 0;
        private int m_IronProduction = 0;
        private string m_ResourcesLastUpdate = "";
        private string m_ResourcePlenty = "";
        private string m_ResourceRare = "";
        private int m_EspionageStorage = 0;
        private bool m_HasEnoughResources = false;

        //Culture
        private bool m_CulturalFestivalsEnabled = false;
        private bool m_CulturalPartyEnabled = false;
        private bool m_CulturalGamesEnabled = false;
        private bool m_CulturalTriumphEnabled = false;
        private bool m_CulturalTheaterEnabled = false;

        //Powers
        private string m_CastedPowers = "";//fertility_improvement, call_of_the_ocean, town_protection

        //Militia
        private int m_MilitiaTrigger = 0;
        private bool m_MilitiaReady = false;

        //Research
        private string m_ResearchPoints = "0";//Not used at the moment

        //Trading
        private int m_WoodInc = 0;//Incoming reosurces by trade
        private int m_StoneInc = 0;//Incoming reosurces by trade
        private int m_IronInc = 0;//Incoming reosurces by trade
        private int m_FreeTradeCapacity = 0;
        private int m_MaxTradeCapacity = 0;
        private bool m_TradeEnabled = false;
        private string m_TradeMode = "send";//send, receive, spy cave
        private int m_TradeRemainingResources = 0;//For sender, how many resources you want to keep for yourself
        private int m_TradeMinSendAmount = 500;//For sender, what the minimum amount of resources is you want to send
        private int m_TradePercentageWarehouse = 95;//For receiver, how far you want to fill the warehouse.
        private int m_TradeMaxDistance = 100;//How far the tradeships should travel.
        private bool m_TradeOmitFromChangeAll = false;//Enable setting to omit trade mode changes made by the set all feature.

        //Quests
        private int m_AvailableQuests = 0;

        //Custom classes
        private List<Farmer> m_Farmers = new List<Farmer>();
        private List<ArmyUnit> m_ArmyUnits = new List<ArmyUnit>();
        private List<Building> m_Buildings = new List<Building>();
        private List<CulturalEvent> m_CulturalEvents = new List<CulturalEvent>();
        private List<Movement> m_Movements = new List<Movement>();
        private Research m_Research = new Research();

//-->Constructors
        
        public Town()
        {
            
        }

        public Town(string p_TownID, string p_TownName, string p_IslandX, string p_IslandY)
        {
            m_TownID = p_TownID;
            m_Name = p_TownName;
            m_IslandX = p_IslandX;
            m_IslandY = p_IslandY;
            addArmyUnits();
            addBuildings();
            addCulturalEvents();
        }

//-->Attributes
        
        //General
        public string TownID
        {
            get { return m_TownID; }
            set { m_TownID = value; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public String God
        {
            get { return m_God; }
            set { m_God = value; }
        }

        public string IslandX
        {
            get { return m_IslandX; }
            set { m_IslandX = value; }
        }

        public string IslandY
        {
            get { return m_IslandY; }
            set { m_IslandY = value; }
        }

        public string Points
        {
            get { return m_Points; }
            set { m_Points = value; }
        }

        public int PopulationMax
        {
            get { return m_PopulationMax; }
            set { m_PopulationMax = value; }
        }

        public int PopulationAvailable
        {
            get { return m_PopulationAvailable; }
            set { m_PopulationAvailable = value; }
        }

        public int PopulationExtra
        {
            get { return m_PopulationExtra; }
            set { m_PopulationExtra = value; }
        }

        public string HasConqueror
        {
            get { return m_HasConqueror; }
            set { m_HasConqueror = value; }
        }

        public bool BuildingsCanBeTearedDown
        {
            get { return m_BuildingsCanBeTearedDown; }
            set { m_BuildingsCanBeTearedDown = value; }
        }

        public string ServerTime
        {
            get { return m_ServerTime; }
            set { m_ServerTime = value; }
        }

        //Building Queue
        public String[] IngameBuildingQueue
        {
            get { return m_IngameBuildingQueue; }
            set { m_IngameBuildingQueue = value; }
        }

        public string IngameBuildingQueueParsed
        {
            get { return m_IngameBuildingQueueParsed; }
            set { m_IngameBuildingQueueParsed = value; }
        }

        public bool IsBuildingOrderQueueFull
        {
            get { return m_IsBuildingOrderQueueFull; }
            set { m_IsBuildingOrderQueueFull = value; }
        }

        public Queue<string> BuildingQueue
        {
            get { return m_BuildingQueue; }
            set { m_BuildingQueue = value; }
        }

        public bool BuildingQueueEnabled
        {
            get { return m_BuildingQueueEnabled; }
            set { m_BuildingQueueEnabled = value; }
        }

        public bool BuildingLevelsTargetEnabled
        {
            get { return m_BuildingLevelsTargetEnabled; }
            set { m_BuildingLevelsTargetEnabled = value; }
        }

        public bool BuildingDowngradeEnabled
        {
            get { return m_BuildingDowngradeEnabled; }
            set { m_BuildingDowngradeEnabled = value; }
        }

        public bool UnitQueueEnabled
        {
            get { return m_UnitQueueEnabled; }
            set { m_UnitQueueEnabled = value; }
        }

        public int SizeOfLandUnitQueue
        {
            get { return m_SizeOfLandUnitQueue; }
            set { m_SizeOfLandUnitQueue = value; }
        }

        public int SizeOfNavyUnitQueue
        {
            get { return m_SizeOfNavyUnitQueue; }
            set { m_SizeOfNavyUnitQueue = value; }
        }

        //Farmers
        public bool FarmersLootEnabled
        {
            get { return m_FarmersLootEnabled; }
            set { m_FarmersLootEnabled = value; }
        }

        public bool FarmersFriendlyDemandsOnly
        {
            get { return m_FarmersFriendlyDemandsOnly; }
            set { m_FarmersFriendlyDemandsOnly = value; }
        }

        public int FarmersMinMood
        {
            get { return m_FarmersMinMood; }
            set { m_FarmersMinMood = value; }
        }

        public string FarmersLootInterval
        {
            get { return m_FarmersLootInterval; }
            set { m_FarmersLootInterval = value; }
        }

        //Resources
        public int Wood
        {
            get
            {
                long l_TimeSinceLastUpdate = long.Parse(m_ServerTime) - long.Parse(m_ResourcesLastUpdate);
                l_TimeSinceLastUpdate -= 600;//Minus 10 minutes for safety
                if (l_TimeSinceLastUpdate < 0)
                    return m_Wood;
                else
                {
                    double l_WoodProduced = m_WoodProduction * (l_TimeSinceLastUpdate / 3600.0);
                    return Math.Min(m_Storage, m_Wood + (int)l_WoodProduced);
                }
            }
            set { m_Wood = value; }
        }

        public int Stone
        {
            get 
            {
                long l_TimeSinceLastUpdate = long.Parse(m_ServerTime) - long.Parse(m_ResourcesLastUpdate);
                l_TimeSinceLastUpdate -= 600;//Minus 10 minutes for safety
                if (l_TimeSinceLastUpdate < 0)
                    return m_Stone;
                else
                {
                    double l_StoneProduced = m_StoneProduction * (l_TimeSinceLastUpdate / 3600.0);
                    return Math.Min(m_Storage, m_Stone + (int)l_StoneProduced);
                }
            }
            set { m_Stone = value; }
        }

        public int Iron
        {
            get 
            {
                long l_TimeSinceLastUpdate = long.Parse(m_ServerTime) - long.Parse(m_ResourcesLastUpdate);
                l_TimeSinceLastUpdate -= 600;//Minus 10 minutes for safety
                if (l_TimeSinceLastUpdate < 0)
                    return m_Iron;
                else
                {
                    double l_IronProduced = m_IronProduction * (l_TimeSinceLastUpdate / 3600.0);
                    return Math.Min(m_Storage, m_Iron + (int)l_IronProduced);
                }
            }
            set { m_Iron = value; }
        }

        public int Storage
        {
            get 
            {
                //if (Research.isResearched("pottery"))
                //    return m_Storage + 2500;
                //else
                    return m_Storage;
            }
            set { m_Storage = value; }
        }

        public int WoodProduction
        {
            get { return m_WoodProduction; }
            set { m_WoodProduction = value; }
        }

        public int StoneProduction
        {
            get { return m_StoneProduction; }
            set { m_StoneProduction = value; }
        }

        public int IronProduction
        {
            get { return m_IronProduction; }
            set { m_IronProduction = value; }
        }

        public string ResourcesLastUpdate
        {
            get { return m_ResourcesLastUpdate; }
            set { m_ResourcesLastUpdate = value; }
        }

        public string ResourcePlenty
        {
            get { return m_ResourcePlenty; }
            set { m_ResourcePlenty = value; }
        }

        public string ResourceRare
        {
            get { return m_ResourceRare; }
            set { m_ResourceRare = value; }
        }

        public int EspionageStorage
        {
            get { return m_EspionageStorage; }
            set { m_EspionageStorage = value; }
        }

        public bool HasEnoughResources
        {
            get { return m_HasEnoughResources; }
            set { m_HasEnoughResources = value; }
        }

        //Culture
        public bool CulturalFestivalsEnabled
        {
            get { return m_CulturalFestivalsEnabled; }
            set { m_CulturalFestivalsEnabled = value; }
        }

        public bool CulturalPartyEnabled
        {
            get { return m_CulturalPartyEnabled; }
            set { m_CulturalPartyEnabled = value; }
        }

        public bool CulturalGamesEnabled
        {
            get { return m_CulturalGamesEnabled; }
            set { m_CulturalGamesEnabled = value; }
        }

        public bool CulturalTriumphEnabled
        {
            get { return m_CulturalTriumphEnabled; }
            set { m_CulturalTriumphEnabled = value; }
        }

        public bool CulturalTheaterEnabled
        {
            get { return m_CulturalTheaterEnabled; }
            set { m_CulturalTheaterEnabled = value; }
        }

        //Powers
        public string CastedPowers
        {
            get { return m_CastedPowers; }
            set { m_CastedPowers = value; }
        }

        //Militia
        public int MilitiaTrigger
        {
            get { return m_MilitiaTrigger; }
            set { m_MilitiaTrigger = value; }
        }

        public bool MilitiaReady
        {
            get { return m_MilitiaReady; }
            set { m_MilitiaReady = value; }
        }

        //Research
        public string ResearchPoints
        {
            get { return m_ResearchPoints; }
            set { m_ResearchPoints = value; }
        }

        //Trading
        public int WoodInc
        {
            get { return m_WoodInc; }
            set { m_WoodInc = value; }
        }

        public int StoneInc
        {
            get { return m_StoneInc; }
            set { m_StoneInc = value; }
        }

        public int IronInc
        {
            get { return m_IronInc; }
            set { m_IronInc = value; }
        }

        public int FreeTradeCapacity
        {
            get { return m_FreeTradeCapacity; }
            set { m_FreeTradeCapacity = value; }
        }

        public int MaxTradeCapacity
        {
            get { return m_MaxTradeCapacity; }
            set { m_MaxTradeCapacity = value; }
        }

        public int TradeRemainingResources
        {
            get { return m_TradeRemainingResources; }
            set { m_TradeRemainingResources = value; }
        }

        public bool TradeEnabled
        {
            get { return m_TradeEnabled; }
            set { m_TradeEnabled = value; }
        }

        public string TradeMode
        {
            get { return m_TradeMode; }
            set { m_TradeMode = value; }
        }

        public int TradeMinSendAmount
        {
            get { return m_TradeMinSendAmount; }
            set { m_TradeMinSendAmount = value; }
        }

        public int TradePercentageWarehouse
        {
            get { return m_TradePercentageWarehouse; }
            set { m_TradePercentageWarehouse = value; }
        }

        public int TradeMaxDistance
        {
            get { return m_TradeMaxDistance; }
            set { m_TradeMaxDistance = value; }
        }

        public bool TradeOmitFromChangeAll
        {
            get { return m_TradeOmitFromChangeAll; }
            set { m_TradeOmitFromChangeAll = value; }
        }

        //Quests
        public int AvailableQuests
        {
            get { return m_AvailableQuests; }
            set { m_AvailableQuests = value; }
        }

        public List<Farmer> Farmers
        {
            get { return m_Farmers; }
            set { m_Farmers = value; }
        }

        public List<ArmyUnit> ArmyUnits
        {
            get { return m_ArmyUnits; }
            set { m_ArmyUnits = value; }
        }

        public List<Building> Buildings
        {
            get { return m_Buildings; }
            set { m_Buildings = value; }
        }

        public List<CulturalEvent> CulturalEvents
        {
            get { return m_CulturalEvents; }
            set { m_CulturalEvents = value; }
        }

        public List<Movement> Movements
        {
            get { return m_Movements; }
            set { m_Movements = value; }
        }

        public Research Research
        {
            get { return m_Research; }
            set { m_Research = value; }
        }

//-->Methods

        public override string ToString()
        {
            return m_Name;
        }

        private void addBuildings()
        {
            m_Buildings.Add(new Building("main", 25, 25, 1, 1.5));
            m_Buildings.Add(new Building("hide", 10, 10, 3, 0.5));
            m_Buildings.Add(new Building("storage", 30, 35, 0, 1));
            m_Buildings.Add(new Building("farm", 40, 45, 0, 0));
            m_Buildings.Add(new Building("lumber", 40, 40, 1, 1.25));
            m_Buildings.Add(new Building("stoner", 40, 40, 1, 1.25));
            m_Buildings.Add(new Building("ironer", 40, 40, 1, 1.25));
            m_Buildings.Add(new Building("market", 30, 40, 2, 1.1));
            m_Buildings.Add(new Building("docks", 30, 30, 4, 1));//8
            m_Buildings.Add(new Building("barracks", 30, 30, 1, 1.3));//9
            m_Buildings.Add(new Building("wall", 25, 25, 2, 1.16));
            m_Buildings.Add(new Building("academy", 30, 36, 3, 1));
            m_Buildings.Add(new Building("temple", 25, 30, 5, 1));//12
            m_Buildings.Add(new Building("theater", 1, 1, 60, 1));
            m_Buildings.Add(new Building("thermal", 1, 1, 60, 1));
            m_Buildings.Add(new Building("library", 1, 1, 60, 1));
            m_Buildings.Add(new Building("lighthouse", 1, 1, 60, 1));
            m_Buildings.Add(new Building("tower", 1, 1, 60, 1));
            m_Buildings.Add(new Building("statue", 1, 1, 60, 1));
            m_Buildings.Add(new Building("oracle", 1, 1, 60, 1));
            m_Buildings.Add(new Building("trade_office", 1, 1, 60, 1));
        }

        private void addArmyUnits()
        {
            m_ArmyUnits.Add(new ArmyUnit("sword", 1, 0, "none", true, true, 95, 0, 85, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("slinger", 1, 0, "none", true, false, 55, 100, 40, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("archer", 1, 0, "none", true, false, 120, 0, 75, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("hoplite", 1, 0, "none", true, false, 0, 75, 150, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("rider", 3, 0, "none", true, false, 240, 120, 360, 0, 0, 10, 0));
            m_ArmyUnits.Add(new ArmyUnit("chariot", 4, 0, "none", true, false, 200, 440, 320, 0, 0, 15, 0));
            m_ArmyUnits.Add(new ArmyUnit("catapult", 15, 0, "none", true, false, 1200, 1200, 1200, 0, 0, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("minotaur", 30, 0, "zeus", true, true, 1400, 600, 3100, 202, 10, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("manticore", 45, 0, "zeus", true, true, 4400, 3000, 3400, 405, 15, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("centaur", 12, 0, "athena", true, true, 1740, 300, 700, 100, 4, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("pegasus", 20, 0, "athena", true, true, 2800, 360, 80, 180, 12, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("harpy", 14, 0, "hera", true, true, 1600, 400, 1360, 130, 5, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("medusa", 18, 0, "hera", true, true, 1500, 3800, 2200, 210, 10, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("zyklop", 40, 0, "poseidon", true, true, 2000, 4200, 3360, 360, 12, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("cerberus", 30, 0, "hades", true, true, 1250, 1500, 3000, 320, 10, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("fury", 55, 0, "hades", true, true, 2500, 5000, 5000, 480, 16, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("griffin", 38, 0, "artemis", true, true, 3800, 1900, 4800, 250, 15, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("calydonian_boar", 20, 0, "artemis", true, true, 2800, 1400, 1600, 110, 10, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("godsent", 3, 0, "none", true, true, 0, 0, 0, 15, 1, 1, 0));
            m_ArmyUnits.Add(new ArmyUnit("big_transporter", 7, 26, "none", false, true, 500, 500, 400, 0, 0, 0, 1));//19
            m_ArmyUnits.Add(new ArmyUnit("bireme", 8, 0, "none", false, false, 800, 700, 180, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("attack_ship", 10, 0, "none", false, false, 1300, 300, 800, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("demolition_ship", 8, 0, "none", false, false, 500, 750, 150, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("small_transporter", 5, 10, "none", false, false, 800, 0, 400, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("trireme", 16, 0, "none", false, false, 2000, 1300, 900, 0, 0, 0, 1));
            m_ArmyUnits.Add(new ArmyUnit("colonize_ship", 170, 0, "none", false, false, 10000, 10000, 10000, 0, 0, 0, 20));
            m_ArmyUnits.Add(new ArmyUnit("sea_monster", 50, 0, "poseidon", false, true, 5400, 2800, 3800, 400, 22, 0, 1));
        }

        private void addCulturalEvents()
        {
            m_CulturalEvents.Add(new CulturalEvent("party"));
            m_CulturalEvents.Add(new CulturalEvent("games"));
            m_CulturalEvents.Add(new CulturalEvent("triumph"));
            m_CulturalEvents.Add(new CulturalEvent("theater"));
        }

        public string getChunkX()
        {
            return ((int)(int.Parse(m_IslandX) / 20)).ToString();
        }

        public string getChunkY()
        {
            return ((int)(int.Parse(m_IslandY) / 20)).ToString();
        }

        public int getIngameBuildingQueueSize()
        {
            int l_IngameBuildingQueue = 0;

            for (int i = 0; i < m_IngameBuildingQueue.Length; i++)
            {
                if (m_IngameBuildingQueue[i].Length > 0)
                    l_IngameBuildingQueue += 1;
                else
                    break;
            }

            return l_IngameBuildingQueue;
        }

        public int getNumberOfFriendlyFarmers()
        {
            int l_FriendlyFarmers = 0;

            for (int i = 0; i < m_Farmers.Count; i++)
            {
                if (m_Farmers[i].RelationStatus)
                    l_FriendlyFarmers++;
            }

            return l_FriendlyFarmers;
        }        

        public bool isUniqueFarm(String p_ID)
        {
            bool l_IsUnique = true;
            for (int i = 0; i < m_Farmers.Count; i++)
            {
                if (m_Farmers[i].ID == p_ID)
                    l_IsUnique = false;
            }
            return l_IsUnique;
        }

        public int getFarmersIndex(String p_ID)
        {
            int l_Index = -1;
            for (int i = 0; i < m_Farmers.Count; i++)
            {
                if (m_Farmers[i].ID == p_ID)
                {
                    l_Index = i;
                    break;
                }
            }
            return l_Index;
        }

        public void resetArmyCurrentAmount()
        {
            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                m_ArmyUnits[i].CurrentAmount = 0;
            }
        }

        public void resetUnitQueueLand()
        {

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (m_ArmyUnits[i].IsFromBarracks)
                    m_ArmyUnits[i].QueueGame = 0;
            }
            //Reset size of army unit queue
            SizeOfLandUnitQueue = 0;
        }

        public void resetUnitQueueNavy()
        {

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if(!m_ArmyUnits[i].IsFromBarracks)
                    m_ArmyUnits[i].QueueGame = 0;
            }
            //Reset size of army unit queue
            SizeOfNavyUnitQueue = 0;
        }

        public int getIndexBuilding(string p_Name)
        {
            int l_Index = -1;
            for (int i = 0; i < m_Buildings.Count; i++)
            {
                if (m_Buildings[i].DevName == p_Name)
                    l_Index = i;
            }
            return l_Index;
        }

        public void setIngameBuildingQueue(string p_Queue)
        {
            string[] l_Buildings = new string[] { "", "" };//dynamic array
            l_Buildings = p_Queue.Split(';');
            for (int i = 0; i < m_IngameBuildingQueue.Length; i++)
            {
                if (i < l_Buildings.Length)
                    m_IngameBuildingQueue[i] = l_Buildings[i];
                else
                    m_IngameBuildingQueue[i] = "";
            }
        }

        public string getIngameBuildingQueue()
        {
            string l_IngameBuildingQueue = "";

            for (int i = 0; i < m_IngameBuildingQueue.Length; i++)
            {
                if (m_IngameBuildingQueue[i].Length > 0)
                    l_IngameBuildingQueue += m_IngameBuildingQueue[i] + ";";
                else
                    break;
            }

            return l_IngameBuildingQueue;
        }

        public int countIngameBuildingQueueByName(string p_Building)
        {
            int l_Count = 0;

            for (int i = 0; i < m_IngameBuildingQueue.Length; i++)
            {
                if (m_IngameBuildingQueue[i].Equals(p_Building))
                    l_Count++;
            }

            return l_Count;
        }

        public string getBuildingLevelsTarget()
        {
            string l_BuildingLevelsTarget = "";

            for (int i = 0; i < m_Buildings.Count; i++)
            {
                l_BuildingLevelsTarget += m_Buildings[i].TargetLevel.ToString() + ";";
            }

            return l_BuildingLevelsTarget;
        }

        /*
         * Return all the buildings in the queue as a string.
         */
        public string getBuildingQueue()
        {
            string l_Queue = "";
            // A queue can be enumerated without disturbing its contents.
            foreach (string l_Building in m_BuildingQueue)
            {
                l_Queue += l_Building;
                l_Queue += ";";
            }
            return l_Queue;
        }

        /*
         * Look at first building in queue without removing it.
         */
        public string peekQueueBuilding()
        {
            return m_BuildingQueue.Peek();
        }

        /*
         * Look at first building in queue and remove it.
         */
        public string dequeueBuilding()
        {
            return m_BuildingQueue.Dequeue();
        }


        public void setBuildingsLevelTarget(string p_BuildingsLevelTarget)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                string[] l_BuildingsLevelsTarget = new string[] { "", "" };//dynamic array
                l_BuildingsLevelsTarget = p_BuildingsLevelTarget.Split(';');

                for (int i = 0; i < m_Buildings.Count; i++)
                {
                    m_Buildings[i].TargetLevel = int.Parse(l_BuildingsLevelsTarget[i]);
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in setBuildingsLevelTarget(): " + e.Message);
            }
        }

        public void setBuildingQueue(string p_BuildingQueue)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                string[] l_BuildingQueue = new string[] { "", "" };//dynamic array
                l_BuildingQueue = p_BuildingQueue.Split(';');
                m_BuildingQueue.Clear();

                for (int i = 0; i < l_BuildingQueue.Length; i++)
                {
                    if (l_BuildingQueue[i].Length > 0)
                        m_BuildingQueue.Enqueue(l_BuildingQueue[i]);
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in setBuildingsLevelTarget(): " + e.Message);
            }
        }

        public string getBuildingLevels()
        {
            string l_BuildingLevels = "";

            for (int i = 0; i < m_Buildings.Count; i++)
            {
                l_BuildingLevels += m_Buildings[i].Level + ";";
            }

            return l_BuildingLevels;
        }

        public int getBuildingIndex(string p_Name)
        {
            int l_Index = m_Buildings.FindIndex(a => a.DevName == p_Name);
            return l_Index;
        }

        public string getBuildingNames()
        {
            string l_BuildingNames = "";

            for (int i = 0; i < m_Buildings.Count; i++)
            {
                l_BuildingNames += m_Buildings[i].LocalName + ";";
            }

            return l_BuildingNames;
        }

        public int getUnitTargetAmount(string p_Name)
        {
            int l_Target = 0;

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (m_ArmyUnits[i].Name.Equals(p_Name))
                    l_Target = m_ArmyUnits[i].QueueBot;
            }

            return l_Target;
        }

        public int getUnitQueueGame(string p_Name)
        {
            int l_QueueGame = 0;

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (m_ArmyUnits[i].Name.Equals(p_Name))
                    l_QueueGame = m_ArmyUnits[i].QueueGame;
            }
            return l_QueueGame;
        }

        public void setUnitTargetAmount(string p_Name, int p_Amount)
        {
            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (m_ArmyUnits[i].Name.Equals(p_Name))
                    m_ArmyUnits[i].QueueBot = p_Amount;
            }
        }

        public int getUnitTotalAmount(string p_Name)
        {
            int l_Target = 0;

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (m_ArmyUnits[i].Name.Equals(p_Name))
                    l_Target = m_ArmyUnits[i].TotalAmount;
            }

            return l_Target;
        }

        public int getUnitIndex(string p_Name)
        {
            int l_Index = m_ArmyUnits.FindIndex(a => a.Name == p_Name);
            return l_Index;

            //int l_Index = 0;
            //
            //for (int i = 0; i < m_ArmyUnits.Count; i++)
            //{
            //    if (m_ArmyUnits[i].Name.Equals(p_Name))
            //        l_Index = i;
            //}
            //
            //return l_Index;
        }

        public string getUnitNames()
        {
            string l_UnitNames = "";

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                l_UnitNames += m_ArmyUnits[i].LocalName + ";";
            }

            return l_UnitNames;
        }

        public bool isBarrackQueueFinished()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            bool l_Isfinished = true;
            int l_Amount = 0;

            try
            {
                for (int i = 0; i < m_ArmyUnits.Count; i++)
                {
                    if (m_ArmyUnits[i].IsFromBarracks)
                    {
                        l_Amount = ArmyUnits[i].QueueBot - (ArmyUnits[i].TotalAmount + ArmyUnits[i].QueueGame);
                        if (l_Amount > 0)
                        {
                            l_Isfinished = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in isBarrackQueueFinished(): " + e.Message);
            }

            return l_Isfinished;
        }

        public bool isDocksQueueFinished()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            bool l_Isfinished = true;
            int l_Amount = 0;

            try
            {
                for (int i = 0; i < m_ArmyUnits.Count; i++)
                {
                    if (!m_ArmyUnits[i].IsFromBarracks)
                    {
                        l_Amount = ArmyUnits[i].QueueBot - (ArmyUnits[i].TotalAmount + ArmyUnits[i].QueueGame);
                        if (l_Amount > 0)
                        {
                            l_Isfinished = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in isDocksQueueFinished(): " + e.Message);
            }

            return l_Isfinished;
        }

        public int getMostTrainableLandUnitInQueue(int p_Favor)
        {
            //updateMaxBuild(p_Favor);
            int l_UnitIndex = -1;
            int l_Trainable = -1;

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (m_ArmyUnits[i].IsFromBarracks)
                {
                    if (m_ArmyUnits[i].QueueBot > 0)
                    {
                        if (m_ArmyUnits[i].QueueBot - (m_ArmyUnits[i].TotalAmount + m_ArmyUnits[i].QueueGame) > 0 && m_ArmyUnits[i].MaxBuild > l_Trainable)
                        {
                            l_UnitIndex = i;
                            l_Trainable = m_ArmyUnits[i].MaxBuild;
                        }
                    }
                }
            }
            return l_UnitIndex;
        }

        public int getMostTrainableNavyUnitInQueue(int p_Favor)
        {
            int l_UnitIndex = -1;
            int l_Trainable = -1;

            for (int i = 0; i < m_ArmyUnits.Count; i++)
            {
                if (!m_ArmyUnits[i].IsFromBarracks)
                {
                    if (m_ArmyUnits[i].QueueBot > 0)
                    {
                        if (m_ArmyUnits[i].QueueBot - (m_ArmyUnits[i].TotalAmount + m_ArmyUnits[i].QueueGame) > 0 && m_ArmyUnits[i].MaxBuild > l_Trainable)
                        {
                            l_UnitIndex = i;
                            l_Trainable = m_ArmyUnits[i].MaxBuild;
                        }
                    }
                }
            }
            return l_UnitIndex;
        }

        public string getFarmersName()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_FarmersName = "";

            try
            {
                for (int i = 0; i < m_Farmers.Count; i++)
                {
                    l_FarmersName += m_Farmers[i].Name + ";";
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getFarmersName(): " + e.Message);
            }

            return l_FarmersName;
        }

        public string getFarmersRelation()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_FarmersRelation = "";

            try
            {
                for (int i = 0; i < m_Farmers.Count; i++)
                {
                    l_FarmersRelation += m_Farmers[i].RelationStatus.ToString() + ";";
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getFarmersRelation(): " + e.Message);
            }

            return l_FarmersRelation;
        }

        public string getFarmersLimitReached()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_FarmersLimit = "";

            try
            {
                for (int i = 0; i < m_Farmers.Count; i++)
                {
                    l_FarmersLimit += m_Farmers[i].FarmersLimitReached.ToString() + ";";
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getFarmersLimitReached(): " + e.Message);
            }

            return l_FarmersLimit;
        }

        public string getFarmersLootTime()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_FarmersLootTime = "";

            try
            {
                for (int i = 0; i < m_Farmers.Count; i++)
                {
                    l_FarmersLootTime += m_Farmers[i].LootTimerHuman + ";";
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getFarmersLootTime(): " + e.Message);
            }

            return l_FarmersLootTime;
        }

        public string getFarmersMood()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_FarmersMood = "";

            try
            {
                for (int i = 0; i < m_Farmers.Count; i++)
                {
                    l_FarmersMood += m_Farmers[i].Mood + ";";
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getFarmersMood(): " + e.Message);
            }

            return l_FarmersMood;
        }

        public string getFarmersLimit()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_FarmersLimit = "";

            try
            {
                for (int i = 0; i < m_Farmers.Count; i++)
                {
                    l_FarmersLimit += m_Farmers[i].Limit + ";";
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getFarmersLimit(): " + e.Message);
            }

            return l_FarmersLimit;
        }

        public int[] getLootableFarmer(string p_ServerTime, int p_FarmersStartIndex)
        {
            int l_FarmersIndex = -1;
            int l_FarmersIndexNormal = -1;
            int j = -1;

            for (int i = p_FarmersStartIndex; i < m_Farmers.Count; i++)
            {
                j = (int)m_FarmersSequence[i];
                if (m_Farmers[j].isLootable(p_ServerTime) && m_Farmers[j].Enabled)
                {
                    l_FarmersIndex = j;
                    l_FarmersIndexNormal = i;
                    break;
                }
            }

            return new int[] { l_FarmersIndex, l_FarmersIndexNormal };
        }

        public void randomizeFarmers()
        {
            ArrayList l_List = new ArrayList();
            Random l_Random = new Random();

            while (m_FarmersSequence.Count > 0)
            {
                int l_Pos = l_Random.Next(m_FarmersSequence.Count);
                l_List.Add(m_FarmersSequence[l_Pos]);
                m_FarmersSequence.RemoveAt(l_Pos);
            }

            m_FarmersSequence = l_List;
        }

        public int randomizedFarmer(int p_Index)
        {
            return (int)m_FarmersSequence[p_Index];            
        }

        public bool isMilitiaNeeded(string p_ServerTime)
        {
            int l_Earliest = -1;
            bool l_Needed = false;
            int l_Arrival_eta = -1;

            for (int i = 0; i < Movements.Count; i++)
            {
                if (Movements[i].Incoming_attack && Movements[i].Type.Equals("attack_incoming"))
                {
                    l_Arrival_eta = int.Parse(Movements[i].Arrival_at) - int.Parse(p_ServerTime);
                    if (l_Earliest == -1)
                        l_Earliest = l_Arrival_eta;
                    else if (l_Earliest > l_Arrival_eta)
                        l_Earliest = l_Arrival_eta;
                }
            }

            if (l_Earliest != -1)
                l_Needed = ((m_MilitiaTrigger*60) >= l_Earliest);

            return l_Needed;
        }

        public bool isStorageFull()
        {
            return (this.Storage == m_Wood && m_Wood == m_Stone && m_Stone == m_Iron && this.Storage != 0);
        }

        public string getResourcesString()
        {
            return Wood.ToString() + ";" + Stone.ToString() + ";" + Iron.ToString() + ";";
        }

        public string getCurrentGodLocal()
        {
            Settings l_Settings = Settings.Instance;

            string l_GodLocal = "no god";
            switch (m_God)
            {

                case "zeus":
                    l_GodLocal = l_Settings.LocalZeus;
                    break;
                case "poseidon":
                    l_GodLocal = l_Settings.LocalPoseidon;
                    break;
                case "hera":
                    l_GodLocal = l_Settings.LocalHera;
                    break;
                case "athena":
                    l_GodLocal = l_Settings.LocalAthena;
                    break;
                case "hades":
                    l_GodLocal = l_Settings.LocalHades;
                    break;
                case "artemis":
                    l_GodLocal = l_Settings.LocalArtemis;
                    break;
            }
            return l_GodLocal;
        }

        public bool isPowerActive(string p_Power)
        {
            return m_CastedPowers.Contains(p_Power);
        }

        public int getNumberOfIncomingAttacks()
        {
            int l_IncomingAttacks = 0;

            for (int i = 0; i < Movements.Count; i++)
            {
                if (Movements[i].Incoming_attack && Movements[i].Type.Equals("attack"))
                {
                    l_IncomingAttacks++;
                }
            }

            return l_IncomingAttacks;
        }

        public string getIncomingAttackMovement()
        {
            Parser l_Parser = Parser.Instance;
            Settings l_Settings = Settings.Instance;

            string l_IncomingAttackMovement = "";

            for (int i = 0; i < Movements.Count; i++)
            {
                //if (Movements[i].Incoming && Movements[i].Incoming_attack)
                if (Movements[i].Incoming_attack && Movements[i].Type.Equals("attack_incoming"))
                {
                    if(l_IncomingAttackMovement.Length > 0)
                        l_IncomingAttackMovement = l_IncomingAttackMovement + Movements[i].OriginTownName + " @ " + l_Parser.unixToHumanTime(Movements[i].Arrival_at) + Environment.NewLine;
                    else
                        l_IncomingAttackMovement = Movements[i].OriginTownName + " @ " + l_Parser.unixToHumanTime(Movements[i].Arrival_at) + Environment.NewLine;
                }
            }

            if (l_IncomingAttackMovement.Length > 0)
                return "@" + Name + " (" + getCurrentGodLocal() + ")" + Environment.NewLine + l_IncomingAttackMovement;
            else
                return l_IncomingAttackMovement;
        }

        public string getIncomingSupportMovement()
        {
            Parser l_Parser = Parser.Instance;
            Settings l_Settings = Settings.Instance;

            string l_IncomingSupportMovement = "";

            for (int i = 0; i < Movements.Count; i++)
            {
                if (Movements[i].Incoming && Movements[i].Type.Equals("support"))
                {
                    if (l_IncomingSupportMovement.Length > 0)
                        l_IncomingSupportMovement = l_IncomingSupportMovement + Movements[i].OriginTownName + " @ " + l_Parser.unixToHumanTime(Movements[i].Arrival_at) + Environment.NewLine;
                    else
                        l_IncomingSupportMovement = Movements[i].OriginTownName + " @ " + l_Parser.unixToHumanTime(Movements[i].Arrival_at) + Environment.NewLine;
                }
            }
            return l_IncomingSupportMovement;
        }

        public string getIncomingAttackMovementForum()
        {
            Parser l_Parser = Parser.Instance;
            Settings l_Settings = Settings.Instance;

            string l_IncomingAttackMovement = "";

            for (int i = 0; i < Movements.Count; i++)
            {
                //if (Movements[i].Incoming && Movements[i].Incoming_attack)
                if (Movements[i].Incoming_attack && Movements[i].Type.Equals("attack"))
                {
                    if (l_IncomingAttackMovement.Length > 0)
                        l_IncomingAttackMovement = l_IncomingAttackMovement + "[town]" + TownID + "[/town] by [town]" + Movements[i].OriginTownID + "[/town]" + " @ " + l_Parser.unixToHumanTime(Movements[i].Arrival_at) + Environment.NewLine;
                    else
                        l_IncomingAttackMovement = "[town]" + TownID + "[/town] by [town]" + Movements[i].OriginTownID + "[/town]" + " @ " + l_Parser.unixToHumanTime(Movements[i].Arrival_at) + Environment.NewLine;
                }
            }

            if (l_IncomingAttackMovement.Length > 0)
                return "[town]" + TownID + "[/town] (" + getCurrentGodLocal() + ")" + Environment.NewLine + l_IncomingAttackMovement;
            else
                return l_IncomingAttackMovement;
        }

        public string getIncomingSupportMovementForum()
        {
            Parser l_Parser = Parser.Instance;
            Settings l_Settings = Settings.Instance;

            string l_IncomingSupportMovement = "";

            for (int i = 0; i < Movements.Count; i++)
            {
                if (Movements[i].Incoming && Movements[i].Type.Equals("support"))
                {
                    if (l_IncomingSupportMovement.Length > 0)
                        l_IncomingSupportMovement = l_IncomingSupportMovement + "[town]" + TownID + "[/town] by [town]" + Movements[i].OriginTownID + "[/town]" + " @ " + l_Parser.unixToHumanTime(Movements[i].Arrival_at) + Environment.NewLine;
                    else
                        l_IncomingSupportMovement = "[town]" + TownID + "[/town] by [town]" + Movements[i].OriginTownID + "[/town]" + " @ " + l_Parser.unixToHumanTime(Movements[i].Arrival_at) + Environment.NewLine;
                }
            }
            return l_IncomingSupportMovement;
        }

        public string getTotalLandUnitPopulation()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            int l_TotalLandUnitPopulation = 0;

            try
            {
                for (int i = 0; i < m_ArmyUnits.Count; i++)
                {
                    if (m_ArmyUnits[i].IsFromBarracks)
                        l_TotalLandUnitPopulation += (m_ArmyUnits[i].QueueBot * m_ArmyUnits[i].Population);
                }
                l_TotalLandUnitPopulation -= (m_ArmyUnits[getUnitIndex("manticore")].QueueBot * m_ArmyUnits[getUnitIndex("manticore")].Population);
                l_TotalLandUnitPopulation -= (m_ArmyUnits[getUnitIndex("pegasus")].QueueBot * m_ArmyUnits[getUnitIndex("pegasus")].Population);
                l_TotalLandUnitPopulation -= (m_ArmyUnits[getUnitIndex("harpy")].QueueBot * m_ArmyUnits[getUnitIndex("harpy")].Population);
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getTotalLandUnitPopulation(): " + e.Message);
            }
            return l_TotalLandUnitPopulation.ToString();
        }

        public string getTotalTransportCapacity()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            int l_TotalTransportCapacity = 0;
            int l_TotalTransportCapacityWithResearch = 0;

            try
            {
                l_TotalTransportCapacity += m_ArmyUnits[getUnitIndex("big_transporter")].QueueBot * m_ArmyUnits[getUnitIndex("big_transporter")].Capacity;
                l_TotalTransportCapacity += m_ArmyUnits[getUnitIndex("small_transporter")].QueueBot * m_ArmyUnits[getUnitIndex("small_transporter")].Capacity;
                l_TotalTransportCapacityWithResearch += m_ArmyUnits[getUnitIndex("big_transporter")].QueueBot * (m_ArmyUnits[getUnitIndex("big_transporter")].Capacity + 6);
                l_TotalTransportCapacityWithResearch += m_ArmyUnits[getUnitIndex("small_transporter")].QueueBot * (m_ArmyUnits[getUnitIndex("small_transporter")].Capacity + 6);
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getUnitTrainingTarget(): " + e.Message);
            }
            return l_TotalTransportCapacity.ToString() + " (" + l_TotalTransportCapacityWithResearch.ToString() + ")";
        }

        public int getTotalUnitPopulation()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            int l_TotalUnitPopulation = 0;

            try
            {
                for (int i = 0; i < ArmyUnits.Count; i++)
                {
                    l_TotalUnitPopulation += (m_ArmyUnits[i].QueueBot * m_ArmyUnits[i].Population);
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getTotalUnitPopulation(): " + e.Message);
            }
            return l_TotalUnitPopulation;
        }

        public int getTotalBuildingPopulation()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            int l_TotalBuildingPopulation = 0;

            try
            {
                for (int i = 0; i < Buildings.Count; i++)
                {
                    l_TotalBuildingPopulation += Buildings[i].getPopulationAtTargetLevel();
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in getTotalBuildingPopulation(): " + e.Message);
            }
            //Add Agora
            l_TotalBuildingPopulation += 1;
            return l_TotalBuildingPopulation;
        }

        public string getSelectedFarmers()
        {
            string l_Selected = "";

            try
            {
                if (Farmers.Count > 0)
                {
                    l_Selected = Farmers[0].Enabled.ToString() + ";" + Farmers[1].Enabled.ToString() + ";" + Farmers[2].Enabled.ToString() + ";" + Farmers[3].Enabled.ToString() + ";" +
                        Farmers[4].Enabled.ToString() + ";" + Farmers[5].Enabled.ToString() + ";" + Farmers[6].Enabled.ToString() + ";" + Farmers[7].Enabled.ToString() + ";";
                }
                else
                {
                    l_Selected = "False;False;False;False;False;False;False;False;";
                }
                
            }
            catch (Exception)
            {
            }
            return l_Selected;
        }

        public void setSelectedFarmers(string p_SelectedFarmers)
        {
            try
            {
                if (Farmers.Count > 0)
                {
                    string[] l_SelectedFarmers = new string[] { "", "" };//dynamic array
                    l_SelectedFarmers = p_SelectedFarmers.Split(';');
                    Farmers[0].Enabled = l_SelectedFarmers[0].Equals("True");
                    Farmers[1].Enabled = l_SelectedFarmers[1].Equals("True");
                    Farmers[2].Enabled = l_SelectedFarmers[2].Equals("True");
                    Farmers[3].Enabled = l_SelectedFarmers[3].Equals("True");
                    Farmers[4].Enabled = l_SelectedFarmers[4].Equals("True");
                    Farmers[5].Enabled = l_SelectedFarmers[5].Equals("True");
                    Farmers[6].Enabled = l_SelectedFarmers[6].Equals("True");
                    Farmers[7].Enabled = l_SelectedFarmers[7].Equals("True");
                }
            }
            catch (Exception)
            {

            }
        }

        public int getLowestEnabledFarmersMood()
        {
            int l_Mood = 100;

            for (int i = 0; i < Farmers.Count; i++)
            {
                if (Farmers[i].Enabled && Farmers[i].RelationStatus && Farmers[i].Mood < l_Mood)
                    l_Mood = Farmers[i].Mood;
            }
            return l_Mood;
        }

        public string getEnabledFarmersID()
        {
            string l_Farmers = "";

            for (int i = 0; i < Farmers.Count; i++)
            {
                if (Farmers[i].Enabled && Farmers[i].RelationStatus)
                    l_Farmers += Farmers[i].ID + ",";
            }

            if (l_Farmers.EndsWith(","))
                l_Farmers = l_Farmers.Substring(0, l_Farmers.Length - 1);

            return l_Farmers;
        }

        public string getEnabledFarmersName()
        {
            string l_Farmers = "";

            for (int i = 0; i < Farmers.Count; i++)
            {
                if (Farmers[i].Enabled && Farmers[i].RelationStatus)
                    l_Farmers += Farmers[i].Name + ",";
            }

            if (l_Farmers.EndsWith(","))
                l_Farmers = l_Farmers.Substring(0, l_Farmers.Length - 1);

            return l_Farmers;
        }

        public bool isEnabledFarmersReady(string p_ServerTime)
        {
            bool l_Ready = true;

            for (int i = 0; i < Farmers.Count; i++)
            {
                if (Farmers[i].Enabled && Farmers[i].RelationStatus)//Check only enabled farmers
                    if (!Farmers[i].isLootable(p_ServerTime) || Farmers[i].FarmersLimitReached)
                        l_Ready = false;
            }
            return l_Ready;
        }

        public void resetFarmersDailyLimit()
        {
            for (int i = 0; i < Farmers.Count; i++)
            {
                Farmers[i].FarmersLimitReached = false;
            }
        }

        public double getTotalNeededResources()
        {
            double l_TotalNeededResources = 0;
            if ((double)((Storage * TradePercentageWarehouse) / 100) - Wood > 0)
            {
                l_TotalNeededResources += (double)((Storage * TradePercentageWarehouse) / 100) - Wood;
            }
            if ((double)((Storage * TradePercentageWarehouse) / 100) - Stone > 0)
            {
                l_TotalNeededResources += (double)((Storage * TradePercentageWarehouse) / 100) - Stone;
            }
            if ((double)((Storage * TradePercentageWarehouse) / 100) - Iron > 0)
            {
                l_TotalNeededResources += (double)((Storage * TradePercentageWarehouse) / 100) - Iron;
            }
            return l_TotalNeededResources;
        }

        public int getTotalAvailableResources()
        {
            int l_TotalAvailableResources = 0;
            if ((Wood - TradeRemainingResources) > 0)
            {
                l_TotalAvailableResources += (Wood - TradeRemainingResources);
            }
            if ((Stone - TradeRemainingResources) > 0)
            {
                l_TotalAvailableResources += (Stone - TradeRemainingResources);
            }
            if ((Iron - TradeRemainingResources) > 0)
            {
                l_TotalAvailableResources += (Iron - TradeRemainingResources);
            }
            return l_TotalAvailableResources;
        }

        public double getDistance(string l_IslandX, string l_IslandY)
        {
            double l_Distance = Math.Sqrt(
                    Math.Pow(Math.Abs(int.Parse(l_IslandX) - int.Parse(IslandX)), 2) +
                    Math.Pow(Math.Abs(int.Parse(l_IslandY) - int.Parse(IslandY)), 2));
            return l_Distance;
        }
    }
}
