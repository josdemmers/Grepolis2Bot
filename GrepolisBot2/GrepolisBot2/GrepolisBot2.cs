using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;
using mshtml;
using System.Security.Permissions;
using System.Diagnostics;
using System.Media;

namespace GrepolisBot2
{
    public partial class GrepolisBot2 : Form
    {
        //Useragent
        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);
        const int URLMON_OPTION_USERAGENT = 0x10000001;
        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);
        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

        //General
        private NotifyIcon m_NotifyIcon = new NotifyIcon();
        private ToolTip m_Tooltip = new ToolTip();

        // getWeb Browser button 
       bool getBrowserComplete;
       string getBrowserLookFor;

        //Custom classes
        private Controller m_Controller = new Controller();

        //GUI States
        private string m_SelectedTownQueue = "0";
        private string m_SelectedTownFarmers = "0";
        private string m_SelectedTownCulture = "0";
        private string m_SelectedTownTrade = "0";
        private string m_SelectedTownCombo = "0";

        //Timer
        private System.Windows.Forms.Timer m_TimerLoginp1 = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer m_TimerLoginp2 = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer m_TimerLoginp3 = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer m_TimerCaptcha = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer m_TimerCaptchaAnswered = new System.Windows.Forms.Timer();

        //Thread Handling
        public delegate void SetStatusBarCallback(object sender);
        public delegate void UpdateFarmerGUICallback();
        public delegate void SetGuiToLoggedInStateCallBack();
        public delegate void SetGuiToTimeoutProcessedStateCallBack();
        public delegate void SetToTradeProcessedStateCallBack();
        public delegate void UpdateRefreshTimerCallBack(object sender);
        public delegate void UpdateQueueTimerCallBack(object sender);
        public delegate void UpdateTradeTimerCallBack(object sender);
        public delegate void UpdateReconnectTimerCallBack(object sender);
        public delegate void UpdateForcedReconnectTimerCallBack(object sender);
        public delegate void UpdateConnectedTimerCallBack(object sender);
        public delegate void UpdateBuildingQueueCallBack(object sender);
        public delegate void LogCallBack(object sender);
        public delegate void VersionInfoCallBack(object sender);
        public delegate void serverRequestDelayRequestCallBack();
        public delegate void serverRequestCaptchaDelayRequestCallBack();
        public delegate void pauseBotRequestCaptchaCallBack();
        public delegate void startCaptchaCheckTimerRequestCallBack();
        public delegate void captchaCheckPreCycleCallBack();
        public delegate void captchaAnswerReadyCallBack();
        public delegate void captchaAnswerModeratedCallBack();
        public delegate void captchaAnswerSendToGrepolisCorrectCallBack();
        public delegate void captchaAnswerSendToGrepolisInCorrectCallBack();
        public delegate void captchaSolver9kwDownCallBack();
        public delegate void townListUpdatedCallBack();

        private SoundPlayer m_SoundPlayer = new SoundPlayer();

       

//-->Constructor

        public GrepolisBot2()
        {
            InitializeComponent();
            initMisc();
            initEventHandlers();
            initNotifyIcon();
            loadSettings();
            initBrowser();
            initLinkLabel();
            initTimers();//Must be called after settings are loaded
            loadNotes();
            loadTemplates();

            //Custom gui customization
            this.Text = String.Format("{0} {1}", AssemblyTitle, AssemblyVersion);
            colorDialog1.AllowFullOpen = false;

            //progressBarCP.CreateGraphics().DrawString("Test", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBarCP.Width / 2 - 10, progressBarCP.Height / 2 - 7));
            m_Tooltip.SetToolTip(progressBarCP, "0/3");
            m_Controller.checkVersion();

            //Write to log/debug
            IOHandler l_IOHandler = IOHandler.Instance;
            string l_Message = "Starting " + AssemblyTitle + " " + AssemblyVersion;
            l_IOHandler.log(DateTime.Now.ToLocalTime() + " " + l_Message);
            l_IOHandler.debug(l_Message);

            //Auto Start
            Settings l_Settings = Settings.Instance;
            if (l_Settings.AdvAutoStart)
            {
                buttonLoginMethod1.Enabled = false;
                loginP1();
            }
        }

//--->Attributes

        #region attributes

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        #endregion

