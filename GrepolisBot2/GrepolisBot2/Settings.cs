using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GrepolisBot2
{
    sealed class Settings
    {
        //Singleton
        private static readonly Settings m_Instance = new Settings();

        //General
        private string m_GenUserName = "name";
        private string m_GenPassword = "password";
        private string m_GenMainServer = "en.grepolis.com";
        private string m_GenServer = "en1.grepolis.com";
        private bool m_GenIsHeroWorld = false;
        private bool m_GenToTryIcon = true;
        
        //Building/training queue
        private int m_QueueTimer = 20;
        private int m_MinUnitQueuePop = 0;
        private int m_QueueLimit = 7;
        private bool m_AdvancedQueue = false;
        private int m_BuildFarmBelow = 10;

        //Trading
        private int m_TradeTimer = 10;
        
        //E-Mail
        private string m_MailAddress = "name@domain.com";
        private bool m_MailNotify = true;
        private bool m_MailIncludeSupport = false;
        private string m_MailUsername = "name@gmail.com";
        private string m_MailPassword = "password";
        private string m_MailServer = "smtp.gmail.com";
        private int m_MailPort = 587;

        //Farmer
        private string m_FarmerScheduler = "True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;";
        private int m_FarmerMaxResources = 50000;
        private bool m_FarmerRandomize = false;

        //Reconnect
        private int m_RecTimerReconnectMin = 2;
        private int m_RecTimerReconnectMax = 20;
        private bool m_RecMaxReconnectsEnabled = true;
        private int m_RecMaxReconnects = 5;
        private bool m_RecForcedReconnects = true;
        private int m_RecMinForcedReconnect = 5;
        private int m_RecMaxForcedReconnect = 10;

        //Premium
        private bool m_PreUseGold = false;
        private bool m_PreUseFarmAllFeature = false;

        //Sound warnings
        private bool m_SoundIncomingAttack = true;
        private string m_SoundAttackWarningSoundLocation = "attack.wav";
        private bool m_SoundCaptchaWarning = true;
        private string m_SoundCaptchaWarningSoundLocation = "captcha.wav";
        private string m_SoundSaveLocation = "save.wav";

        //Advanced
        private int m_AdvMinTimerRefresh = 2;
        private int m_AdvMaxTimerRefresh = 5;
        private int m_AdvMinDelayRequests = 1200;
        private int m_AdvMaxDelayRequests = 1200;
        private int m_AdvMinDelayFarmers = 1500;
        private int m_AdvMaxDelayFarmers = 3000;
        private int m_AdvFarmerLootLag = 30;
        private int m_AdvTimeout = 20;
        private bool m_AdvSchedulerBot = false;
        private bool m_AdvAutoStart = false;
        private bool m_AdvAutoPause = false;
        private bool m_AdvDebugMode = true;
        private bool m_AdvOutputAllMode = false;

        private string m_AdvDebugFile = "debug.txt";//Not visible on GUI
        private string m_AdvLogFile = "log.txt";//Not visible on GUI
        private string m_AdvErrorFile = "error.txt";//Not visible on GUI
        private string m_AdvProgramSettingsFile = "Config/settings.xml";//Not visible on GUI
        private string m_AdvTownsSettingsFile = "Config/towns.xml";//Not visible on GUI
        private string m_AdvFarmersSettingsFile = "Config/farmers.xml";//Not visible on GUI
        private string m_AdvTemplatesUnitQueueSettingsFile = "Config/templatesunitqueue.xml";//Not visible on GUI
        private string m_AdvTemplatesBuildingQueueSettingsFile = "Config/templatesbuildingqueue.xml";//Not visible on GUI
        private string m_AdvUnitQueueSettingsFile = "Config/unitqueue.xml";//Not visible on GUI
        private string m_AdvCookiesFile = "Config/cookies.txt";//Not visible on GUI
        private string m_AdvOverviewFile = "Overview/index.html";//Not visible on GUI

        private string m_AdvUserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";//Not vissible on GUI
        private string m_AdvNotesFile = "Config/notes.txt";//Not visible on GUI
        private string m_AdvResponseDir = "Response/";//Not visible on GUI

        //GUI
        private Color m_GUIBuildingLevelTargetColor = Color.GreenYellow;
        private bool m_GUIBuildingTooltipsEnabled = true;
        private int m_GUINotificationSize = 500;
        private int m_GUILogSize = 500;
        private bool m_GUITownTabsLinked = false;
        private bool m_GUIReconnectTimerTitleBar = false;

        //Translations
        private string m_LocalZeus = "zeus";//Loaded from server at startup
        private string m_LocalPoseidon = "poseidon";//Loaded from server at startup
        private string m_LocalHera = "hera";//Loaded from server at startup
        private string m_LocalAthena = "athena";//Loaded from server at startup
        private string m_LocalHades = "hades";//Loaded from server at startup
        private string m_LocalArtemis = "artemis";//Loaded from server at startup

        //Captcha
        private string m_CaptchaApikey = "";
        private bool m_CaptchaSolverEnabled = true;
        private bool m_CaptchaSelfSolve = false;
        private int m_CaptchaPriority = 0;
        private int m_CaptchaMinWorkers = 2;
        private int m_CaptchaMinWorkersALT = 10;
        private int m_CaptchaMaxQueue = 50;
        private int m_CaptchaExtraDelay = 120;

        //Feature control
        private bool m_MasterBuildingQueue = true;
        private bool m_MasterUnitQueue = true;
        private bool m_MasterCulture = true;
        private bool m_MasterFarmers = true;
        private bool m_MasterTrade = true;

        //Others
        private string m_ServerTimeOffset = "0";//Loaded from server at startup
        private string m_LocaleTimeOffset = "0";//Loaded from server at startup

//-->Constructor

        private Settings()
        {

        }

//-->Attributes

        //Singleton
        public static Settings Instance
        {
            get
            {
                return m_Instance;
            }
        }

        //General
        public string GenUserName
        {
            get { return m_GenUserName; }
            set { m_GenUserName = value; }
        }

        public string GenPassword
        {
            get { return m_GenPassword; }
            set { m_GenPassword = value; }
        }

        public string GenMainServer
        {
            get { return m_GenMainServer; }
            set { m_GenMainServer = value; }
        }

        public string GenServer
        {
            get { return m_GenServer; }
            set { m_GenServer = value; }
        }

        public bool GenIsHeroWorld
        {
            get { return m_GenIsHeroWorld; }
            set { m_GenIsHeroWorld = value; }
        }

        public bool GenToTryIcon
        {
            get { return m_GenToTryIcon; }
            set { m_GenToTryIcon = value; }
        }

        //Building/training queue
        public int QueueTimer
        {
            get { return m_QueueTimer; }
            set { m_QueueTimer = value; }
        }

        public int MinUnitQueuePop
        {
            get { return m_MinUnitQueuePop; }
            set { m_MinUnitQueuePop = value; }
        }

        public int QueueLimit
        {
            get { return m_QueueLimit; }
            set { m_QueueLimit = value; }
        }

        public bool AdvancedQueue
        {
            get { return m_AdvancedQueue; }
            set { m_AdvancedQueue = value; }
        }

        public int BuildFarmBelow
        {
            get { return m_BuildFarmBelow; }
            set { m_BuildFarmBelow = value; }
        }

        //Trade
        public int TradeTimer
        {
            get { return m_TradeTimer; }
            set { m_TradeTimer = value; }
        }

        //E-Mail
        public string MailAddress
        {
            get { return m_MailAddress; }
            set { m_MailAddress = value; }
        }

        public bool MailNotify
        {
            get { return m_MailNotify; }
            set { m_MailNotify = value; }
        }

        public bool MailIncludeSupport
        {
            get { return m_MailIncludeSupport; }
            set { m_MailIncludeSupport = value; }
        }

        public string MailUsername
        {
            get { return m_MailUsername; }
            set { m_MailUsername = value; }
        }

        public string MailPassword
        {
            get { return m_MailPassword; }
            set { m_MailPassword = value; }
        }

        public string MailServer
        {
            get { return m_MailServer; }
            set { m_MailServer = value; }
        }

        public int MailPort
        {
            get { return m_MailPort; }
            set { m_MailPort = value; }
        }

        //Farmers
        public string FarmerScheduler
        {
            get { return m_FarmerScheduler; }
            set { m_FarmerScheduler = value; }
        }

        public int FarmerMaxResources
        {
            get { return m_FarmerMaxResources; }
            set { m_FarmerMaxResources = value; }
        }

        public bool FarmerRandomize
        {
            get { return m_FarmerRandomize; }
            set { m_FarmerRandomize = value; }
        }

        //Reconnect
        public int RecTimerReconnectMin
        {
            get { return m_RecTimerReconnectMin; }
            set { m_RecTimerReconnectMin = value; }
        }

        public int RecTimerReconnectMax
        {
            get { return m_RecTimerReconnectMax; }
            set { m_RecTimerReconnectMax = value; }
        }

        public bool RecMaxReconnectsEnabled
        {
            get { return m_RecMaxReconnectsEnabled; }
            set { m_RecMaxReconnectsEnabled = value; }
        }

        public int RecMaxReconnects
        {
            get { return m_RecMaxReconnects; }
            set { m_RecMaxReconnects = value; }
        }

        public bool RecForcedReconnects
        {
            get { return m_RecForcedReconnects; }
            set { m_RecForcedReconnects = value; }
        }

        public int RecMinForcedReconnect
        {
            get { return m_RecMinForcedReconnect; }
            set { m_RecMinForcedReconnect = value; }
        }

        public int RecMaxForcedReconnect
        {
            get { return m_RecMaxForcedReconnect; }
            set { m_RecMaxForcedReconnect = value; }
        }

        //Premium
        public bool PreUseGold
        {
            get { return m_PreUseGold; }
            set { m_PreUseGold = value; }
        }

        public bool PreUseFarmAllFeature
        {
            get { return m_PreUseFarmAllFeature; }
            set { m_PreUseFarmAllFeature = value; }
        }

        //Sound warnings
        public bool SoundIncomingAttack
        {
            get { return m_SoundIncomingAttack; }
            set { m_SoundIncomingAttack = value; }
        }

        public string SoundAttackWarningSoundLocation
        {
            get { return m_SoundAttackWarningSoundLocation; }
            set { m_SoundAttackWarningSoundLocation = value; }
        }

        public bool SoundCaptchaWarning
        {
            get { return m_SoundCaptchaWarning; }
            set { m_SoundCaptchaWarning = value; }
        }

        public string SoundCaptchaWarningSoundLocation
        {
            get { return m_SoundCaptchaWarningSoundLocation; }
            set { m_SoundCaptchaWarningSoundLocation = value; }
        }

        public string SoundSaveLocation
        {
            get { return m_SoundSaveLocation; }
            set { m_SoundSaveLocation = value; }
        }

        //Advanced
        public int AdvMinTimerRefresh
        {
            get { return m_AdvMinTimerRefresh; }
            set { m_AdvMinTimerRefresh = value; }
        }

        public int AdvMaxTimerRefresh
        {
            get { return m_AdvMaxTimerRefresh; }
            set { m_AdvMaxTimerRefresh = value; }
        }

        public bool AdvSchedulerBot
        {
            get { return m_AdvSchedulerBot; }
            set { m_AdvSchedulerBot = value; }
        }

        public bool AdvAutoStart
        {
            get { return m_AdvAutoStart; }
            set { m_AdvAutoStart = value; }
        }

        public bool AdvAutoPause
        {
            get { return m_AdvAutoPause; }
            set { m_AdvAutoPause = value; }
        }

        public bool AdvDebugMode
        {
            get { return m_AdvDebugMode; }
            set { m_AdvDebugMode = value; }
        }

        public bool AdvOutputAllMode
        {
            get { return m_AdvOutputAllMode; }
            set { m_AdvOutputAllMode = value; }
        }

        public int AdvMinDelayRequests
        {
            get { return m_AdvMinDelayRequests; }
            set { m_AdvMinDelayRequests = value; }
        }

        public int AdvMaxDelayRequests
        {
            get { return m_AdvMaxDelayRequests; }
            set { m_AdvMaxDelayRequests = value; }
        }

        public int AdvMinDelayFarmers
        {
            get { return m_AdvMinDelayFarmers; }
            set { m_AdvMinDelayFarmers = value; }
        }

        public int AdvMaxDelayFarmers
        {
            get { return m_AdvMaxDelayFarmers; }
            set { m_AdvMaxDelayFarmers = value; }
        }

        public int AdvFarmerLootLag
        {
            get { return m_AdvFarmerLootLag; }
            set { m_AdvFarmerLootLag = value; }
        }

        public int AdvTimeout
        {
            get { return m_AdvTimeout; }
            set { m_AdvTimeout = value; }
        }

        public string AdvDebugFile
        {
            get { return m_AdvDebugFile; }
            set { m_AdvDebugFile = value; }
        }

        public string AdvLogFile
        {
            get { return m_AdvLogFile; }
            set { m_AdvLogFile = value; }
        }

        public string AdvErrorFile
        {
            get { return m_AdvErrorFile; }
            set { m_AdvErrorFile = value; }
        }

        public string AdvProgramSettingsFile
        {
            get { return m_AdvProgramSettingsFile; }
            set { m_AdvProgramSettingsFile = value; }
        }

        public string AdvTownsSettingsFile
        {
            get { return m_AdvTownsSettingsFile; }
            set { m_AdvTownsSettingsFile = value; }
        }

        public string AdvFarmersSettingsFile
        {
            get { return m_AdvFarmersSettingsFile; }
            set { m_AdvFarmersSettingsFile = value; }
        }

        public string AdvTemplatesUnitQueueSettingsFile
        {
            get { return m_AdvTemplatesUnitQueueSettingsFile; }
            set { m_AdvTemplatesUnitQueueSettingsFile = value; }
        }

        public string AdvTemplatesBuildingQueueSettingsFile
        {
            get { return m_AdvTemplatesBuildingQueueSettingsFile; }
            set { m_AdvTemplatesBuildingQueueSettingsFile = value; }
        }

        public string AdvUnitQueueSettingsFile
        {
            get { return m_AdvUnitQueueSettingsFile; }
            set { m_AdvUnitQueueSettingsFile = value; }
        }

        public string AdvCookiesFile
        {
            get { return m_AdvCookiesFile; }
            set { m_AdvCookiesFile = value; }
        }

        public string AdvOverviewFile
        {
            get { return m_AdvOverviewFile; }
            set { m_AdvOverviewFile = value; }
        }

        public string AdvUserAgent
        {
            get { return m_AdvUserAgent; }
            set { m_AdvUserAgent = value; }
        }

        public string AdvNotesFile
        {
            get { return m_AdvNotesFile; }
            set { m_AdvNotesFile = value; }
        }

        public string AdvResponseDir
        {
            get { return m_AdvResponseDir; }
            set { m_AdvResponseDir = value; }
        }

        //GUI
        public Color GUIBuildingLevelTargetColor
        {
            get { return m_GUIBuildingLevelTargetColor; }
            set { m_GUIBuildingLevelTargetColor = value; }
        }

        public bool GUIBuildingTooltipsEnabled
        {
            get { return m_GUIBuildingTooltipsEnabled; }
            set { m_GUIBuildingTooltipsEnabled = value; }
        }

        public int GUINotificationSize
        {
            get { return m_GUINotificationSize; }
            set { m_GUINotificationSize = value; }
        }

        public int GUILogSize
        {
            get { return m_GUILogSize; }
            set { m_GUILogSize = value; }
        }

        public bool GUITownTabsLinked
        {
            get { return m_GUITownTabsLinked; }
            set { m_GUITownTabsLinked = value; }
        }

        public bool GUIReconnectTimerTitleBar
        {
            get { return m_GUIReconnectTimerTitleBar; }
            set { m_GUIReconnectTimerTitleBar = value; }
        }

        //Translations
        public string LocalZeus
        {
            get { return m_LocalZeus; }
            set { m_LocalZeus = value; }
        }

        public string LocalPoseidon
        {
            get { return m_LocalPoseidon; }
            set { m_LocalPoseidon = value; }
        }

        public string LocalHera
        {
            get { return m_LocalHera; }
            set { m_LocalHera = value; }
        }

        public string LocalAthena
        {
            get { return m_LocalAthena; }
            set { m_LocalAthena = value; }
        }

        public string LocalHades
        {
            get { return m_LocalHades; }
            set { m_LocalHades = value; }
        }

        public string LocalArtemis
        {
            get { return m_LocalArtemis; }
            set { m_LocalArtemis = value; }
        }

        //Captcha
        public string CaptchaApikey
        {
            get { return m_CaptchaApikey; }
            set { m_CaptchaApikey = value; }
        }

        public bool CaptchaSolverEnabled
        {
            get { return m_CaptchaSolverEnabled; }
            set { m_CaptchaSolverEnabled = value; }
        }

        public bool CaptchaSelfSolve
        {
            get { return m_CaptchaSelfSolve; }
            set { m_CaptchaSelfSolve = value; }
        }

        public int CaptchaPriority
        {
            get { return m_CaptchaPriority; }
            set { m_CaptchaPriority = value; }
        }

        public int CaptchaMinWorkers
        {
            get { return m_CaptchaMinWorkers; }
            set { m_CaptchaMinWorkers = value; }
        }

        public int CaptchaMinWorkersALT
        {
            get { return m_CaptchaMinWorkersALT; }
            set { m_CaptchaMinWorkersALT = value; }
        }

        public int CaptchaMaxQueue
        {
            get { return m_CaptchaMaxQueue; }
            set { m_CaptchaMaxQueue = value; }
        }

        public int CaptchaExtraDelay
        {
            get { return m_CaptchaExtraDelay; }
            set { m_CaptchaExtraDelay = value; }
        }

        //Feature control
        public bool MasterBuildingQueue
        {
            get { return m_MasterBuildingQueue; }
            set { m_MasterBuildingQueue = value; }
        }

        public bool MasterUnitQueue
        {
            get { return m_MasterUnitQueue; }
            set { m_MasterUnitQueue = value; }
        }

        public bool MasterCulture
        {
            get { return m_MasterCulture; }
            set { m_MasterCulture = value; }
        }

        public bool MasterFarmers
        {
            get { return m_MasterFarmers; }
            set { m_MasterFarmers = value; }
        }

        public bool MasterTrade
        {
            get { return m_MasterTrade; }
            set { m_MasterTrade = value; }
        }

        //Others
        public string ServerTimeOffset
        {
            get { return m_ServerTimeOffset; }
            set { m_ServerTimeOffset = value; }
        }

        public string LocaleTimeOffset
        {
            get { return m_LocaleTimeOffset; }
            set { m_LocaleTimeOffset = value; }
        }

//--Methods

    }
}
