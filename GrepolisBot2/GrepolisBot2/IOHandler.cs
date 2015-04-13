using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;

namespace GrepolisBot2
{
    sealed class IOHandler
    {
        //Singleton
        private static readonly IOHandler m_Instance = new IOHandler();

//-->Constructor

        private IOHandler()
        {

        }

//-->Attributes

        public static IOHandler Instance
        {
            get
            {
                return m_Instance;
            }
        }

//--Methods

        public void debug(string p_Message)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                TextWriter l_TwDebug = new StreamWriter(l_Settings.AdvDebugFile, true);
                l_TwDebug.WriteLine(DateTime.Now.ToLocalTime() + " " + p_Message);
                l_TwDebug.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in debug(): " + e.Message);
            }
        }

        public void log(string p_Message)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                TextWriter l_TwLog = new StreamWriter(l_Settings.AdvLogFile, true);
                l_TwLog.WriteLine(p_Message);
                l_TwLog.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in log(): " + e.Message);
            }
        }

        public void writeCookies(string p_Cookies)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                TextWriter l_TwCookies = new StreamWriter(l_Settings.AdvCookiesFile, false);
                l_TwCookies.WriteLine(p_Cookies);
                l_TwCookies.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in writeCookies(): " + e.Message);
            }
        }

        public string loadError()
        {
            Settings l_Settings = Settings.Instance;

            string l_Error = "";

            try
            {
                TextReader l_TrError = new StreamReader(l_Settings.AdvErrorFile);
                l_Error = l_TrError.ReadToEnd();
                l_TrError.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in loadError(): " + e.Message);
            }
            return l_Error;
        }

        public void saveNotes(string p_Notes)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                TextWriter l_TwNotes = new StreamWriter(l_Settings.AdvNotesFile, false);
                l_TwNotes.WriteLine(p_Notes);
                l_TwNotes.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveNotes(): " + e.Message);
            }
        }

        public string loadNotes()
        {
            Settings l_Settings = Settings.Instance;

            string l_Notes = "";

            try
            {
                TextReader l_TrNotes = new StreamReader(l_Settings.AdvNotesFile);
                l_Notes = l_TrNotes.ReadToEnd();
                l_TrNotes.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in loadNotes(): " + e.Message);
            }

            return l_Notes;
        }

        public void saveProgramSettings()
        {
            //Note Modify this when adding new program settings (3/4)
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextWriter l_Writer = new XmlTextWriter(l_Settings.AdvProgramSettingsFile, Encoding.UTF8);
                l_Writer.Formatting = Formatting.Indented;
                l_Writer.WriteStartDocument();
                l_Writer.WriteComment("Program settings.");
                l_Writer.WriteStartElement("Users");//Start of Users
                //General
                l_Writer.WriteStartElement("User");//Start of User
                l_Writer.WriteStartAttribute("GenUserName");
                l_Writer.WriteString(l_Settings.GenUserName);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GenPassword");
                l_Writer.WriteString(l_Settings.GenPassword);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GenMainServer");
                l_Writer.WriteString(l_Settings.GenMainServer);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GenServer");
                l_Writer.WriteString(l_Settings.GenServer);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GenIsHeroWorld");
                l_Writer.WriteString(l_Settings.GenIsHeroWorld.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GenToTryIcon");
                l_Writer.WriteString(l_Settings.GenToTryIcon.ToString());
                l_Writer.WriteEndAttribute();
                //Building/Unit Queue
                l_Writer.WriteStartAttribute("QueueTimer");
                l_Writer.WriteString(l_Settings.QueueTimer.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MinUnitQueuePop");
                l_Writer.WriteString(l_Settings.MinUnitQueuePop.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("QueueLimit");
                l_Writer.WriteString(l_Settings.QueueLimit.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("BuildFarmBelow");
                l_Writer.WriteString(l_Settings.BuildFarmBelow.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvancedQueue");
                l_Writer.WriteString(l_Settings.AdvancedQueue.ToString());
                l_Writer.WriteEndAttribute();
                //Trade
                l_Writer.WriteStartAttribute("TradeTimer");
                l_Writer.WriteString(l_Settings.TradeTimer.ToString());
                l_Writer.WriteEndAttribute();
                //Mail
                l_Writer.WriteStartAttribute("MailAddress");
                l_Writer.WriteString(l_Settings.MailAddress);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MailNotify");
                l_Writer.WriteString(l_Settings.MailNotify.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MailIncludeSupport");
                l_Writer.WriteString(l_Settings.MailIncludeSupport.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MailUsername");
                l_Writer.WriteString(l_Settings.MailUsername);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MailPassword");
                l_Writer.WriteString(l_Settings.MailPassword);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MailServer");
                l_Writer.WriteString(l_Settings.MailServer);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MailPort");
                l_Writer.WriteString(l_Settings.MailPort.ToString());
                l_Writer.WriteEndAttribute();
                //Farmer
                l_Writer.WriteStartAttribute("FarmerScheduler");
                l_Writer.WriteString(l_Settings.FarmerScheduler);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("FarmerMaxResources");
                l_Writer.WriteString(l_Settings.FarmerMaxResources.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("FarmerRandomize");
                l_Writer.WriteString(l_Settings.FarmerRandomize.ToString());
                l_Writer.WriteEndAttribute();
                //Reconnect
                l_Writer.WriteStartAttribute("AdvTimerReconnectMin");
                l_Writer.WriteString(l_Settings.RecTimerReconnectMin.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvTimerReconnectMax");
                l_Writer.WriteString(l_Settings.RecTimerReconnectMax.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMaxReconnectsEnabled");
                l_Writer.WriteString(l_Settings.RecMaxReconnectsEnabled.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMaxReconnects");
                l_Writer.WriteString(l_Settings.RecMaxReconnects.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvForcedReconnects");
                l_Writer.WriteString(l_Settings.RecForcedReconnects.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMinForcedReconnect");
                l_Writer.WriteString(l_Settings.RecMinForcedReconnect.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMaxForcedReconnect");
                l_Writer.WriteString(l_Settings.RecMaxForcedReconnect.ToString());
                l_Writer.WriteEndAttribute();
                //Premium
                l_Writer.WriteStartAttribute("PreUseGold");
                l_Writer.WriteString(l_Settings.PreUseGold.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("PreUseFarmAllFeature");
                l_Writer.WriteString(l_Settings.PreUseFarmAllFeature.ToString());
                l_Writer.WriteEndAttribute();
                //Sound warning
                l_Writer.WriteStartAttribute("SoundIncomingAttack");
                l_Writer.WriteString(l_Settings.SoundIncomingAttack.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("SoundCaptchaWarning");
                l_Writer.WriteString(l_Settings.SoundCaptchaWarning.ToString());
                l_Writer.WriteEndAttribute();
                //Advanced
                l_Writer.WriteStartAttribute("AdvMinTimerRefresh");
                l_Writer.WriteString(l_Settings.AdvMinTimerRefresh.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMaxTimerRefresh");
                l_Writer.WriteString(l_Settings.AdvMaxTimerRefresh.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvSchedulerBot");
                l_Writer.WriteString(l_Settings.AdvSchedulerBot.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvAutoStart");
                l_Writer.WriteString(l_Settings.AdvAutoStart.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvAutoPause");
                l_Writer.WriteString(l_Settings.AdvAutoPause.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvDebugMode");
                l_Writer.WriteString(l_Settings.AdvDebugMode.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvOutputAllMode");
                l_Writer.WriteString(l_Settings.AdvOutputAllMode.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMinDelayRequests");
                l_Writer.WriteString(l_Settings.AdvMinDelayRequests.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMaxDelayRequests");
                l_Writer.WriteString(l_Settings.AdvMaxDelayRequests.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMinDelayFarmers");
                l_Writer.WriteString(l_Settings.AdvMinDelayFarmers.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvMaxDelayFarmers");
                l_Writer.WriteString(l_Settings.AdvMaxDelayFarmers.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvFarmerLootLag");
                l_Writer.WriteString(l_Settings.AdvFarmerLootLag.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvTimeout");
                l_Writer.WriteString(l_Settings.AdvTimeout.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvDebugFile");
                l_Writer.WriteString(l_Settings.AdvDebugFile);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvProgramSettingsFile");
                l_Writer.WriteString(l_Settings.AdvProgramSettingsFile);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvTownsSettingsFile");
                l_Writer.WriteString(l_Settings.AdvTownsSettingsFile);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvUserAgent");
                l_Writer.WriteString(l_Settings.AdvUserAgent);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("AdvNotesFile");
                l_Writer.WriteString(l_Settings.AdvNotesFile);
                l_Writer.WriteEndAttribute();
                //GUI
                l_Writer.WriteStartAttribute("GUIBuildingLevelTargetColor");
                l_Writer.WriteString(l_Settings.GUIBuildingLevelTargetColor.ToArgb().ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GUIBuildingTooltipsEnabled");
                l_Writer.WriteString(l_Settings.GUIBuildingTooltipsEnabled.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GUITownTabsLinked");
                l_Writer.WriteString(l_Settings.GUITownTabsLinked.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GUIReconnectTimerTitleBar");
                l_Writer.WriteString(l_Settings.GUIReconnectTimerTitleBar.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GUINotificationSize");
                l_Writer.WriteString(l_Settings.GUINotificationSize.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("GUILogSize");
                l_Writer.WriteString(l_Settings.GUILogSize.ToString());
                l_Writer.WriteEndAttribute();
                //Captcha
                l_Writer.WriteStartAttribute("CaptchaApikey");
                l_Writer.WriteString(l_Settings.CaptchaApikey);
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("CaptchaSolverEnabled");
                l_Writer.WriteString(l_Settings.CaptchaSolverEnabled.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("CaptchaSelfSolve");
                l_Writer.WriteString(l_Settings.CaptchaSelfSolve.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("CaptchaPriority");
                l_Writer.WriteString(l_Settings.CaptchaPriority.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("CaptchaMinWorkers");
                l_Writer.WriteString(l_Settings.CaptchaMinWorkers.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("CaptchaMinWorkersALT");
                l_Writer.WriteString(l_Settings.CaptchaMinWorkersALT.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("CaptchaMaxQueue");
                l_Writer.WriteString(l_Settings.CaptchaMaxQueue.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("CaptchaExtraDelay");
                l_Writer.WriteString(l_Settings.CaptchaExtraDelay.ToString());
                l_Writer.WriteEndAttribute();
                //Feature control
                l_Writer.WriteStartAttribute("MasterBuildingQueue");
                l_Writer.WriteString(l_Settings.MasterBuildingQueue.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MasterUnitQueue");
                l_Writer.WriteString(l_Settings.MasterUnitQueue.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MasterCulture");
                l_Writer.WriteString(l_Settings.MasterCulture.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MasterFarmers");
                l_Writer.WriteString(l_Settings.MasterFarmers.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteStartAttribute("MasterTrade");
                l_Writer.WriteString(l_Settings.MasterTrade.ToString());
                l_Writer.WriteEndAttribute();
                l_Writer.WriteEndElement();//End of User
                //
                l_Writer.WriteEndElement();//End of Users
                l_Writer.WriteEndDocument();
                l_Writer.Flush();
                l_Writer.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveProgramSettings(): " + e.Message);
            }
        }

        public void loadProgramSettings()
        {
            //Note Modify this when adding new program settings (4/4)
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextReader l_TextReader = new XmlTextReader(l_Settings.AdvProgramSettingsFile);

                l_TextReader.Read();

                // If the node has value
                while (l_TextReader.Read())
                {
                    // Move to fist element
                    l_TextReader.MoveToElement();

                    if (l_TextReader.Name.Equals("User"))
                    {
                        //General
                        if (l_TextReader.GetAttribute("GenUserName") != null)
                            l_Settings.GenUserName = l_TextReader.GetAttribute("GenUserName");
                        if (l_TextReader.GetAttribute("GenPassword") != null)
                            l_Settings.GenPassword = l_TextReader.GetAttribute("GenPassword");
                        if (l_TextReader.GetAttribute("GenMainServer") != null) 
                            l_Settings.GenMainServer = l_TextReader.GetAttribute("GenMainServer");
                        if (l_TextReader.GetAttribute("GenServer") != null) 
                            l_Settings.GenServer = l_TextReader.GetAttribute("GenServer");
                        if (l_TextReader.GetAttribute("GenIsHeroWorld") != null)
                            l_Settings.GenIsHeroWorld = l_TextReader.GetAttribute("GenIsHeroWorld").Equals("True");
                        if (l_TextReader.GetAttribute("GenToTryIcon") != null) 
                            l_Settings.GenToTryIcon = l_TextReader.GetAttribute("GenToTryIcon").Equals("True");
                        //Building/training queue
                        if (l_TextReader.GetAttribute("QueueTimer") != null) 
                            l_Settings.QueueTimer = int.Parse(l_TextReader.GetAttribute("QueueTimer"));
                        if (l_TextReader.GetAttribute("MinUnitQueuePop") != null)
                            l_Settings.MinUnitQueuePop = int.Parse(l_TextReader.GetAttribute("MinUnitQueuePop"));
                        if (l_TextReader.GetAttribute("QueueLimit") != null)
                            l_Settings.QueueLimit = int.Parse(l_TextReader.GetAttribute("QueueLimit"));
                        if (l_TextReader.GetAttribute("BuildFarmBelow") != null)
                            l_Settings.BuildFarmBelow = int.Parse(l_TextReader.GetAttribute("BuildFarmBelow"));
                        if (l_TextReader.GetAttribute("AdvancedQueue") != null)
                            l_Settings.AdvancedQueue = l_TextReader.GetAttribute("AdvancedQueue").Equals("True");
                        //Trade
                        if (l_TextReader.GetAttribute("TradeTimer") != null)
                            l_Settings.TradeTimer = int.Parse(l_TextReader.GetAttribute("TradeTimer"));
                        //E-Mail
                        if (l_TextReader.GetAttribute("MailAddress") != null) 
                            l_Settings.MailAddress = l_TextReader.GetAttribute("MailAddress");
                        if (l_TextReader.GetAttribute("MailNotify") != null) 
                            l_Settings.MailNotify = l_TextReader.GetAttribute("MailNotify").Equals("True");
                        if (l_TextReader.GetAttribute("MailIncludeSupport") != null)
                            l_Settings.MailIncludeSupport = l_TextReader.GetAttribute("MailIncludeSupport").Equals("True");
                        if (l_TextReader.GetAttribute("MailUsername") != null) 
                            l_Settings.MailUsername = l_TextReader.GetAttribute("MailUsername");
                        if (l_TextReader.GetAttribute("MailPassword") != null) 
                            l_Settings.MailPassword = l_TextReader.GetAttribute("MailPassword");
                        if (l_TextReader.GetAttribute("MailServer") != null) 
                            l_Settings.MailServer = l_TextReader.GetAttribute("MailServer");
                        if (l_TextReader.GetAttribute("MailPort") != null) 
                            l_Settings.MailPort = int.Parse(l_TextReader.GetAttribute("MailPort"));
                        //Farmer
                        if (l_TextReader.GetAttribute("FarmerScheduler") != null)
                            l_Settings.FarmerScheduler = l_TextReader.GetAttribute("FarmerScheduler");
                        if (l_TextReader.GetAttribute("FarmerMaxResources") != null)
                            l_Settings.FarmerMaxResources = int.Parse(l_TextReader.GetAttribute("FarmerMaxResources"));
                        if (l_TextReader.GetAttribute("FarmerRandomize") != null)
                            l_Settings.FarmerRandomize = l_TextReader.GetAttribute("FarmerRandomize").Equals("True");
                        //Reconnect
                        if (l_TextReader.GetAttribute("AdvTimerReconnectMin") != null)
                            l_Settings.RecTimerReconnectMin = int.Parse(l_TextReader.GetAttribute("AdvTimerReconnectMin"));
                        if (l_TextReader.GetAttribute("AdvTimerReconnectMax") != null)
                            l_Settings.RecTimerReconnectMax = int.Parse(l_TextReader.GetAttribute("AdvTimerReconnectMax"));
                        if (l_TextReader.GetAttribute("AdvMaxReconnectsEnabled") != null)
                            l_Settings.RecMaxReconnectsEnabled = l_TextReader.GetAttribute("AdvMaxReconnectsEnabled").Equals("True");
                        if (l_TextReader.GetAttribute("AdvMaxReconnects") != null)
                            l_Settings.RecMaxReconnects = int.Parse(l_TextReader.GetAttribute("AdvMaxReconnects"));
                        if (l_TextReader.GetAttribute("AdvForcedReconnects") != null)
                            l_Settings.RecForcedReconnects = l_TextReader.GetAttribute("AdvForcedReconnects").Equals("True");
                        if (l_TextReader.GetAttribute("AdvMinForcedReconnect") != null)
                            l_Settings.RecMinForcedReconnect = int.Parse(l_TextReader.GetAttribute("AdvMinForcedReconnect"));
                        if (l_TextReader.GetAttribute("AdvMaxForcedReconnect") != null)
                            l_Settings.RecMaxForcedReconnect = int.Parse(l_TextReader.GetAttribute("AdvMaxForcedReconnect"));
                        //Premium
                        if (l_TextReader.GetAttribute("PreUseGold") != null)
                            l_Settings.PreUseGold = l_TextReader.GetAttribute("PreUseGold").Equals("True");
                        if (l_TextReader.GetAttribute("PreUseFarmAllFeature") != null)
                            l_Settings.PreUseFarmAllFeature = l_TextReader.GetAttribute("PreUseFarmAllFeature").Equals("True");
                        //Sound warning
                        if (l_TextReader.GetAttribute("SoundIncomingAttack") != null)
                            l_Settings.SoundIncomingAttack = l_TextReader.GetAttribute("SoundIncomingAttack").Equals("True");
                        if (l_TextReader.GetAttribute("SoundCaptchaWarning") != null)
                            l_Settings.SoundCaptchaWarning = l_TextReader.GetAttribute("SoundCaptchaWarning").Equals("True");
                        //Advanced
                        if (l_TextReader.GetAttribute("AdvMinTimerRefresh") != null) 
                            l_Settings.AdvMinTimerRefresh = int.Parse(l_TextReader.GetAttribute("AdvMinTimerRefresh"));
                        if (l_TextReader.GetAttribute("AdvMaxTimerRefresh") != null)
                            l_Settings.AdvMaxTimerRefresh = int.Parse(l_TextReader.GetAttribute("AdvMaxTimerRefresh"));
                        if (l_TextReader.GetAttribute("AdvSchedulerBot") != null)
                            l_Settings.AdvSchedulerBot = l_TextReader.GetAttribute("AdvSchedulerBot").Equals("True");
                        if (l_TextReader.GetAttribute("AdvAutoStart") != null)
                            l_Settings.AdvAutoStart = l_TextReader.GetAttribute("AdvAutoStart").Equals("True");
                        if (l_TextReader.GetAttribute("AdvAutoPause") != null)
                            l_Settings.AdvAutoPause = l_TextReader.GetAttribute("AdvAutoPause").Equals("True");
                        if (l_TextReader.GetAttribute("AdvDebugMode") != null) 
                            l_Settings.AdvDebugMode = l_TextReader.GetAttribute("AdvDebugMode").Equals("True");
                        if (l_TextReader.GetAttribute("AdvOutputAllMode") != null)
                            l_Settings.AdvOutputAllMode = l_TextReader.GetAttribute("AdvOutputAllMode").Equals("True");
                        if (l_TextReader.GetAttribute("AdvMinDelayRequests") != null)
                            l_Settings.AdvMinDelayRequests = int.Parse(l_TextReader.GetAttribute("AdvMinDelayRequests"));
                        if (l_TextReader.GetAttribute("AdvMaxDelayRequests") != null)
                            l_Settings.AdvMaxDelayRequests = int.Parse(l_TextReader.GetAttribute("AdvMaxDelayRequests"));
                        if (l_TextReader.GetAttribute("AdvMinDelayFarmers") != null)
                            l_Settings.AdvMinDelayFarmers = int.Parse(l_TextReader.GetAttribute("AdvMinDelayFarmers"));
                        if (l_TextReader.GetAttribute("AdvMaxDelayFarmers") != null)
                            l_Settings.AdvMaxDelayFarmers = int.Parse(l_TextReader.GetAttribute("AdvMaxDelayFarmers"));
                        if (l_TextReader.GetAttribute("AdvFarmerLootLag") != null)
                            l_Settings.AdvFarmerLootLag = int.Parse(l_TextReader.GetAttribute("AdvFarmerLootLag"));
                        if (l_TextReader.GetAttribute("AdvTimeout") != null)
                            l_Settings.AdvTimeout = int.Parse(l_TextReader.GetAttribute("AdvTimeout"));
                        if (l_TextReader.GetAttribute("AdvDebugFile") != null) 
                            l_Settings.AdvDebugFile = l_TextReader.GetAttribute("AdvDebugFile");
                        if (l_TextReader.GetAttribute("AdvProgramSettingsFile") != null) 
                            l_Settings.AdvProgramSettingsFile = l_TextReader.GetAttribute("AdvProgramSettingsFile");
                        if (l_TextReader.GetAttribute("AdvTownsSettingsFile") != null) 
                            l_Settings.AdvTownsSettingsFile = l_TextReader.GetAttribute("AdvTownsSettingsFile");
                        if (l_TextReader.GetAttribute("AdvUserAgent") != null) 
                            l_Settings.AdvUserAgent = l_TextReader.GetAttribute("AdvUserAgent");
                        if (l_TextReader.GetAttribute("AdvNotesFile") != null) 
                            l_Settings.AdvNotesFile = l_TextReader.GetAttribute("AdvNotesFile");
                        //GUI
                        if (l_TextReader.GetAttribute("GUIBuildingLevelTargetColor") != null)
                            l_Settings.GUIBuildingLevelTargetColor = Color.FromArgb(int.Parse(l_TextReader.GetAttribute("GUIBuildingLevelTargetColor")));
                        if (l_TextReader.GetAttribute("GUIBuildingTooltipsEnabled") != null)
                            l_Settings.GUIBuildingTooltipsEnabled = l_TextReader.GetAttribute("GUIBuildingTooltipsEnabled").Equals("True");
                        if (l_TextReader.GetAttribute("GUITownTabsLinked") != null)
                            l_Settings.GUITownTabsLinked = l_TextReader.GetAttribute("GUITownTabsLinked").Equals("True");
                        if (l_TextReader.GetAttribute("GUIReconnectTimerTitleBar") != null)
                            l_Settings.GUIReconnectTimerTitleBar = l_TextReader.GetAttribute("GUIReconnectTimerTitleBar").Equals("True");
                        if (l_TextReader.GetAttribute("GUINotificationSize") != null)
                            l_Settings.GUINotificationSize = int.Parse(l_TextReader.GetAttribute("GUINotificationSize"));
                        if (l_TextReader.GetAttribute("GUILogSize") != null)
                            l_Settings.GUILogSize = int.Parse(l_TextReader.GetAttribute("GUILogSize"));
                        //Captcha
                        if (l_TextReader.GetAttribute("CaptchaApikey") != null)
                            l_Settings.CaptchaApikey = l_TextReader.GetAttribute("CaptchaApikey");
                        if (l_TextReader.GetAttribute("CaptchaSolverEnabled") != null)
                            l_Settings.CaptchaSolverEnabled = l_TextReader.GetAttribute("CaptchaSolverEnabled").Equals("True");
                        if (l_TextReader.GetAttribute("CaptchaSelfSolve") != null)
                            l_Settings.CaptchaSelfSolve = l_TextReader.GetAttribute("CaptchaSelfSolve").Equals("True");
                        if (l_TextReader.GetAttribute("CaptchaPriority") != null)
                            l_Settings.CaptchaPriority = int.Parse(l_TextReader.GetAttribute("CaptchaPriority"));
                        if (l_TextReader.GetAttribute("CaptchaMinWorkers") != null)
                            l_Settings.CaptchaMinWorkers = int.Parse(l_TextReader.GetAttribute("CaptchaMinWorkers"));
                        if (l_TextReader.GetAttribute("CaptchaMinWorkersALT") != null)
                            l_Settings.CaptchaMinWorkersALT = int.Parse(l_TextReader.GetAttribute("CaptchaMinWorkersALT"));
                        if (l_TextReader.GetAttribute("CaptchaMaxQueue") != null)
                            l_Settings.CaptchaMaxQueue = int.Parse(l_TextReader.GetAttribute("CaptchaMaxQueue"));
                        if (l_TextReader.GetAttribute("CaptchaExtraDelay") != null)
                            l_Settings.CaptchaExtraDelay = int.Parse(l_TextReader.GetAttribute("CaptchaExtraDelay"));
                        //Feature control
                        if (l_TextReader.GetAttribute("MasterBuildingQueue") != null)
                            l_Settings.MasterBuildingQueue = l_TextReader.GetAttribute("MasterBuildingQueue").Equals("True");
                        if (l_TextReader.GetAttribute("MasterUnitQueue") != null)
                            l_Settings.MasterUnitQueue = l_TextReader.GetAttribute("MasterUnitQueue").Equals("True");
                        if (l_TextReader.GetAttribute("MasterCulture") != null)
                            l_Settings.MasterCulture = l_TextReader.GetAttribute("MasterCulture").Equals("True");
                        if (l_TextReader.GetAttribute("MasterFarmers") != null)
                            l_Settings.MasterFarmers = l_TextReader.GetAttribute("MasterFarmers").Equals("True");
                        if (l_TextReader.GetAttribute("MasterTrade") != null)
                            l_Settings.MasterTrade = l_TextReader.GetAttribute("MasterTrade").Equals("True");
                    }
                }
                l_TextReader.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in loadProgramSettings(): " + e.Message);
            }
        }

        public void saveTownsSettings(Player p_Player)
        {
            //Note Modify this when adding new town settings (1/2)
            Settings l_Settings = Settings.Instance;
            saveUnitQueueSettings(p_Player);

            try
            {
                XmlTextWriter l_Writer = new XmlTextWriter(l_Settings.AdvTownsSettingsFile, Encoding.UTF8);
                l_Writer.Formatting = Formatting.Indented;
                l_Writer.WriteStartDocument();
                l_Writer.WriteComment("Towns settings.");
                l_Writer.WriteStartElement("Towns");//Start of Towns
                for (int i = 0; i < p_Player.Towns.Count; i++)
                {
                    l_Writer.WriteStartElement("Town");//Start of Town
                    l_Writer.WriteStartAttribute("TownID");
                    l_Writer.WriteString(p_Player.Towns[i].TownID);
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("Name");
                    l_Writer.WriteString(p_Player.Towns[i].Name);
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("BuildingQueueEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].BuildingQueueEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("BuildingLevelsTargetEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].BuildingLevelsTargetEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("BuildingDowngradeEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].BuildingDowngradeEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("BuildingLevelsTarget");
                    l_Writer.WriteString(p_Player.Towns[i].getBuildingLevelsTarget());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("BuildingQueue");
                    l_Writer.WriteString(p_Player.Towns[i].getBuildingQueue());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("UnitQueueEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].UnitQueueEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    //l_Writer.WriteStartAttribute("UnitQueue");
                    //l_Writer.WriteString(p_Player.Towns[i].getUnitQueue());
                    //l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("FarmersLootEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].FarmersLootEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("FarmersFriendlyDemandsOnly");
                    l_Writer.WriteString(p_Player.Towns[i].FarmersFriendlyDemandsOnly.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("FarmersMinMood");
                    l_Writer.WriteString(p_Player.Towns[i].FarmersMinMood.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("FarmersLootInterval");
                    l_Writer.WriteString(p_Player.Towns[i].FarmersLootInterval);
                    l_Writer.WriteEndAttribute();
                    //l_Writer.WriteStartAttribute("FarmersSelected");
                    //l_Writer.WriteString(p_Player.Towns[i].getSelectedFarmers());
                    //l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("CulturalFestivalsEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].CulturalFestivalsEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("CulturalPartyEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].CulturalPartyEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("CulturalGamesEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].CulturalGamesEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("CulturalTriumphEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].CulturalTriumphEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("CulturalTheaterEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].CulturalTheaterEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("MilitiaTrigger");
                    l_Writer.WriteString(p_Player.Towns[i].MilitiaTrigger.ToString());
                    l_Writer.WriteEndAttribute();
                    //Trading
                    l_Writer.WriteStartAttribute("TradeEnabled");
                    l_Writer.WriteString(p_Player.Towns[i].TradeEnabled.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("TradeMode");
                    l_Writer.WriteString(p_Player.Towns[i].TradeMode);
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("TradeRemainingResources");
                    l_Writer.WriteString(p_Player.Towns[i].TradeRemainingResources.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("TradeMinSendAmount");
                    l_Writer.WriteString(p_Player.Towns[i].TradeMinSendAmount.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("TradePercentageWarehouse");
                    l_Writer.WriteString(p_Player.Towns[i].TradePercentageWarehouse.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("TradeMaxDistance");
                    l_Writer.WriteString(p_Player.Towns[i].TradeMaxDistance.ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("TradeOmitFromChangeAll");
                    l_Writer.WriteString(p_Player.Towns[i].TradeOmitFromChangeAll.ToString());
                    l_Writer.WriteEndAttribute();

                    l_Writer.WriteEndElement();//End of Town
                }
                l_Writer.WriteEndElement();//End of Towns
                l_Writer.WriteEndDocument();
                l_Writer.Flush();
                l_Writer.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveTownsSettings(): " + e.Message);
            }
        }

        public void saveUnitQueueSettings(Player p_Player)
        {
            //Note Modify this when adding new units (11/15)
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextWriter l_Writer = new XmlTextWriter(l_Settings.AdvUnitQueueSettingsFile, Encoding.UTF8);
                l_Writer.Formatting = Formatting.Indented;
                l_Writer.WriteStartDocument();
                l_Writer.WriteComment("Unit queue settings.");
                l_Writer.WriteStartElement("Towns");//Start of Towns
                for (int i = 0; i < p_Player.Towns.Count; i++)
                {
                    l_Writer.WriteStartElement("Town");//Start of Town
                    
                    l_Writer.WriteStartAttribute("TownID");
                    l_Writer.WriteString(p_Player.Towns[i].TownID);
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("sword");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("sword").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("slinger");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("slinger").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("archer");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("archer").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("hoplite");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("hoplite").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("rider");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("rider").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("chariot");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("chariot").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("catapult");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("catapult").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("minotaur");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("minotaur").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("manticore");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("manticore").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("centaur");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("centaur").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("pegasus");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("pegasus").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("harpy");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("harpy").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("medusa");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("medusa").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("zyklop");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("zyklop").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("cerberus");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("cerberus").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("fury");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("fury").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("griffin");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("griffin").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("calydonian_boar");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("calydonian_boar").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("godsent");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("godsent").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("big_transporter");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("big_transporter").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("bireme");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("bireme").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("attack_ship");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("attack_ship").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("demolition_ship");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("demolition_ship").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("small_transporter");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("small_transporter").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("trireme");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("trireme").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("colonize_ship");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("colonize_ship").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("sea_monster");
                    l_Writer.WriteString(p_Player.Towns[i].getUnitTargetAmount("sea_monster").ToString());
                    l_Writer.WriteEndAttribute();

                    l_Writer.WriteEndElement();//End of Town
                }
                l_Writer.WriteEndElement();//End of Towns
                l_Writer.WriteEndDocument();
                l_Writer.Flush();
                l_Writer.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveUnitQueueSettings(): " + e.Message);
            }
        }

        public void saveFarmersSettings(Player p_Player)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextWriter l_Writer = new XmlTextWriter(l_Settings.AdvFarmersSettingsFile, Encoding.UTF8);
                l_Writer.Formatting = Formatting.Indented;
                l_Writer.WriteStartDocument();
                l_Writer.WriteComment("Farmers settings.");
                l_Writer.WriteStartElement("Towns");//Start of Towns
                for (int i = 0; i < p_Player.Towns.Count; i++)
                {
                    l_Writer.WriteStartElement("Town");//Start of Town
                    l_Writer.WriteStartAttribute("TownID");
                    l_Writer.WriteString(p_Player.Towns[i].TownID);
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("FarmersSelected");
                    l_Writer.WriteString(p_Player.Towns[i].getSelectedFarmers());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteEndElement();//End of Town
                }
                l_Writer.WriteEndElement();//End of Towns
                l_Writer.WriteEndDocument();
                l_Writer.Flush();
                l_Writer.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveFarmersSettings(): " + e.Message);
            }
        }


        /*
        public void loadTownsSettings()
        {
            //Moved to the player class
        }
        */

        public void saveTemplatesUnitQueue(List<QueueTemplate> p_TemplateUnitQueue)
        {
            //Note Modify this when adding new units (12/15)
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextWriter l_Writer = new XmlTextWriter(l_Settings.AdvTemplatesUnitQueueSettingsFile, Encoding.UTF8);
                l_Writer.Formatting = Formatting.Indented;
                l_Writer.WriteStartDocument();
                l_Writer.WriteComment("Templates unit queue.");
                l_Writer.WriteStartElement("Templates");//Start of templates
                for (int i = 0; i < p_TemplateUnitQueue.Count; i++)
                {
                    l_Writer.WriteStartElement("Template");//Start of template
                    l_Writer.WriteStartAttribute("Name");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].Name);
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("sword");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("sword").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("slinger");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("slinger").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("archer");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("archer").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("hoplite");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("hoplite").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("rider");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("rider").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("chariot");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("chariot").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("catapult");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("catapult").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("minotaur");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("minotaur").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("manticore");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("manticore").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("centaur");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("centaur").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("pegasus");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("pegasus").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("harpy");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("harpy").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("medusa");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("medusa").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("zyklop");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("zyklop").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("cerberus");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("cerberus").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("fury");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("fury").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("griffin");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("griffin").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("calydonian_boar");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("calydonian_boar").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("godsent");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("godsent").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("big_transporter");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("big_transporter").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("bireme");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("bireme").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("attack_ship");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("attack_ship").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("demolition_ship");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("demolition_ship").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("small_transporter");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("small_transporter").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("trireme");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("trireme").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("colonize_ship");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("colonize_ship").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("sea_monster");
                    l_Writer.WriteString(p_TemplateUnitQueue[i].getUnitTargetAmount("sea_monster").ToString());
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteEndElement();//End of template
                }
                l_Writer.WriteEndElement();//End of templates
                l_Writer.WriteEndDocument();
                l_Writer.Flush();
                l_Writer.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveTemplatesUnitQueue(): " + e.Message);
            }
        }

        /*
        public void loadTemplatesUnitQueue()
        {
            //Moved to the player class
        }
        */

        public void saveTemplatesBuildingQueue(List<QueueTemplate> p_TemplateBuildingQueue)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                XmlTextWriter l_Writer = new XmlTextWriter(l_Settings.AdvTemplatesBuildingQueueSettingsFile, Encoding.UTF8);
                l_Writer.Formatting = Formatting.Indented;
                l_Writer.WriteStartDocument();
                l_Writer.WriteComment("Templates building queue.");
                l_Writer.WriteStartElement("Templates");//Start of templates
                for (int i = 0; i < p_TemplateBuildingQueue.Count; i++)
                {
                    l_Writer.WriteStartElement("Template");//Start of template
                    l_Writer.WriteStartAttribute("Name");
                    l_Writer.WriteString(p_TemplateBuildingQueue[i].Name);
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteStartAttribute("Queue");
                    l_Writer.WriteString(p_TemplateBuildingQueue[i].Queue);
                    l_Writer.WriteEndAttribute();
                    l_Writer.WriteEndElement();//End of template
                }
                l_Writer.WriteEndElement();//End of templates
                l_Writer.WriteEndDocument();
                l_Writer.Flush();
                l_Writer.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveTemplatesBuildingQueue(): " + e.Message);
            }
        }

        /*
        public void loadTemplatesBuildingQueue()
        {
            //Moved to the player class
        }
        */

        public void saveServerResponse(string p_Method, string p_Response)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                TextWriter l_TwResponse = new StreamWriter(l_Settings.AdvResponseDir + p_Method + ".txt", false);
                l_TwResponse.Write(p_Response);
                l_TwResponse.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveServerResponse(): " + e.Message);
            }
        }

        public void saveServerResponse2(string p_State, string p_Response)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                TextWriter l_TwResponse = new StreamWriter(l_Settings.AdvResponseDir + System.DateTime.Now.Ticks.ToString() + "_" + p_State + ".txt", false);
                l_TwResponse.Write(p_Response);
                l_TwResponse.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveServerResponse2(): " + e.Message);
            }
        }

        public void saveServerResponseCaptcha(string p_Response)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                TextWriter l_TwResponse = new StreamWriter(l_Settings.AdvResponseDir + System.DateTime.Now.Ticks.ToString() + "_" + "CAPTCHA" + ".txt", false);
                l_TwResponse.Write(p_Response);
                l_TwResponse.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in saveServerResponseCaptcha(): " + e.Message);
            }
        }

        public void createHtmlOverview(Player p_Player)
        {
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;
            //string l_Overview = l_Parser.createHtmlOverview(p_Player);
            string l_Overview = l_Parser.createHtmlOverview2(p_Player);

            try
            {
                TextWriter l_TwResponse = new StreamWriter(l_Settings.AdvOverviewFile, false);
                l_TwResponse.Write(l_Overview);
                l_TwResponse.Close();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    debug("Exception in createHtmlOverview(): " + e.Message);
            }
        }
    }
}