//-->Methods

        #region Init

        private void initMisc()
        {
            //Fix for "The remote server returned an error: (417) Expectation Failed."
            System.Net.ServicePointManager.Expect100Continue = false;
            toolStripStatusLabelGrep.Text = String.Format("{0} v{1}", AssemblyTitle, AssemblyVersion);
        }

        private void initBrowser()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                /** This part works with the useragent of the webbrowser control **/
                string l_UserAgent = "";
                string l_Js = @"<script type='text/javascript'>function getUserAgent(){document.write(navigator.userAgent)}</script>";
                webBrowserGrepo.Document.Write(l_Js);
                webBrowserGrepo.Document.InvokeScript("getUserAgent");
                l_UserAgent = webBrowserGrepo.DocumentText.Substring(l_Js.Length);
                l_Settings.AdvUserAgent = l_UserAgent;//Disable this when using a custom user-agent

                /** This part works with a custom useragent **/
                //UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, l_Settings.AdvUserAgent, l_Settings.AdvUserAgent.Length, 0);

                webBrowserGrepo.ScriptErrorsSuppressed = true;
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in initBrowser(): " + e.Message);
                }
            }
        }

        private void initEventHandlers()
        {
            m_NotifyIcon.Click += new EventHandler(m_NotifyIcon_Click);
            this.FormClosed += new FormClosedEventHandler(GrepolisBot2_FormClosed);
            m_Controller.statusBarUpdated += new Controller.SetStatusBarHandler(m_Controller_statusBarUpdated);
            m_Controller.timeoutProcessedStateChanged += new Controller.SetGuiToTimeoutProcessedStateHandler(m_Controller_timeoutProcessedStateChanged);
            m_Controller.tradeProcessedStateChanged += new Controller.SetToTradeProcessedStateHandler(m_Controller_tradeProcessedStateChanged);
            m_Controller.townProcessedStateChanged += new Controller.SetToTownProcessedStateHandler(m_Controller_townProcessedStateChanged);
            m_Controller.refreshTimerUpdated += new Controller.updateRefreshTimerHandler(m_Controller_refreshTimerUpdated);
            m_Controller.queueTimerUpdated += new Controller.updateQueueTimerHandler(m_Controller_queueTimerUpdated);
            m_Controller.tradeTimerUpdated += new Controller.updateTradeTimerHandler(m_Controller_tradeTimerUpdated);
            m_Controller.reconnectTimerUpdated += new Controller.updateReconnectTimerHandler(m_Controller_reconnectTimerUpdated);
            m_Controller.forcedReconnectTimerUpdated += new Controller.updateForcedReconnectTimerHandler(m_Controller_forcedReconnectTimerUpdated);
            m_Controller.connectedTimerUpdated += new Controller.updateConnectedTimerHandler(m_Controller_connectedTimerUpdated);
            m_Controller.versionInfoUpdated += new Controller.versionInfoHandler(m_Controller_versionInfoUpdated);
            m_Controller.logUpdated += new Controller.logHandler(m_Controller_logUpdated);
            m_Controller.serverRequestDelayRequested += new Controller.serverRequestDelayRequestHandler(m_Controller_serverRequestDelayRequested);
            m_Controller.serverRequestCaptchaDelayRequested += new Controller.serverRequestCaptchaDelayHandler(m_Controller_serverRequestCaptchaDelayRequested);
            m_Controller.pauseBotRequestCaptcha += new Controller.pauseBotRequestCaptchaHandler(m_Controller_pauseBotRequestCaptcha);
            m_Controller.startCaptchaCheckTimerRequest += new Controller.startCaptchaCheckTimerRequestHandler(m_Controller_startCaptchaCheckTimerRequest);
            m_Controller.captchaCheckPreCycle += new Controller.captchaCheckPreCycleHandler(m_Controller_captchaCheckPreCycle);
            m_Controller.captchaAnswerReady += new Controller.captchaAnswerReadyHandler(m_Controller_captchaAnswerReady);
            m_Controller.captchaAnswerModerated += new Controller.captchaAnswerModeratedHandler(m_Controller_captchaAnswerModerated);
            m_Controller.captchaAnswerSendToGrepolisCorrect += new Controller.captchaAnswerSendToGrepolisCorrectHandler(m_Controller_captchaAnswerSendToGrepolisCorrect);
            m_Controller.captchaAnswerSendToGrepolisInCorrect += new Controller.captchaAnswerSendToGrepolisInCorrectHandler(m_Controller_captchaAnswerSendToGrepolisInCorrect);
            m_Controller.captchaSolver9kwDown += new Controller.captchaSolver9kwDownHandler(m_Controller_captchaSolver9kwDown);
            m_Controller.townListUpdated += m_Controller_townListUpdated;
            webBrowserGrepo.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowserGrepo_DocumentCompleted);
            m_TimerLoginp1.Tick += new EventHandler(m_TimerLoginp1_Tick);
            m_TimerLoginp2.Tick += new EventHandler(m_TimerLoginp2_Tick);
            m_TimerLoginp3.Tick += new EventHandler(m_TimerLoginp3_Tick);
            m_TimerCaptcha.Tick += new EventHandler(m_TimerCaptcha_Tick);
            m_TimerCaptchaAnswered.Tick += new EventHandler(m_TimerCaptchaAnswered_Tick);         
        }

        /*
         * Initialize notify icon
         */
        private void initNotifyIcon()
        {
            m_NotifyIcon.Visible = true;
            m_NotifyIcon.BalloonTipText = "Grepolis bot 2";
            m_NotifyIcon.ShowBalloonTip(2);  //show balloon tip for 2 seconds
            m_NotifyIcon.Text = "Grepolis bot 2";
            m_NotifyIcon.Icon = this.Icon;
        }

        /*
         * Initialize timers 
         */
        private void initTimers()
        {
            m_Controller.initTimers();
            m_TimerLoginp1.Interval = 2000;
            m_TimerLoginp2.Interval = 2000;
            m_TimerLoginp3.Interval = 2000;
            m_TimerCaptcha.Interval = 20000;
            m_TimerCaptchaAnswered.Interval = 5000;
        }

        private void initLinkLabel()
        {
            linkLabelLatestVersion.Text = "Latest version: unknown";
            linkLabelLatestVersion.Links.Add(0, linkLabelLatestVersion.Text.Length+1, "http://bots.uthar.nl/downloads");
            linkLabelLatestVersion.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabelLatestVersion_LinkClicked);
            linkLabelApikey.Text = "Apikey";
            linkLabelApikey.Links.Add(0, linkLabelApikey.Text.Length, "http://www.9kw.eu/register_2709.html");
            linkLabelApikey.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabelApikey_LinkClicked);
        }

        #endregion

        #region Settings

        private void loadNotes()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            textBoxNotes.Text = l_IOHandler.loadNotes();
        }

        private void loadTemplates()
        {
            m_Controller.Player.loadTemplatesUnitQueue();
            m_Controller.Player.loadTemplatesBuildingQueue();
            loadTemplatesUnitQueue();
            loadTemplatesBuildingQueue();
        }

        private void loadTemplatesUnitQueue()
        {
            //Queue tab
            comboBoxTemplatesUnitQueue.Items.Clear();
            for (int i = 0; i < m_Controller.Player.TemplatesUnitQueue.Count; i++)
            {
                comboBoxTemplatesUnitQueue.Items.Add(m_Controller.Player.TemplatesUnitQueue[i].Name);
            }

            //Combo tab
            comboBoxTemplatesUnitQueueCombo.Items.Clear();
            for (int i = 0; i < m_Controller.Player.TemplatesUnitQueue.Count; i++)
            {
                comboBoxTemplatesUnitQueueCombo.Items.Add(m_Controller.Player.TemplatesUnitQueue[i].Name);
            }
        }

        private void loadTemplatesBuildingQueue()
        {
            //Queue tab
            comboBoxTemplatesBuildingQueue.Items.Clear();
            for (int i = 0; i < m_Controller.Player.TemplatesBuildingQueue.Count; i++)
            {
                comboBoxTemplatesBuildingQueue.Items.Add(m_Controller.Player.TemplatesBuildingQueue[i].Name);
            }

            //Combo tab
            comboBoxTemplatesBuildingQueueCombo.Items.Clear();
            for (int i = 0; i < m_Controller.Player.TemplatesBuildingQueue.Count; i++)
            {
                comboBoxTemplatesBuildingQueueCombo.Items.Add(m_Controller.Player.TemplatesBuildingQueue[i].Name);
            }
        }

        /*
         * Load settings file
         */
        private void loadSettings()
        {
            //Note Modify this when adding new program settings (1/4)

            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                //Load settings from file
                l_IOHandler.loadProgramSettings();

                //General
                textBoxUserName.Text = l_Settings.GenUserName;
                textBoxPassword.Text = l_Settings.GenPassword;
                textBoxMainServer.Text = l_Settings.GenMainServer;
                textBoxServer.Text = l_Settings.GenServer;
                checkBoxHeroWorld.Checked = l_Settings.GenIsHeroWorld;
                checkBoxTrayIcon.Checked = l_Settings.GenToTryIcon;
                //Building/training queue
                numericUpDownTimerQueue.Value = l_Settings.QueueTimer;
                numericUpDownMinUnitQueuePop.Value = l_Settings.MinUnitQueuePop;
                numericUpDownQueueLimit.Value = l_Settings.QueueLimit;
                numericUpDownBuildFarmBelow.Value = l_Settings.BuildFarmBelow;
                checkBoxAdvancedQueue.Checked = l_Settings.AdvancedQueue;
                //Trade
                numericUpDownTimerTrade.Value = l_Settings.TradeTimer;
                //E-Mail
                textBoxEmail.Text = l_Settings.MailAddress;
                checkBoxEmail.Checked = l_Settings.MailNotify;
                checkBoxMailIncludeSupport.Checked = l_Settings.MailIncludeSupport;
                textBoxMailUsername.Text = l_Settings.MailUsername;
                textBoxMailPassword.Text = l_Settings.MailPassword;
                textBoxMailServer.Text = l_Settings.MailServer;
                numericUpDownMailPort.Value = l_Settings.MailPort;
                //Farmer
                grepSchedulerSmall1.setScheduler(l_Settings.FarmerScheduler);
                numericUpDownFarmerMaxResources.Value = l_Settings.FarmerMaxResources;
                checkBoxFarmerRandomize.Checked = l_Settings.FarmerRandomize;
                //Reconnect
                numericUpDownTimerReconnectMin.Value = l_Settings.RecTimerReconnectMin;
                numericUpDownTimerReconnectMax.Value = l_Settings.RecTimerReconnectMax;
                checkBoxMaxReconnects.Checked = l_Settings.RecMaxReconnectsEnabled;
                numericUpDownMaxReconnects.Value = l_Settings.RecMaxReconnects;
                checkBoxForcedReconnects.Checked = l_Settings.RecForcedReconnects;
                numericUpDownMinForcedReconnect.Value = l_Settings.RecMinForcedReconnect;
                numericUpDownMaxForcedReconnect.Value = l_Settings.RecMaxForcedReconnect;
                //Premium
                checkBoxUseGold.Checked = l_Settings.PreUseGold;
                checkBoxUseFarmAllFeature.Checked = l_Settings.PreUseFarmAllFeature;
                //Sound warning
                checkBoxIncomingAttackWarning.Checked = l_Settings.SoundIncomingAttack;
                checkBoxCaptchaWarning.Checked = l_Settings.SoundCaptchaWarning;
                //Advanced
                numericUpDownMinTimerRefresh.Value = l_Settings.AdvMinTimerRefresh;
                numericUpDownMaxTimerRefresh.Value = l_Settings.AdvMaxTimerRefresh;
                checkBoxSchedulerBot.Checked = l_Settings.AdvSchedulerBot;
                checkBoxAutoStart.Checked = l_Settings.AdvAutoStart;
                checkBoxAutoPause.Checked = l_Settings.AdvAutoPause;
                checkBoxDebugMode.Checked = l_Settings.AdvDebugMode;
                checkBoxOutputAllMode.Checked = l_Settings.AdvOutputAllMode;
                numericUpDownMinDelayRequests.Value = l_Settings.AdvMinDelayRequests;
                numericUpDownMaxDelayRequests.Value = l_Settings.AdvMaxDelayRequests;
                numericUpDownMinDelayFarmers.Value = l_Settings.AdvMinDelayFarmers;
                numericUpDownMaxDelayFarmers.Value = l_Settings.AdvMaxDelayFarmers;
                numericUpDownFarmersLootLag.Value = l_Settings.AdvFarmerLootLag;
                numericUpDownTimeout.Value = l_Settings.AdvTimeout;
                //GUI
                labelGUIBuildingLevelTargetColor.BackColor = l_Settings.GUIBuildingLevelTargetColor;
                checkBoxGUIBuildingTooltips.Checked = l_Settings.GUIBuildingTooltipsEnabled;
                checkBoxGUISameTownOnAllTabs.Checked = l_Settings.GUITownTabsLinked;
                checkBoxGUIReconnectTimerTitleBar.Checked = l_Settings.GUIReconnectTimerTitleBar;
                numericUpDownNotificationSize.Value = l_Settings.GUINotificationSize;
                numericUpDownLogSize.Value = l_Settings.GUILogSize;
                //Captcha
                textBoxCaptchaApikey.Text = l_Settings.CaptchaApikey;
                checkBoxCaptchaSolverEnabled.Checked = l_Settings.CaptchaSolverEnabled;
                checkBoxCaptchaSelfSolve.Checked = l_Settings.CaptchaSelfSolve;
                numericUpDownCaptchaPriority.Value = l_Settings.CaptchaPriority;
                numericUpDownCaptchaMinWorkers.Value = l_Settings.CaptchaMinWorkers;
                numericUpDownCaptchaMinWorkersALT.Value = l_Settings.CaptchaMinWorkersALT;
                numericUpDownCaptchaMaxQueue.Value = l_Settings.CaptchaMaxQueue;
                numericUpDownCaptchaExtraDelay.Value = l_Settings.CaptchaExtraDelay;
                //Feature control
                this.checkBoxMBuildingQueue.CheckedChanged -= checkBoxMBuildingQueue_CheckedChanged;
                this.checkBoxMUnitQueue.CheckedChanged -= checkBoxMUnitQueue_CheckedChanged;
                this.checkBoxMCulture.CheckedChanged -= checkBoxMCulture_CheckedChanged;
                this.checkBoxMFarmers.CheckedChanged -= checkBoxMFarmers_CheckedChanged;
                this.checkBoxMTrade.CheckedChanged -= checkBoxMTrade_CheckedChanged;
                checkBoxMBuildingQueue.Checked = l_Settings.MasterBuildingQueue;
                checkBoxMUnitQueue.Checked = l_Settings.MasterUnitQueue;
                checkBoxMCulture.Checked = l_Settings.MasterCulture;
                checkBoxMFarmers.Checked = l_Settings.MasterFarmers;
                checkBoxMTrade.Checked = l_Settings.MasterTrade;
                this.checkBoxMBuildingQueue.CheckedChanged += new System.EventHandler(this.checkBoxMBuildingQueue_CheckedChanged);
                this.checkBoxMUnitQueue.CheckedChanged += new System.EventHandler(this.checkBoxMUnitQueue_CheckedChanged);
                this.checkBoxMCulture.CheckedChanged += new System.EventHandler(this.checkBoxMCulture_CheckedChanged);
                this.checkBoxMFarmers.CheckedChanged += new System.EventHandler(this.checkBoxMFarmers_CheckedChanged);
                this.checkBoxMTrade.CheckedChanged += new System.EventHandler(this.checkBoxMTrade_CheckedChanged);
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loadSettings(): " + e.Message);
            }
        }

        private void saveSettings()
        {
            //Note Modify this when adding new program settings (2/4)

            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                //General
                l_Settings.GenUserName = textBoxUserName.Text;
                l_Settings.GenPassword = textBoxPassword.Text;
                l_Settings.GenMainServer = textBoxMainServer.Text;
                l_Settings.GenServer = textBoxServer.Text;
                l_Settings.GenIsHeroWorld = checkBoxHeroWorld.Checked;
                l_Settings.GenToTryIcon = checkBoxTrayIcon.Checked;
                //Building/training queue
                l_Settings.QueueTimer = (int)numericUpDownTimerQueue.Value;
                l_Settings.MinUnitQueuePop = (int)numericUpDownMinUnitQueuePop.Value;
                l_Settings.QueueLimit = (int)numericUpDownQueueLimit.Value;
                l_Settings.BuildFarmBelow = (int)numericUpDownBuildFarmBelow.Value;
                l_Settings.AdvancedQueue = checkBoxAdvancedQueue.Checked;
                //Trade
                l_Settings.TradeTimer = (int)numericUpDownTimerTrade.Value;
                //E-Mail
                l_Settings.MailAddress = textBoxEmail.Text;
                l_Settings.MailNotify = checkBoxEmail.Checked;
                l_Settings.MailIncludeSupport = checkBoxMailIncludeSupport.Checked;
                l_Settings.MailUsername = textBoxMailUsername.Text;
                l_Settings.MailPassword = textBoxMailPassword.Text;
                l_Settings.MailServer = textBoxMailServer.Text;
                l_Settings.MailPort = (int)numericUpDownMailPort.Value;
                //Farmer
                l_Settings.FarmerScheduler = grepSchedulerSmall1.getScheduler();
                l_Settings.FarmerMaxResources = (int)numericUpDownFarmerMaxResources.Value;
                l_Settings.FarmerRandomize = checkBoxFarmerRandomize.Checked;
                //Reconnect
                l_Settings.RecTimerReconnectMin = (int)numericUpDownTimerReconnectMin.Value;
                l_Settings.RecTimerReconnectMax = (int)numericUpDownTimerReconnectMax.Value;
                l_Settings.RecMaxReconnectsEnabled = checkBoxMaxReconnects.Checked;
                l_Settings.RecMaxReconnects = (int)numericUpDownMaxReconnects.Value;
                l_Settings.RecForcedReconnects = checkBoxForcedReconnects.Checked;
                l_Settings.RecMinForcedReconnect = (int)numericUpDownMinForcedReconnect.Value;
                l_Settings.RecMaxForcedReconnect = (int)numericUpDownMaxForcedReconnect.Value;
                //Premium
                l_Settings.PreUseGold = checkBoxUseGold.Checked;
                l_Settings.PreUseFarmAllFeature = checkBoxUseFarmAllFeature.Checked;
                //Sound warning
                l_Settings.SoundIncomingAttack = checkBoxIncomingAttackWarning.Checked;
                l_Settings.SoundCaptchaWarning = checkBoxCaptchaWarning.Checked;
                //Advanced
                l_Settings.AdvMinTimerRefresh = (int)numericUpDownMinTimerRefresh.Value;
                l_Settings.AdvMaxTimerRefresh = (int)numericUpDownMaxTimerRefresh.Value;
                l_Settings.AdvSchedulerBot = checkBoxSchedulerBot.Checked;
                l_Settings.AdvAutoStart = checkBoxAutoStart.Checked;
                l_Settings.AdvAutoPause = checkBoxAutoPause.Checked;
                l_Settings.AdvDebugMode = checkBoxDebugMode.Checked;
                l_Settings.AdvOutputAllMode = checkBoxOutputAllMode.Checked;
                l_Settings.AdvMinDelayRequests = (int)numericUpDownMinDelayRequests.Value;
                l_Settings.AdvMaxDelayRequests = (int)numericUpDownMaxDelayRequests.Value;
                l_Settings.AdvMinDelayFarmers = (int)numericUpDownMinDelayFarmers.Value;
                l_Settings.AdvMaxDelayFarmers = (int)numericUpDownMaxDelayFarmers.Value;
                l_Settings.AdvFarmerLootLag = (int)numericUpDownFarmersLootLag.Value;
                l_Settings.AdvTimeout = (int)numericUpDownTimeout.Value;
                //GUI
                l_Settings.GUIBuildingLevelTargetColor = labelGUIBuildingLevelTargetColor.BackColor;
                l_Settings.GUIBuildingTooltipsEnabled = checkBoxGUIBuildingTooltips.Checked;
                l_Settings.GUITownTabsLinked = checkBoxGUISameTownOnAllTabs.Checked;
                l_Settings.GUIReconnectTimerTitleBar = checkBoxGUIReconnectTimerTitleBar.Checked;
                l_Settings.GUINotificationSize = (int)numericUpDownNotificationSize.Value;
                l_Settings.GUILogSize = (int)numericUpDownLogSize.Value;
                //Captcha
                l_Settings.CaptchaApikey = textBoxCaptchaApikey.Text;
                l_Settings.CaptchaSolverEnabled = checkBoxCaptchaSolverEnabled.Checked;
                l_Settings.CaptchaSelfSolve = checkBoxCaptchaSelfSolve.Checked;
                l_Settings.CaptchaPriority = (int)numericUpDownCaptchaPriority.Value;
                l_Settings.CaptchaMinWorkers = (int)numericUpDownCaptchaMinWorkers.Value;
                l_Settings.CaptchaMinWorkersALT = (int)numericUpDownCaptchaMinWorkersALT.Value;
                l_Settings.CaptchaMaxQueue = (int)numericUpDownCaptchaMaxQueue.Value;
                l_Settings.CaptchaExtraDelay = (int)numericUpDownCaptchaExtraDelay.Value;
                //Feature control
                l_Settings.MasterBuildingQueue = checkBoxMBuildingQueue.Checked;
                l_Settings.MasterUnitQueue = checkBoxMUnitQueue.Checked;
                l_Settings.MasterCulture = checkBoxMCulture.Checked;
                l_Settings.MasterFarmers = checkBoxMFarmers.Checked;
                l_Settings.MasterTrade = checkBoxMTrade.Checked;

                //Save settings to file
                l_IOHandler.saveProgramSettings();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in saveSettings(): " + e.Message);
            }
        }

        #endregion

        #region Login

        private void loginP1()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (!m_Controller.LoggedIn)
                {
                    CookieHelpers.ClearCookie();
                    m_Controller.State = "loginp1";
                    webBrowserGrepo.Navigate("https://" + l_Settings.GenMainServer);
                }
            }
            catch(Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loginP1(): " + e.Message);
            }
        }

        private void loginP2()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (!m_Controller.LoggedIn)
                {
                    m_Controller.State = "loginp2";

                    /*setStatusBar("Connection to: " + l_Settings.GenUserName + "@" + l_Settings.GenServer);
                    //http://###.grepolis.com/start/index?action=login_from_start_page
                    string l_Url = "http://" + l_Settings.GenMainServer + "/start/index?action=login_from_start_page";
                    Uri l_Uri = new Uri(l_Url);

                    System.Text.Encoding l_Encoding = System.Text.Encoding.UTF8;
                    string l_PostData = "json={\"name\":\"" + l_Settings.GenUserName + "\",\"password\":\"" + l_Settings.GenPassword + "\",\"passwordhash\":\"\",\"autologin\":false,\"legacy\":true}";
                    byte[] l_PostDataBytes = l_Encoding.GetBytes(l_PostData);
                    webBrowserGrepo.Navigate(l_Uri, "", l_PostDataBytes, "Content-Type: application/x-www-form-urlencoded" + Environment.NewLine + "X-Requested-With: XMLHttpRequest");*/

                    HtmlElement l_Name = webBrowserGrepo.Document.GetElementById("name");
                    l_Name.InnerText = l_Settings.GenUserName;
                    HtmlElement l_Password = webBrowserGrepo.Document.GetElementById("password");
                    l_Password.InnerText = l_Settings.GenPassword;

                    // Activate javascript for login button
                    webBrowserGrepo.Document.InvokeScript("submit_form_light", new object[] { "loginform" });
                    
                    m_TimerLoginp2.Start();
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loginP2(): " + e.Message);
            }
        }

        private void loginP3()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (!m_Controller.LoggedIn)
                {
                    m_Controller.State = "loginp3";

                    setStatusBar("Connection to: " + l_Settings.GenUserName + "@" + l_Settings.GenServer);
                    //2.10 and earlier --> http://zz.grepolis.com/start?action=login
                    //2.11 and later --> http://zz.grepolis.com/start?action=login_to_game_world

                    string l_Url = "https://" + l_Settings.GenMainServer + "/start?action=login_to_game_world";
                    Uri l_Uri = new Uri(l_Url);
                    String l_ServerNr = "";
                    l_ServerNr = l_Settings.GenServer.Substring(0, l_Settings.GenServer.IndexOf(".", 0));
                    
                    //Needed for kr version of Grepolis
                    int l_Result = 0;
                    while (!int.TryParse(l_ServerNr[l_ServerNr.Length - 1].ToString(), out l_Result))
                    {
                        l_ServerNr = l_ServerNr.Substring(0, l_ServerNr.Length - 1);
                    }
                    //~Needed for kr version of Grepolis

                    System.Text.Encoding l_Encoding = System.Text.Encoding.UTF8;
                    //string l_PostData = "world=" + l_ServerNr + "&facebook_session=&facebook_login=&gift_key=&portal_sid=&name=" + l_Settings.GenUserName + "&password=" + l_Settings.GenPassword;
                    string l_PostData = "world=" + l_ServerNr + "&facebook_session=&facebook_login=&portal_sid=&name=" + l_Settings.GenUserName + "&password=" + l_Settings.GenPassword;
                    byte[] l_PostDataBytes = l_Encoding.GetBytes(l_PostData);
                    webBrowserGrepo.Navigate(l_Uri, "", l_PostDataBytes, "Content-Type: application/x-www-form-urlencoded");
                    //webBrowserGrepo.Navigate("http://whatsmyuseragent.com");
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in loginP3(): " + e.Message);
            }
        }

        #endregion

        #region GUI Updates

        private void updateQueueGUI()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            //Note Modify this when adding new units (01/15)

            try
            {
                if (!m_SelectedTownQueue.Equals("0"))
                {
                    flowLayoutPanelQueue.Invoke(new MethodInvoker(delegate
                    {
                        int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownQueue);

                        checkBoxUnitQueueEnabled.Checked = m_Controller.Player.Towns[l_Index].UnitQueueEnabled;
                        grepUnits2.TownID = m_SelectedTownQueue;
                        grepUnits2.TownName = m_Controller.Player.Towns[l_Index].Name;
                        grepUnits2.setUnitNames(m_Controller.Player.Towns[l_Index].getUnitNames());
                        grepUnits2.setTotalUnits("sword",m_Controller.Player.Towns[l_Index].getUnitTotalAmount("sword").ToString());
                        grepUnits2.setTotalUnits("slinger", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("slinger").ToString());
                        grepUnits2.setTotalUnits("archer", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("archer").ToString());
                        grepUnits2.setTotalUnits("hoplite", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("hoplite").ToString());
                        grepUnits2.setTotalUnits("rider", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("rider").ToString());
                        grepUnits2.setTotalUnits("chariot", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("chariot").ToString());
                        grepUnits2.setTotalUnits("catapult", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("catapult").ToString());
                        grepUnits2.setTotalUnits("minotaur", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("minotaur").ToString());
                        grepUnits2.setTotalUnits("manticore", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("manticore").ToString());
                        grepUnits2.setTotalUnits("centaur", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("centaur").ToString());
                        grepUnits2.setTotalUnits("pegasus", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("pegasus").ToString());
                        grepUnits2.setTotalUnits("harpy", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("harpy").ToString());
                        grepUnits2.setTotalUnits("medusa", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("medusa").ToString());
                        grepUnits2.setTotalUnits("zyklop", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("zyklop").ToString());
                        grepUnits2.setTotalUnits("cerberus", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("cerberus").ToString());
                        grepUnits2.setTotalUnits("fury", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("fury").ToString());
                        grepUnits2.setTotalUnits("griffin", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("griffin").ToString());
                        grepUnits2.setTotalUnits("calydonian_boar", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("calydonian_boar").ToString());
                        grepUnits2.setTotalUnits("godsent", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("godsent").ToString());
                        grepUnits2.setTotalUnits("big_transporter", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("big_transporter").ToString());
                        grepUnits2.setTotalUnits("bireme", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("bireme").ToString());
                        grepUnits2.setTotalUnits("attack_ship", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("attack_ship").ToString());
                        grepUnits2.setTotalUnits("demolition_ship", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("demolition_ship").ToString());
                        grepUnits2.setTotalUnits("small_transporter", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("small_transporter").ToString());
                        grepUnits2.setTotalUnits("trireme", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("trireme").ToString());
                        grepUnits2.setTotalUnits("colonize_ship", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("colonize_ship").ToString());
                        grepUnits2.setTotalUnits("sea_monster", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("sea_monster").ToString());

                        grepUnits2.setUnitQueue("sword", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("sword"));
                        grepUnits2.setUnitQueue("slinger", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("slinger"));
                        grepUnits2.setUnitQueue("archer", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("archer"));
                        grepUnits2.setUnitQueue("hoplite", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("hoplite"));
                        grepUnits2.setUnitQueue("rider", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("rider"));
                        grepUnits2.setUnitQueue("chariot", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("chariot"));
                        grepUnits2.setUnitQueue("catapult", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("catapult"));
                        grepUnits2.setUnitQueue("minotaur", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("minotaur"));
                        grepUnits2.setUnitQueue("manticore", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("manticore"));
                        grepUnits2.setUnitQueue("centaur", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("centaur"));
                        grepUnits2.setUnitQueue("pegasus", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("pegasus"));
                        grepUnits2.setUnitQueue("harpy", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("harpy"));
                        grepUnits2.setUnitQueue("medusa", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("medusa"));
                        grepUnits2.setUnitQueue("zyklop", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("zyklop"));
                        grepUnits2.setUnitQueue("cerberus", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("cerberus"));
                        grepUnits2.setUnitQueue("fury", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("fury"));
                        grepUnits2.setUnitQueue("griffin", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("griffin"));
                        grepUnits2.setUnitQueue("calydonian_boar", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("calydonian_boar"));
                        grepUnits2.setUnitQueue("godsent", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("godsent"));
                        grepUnits2.setUnitQueue("big_transporter", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("big_transporter"));
                        grepUnits2.setUnitQueue("bireme", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("bireme"));
                        grepUnits2.setUnitQueue("attack_ship", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("attack_ship"));
                        grepUnits2.setUnitQueue("demolition_ship", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("demolition_ship"));
                        grepUnits2.setUnitQueue("small_transporter", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("small_transporter"));
                        grepUnits2.setUnitQueue("trireme", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("trireme"));
                        grepUnits2.setUnitQueue("colonize_ship", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("colonize_ship"));
                        grepUnits2.setUnitQueue("sea_monster", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("sea_monster"));

                        grepUnits2.setUnitGameQueue("sword", m_Controller.Player.Towns[l_Index].getUnitQueueGame("sword"));
                        grepUnits2.setUnitGameQueue("slinger", m_Controller.Player.Towns[l_Index].getUnitQueueGame("slinger"));
                        grepUnits2.setUnitGameQueue("archer", m_Controller.Player.Towns[l_Index].getUnitQueueGame("archer"));
                        grepUnits2.setUnitGameQueue("hoplite", m_Controller.Player.Towns[l_Index].getUnitQueueGame("hoplite"));
                        grepUnits2.setUnitGameQueue("rider", m_Controller.Player.Towns[l_Index].getUnitQueueGame("rider"));
                        grepUnits2.setUnitGameQueue("chariot", m_Controller.Player.Towns[l_Index].getUnitQueueGame("chariot"));
                        grepUnits2.setUnitGameQueue("catapult", m_Controller.Player.Towns[l_Index].getUnitQueueGame("catapult"));
                        grepUnits2.setUnitGameQueue("minotaur", m_Controller.Player.Towns[l_Index].getUnitQueueGame("minotaur"));
                        grepUnits2.setUnitGameQueue("manticore", m_Controller.Player.Towns[l_Index].getUnitQueueGame("manticore"));
                        grepUnits2.setUnitGameQueue("centaur", m_Controller.Player.Towns[l_Index].getUnitQueueGame("centaur"));
                        grepUnits2.setUnitGameQueue("pegasus", m_Controller.Player.Towns[l_Index].getUnitQueueGame("pegasus"));
                        grepUnits2.setUnitGameQueue("harpy", m_Controller.Player.Towns[l_Index].getUnitQueueGame("harpy"));
                        grepUnits2.setUnitGameQueue("medusa", m_Controller.Player.Towns[l_Index].getUnitQueueGame("medusa"));
                        grepUnits2.setUnitGameQueue("zyklop", m_Controller.Player.Towns[l_Index].getUnitQueueGame("zyklop"));
                        grepUnits2.setUnitGameQueue("cerberus", m_Controller.Player.Towns[l_Index].getUnitQueueGame("cerberus"));
                        grepUnits2.setUnitGameQueue("fury", m_Controller.Player.Towns[l_Index].getUnitQueueGame("fury"));
                        grepUnits2.setUnitGameQueue("griffin", m_Controller.Player.Towns[l_Index].getUnitQueueGame("griffin"));
                        grepUnits2.setUnitGameQueue("calydonian_boar", m_Controller.Player.Towns[l_Index].getUnitQueueGame("calydonian_boar"));
                        grepUnits2.setUnitGameQueue("godsent", m_Controller.Player.Towns[l_Index].getUnitQueueGame("godsent"));
                        grepUnits2.setUnitGameQueue("big_transporter", m_Controller.Player.Towns[l_Index].getUnitQueueGame("big_transporter"));
                        grepUnits2.setUnitGameQueue("bireme", m_Controller.Player.Towns[l_Index].getUnitQueueGame("bireme"));
                        grepUnits2.setUnitGameQueue("attack_ship", m_Controller.Player.Towns[l_Index].getUnitQueueGame("attack_ship"));
                        grepUnits2.setUnitGameQueue("demolition_ship", m_Controller.Player.Towns[l_Index].getUnitQueueGame("demolition_ship"));
                        grepUnits2.setUnitGameQueue("small_transporter", m_Controller.Player.Towns[l_Index].getUnitQueueGame("small_transporter"));
                        grepUnits2.setUnitGameQueue("trireme", m_Controller.Player.Towns[l_Index].getUnitQueueGame("trireme"));
                        grepUnits2.setUnitGameQueue("colonize_ship", m_Controller.Player.Towns[l_Index].getUnitQueueGame("colonize_ship"));
                        grepUnits2.setUnitGameQueue("sea_monster", m_Controller.Player.Towns[l_Index].getUnitQueueGame("sea_monster"));

                        checkBoxBuildingQueueEnabled.Checked = m_Controller.Player.Towns[l_Index].BuildingQueueEnabled;
                        checkBoxBuildingQueueTarget.Checked = m_Controller.Player.Towns[l_Index].BuildingLevelsTargetEnabled;
                        checkBoxBuildingDowngrade.Checked = m_Controller.Player.Towns[l_Index].BuildingDowngradeEnabled;
                        grepBuildings2.setHeroMode(l_Settings.GenIsHeroWorld);//Call this method before setBuildingLevelsTarget
                        grepBuildings2.setTownName(m_Controller.Player.Towns[l_Index].Name);
                        grepBuildings2.TooltipsEnabled = l_Settings.GUIBuildingTooltipsEnabled;
                        grepBuildings2.setBuildingNames(m_Controller.Player.Towns[l_Index].getBuildingNames());
                        grepBuildings2.setBuildingLevelsTarget(m_Controller.Player.Towns[l_Index].getBuildingLevelsTarget());
                        grepBuildings2.LevelTargetColor = l_Settings.GUIBuildingLevelTargetColor;
                        grepBuildings2.setBuildingLevels(m_Controller.Player.Towns[l_Index].getBuildingLevels());//Call this method after setBuildingLevelsTarget
                        grepBuildings2.setBuildingQueueGame(m_Controller.Player.Towns[l_Index].getIngameBuildingQueue());
                        grepBuildings2.setBuildingQueueBot(m_Controller.Player.Towns[l_Index].getBuildingQueue());//Call this method after setBuildingLevelsTarget
                        
                        //Set resources
                        string[] l_Resources = new string[] { "", "" };//dynamic array
                        l_Resources = m_Controller.Player.Towns[l_Index].getResourcesString().Split(';');
                        labelWoodAmountQueue.Text = l_Resources[0] + " / " + m_Controller.Player.Towns[l_Index].Storage;
                        labelStoneAmountQueue.Text = l_Resources[1] + " / " + m_Controller.Player.Towns[l_Index].Storage;
                        labelIronAmountQueue.Text = l_Resources[2] + " / " + m_Controller.Player.Towns[l_Index].Storage;

                        //Set favor
                        labelFavorAmountQueue.Text = m_Controller.Player.getFavorByTownIndex(l_Index);

                        //Set god
                        switch (m_Controller.Player.Towns[l_Index].God)
                        {
                            case "none":
                                labelGodImgQueue.Image = global::GrepolisBot2.Properties.Resources.god_mini;
                                break;
                            case "zeus":
                                labelGodImgQueue.Image = global::GrepolisBot2.Properties.Resources.zeus_mini;
                                break;
                            case "poseidon":
                                labelGodImgQueue.Image = global::GrepolisBot2.Properties.Resources.poseidon_mini;
                                break;
                            case "hera":
                                labelGodImgQueue.Image = global::GrepolisBot2.Properties.Resources.hera_mini;
                                break;
                            case "athena":
                                labelGodImgQueue.Image = global::GrepolisBot2.Properties.Resources.athena_mini;
                                break;
                            case "hades":
                                labelGodImgQueue.Image = global::GrepolisBot2.Properties.Resources.hades_mini;
                                break;
                            case "artemis":
                                labelGodImgQueue.Image = global::GrepolisBot2.Properties.Resources.artemis_mini;
                                break;
                        }

                        //Set population
                        setQueuePopulationCalculations();

                        //Set militia
                        numericUpDownMilitiaTrigger.Value = m_Controller.Player.Towns[l_Index].MilitiaTrigger;
                    }));
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateQueueGUI(): " + e.Message);
            }
        }

        private void setQueuePopulationCalculations()
        {
            if (m_SelectedTownQueue != "0")
            {
                int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownQueue);
                string l_Remaining = m_Controller.Player.Towns[l_Index].PopulationAvailable.ToString() + " (" + m_Controller.Player.Towns[l_Index].Buildings[m_Controller.Player.Towns[l_Index].getIndexBuilding("farm")].Level + ")";
                string l_LandUnits = m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].getTotalLandUnitPopulation();
                string l_TransportCapacity = m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].getTotalTransportCapacity();
                int l_TotalUnits = m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].getTotalUnitPopulation();
                int l_TotalBuildings = m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].getTotalBuildingPopulation();
                int l_Farmer = 0;
                //double l_FarmerExact = (int)(14 * Math.Pow(m_Controller.Player.Towns[l_Index].Buildings[m_Controller.Player.Towns[l_Index].getIndexBuilding("farm")].TargetLevel,1.455));
                double l_FarmerExact = (14 * Math.Pow(m_Controller.Player.Towns[l_Index].Buildings[m_Controller.Player.Towns[l_Index].getIndexBuilding("farm")].TargetLevel, 1.455));
                if (m_Controller.Player.Towns[l_Index].Buildings[m_Controller.Player.Towns[l_Index].getIndexBuilding("thermal")].TargetLevel == 1)
                    l_FarmerExact = l_FarmerExact * 1.1;
                //l_Farmer = (int)l_FarmerExact;
                l_Farmer = (int)(l_FarmerExact + 0.5);

                labelPopulationAmountRemainingQueue.Text = l_Remaining;
                labelPopulationAmountRemainingTrade.Text = l_Remaining;

                labelPopulationAmountRemainingCombo.Text = l_Remaining;
                labelPopulationAmountRemainingCombo2.Text = l_Remaining;

                //queue tab
                labelLandUnitPopulationQueue.Text = l_LandUnits;
                labelTransportCapacityQueue.Text = l_TransportCapacity;
                labelTotalUnitPopulationQueue.Text = l_TotalUnits.ToString();
                labelTotalBuildingPopulationQueue.Text = l_TotalBuildings.ToString();
                labelTotalPopulationQueue.Text = (l_TotalUnits + l_TotalBuildings).ToString();
                labelRemainingPopulationQueue.Text = (l_Farmer - l_TotalUnits - l_TotalBuildings + 200).ToString() + " (" + (l_Farmer - l_TotalUnits - l_TotalBuildings).ToString() + ")";
                //combo tab
                labelLandUnitPopulationCombo.Text = l_LandUnits;
                labelTransportCapacityCombo.Text = l_TransportCapacity;
                labelTotalUnitPopulationCombo.Text = l_TotalUnits.ToString();
                labelTotalBuildingPopulationCombo.Text = l_TotalBuildings.ToString();
                labelTotalPopulationCombo.Text = (l_TotalUnits + l_TotalBuildings).ToString();
                labelRemainingPopulationCombo.Text = (l_Farmer - l_TotalUnits - l_TotalBuildings + 200).ToString() + " (" + (l_Farmer - l_TotalUnits - l_TotalBuildings).ToString() + ")";
            }
        }

        public void updateOverviewGUI()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            webBrowserOverview.Invoke(new MethodInvoker(delegate
            {
                    l_IOHandler.createHtmlOverview(m_Controller.Player);
                    webBrowserOverview.Navigate(Application.StartupPath + "\\" + l_Settings.AdvOverviewFile);
            }));
        }

        private void updateCulturalGUI()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (labelCulturalLevel.InvokeRequired)
                    labelCulturalLevel.Invoke(new MethodInvoker(delegate { labelCulturalLevel.Text = m_Controller.Player.CulturalLevelStr; }));
                else
                    labelCulturalLevel.Text = m_Controller.Player.CulturalLevelStr;
                if (labelCulturalCities.InvokeRequired)
                    labelCulturalCities.Invoke(new MethodInvoker(delegate { labelCulturalCities.Text = m_Controller.Player.CulturalCitiesStr; }));
                else
                    labelCulturalCities.Text = m_Controller.Player.CulturalCitiesStr;
                if (progressBarCP.InvokeRequired)
                    progressBarCP.Invoke(new MethodInvoker(delegate { progressBarCP.Maximum = m_Controller.Player.CulturalPointsMax; progressBarCP.Value = m_Controller.Player.CulturalPointsCurrent; }));
                else
                {
                    progressBarCP.Maximum = m_Controller.Player.CulturalPointsMax;
                    progressBarCP.Value = m_Controller.Player.CulturalPointsCurrent;
                }

                if (progressBarCP.InvokeRequired)
                    progressBarCP.Invoke(new MethodInvoker(delegate { m_Tooltip.SetToolTip(progressBarCP, m_Controller.Player.CulturalPointsStr); }));
                else
                    m_Tooltip.SetToolTip(progressBarCP, m_Controller.Player.CulturalPointsStr);

                if (!m_SelectedTownCulture.Equals("0"))
                {
                    flowLayoutPanelCultureFestivals.Invoke(new MethodInvoker(delegate
                    {
                        int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownCulture);

                        checkBoxCultureEnabled.Checked = m_Controller.Player.Towns[l_Index].CulturalFestivalsEnabled;
                        grepCulture1.EnabledParty = m_Controller.Player.Towns[l_Index].CulturalPartyEnabled;
                        grepCulture1.EnabledGames = m_Controller.Player.Towns[l_Index].CulturalGamesEnabled;
                        grepCulture1.EnabledTriumph = m_Controller.Player.Towns[l_Index].CulturalTriumphEnabled;
                        grepCulture1.EnabledTheater = m_Controller.Player.Towns[l_Index].CulturalTheaterEnabled;
                        grepCulture1.TownName = m_Controller.Player.Towns[l_Index].Name;
                        grepCulture1.TownID = m_Controller.Player.Towns[l_Index].TownID;
                    }));
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateCulturalGUI(): " + e.Message);
            }
        }

        private void updateFarmersGUI()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (!m_SelectedTownFarmers.Equals("0") && m_Controller.CanIModifyFarmers)
                {
                    flowLayoutPanelFarmers.Invoke(new MethodInvoker(delegate
                    {
                        int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownFarmers);
                        if (m_Controller.Player.Towns[l_Index].Farmers.Count > 0)
                        {
                            //Selected farmers
                            grepFarmers1.showFarmers();
                            grepFarmers1.TownName = m_Controller.Player.Towns[l_Index].Name;
                            grepFarmers1.setSelectedFarmers(m_Controller.Player.Towns[l_Index].getSelectedFarmers());

                            //Farmers details
                            grepFarmers1.setFarmersName(m_Controller.Player.Towns[l_Index].getFarmersName());
                            grepFarmers1.setFarmersStatus(m_Controller.Player.Towns[l_Index].getFarmersRelation(), m_Controller.Player.Towns[l_Index].getFarmersLimitReached());
                            grepFarmers1.setFarmersLootTime(m_Controller.Player.Towns[l_Index].getFarmersLootTime());
                            grepFarmers1.setFarmersMood(m_Controller.Player.Towns[l_Index].getFarmersMood());
                            grepFarmers1.setFarmersLimit(m_Controller.Player.Towns[l_Index].getFarmersLimit());

                            //Enabled
                            checkBoxFarmersLootEnabled.Checked = m_Controller.Player.Towns[l_Index].FarmersLootEnabled;

                            //Friendly demands only
                            checkBoxFriendlyDemandsOnly.Checked = m_Controller.Player.Towns[l_Index].FarmersFriendlyDemandsOnly;

                            //Min. mood
                            numericUpDownFarmersMinMood.Value = m_Controller.Player.Towns[l_Index].FarmersMinMood;

                            //Loot interval
                            domainUpDownLootInterval.Text = m_Controller.Player.Towns[l_Index].FarmersLootInterval;

                            //Towns on Island
                            textBoxFarmersTownsOnIsland.Text = m_Controller.Player.getTownsOnIsland(m_Controller.Player.Towns[l_Index].IslandX, m_Controller.Player.Towns[l_Index].IslandY);

                            //Farmer (global settings)
                            grepSchedulerSmall1.setScheduler(l_Settings.FarmerScheduler);
                            numericUpDownFarmerMaxResources.Value = l_Settings.FarmerMaxResources;
                        }
                        else
                        {
                            grepFarmers1.hideFarmers();
                            grepFarmers1.TownName = m_Controller.Player.Towns[l_Index].Name;
                        }
                    }));
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateFarmersGUI(): " + e.Message);
            }
        }

        private void updateTradeGUI()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (!m_SelectedTownTrade.Equals("0"))
                {
                    groupBoxTrading.Invoke(new MethodInvoker(delegate
                    {
                        int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownTrade);
                        groupBoxTrading.Text = "Trading - " + m_Controller.Player.Towns[l_Index].Name;

                        checkBoxTradeEnabled.Checked = m_Controller.Player.Towns[l_Index].TradeEnabled;
                        if (m_Controller.Player.Towns[l_Index].TradeMode.Equals("send"))
                            comboBoxTradeMode.SelectedIndex = 0;
                        else if (m_Controller.Player.Towns[l_Index].TradeMode.Equals("receive"))
                            comboBoxTradeMode.SelectedIndex = 1;
                        else
                            comboBoxTradeMode.SelectedIndex = 2;
                        //comboBoxTradeMode.SelectedText = m_Controller.Player.Towns[l_Index].TradeMode;
                        numericUpDownTradeRemainingResources.Value = m_Controller.Player.Towns[l_Index].TradeRemainingResources;
                        numericUpDownTradePercentageWarehouse.Value = m_Controller.Player.Towns[l_Index].TradePercentageWarehouse;
                        numericUpDownTradeMinSendAmount.Value = m_Controller.Player.Towns[l_Index].TradeMinSendAmount;

                        //Maximum trade distance
                        numericUpDownTradeMaxDistance.Value = m_Controller.Player.Towns[l_Index].TradeMaxDistance;
                        textBoxTradeTownsInRange.Text = m_Controller.Player.getTradeTownsInRange(l_Index);

                        //Set population
                        setQueuePopulationCalculations();
                    }));
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateTradeGUI(): " + e.Message);
            }
        }

        private void updateComboGUI()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            //Note Modify this when adding new units (02/15)

            try
            {
                if (!m_SelectedTownCombo.Equals("0"))
                {
                    //flowLayoutPanelCombo.Invoke(new MethodInvoker(delegate
                    //Do not comment out invoke method. Causes Cross-thread operation exceptions when GUI elements are accessed from a thread other than the thread they were created on.
                    tabPageCombo.Invoke(new MethodInvoker(delegate
                    {
                        int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownCombo);

                        comboBoxTownListCombo.SelectedIndex = l_Index;

                        //Building/unit queue related
                        checkBoxUnitQueueEnabledCombo.Checked = m_Controller.Player.Towns[l_Index].UnitQueueEnabled;
                        grepUnitsCombo.TownID = m_SelectedTownQueue;
                        grepUnitsCombo.TownName = m_Controller.Player.Towns[l_Index].Name;
                        grepUnitsCombo.setUnitNames(m_Controller.Player.Towns[l_Index].getUnitNames());
                        grepUnitsCombo.setTotalUnits("sword", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("sword").ToString());
                        grepUnitsCombo.setTotalUnits("slinger", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("slinger").ToString());
                        grepUnitsCombo.setTotalUnits("archer", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("archer").ToString());
                        grepUnitsCombo.setTotalUnits("hoplite", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("hoplite").ToString());
                        grepUnitsCombo.setTotalUnits("rider", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("rider").ToString());
                        grepUnitsCombo.setTotalUnits("chariot", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("chariot").ToString());
                        grepUnitsCombo.setTotalUnits("catapult", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("catapult").ToString());
                        grepUnitsCombo.setTotalUnits("minotaur", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("minotaur").ToString());
                        grepUnitsCombo.setTotalUnits("manticore", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("manticore").ToString());
                        grepUnitsCombo.setTotalUnits("centaur", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("centaur").ToString());
                        grepUnitsCombo.setTotalUnits("pegasus", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("pegasus").ToString());
                        grepUnitsCombo.setTotalUnits("harpy", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("harpy").ToString());
                        grepUnitsCombo.setTotalUnits("medusa", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("medusa").ToString());
                        grepUnitsCombo.setTotalUnits("zyklop", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("zyklop").ToString());
                        grepUnitsCombo.setTotalUnits("cerberus", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("cerberus").ToString());
                        grepUnitsCombo.setTotalUnits("fury", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("fury").ToString());
                        grepUnitsCombo.setTotalUnits("griffin", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("griffin").ToString());
                        grepUnitsCombo.setTotalUnits("calydonian_boar", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("calydonian_boar").ToString());
                        grepUnitsCombo.setTotalUnits("godsent", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("godsent").ToString());
                        grepUnitsCombo.setTotalUnits("big_transporter", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("big_transporter").ToString());
                        grepUnitsCombo.setTotalUnits("bireme", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("bireme").ToString());
                        grepUnitsCombo.setTotalUnits("attack_ship", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("attack_ship").ToString());
                        grepUnitsCombo.setTotalUnits("demolition_ship", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("demolition_ship").ToString());
                        grepUnitsCombo.setTotalUnits("small_transporter", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("small_transporter").ToString());
                        grepUnitsCombo.setTotalUnits("trireme", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("trireme").ToString());
                        grepUnitsCombo.setTotalUnits("colonize_ship", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("colonize_ship").ToString());
                        grepUnitsCombo.setTotalUnits("sea_monster", m_Controller.Player.Towns[l_Index].getUnitTotalAmount("sea_monster").ToString());

                        grepUnitsCombo.setUnitQueue("sword", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("sword"));
                        grepUnitsCombo.setUnitQueue("slinger", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("slinger"));
                        grepUnitsCombo.setUnitQueue("archer", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("archer"));
                        grepUnitsCombo.setUnitQueue("hoplite", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("hoplite"));
                        grepUnitsCombo.setUnitQueue("rider", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("rider"));
                        grepUnitsCombo.setUnitQueue("chariot", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("chariot"));
                        grepUnitsCombo.setUnitQueue("catapult", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("catapult"));
                        grepUnitsCombo.setUnitQueue("minotaur", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("minotaur"));
                        grepUnitsCombo.setUnitQueue("manticore", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("manticore"));
                        grepUnitsCombo.setUnitQueue("centaur", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("centaur"));
                        grepUnitsCombo.setUnitQueue("pegasus", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("pegasus"));
                        grepUnitsCombo.setUnitQueue("harpy", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("harpy"));
                        grepUnitsCombo.setUnitQueue("medusa", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("medusa"));
                        grepUnitsCombo.setUnitQueue("zyklop", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("zyklop"));
                        grepUnitsCombo.setUnitQueue("cerberus", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("cerberus"));
                        grepUnitsCombo.setUnitQueue("fury", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("fury"));
                        grepUnitsCombo.setUnitQueue("griffin", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("griffin"));
                        grepUnitsCombo.setUnitQueue("calydonian_boar", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("calydonian_boar"));
                        grepUnitsCombo.setUnitQueue("godsent", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("godsent"));
                        grepUnitsCombo.setUnitQueue("big_transporter", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("big_transporter"));
                        grepUnitsCombo.setUnitQueue("bireme", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("bireme"));
                        grepUnitsCombo.setUnitQueue("attack_ship", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("attack_ship"));
                        grepUnitsCombo.setUnitQueue("demolition_ship", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("demolition_ship"));
                        grepUnitsCombo.setUnitQueue("small_transporter", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("small_transporter"));
                        grepUnitsCombo.setUnitQueue("trireme", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("trireme"));
                        grepUnitsCombo.setUnitQueue("colonize_ship", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("colonize_ship"));
                        grepUnitsCombo.setUnitQueue("sea_monster", m_Controller.Player.Towns[l_Index].getUnitTargetAmount("sea_monster"));

                        grepUnitsCombo.setUnitGameQueue("sword", m_Controller.Player.Towns[l_Index].getUnitQueueGame("sword"));
                        grepUnitsCombo.setUnitGameQueue("slinger", m_Controller.Player.Towns[l_Index].getUnitQueueGame("slinger"));
                        grepUnitsCombo.setUnitGameQueue("archer", m_Controller.Player.Towns[l_Index].getUnitQueueGame("archer"));
                        grepUnitsCombo.setUnitGameQueue("hoplite", m_Controller.Player.Towns[l_Index].getUnitQueueGame("hoplite"));
                        grepUnitsCombo.setUnitGameQueue("rider", m_Controller.Player.Towns[l_Index].getUnitQueueGame("rider"));
                        grepUnitsCombo.setUnitGameQueue("chariot", m_Controller.Player.Towns[l_Index].getUnitQueueGame("chariot"));
                        grepUnitsCombo.setUnitGameQueue("catapult", m_Controller.Player.Towns[l_Index].getUnitQueueGame("catapult"));
                        grepUnitsCombo.setUnitGameQueue("minotaur", m_Controller.Player.Towns[l_Index].getUnitQueueGame("minotaur"));
                        grepUnitsCombo.setUnitGameQueue("manticore", m_Controller.Player.Towns[l_Index].getUnitQueueGame("manticore"));
                        grepUnitsCombo.setUnitGameQueue("centaur", m_Controller.Player.Towns[l_Index].getUnitQueueGame("centaur"));
                        grepUnitsCombo.setUnitGameQueue("pegasus", m_Controller.Player.Towns[l_Index].getUnitQueueGame("pegasus"));
                        grepUnitsCombo.setUnitGameQueue("harpy", m_Controller.Player.Towns[l_Index].getUnitQueueGame("harpy"));
                        grepUnitsCombo.setUnitGameQueue("medusa", m_Controller.Player.Towns[l_Index].getUnitQueueGame("medusa"));
                        grepUnitsCombo.setUnitGameQueue("zyklop", m_Controller.Player.Towns[l_Index].getUnitQueueGame("zyklop"));
                        grepUnitsCombo.setUnitGameQueue("cerberus", m_Controller.Player.Towns[l_Index].getUnitQueueGame("cerberus"));
                        grepUnitsCombo.setUnitGameQueue("fury", m_Controller.Player.Towns[l_Index].getUnitQueueGame("fury"));
                        grepUnitsCombo.setUnitGameQueue("griffin", m_Controller.Player.Towns[l_Index].getUnitQueueGame("griffin"));
                        grepUnitsCombo.setUnitGameQueue("calydonian_boar", m_Controller.Player.Towns[l_Index].getUnitQueueGame("calydonian_boar"));
                        grepUnitsCombo.setUnitGameQueue("godsent", m_Controller.Player.Towns[l_Index].getUnitQueueGame("godsent"));
                        grepUnitsCombo.setUnitGameQueue("big_transporter", m_Controller.Player.Towns[l_Index].getUnitQueueGame("big_transporter"));
                        grepUnitsCombo.setUnitGameQueue("bireme", m_Controller.Player.Towns[l_Index].getUnitQueueGame("bireme"));
                        grepUnitsCombo.setUnitGameQueue("attack_ship", m_Controller.Player.Towns[l_Index].getUnitQueueGame("attack_ship"));
                        grepUnitsCombo.setUnitGameQueue("demolition_ship", m_Controller.Player.Towns[l_Index].getUnitQueueGame("demolition_ship"));
                        grepUnitsCombo.setUnitGameQueue("small_transporter", m_Controller.Player.Towns[l_Index].getUnitQueueGame("small_transporter"));
                        grepUnitsCombo.setUnitGameQueue("trireme", m_Controller.Player.Towns[l_Index].getUnitQueueGame("trireme"));
                        grepUnitsCombo.setUnitGameQueue("colonize_ship", m_Controller.Player.Towns[l_Index].getUnitQueueGame("colonize_ship"));
                        grepUnitsCombo.setUnitGameQueue("sea_monster", m_Controller.Player.Towns[l_Index].getUnitQueueGame("sea_monster"));

                        checkBoxBuildingQueueEnabledCombo.Checked = m_Controller.Player.Towns[l_Index].BuildingQueueEnabled;
                        checkBoxBuildingQueueTargetCombo.Checked = m_Controller.Player.Towns[l_Index].BuildingLevelsTargetEnabled;
                        checkBoxBuildingDowngradeCombo.Checked = m_Controller.Player.Towns[l_Index].BuildingDowngradeEnabled;
                        grepBuildingsCombo.setHeroMode(l_Settings.GenIsHeroWorld);//Call this method before setBuildingLevelsTarget
                        grepBuildingsCombo.setTownName(m_Controller.Player.Towns[l_Index].Name);
                        grepBuildingsCombo.TooltipsEnabled = l_Settings.GUIBuildingTooltipsEnabled;
                        grepBuildingsCombo.setBuildingNames(m_Controller.Player.Towns[l_Index].getBuildingNames());
                        grepBuildingsCombo.setBuildingLevelsTarget(m_Controller.Player.Towns[l_Index].getBuildingLevelsTarget());
                        grepBuildingsCombo.LevelTargetColor = l_Settings.GUIBuildingLevelTargetColor;
                        grepBuildingsCombo.setBuildingLevels(m_Controller.Player.Towns[l_Index].getBuildingLevels());//Call this method after setBuildingLevelsTarget
                        grepBuildingsCombo.setBuildingQueueGame(m_Controller.Player.Towns[l_Index].getIngameBuildingQueue());
                        grepBuildingsCombo.setBuildingQueueBot(m_Controller.Player.Towns[l_Index].getBuildingQueue());//Call this method after setBuildingLevelsTarget


                        //Set resources/favor/god
                        string[] l_Resources = new string[] { "", "" };//dynamic array
                        l_Resources = m_Controller.Player.Towns[l_Index].getResourcesString().Split(';');

                        labelWarehouseMax.Text = "" + m_Controller.Player.Towns[l_Index].Storage;
                        labelWoodAmountCombo.Text = l_Resources[0];
                        labelStoneAmountCombo.Text = l_Resources[1];
                        labelIronAmountCombo.Text = l_Resources[2];

                        labelFavorAmountCombo.Text = m_Controller.Player.getFavorByTownIndex(l_Index);

                        switch (m_Controller.Player.Towns[l_Index].God)
                        {
                            case "none":
                                labelGodImgCombo.Image = global::GrepolisBot2.Properties.Resources.god_mini;
                                break;
                            case "zeus":
                                labelGodImgCombo.Image = global::GrepolisBot2.Properties.Resources.zeus_mini;
                                break;
                            case "poseidon":
                                labelGodImgCombo.Image = global::GrepolisBot2.Properties.Resources.poseidon_mini;
                                break;
                            case "hera":
                                labelGodImgCombo.Image = global::GrepolisBot2.Properties.Resources.hera_mini;
                                break;
                            case "athena":
                                labelGodImgCombo.Image = global::GrepolisBot2.Properties.Resources.athena_mini;
                                break;
                            case "hades":
                                labelGodImgCombo.Image = global::GrepolisBot2.Properties.Resources.hades_mini;
                                break;
                            case "artemis":
                                labelGodImgCombo.Image = global::GrepolisBot2.Properties.Resources.artemis_mini;
                                break;
                        }

                        //Set population
                        setQueuePopulationCalculations();

                        //Set militia
                        numericUpDownMilitiaTriggerCombo.Value = m_Controller.Player.Towns[l_Index].MilitiaTrigger;

                        //Culture related
                        checkBoxCultureEnabledCombo.Checked = m_Controller.Player.Towns[l_Index].CulturalFestivalsEnabled;
                        grepCultureCombo.EnabledParty = m_Controller.Player.Towns[l_Index].CulturalPartyEnabled;
                        grepCultureCombo.EnabledGames = m_Controller.Player.Towns[l_Index].CulturalGamesEnabled;
                        grepCultureCombo.EnabledTriumph = m_Controller.Player.Towns[l_Index].CulturalTriumphEnabled;
                        grepCultureCombo.EnabledTheater = m_Controller.Player.Towns[l_Index].CulturalTheaterEnabled;
                        grepCultureCombo.TownName = m_Controller.Player.Towns[l_Index].Name;
                        grepCultureCombo.TownID = m_Controller.Player.Towns[l_Index].TownID;

                        //Trade related
                        groupBoxTradingCombo.Text = "Trading - " + m_Controller.Player.Towns[l_Index].Name;

                        checkBoxTradeEnabledCombo.Checked = m_Controller.Player.Towns[l_Index].TradeEnabled;
                        if (m_Controller.Player.Towns[l_Index].TradeMode.Equals("send"))
                            comboBoxTradeModeCombo.SelectedIndex = 0;
                        else if (m_Controller.Player.Towns[l_Index].TradeMode.Equals("receive"))
                            comboBoxTradeModeCombo.SelectedIndex = 1;
                        else
                            comboBoxTradeModeCombo.SelectedIndex = 2;
                        numericUpDownTradeRemainingResourcesCombo.Value = m_Controller.Player.Towns[l_Index].TradeRemainingResources;
                        numericUpDownTradePercentageWarehouseCombo.Value = m_Controller.Player.Towns[l_Index].TradePercentageWarehouse;
                        numericUpDownTradeMinSendAmountCombo.Value = m_Controller.Player.Towns[l_Index].TradeMinSendAmount;
                        numericUpDownTradeMaxDistanceCombo.Value = m_Controller.Player.Towns[l_Index].TradeMaxDistance;
                        m_Tooltip.SetToolTip(labelMaximumTradeDistanceCombo, m_Controller.Player.getTradeTownsInRange(l_Index));
                        checkBoxTradeOmitFromChangeAll.Checked = m_Controller.Player.Towns[l_Index].TradeOmitFromChangeAll;
                    }));
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateComboGUI(): " + e.Message);
            }
        }

        private void updateNotificationGUI()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (textBoxNotifications.InvokeRequired)
                {
                    textBoxNotifications.Invoke(new MethodInvoker(delegate
                    {
                        textBoxNotifications.Text = m_Controller.Player.getNotifications();
                        groupBoxNotifications.Text = "Notifications(" + textBoxNotifications.Lines.Length + ")";
                    }));
                }
                else
                {
                    textBoxNotifications.Text = m_Controller.Player.getNotifications();
                    groupBoxNotifications.Text = "Notifications(" + textBoxNotifications.Lines.Length + ")";
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateNotificationGUI(): " + e.Message);
            }
        }

        private String getUnitName(String ID)
        {
            int l_UnitIndex = m_Controller.Player.Towns[0].getUnitIndex(ID);//Enter unit id of unit
            return m_Controller.Player.Towns[0].ArmyUnits[l_UnitIndex].LocalName;
       
            //You can find a list of unit ids in the method addArmyUnits() in Towns.cs
        }

        private String getBuildingName(String ID)
        {
            int l_Index = m_Controller.Player.Towns[0].getBuildingIndex(ID);//Enter unit id of unit
            return m_Controller.Player.Towns[0].Buildings[l_Index].LocalName;

            //You can find a list of unit ids in the method addArmyUnits() in Towns.cs
        }

        private void updateHeaderOverviewGrid()
        {
            try
            {
                dataGridOverview.Columns["dataGridOverview_Storage"].HeaderText = "Storage";
                dataGridOverview.Columns["dataGridOverview_Farming"].HeaderText = "Farm Villages";
                dataGridOverview.Columns["dataGridOverview_Wood"].HeaderText = "Wood";
                dataGridOverview.Columns["dataGridOverview_Rock"].HeaderText = "Stone";
                dataGridOverview.Columns["dataGridOverview_Silver"].HeaderText = "Silver";
             
                dataGridOverview.Columns["dataGridOverview_Harbor"].HeaderText = getBuildingName("docks") + " Orders";
                dataGridOverview.Columns["dataGridOverview_Barracks"].HeaderText = getBuildingName("barracks") + " Orders";

                dataGridOverview.Columns["dataGridOverview_Swords"].HeaderText = getUnitName("sword");
                dataGridOverview.Columns["dataGridOverview_Slingers"].HeaderText = getUnitName("slinger");
                dataGridOverview.Columns["dataGridOverview_Archer"].HeaderText = getUnitName("archer");
                dataGridOverview.Columns["dataGridOverview_Hoplite"].HeaderText = getUnitName("hoplite");
                dataGridOverview.Columns["dataGridOverview_Horsemen"].HeaderText = getUnitName("rider");
                dataGridOverview.Columns["dataGridOverview_Chariot"].HeaderText = getUnitName("chariot");
                dataGridOverview.Columns["dataGridOverview_Catapult"].HeaderText = getUnitName("catapult");
                dataGridOverview.Columns["dataGridOverview_Minotaur"].HeaderText = getUnitName("minotaur");
                dataGridOverview.Columns["dataGridOverview_Manticore"].HeaderText = getUnitName("manticore");
                dataGridOverview.Columns["dataGridOverview_Centaur"].HeaderText = getUnitName("centaur");
                dataGridOverview.Columns["dataGridOverview_Pegasus"].HeaderText = getUnitName("pegasus");
                dataGridOverview.Columns["dataGridOverview_Harpy"].HeaderText = getUnitName("harpy");
                dataGridOverview.Columns["dataGridOverview_Medusa"].HeaderText = getUnitName("medusa");
                dataGridOverview.Columns["dataGridOverview_Zyklop"].HeaderText = getUnitName("zyklop");
                dataGridOverview.Columns["dataGridOverview_Cerberus"].HeaderText = getUnitName("cerberus");
                dataGridOverview.Columns["dataGridOverview_Fury"].HeaderText = getUnitName("fury");
                dataGridOverview.Columns["dataGridOverview_Griffin"].HeaderText = getUnitName("griffin");
                dataGridOverview.Columns["dataGridOverview_CalydonianBoar"].HeaderText = getUnitName("calydonian_boar");
                dataGridOverview.Columns["dataGridOverview_Godsent"].HeaderText = getUnitName("godsent");

                dataGridOverview.Columns["dataGridOverview_Transport"].HeaderText = getUnitName("big_transporter");
                dataGridOverview.Columns["dataGridOverview_Bireme"].HeaderText = getUnitName("bireme");
                dataGridOverview.Columns["dataGridOverview_Lightship"].HeaderText = getUnitName("attack_ship");
                dataGridOverview.Columns["dataGridOverview_Fireship"].HeaderText = getUnitName("demolition_ship");
                dataGridOverview.Columns["dataGridOverview_FastTransport"].HeaderText = getUnitName("small_transporter");
                dataGridOverview.Columns["dataGridOverview_Trireme"].HeaderText = getUnitName("trireme");
                dataGridOverview.Columns["dataGridOverview_ColonyShip"].HeaderText = getUnitName("colonize_ship");
                dataGridOverview.Columns["dataGridOverview_Hydra"].HeaderText = getUnitName("sea_monster");
            }
            catch (Exception ex)
            {
                Debug.Print("Error Overview Grid " + ex.Message + "  " + ex.StackTrace);
            }
        }

        public void updateGridOverviewGUI()
        {
            string l_TradeMode;
            DataGridViewRow row;

            updateHeaderOverviewGrid();

            /* Notes from Jos 2-15-15 --- Doc ---
                       Regarding the question about the army count, you can't add CurrentAmount and TotalAmount together. 
             * The CurrentAmount is the amount of your units that are currently in your town. 
             * 
             * The TotalAmount is the amount of your units, no matter where they are located. I'll give a small example:
             * 
            Lets say you have trained 100 swordsman in a town and you send 10 away as support, this will result in CurrentAmount = 90, TotalAmount = 100.
            This information should help you to get the correct value displayed I assume? Good luck with the grid.
        
            */

            try
            {
                tabPageOverviewGrid.Invoke(new MethodInvoker(delegate
                  {

                      dataGridOverview.Rows.Clear();
                      dataGridOverview.Refresh();

                      dataGridOverview.BackgroundColor = ColorTranslator.FromHtml("#fff");
                      dataGridOverview.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                      dataGridOverview.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#9ac0e4");

                      dataGridOverview.RowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#ebf2fa");
                      dataGridOverview.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#fff");

                      dataGridOverview.EnableHeadersVisualStyles = false;

                      dataGridOverview.Columns["dataGridOverview_Town"].Frozen = true;


                      for (int i = 0; i < m_Controller.Player.Towns.Count; i++)
                      {


                          /* 
                           * 
                                          l_ActiveBuffs = "";

                                          if (m_Controller.Player.Towns[i].isPowerActive("call_of_the_ocean"))
                                              l_ActiveBuffs = "Overview\\power_call_of_the_ocean_16x16.png";

                                          if (m_Controller.Player.Towns[i].isPowerActive("fertility_improvement"))
                                              l_ActiveBuffs += "Overview\\power_fertility_improvement_16x16.png";

                                          if (m_Controller.Player.Towns[i].isPowerActive("happiness"))
                                              l_ActiveBuffs += "Overview\\power_happiness_16x16.png>";

                                          if (m_Controller.Player.Towns[i].isPowerActive("pest"))
                                              l_ActiveBuffs += "Overview\\power_pest_16x16.png";

                                          if (m_Controller.Player.Towns[i].isPowerActive("town_protection"))
                                              l_ActiveBuffs += "Overview\\power_town_protection_16x16.png";

                          */


                          if (m_Controller.Player.Towns[i].TradeEnabled)
                          {
                              if (m_Controller.Player.Towns[i].TradeMode.Equals("send"))
                                  l_TradeMode = "S";
                              else if (m_Controller.Player.Towns[i].TradeMode.Equals("receive"))
                                  l_TradeMode = "R";
                              else
                                  l_TradeMode = "Spy";
                          }
                          else
                          {
                              l_TradeMode = "-";
                          }


                          dataGridOverview.Rows.Add(
                              m_Controller.Player.Towns[i].TownID,
                              m_Controller.Player.Towns[i].Name, l_TradeMode,
                              m_Controller.Player.Towns[i].PopulationAvailable,
                              m_Controller.Player.Towns[i].Wood,
                              m_Controller.Player.Towns[i].Stone,
                              m_Controller.Player.Towns[i].Iron,
                              m_Controller.Player.Towns[i].Storage,
                              "",

                              m_Controller.Player.Towns[i].getIngameBuildingQueueSize(),
                              m_Controller.Player.Towns[i].SizeOfLandUnitQueue.ToString(),
                              m_Controller.Player.Towns[i].SizeOfNavyUnitQueue.ToString(),
                              m_Controller.Player.Towns[i].getNumberOfFriendlyFarmers().ToString(),
                              m_Controller.Player.Towns[i].AvailableQuests.ToString(),
                              m_Controller.Player.Towns[i].getNumberOfIncomingAttacks().ToString(),
                              m_Controller.Player.getFavorByTownIndexNewLine(i),
                              "",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("sword")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("sword")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("slinger")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("slinger")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("archer")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("archer")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("hoplite")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("hoplite")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("rider")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("rider")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("chariot")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("chariot")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("catapult")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("catapult")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("minotaur")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("minotaur")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("manticore")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("manticore")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("centaur")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("centaur")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("pegasus")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("pegasus")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("harpy")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("harpy")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("medusa")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("medusa")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("zyklop")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("zyklop")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("cerberus")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("cerberus")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("fury")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("fury")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("griffin")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("griffin")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("calydonian_boar")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("calydonian_boar")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("godsent")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("godsent")].TotalAmount.ToString()) + ")",
                              "",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("big_transporter")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("big_transporter")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("bireme")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("bireme")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("attack_ship")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("attack_ship")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("demolition_ship")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("demolition_ship")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("small_transporter")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("small_transporter")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("trireme")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("trireme")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("colonize_ship")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("colonize_ship")].TotalAmount.ToString()) + ")",
                              m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("sea_monster")].CurrentAmount.ToString() + Environment.NewLine + "(" + (m_Controller.Player.Towns[i].ArmyUnits[m_Controller.Player.Towns[i].getUnitIndex("sea_monster")].TotalAmount) + ")"


                              );  /// rows add
                          /// 
                          row = dataGridOverview.Rows[dataGridOverview.Rows.Count - 1];
                          row.Height = 35;

                          row.Cells["dataGridOverview_Blank1"].Style.BackColor = ColorTranslator.FromHtml("#9ac0e4");
                          row.Cells["dataGridOverview_Blank2"].Style.BackColor = ColorTranslator.FromHtml("#9ac0e4");
                          row.Cells["dataGridOverview_Blank3"].Style.BackColor = ColorTranslator.FromHtml("#9ac0e4");

                          if (m_Controller.Player.Towns[i].Storage == m_Controller.Player.Towns[i].Wood)
                          {
                              row.Cells["dataGridOverview_Wood"].Style.BackColor = Color.Red;
                              row.Cells["dataGridOverview_Wood"].Style.ForeColor = Color.White;
                          }

                          if (m_Controller.Player.Towns[i].Storage == m_Controller.Player.Towns[i].Stone)
                          {
                              row.Cells["dataGridOverview_Rock"].Style.BackColor = Color.Red;
                              row.Cells["dataGridOverview_Rock"].Style.ForeColor = Color.White;
                          }


                          if (m_Controller.Player.Towns[i].Storage == m_Controller.Player.Towns[i].Iron)
                          {
                              row.Cells["dataGridOverview_Silver"].Style.BackColor = Color.Red;
                              row.Cells["dataGridOverview_Silver"].Style.ForeColor = Color.White;
                          }

                          /// 
                      }  // for loop towns


                      //Total


                      dataGridOverview.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",

                      m_Controller.Player.getTotalUnits("sword") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("sword") + ")",
                      m_Controller.Player.getTotalUnits("slinger") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("slinger") + ")",
                      m_Controller.Player.getTotalUnits("archer") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("archer") + ")",
                      m_Controller.Player.getTotalUnits("hoplite") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("hoplite") + ")",
                      m_Controller.Player.getTotalUnits("rider") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("rider") + ")",
                      m_Controller.Player.getTotalUnits("chariot") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("chariot") + ")",
                      m_Controller.Player.getTotalUnits("catapult") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("catapult") + ")",
                      m_Controller.Player.getTotalUnits("minotaur") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("minotaur") + ")",
                      m_Controller.Player.getTotalUnits("manticore") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("manticore") + ")",
                      m_Controller.Player.getTotalUnits("centaur") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("centaur") + ")",
                      m_Controller.Player.getTotalUnits("pegasus") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("pegasus") + ")",
                      m_Controller.Player.getTotalUnits("harpy") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("harpy") + ")",
                      m_Controller.Player.getTotalUnits("medusa") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("medusa") + ")",
                      m_Controller.Player.getTotalUnits("zyklop") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("zyklop") + ")",
                      m_Controller.Player.getTotalUnits("cerberus") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("cerberus") + ")",
                      m_Controller.Player.getTotalUnits("fury") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("fury") + ")",
                      m_Controller.Player.getTotalUnits("griffin") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("griffin") + ")",
                      m_Controller.Player.getTotalUnits("calydonian_boar") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("calydonian_boar") + ")",
                      m_Controller.Player.getTotalUnits("godsent") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("godsent") + ")",
                      "",
                      m_Controller.Player.getTotalUnits("big_transporter") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("big_transporter") + ")",
                      m_Controller.Player.getTotalUnits("bireme") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("bireme") + ")",
                      m_Controller.Player.getTotalUnits("attack_ship") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("attack_ship") + ")",
                      m_Controller.Player.getTotalUnits("demolition_ship") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("demolition_ship") + ")",
                      m_Controller.Player.getTotalUnits("small_transporter") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("small_transporter") + ")",
                      m_Controller.Player.getTotalUnits("trireme") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("trireme") + ")",
                      m_Controller.Player.getTotalUnits("colonize_ship") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("colonize_ship") + ")",
                      m_Controller.Player.getTotalUnits("sea_monster") + Environment.NewLine + "(" + m_Controller.Player.getTotalUnitsAll("sea_monster") + ")"


                      );

                      row = dataGridOverview.Rows[dataGridOverview.Rows.Count - 1];
                      row.Height = 35;

                      row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#9ac0e4");
                      row.DefaultCellStyle.ForeColor = Color.Black;

                      Graphics g = this.CreateGraphics();

                      for (int x = 1; x < dataGridOverview.Columns.Count; x++)
                      {
                          dataGridOverview.Columns[x].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                          dataGridOverview.Columns[x].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                          dataGridOverview.Columns[x].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                          string headerText = dataGridOverview.Columns[x].HeaderText;

                          if (headerText.IndexOf(' ') > 0)
                          {

                              string[] headerWords = headerText.Split(' ');
                              string maxLenthWords = "";

                              foreach (String s in headerWords)
                                  if (s.Length > maxLenthWords.Length) maxLenthWords = s;

                              dataGridOverview.Columns[x].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                              dataGridOverview.Columns[x].Width =
                                    (int)g.MeasureString(maxLenthWords, dataGridOverview.Font).Width + 45;  // +45 leave room for the up and down sorting arrow

                          }
                      }
                      loadGridOverviewImages();
                  }));
            } //try
            catch (Exception ex)
            {
                Debug.Print("Error Overview Grid " + ex.Message + "  " + ex.StackTrace);

            }
        }

        #endregion

//--> Methods Crossthread

        #region Crossthread Handlers

        private void setStatusBarCrossThread(object p_Message)
        {
            try
            {
                if (statusStrip1.InvokeRequired)
                {
                    SetStatusBarCallback l_Delegate = new SetStatusBarCallback(setStatusBar);
                    this.Invoke(l_Delegate, new object[] { (string)p_Message });
                }
                else
                {
                    setStatusBar(p_Message);
                }
            }
            catch (Exception)
            {
                //No statusbar update this time :(
            }
        }

        /*
         * Set a message on the statusbar
         */
        private void setStatusBar(object p_Message)
        {
            toolStripStatusLabelGrep.Text = p_Message.ToString();
        }

        private void setGuiToLoggedInStateCrossThread()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (buttonLoginMethod1.InvokeRequired || buttonTownPauseResume.InvokeRequired)
                {
                    SetGuiToLoggedInStateCallBack l_Delegate = new SetGuiToLoggedInStateCallBack(setGuiToLoggedInState);
                    this.Invoke(l_Delegate);
                }
                else
                {
                    setGuiToLoggedInState();
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in setGuiToLoggedInStateCrossThread(): " + e.Message);
            }
        }

        private void setGuiToLoggedInState()
        {
            //IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            buttonLoginMethod1.Enabled = false;
            buttonTownPauseResume.Enabled = true;

            //Sort towns by name on first run only
            if (!m_Controller.TownsSorted)//This boolean is set to false again when a new town is found in updateGameDataResponse()
            {
                m_Controller.TownsSorted = true;
                m_Controller.Player.sortTownsByName();
                log("Towns sorted alphabetically.");

                //Empty all town lists
                comboBoxTownListQueue.Items.Clear();
                comboBoxTownListFarmers.Items.Clear();
                comboBoxTownListCulture.Items.Clear();
                comboBoxTownListTrade.Items.Clear();
                comboBoxTownListCombo.Items.Clear();
            }

            //Add towns for building/unit queue, farmers and trading
            if (comboBoxTownListQueue.Items.Count == 0)
            {
                for (int i = 0; i < m_Controller.Player.Towns.Count; i++)
                {
                    comboBoxTownListQueue.Items.Add(m_Controller.Player.Towns[i]);
                    comboBoxTownListFarmers.Items.Add(m_Controller.Player.Towns[i]);
                    comboBoxTownListCulture.Items.Add(m_Controller.Player.Towns[i]);
                    comboBoxTownListTrade.Items.Add(m_Controller.Player.Towns[i]);
                    comboBoxTownListCombo.Items.Add(m_Controller.Player.Towns[i]);
                }
            }

            setStatusBar("Connected to " + l_Settings.GenUserName + "@" + l_Settings.GenServer);

            m_Controller.Player.loadTownsSettings();
            //Note Update all GUI's here that have town specific data (1/3)
            updateOverviewGUI();
            updateGridOverviewGUI();
            updateQueueGUI();
            updateCulturalGUI();
            //updateFarmersGUI();//Only call this in the refresh timeout.
            updateNotificationGUI();
            updateTradeGUI();
            updateComboGUI();

            //Set player name
            tabControl1.TabPages[0].Text = textBoxUserName.Text + " ("+ l_Settings.GenServer.Substring(0,l_Settings.GenServer.IndexOf(".")) +")";
            m_NotifyIcon.BalloonTipText = "Grepolis bot 2 - " + tabControl1.TabPages[0].Text;
            m_NotifyIcon.Text = "Grepolis bot 2 - " + tabControl1.TabPages[0].Text;

            m_Controller.startbotFirstRun();//m_Controller.startbot();
            buttonTownPauseResume.Text = "Pause";

            m_Controller.startConnectedTimer();
            if(l_Settings.RecForcedReconnects)
                m_Controller.startForcedReconnectTimer();
        }

        private void setGuiToTimeoutProcessedStateCrossThread()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                setGuiToTimeoutProcessedState();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in setGuiToTimeoutProcessedStateCrossThread(): " + e.Message);
            }
        }

        /*
         * Bot has completed the refresh cycle,
         * all town data has been updated.
         */
        private void setGuiToTimeoutProcessedState()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;
            Parser l_Parser = Parser.Instance;

            string l_Response = "";

            try
            {
                if (!m_Controller.CanIModifyFarmers)
                {
                    m_Controller.Player.loadFarmersSettings();
                    m_Controller.CanIModifyFarmers = true;
                    flowLayoutPanelFarmers.Invoke(new MethodInvoker(delegate
                    {
                        tabPageFarmers.Text = "Farmers";
                        flowLayoutPanelFarmers.Visible = true;
                    }));
                }

                //Update reconnect counter
                labelReconnect.Invoke(new MethodInvoker(delegate
                {
                    labelReconnect.Text = "Reconnect(" + m_Controller.ReconnectCount + "/" + l_Settings.RecMaxReconnects + "):";
                }));

                if (!l_Response.Contains("login_to_game_world"))
                {
                    //Note Update all GUI's here that have town specific data (2/3)
                    updateOverviewGUI();
                    updateGridOverviewGUI();
                    updateQueueGUI();
                    updateCulturalGUI();
                    updateFarmersGUI();
                    updateNotificationGUI();
                    updateTradeGUI();
                    updateComboGUI();
                 
                    //Note Call all methods that are processed after every processed timeout (1/1)
                    m_Controller.sendUnderAttackWarning();

                    if (m_Controller.IsBotRunning)// || m_Controller.IsBotRunningOnce)
                    {
                        if (!m_Controller.RequestedToPauseBot)
                        {
                            //Start trade when ready AND bot not disabled by scheduler
                            if (m_Controller.TimeoutOnTradeTimer && !(l_Settings.AdvSchedulerBot && l_Settings.FarmerScheduler.Split(';')[DateTime.Now.Hour].Equals("False")))
                            {
                                if (l_Settings.MasterTrade)
                                    m_Controller.startTradeSequence();
                                else
                                {
                                    log("Trading disabled.");
                                    m_Controller.resumeTimers();
                                    toolStripStatusLabelGrep.Text = "Waiting";
                                }
                            }
                            else
                            {
                                m_Controller.resumeTimers();
                                toolStripStatusLabelGrep.Text = "Waiting";
                            }
                        }
                        else
                        {
                            m_Controller.RequestedToPauseBot = false;
                            toolStripStatusLabelGrep.Text = "Paused";
                        }
                    }
                }
                else//Reconnect needed
                {
                    m_Controller.startReconnect(2);
                }
            }
            catch (Exception e2)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in setGuiToTimeoutProcessedState(): " + e2.Message);
                }
            }
        }

        private void setToTradeProcessedStateCrossThread()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                setToTradeProcessedState();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in setToTradeProcessedStateCrossThread(): " + e.Message);
            }
        }

        private void setToTradeProcessedState()
        {
            if (m_Controller.IsBotRunning)// || m_Controller.IsBotRunningOnce)
            {
                //Note Update all GUI's here that have town specific data (3/3)
                updateOverviewGUI();
                updateGridOverviewGUI();
                updateQueueGUI();
                updateCulturalGUI();
                updateFarmersGUI();
                updateNotificationGUI();
                updateTradeGUI();
                updateComboGUI();
            

                if (!m_Controller.RequestedToPauseBot)
                {
                    m_Controller.resumeTimers();
                    toolStripStatusLabelGrep.Text = "Waiting";
                }
                else
                {
                    m_Controller.RequestedToPauseBot = false;
                    toolStripStatusLabelGrep.Text = "Paused";
                }
            }
        }

        private void setToTownProcessedStateCrossThread()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                setToTownProcessedState();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in setToTownProcessedStateCrossThread(): " + e.Message);
            }
        }

        private void setToTownProcessedState()
        {
            //Note Call all methods that are processed after every processed town (1/1)
            m_Controller.soundUnderAttackWarning();
            m_Controller.sendUnderAttackWarning();
        }

        private void updateRefreshTimerCrossThread(object p_TimeLeft)
        {
            try
            {
                if (labelNextUpdateTimeLeft.InvokeRequired || statusStrip1.InvokeRequired)
                {
                    UpdateRefreshTimerCallBack l_Delegate = new UpdateRefreshTimerCallBack(updateRefreshTimer);
                    this.Invoke(l_Delegate, new object[] { (string)p_TimeLeft });
                }
                else
                {
                    updateRefreshTimer(p_TimeLeft);
                }
            }
            catch (Exception)
            {
                //No timer update this time :(
            }
        }

        private void updateRefreshTimer(object p_TimeLeft)
        {
            if (p_TimeLeft.ToString().Contains("-"))
            {
                labelNextUpdateTimeLeft.Text = "00:00:00";
                toolStripStatusLabelGrep.Text = "Next update: 00:00:00";
            }
            else
            {
                labelNextUpdateTimeLeft.Text = p_TimeLeft.ToString();
                toolStripStatusLabelGrep.Text = "Next update: " + p_TimeLeft;
            }
        }

        private void updateQueueTimerCrossThread(object p_TimeLeft)
        {
            try
            {
                if (labelQueueTimeLeft.InvokeRequired)
                {
                    UpdateQueueTimerCallBack l_Delegate = new UpdateQueueTimerCallBack(updateQueueTimer);
                    this.Invoke(l_Delegate, new object[] { (string)p_TimeLeft });
                }
                else
                {
                    updateQueueTimer(p_TimeLeft);
                }
            }
            catch (Exception)
            {
                //No timer update this time :(
            }
        }

        private void updateQueueTimer(object p_TimeLeft)
        {
            if(p_TimeLeft.ToString().Contains("-"))
                labelQueueTimeLeft.Text = "00:00:00";
            else
                labelQueueTimeLeft.Text = p_TimeLeft.ToString();
        }

        private void updateTradeTimerCrossThread(object p_TimeLeft)
        {
            try
            {
                if (labelTradeTimeLeft.InvokeRequired)
                {
                    UpdateTradeTimerCallBack l_Delegate = new UpdateTradeTimerCallBack(updateTradeTimer);
                    this.Invoke(l_Delegate, new object[] { (string)p_TimeLeft });
                }
                else
                {
                    updateTradeTimer(p_TimeLeft);
                }
            }
            catch (Exception)
            {
                //No timer update this time :(
            }
        }

        private void updateTradeTimer(object p_TimeLeft)
        {
            Settings l_Settings = Settings.Instance;

            if (p_TimeLeft.ToString().Contains("-"))
                labelTradeTimeLeft.Text = "00:00:00";
            else
                labelTradeTimeLeft.Text = p_TimeLeft.ToString();
        }

        private void updateReconnectTimerCrossThread(object p_TimeLeft)
        {
            try
            {
                if (labelReconnectTimeLeft.InvokeRequired || statusStrip1.InvokeRequired)
                {
                    UpdateReconnectTimerCallBack l_Delegate = new UpdateReconnectTimerCallBack(updateReconnectTimer);
                    this.Invoke(l_Delegate, new object[] { (string)p_TimeLeft });
                }
                else
                {
                    updateReconnectTimer(p_TimeLeft);
                }
            }
            catch (Exception)
            {
                //No timer update this time :(
            }
        }

        private void updateReconnectTimer(object p_TimeLeft)
        {
            Settings l_Settings = Settings.Instance;

            if (p_TimeLeft.ToString().Contains("-"))
            {
                labelReconnect.Text = "Reconnect(" + m_Controller.ReconnectCount + "/" + l_Settings.RecMaxReconnects + "):";
                labelReconnectTimeLeft.Text = "00:00:00";
                toolStripStatusLabelGrep.Text = "Reconnect(" + m_Controller.ReconnectCount + "/" + l_Settings.RecMaxReconnects + "): 00:00:00";
            }
            else
            {
                labelReconnect.Text = "Reconnect(" + m_Controller.ReconnectCount + "/" + l_Settings.RecMaxReconnects + "):";
                labelReconnectTimeLeft.Text = p_TimeLeft.ToString();
                toolStripStatusLabelGrep.Text = "Reconnect(" + m_Controller.ReconnectCount + "/" + l_Settings.RecMaxReconnects + "): " + p_TimeLeft;
            }

            if (m_Controller.ReconnectTimer.isTimerDone())
            {
                m_Controller.ReconnectTimer.stop();
                loginP1();
            }
            else
            {
                m_Controller.ReconnectTimer.InternalTimer.Start();
            }
        }

        private void updateForcedReconnectTimerCrossThread(object p_TimeLeft)
        {
            try
            {
                if (labelForcedReconnectTimeLeft.InvokeRequired)
                {
                    UpdateForcedReconnectTimerCallBack l_Delegate = new UpdateForcedReconnectTimerCallBack(updateForcedReconnectTimer);
                    this.Invoke(l_Delegate, new object[] { (string)p_TimeLeft });
                }
                else
                {
                    updateForcedReconnectTimer(p_TimeLeft);
                }
            }
            catch (Exception)
            {
                //No timer update this time :(
            }
        }

        private void updateForcedReconnectTimer(object p_TimeLeft)
        {
            Settings l_Settings = Settings.Instance;

            if (p_TimeLeft.ToString().Contains("-"))
            {
                labelForcedReconnectTimeLeft.Text = "00:00:00";
                if(l_Settings.GUIReconnectTimerTitleBar)
                    this.Text = String.Format("{0} {1} - {2}", AssemblyTitle, AssemblyVersion, "00:00:00");
                else
                    this.Text = String.Format("{0} {1}", AssemblyTitle, AssemblyVersion);
            }
            else
            {
                //Creates some simple changes in the title during the last minute before the forced reconnect occurs.
                labelForcedReconnectTimeLeft.Text = p_TimeLeft.ToString();
                if (l_Settings.GUIReconnectTimerTitleBar)
                {
                    if (p_TimeLeft.ToString().Contains("00:00:") &&
                        (p_TimeLeft.ToString().EndsWith("1") ||
                        p_TimeLeft.ToString().EndsWith("3") ||
                        p_TimeLeft.ToString().EndsWith("5") ||
                        p_TimeLeft.ToString().EndsWith("7") ||
                        p_TimeLeft.ToString().EndsWith("9")))
                    {
                        this.Text = String.Format("{0} {1}", AssemblyTitle, AssemblyVersion);
                    }
                    else
                    {
                        this.Text = String.Format("{0} {1} - {2}", AssemblyTitle, AssemblyVersion, p_TimeLeft.ToString());                        
                    }
                }
                else
                    this.Text = String.Format("{0} {1}", AssemblyTitle, AssemblyVersion);
            }
        }

        private void updateConnectedTimerCrossThread(object p_TimeElapsed)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (labelConnectedTimer.InvokeRequired)
                {
                    UpdateConnectedTimerCallBack l_Delegate = new UpdateConnectedTimerCallBack(updateConnectedTimer);
                    this.Invoke(l_Delegate, new object[] { (string)p_TimeElapsed });
                }
                else
                {
                    updateConnectedTimer(p_TimeElapsed);
                }
            }
            catch (Exception)
            {
                //No timer update this time :(
                //if (l_Settings.AdvDebugMode)
                //   l_IOHandler.debug("Exception in updateConnectedTimerCrossThread(): " + ex.Message);
            }
        }

        private void updateConnectedTimer(object p_TimeElapsed)
        {
            labelConnectedTimer.Text = p_TimeElapsed.ToString();
        }

        private void logCrossThread(object p_Message)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (textBoxLog.InvokeRequired)
                {
                    LogCallBack l_Delegate = new LogCallBack(log);
                    this.Invoke(l_Delegate, new object[] { (string)p_Message });
                }
                else
                {
                    log(p_Message);
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in logCrossThread(): " + ex.Message);
            }
        }

        private void log(object p_Message)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                int l_Length = 0;
                string l_Message = DateTime.Now.ToLocalTime() + " " + p_Message.ToString();

                l_IOHandler.log(l_Message);

                textBoxLog.Text = l_Message + Environment.NewLine + textBoxLog.Text;
                groupBoxLog.Text = "Log(" + textBoxLog.Lines.Length + ")";

                if (textBoxLog.Lines.Length > l_Settings.GUILogSize)
                {
                    for (int i = l_Settings.GUILogSize; i < textBoxLog.Lines.Length; i++)
                    {
                        l_Length += textBoxLog.Lines[i].Length;
                    }
                    textBoxLog.Text = textBoxLog.Text.Remove(textBoxLog.Text.Length - l_Length);
                    textBoxLog.Lines[textBoxLog.Lines.Length - 1].Remove(0);
                }
            }
            catch (Exception)
            {
                //No action required
            }
        }

        private void versionInfoCrossThread(object p_VersionInfo)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (linkLabelLatestVersion.InvokeRequired)
                {
                    VersionInfoCallBack l_Delegate = new VersionInfoCallBack(versionInfo);
                    this.Invoke(l_Delegate, new object[] { (string)p_VersionInfo });
                }
                else
                {
                    versionInfo(p_VersionInfo);
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in versionInfoCrossThread(): " + ex.Message);
            }
        }

        private void versionInfo(object p_VersionInfo)
        {
            linkLabelLatestVersion.Text = "Latest version: " + p_VersionInfo.ToString();
        }

        private void serverRequestDelayRequestCrossThread()
        {
            serverRequestDelayRequestCallBack l_Delegate = new serverRequestDelayRequestCallBack(serverRequestDelayRequest);
            this.Invoke(l_Delegate);
        }

        private void serverRequestDelayRequest()
        {
            m_Controller.RequestTimer.Start();
        }

        private void serverRequestCaptchaDelayRequestCrossThread()
        {
            serverRequestDelayRequestCallBack l_Delegate = new serverRequestDelayRequestCallBack(serverRequestCaptchaDelayRequest);
            this.Invoke(l_Delegate);
        }

        private void serverRequestCaptchaDelayRequest()
        {
            m_Controller.RequestTimerCaptcha.Start();
        }

        private void pauseBotRequestCaptchaCrossThread()
        {
            pauseBotRequestCaptchaCallBack l_Delegate = new pauseBotRequestCaptchaCallBack(pauseBotRequestCaptcha);
            this.Invoke(l_Delegate);
        }

        private void pauseBotRequestCaptcha()
        {
            m_Controller.pause();
            buttonTownPauseResume.Text = "Resume";
            setStatusBar("Captcha detected");
        }

        private void startCaptchaCheckTimerRequestCrossThread()
        {
            startCaptchaCheckTimerRequestCallBack l_Delegate = new startCaptchaCheckTimerRequestCallBack(startCaptchaCheckTimerRequest);
            this.Invoke(l_Delegate);
        }

        private void startCaptchaCheckTimerRequest()
        {
            buttonTownPauseResume.Enabled = false;
            m_TimerCaptcha.Start();
        }

        private void captchaCheckPreCycleCrossThread()
        {
            captchaCheckPreCycleCallBack l_Delegate = new captchaCheckPreCycleCallBack(captchaCheckPreCycle);
            this.Invoke(l_Delegate);
        }

        /*
         * Checks if the captcha window is open in the browser tab.
         * Method is called before the bot starts the update cycle.
         */ 
        private void captchaCheckPreCycle()
        {
            Settings l_Settings = Settings.Instance; 
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                string l_Response = webBrowserGrepo.Document.Body.InnerHtml;

                if (l_Response.Contains("id=\"captcha_curtain\"") || l_Response.Contains("id=captcha_curtain"))
                {
                    string l_Message = DateTime.Now.ToLocalTime() + " captchaDetectedSequence() started through captchaCheckPreCycle()";
                    l_IOHandler.log(l_Message);
                    m_Controller.CaptchaDetectedPrecycle = true;
                    m_Controller.captchaDetectedSequence(!l_Response.Contains("message_new_preview"));
                }
                else
                {
                    m_Controller.switchTown();
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in captchaCheckPreCycle(): " + e.Message);
                }
            }
        }

        private void captchaAnswerReadyCrossThread()
        {
            captchaAnswerReadyCallBack l_Delegate = new captchaAnswerReadyCallBack(captchaAnswerReady);
            this.Invoke(l_Delegate);
        }

        private void captchaAnswerReady()
        {
            m_Controller.captchaSendToGrepolis();
        }

        private void captchaAnswerModeratedCrossThread()
        {
            captchaAnswerModeratedCallBack l_Delegate = new captchaAnswerModeratedCallBack(captchaAnswerModerated);
            this.Invoke(l_Delegate);
        }

        private void captchaAnswerModerated()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                string l_ResponseCaptcha = webBrowserGrepo.Document.Body.InnerHtml;
                if (l_ResponseCaptcha.Contains("id=\"captcha_curtain\"") || l_ResponseCaptcha.Contains("id=captcha_curtain"))
                {
                    if (m_Controller.CaptchaAnswerInCorrectCount < 10)
                    {
                        m_TimerCaptcha.Start();
                    }
                    else
                    {
                        //Enable resume button after a sequence of incorrect answers, wait for user to enter captcha
                        buttonTownPauseResume.Enabled = true;
                        m_Controller.RequestedToPauseBot = false;
                    }
                }
                else
                {
                    l_IOHandler.debug("Captcha answer moderated, starting bot.");
                    m_Controller.startbot();
                    buttonTownPauseResume.Text = "Pause";
                    buttonTownPauseResume.Enabled = true;
                }
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in captchaAnswerModerated(): " + e.Message);
                }
            }
        }

        private void captchaAnswerSendToGrepolisCorrectCrossThread()
        {
            captchaAnswerSendToGrepolisCorrectCallBack l_Delegate = new captchaAnswerSendToGrepolisCorrectCallBack(captchaAnswerSendToGrepolisCorrect);
            this.Invoke(l_Delegate);
        }

        private void captchaAnswerSendToGrepolisCorrect()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                webBrowserGrepo.Refresh();
                m_TimerCaptchaAnswered.Start();//On timer tick message is send to server containing whether or not the answer was correct
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in captchaAnswerSendToGrepolisCorrect(): " + e.Message);
                }
            }
        }

        private void captchaAnswerSendToGrepolisInCorrectCrossThread()
        {
            captchaAnswerSendToGrepolisInCorrectCallBack l_Delegate = new captchaAnswerSendToGrepolisInCorrectCallBack(captchaAnswerSendToGrepolisInCorrect);
            this.Invoke(l_Delegate);
        }

        private void captchaAnswerSendToGrepolisInCorrect()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                string l_ResponseCaptcha = webBrowserGrepo.Document.Body.InnerHtml;
                if (l_ResponseCaptcha.Contains("id=\"captcha_curtain\"") || l_ResponseCaptcha.Contains("id=captcha_curtain"))
                {
                    //Click confirm button
                    HtmlElementCollection l_Elements = webBrowserGrepo.Document.GetElementsByTagName("div");

                    for (int i = 0; i < l_Elements.Count; i++)
                    {
                        if (l_Elements[i].OuterHtml.Equals("<div class=\"btn_reload button_new square reload\"></div>"))
                        {
                            l_Elements[i].InvokeMember("click");
                            break;
                        }
                    }
                }
                m_TimerCaptchaAnswered.Start();//On timer tick message is send to server containing whether or not the answer was correct
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in captchaAnswerSendToGrepolisInCorrect(): " + e.Message);
                }
            }
        }

        private void captchaSolver9kwDownCrossThread()
        {
            captchaSolver9kwDownCallBack l_Delegate = new captchaSolver9kwDownCallBack(captchaSolver9kwDown);
            this.Invoke(l_Delegate);
        }

        private void captchaSolver9kwDown()
        {
            buttonTownPauseResume.Enabled = true;
        }

        #endregion

//-->Event Handlers

        #region GUI Events - Settings tab

        //Settings tab
        private void buttonSettingsOK_Click(object sender, EventArgs e)
        {
            saveSettings();
            updateQueueGUI();
            m_Controller.soundUnderAttackWarning();
        }

        private void buttonSettingsCancel_Click(object sender, EventArgs e)
        {
            loadSettings();
        }

        private void labelGUIBuildingLevelTargetColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                labelGUIBuildingLevelTargetColor.BackColor = colorDialog1.Color;
            }
        }

        private void numericUpDownMinDelayFarmers_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMaxDelayFarmers.Value < numericUpDownMinDelayFarmers.Value)
                numericUpDownMaxDelayFarmers.Value = numericUpDownMinDelayFarmers.Value;
        }

        private void numericUpDownMaxDelayFarmers_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMaxDelayFarmers.Value < numericUpDownMinDelayFarmers.Value)
                numericUpDownMaxDelayFarmers.Value = numericUpDownMinDelayFarmers.Value;
        }

        private void numericUpDownTimerReconnectMin_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownTimerReconnectMax.Value < numericUpDownTimerReconnectMin.Value)
                numericUpDownTimerReconnectMax.Value = numericUpDownTimerReconnectMin.Value;
        }

        private void numericUpDownTimerReconnectMax_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownTimerReconnectMax.Value < numericUpDownTimerReconnectMin.Value)
                numericUpDownTimerReconnectMax.Value = numericUpDownTimerReconnectMin.Value;
        }

        private void numericUpDownMinForcedReconnect_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMaxForcedReconnect.Value < numericUpDownMinForcedReconnect.Value)
                numericUpDownMaxForcedReconnect.Value = numericUpDownMinForcedReconnect.Value;
        }

        private void numericUpDownMaxForcedReconnect_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMaxForcedReconnect.Value < numericUpDownMinForcedReconnect.Value)
                numericUpDownMaxForcedReconnect.Value = numericUpDownMinForcedReconnect.Value;
        }

        private void numericUpDownMinTimerRefresh_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMaxTimerRefresh.Value < numericUpDownMinTimerRefresh.Value)
                numericUpDownMaxTimerRefresh.Value = numericUpDownMinTimerRefresh.Value;
        }

        private void numericUpDownMaxTimerRefresh_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMaxTimerRefresh.Value < numericUpDownMinTimerRefresh.Value)
                numericUpDownMaxTimerRefresh.Value = numericUpDownMinTimerRefresh.Value;
        }

        private void numericUpDownMinDelayRequests_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMaxDelayRequests.Value < numericUpDownMinDelayRequests.Value)
                numericUpDownMaxDelayRequests.Value = numericUpDownMinDelayRequests.Value;
        }

        private void numericUpDownMaxDelayRequests_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMaxDelayRequests.Value < numericUpDownMinDelayRequests.Value)
                numericUpDownMaxDelayRequests.Value = numericUpDownMinDelayRequests.Value;
        }

        private void buttonEmailTest_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;

            string l_Subject = "[Grepolis2][" + l_Settings.GenServer + "] Test";
            string l_EmailMessage = "Your email configuration is correct.";
            m_Controller.MailClient.sendMail(l_Settings.MailServer, l_Settings.MailPort, true, l_Settings.MailUsername, l_Settings.MailPassword, l_Settings.MailAddress, l_Subject, l_EmailMessage, true);
        }

        private void button9kwTest_Click(object sender, EventArgs e)
        {
            textBox9kwBalance.Text = m_Controller.captchaBalanceQuerySingle();
        }

        private void textBoxMainServer_TextChanged(object sender, EventArgs e)
        {
            if (textBoxMainServer.Text.Contains("http") ||
                textBoxMainServer.Text.Contains(":") ||
                textBoxMainServer.Text.Contains("/"))
            {
                textBoxMainServer.BackColor = Color.Red;
            }
            else
            {
                textBoxMainServer.BackColor = SystemColors.Window;
            }
        }

        private void textBoxServer_TextChanged(object sender, EventArgs e)
        {
            if (textBoxServer.Text.Contains("http") ||
                textBoxServer.Text.Contains(":") ||
                textBoxServer.Text.Contains("/"))
            {
                textBoxServer.BackColor = Color.Red;
            }
            else
            {
                textBoxServer.BackColor = SystemColors.Window;
            }
        }

        private void textBoxCaptchaApikey_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBoxCaptchaApikey.Text, "[^A-Za-z0-9]") || textBoxCaptchaApikey.Text.Length < 5 || textBoxCaptchaApikey.Text.Length > 50)
            {
                textBoxCaptchaApikey.BackColor = Color.Red;
            }
            else
            {
                textBoxCaptchaApikey.BackColor = SystemColors.Window;
            }
        }

        private void setHelpSettings(string p_Tittle, string p_Description)
        {
            groupBoxHelpSettings.Text = "Description - " + p_Tittle;
            textBoxHelpSettings.Text = "\r\n" + p_Description;
        }

        private void labelUserName_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("User Name", "Your Grepolis account name.");
        }

        private void labelPassword_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Password", "Your Grepolis account password.");
        }

        private void labelMainServer_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Main Server", "The Grepolis server of your prefered language.");
        }

        private void labelServer_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Server", "The Grepolis server you play on. You can find your server number in the address bar of your browser.");
        }

        private void labelTrayIcon_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Minimize To Tray", "Whether or not the bot should minimize to the taskbar.");
        }

        private void labelTimer_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Timer (Minutes)", "The time between each check of the building/unit queue.");
        }

        private void labelBuildFarmBelow_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Build Farm below", "Adds a Farm to your queue when your population is below this value.");
        }

        private void labelMinUnitQueuePop_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Minimum Population", "The minimum amount of units (in population) before adding them to the queue.");
        }

        private void labelQueueLimit_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Queue Limit", "The amount of places you want to use in unit queue.");
        }

        private void labelEmail_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Email", "The Email address that will receive all the notifications.");
        }

        private void labelNotify_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Notify By Mail", "Whether or not you want to receive notifications.\r\nEnable the second checked box to include incoming support.");
        }

        private void labelMailUsername_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("User Name", "The Email address you want to use to send the notifications. A Gmail address recommended.");
        }

        private void labelMailPassword_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Password", "The password for the above Email address.");
        }

        private void labelMailServer_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Server", "The email server you want to use to send your mails.");
        }

        private void labelMailPort_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Port", "The port that the above server uses.");
        }

        private void labelGUIBuildingLevelTarget_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Building lvl. target", "The color of the building level when the target level is reached.");
        }

        private void labelSameTownOnAllTabs_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Same town on all tabs", "Whether or not you want to see the same town on all the tabs.");
        }

        private void labelBuildingTooltips_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Building Tooltips", "Whether or not you want to see the building tooltips.");
        }

        private void labelNotificationSize_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Notification Size", "The maximum number of notifications in the Notifications tab.");
        }

        private void labelLogSize_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Log Size", "The maximum number of events in the Log tab");
        }

        private void labelMinRefreshTimer_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Min. Refresh Timer (min)", "The delay (in minutes) between the bots activity.");
        }

        private void labelMaxRefreshTimer_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Max. Refresh Timer (min)", "The delay (in minutes) between the bots activity.");
        }

        private void labelReconnectTimerMin_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Min. Reconnect Timer (min)", "The minimum time in minutes before the bot reconnects with the server.");
        }

        private void labelReconnectTimerMax_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Max. Reconnect Timer (min)", "The maximum time in minutes before the bot reconnects with the server.");
        }

        private void labelMinDelayRequests_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Delay Requests (ms)", "The minimum time, in miliseconds, between every server request.");
        }

        private void labelMaxDelayRequests_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Delay Requests (ms)", "The maximum time, in miliseconds, between every server request.");
        }

        private void labelMinDelayFarmers_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Min. Delay Farmers (ms)", "The minimum time, in miliseconds, between looting farmers.");
        }

        private void labelMaxDelayFarmers_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Max. Delay Farmers (ms)", "The maximum time, in miliseconds, between looting farmers.");
        }

        private void labelFarmersLootLag_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Farmer Loot Lag (sec)", "Adds some extra time (in seconds) before the farmers are ready.");
        }

        private void labelMaxReconnectsCheck_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Max. Reconnects", "Whether or not you want to stop the bot when the maximum amount of reconnects is reached.");
        }

        private void labelMaxReconnects_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Max. Reconnects", "How many reconnects the bot is allowed to make.");
        }

        private void labelDebugMode_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Debug Mode", "Whether or not you want to save error messages in the file debug.txt.");
        }

        private void labelAdvancedQueue_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Advanced Queue", "Whether or not you want to use the advanced building queue mode. This advanced mode is only for when you use the Target mode of the building queue. The advanced building queue allows you to use the Non-Target mode at the same time.");
        }

        private void labelForcedReconnects_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Forced Reconnects", "Whether or not you want to reconnect with the server after a specific time.");
        }

        private void labelMinForcedReconnect_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Min. Forced Reconnect", "The minimum time (in hours) for a forced reconnect.");
        }

        private void labelMaxForcedReconnect_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Max. Forced Reconnect", "The maximum time (in hours) for a forced reconnect.");
        }

        private void labelSchedulerBot_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Scheduler Bot", "Use the scheduler of the farmers also for turning off the bot. Note: The bot will remain open but it will not request new data from the server.");
        }

        private void labelTimeout_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Timeout", "The timeout in seconds of a server request.");
        }

        private void labelUseGold_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Use gold", "Enable this setting to allow the bot to use gold. You need to enable this when you want to use the paid cultural festival.");
        }

        private void labelUseFarmAllFeature_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Use farm all feature", "Enable this setting to use the farm all feature. Remember that you need to have Captain active for this to work. \r\nWARNING: The \"Max. Resources\" setting doesn't work when you use this feature.");
        }

        private void labelTimerTrade_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Timer (Minutes)", "The time between trading.");
        }

        private void labelIncomingAttackWarning_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Incoming Attack", "When this is enabled you'll hear a alarm when you're under attack. You can choose your own alarm my replacing the attack.wav file.");
        }

        private void labelCaptchaWarning_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Captcha", "When this is enabled you'll hear a alarm when a captcha is detected. You can choose your own alarm my replacing the captcha.wav file.");
        }

        private void labelAutoStart_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Auto Start", "Starts/login the bot automatic when you open it.");
        }

        private void labelAutoPause_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Auto Pause", "Pauses the bot when the you switch to the browser tab.");
        }

        private void labelOutputAllMode_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Output All Mode", "Saves every server response in the Response folder.");
        }

        private void labelCaptchaCaptchaSolver_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Captcha Solver", "Enable or disable captcha solver.");
        }

        private void labelCaptchaSelfSolve_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Self Solve", "EXPERIMENTAL Solve your own captchas. Works only if you enable the \"Self Solve\" option at 9kw.eu. You also need to download the 9kw mobile app, see the guide for more information.");
        }

        private void labelCaptchaPriority_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Priority", "Gives your captcha request extra priority on 9kw.eu. WARNING: This cost extra credits (+1-10). Set to 0 to turn off.");
        }

        private void labelCaptchaMinWorkers_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Min. Workers", "LEFT: The minimum amount of workers that should be available when solving a captcha.\r\nRIGHT: When there are more workers that this value the queue size will be ignored.");
        }

        private void labelCaptchaMaxQueue_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Max. Queue", "The maximum amount of captchas that are allowed in the queue when solving a captcha.");
        }

        private void labelCaptchaExtraDelay_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Extra Delay", "The extra delay added when the conditions of \"Min. Workers\" or \"Max. Queue\" are met.");
        }   

        private void linkLabelApikey_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Apikey", "Enter your 9kw.eu apikey here.");
        }

        private void labelRandomize_MouseHover(object sender, EventArgs e)
        {
            setHelpSettings("Randomize", "Randomize the loot sequence of the farmers.");
        }

        private void checkBoxCaptchaWarning_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxCaptchaWarning.Checked)
            {
                m_Controller.CaptchaDetected = false;
                m_Controller.soundCaptchaWarning();
            }
        }
        //~Settings tab

        #endregion

        #region GUI Events - Player tab

        //Player tab
        private void buttonLoginMethod1_Click(object sender, EventArgs e)
        {
            //Check useragent        
            Settings l_Settings = Settings.Instance;
            if (l_Settings.AdvUserAgent.Contains("MSIE 7.0"))
            {
                MessageBox.Show("It's recommended to upgrade your browser before starting the bot.\n\r" +
                    "Run Enable_IE9.reg for IE 9.0 and older.\n\r" +
                    "Run Enable_IE10.reg for IE 10.0 and newer.\n\r");
            }
            else
            {
                buttonLoginMethod1.Enabled = false;
                loginP1();
            }
        }

        private void textBoxNotes_MouseLeave(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            l_IOHandler.saveNotes(textBoxNotes.Text);
        }

        private void labelReload_DoubleClick(object sender, EventArgs e)
        {
            m_Controller.checkVersion();
        }

        private void linkLabelLatestVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Determine which link was clicked within the LinkLabel.
            linkLabelLatestVersion.Links[linkLabelLatestVersion.Links.IndexOf(e.Link)].Visited = true;
            // Display the appropriate link based on the value of the 
            // LinkData property of the Link object.
            String l_Target = e.Link.LinkData as String;
            System.Diagnostics.Process.Start(l_Target);
        }

        private void linkLabelApikey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Determine which link was clicked within the LinkLabel.
            linkLabelApikey.Links[linkLabelApikey.Links.IndexOf(e.Link)].Visited = true;
            // Display the appropriate link based on the value of the 
            // LinkData property of the Link object.
            String l_Target = e.Link.LinkData as String;
            System.Diagnostics.Process.Start(l_Target);
        }

        private void buttonTownPauseResume_Click(object sender, EventArgs e)
        {
            if (buttonTownPauseResume.Text.Equals("Resume"))
            {
                if ((m_Controller.LoggedIn && !m_Controller.RequestedToPauseBot) || m_Controller.CaptchaMassMailDetected)
                {
                    m_Controller.CaptchaMassMailDetected = false;
                    m_Controller.startbot();
                    buttonTownPauseResume.Text = "Pause";
                }
            }
            else
            {
                m_Controller.pause();
                buttonTownPauseResume.Text = "Resume";
            }
        }

        private void checkBoxMBuildingQueue_CheckedChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            l_Settings.MasterBuildingQueue = checkBoxMBuildingQueue.Checked;
            saveSettings();
        }

        private void checkBoxMUnitQueue_CheckedChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            l_Settings.MasterUnitQueue = checkBoxMUnitQueue.Checked;
            saveSettings();
        }

        private void checkBoxMCulture_CheckedChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            l_Settings.MasterCulture = checkBoxMCulture.Checked;
            saveSettings();
        }

        private void checkBoxMFarmers_CheckedChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            l_Settings.MasterFarmers = checkBoxMFarmers.Checked;
            saveSettings();
        }

        private void checkBoxMTrade_CheckedChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            l_Settings.MasterTrade = checkBoxMTrade.Checked;
            saveSettings();
        }

        //~Player tab

        #endregion

        #region GUI Events - Queue tab

        //Queue tab
        private void buttonSaveQueue_Click(object sender, EventArgs e)
        {
            //Note Modify this when adding new units (03/15)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (comboBoxTownListQueue.Items.Count > 0 && !m_SelectedTownQueue.Equals("0"))
                {
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].BuildingQueueEnabled = checkBoxBuildingQueueEnabled.Checked;
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].BuildingLevelsTargetEnabled = checkBoxBuildingQueueTarget.Checked;
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].BuildingDowngradeEnabled = checkBoxBuildingDowngrade.Checked;
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setBuildingQueue(grepBuildings2.getBuildingQueue());
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setBuildingsLevelTarget(grepBuildings2.getBuildingLevelsTarget());
                    
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("sword", grepUnits2.getUnitQueue("sword"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("slinger", grepUnits2.getUnitQueue("slinger"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("archer", grepUnits2.getUnitQueue("archer"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("hoplite", grepUnits2.getUnitQueue("hoplite"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("rider", grepUnits2.getUnitQueue("rider"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("chariot", grepUnits2.getUnitQueue("chariot"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("catapult", grepUnits2.getUnitQueue("catapult"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("minotaur", grepUnits2.getUnitQueue("minotaur"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("manticore", grepUnits2.getUnitQueue("manticore"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("centaur", grepUnits2.getUnitQueue("centaur"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("pegasus", grepUnits2.getUnitQueue("pegasus"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("harpy", grepUnits2.getUnitQueue("harpy"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("medusa", grepUnits2.getUnitQueue("medusa"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("zyklop", grepUnits2.getUnitQueue("zyklop"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("cerberus", grepUnits2.getUnitQueue("cerberus"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("fury", grepUnits2.getUnitQueue("fury"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("griffin", grepUnits2.getUnitQueue("griffin"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("calydonian_boar", grepUnits2.getUnitQueue("calydonian_boar"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("godsent", grepUnits2.getUnitQueue("godsent"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("big_transporter", grepUnits2.getUnitQueue("big_transporter"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("bireme", grepUnits2.getUnitQueue("bireme"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("attack_ship", grepUnits2.getUnitQueue("attack_ship"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("demolition_ship", grepUnits2.getUnitQueue("demolition_ship"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("small_transporter", grepUnits2.getUnitQueue("small_transporter"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("trireme", grepUnits2.getUnitQueue("trireme"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("colonize_ship", grepUnits2.getUnitQueue("colonize_ship"));
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].setUnitTargetAmount("sea_monster", grepUnits2.getUnitQueue("sea_monster"));

                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].UnitQueueEnabled = checkBoxUnitQueueEnabled.Checked;

                    //militia
                    /*for (int i = 0; i < m_Controller.Player.Towns.Count; i++)//All towns get the same militia setting
                    {
                        m_Controller.Player.Towns[i].MilitiaTrigger = (int)numericUpDownMilitiaTrigger.Value;
                    }*/
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownQueue)].MilitiaTrigger = (int)numericUpDownMilitiaTrigger.Value;

                    //Set population clculations
                    setQueuePopulationCalculations();

                    l_IOHandler.saveTownsSettings(m_Controller.Player);
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonSaveQueue_Click(): " + ex.Message);
            }
        }

        private void buttonUpdArmyQueueTemplate_Click(object sender, EventArgs e)
        {
            //Note Modify this when adding new units (04/15)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (comboBoxTemplatesUnitQueue.Items.Count > 0)
                {
                    if (comboBoxTemplatesUnitQueue.SelectedItem != null)
                    {
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("sword", grepUnits2.getUnitQueue("sword"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("slinger", grepUnits2.getUnitQueue("slinger"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("archer", grepUnits2.getUnitQueue("archer"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("hoplite", grepUnits2.getUnitQueue("hoplite"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("rider", grepUnits2.getUnitQueue("rider"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("chariot", grepUnits2.getUnitQueue("chariot"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("catapult", grepUnits2.getUnitQueue("catapult"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("minotaur", grepUnits2.getUnitQueue("minotaur"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("manticore", grepUnits2.getUnitQueue("manticore"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("centaur", grepUnits2.getUnitQueue("centaur"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("pegasus", grepUnits2.getUnitQueue("pegasus"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("harpy", grepUnits2.getUnitQueue("harpy"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("medusa", grepUnits2.getUnitQueue("medusa"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("zyklop", grepUnits2.getUnitQueue("zyklop"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("cerberus", grepUnits2.getUnitQueue("cerberus"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("fury", grepUnits2.getUnitQueue("fury"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("griffin", grepUnits2.getUnitQueue("griffin"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("calydonian_boar", grepUnits2.getUnitQueue("calydonian_boar"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("godsent", grepUnits2.getUnitQueue("godsent"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("big_transporter", grepUnits2.getUnitQueue("big_transporter"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("bireme", grepUnits2.getUnitQueue("bireme"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("attack_ship", grepUnits2.getUnitQueue("attack_ship"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("demolition_ship", grepUnits2.getUnitQueue("demolition_ship"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("small_transporter", grepUnits2.getUnitQueue("small_transporter"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("trireme", grepUnits2.getUnitQueue("trireme"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("colonize_ship", grepUnits2.getUnitQueue("colonize_ship"));
                        m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].setUnitTargetAmount("sea_monster", grepUnits2.getUnitQueue("sea_monster"));

                        l_IOHandler.saveTemplatesUnitQueue(m_Controller.Player.TemplatesUnitQueue);
                    }
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonUpdArmyQueueTemplate_Click(): " + ex.Message);
            }
        }

        private void buttonLoadArmyQueueTemplate_Click(object sender, EventArgs e)
        {
            //Note Modify this when adding new units (05/15)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (comboBoxTemplatesUnitQueue.Items.Count > 0)
                {
                    if (comboBoxTemplatesUnitQueue.SelectedItem != null)
                    {
                        grepUnits2.setUnitQueue("sword", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("sword"));
                        grepUnits2.setUnitQueue("slinger", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("slinger"));
                        grepUnits2.setUnitQueue("archer", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("archer"));
                        grepUnits2.setUnitQueue("hoplite", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("hoplite"));
                        grepUnits2.setUnitQueue("rider", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("rider"));
                        grepUnits2.setUnitQueue("chariot", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("chariot"));
                        grepUnits2.setUnitQueue("catapult", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("catapult"));
                        grepUnits2.setUnitQueue("minotaur", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("minotaur"));
                        grepUnits2.setUnitQueue("manticore", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("manticore"));
                        grepUnits2.setUnitQueue("centaur", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("centaur"));
                        grepUnits2.setUnitQueue("pegasus", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("pegasus"));
                        grepUnits2.setUnitQueue("harpy", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("harpy"));
                        grepUnits2.setUnitQueue("medusa", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("medusa"));
                        grepUnits2.setUnitQueue("zyklop", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("zyklop"));
                        grepUnits2.setUnitQueue("cerberus", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("cerberus"));
                        grepUnits2.setUnitQueue("fury", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("fury"));
                        grepUnits2.setUnitQueue("griffin", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("griffin"));
                        grepUnits2.setUnitQueue("calydonian_boar", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("calydonian_boar"));
                        grepUnits2.setUnitQueue("godsent", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("godsent"));
                        grepUnits2.setUnitQueue("big_transporter", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("big_transporter"));
                        grepUnits2.setUnitQueue("bireme", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("bireme"));
                        grepUnits2.setUnitQueue("attack_ship", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("attack_ship"));
                        grepUnits2.setUnitQueue("demolition_ship", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("demolition_ship"));
                        grepUnits2.setUnitQueue("small_transporter", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("small_transporter"));
                        grepUnits2.setUnitQueue("trireme", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("trireme"));
                        grepUnits2.setUnitQueue("colonize_ship", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("colonize_ship"));
                        grepUnits2.setUnitQueue("sea_monster", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueue.SelectedIndex].getUnitTargetAmount("sea_monster"));
                    }
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonLoadArmyQueueTemplate_Click(): " + ex.Message);
            }
        }

        private void buttonAddArmyQueueTemplate_Click(object sender, EventArgs e)
        {
            //Note Modify this when adding new units (06/15)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (!textBoxTemplateUnitQueueNew.Text.Equals(""))
                {
                    bool l_IsUnique = true;
                    for (int i = 0; i < m_Controller.Player.TemplatesUnitQueue.Count; i++)
                    {
                        if (m_Controller.Player.TemplatesUnitQueue[i].Name.Equals(textBoxTemplateUnitQueueNew.Text))
                            l_IsUnique = false;
                    }
                    if (l_IsUnique)
                    {
                        m_Controller.Player.TemplatesUnitQueue.Add(new QueueTemplate(textBoxTemplateUnitQueueNew.Text));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("sword", grepUnits2.getUnitQueue("sword"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("slinger", grepUnits2.getUnitQueue("slinger"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("archer", grepUnits2.getUnitQueue("archer"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("hoplite", grepUnits2.getUnitQueue("hoplite"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("rider", grepUnits2.getUnitQueue("rider"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("chariot", grepUnits2.getUnitQueue("chariot"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("catapult", grepUnits2.getUnitQueue("catapult"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("minotaur", grepUnits2.getUnitQueue("minotaur"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("manticore", grepUnits2.getUnitQueue("manticore"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("centaur", grepUnits2.getUnitQueue("centaur"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("pegasus", grepUnits2.getUnitQueue("pegasus"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("harpy", grepUnits2.getUnitQueue("harpy"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("medusa", grepUnits2.getUnitQueue("medusa"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("zyklop", grepUnits2.getUnitQueue("zyklop"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("cerberus", grepUnits2.getUnitQueue("cerberus"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("fury", grepUnits2.getUnitQueue("fury"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("griffin", grepUnits2.getUnitQueue("griffin"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("calydonian_boar", grepUnits2.getUnitQueue("calydonian_boar"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("godsent", grepUnits2.getUnitQueue("godsent"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("big_transporter", grepUnits2.getUnitQueue("big_transporter"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("bireme", grepUnits2.getUnitQueue("bireme"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("attack_ship", grepUnits2.getUnitQueue("attack_ship"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("demolition_ship", grepUnits2.getUnitQueue("demolition_ship"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("small_transporter", grepUnits2.getUnitQueue("small_transporter"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("trireme", grepUnits2.getUnitQueue("trireme"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("colonize_ship", grepUnits2.getUnitQueue("colonize_ship"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count-1].setUnitTargetAmount("sea_monster", grepUnits2.getUnitQueue("sea_monster"));
                        m_Controller.Player.TemplatesUnitQueue.Sort();
                        loadTemplatesUnitQueue();
                        l_IOHandler.saveTemplatesUnitQueue(m_Controller.Player.TemplatesUnitQueue);
                    }
                    textBoxTemplateUnitQueueNew.Text = "";
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonAddArmyQueueTemplate_Click(): " + ex.Message);
            }
        }

        private void buttonDeleteArmyQueueTemplate_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (comboBoxTemplatesUnitQueue.Items.Count > 0)
                {
                    if (comboBoxTemplatesUnitQueue.SelectedItem != null)
                    {
                        m_Controller.Player.TemplatesUnitQueue.RemoveAt(comboBoxTemplatesUnitQueue.SelectedIndex);
                        loadTemplatesUnitQueue();
                        l_IOHandler.saveTemplatesUnitQueue(m_Controller.Player.TemplatesUnitQueue);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonUpdBuildingQueueTemplate_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (comboBoxTemplatesBuildingQueue.Items.Count > 0)
                {
                    if (comboBoxTemplatesBuildingQueue.SelectedItem != null)
                    {
                        m_Controller.Player.TemplatesBuildingQueue[comboBoxTemplatesBuildingQueue.SelectedIndex].Queue = grepBuildings2.getBuildingLevelsTarget();
                        l_IOHandler.saveTemplatesBuildingQueue(m_Controller.Player.TemplatesBuildingQueue);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonLoadBuildingQueueTemplate_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (comboBoxTemplatesBuildingQueue.Items.Count > 0)
                {
                    if (comboBoxTemplatesBuildingQueue.SelectedItem != null)
                    {
                        grepBuildings2.setBuildingLevelsTarget(m_Controller.Player.TemplatesBuildingQueue[comboBoxTemplatesBuildingQueue.SelectedIndex].Queue);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonDeleteBuildingQueueTemplate_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (comboBoxTemplatesBuildingQueue.Items.Count > 0)
                {
                    if (comboBoxTemplatesBuildingQueue.SelectedItem != null)
                    {
                        m_Controller.Player.TemplatesBuildingQueue.RemoveAt(comboBoxTemplatesBuildingQueue.SelectedIndex);
                        loadTemplatesBuildingQueue();
                        l_IOHandler.saveTemplatesBuildingQueue(m_Controller.Player.TemplatesBuildingQueue);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonAddBuildingQueueTemplate_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (!textBoxTemplateBuildingQueueNew.Text.Equals(""))
                {
                    bool l_IsUnique = true;
                    for (int i = 0; i < m_Controller.Player.TemplatesBuildingQueue.Count; i++)
                    {
                        if (m_Controller.Player.TemplatesBuildingQueue[i].Name.Equals(textBoxTemplateBuildingQueueNew.Text))
                            l_IsUnique = false;
                    }
                    if (l_IsUnique)
                    {
                        m_Controller.Player.TemplatesBuildingQueue.Add(new QueueTemplate(textBoxTemplateBuildingQueueNew.Text, grepBuildings2.getBuildingLevelsTarget()));
                        m_Controller.Player.TemplatesBuildingQueue.Sort();
                        loadTemplatesBuildingQueue();
                        l_IOHandler.saveTemplatesBuildingQueue(m_Controller.Player.TemplatesBuildingQueue);
                    }
                    textBoxTemplateBuildingQueueNew.Text = "";
                }
            }
            catch (Exception)
            {

            }
        }

        private void comboBoxTownListQueue_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                if (comboBoxTownListQueue.Items.Count > 0)
                {
                    m_SelectedTownQueue = ((Town)comboBoxTownListQueue.SelectedItem).TownID;

                    m_SelectedTownCulture = m_SelectedTownQueue;
                    m_SelectedTownFarmers = m_SelectedTownQueue;
                    m_SelectedTownTrade = m_SelectedTownQueue;
                    m_SelectedTownCombo = m_SelectedTownQueue;
                    updateCulturalGUI();
                    updateFarmersGUI();
                    updateTradeGUI();

                    updateComboGUI();
                    updateQueueGUI();
                    buttonSelectAllFarmers.Text = "Select All";
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonSelectQueue_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;

            try
            {
                if (comboBoxTownListQueue.Items.Count > 0)
                {
                    m_SelectedTownQueue = ((Town)comboBoxTownListQueue.SelectedItem).TownID;

                    m_SelectedTownCulture = m_SelectedTownQueue;
                    m_SelectedTownFarmers = m_SelectedTownQueue;
                    m_SelectedTownTrade = m_SelectedTownQueue;
                    m_SelectedTownCombo = m_SelectedTownQueue;
                    updateCulturalGUI();
                    updateFarmersGUI();
                    updateTradeGUI();

                    updateComboGUI();
                    updateQueueGUI();
                    buttonSelectAllFarmers.Text = "Select All";
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonPrevQueue_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListQueue.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownQueue);
                    l_NewIndex = l_CurrentIndex - 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownQueue = m_Controller.Player.Towns[l_NewIndex].TownID;

                        m_SelectedTownCulture = m_SelectedTownQueue;
                        m_SelectedTownFarmers = m_SelectedTownQueue;
                        m_SelectedTownTrade = m_SelectedTownQueue;
                        m_SelectedTownCombo = m_SelectedTownQueue;
                        updateCulturalGUI();
                        updateFarmersGUI();
                        updateTradeGUI();

                        updateComboGUI();
                        updateQueueGUI();
                        buttonSelectAllFarmers.Text = "Select All";
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonNextQueue_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListQueue.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownQueue);
                    l_NewIndex = l_CurrentIndex + 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownQueue = m_Controller.Player.Towns[l_NewIndex].TownID;

                        m_SelectedTownCulture = m_SelectedTownQueue;
                        m_SelectedTownFarmers = m_SelectedTownQueue;
                        m_SelectedTownTrade = m_SelectedTownQueue;
                        m_SelectedTownCombo = m_SelectedTownQueue;
                        updateCulturalGUI();
                        updateFarmersGUI();
                        updateTradeGUI();

                        updateComboGUI();
                        updateQueueGUI();
                        buttonSelectAllFarmers.Text = "Select All";
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        //~Queue tab

        #endregion

        #region GUI Events - Farmers tab

        //Farmers tab
        private void labelMaxResources_Click(object sender, EventArgs e)
        {
            int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownFarmers);
            m_Controller.Player.Towns[l_Index].resetFarmersDailyLimit();
        }

        private void buttonSaveFarmers_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            int l_Index = -1;

            try
            {
                if (m_Controller.CanIModifyFarmers)
                {
                    if (comboBoxTownListFarmers.Items.Count > 0 && !m_SelectedTownFarmers.Equals("0"))
                    {
                        l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownFarmers);

                        //Get enable/disable
                        m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownFarmers)].FarmersLootEnabled = checkBoxFarmersLootEnabled.Checked;

                        /*for (int i = 0; i < m_Controller.Player.Towns.Count; i++)//All towns get the same farmer settings, causes ingame error when academic research is missing
                        {
                            //Get friendly
                            m_Controller.Player.Towns[l_Index].FarmersFriendlyDemandsOnly = checkBoxFriendlyDemandsOnly.Checked;
                            //Get Min. Mood
                            m_Controller.Player.Towns[l_Index].FarmersMinMood = (int)numericUpDownFarmersMinMood.Value;
                        }*/
                        //Get friendly
                        m_Controller.Player.Towns[l_Index].FarmersFriendlyDemandsOnly = checkBoxFriendlyDemandsOnly.Checked;
                        //Get Min. Mood
                        m_Controller.Player.Towns[l_Index].FarmersMinMood = (int)numericUpDownFarmersMinMood.Value;

                        //Get loot interval
                        m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownFarmers)].FarmersLootInterval = domainUpDownLootInterval.Text;
                        //Looting enabled
                        m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownFarmers)].setSelectedFarmers(grepFarmers1.getSelectedFarmers());

                        l_IOHandler.saveTownsSettings(m_Controller.Player);//Town settings
                        l_IOHandler.saveFarmersSettings(m_Controller.Player);//Selected farmers

                        //Farmer (global settings)
                        l_Settings.FarmerScheduler = grepSchedulerSmall1.getScheduler();
                        l_Settings.FarmerMaxResources = (int)numericUpDownFarmerMaxResources.Value;
                        saveSettings();//Program settings
                    }
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonSaveFarmers_Click(): " + ex.Message);
            }
        }

        private void comboBoxTownListFarmers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            try
            {
                if (comboBoxTownListFarmers.Items.Count > 0)
                {
                    m_SelectedTownFarmers = ((Town)comboBoxTownListFarmers.SelectedItem).TownID;
                    buttonSelectAllFarmers.Text = "Select All";

                    m_SelectedTownQueue = m_SelectedTownFarmers;
                    m_SelectedTownCulture = m_SelectedTownFarmers;
                    m_SelectedTownTrade = m_SelectedTownFarmers;
                    m_SelectedTownCombo = m_SelectedTownFarmers;

                    updateComboGUI();
                    updateComboGUI();
                    updateQueueGUI();
                    updateFarmersGUI();
                    updateCulturalGUI();
                    updateTradeGUI();
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonSelectTownFarmers_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            try
            {
                if (comboBoxTownListFarmers.Items.Count > 0)
                {
                    m_SelectedTownFarmers = ((Town)comboBoxTownListFarmers.SelectedItem).TownID;
                    buttonSelectAllFarmers.Text = "Select All";

                    m_SelectedTownQueue = m_SelectedTownFarmers;
                    m_SelectedTownCulture = m_SelectedTownFarmers;
                    m_SelectedTownTrade = m_SelectedTownFarmers;
                    m_SelectedTownCombo = m_SelectedTownFarmers;

                    updateComboGUI();
                    updateComboGUI();
                    updateQueueGUI();
                    updateFarmersGUI();
                    updateCulturalGUI();
                    updateTradeGUI();
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonPrevTownFarmers_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListFarmers.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownFarmers);
                    l_NewIndex = l_CurrentIndex - 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownFarmers = m_Controller.Player.Towns[l_NewIndex].TownID;

                        buttonSelectAllFarmers.Text = "Select All";

                        m_SelectedTownQueue = m_SelectedTownFarmers;
                        m_SelectedTownCulture = m_SelectedTownFarmers;
                        m_SelectedTownTrade = m_SelectedTownFarmers;
                        m_SelectedTownCombo = m_SelectedTownFarmers;
                        updateComboGUI();
                        updateQueueGUI();
                        updateCulturalGUI();
                        updateFarmersGUI();
                        updateTradeGUI();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonNextTownFarmers_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListFarmers.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownFarmers);
                    l_NewIndex = l_CurrentIndex + 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownFarmers = m_Controller.Player.Towns[l_NewIndex].TownID;

                        buttonSelectAllFarmers.Text = "Select All";
                        m_SelectedTownQueue = m_SelectedTownFarmers;
                        m_SelectedTownCulture = m_SelectedTownFarmers;
                        m_SelectedTownTrade = m_SelectedTownFarmers;
                        m_SelectedTownCombo = m_SelectedTownFarmers;
                        updateComboGUI();
                        updateQueueGUI();
                        updateCulturalGUI();
                        updateFarmersGUI();
                        updateTradeGUI();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonSelectAllFarmers_Click(object sender, EventArgs e)
        {
            if (!m_SelectedTownFarmers.Equals("0"))
            {
                if (buttonSelectAllFarmers.Text.Equals("Select All"))
                {
                    grepFarmers1.selectAll();
                    buttonSelectAllFarmers.Text = "Deselect All";
                }
                else
                {
                    grepFarmers1.deselectAll();
                    buttonSelectAllFarmers.Text = "Select All";
                }
            }
        }

        private void buttonSetIntervalAllTownsFarmers_Click(object sender, EventArgs e)
        {
            if (comboBoxTownListFarmers.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to set the loot interval for ALL TOWNS?",
                    "CONFIRM",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {

                    IOHandler l_IOHandler = IOHandler.Instance;
                    bool l_MissingResearch = false;

                    l_MissingResearch = m_Controller.Player.setAllTownsLootInterval(domainUpDownLootInterval.Text);
                    if (!l_MissingResearch)
                    {
                        MessageBox.Show("Loot interval has been set to " + domainUpDownLootInterval.Text, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("You are missing a research, check your academy.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    l_IOHandler.saveTownsSettings(m_Controller.Player);
                }
            }
        }

        private void buttonReloadFarmers_Click(object sender, EventArgs e)
        {
            m_Controller.Player.loadFarmersSettings();
        }
        //~Farmers tab

        #endregion

        #region GUI Events - Trade tab

        //Trade tab
        private void buttonSaveTrade_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            int l_Index = 0;

            try
            {
                if (comboBoxTownListTrade.Items.Count > 0 && !m_SelectedTownTrade.Equals("0"))
                {
                    l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownTrade);

                    //Get enable/disable
                    m_Controller.Player.Towns[l_Index].TradeEnabled = checkBoxTradeEnabled.Checked;

                    if (comboBoxTradeMode.SelectedItem.ToString().Equals("send") || comboBoxTradeMode.SelectedItem.ToString().Equals("receive") || comboBoxTradeMode.SelectedItem.ToString().Equals("spy cave"))
                        m_Controller.Player.Towns[l_Index].TradeMode = comboBoxTradeMode.SelectedItem.ToString();
                    m_Controller.Player.Towns[l_Index].TradeRemainingResources = (int)numericUpDownTradeRemainingResources.Value;
                    m_Controller.Player.Towns[l_Index].TradeMinSendAmount = (int)numericUpDownTradeMinSendAmount.Value;
                    m_Controller.Player.Towns[l_Index].TradePercentageWarehouse = (int)numericUpDownTradePercentageWarehouse.Value;

                    //Maximum trade distance
                    m_Controller.Player.Towns[l_Index].TradeMaxDistance = (int)numericUpDownTradeMaxDistance.Value;

                    l_IOHandler.saveTownsSettings(m_Controller.Player);
                    updateTradeGUI();
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonSaveTrade_Click(): " + ex.Message);
            }
        }

        private void comboBoxTownListTrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            try
            {
                if (comboBoxTownListTrade.Items.Count > 0)
                {
                    m_SelectedTownTrade = ((Town)comboBoxTownListTrade.SelectedItem).TownID;

                    m_SelectedTownQueue = m_SelectedTownTrade;
                    m_SelectedTownFarmers = m_SelectedTownTrade;
                    m_SelectedTownCulture = m_SelectedTownTrade;
                    m_SelectedTownCombo = m_SelectedTownTrade;
                    updateComboGUI();
                    updateQueueGUI();
                    updateFarmersGUI();
                    updateCulturalGUI();
                    updateTradeGUI();
                    buttonSelectAllFarmers.Text = "Select All";
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonSelectTownTrade_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            try
            {
                if (comboBoxTownListTrade.Items.Count > 0)
                {
                    m_SelectedTownTrade = ((Town)comboBoxTownListTrade.SelectedItem).TownID;

                    m_SelectedTownQueue = m_SelectedTownTrade;
                    m_SelectedTownFarmers = m_SelectedTownTrade;
                    m_SelectedTownCulture = m_SelectedTownTrade;
                    m_SelectedTownCombo = m_SelectedTownTrade;
                    updateComboGUI();
                    updateQueueGUI();
                    updateFarmersGUI();
                    updateCulturalGUI();
                    updateTradeGUI();
                    buttonSelectAllFarmers.Text = "Select All";
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonPrevTownTrade_Click(object sender, EventArgs e)
        {
            {
                Settings l_Settings = Settings.Instance;
                int l_CurrentIndex = 0;
                int l_NewIndex = 0;
                try
                {
                    if (comboBoxTownListTrade.Items.Count > 0)
                    {
                        l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownTrade);
                        l_NewIndex = l_CurrentIndex - 1;
                        if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                        {
                            m_SelectedTownTrade = m_Controller.Player.Towns[l_NewIndex].TownID;

                            m_SelectedTownQueue = m_SelectedTownTrade;
                            m_SelectedTownFarmers = m_SelectedTownTrade;
                            m_SelectedTownCulture = m_SelectedTownTrade;
                            m_SelectedTownCombo = m_SelectedTownTrade;
                            updateComboGUI();
                            updateQueueGUI();
                            updateFarmersGUI();
                            updateCulturalGUI();
                            updateTradeGUI();
                            buttonSelectAllFarmers.Text = "Select All";
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void buttonNextTownTrade_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListTrade.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownTrade);
                    l_NewIndex = l_CurrentIndex + 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownTrade = m_Controller.Player.Towns[l_NewIndex].TownID;

                        m_SelectedTownQueue = m_SelectedTownTrade;
                        m_SelectedTownFarmers = m_SelectedTownTrade;
                        m_SelectedTownCulture = m_SelectedTownTrade;
                        m_SelectedTownCombo = m_SelectedTownTrade;
                        updateComboGUI();
                        updateQueueGUI();
                        updateFarmersGUI();
                        updateCulturalGUI();
                        updateTradeGUI();
                        buttonSelectAllFarmers.Text = "Select All";
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /*
         * Doesn't work very well anymore.
         * Used to select the town in the bot that is show in the browser.
         * Currently only works when the browser is refreshed first.
         */ 
        private void buttonSelectTownBrowser_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            int l_Index = -1;
            string l_Search ="\"townId\":";
  
            // example   \"townId\":48718,
            
            try
            {
                getBrowserComplete = false;
                getBrowserLookFor = l_Search;

                webBrowserGrepo.Refresh();


                while (webBrowserGrepo.ReadyState != WebBrowserReadyState.Complete || !getBrowserComplete)
                {
                    Application.DoEvents();
                }

                string l_Response = webBrowserGrepo.DocumentText;

                l_Index = l_Response.IndexOf(l_Search);
                m_SelectedTownTrade = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
           
                if (comboBoxTownListTrade.Items.Count > 0)
                {
                    if (l_Response.Contains(l_Search))
                    {
                        l_Index = l_Response.IndexOf(l_Search);
                        m_SelectedTownTrade = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                        l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownTrade);

                        comboBoxTownListCombo.SelectedIndex = l_Index;
                        comboBoxTownListTrade.SelectedIndex = l_Index;
                        comboBoxTownListQueue.SelectedIndex = l_Index;
                        comboBoxTownListCulture.SelectedIndex = l_Index;
                        comboBoxTownListFarmers.SelectedIndex = l_Index;

                        m_SelectedTownQueue = m_SelectedTownTrade;
                        m_SelectedTownFarmers = m_SelectedTownTrade;
                        m_SelectedTownCulture = m_SelectedTownTrade;
                        m_SelectedTownCombo = m_SelectedTownTrade;

                        updateTradeGUI();
                        updateComboGUI();
                        updateQueueGUI();
                        updateFarmersGUI();
                        updateCulturalGUI();

                        buttonSelectAllFarmers.Text = "Select All";
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void labelTradeMode_Click(object sender, EventArgs e)
        {
            int l_Index = comboBoxTradeMode.SelectedIndex;
            if (l_Index >= 0)
                comboBoxTradeMode.SelectedIndex = ++l_Index % 3;
        }
        //~Trade tab

        #endregion

        #region GUI Events - Cultural tab

        //Cultural tab
        private void buttonSaveCulture_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            int l_Index = 0;

            try
            {
                if (comboBoxTownListCulture.Items.Count > 0 && !m_SelectedTownCulture.Equals("0"))
                {
                    l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownCulture);

                    //Get enable/disable
                    m_Controller.Player.Towns[l_Index].CulturalFestivalsEnabled = checkBoxCultureEnabled.Checked;

                    //Festivals
                    m_Controller.Player.Towns[l_Index].CulturalPartyEnabled = grepCulture1.EnabledParty;
                    m_Controller.Player.Towns[l_Index].CulturalGamesEnabled = grepCulture1.EnabledGames;
                    m_Controller.Player.Towns[l_Index].CulturalTriumphEnabled = grepCulture1.EnabledTriumph;
                    m_Controller.Player.Towns[l_Index].CulturalTheaterEnabled = grepCulture1.EnabledTheater;

                    l_IOHandler.saveTownsSettings(m_Controller.Player);
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonSaveCulture_Click(): " + ex.Message);
            }
        }

        private void comboBoxTownListCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            try
            {
                if (comboBoxTownListCulture.Items.Count > 0)
                {
                    m_SelectedTownCulture = ((Town)comboBoxTownListCulture.SelectedItem).TownID;

                    m_SelectedTownQueue = m_SelectedTownCulture;
                    m_SelectedTownFarmers = m_SelectedTownCulture;
                    m_SelectedTownTrade = m_SelectedTownCulture;
                    m_SelectedTownCombo = m_SelectedTownCulture;
                    updateComboGUI();
                    updateQueueGUI();
                    updateFarmersGUI();
                    updateTradeGUI();
                    updateCulturalGUI();
                    buttonSelectAllFarmers.Text = "Select All";
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonSelectCulture_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            try
            {
                if (comboBoxTownListCulture.Items.Count > 0)
                {
                    m_SelectedTownCulture = ((Town)comboBoxTownListCulture.SelectedItem).TownID;

                    m_SelectedTownQueue = m_SelectedTownCulture;
                    m_SelectedTownFarmers = m_SelectedTownCulture;
                    m_SelectedTownTrade = m_SelectedTownCulture;
                    m_SelectedTownCombo = m_SelectedTownCulture;
                    updateComboGUI();
                    updateQueueGUI();
                    updateFarmersGUI();
                    updateTradeGUI();
                    updateCulturalGUI();
                    buttonSelectAllFarmers.Text = "Select All";
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonPrevCulture_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListCulture.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownCulture);
                    l_NewIndex = l_CurrentIndex - 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownCulture = m_Controller.Player.Towns[l_NewIndex].TownID;

                        m_SelectedTownQueue = m_SelectedTownCulture;
                        m_SelectedTownFarmers = m_SelectedTownCulture;
                        m_SelectedTownTrade = m_SelectedTownCulture;
                        m_SelectedTownCombo = m_SelectedTownCulture;
                        updateComboGUI();
                        updateQueueGUI();
                        updateFarmersGUI();
                        updateTradeGUI();
                        updateCulturalGUI();
                        buttonSelectAllFarmers.Text = "Select All";
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonNextCulture_Click(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListCulture.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownCulture);
                    l_NewIndex = l_CurrentIndex + 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownCulture = m_Controller.Player.Towns[l_NewIndex].TownID;

                        m_SelectedTownQueue = m_SelectedTownCulture;
                        m_SelectedTownFarmers = m_SelectedTownCulture;
                        m_SelectedTownTrade = m_SelectedTownCulture;
                        m_SelectedTownCombo = m_SelectedTownCulture;
                        updateComboGUI();
                        updateQueueGUI();
                        updateFarmersGUI();
                        updateTradeGUI();
                        updateCulturalGUI();
                        buttonSelectAllFarmers.Text = "Select All";
                    }
                }
            }
            catch (Exception)
            {

            }
        }

      
        //~Cultural tab

        #endregion

        #region GUI Events - Grid Overview and Images
   
        void dataGridOverview_Scroll(object sender, ScrollEventArgs e)
        {

            try
            {
                DataGridView src;
                DataGridView dst1 = null;

                src = (DataGridView)sender;

                if (src == dataGridOverview)
                    dst1 = dataGridOverviewImages;

                else if (src == dataGridOverviewImages)
                    dst1 = dataGridOverview;


                if (dst1 != null)
                {
                    dst1.HorizontalScrollingOffset = src.HorizontalScrollingOffset;
                    if (dst1.RowCount > 1)
                    {
                        dst1.FirstDisplayedScrollingRowIndex = Math.Min(dst1.RowCount - 1, src.FirstDisplayedScrollingRowIndex);
                    }
                }

            }
            catch
            {

            }
        }

        private void sortGridOverview()
        {
            // Check which column is selected, otherwise set NewColumn to null.
            DataGridViewColumn newColumn =
                dataGridOverviewImages.Columns.GetColumnCount(
                DataGridViewElementStates.Selected) == 1 ?
                dataGridOverviewImages.SelectedColumns[0] : null;

            DataGridViewColumn oldColumn = dataGridOverviewImages.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not currently sorted. 
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder. 
                if (oldColumn == newColumn &&
                    dataGridOverviewImages.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            // If no column has been selected, display an error dialog  box. 
            if (newColumn == null)
            {
                MessageBox.Show("Select a single column and try again.",
                    "Error: Invalid Selection", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                dataGridOverview.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private void dataGridOveriew_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            try
            {
                if (e.RowIndex1 == this.dataGridOverview.Rows.Count - 1)
                    e.Handled = true;

                if (e.RowIndex2 == this.dataGridOverview.Rows.Count - 1)
                    e.Handled = true;

                // sortGridOverview();

                return;
            }
            catch (Exception ex)
            {
                // ex.ToString();
            }
        }

        private void dataGridOverview_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            string TownID = dataGridOverview.CurrentRow.Cells["dataGridOverview_ID"].Value.ToString();

            int l_Index = m_Controller.Player.getTownIndexByID(TownID);

            comboBoxTownListCombo.SelectedIndex = l_Index;

            updateComboGUI();


            if (e.ColumnIndex == 2)
            {
                Debug.Print("Click Cell " + dataGridOverview.CurrentRow.Cells[2].Value.ToString());

                if (dataGridOverview.CurrentRow.Cells[2].Value.ToString() == "R")
                {
                    dataGridOverview.CurrentRow.Cells[2].Value = "S";
                    comboBoxTradeModeCombo.SelectedIndex = 0;
                }
                else if (dataGridOverview.CurrentRow.Cells[2].Value.ToString() == "S")
                {
                    dataGridOverview.CurrentRow.Cells[2].Value = "R";
                    comboBoxTradeModeCombo.SelectedIndex = 1;
                }
                saveComboAll();
            }

            if (e.ColumnIndex == 1)//Select combo tab
            {
                tabControl1.SelectedIndex = 7;
            }
        }

        public void loadGridOverviewImages()
        {
            int x;

            try
            {

                dataGridOverviewImages.Rows.Clear();
                dataGridOverviewImages.Refresh();

                dataGridOverviewImages.BackgroundColor = ColorTranslator.FromHtml("#fff");
                dataGridOverviewImages.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                dataGridOverviewImages.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#9ac0e4");

                dataGridOverviewImages.RowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#ebf2fa");
                dataGridOverviewImages.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#fff");

                dataGridOverviewImages.EnableHeadersVisualStyles = false;

                dataGridOverviewImages.Columns[1].Frozen = true;




                x = 1;
                dataGridOverviewImages.Rows.Add(0, "");

                dataGridOverviewImages.Rows[0].Cells[x++].Value = DateTime.Now;

                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\trademode_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\free_pop_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\wood_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\stone_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\iron_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\storage_25x25.png");
                x++;

                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\main_20x20.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\barracks_20x20.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\docks_20x20.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\farmer_20x20.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\quest_island_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\attack_in.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\favor_25x25.png");
                x++;
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\sword_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\slinger_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\archer_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\hoplite_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\rider_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\chariot_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\catapult_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\minotaur_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\manticore_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\centaur_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\pegasus_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\harpy_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\medusa_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\zyklop_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\cerberus_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\fury_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\griffin_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\calydonian_boar_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\godsent_25x25.png");
                x++;
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\big_transporter_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\bireme_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\attack_ship_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\demolition_ship_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\small_transporter_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\trireme_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\colonize_ship_25x25.png");
                dataGridOverviewImages.Rows[0].Cells[x++].Value = Image.FromFile("Overview\\sea_monster_25x25.png");

                for (x = 2; x < dataGridOverviewImages.Columns.Count; x++)
                {
                    dataGridOverviewImages.Columns[x].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridOverviewImages.Columns[x].Width = dataGridOverview.Columns[x].Width;

                }

            } //try

            catch (Exception ex)
            {
                Debug.Print("ERROR " + ex.Message + " t:" + ex.StackTrace);

            }
        }

        //~Grid Overview tab

        #endregion

        #region GUI Events - Combo tab

        //Combo tab

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.Handled = true;
            }

            if (tabControl1.SelectedIndex == 7) // combo tab
            {
                Debug.Print("keypress " + e.KeyCode);

                switch (e.KeyCode)
                {
                    case Keys.Left:
                        doQuickSet(2);
                        break;

                    case Keys.Right:
                        doQuickSet(3);
                        break;
                }
            }
        }

        private void loadTownGrid()
        {
            try
            {
                townGridCombo.Rows.Clear();
                townGridCombo.Refresh();

                this.townGridCombo.Columns["Population"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                townGridCombo.BackgroundColor = ColorTranslator.FromHtml("#fff");
                townGridCombo.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                townGridCombo.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#9ac0e4");
                townGridCombo.RowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#ebf2fa");
                townGridCombo.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#fff");

                townGridCombo.EnableHeadersVisualStyles = false;

                for (int i = 0; i < m_Controller.Player.Towns.Count; i++)
                {

                    switch (m_Controller.Player.Towns[i].TradeMode)
                    {
                        case "receive":
                            townGridCombo.Rows.Add("R", m_Controller.Player.Towns[i].TradeOmitFromChangeAll, m_Controller.Player.Towns[i], m_Controller.Player.Towns[i].PopulationAvailable, m_Controller.Player.Towns[i].TownID);
                            break;

                        case "send":
                            townGridCombo.Rows.Add("S", m_Controller.Player.Towns[i].TradeOmitFromChangeAll, m_Controller.Player.Towns[i], m_Controller.Player.Towns[i].PopulationAvailable, m_Controller.Player.Towns[i].TownID);
                            break;

                        case "spy cave":
                            townGridCombo.Rows.Add("SP", m_Controller.Player.Towns[i].TradeOmitFromChangeAll, m_Controller.Player.Towns[i], m_Controller.Player.Towns[i].PopulationAvailable, m_Controller.Player.Towns[i].TownID);
                            break;

                    } //switch
                }//for i
            }
            catch// (Exception ex)
            {

            }
        } //loadTownGrid

        private void townGridCombo_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = this.townGridCombo.Rows[e.RowIndex];

                string TownID = row.Cells["ID"].Value.ToString();

                int l_Index = m_Controller.Player.getTownIndexByID(TownID);

                comboBoxTownListCombo.SelectedIndex = l_Index;

                //updateComboGUI();

                if (e.ColumnIndex == 1)
                {
                    //CheckBox chk = new CheckBox();
                    //chk  = (CheckBox)gatagrid.Rows[0].Cells[0].FindControl["chkId"];
                    //if(chk.Checked == true){

                    Debug.Print("Click Cell Omit " + townGridCombo.CurrentRow.Cells[1].Value.ToString());

                    if ((bool)(townGridCombo.CurrentRow.Cells[1].Value) == true)
                        checkBoxTradeOmitFromChangeAll.Checked = false;
                    else
                        checkBoxTradeOmitFromChangeAll.Checked = true;

                    saveComboAll();
                }

                if (e.ColumnIndex == 0)
                {
                    Debug.Print("Click Cell Trade " + townGridCombo.CurrentRow.Cells[0].Value.ToString());

                    if (townGridCombo.CurrentRow.Cells[0].Value.ToString() == "R")
                    {
                        townGridCombo.CurrentRow.Cells[0].Value = "S";
                        comboBoxTradeModeCombo.SelectedIndex = 0;
                    }
                    else if (townGridCombo.CurrentRow.Cells[0].Value.ToString() == "S")
                    {
                        townGridCombo.CurrentRow.Cells[0].Value = "R";
                        comboBoxTradeModeCombo.SelectedIndex = 1;
                    }
                    saveComboAll();
                }
                updateComboGUI();
            }
            catch (Exception ex)// (Exception ex)
            {
                Debug.Print("Click Error " + ex.Message + "  " + ex.StackTrace);
            }
        }

        private void buttonUpdArmyQueueTemplateCombo_Click(object sender, EventArgs e)
        {
            if (comboBoxTemplatesUnitQueueCombo.Items.Count > 0)
            {
                if (comboBoxTemplatesUnitQueueCombo.SelectedItem != null)
                {
                    if (
                        MessageBox.Show("Are you sure you want to REPLACE the current template with the Unit selection above?",
                        "CONFIRM",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        //Note Modify this when adding new units (07/15)
                        IOHandler l_IOHandler = IOHandler.Instance;
                        Settings l_Settings = Settings.Instance;

                        try
                        {
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("sword", grepUnitsCombo.getUnitQueue("sword"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("slinger", grepUnitsCombo.getUnitQueue("slinger"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("archer", grepUnitsCombo.getUnitQueue("archer"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("hoplite", grepUnitsCombo.getUnitQueue("hoplite"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("rider", grepUnitsCombo.getUnitQueue("rider"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("chariot", grepUnitsCombo.getUnitQueue("chariot"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("catapult", grepUnitsCombo.getUnitQueue("catapult"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("minotaur", grepUnitsCombo.getUnitQueue("minotaur"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("manticore", grepUnitsCombo.getUnitQueue("manticore"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("centaur", grepUnitsCombo.getUnitQueue("centaur"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("pegasus", grepUnitsCombo.getUnitQueue("pegasus"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("harpy", grepUnitsCombo.getUnitQueue("harpy"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("medusa", grepUnitsCombo.getUnitQueue("medusa"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("zyklop", grepUnitsCombo.getUnitQueue("zyklop"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("cerberus", grepUnitsCombo.getUnitQueue("cerberus"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("fury", grepUnitsCombo.getUnitQueue("fury"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("griffin", grepUnitsCombo.getUnitQueue("griffin"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("calydonian_boar", grepUnitsCombo.getUnitQueue("calydonian_boar"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("godsent", grepUnitsCombo.getUnitQueue("godsent"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("big_transporter", grepUnitsCombo.getUnitQueue("big_transporter"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("bireme", grepUnitsCombo.getUnitQueue("bireme"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("attack_ship", grepUnitsCombo.getUnitQueue("attack_ship"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("demolition_ship", grepUnitsCombo.getUnitQueue("demolition_ship"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("small_transporter", grepUnitsCombo.getUnitQueue("small_transporter"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("trireme", grepUnitsCombo.getUnitQueue("trireme"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("colonize_ship", grepUnitsCombo.getUnitQueue("colonize_ship"));
                            m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].setUnitTargetAmount("sea_monster", grepUnitsCombo.getUnitQueue("sea_monster"));

                            l_IOHandler.saveTemplatesUnitQueue(m_Controller.Player.TemplatesUnitQueue);
                        }
                        catch (Exception ex)
                        {
                            if (l_Settings.AdvDebugMode)
                                l_IOHandler.debug("Exception in buttonUpdArmyQueueTemplateCombo_Click(): " + ex.Message);
                        }
                    }// messagebox
                } // !null
            } // count
        } 

        private void buttonLoadArmyQueueTemplateCombo_Click(object sender, EventArgs e)
        {
            //Note Modify this when adding new units (08/15)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (comboBoxTemplatesUnitQueueCombo.Items.Count > 0)
                {
                    if (comboBoxTemplatesUnitQueueCombo.SelectedItem != null)
                    {
                        grepUnitsCombo.setUnitQueue("sword", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("sword"));
                        grepUnitsCombo.setUnitQueue("slinger", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("slinger"));
                        grepUnitsCombo.setUnitQueue("archer", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("archer"));
                        grepUnitsCombo.setUnitQueue("hoplite", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("hoplite"));
                        grepUnitsCombo.setUnitQueue("rider", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("rider"));
                        grepUnitsCombo.setUnitQueue("chariot", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("chariot"));
                        grepUnitsCombo.setUnitQueue("catapult", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("catapult"));
                        grepUnitsCombo.setUnitQueue("minotaur", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("minotaur"));
                        grepUnitsCombo.setUnitQueue("manticore", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("manticore"));
                        grepUnitsCombo.setUnitQueue("centaur", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("centaur"));
                        grepUnitsCombo.setUnitQueue("pegasus", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("pegasus"));
                        grepUnitsCombo.setUnitQueue("harpy", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("harpy"));
                        grepUnitsCombo.setUnitQueue("medusa", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("medusa"));
                        grepUnitsCombo.setUnitQueue("zyklop", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("zyklop"));
                        grepUnitsCombo.setUnitQueue("cerberus", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("cerberus"));
                        grepUnitsCombo.setUnitQueue("fury", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("fury"));
                        grepUnitsCombo.setUnitQueue("griffin", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("griffin"));
                        grepUnitsCombo.setUnitQueue("calydonian_boar", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("calydonian_boar"));
                        grepUnitsCombo.setUnitQueue("godsent", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("godsent"));
                        grepUnitsCombo.setUnitQueue("big_transporter", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("big_transporter"));
                        grepUnitsCombo.setUnitQueue("bireme", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("bireme"));
                        grepUnitsCombo.setUnitQueue("attack_ship", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("attack_ship"));
                        grepUnitsCombo.setUnitQueue("demolition_ship", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("demolition_ship"));
                        grepUnitsCombo.setUnitQueue("small_transporter", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("small_transporter"));
                        grepUnitsCombo.setUnitQueue("trireme", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("trireme"));
                        grepUnitsCombo.setUnitQueue("colonize_ship", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("colonize_ship"));
                        grepUnitsCombo.setUnitQueue("sea_monster", m_Controller.Player.TemplatesUnitQueue[comboBoxTemplatesUnitQueueCombo.SelectedIndex].getUnitTargetAmount("sea_monster"));
                    }
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonLoadArmyQueueTemplateCombo_Click(): " + ex.Message);
            }
        }

        private void buttonDeleteArmyQueueTemplateCombo_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (comboBoxTemplatesUnitQueueCombo.Items.Count > 0)
                {
                    if (comboBoxTemplatesUnitQueueCombo.SelectedItem != null)
                    {
                        if (MessageBox.Show("Are you sure you want to DELETE the current template?",
                            "CONFIRM",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question)
                            == DialogResult.Yes)
                        {
                            m_Controller.Player.TemplatesUnitQueue.RemoveAt(comboBoxTemplatesUnitQueueCombo.SelectedIndex);
                            loadTemplatesUnitQueue();
                            l_IOHandler.saveTemplatesUnitQueue(m_Controller.Player.TemplatesUnitQueue);
                        }//!nul
                    }// count > 0
                }//message
            }
            catch (Exception)
            {

            }
        }

        private void buttonAddArmyQueueTemplateCombo_Click(object sender, EventArgs e)
        {
            //Note Modify this when adding new units (09/15)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (!textBoxTemplateUnitQueueNewCombo.Text.Equals(""))
                {
                    bool l_IsUnique = true;
                    for (int i = 0; i < m_Controller.Player.TemplatesUnitQueue.Count; i++)
                    {
                        if (m_Controller.Player.TemplatesUnitQueue[i].Name.Equals(textBoxTemplateUnitQueueNewCombo.Text))
                            l_IsUnique = false;
                    }
                    if (l_IsUnique)
                    {
                        m_Controller.Player.TemplatesUnitQueue.Add(new QueueTemplate(textBoxTemplateUnitQueueNewCombo.Text));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("sword", grepUnitsCombo.getUnitQueue("sword"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("slinger", grepUnitsCombo.getUnitQueue("slinger"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("archer", grepUnitsCombo.getUnitQueue("archer"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("hoplite", grepUnitsCombo.getUnitQueue("hoplite"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("rider", grepUnitsCombo.getUnitQueue("rider"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("chariot", grepUnitsCombo.getUnitQueue("chariot"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("catapult", grepUnitsCombo.getUnitQueue("catapult"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("minotaur", grepUnitsCombo.getUnitQueue("minotaur"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("manticore", grepUnitsCombo.getUnitQueue("manticore"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("centaur", grepUnitsCombo.getUnitQueue("centaur"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("pegasus", grepUnitsCombo.getUnitQueue("pegasus"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("harpy", grepUnitsCombo.getUnitQueue("harpy"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("medusa", grepUnitsCombo.getUnitQueue("medusa"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("zyklop", grepUnitsCombo.getUnitQueue("zyklop"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("cerberus", grepUnitsCombo.getUnitQueue("cerberus"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("fury", grepUnitsCombo.getUnitQueue("fury"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("griffin", grepUnitsCombo.getUnitQueue("griffin"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("calydonian_boar", grepUnitsCombo.getUnitQueue("calydonian_boar"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("godsent", grepUnitsCombo.getUnitQueue("godsent"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("big_transporter", grepUnitsCombo.getUnitQueue("big_transporter"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("bireme", grepUnitsCombo.getUnitQueue("bireme"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("attack_ship", grepUnitsCombo.getUnitQueue("attack_ship"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("demolition_ship", grepUnitsCombo.getUnitQueue("demolition_ship"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("small_transporter", grepUnitsCombo.getUnitQueue("small_transporter"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("trireme", grepUnitsCombo.getUnitQueue("trireme"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("colonize_ship", grepUnitsCombo.getUnitQueue("colonize_ship"));
                        m_Controller.Player.TemplatesUnitQueue[m_Controller.Player.TemplatesUnitQueue.Count - 1].setUnitTargetAmount("sea_monster", grepUnitsCombo.getUnitQueue("sea_monster"));
                        m_Controller.Player.TemplatesUnitQueue.Sort();
                        loadTemplatesUnitQueue();
                        l_IOHandler.saveTemplatesUnitQueue(m_Controller.Player.TemplatesUnitQueue);
                    }
                    textBoxTemplateUnitQueueNew.Text = "";
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonAddArmyQueueTemplateCombo_Click(): " + ex.Message);
            }
        }

        private void buttonUpdBuildingQueueTemplateCombo_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (comboBoxTemplatesBuildingQueueCombo.Items.Count > 0)
                {
                    if (comboBoxTemplatesBuildingQueueCombo.SelectedItem != null)
                    {

                        if (MessageBox.Show("Are you sure you want to REPLACE the current template with the building selection above?",
                            "CONFIRM",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question)
                            == DialogResult.Yes)
                        {
                            m_Controller.Player.TemplatesBuildingQueue[comboBoxTemplatesBuildingQueueCombo.SelectedIndex].Queue = grepBuildingsCombo.getBuildingLevelsTarget();
                            l_IOHandler.saveTemplatesBuildingQueue(m_Controller.Player.TemplatesBuildingQueue);
                        } // messagebox
                    } // !null
                } // count > 0
            }
            catch (Exception)
            {

            }
        }

        private void buttonLoadBuildingQueueTemplateCombo_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (comboBoxTemplatesBuildingQueueCombo.Items.Count > 0)
                {
                    if (comboBoxTemplatesBuildingQueueCombo.SelectedItem != null)
                    {
                        grepBuildingsCombo.setBuildingLevelsTarget(m_Controller.Player.TemplatesBuildingQueue[comboBoxTemplatesBuildingQueueCombo.SelectedIndex].Queue);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void buttonDeleteBuildingQueueTemplateCombo_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (comboBoxTemplatesBuildingQueueCombo.Items.Count > 0)
                {
                    if (comboBoxTemplatesBuildingQueueCombo.SelectedItem != null)
                    {
                        if (MessageBox.Show("Are you sure you want to DELETE the current template?",
                            "CONFIRM",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question)
                            == DialogResult.Yes)
                        {
                            m_Controller.Player.TemplatesBuildingQueue.RemoveAt(comboBoxTemplatesBuildingQueueCombo.SelectedIndex);
                            loadTemplatesBuildingQueue();
                            l_IOHandler.saveTemplatesBuildingQueue(m_Controller.Player.TemplatesBuildingQueue);
                        } // message
                    }// !null
                } // > 0 count
            }
            catch (Exception)
            {

            }
        }

        private void buttonAddBuildingQueueTemplateCombo_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (!textBoxTemplateBuildingQueueNewCombo.Text.Equals(""))
                {
                    bool l_IsUnique = true;
                    for (int i = 0; i < m_Controller.Player.TemplatesBuildingQueue.Count; i++)
                    {
                        if (m_Controller.Player.TemplatesBuildingQueue[i].Name.Equals(textBoxTemplateBuildingQueueNew.Text))
                            l_IsUnique = false;
                    }
                    if (l_IsUnique)
                    {
                        m_Controller.Player.TemplatesBuildingQueue.Add(new QueueTemplate(textBoxTemplateBuildingQueueNewCombo.Text, grepBuildingsCombo.getBuildingLevelsTarget()));
                        m_Controller.Player.TemplatesBuildingQueue.Sort();
                        loadTemplatesBuildingQueue();
                        l_IOHandler.saveTemplatesBuildingQueue(m_Controller.Player.TemplatesBuildingQueue);
                    }
                    textBoxTemplateBuildingQueueNewCombo.Text = "";
                }
            }
            catch (Exception)
            {

            }
        }

        private void comboBoxTownListCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
    
            Settings l_Settings = Settings.Instance;

            try
            {
                if (comboBoxTownListCombo.Items.Count > 0)
                {
                    m_SelectedTownCombo = ((Town)comboBoxTownListCombo.SelectedItem).TownID;

                    m_SelectedTownQueue = m_SelectedTownCombo;
                    m_SelectedTownCulture = m_SelectedTownCombo;
                    m_SelectedTownFarmers = m_SelectedTownCombo;
                    m_SelectedTownTrade = m_SelectedTownCombo;
                    updateQueueGUI();
                    updateCulturalGUI();
                    updateFarmersGUI();
                    updateTradeGUI();
                    updateComboGUI();
                    buttonSelectAllFarmers.Text = "Select All";
                }
            }
            catch (Exception)
            {

            }
        }

        private void doQuickSet(int x)
        {
            switch (x)
            {
                case 1: // save
                    saveComboAll();
                    break;

                case 2:  //prev
                    previousCombo();
                    break;

                case 3:  //next
                    nextCombo();
                    break;

                case 4: // unit queue check
                    checkBoxUnitQueueEnabledCombo.Checked = !checkBoxUnitQueueEnabledCombo.Checked;
                    break;

                case 5:  //building
                    checkBoxBuildingQueueEnabledCombo.Checked = !checkBoxBuildingQueueEnabledCombo.Checked;
                    break;

                case 6:  //target
                    checkBoxBuildingQueueTargetCombo.Checked = !checkBoxBuildingQueueTargetCombo.Checked;
                    break;

                case 7:  //trading
                    checkBoxTradeEnabledCombo.Checked = !checkBoxTradeEnabledCombo.Checked;
                    break;

                case 8: // send
                    comboBoxTradeModeCombo.SelectedIndex = 0;
                    break;

                case 9: // receive
                    comboBoxTradeModeCombo.SelectedIndex = 1;
                    break;

                case 0: //spy
                    comboBoxTradeModeCombo.SelectedIndex = 2;
                    break;
            }
        }
     
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int x;

            if (textBox1.Text.Length == 1 && Regex.IsMatch(textBox1.Text, @"^\d+$"))
            {
                x = int.Parse(textBox1.Text);
                doQuickSet(x);
            } // len == 1
            textBox1.Text = "";
        }

        void previousCombo()
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListCombo.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownCombo);
                    l_NewIndex = l_CurrentIndex - 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownCombo = m_Controller.Player.Towns[l_NewIndex].TownID;
                        m_SelectedTownQueue = m_SelectedTownCombo;
                        m_SelectedTownCulture = m_SelectedTownCombo;
                        m_SelectedTownFarmers = m_SelectedTownCombo;
                        m_SelectedTownTrade = m_SelectedTownCombo;
                        updateQueueGUI();
                        updateCulturalGUI();
                        updateFarmersGUI();
                        updateTradeGUI();
                        updateComboGUI();
                        buttonSelectAllFarmers.Text = "Select All";
                    }
                }
            }
            catch (Exception)
            {

            }
        }
     
        private void buttonPrevCombo_Click(object sender, EventArgs e)
        {
            previousCombo();
        }

        void nextCombo ()
        {
            Settings l_Settings = Settings.Instance;
            int l_CurrentIndex = 0;
            int l_NewIndex = 0;
            try
            {
                if (comboBoxTownListCombo.Items.Count > 0)
                {
                    l_CurrentIndex = m_Controller.Player.getTownIndexByID(m_SelectedTownCombo);
                    l_NewIndex = l_CurrentIndex + 1;
                    if (l_NewIndex >= 0 && l_NewIndex < m_Controller.Player.Towns.Count)
                    {
                        m_SelectedTownCombo = m_Controller.Player.Towns[l_NewIndex].TownID;
                        m_SelectedTownQueue = m_SelectedTownCombo;
                        m_SelectedTownCulture = m_SelectedTownCombo;
                        m_SelectedTownFarmers = m_SelectedTownCombo;
                        m_SelectedTownTrade = m_SelectedTownCombo;
                        updateQueueGUI();
                        updateCulturalGUI();
                        updateFarmersGUI();
                        updateTradeGUI();
                        updateComboGUI();
                        buttonSelectAllFarmers.Text = "Select All";
                    }
                }
            }
            catch (Exception)
            {

            }
        }


        private void buttonNextCombo_Click(object sender, EventArgs e)
        {
            nextCombo();
        }

        private void labelTradeModeCombo_Click(object sender, EventArgs e)
        {
            int l_Index = comboBoxTradeModeCombo.SelectedIndex;
            if (l_Index >= 0)
                comboBoxTradeModeCombo.SelectedIndex = ++l_Index % 3;
        }

        private void numericUpDownTradeMaxDistanceCombo_ValueChanged(object sender, EventArgs e)
        {
            int l_Index = comboBoxTradeModeCombo.SelectedIndex;
            if (l_Index >= 0)
                m_Tooltip.SetToolTip(labelMaximumTradeDistanceCombo, m_Controller.Player.getTradeTownsInRange(l_Index));
        }

        private void buttonFeaturesAllTowns_Click(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            if (comboBoxTownListCombo.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to enable the same features for all your towns?",
                    "CONFIRM",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    for (int i = 0; i < m_Controller.Player.Towns.Count; i++)
                    {
                        m_Controller.Player.Towns[i].UnitQueueEnabled = checkBoxUnitQueueEnabledCombo.Checked;
                        m_Controller.Player.Towns[i].BuildingQueueEnabled = checkBoxBuildingQueueEnabledCombo.Checked;
                        m_Controller.Player.Towns[i].BuildingLevelsTargetEnabled = checkBoxBuildingQueueTargetCombo.Checked;
                        m_Controller.Player.Towns[i].BuildingDowngradeEnabled = checkBoxBuildingDowngradeCombo.Checked;
                        m_Controller.Player.Towns[i].TradeEnabled = checkBoxTradeEnabledCombo.Checked;
                        m_Controller.Player.Towns[i].CulturalFestivalsEnabled = checkBoxCultureEnabledCombo.Checked;
                        m_Controller.Player.Towns[i].CulturalPartyEnabled = grepCultureCombo.EnabledParty;
                        m_Controller.Player.Towns[i].CulturalGamesEnabled = grepCultureCombo.EnabledGames;
                        m_Controller.Player.Towns[i].CulturalTriumphEnabled = grepCultureCombo.EnabledTriumph;
                        m_Controller.Player.Towns[i].CulturalTheaterEnabled = grepCultureCombo.EnabledTheater;
                    }
                    l_IOHandler.saveTownsSettings(m_Controller.Player);
                }
            }
        }

        private void buttonAutoSendCombo_Click(object sender, EventArgs e)
        {
            if (comboBoxTownListCombo.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to update ALL TOWNS to send mode?",
                    "CONFIRM",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {

                    IOHandler l_IOHandler = IOHandler.Instance;

                    m_Controller.Player.autoUpdateTownTrade();

                    l_IOHandler.saveTownsSettings(m_Controller.Player);

                    comboBoxTradeModeCombo.SelectedIndex = 0;

                    loadTownGrid();

                    MessageBox.Show("All towns have been set to SEND", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void buttonAutoSendValue_Click(object sender, EventArgs e)
        {
            if (comboBoxTownListCombo.Items.Count > 0)
            {
                int pop = (int)numericAutoSendPopulation.Value;
                int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownCombo);
                string msg = "Are you sure you want to UPDATE ALL TOWNS to receive above " + pop + " popultion";
                int numChanges = 0;

                if (MessageBox.Show(msg,
                    "CONFIRM",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    IOHandler l_IOHandler = IOHandler.Instance;

                    numChanges = m_Controller.Player.autoUpdateTownTrade(pop);


                    l_IOHandler.saveTownsSettings(m_Controller.Player);

                    // make sure we update the gui too
                    if (m_Controller.Player.Towns[l_Index].PopulationAvailable > pop)
                        comboBoxTradeModeCombo.SelectedIndex = 1;
                    else
                        comboBoxTradeModeCombo.SelectedIndex = 0;

                    loadTownGrid();

                    MessageBox.Show(numChanges + " towns have been set to RECEIVE", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void playSoundInfo()
        {
            Settings l_Settings = Settings.Instance;
            SoundPlayer sndPing = new SoundPlayer(l_Settings.SoundSaveLocation);
            sndPing.Play();
        }

        void saveComboAll()
        {
            //Note Modify this when adding new units (10/15)
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                if (comboBoxTownListCombo.Items.Count > 0 && !m_SelectedTownCombo.Equals("0"))
                {
                    int l_Index = m_Controller.Player.getTownIndexByID(m_SelectedTownCombo);
                    //Queue related
                    m_Controller.Player.Towns[l_Index].BuildingQueueEnabled = checkBoxBuildingQueueEnabledCombo.Checked;
                    m_Controller.Player.Towns[l_Index].BuildingLevelsTargetEnabled = checkBoxBuildingQueueTargetCombo.Checked;
                    m_Controller.Player.Towns[l_Index].BuildingDowngradeEnabled = checkBoxBuildingDowngradeCombo.Checked;
                    m_Controller.Player.Towns[l_Index].setBuildingQueue(grepBuildingsCombo.getBuildingQueue());
                    m_Controller.Player.Towns[l_Index].setBuildingsLevelTarget(grepBuildingsCombo.getBuildingLevelsTarget());

                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("sword", grepUnitsCombo.getUnitQueue("sword"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("slinger", grepUnitsCombo.getUnitQueue("slinger"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("archer", grepUnitsCombo.getUnitQueue("archer"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("hoplite", grepUnitsCombo.getUnitQueue("hoplite"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("rider", grepUnitsCombo.getUnitQueue("rider"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("chariot", grepUnitsCombo.getUnitQueue("chariot"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("catapult", grepUnitsCombo.getUnitQueue("catapult"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("minotaur", grepUnitsCombo.getUnitQueue("minotaur"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("manticore", grepUnitsCombo.getUnitQueue("manticore"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("centaur", grepUnitsCombo.getUnitQueue("centaur"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("pegasus", grepUnitsCombo.getUnitQueue("pegasus"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("harpy", grepUnitsCombo.getUnitQueue("harpy"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("medusa", grepUnitsCombo.getUnitQueue("medusa"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("zyklop", grepUnitsCombo.getUnitQueue("zyklop"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("cerberus", grepUnitsCombo.getUnitQueue("cerberus"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("fury", grepUnitsCombo.getUnitQueue("fury"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("griffin", grepUnitsCombo.getUnitQueue("griffin"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("calydonian_boar", grepUnitsCombo.getUnitQueue("calydonian_boar"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("godsent", grepUnitsCombo.getUnitQueue("godsent"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("big_transporter", grepUnitsCombo.getUnitQueue("big_transporter"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("bireme", grepUnitsCombo.getUnitQueue("bireme"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("attack_ship", grepUnitsCombo.getUnitQueue("attack_ship"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("demolition_ship", grepUnitsCombo.getUnitQueue("demolition_ship"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("small_transporter", grepUnitsCombo.getUnitQueue("small_transporter"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("trireme", grepUnitsCombo.getUnitQueue("trireme"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("colonize_ship", grepUnitsCombo.getUnitQueue("colonize_ship"));
                    m_Controller.Player.Towns[l_Index].setUnitTargetAmount("sea_monster", grepUnitsCombo.getUnitQueue("sea_monster"));

                    m_Controller.Player.Towns[l_Index].UnitQueueEnabled = checkBoxUnitQueueEnabledCombo.Checked;

                    //Militia
                    m_Controller.Player.Towns[m_Controller.Player.getTownIndexByID(m_SelectedTownCombo)].MilitiaTrigger = (int)numericUpDownMilitiaTriggerCombo.Value;

                    //Set population calculations
                    setQueuePopulationCalculations();

                    //Culture related
                    m_Controller.Player.Towns[l_Index].CulturalFestivalsEnabled = checkBoxCultureEnabledCombo.Checked;
                    m_Controller.Player.Towns[l_Index].CulturalPartyEnabled = grepCultureCombo.EnabledParty;
                    m_Controller.Player.Towns[l_Index].CulturalGamesEnabled = grepCultureCombo.EnabledGames;
                    m_Controller.Player.Towns[l_Index].CulturalTriumphEnabled = grepCultureCombo.EnabledTriumph;
                    m_Controller.Player.Towns[l_Index].CulturalTheaterEnabled = grepCultureCombo.EnabledTheater;

                    //Trade related
                    m_Controller.Player.Towns[l_Index].TradeEnabled = checkBoxTradeEnabledCombo.Checked;

                    if (comboBoxTradeModeCombo.SelectedItem.ToString().Equals("send") || comboBoxTradeModeCombo.SelectedItem.ToString().Equals("receive") || comboBoxTradeModeCombo.SelectedItem.ToString().Equals("spy cave"))
                        m_Controller.Player.Towns[l_Index].TradeMode = comboBoxTradeModeCombo.SelectedItem.ToString();
                    
                    m_Controller.Player.Towns[l_Index].TradeRemainingResources = (int)numericUpDownTradeRemainingResourcesCombo.Value;
                    m_Controller.Player.Towns[l_Index].TradeMinSendAmount = (int)numericUpDownTradeMinSendAmountCombo.Value;
                    m_Controller.Player.Towns[l_Index].TradePercentageWarehouse = (int)numericUpDownTradePercentageWarehouseCombo.Value;
                    m_Controller.Player.Towns[l_Index].TradeMaxDistance = (int)numericUpDownTradeMaxDistanceCombo.Value;
                    m_Controller.Player.Towns[l_Index].TradeOmitFromChangeAll = checkBoxTradeOmitFromChangeAll.Checked;

                    l_IOHandler.saveTownsSettings(m_Controller.Player);

                    loadTownGrid();

                    playSoundInfo();

                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buttonSaveQueue_Click(): " + ex.Message);
            }

        }

        private void buttonSaveCombo_Click(object sender, EventArgs e)
        {
            saveComboAll();
        }

        //~Combo tab

        #endregion

        #region GUI Events - Browser tab

        //Browser tab
        private void m_TimerCaptcha_Tick(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                m_TimerCaptcha.Stop();

                //Clipboard permission fix
                UIPermission l_ClipBoardPermission = new UIPermission(PermissionState.None);
                l_ClipBoardPermission.Clipboard = UIPermissionClipboard.AllClipboard;

                //Captcha check
                string l_Response = webBrowserGrepo.Document.Body.InnerHtml;
                bool l_CaptchaImgFound = false;

                if (l_Response.Contains("id=\"captcha_curtain\""))
                {
                    m_Controller.logEvent("Captcha detected IE9+");
                    if (!l_Response.Contains("message_new_preview"))
                    {
                        //Turn alarm off
                        if (l_Settings.CaptchaSolverEnabled)
                        {
                            m_Controller.CaptchaDetected = false;
                            m_Controller.soundCaptchaWarning();
                        }

                        IHTMLDocument2 doc = (IHTMLDocument2)webBrowserGrepo.Document.DomDocument;
                        IHTMLControlRange imgRange = (IHTMLControlRange)((HTMLBody)doc.body).createControlRange();

                        //Image l_Image;
                        //Bitmap l_Bitmap = new Bitmap(150, 50);

                        try
                        {
                            foreach (IHTMLImgElement img in doc.images)
                            {
                                imgRange.add((IHTMLControlElement)img);
                                imgRange.execCommand("Copy", false, null);
                                //TODO: change this so that img is only copied once to clipboard
                                using (Bitmap bmp = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap))
                                {
                                    if (img.nameProp != null)
                                    {
                                        if (img.nameProp.Contains("captcha"))
                                        {
                                            l_CaptchaImgFound = true;
                                            bmp.Save(@"./Captcha/" + img.nameProp.Substring(img.nameProp.Length - 9) + ".png");
                                            //l_Bitmap = new Bitmap(bmp);

                                            if (l_Settings.CaptchaSolverEnabled)
                                                m_Controller.startCaptchaSequence(bmp, "math");
                                            else
                                            {
                                                m_Controller.logEvent("Bot paused (Captcha solver disabled)");
                                                buttonTownPauseResume.Enabled = true;
                                                //Reset pause request so that user can resume the bot.
                                                m_Controller.RequestedToPauseBot = false;
                                            }
                                            break;
                                        }
                                        else if (img.nameProp.Contains("image?c="))
                                        {
                                            l_CaptchaImgFound = true;
                                            string l_RecaptchaID = "image?c=";
                                            l_RecaptchaID = img.nameProp.Substring(l_RecaptchaID.Length);
                                            l_RecaptchaID = l_RecaptchaID.Substring(0,l_RecaptchaID.IndexOf("&"));
                                            m_Controller.CaptchaReCaptchaID = l_RecaptchaID;
                                            bmp.Save(@"./Captcha/" + img.nameProp.Substring(img.nameProp.Length - 10) + ".png");//Full name is too long. Save only last 10 digits.

                                            if (l_Settings.CaptchaSolverEnabled)
                                                m_Controller.startCaptchaSequence(bmp, "text");
                                            else
                                            {
                                                m_Controller.logEvent("Bot paused (Captcha solver disabled)");
                                                buttonTownPauseResume.Enabled = true;
                                                //Reset pause request so that user can resume the bot.
                                                m_Controller.RequestedToPauseBot = false;
                                            }
                                            break;
                                        }
                                    }
                                }
                                //l_Image = new Bitmap(l_Bitmap);
                                //m_Controller.startCaptchaSequence(l_Image);
                            }
                            if (!l_CaptchaImgFound)
                            {
                                m_Controller.logEvent("Can't find captcha, retry.");
                                if (l_Settings.AdvOutputAllMode)
                                    l_IOHandler.saveServerResponseCaptcha(l_Response);
                                m_TimerCaptcha.Start();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (l_Settings.AdvDebugMode)
                            {
                                l_IOHandler.debug("Exception in m_TimerCaptcha_Tick(1): " + ex.Message);
                                l_IOHandler.debug("Can't save captcha, retry.");
                            }
                            m_Controller.logEvent("Can't save captcha, retry.");
                            webBrowserGrepo.Refresh();
                            m_TimerCaptcha.Start();
                        }
                    }
                    else
                    {
                        m_Controller.logEvent("Bot paused (Mass mail captcha detected)");
                        buttonTownPauseResume.Enabled = true;
                        //Reset pause request so that user can resume the bot.
                        m_Controller.RequestedToPauseBot = false;
                    }
                }
                else if (l_Response.Contains("id=captcha_curtain"))
                {
                    m_Controller.logEvent("Captcha detected IE8");
                    if (!l_Response.Contains("message_new_preview"))
                    {
                        //Turn alarm off
                        if (l_Settings.CaptchaSolverEnabled)
                        {
                            m_Controller.CaptchaDetected = false;
                            m_Controller.soundCaptchaWarning();
                        }

                        IHTMLDocument2 doc = (IHTMLDocument2)webBrowserGrepo.Document.DomDocument;
                        IHTMLControlRange imgRange = (IHTMLControlRange)((HTMLBody)doc.body).createControlRange();

                        //Image l_Image;
                        //Bitmap l_Bitmap = new Bitmap(150, 50);

                        try
                        {
                            foreach (IHTMLImgElement img in doc.images)
                            {
                                imgRange.add((IHTMLControlElement)img);
                                imgRange.execCommand("Copy", false, null);
                                
                                using (Bitmap bmp = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap))
                                {
                                    if (img.nameProp != null)
                                    {
                                        if (img.nameProp.Contains("captcha"))
                                        {
                                            l_CaptchaImgFound = true;
                                            bmp.Save(@"./Captcha/" + img.nameProp.Substring(img.nameProp.Length - 9) + ".png");
                                            //l_Bitmap = new Bitmap(bmp);

                                            if (l_Settings.CaptchaSolverEnabled)
                                                m_Controller.startCaptchaSequence(bmp, "math");
                                            else
                                            {
                                                m_Controller.logEvent("Bot paused (Captcha solver disabled)");
                                                buttonTownPauseResume.Enabled = true;
                                                //Reset pause request so that user can resume the bot.
                                                m_Controller.RequestedToPauseBot = false;
                                            }
                                            break;
                                        }
                                        else if (img.nameProp.Contains("image?c="))
                                        {
                                            l_CaptchaImgFound = true;
                                            string l_RecaptchaID = "image?c=";
                                            l_RecaptchaID = img.nameProp.Substring(l_RecaptchaID.Length);
                                            l_RecaptchaID = l_RecaptchaID.Substring(0, l_RecaptchaID.IndexOf("&"));
                                            m_Controller.CaptchaReCaptchaID = l_RecaptchaID;
                                            bmp.Save(@"./Captcha/" + img.nameProp.Substring(img.nameProp.Length - 10) + ".png");//Full name is too long. Save only last 10 digits.

                                            if (l_Settings.CaptchaSolverEnabled)
                                                m_Controller.startCaptchaSequence(bmp, "text");
                                            else
                                            {
                                                m_Controller.logEvent("Bot paused (Captcha solver disabled)");
                                                buttonTownPauseResume.Enabled = true;
                                                //Reset pause request so that user can resume the bot.
                                                m_Controller.RequestedToPauseBot = false;
                                            }
                                            break;
                                        }
                                    }
                                }
                                //l_Image = new Bitmap(l_Bitmap);
                                //m_Controller.startCaptchaSequence(l_Image);
                            }
                            if (!l_CaptchaImgFound)
                            {
                                m_Controller.logEvent("Can't find captcha, retry.");
                                if (l_Settings.AdvOutputAllMode)
                                    l_IOHandler.saveServerResponseCaptcha(l_Response);
                                m_TimerCaptcha.Start();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (l_Settings.AdvDebugMode)
                            {
                                l_IOHandler.debug("Exception in m_TimerCaptcha_Tick(2): " + ex.Message);
                                l_IOHandler.debug("Can't save captcha, retry.");
                            }
                            m_Controller.logEvent("Can't save captcha, retry.");
                            webBrowserGrepo.Refresh();
                            m_TimerCaptcha.Start();
                        }
                    }
                    else
                    {
                        m_Controller.logEvent("Bot paused (Mass mail captcha detected)");
                        buttonTownPauseResume.Enabled = true;
                    }
                }
                else
                {
                    TimeSpan l_TimeWaited = DateTime.Now - m_Controller.CaptchaDetectedTime;
                    m_Controller.logEvent("Waiting for captcha: " + l_TimeWaited.ToString());
                    if (l_TimeWaited.Minutes.Equals(10))
                        webBrowserGrepo.Refresh();
                    m_TimerCaptcha.Start();
                }
            }
            catch (Exception ex2)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in m_TimerCaptcha_Tick(3): " + ex2.Message);
                    m_Controller.logEvent("Can't save captcha, retry.");
                }
                m_Controller.logEvent("Can't save captcha, retry.");
                //Note: Don't add webBrowserGrepo.Refresh() here. Should work fine the next time m_TimerCaptcha_Tick() is called.
                m_TimerCaptcha.Start();
            }
        }

        private void m_TimerCaptchaAnswered_Tick(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                bool l_RequestRefund = false;

                m_TimerCaptchaAnswered.Stop();

                string l_Response = webBrowserGrepo.Document.Body.InnerHtml;
                if (l_Response.Contains("id=\"captcha_curtain\"") || l_Response.Contains("id=captcha_curtain"))
                {
                    m_Controller.logEvent("Captcha answer was not correct.");
                    l_RequestRefund = true;
                }
                else
                {
                    m_Controller.logEvent("Captcha answer was correct.");
                    l_RequestRefund = false;
                }

                m_Controller.CaptchaAnswerCorrect = !l_RequestRefund;
                m_Controller.captchaCaptchaCorrect();
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in m_TimerCaptchaAnswered_Tick(): " + ex.Message);
                }
            }
        }

        private void m_TimerLoginp1_Tick(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            m_TimerLoginp1.Stop();
            loginP2();
        }

        private void m_TimerLoginp2_Tick(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            m_TimerLoginp2.Stop();
            loginP3();
        }

        private void m_TimerLoginp3_Tick(object sender, EventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                string l_Response = webBrowserGrepo.DocumentText;

                m_TimerLoginp3.Stop();
                if (!l_Response.Contains("csrfToken\":\""))
                {
                    if (m_Controller.LoginVerified)
                        m_Controller.startReconnect(3);
                    else
                        setStatusBar("Can't connect, check your settings");
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in m_TimerLoginp3_Tick(): " + ex.Message);
                }
            }
        }

        private void webBrowserGrepo_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;
            string l_Response = "";

            string l_Search = "";
            int l_Index = -1;

            try
            {
                //m_Controller.logEvent("webBrowserGrepo_DocumentCompleted: " + m_Controller.State);

                l_Response = webBrowserGrepo.DocumentText;

                if (getBrowserLookFor != null)
                {
                    if (l_Response.IndexOf(getBrowserLookFor, 0) > 0) getBrowserComplete = true;
                }
    
                if (l_Settings.AdvOutputAllMode)
                    l_IOHandler.saveServerResponse2(m_Controller.State + "(browser)", l_Response);

                if (m_Controller.State.Equals("loginp1"))
                {
                    m_TimerLoginp1.Start();
                }
                /*else if (m_Controller.State.Equals("loginp2"))
                {
                    m_TimerLoginp2.Start();
                }*/
                else if (m_Controller.State.Equals("loginp3"))
                {
                    if (l_Response.Contains("csrfToken\":\""))
                    {
                        //m_Controller.setCookies(webBrowserGrepo.Document.Cookie);
                        m_Controller.setCookies(webBrowserGrepo);

                          m_Controller.LoggedIn = true;
                        m_Controller.LoginVerified = true;

                        //Get csrfToken
                        l_Search = "csrfToken\":\"";
                        l_Index = l_Response.IndexOf(l_Search, 0);
                        m_Controller.H = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //Get locale time offset
                        l_Search = "locale_gmt_offset\":";
                        l_Index = l_Response.IndexOf(l_Search, l_Index);
                        l_Settings.LocaleTimeOffset = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //Get player id
                        l_Search = "player_id\":";
                        l_Index = l_Response.IndexOf(l_Search, l_Index);
                        m_Controller.Player.PlayerID = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //Get server time offset
                        l_Search = "server_gmt_offset\":";
                        l_Index = l_Response.IndexOf(l_Search, l_Index);
                        l_Settings.ServerTimeOffset = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //Get server time
                        l_Search = "server_time\":";
                        l_Index = l_Response.IndexOf(l_Search, l_Index);
                        m_Controller.ServerTime = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //Get town id
                        l_Search = "townId\":";
                        l_Index = l_Response.IndexOf(l_Search, l_Index);
                        m_Controller.Player.DefaultTownID = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                        //Premium status
                        string l_PremiumStatus = "";
                        l_Search = "\"premium_features\":{";
                        l_Index = l_Response.IndexOf(l_Search, 0);
                        if (l_Index != -1)
                        {
                            l_PremiumStatus = l_Response.Substring(l_Index, l_Response.IndexOf("}", l_Index) - l_Index);
                            l_Search = "\"commander\":";
                            l_Index = l_PremiumStatus.IndexOf(l_Search,0);
                            string l_PremiumTime = l_PremiumStatus.Substring(l_Index + l_Search.Length, l_PremiumStatus.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            if(l_PremiumTime.Equals("null"))
                                m_Controller.Player.CommanderActive = "0";
                            else
                                m_Controller.Player.CommanderActive = l_PremiumTime;
                            
                            l_Search = "\"curator\":";
                            l_Index = l_PremiumStatus.IndexOf(l_Search, 0);
                            l_PremiumTime = l_PremiumStatus.Substring(l_Index + l_Search.Length, l_PremiumStatus.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            if (l_PremiumTime.Equals("null"))
                                m_Controller.Player.CuratorActive = "0";
                            else
                                m_Controller.Player.CuratorActive = l_PremiumTime;
                            
                            l_Search = "\"captain\":";
                            l_Index = l_PremiumStatus.IndexOf(l_Search, 0);
                            l_PremiumTime = l_PremiumStatus.Substring(l_Index + l_Search.Length, l_PremiumStatus.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            if (l_PremiumTime.Equals("null"))
                                m_Controller.Player.CaptainActive = "0";
                            else
                                m_Controller.Player.CaptainActive = l_PremiumTime;

                            l_Search = "\"priest\":";
                            l_Index = l_PremiumStatus.IndexOf(l_Search, 0);
                            l_PremiumTime = l_PremiumStatus.Substring(l_Index + l_Search.Length, l_PremiumStatus.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            if (l_PremiumTime.Equals("null"))
                                m_Controller.Player.PriestActive = "0";
                            else
                                m_Controller.Player.PriestActive = l_PremiumTime;

                            l_Search = "\"trader\":";
                            l_Index = l_PremiumStatus.IndexOf(l_Search, 0);
                            l_PremiumTime = l_PremiumStatus.Substring(l_Index + l_Search.Length);
                            if (l_PremiumTime.Equals("null"))
                                m_Controller.Player.TraderActive = "0";
                            else
                                m_Controller.Player.TraderActive = l_PremiumTime;
                        }

                        //Finish part one login sequence, need to gather town data now.
                        m_Controller.State = "loggedinp1";
                        //m_Controller.getTownList();
                        m_Controller.updateGameData();
                    }
                    else
                    {
                        m_TimerLoginp3.Start();
                    }
                }
                else
                {
                    m_Controller.setCookies(webBrowserGrepo);
                }

                //Fix for .NET browser memory leak
                IntPtr pHandle = GetCurrentProcess();
                SetProcessWorkingSetSize(pHandle, -1, -1);
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in webBrowserGrepo_DocumentCompleted(): " + ex.Message);
                    l_IOHandler.saveServerResponse("webBrowserGrepo_DocumentCompleted", ex.Message + "\n" + l_Response);
                }
            }
        }
        //~Browser tab

        #endregion
        
        #region GUI Events - Main Form

        //Main Form
        private void m_NotifyIcon_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void GrepolisBotII_Resize(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;

            if (this.WindowState == FormWindowState.Minimized && l_Settings.GenToTryIcon == true)
            {
                this.ShowInTaskbar = false;
            }
        }

        private void GrepolisBot2_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_NotifyIcon.Visible = false;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Settings l_Settings = Settings.Instance;

                switch (tabControl1.SelectedIndex)
                {
                    //Updating selected tab to make sure settings are the same on all tabs.
                    //e.g when making changes on the combo tab and switching back to the farmer tab.
                    case 2: // grid overview
                        loadGridOverviewImages();
                        break;
                    case 3:
                        if (comboBoxTownListQueue.SelectedIndex < 0 && comboBoxTownListQueue.Items.Count > 0)
                            comboBoxTownListQueue.SelectedIndex = 0;
                        updateQueueGUI();
                        break;
                    case 4:
                        if (comboBoxTownListCulture.SelectedIndex < 0 && comboBoxTownListCulture.Items.Count > 0)
                            comboBoxTownListCulture.SelectedIndex = 0;
                        updateCulturalGUI();
                        break;
                    case 5:
                        if (comboBoxTownListTrade.SelectedIndex < 0 && comboBoxTownListTrade.Items.Count > 0)
                            comboBoxTownListTrade.SelectedIndex = 0;
                        updateTradeGUI();
                        break;
                    case 6:
                        if (comboBoxTownListFarmers.SelectedIndex < 0 && comboBoxTownListFarmers.Items.Count > 0)
                            comboBoxTownListFarmers.SelectedIndex = 0;
                        updateFarmersGUI();
                        break;
                    case 7: // combo 
                        if (comboBoxTownListCombo.SelectedIndex < 0 && comboBoxTownListCombo.Items.Count > 0)
                            comboBoxTownListCombo.SelectedIndex = 0;
                        textBox1.Focus();
                        updateComboGUI();
                        loadTownGrid();
                        break;
                    case 11: //browser

                        //Auto pause bot when switching to browser tab. With m_Controller.CanIModifyFarmers this function becomes active after the first completed cycle.
                        if (l_Settings.AdvAutoPause && m_Controller.CanIModifyFarmers)
                        {
                            if (buttonTownPauseResume.Text.Equals("Pause"))
                            {
                                m_Controller.pause();
                                buttonTownPauseResume.Text = "Resume";
                            }
                        }
                        break;
                } // switch
            }
            catch(Exception)
            {

            }
        }
        //~Form

        #endregion

        #region Class Events - Controller
        //Controller class
        private void m_Controller_statusBarUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ParameterizedThreadStart(setStatusBarCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_timeoutProcessedStateChanged(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(setGuiToTimeoutProcessedStateCrossThread)).Start();
        }

        private void m_Controller_tradeProcessedStateChanged(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(setToTradeProcessedStateCrossThread)).Start();
        }

        private void m_Controller_townProcessedStateChanged(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(setToTownProcessedStateCrossThread)).Start();
        }

        private void m_Controller_refreshTimerUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ParameterizedThreadStart(updateRefreshTimerCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_queueTimerUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ParameterizedThreadStart(updateQueueTimerCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_tradeTimerUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ParameterizedThreadStart(updateTradeTimerCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_reconnectTimerUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ParameterizedThreadStart(updateReconnectTimerCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_forcedReconnectTimerUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ParameterizedThreadStart(updateForcedReconnectTimerCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_connectedTimerUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ParameterizedThreadStart(updateConnectedTimerCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_logUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ParameterizedThreadStart(logCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_versionInfoUpdated(object sender, CustomArgs ca)
        {
            new Thread(new ParameterizedThreadStart(versionInfoCrossThread)).Start(ca.getMessage());
        }

        private void m_Controller_serverRequestDelayRequested(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(serverRequestDelayRequestCrossThread)).Start();
        }

        private void m_Controller_serverRequestCaptchaDelayRequested(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(serverRequestCaptchaDelayRequestCrossThread)).Start();
        }

        private void m_Controller_pauseBotRequestCaptcha(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(pauseBotRequestCaptchaCrossThread)).Start();
        }

        private void m_Controller_startCaptchaCheckTimerRequest(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(startCaptchaCheckTimerRequestCrossThread)).Start();
        }

        private void m_Controller_captchaCheckPreCycle(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(captchaCheckPreCycleCrossThread)).Start();
        }

        private void m_Controller_captchaAnswerReady(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(captchaAnswerReadyCrossThread)).Start();
        }

        private void m_Controller_captchaAnswerModerated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(captchaAnswerModeratedCrossThread)).Start();
        }

        private void m_Controller_captchaAnswerSendToGrepolisCorrect(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(captchaAnswerSendToGrepolisCorrectCrossThread)).Start();
        }

        private void m_Controller_captchaAnswerSendToGrepolisInCorrect(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(captchaAnswerSendToGrepolisInCorrectCrossThread)).Start();
        }

        private void m_Controller_captchaSolver9kwDown(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(captchaSolver9kwDownCrossThread)).Start();
        }

        private void m_Controller_townListUpdated(object sender, CustomArgs ca)
        {
            //Create new thread
            new Thread(new ThreadStart(setGuiToLoggedInStateCrossThread)).Start();
        }

        //~Controller class
        #endregion
    }
}
