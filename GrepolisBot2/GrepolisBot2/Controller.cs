using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Media;
using System.Drawing;

namespace GrepolisBot2
{
    class Controller
    {
        //General
        private bool m_LoggedIn = false;
        private bool m_LoginVerified = false;
        private bool m_IsBotRunning = false;
        private bool m_CanIModifyFarmers = false;
        private bool m_TimeoutOnQueueTimer = false;
        private bool m_TimeoutOnTradeTimer = false;
        private bool m_RequestedToPauseBot = false;
        private bool m_RequestingServerData = false;
        private bool m_TownsSorted = false;
        private bool m_TownListInitialized = false;
        private bool m_IgnoreScheduler = true;
        private int m_ReconnectCount = 0;
        private string m_BuildingTearDown = "";

        private string m_State = "";
        private int m_RetryCount = 0;
        private int m_RetryCountServerError = 0;
        private int m_CurrentTownIntern = 0;
        private int m_CurrentFarmIntern = 0;
        private int m_CurrentTownTradeIntern = 0;//Normal index
        private int m_CurrentTownTradeIntern2 = 0;//Sorted index by distance
        private string m_TownsSortedByDistance = "";
        private int m_StateManagerMode = 0;
        private int m_StateManagerCount = 2;
        private bool m_AddedExtraBuildingInNonTargetQueueMode = false;
        private bool m_CulturalFestivalStarted = false;
        private string m_EmailMessage = "";

        private bool m_CaptchaDetected = false;
        private bool m_CaptchaDetectedPrecycle = false;
        private Image m_CaptchaImage;
        private string m_CaptchaID = "";
        private string m_CaptchaAnswer = "";
        private bool m_CaptchaAnswerCorrect = true;
        private int m_CaptchaAnswerInCorrectCount = 0;
        private int m_CaptchaRetryCounter = 0;
        private string m_CaptchaType = "";//math, text
        private string m_CaptchaReCaptchaID = "";
        private int m_CaptchaCurrentWorkers = -1;
        private int m_CaptchaCurrentQueue = -1;
        private bool m_CaptchaMassMailDetected = false;
        private DateTime m_CaptchaDetectedTime = new DateTime();

        private string m_H = "";//csrfToken
        private string m_Nlreq_id = "0";//Currently only used for notifications, in the past for http requests as well. Circa 2.82 replaced by "nl_init":true for all server requests.
        private string m_ServerTime = "";
        private string m_DisconnectedString = "/start?action=login";

        private SoundPlayer m_SoundPlayer = new SoundPlayer();
        private SoundPlayer m_SoundPlayerCaptcha = new SoundPlayer();

        private Timer m_RequestTimer = new Timer();
        private Timer m_RequestTimerCaptcha = new Timer();
        private Timer m_TimeoutTimer = new Timer();

        //Custom classes
        private HttpHandler m_HttpHandler = new HttpHandler();
        private HttpHandler m_HttpHandlerCaptcha = new HttpHandler();
        private HttpHandler m_HttpHandlerCheckVersion = new HttpHandler();
        private MailClient m_MailClient = new MailClient();
        private Player m_Player = new Player();
        private jdTimer m_RefreshTimer = new jdTimer();
        private jdTimer m_QueueTimer = new jdTimer();
        private jdTimer m_TradeTimer = new jdTimer();
        private jdTimer m_ReconnectTimer = new jdTimer();
        private jdTimer m_ForcedReconnectTimer = new jdTimer();
        private jdTimer m_ConnectedTimer = new jdTimer();

        //Events
        public delegate void SetStatusBarHandler(object sender, CustomArgs ca);
        public event SetStatusBarHandler statusBarUpdated;
        public delegate void SetGuiToTimeoutProcessedStateHandler(object sender, CustomArgs ca);
        public event SetGuiToTimeoutProcessedStateHandler timeoutProcessedStateChanged;
        public delegate void SetToTradeProcessedStateHandler(object sender, CustomArgs ca);
        public event SetToTradeProcessedStateHandler tradeProcessedStateChanged;
        public delegate void SetToTownProcessedStateHandler(object sender, CustomArgs ca);
        public event SetToTownProcessedStateHandler townProcessedStateChanged;
        public delegate void updateRefreshTimerHandler(object sender, CustomArgs ca);
        public event updateRefreshTimerHandler refreshTimerUpdated;
        public delegate void updateQueueTimerHandler(object sender, CustomArgs ca);
        public event updateQueueTimerHandler queueTimerUpdated;
        public delegate void updateTradeTimerHandler(object sender, CustomArgs ca);
        public event updateTradeTimerHandler tradeTimerUpdated;
        public delegate void updateReconnectTimerHandler(object sender, CustomArgs ca);
        public event updateReconnectTimerHandler reconnectTimerUpdated;
        public delegate void updateForcedReconnectTimerHandler(object sender, CustomArgs ca);
        public event updateForcedReconnectTimerHandler forcedReconnectTimerUpdated;
        public delegate void updateConnectedTimerHandler(object sender, CustomArgs ca);
        public event updateConnectedTimerHandler connectedTimerUpdated;
        public delegate void versionInfoHandler(object sender, CustomArgs ca);
        public event versionInfoHandler versionInfoUpdated;
        public delegate void logHandler(object sender, CustomArgs ca);
        public event logHandler logUpdated;
        public delegate void serverRequestDelayRequestHandler(object sender, CustomArgs ca);
        public event serverRequestDelayRequestHandler serverRequestDelayRequested;
        public delegate void serverRequestCaptchaDelayHandler(object sender, CustomArgs ca);
        public event serverRequestCaptchaDelayHandler serverRequestCaptchaDelayRequested;
        public delegate void pauseBotRequestCaptchaHandler(object sender, CustomArgs ca);
        public event pauseBotRequestCaptchaHandler pauseBotRequestCaptcha;
        public delegate void startCaptchaCheckTimerRequestHandler(object sender, CustomArgs ca);
        public event startCaptchaCheckTimerRequestHandler startCaptchaCheckTimerRequest;
        public delegate void captchaCheckPreCycleHandler(object sender, CustomArgs ca);
        public event captchaCheckPreCycleHandler captchaCheckPreCycle;
        public delegate void captchaAnswerReadyHandler(object sender, CustomArgs ca);
        public event captchaAnswerReadyHandler captchaAnswerReady;
        public delegate void captchaAnswerModeratedHandler(object sender, CustomArgs ca);
        public event captchaAnswerModeratedHandler captchaAnswerModerated;
        public delegate void captchaAnswerSendToGrepolisCorrectHandler(object sender, CustomArgs ca);
        public event captchaAnswerSendToGrepolisCorrectHandler captchaAnswerSendToGrepolisCorrect;
        public delegate void captchaAnswerSendToGrepolisInCorrectHandler(object sender, CustomArgs ca);
        public event captchaAnswerSendToGrepolisInCorrectHandler captchaAnswerSendToGrepolisInCorrect;
        public delegate void captchaSolver9kwDownHandler(object sender, CustomArgs ca);
        public event captchaSolver9kwDownHandler captchaSolver9kwDown;
        public delegate void townListUpdatedHandler(object sender, CustomArgs ca);
        public event townListUpdatedHandler townListUpdated;

        
//-->Constructor

        public Controller()
        {
            initEventHandlers();
            initTimers();
            initHttpHandlers();
            initSoundPlayer();
        }

//-->Attributes

        #region Attributes

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

        //General
        public bool LoggedIn
        {
            get { return m_LoggedIn; }
            set { m_LoggedIn = value; }
        }

        public bool LoginVerified
        {
            get { return m_LoginVerified; }
            set { m_LoginVerified = value; }
        }

        public bool TimeoutOnTradeTimer
        {
            get { return m_TimeoutOnTradeTimer; }
            set { m_TimeoutOnTradeTimer = value; }
        }

        public bool CanIModifyFarmers
        {
            get { return m_CanIModifyFarmers; }
            set { m_CanIModifyFarmers = value; }
        }
        
        public bool IsBotRunning
        {
            get { return m_IsBotRunning; }
            set { m_IsBotRunning = value; }
        }

        public bool RequestedToPauseBot
        {
            get { return m_RequestedToPauseBot; }
            set { m_RequestedToPauseBot = value; }
        }

        public bool RequestingServerData
        {
            get { return m_RequestingServerData; }
            set { m_RequestingServerData = value; }
        }

        public bool TownsSorted
        {
            get { return m_TownsSorted; }
            set { m_TownsSorted = value; }
        }

        public bool TownListInitialized
        {
            get { return m_TownListInitialized; }
            set { m_TownListInitialized = value; }
        }

        public bool IgnoreScheduler
        {
            get { return m_IgnoreScheduler; }
            set { m_IgnoreScheduler = value; }
        }

        public int ReconnectCount
        {
            get { return m_ReconnectCount; }
            set { m_ReconnectCount = value; }
        }

        public bool CaptchaDetected //Set to true when captcha is detected in notifications.
        {
            get { return m_CaptchaDetected; }
            set { m_CaptchaDetected = value; }
        }

        public bool CaptchaDetectedPrecycle //Set to true when captcha is detected in precycle.
        {
            get { return m_CaptchaDetectedPrecycle; }
            set { m_CaptchaDetectedPrecycle = value; }
        }

        public string CaptchaAnswer
        {
            get { return m_CaptchaAnswer; }
            set { m_CaptchaAnswer = value; }
        }

        public bool CaptchaAnswerCorrect
        {
            get { return m_CaptchaAnswerCorrect; }
            set { m_CaptchaAnswerCorrect = value; }
        }

        public int CaptchaAnswerInCorrectCount
        {
            get { return m_CaptchaAnswerInCorrectCount; }
            set { m_CaptchaAnswerInCorrectCount = value; }
        }

        public string CaptchaReCaptchaID
        {
            get { return m_CaptchaReCaptchaID; }
            set { m_CaptchaReCaptchaID = value; }
        }

        public int CaptchaCurrentWorkers
        {
            get { return m_CaptchaCurrentWorkers; }
            set { m_CaptchaCurrentWorkers = value; }
        }

        public int CaptchaCurrentQueue
        {
            get { return m_CaptchaCurrentQueue; }
            set { m_CaptchaCurrentQueue = value; }
        }

        public bool CaptchaMassMailDetected
        {
            get { return m_CaptchaMassMailDetected; }
            set { m_CaptchaMassMailDetected = value; }
        }

        public DateTime CaptchaDetectedTime
        {
            get { return m_CaptchaDetectedTime; }
            set { m_CaptchaDetectedTime = value; }
        }

        public string H
        {
            get { return m_H; }
            set { m_H = value; }
        }

        public string Nlreq_id
        {
            get { return m_Nlreq_id; }
            set { m_Nlreq_id = value; }
        }

        public string ServerTime
        {
            get { return m_ServerTime; }
            set 
            {
                if (value.Length > 10)
                    m_ServerTime = value.Substring(0, 10);
                else
                    m_ServerTime = value;
            }
        }

        public string State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        public Timer RequestTimer
        {
            get { return m_RequestTimer; }
            set { m_RequestTimer = value; }
        }

        public Timer RequestTimerCaptcha
        {
            get { return m_RequestTimerCaptcha; }
            set { m_RequestTimerCaptcha = value; }
        }

        //Custom classes
        public Player Player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }

        public jdTimer ReconnectTimer
        {
            get { return m_ReconnectTimer; }
            set { m_ReconnectTimer = value; }
        }

        public MailClient MailClient
        {
            get { return m_MailClient; }
            set { m_MailClient = value; }
        }

        #endregion

        //-->Methods

        #region Init

        private void initEventHandlers()
        {
            m_HttpHandler.UploadValuesCompleted += new System.Net.UploadValuesCompletedEventHandler(m_HttpHandler_UploadValuesCompleted);
            m_HttpHandler.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(m_HttpHandler_DownloadStringCompleted);
            m_HttpHandlerCaptcha.DownloadStringCompleted +=new DownloadStringCompletedEventHandler(m_HttpHandlerCaptcha_DownloadStringCompleted);
            m_HttpHandlerCaptcha.UploadValuesCompleted += new UploadValuesCompletedEventHandler(m_HttpHandlerCaptcha_UploadValuesCompleted);
            m_HttpHandlerCheckVersion.DownloadStringCompleted += new DownloadStringCompletedEventHandler(m_HttpHandlerCheckVersion_DownloadStringCompleted);
            m_RefreshTimer.InternalTimer.Elapsed += new System.Timers.ElapsedEventHandler(RefreshTimer_InternalTimer_Elapsed);
            m_QueueTimer.InternalTimer.Elapsed += new System.Timers.ElapsedEventHandler(QueueTimer_InternalTimer_Elapsed);
            m_TradeTimer.InternalTimer.Elapsed += new System.Timers.ElapsedEventHandler(TradeTimer_InternalTimer_Elapsed);
            m_ReconnectTimer.InternalTimer.Elapsed += new System.Timers.ElapsedEventHandler(ReconnectTimer_InternalTimer_Elapsed);
            m_ForcedReconnectTimer.InternalTimer.Elapsed += new System.Timers.ElapsedEventHandler(ForcedReconnectTimer_InternalTimer_Elapsed);
            m_ConnectedTimer.InternalTimer.Elapsed +=new System.Timers.ElapsedEventHandler(ConnectedTimer_InternalTimer_Elapsed);
            m_RequestTimer.Tick += new EventHandler(m_RequestTimer_Tick);
            m_RequestTimerCaptcha.Tick += new EventHandler(m_RequestTimerCaptcha_Tick);
            m_TimeoutTimer.Tick += new EventHandler(m_TimeoutTimer_Tick);
        }

        public void initTimers()
        {
            Settings l_Settings = Settings.Instance;

            m_RefreshTimer.Duration = l_Settings.AdvMinTimerRefresh;
            m_QueueTimer.Duration = l_Settings.QueueTimer;
            m_TradeTimer.Duration = l_Settings.TradeTimer;
            m_ReconnectTimer.Duration = l_Settings.RecTimerReconnectMin;
            m_ForcedReconnectTimer.Duration = l_Settings.RecMinForcedReconnect * 60;
            m_ConnectedTimer.Duration = 0;
            m_RequestTimer.Interval = 100;
            m_RequestTimerCaptcha.Interval = 500;
            m_TimeoutTimer.Interval = l_Settings.AdvTimeout * 1000;
        }

        private void initHttpHandlers()
        {
            m_HttpHandlerCheckVersion.Headers.Add("Referer", AssemblyTitle + " " + AssemblyVersion);
        }

        private void initSoundPlayer()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                m_SoundPlayer.SoundLocation = l_Settings.SoundAttackWarningSoundLocation;
                m_SoundPlayerCaptcha.SoundLocation = l_Settings.SoundCaptchaWarningSoundLocation;
                //m_SoundPlayer.LoadAsync();
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in initSoundPlayer(): " + e.Message);
            }
        }

        #endregion

        #region Webrequests - Bot related

        public void checkVersion()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                string l_Url = "http://bots.uthar.nl/files/grepolis2latestversion.txt";
                Uri l_Uri = new Uri(l_Url);
                m_HttpHandlerCheckVersion.DownloadStringAsync(l_Uri);
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkVersion(): " + e.Message);
            }
        }

        #endregion

        public void setCookies(WebBrowser p_WebBrowser)
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                String l_HostName = p_WebBrowser.Url.Scheme + Uri.SchemeDelimiter + p_WebBrowser.Url.Host;
                Uri l_HostUri = new Uri(l_HostName);
                CookieContainer l_Container = CookieHelpers.GetUriCookieContainer(l_HostUri);
                CookieCollection l_CookieCollection = l_Container.GetCookies(l_HostUri);
                m_HttpHandler.CookieContainer.Add(l_CookieCollection);

                //Save cookies to file
                string l_Cookies = "";
                for (int i = 0; i < l_CookieCollection.Count; i++)
                {
                    l_Cookies += l_CookieCollection[i].Name + "=" + l_CookieCollection[i].Value + ";";
                }
                l_IOHandler.writeCookies(l_Cookies);
            }
            catch (Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in setCookies(): " + e.Message);
            }
        }

        /*
         * Start bot using the start button on the GUI
         * First run uses modified timers to be able to start all features during the first run
         */
        public void startbotFirstRun()
        {
            Settings l_Settings = Settings.Instance;

            if (m_LoggedIn)
            {
                m_IsBotRunning = true;//Set to true again when there was a reconnect needed

                //m_QueueTimer.start(randomizeTimer(l_Settings.QueueTimer));
                //m_TradeTimer.start(randomizeTimer(l_Settings.TradeTimer));
                m_QueueTimer.start(0.01);
                m_TradeTimer.start(0.01);
                m_TimeoutOnQueueTimer = false;
                m_TimeoutOnTradeTimer = false;

                m_RefreshTimer.start(0.1);//Call as last to prevent timer issues
            }
        }

        /*
         * Start bot using the start button on the GUI
         */
        public void startbot()
        {
            Settings l_Settings = Settings.Instance;

            if (m_LoggedIn)
            {
                Random l_Random = new Random();
                int l_Interval = l_Random.Next(l_Settings.AdvMinTimerRefresh, l_Settings.AdvMaxTimerRefresh);

                m_IsBotRunning = true;//Set to true again when there was a reconnect needed
                m_RefreshTimer.start(randomizeTimer(l_Interval));

                m_QueueTimer.start(randomizeTimer(l_Settings.QueueTimer));
                m_TradeTimer.start(randomizeTimer(l_Settings.TradeTimer));
                m_TimeoutOnQueueTimer = false;
                m_TimeoutOnTradeTimer = false;
            }
        }

        /*
         * Pause bot
         */
        public void pause()
        {
            if (!m_RequestingServerData || m_CaptchaDetectedPrecycle)
            {
                m_RefreshTimer.stop();
                m_QueueTimer.stop();
                m_TradeTimer.stop();
                m_IsBotRunning = false;
                m_RequestingServerData = false;
                m_CaptchaDetectedPrecycle = false;
            }
            else
            {
                m_RequestedToPauseBot = true;
            }
        }

        /**
         * This method is called after setGuiToTimeoutProcessedState()
         * It starts all the timers again after the bot has completed an cycle
         * 
         * Note: When trade timer is ready the trade feature will be started first.
         *       After that the timers are resumed.
         */
        public void resumeTimers()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (m_LoggedIn)
                {
                    m_IsBotRunning = true;//Set to true again when there was a reconnect needed
                    if (!m_ForcedReconnectTimer.isTimerDone() || !l_Settings.RecForcedReconnects)
                    {
                        //add new line to log
                        logEvent("");

                        Random l_Random = new Random();
                        int l_Interval = l_Random.Next(l_Settings.AdvMinTimerRefresh, l_Settings.AdvMaxTimerRefresh);
                        m_RefreshTimer.start(randomizeTimer(l_Interval));

                        //Start or restart queue timer again
                        if (m_TimeoutOnQueueTimer)
                        {
                            m_QueueTimer.start(randomizeTimer(l_Settings.QueueTimer));
                            m_TimeoutOnQueueTimer = false;
                        }
                        else
                            m_QueueTimer.resume();
                        //Start or restart trade timer again
                        if (m_TimeoutOnTradeTimer)
                        {
                            m_TradeTimer.start(randomizeTimer(l_Settings.TradeTimer));
                            m_TimeoutOnTradeTimer = false;
                        }
                        else
                            m_TradeTimer.resume();
                    }
                    else
                    {
                        //Set new forced reconnect timer to prevent multiple fast connection sequences
                        startForcedReconnectTimer();
                        m_ForcedReconnectTimer.stop();

                        startReconnectFast();
                    }
                }
            }
            catch(Exception e)
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in resumeTimers(): " + e.Message);
            }
        }

        /**
         * This method is called after setGuiToTimeoutProcessedState()
         * Only when the trade timer is ready (00:00:00)
         */
        public void startTradeSequence()
        {
            //Reset hasEnoughResources check
            for (int i = 0; i < m_Player.Towns.Count; i++)
            {
                m_Player.Towns[i].HasEnoughResources = false;
            }

            logEvent("");
            logEvent("Start trading.");
            m_RequestingServerData = true;
            m_CurrentTownTradeIntern = 0;//Used to cycle through all towns. Next index handled by checkTradeNext()
            m_CurrentTownTradeIntern2 = 0;//Used to cycle through all towns in reveiver mode
            m_TownsSortedByDistance = m_Player.getTownsSortedByDistance(m_CurrentTownTradeIntern);
            m_TimeoutTimer.Start();
            checkTrade();
        }

        /**
         * This method is called when a captcha message is detected
         * 9kw.eu is used as captcha solve service
         */
        public void startCaptchaSequence(Image p_Image, string p_Type)
        {
            //m_RequestingServerData not used in this sequence
            //m_TimeoutTimer not used in the sequence
            m_CaptchaImage = new Bitmap(p_Image);
            m_CaptchaID = "";
            m_CaptchaAnswer = "";
            m_CaptchaType = p_Type;
            captcha9kwServiceStatus();
        }

        public void startConnectedTimer()
        {
            m_ConnectedTimer.start(0);
        }

        public void startForcedReconnectTimer()
        {
            Settings l_Settings = Settings.Instance;

            Random l_Random = new Random();
            m_ForcedReconnectTimer.start(l_Random.Next(l_Settings.RecMinForcedReconnect*60, l_Settings.RecMaxForcedReconnect*60));
        }

        /*
         * Starts reconnect sequence
         */
        public void startReconnect(int p_Reason)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            switch (p_Reason)
            {
                case 0:
                {
                    l_IOHandler.debug("Connection to server lost. Starting reconnect...(Request)");
                    break;
                }
                case 1:
                {
                    l_IOHandler.debug("Connection to server lost. Starting reconnect...(Parse)");
                    break;
                }
                case 2:
                {
                    l_IOHandler.debug("Connection to server lost. Starting reconnect...(Response)");
                    break;
                }
                case 3:
                {
                    l_IOHandler.debug("Connection to server lost. Starting reconnect...(Login)");
                    break;
                }
                default:
                {
                    l_IOHandler.debug("Connection to server lost. Starting reconnect...");
                    break;
                }
            }

            //Cancels active request
            if (m_HttpHandler.IsBusy)
            {
                m_HttpHandler.CancelAsync();
                l_IOHandler.debug("Active request canceled. (startReconnect)");
            }

            m_State = "reconnect";
            m_LoggedIn = false;
            m_IsBotRunning = false;
            m_TimeoutOnQueueTimer = false;
            m_RequestedToPauseBot = false;
            m_RequestingServerData = false;
            m_TownListInitialized = false;
            m_CurrentTownIntern = 0;
            m_CurrentFarmIntern = 0;
            m_TimeoutTimer.Stop();
            m_RefreshTimer.stop();
            m_QueueTimer.stop();
            m_TradeTimer.stop();
            m_ConnectedTimer.stop();
            m_ForcedReconnectTimer.stop();
            m_HttpHandler.clearCookies();
            setStatusBarEvent("Waiting for reconnect");

            m_ReconnectCount++;
            if (m_ReconnectCount > l_Settings.RecMaxReconnects && l_Settings.RecMaxReconnectsEnabled)
            {
                setStatusBarEvent("Bot stopped. Too many errors occured.");
            }
            else
            {
                Random l_Random = new Random();
                m_ReconnectTimer.start(l_Random.Next(l_Settings.RecTimerReconnectMin, l_Settings.RecTimerReconnectMax));
            }
        }

        /*
         * Starts reconnect sequence
         * Only used for the forced reconnect timer
         */
        public void startReconnectFast()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            l_IOHandler.debug("Forced reconnect. Starting reconnect...");
            m_State = "reconnect";
            m_LoggedIn = false;
            m_IsBotRunning = false;
            m_TimeoutOnQueueTimer = false;
            m_RequestedToPauseBot = false;
            m_RequestingServerData = false;
            m_TownListInitialized = false;
            m_CurrentTownIntern = 0;
            m_CurrentFarmIntern = 0;
            m_RefreshTimer.stop();
            m_QueueTimer.stop();
            m_TradeTimer.stop();
            m_ConnectedTimer.stop();
            m_ForcedReconnectTimer.stop();
            m_HttpHandler.clearCookies();
            setStatusBarEvent("Waiting for reconnect");

            m_ReconnectCount++;
            if (m_ReconnectCount > l_Settings.RecMaxReconnects && l_Settings.RecMaxReconnectsEnabled)
            {
                setStatusBarEvent("Bot stopped. Too many errors occured.");
            }
            else
            {
                m_ReconnectTimer.start(l_Settings.RecTimerReconnectMin);
            }
        }

        /*
         * Randomizes all the timers used by the bot
         */ 
        private double randomizeTimer(int p_Interval)
        {
            double l_Quotient = 0;

            Random l_Random = new Random();
            //l_Quotient = l_Random.Next(-10, 10);
            l_Quotient = l_Random.Next(0, 20);
            l_Quotient = 100 + l_Quotient;//Value between 90% and 110%;
            l_Quotient = (l_Quotient / 100.0);//Value between 0.9 and 1.1
            return p_Interval * l_Quotient;
        }

        /*
         * The bot currently uses two different sequences of server requests, this method switches between the two.
         * Should make it harder to detect the bot by making it harder to detect patterns in the server requests.
         */ 
        private void randomizeStateManagerMode()
        {
            Random l_Random = new Random();
            m_StateManagerMode = l_Random.Next(m_StateManagerCount);
        }

        public void sendUnderAttackWarning()
        {
            Settings l_Settings = Settings.Instance;

            bool l_AttackStatusChanged = false;
            string l_EmailMessage = m_Player.getAttackWarning(l_Settings.MailIncludeSupport);

            if(l_EmailMessage.Length > 0)
                l_EmailMessage += Environment.NewLine + Environment.NewLine + Environment.NewLine + m_Player.getAttackWarningForum(l_Settings.MailIncludeSupport);

            if (!l_EmailMessage.Equals(m_EmailMessage) && l_EmailMessage.Length > 0)
                l_AttackStatusChanged = true;
            m_EmailMessage = l_EmailMessage;

            if (l_Settings.MailNotify && l_AttackStatusChanged)
            {
                string l_Subject = "[Grepolis2][" + l_Settings.GenUserName + "][" + l_Settings.GenServer + "] Incoming attacks";
                m_MailClient.sendMail(l_Settings.MailServer, l_Settings.MailPort, true, l_Settings.MailUsername, l_Settings.MailPassword, l_Settings.MailAddress, l_Subject, l_EmailMessage, false);
            }

            //sms notify check
            //if (l_AttackStatusChanged)
            //{
            //    m_MailClient.sendSMS(l_Settings.MailUsername, l_Settings.MailPassword, l_EmailMessage);
            //}
        }

        public void soundUnderAttackWarning()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (m_Player.IncomingAttacks > 0 && l_Settings.SoundIncomingAttack)
                {
                    m_SoundPlayer.PlayLooping();
                }
                else
                {
                    m_SoundPlayer.Stop();
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in soundUnderAttackWarning(): " + ex.Message);
                }
            }
        }

        public void soundCaptchaWarning()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (l_Settings.SoundCaptchaWarning && m_CaptchaDetected)
                {
                    m_SoundPlayerCaptcha.PlayLooping();
                }
                else
                {
                    m_SoundPlayerCaptcha.Stop();
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in soundCaptchaWarning(): " + ex.Message);
                }
            }
        }

        private void updateServerTime(string p_Type, string p_Response)
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                string l_Search = "";
                int l_Index = 0;

                switch (p_Type)
                {
                    //Preserver switch case for the time being, in previous releases the server time had different formats. However, currently there is only one format in use.
                    case "_srvtime":
                        l_Search = "_srvtime\":";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        ServerTime = p_Response.Substring(l_Index + l_Search.Length, 10);// p_Response.IndexOf("}", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        ServerTime = String.Join("", Regex.Split(ServerTime, "[^\\d]"));
                        m_Player.Towns[m_CurrentTownIntern].ServerTime = ServerTime;
                        break;
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in updateServerTime(" + p_Type + "@"+ m_State +"): " + ex.Message);
                }
            }
        }

        //-->Events

        #region Events

        private void setStatusBarEvent(string p_Message)
        {
            CustomArgs l_CustomArgs = new CustomArgs(p_Message);
            statusBarUpdated(this, l_CustomArgs);
        }

        private void versionInfoEvent(string p_VersionInfo)
        {
            CustomArgs l_CustomArgs = new CustomArgs(p_VersionInfo);
            versionInfoUpdated(this, l_CustomArgs);
        }

        private void setGuiToTimeoutProcessedStateEvent()
        {
            CustomArgs l_CustomArgs = new CustomArgs("");
            timeoutProcessedStateChanged(this, l_CustomArgs);

            if (!m_RequestedToPauseBot)
                m_CurrentTownIntern = 0;
            else
            {
                if (m_CurrentTownIntern >= m_Player.Towns.Count)
                    m_CurrentTownIntern = 0;
            }
            m_RequestingServerData = false;
        }

        private void setToTradeProcessedStateEvent()
        {
            CustomArgs l_CustomArgs = new CustomArgs("");
            tradeProcessedStateChanged(this, l_CustomArgs);

            m_CurrentTownTradeIntern = 0;
            m_CurrentTownTradeIntern2 = 0;
            m_RequestingServerData = false;
        }

        private void setToTownProcessedStateEvent()
        {
            CustomArgs l_CustomArgs = new CustomArgs("");
            townProcessedStateChanged(this, l_CustomArgs);
        }

        public void logEvent(string p_Message)
        {
            CustomArgs l_CustomArgs = new CustomArgs(p_Message);
            logUpdated(this, l_CustomArgs);
        }

        private void updateRefreshTimerEvent()
        {
            Settings l_Settings = Settings.Instance;
            m_RefreshTimer.InternalTimer.Stop();

            CustomArgs l_CustomArgs = new CustomArgs(m_RefreshTimer.getTimeLeft());
            refreshTimerUpdated(this, l_CustomArgs);

            if (m_RefreshTimer.isTimerDone())
            {
                m_RefreshTimer.stop();
                m_QueueTimer.stop();
                //m_TradeTimer.stop();
                if (m_QueueTimer.isTimerDone())
                    m_TimeoutOnQueueTimer = true;
                else
                    m_TimeoutOnQueueTimer = false;
                if (m_TradeTimer.isTimerDone())
                    m_TimeoutOnTradeTimer = true;
                else
                    m_TimeoutOnTradeTimer = false;
                m_RequestingServerData = true;
                if (l_Settings.AdvSchedulerBot)
                {
                    if (l_Settings.FarmerScheduler.Split(';')[DateTime.Now.Hour].Equals("False"))
                    {
                        if (!m_IgnoreScheduler)
                        {
                            logEvent("Bot is disabled by scheduler.");
                            m_CurrentTownIntern = m_Player.Towns.Count;
                            m_CurrentTownTradeIntern = m_Player.Towns.Count;
                        }
                        else
                        {
                            //Ignore schuler during first update, need when scheduler disables bot at startup
                            m_IgnoreScheduler = false;
                        }
                    }
                }
                //Captcha check pre cycle. Helps detecting captcha's that occured during idle period.
                CustomArgs l_CustomArgs2 = new CustomArgs("");
                captchaCheckPreCycle(this, l_CustomArgs2);
                //switchTown();
            }
            else
                m_RefreshTimer.InternalTimer.Start();
        }

        private void updateQueueTimerEvent()
        {
            m_QueueTimer.InternalTimer.Stop();

            CustomArgs l_CustomArgs = new CustomArgs(m_QueueTimer.getTimeLeft());
            queueTimerUpdated(this, l_CustomArgs);

            if (!m_RefreshTimer.isTimerDone())
            {
                if (m_QueueTimer.isTimerDone())
                {
                    m_QueueTimer.stop();
                    m_TimeoutOnQueueTimer = true;
                }
                else
                {
                    m_TimeoutOnQueueTimer = false;
                    m_QueueTimer.InternalTimer.Start();
                }
            }
        }

        private void updateTradeTimerEvent()
        {
            m_TradeTimer.InternalTimer.Stop();

            CustomArgs l_CustomArgs = new CustomArgs(m_TradeTimer.getTimeLeft());
            tradeTimerUpdated(this, l_CustomArgs);

            //if (!m_RefreshTimer.isTimerDone())
            //{
                if (m_TradeTimer.isTimerDone())
                {
                    m_TradeTimer.stop();
                    m_TimeoutOnTradeTimer = true;
                }
                else
                {
                    m_TimeoutOnTradeTimer = false;
                    m_TradeTimer.InternalTimer.Start();
                }
            //}
        }

        private void updateReconnectTimerEvent()
        {
            m_ReconnectTimer.InternalTimer.Stop();

            CustomArgs l_CustomArgs = new CustomArgs(m_ReconnectTimer.getTimeLeft());
            reconnectTimerUpdated(this, l_CustomArgs);
        }

        private void updateForcedReconnectTimerEvent()
        {
            m_ForcedReconnectTimer.InternalTimer.Stop();

            CustomArgs l_CustomArgs = new CustomArgs(m_ForcedReconnectTimer.getTimeLeft());
            forcedReconnectTimerUpdated(this, l_CustomArgs);

            m_ForcedReconnectTimer.InternalTimer.Start();
        }

        private void updateConnectedTimerEvent()
        {
            m_ConnectedTimer.InternalTimer.Stop();

            CustomArgs l_CustomArgs = new CustomArgs(m_ConnectedTimer.getElapsedTime());
            connectedTimerUpdated(this, l_CustomArgs);

            m_ConnectedTimer.InternalTimer.Start();
        }

        #endregion

        //-->Http requests and handlers

        #region Http requests and handlers
        /*
         * Validate response
         * Codes:
         * 1 = OK
         * 2 = Reconnect necessarily
         * 3 = Server error
         * 4 = sec_check_failed
         */
        private int validateResponse(string p_Response, string p_Method)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            int l_ValidCode = 1;
            //58 {"redirect":"http:\/\/beta.grepolis.com\/start?nosession"}8|10 _srvtime1303484209
            if (!p_Response.Contains(m_DisconnectedString) && !p_Response.Contains("start?nosession"))//Checks if you're still connected with the server
            {
                if (p_Response.Contains(": startIndex"))//Error caused by game engine
                {
                    l_ValidCode = 3;
                    if (l_Settings.AdvDebugMode)
                        l_IOHandler.saveServerResponse(p_Method, p_Response);
                }
                else if (p_Response.Contains("start?sec_check_failed"))//What causes this error??
                {
                    l_ValidCode = 4;
                }
                else
                {
                    //Add future checks here
                    l_ValidCode = 1;
                }
            }
            else//you're disconnected from server
            {
                l_ValidCode = 2;
            }

            return l_ValidCode;
        }

        private void processValidatedResponse(int p_ValidCode)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            switch (p_ValidCode)
            {
                case 2://Reconnect necessarily
                    startReconnect(1);
                    if (l_Settings.AdvDebugMode)
                        l_IOHandler.debug("Server response indicated that you were no longer logged in.");
                    break;
                case 3://Game server error, reconnect necessarily
                    startReconnect(1);
                    if (l_Settings.AdvDebugMode)
                        l_IOHandler.debug("Server response indicated that there was a gameserver error.");
                    break;
                case 4:
                    startReconnect(2);
                    if (l_Settings.AdvDebugMode)
                        l_IOHandler.debug("Server response indicated the following error: sec_check_failed.");
                    break;
            }
        }

        /*
         * Updates some general town information available from notifications
         * Resources
         * Production
         * Storage
         * Conquest check
         */
        private void updateResourcesFromNotification(string p_Response, bool p_IsTrade)
        {
            int l_Index = -1;
            int l_IndexTown = -1;
            string l_Search = "";
            string l_Population = "";
            string l_HasConqueror = "";
            string l_Wood = "";
            string l_Stone = "";
            string l_Iron = "";
            string l_WoodProduction = "";
            string l_StoneProduction = "";
            string l_IronProduction = "";
            string l_Storage = "";
            string l_ResourcesLastUpdate = "";

            if (p_Response.Contains("resources_last_update"))
            {
                l_Search = "resources_last_update\\\":";
                l_Index = p_Response.IndexOf(l_Search, 0);
                l_ResourcesLastUpdate = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Search = "available_population\\\":";
                l_Index = p_Response.IndexOf(l_Search, 0);
                l_Population = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Search = "has_conqueror\\\":";
                l_Index = p_Response.IndexOf(l_Search, l_Index);
                l_HasConqueror = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Search = "last_wood\\\":";
                l_Index = p_Response.IndexOf(l_Search, l_Index);
                l_Wood = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Search = "last_stone\\\":";
                l_Index = p_Response.IndexOf(l_Search, l_Index);
                l_Stone = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Search = "last_iron\\\":";
                l_Index = p_Response.IndexOf(l_Search, l_Index);
                l_Iron = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Search = "\\\"storage\\\":";
                l_Index = p_Response.IndexOf(l_Search, 0);
                l_Storage = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                l_Search = "production\\\":";
                l_Index = p_Response.IndexOf(l_Search, 0);
                l_Search = "\\\"wood\\\":";
                l_Index = p_Response.IndexOf(l_Search, l_Index);
                l_WoodProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Search = "\\\"stone\\\":";
                l_Index = p_Response.IndexOf(l_Search, l_Index);
                l_StoneProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Search = "\\\"iron\\\":";
                l_Index = p_Response.IndexOf(l_Search, l_Index);
                l_IronProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("}", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                if (p_IsTrade)
                    l_IndexTown = m_CurrentTownTradeIntern;
                else
                    l_IndexTown = m_CurrentTownIntern;

                m_Player.Towns[l_IndexTown].ResourcesLastUpdate = l_ResourcesLastUpdate;
                m_Player.Towns[l_IndexTown].PopulationAvailable = int.Parse(l_Population);
                m_Player.Towns[l_IndexTown].HasConqueror = l_HasConqueror;
                m_Player.Towns[l_IndexTown].Wood = int.Parse(l_Wood);
                m_Player.Towns[l_IndexTown].Stone = int.Parse(l_Stone);
                m_Player.Towns[l_IndexTown].Iron = int.Parse(l_Iron);
                m_Player.Towns[l_IndexTown].WoodProduction = int.Parse(l_WoodProduction);
                m_Player.Towns[l_IndexTown].StoneProduction = int.Parse(l_StoneProduction);
                m_Player.Towns[l_IndexTown].IronProduction = int.Parse(l_IronProduction);
                m_Player.Towns[l_IndexTown].Storage = int.Parse(l_Storage);
            }
        }

        private void addNotifications(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;
            string l_Search = "";
            string l_Response = "";
            int l_Index = -1;

            //Notifications
            string l_Notifications = "";
            string l_Notify_id = "";
            string l_Time = "";
            string l_Type = "";
            string l_Subject = "";
            string l_BuildingLocal = "";
            string l_TownName = "";
            string l_Level = "";

            try
            {
                l_Search = "\"notifications\":[";
                l_Index = p_Response.IndexOf(l_Search, 0);
                if (l_Index != -1)
                {
                    l_Response = p_Response.Substring(l_Index, p_Response.Length - l_Index);
                    l_Index = l_Response.IndexOf(l_Search, 0);
                    if (l_Response.Contains("}],"))
                    {
                        //Uses LastIndexOf because hero data also uses "}],"
                        //If there is another "}]," after the notifications data set it might cause issues.
                        //Solution, search for "}],\"" using IndexOf
                        l_Notifications = l_Response.Substring(l_Index + l_Search.Length, l_Response.LastIndexOf("}],") - (l_Index + l_Search.Length));
                        l_Notifications += "}";
                    }
                    else
                        l_Notifications = l_Response.Substring(l_Index + l_Search.Length, l_Response.IndexOf("]", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                    if (l_Notifications.Length > 0)
                    {
                        l_Search = "\"id\":";
                        l_Index = l_Notifications.IndexOf(l_Search, 0);
                        while (l_Index != -1)
                        {
                            l_Notify_id = l_Notifications.Substring(l_Index + l_Search.Length, l_Notifications.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"time\":";
                            l_Index = l_Notifications.IndexOf(l_Search, l_Index);
                            l_Time = l_Notifications.Substring(l_Index + l_Search.Length, l_Notifications.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"type\":\"";
                            l_Index = l_Notifications.IndexOf(l_Search, l_Index);
                            l_Type = l_Notifications.Substring(l_Index + l_Search.Length, l_Notifications.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            //param_str
                            if (l_Type.Equals("building_finished"))
                            {
                                l_Search = "building_name\\\":\\\"";
                                l_Index = l_Notifications.IndexOf(l_Search, l_Index);
                                l_BuildingLocal = l_Notifications.Substring(l_Index + l_Search.Length, l_Notifications.IndexOf("\\\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_BuildingLocal = l_Parser.fixSpecialCharacters(l_BuildingLocal);
                                l_Search = "town_name\\\":\\\"";
                                l_Index = l_Notifications.IndexOf(l_Search, l_Index);
                                l_TownName = l_Notifications.Substring(l_Index + l_Search.Length, l_Notifications.IndexOf("\\\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_TownName = l_Parser.fixSpecialCharacters(l_TownName);
                                l_Search = "new_level\\\":";
                                l_Index = l_Notifications.IndexOf(l_Search, l_Index);
                                l_Level = l_Notifications.Substring(l_Index + l_Search.Length, l_Notifications.IndexOf(",\\", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_Subject = "Expansion completed: " + l_BuildingLocal + " (" + l_Level + ") in " + l_TownName;
                            }
                            else if (l_Type.Equals("newaward"))
                            {
                                l_Search = "name\":\"";
                                l_Index = l_Notifications.IndexOf(l_Search, l_Index);
                                l_Subject = "Achievement: " + l_Notifications.Substring(l_Index + l_Search.Length, l_Notifications.IndexOf("\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            }
                            else
                            {
                                l_Search = "\"subject\":";
                                l_Index = l_Notifications.IndexOf(l_Search, l_Index);
                                l_Subject = l_Notifications.Substring(l_Index + l_Search.Length, l_Notifications.IndexOf("}", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                if (l_Subject.StartsWith("\""))
                                    l_Subject = l_Subject.Substring(1, l_Subject.Length - 1);
                                if (l_Subject.EndsWith("\""))
                                    l_Subject = l_Subject.Substring(0, l_Subject.Length - 1);
                            }
                            l_Subject = l_Parser.fixSpecialCharacters(l_Subject);
                            //Add notification
                            if (l_Type.Equals("botcheck"))
                            {
                                m_Player.addNotification(m_ServerTime, l_Notify_id, l_Time, l_Type, "Captcha notification detected");
                                logEvent("Captcha notification detected");
                                m_CaptchaDetectedTime = DateTime.Now;

                                captchaDetectedSequence(true);
                            }
                            else if (!l_Type.Equals("backbone") && !l_Type.Equals("systemmessage"))
                            {
                                m_Player.addNotification(m_ServerTime, l_Notify_id, l_Time, l_Type, l_Subject);
                            }
                            //Search next notification
                            l_Search = "\"id\":";
                            l_Index = l_Notifications.IndexOf(l_Search, l_Index);
                        }
                    }
                    //Update Nlreq_id
                    m_Nlreq_id = m_Player.getLatestNotificationID();
                }
            }
            catch (Exception ex)
            {
                if (l_Settings.AdvDebugMode)
                {
                    l_IOHandler.debug("Exception in addNotifications(): " + ex.Message);
                    l_IOHandler.saveServerResponse("addNotifications", ex.Message + "\n" + p_Response);
                }
            }
        }

        public void captchaDetectedSequence(bool p_RealCaptcha)
        {
            m_CaptchaDetected = true;
            if (p_RealCaptcha)
                soundCaptchaWarning();
            else
                m_CaptchaMassMailDetected = true;
            //Pause game
            CustomArgs l_CustomArgs = new CustomArgs("");
            pauseBotRequestCaptcha(this, l_CustomArgs);
            if (p_RealCaptcha)
            {
                //Start captcha check timer
                startCaptchaCheckTimerRequest(this, l_CustomArgs);
            }
        }

        /*
         * The main update function for the latest town data.
         * It's the only server request that returns information about multiple towns.
         * Note: Only call this once every login/reconnect, will result in ban otherwise.
         */ 
        public void updateGameData()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_Townid = m_Player.DefaultTownID;
            string l_Action = "get";

            try
            {
                m_State = "updateGameData";
                setStatusBarEvent("Updating game data");

                //"type":"frontendBridge"
                //"type":"buildings"
                //"type":"powers"
                //"type":"godPowersTown"
                //"type":"units"
                //"type":"awards"
                //"type":"inventoryItems"
                //"type":"heroes"
                //"type":"progressable"
                //"type":"hercules2014Units"
                //"type":"hercules2014Stages"
                //"type":"hercules2014Meta"
                //"type":"halloweenIngredients"
                //"type":"map","param":{"x":13,"y":6}
                //"type":"bar"
                //"type":"backbone"

                NameValueCollection l_Content = new NameValueCollection();
                string l_Url = "http://" + l_Settings.GenServer + "/game/data?town_id=" + l_Townid + "&action=" + l_Action + "&h=" + m_H;
                Uri l_Uri = new Uri(l_Url);
                //l_Content.Add("json", "{\"types\":[{\"type\":\"buildings\"},{\"type\":\"units\"},{\"type\":\"map\",\"param\":{\"x\":10,\"y\":6}},{\"type\":\"bar\"},{\"type\":\"backbone\"}],\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}");
                l_Content.Add("json", "{\"types\":[{\"type\":\"buildings\"},{\"type\":\"units\"},{\"type\":\"map\",\"param\":{\"x\":10,\"y\":6}},{\"type\":\"bar\"},{\"type\":\"backbone\"}],\"town_id\":" + l_Townid + ",\"nl_init\":false}");//2.80 nlred_id replaced

                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                m_HttpHandler.Headers.Remove("X-Requested-With");
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateGameData(): " + e.Message);
            }
        }

        /*
         * Handles the server response of updateGameData().
         */
        public void updateGameDataResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            //p_Response = l_IOHandler.loadError();

            try
            {
                string l_Search = "";
                string l_Error = "";
                int l_Index = -1;
                int l_IndexEnd = -1;
                int l_IndexSub = -1;

                int l_ValidCode = validateResponse(p_Response, "updateGameDataResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        //Map.Player
                        int l_CulturalLevel = 2;
                        int l_CulturalPointsNext = 0;
                        string l_CulturalPoints = "";
                        string l_Villages = "";
                        string l_CulturalLevelStr = "";
                        string l_CitiesStr = "";
                        string l_CulturalPointsStr = "";
                        l_Search = "\"villages\":";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_Villages = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Search = "\"cultural_points\":";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_CulturalPoints = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_CulturalPointsNext = (int)((3.0 / 2.0) * (Math.Pow(l_CulturalLevel + 1, 2.0) + (-3.0 * (l_CulturalLevel + 1) + 2.0)));
                        while (int.Parse(l_CulturalPoints) >= l_CulturalPointsNext)
                        {
                            l_CulturalLevel++;
                            l_CulturalPointsNext = (int)((3.0 / 2.0) * (Math.Pow(l_CulturalLevel + 1, 2.0) + (-3.0 * (l_CulturalLevel + 1) + 2.0)));
                        }
                        l_CulturalLevelStr = "Cultural Level: " + l_CulturalLevel;//Localisation available in cultural request
                        m_Player.CulturalLevelStr = l_CulturalLevelStr;
                        l_CitiesStr = "Cities: " + l_Villages + "/" + l_CulturalLevel;//Localisation available in cultural request
                        m_Player.CulturalCitiesStr = l_CitiesStr;
                        l_CulturalPointsStr = l_CulturalPoints + "/" + l_CulturalPointsNext;
                        m_Player.CulturalPointsStr = l_CulturalPointsStr;
                        m_Player.CulturalPointsCurrent = int.Parse(l_CulturalPoints);
                        m_Player.CulturalPointsMax = l_CulturalPointsNext;

                        //PlayerLedger
                        l_Search = "{\"model_class_name\":\"PlayerLedger\",\"data\":{";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_Search = "\"gold\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        string l_Gold = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.Gold = int.Parse(l_Gold);

                        //PlayerGods
                        int l_IndexPlayerGods = -1;
                        string l_Favor = "";
                        l_Search = "{\"model_class_name\":\"PlayerGods\",\"data\":{";
                        l_IndexPlayerGods = p_Response.IndexOf(l_Search, 0);
                        //zeus
                        l_Search = "\"zeus\":{\"current\":";
                        l_Index = p_Response.IndexOf(l_Search, l_IndexPlayerGods);
                        l_Favor = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.FavorZeus = int.Parse(l_Favor.Split('.')[0]);
                        l_Search = "\"production\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        m_Player.FavorZeusProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //poseidon
                        l_Search = "\"poseidon\":{\"current\":";
                        l_Index = p_Response.IndexOf(l_Search, l_IndexPlayerGods);
                        l_Favor = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.FavorPoseidon = int.Parse(l_Favor.Split('.')[0]);
                        l_Search = "\"production\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        m_Player.FavorPoseidonProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //hera
                        l_Search = "\"hera\":{\"current\":";
                        l_Index = p_Response.IndexOf(l_Search, l_IndexPlayerGods);
                        l_Favor = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.FavorHera = int.Parse(l_Favor.Split('.')[0]);
                        l_Search = "\"production\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        m_Player.FavorHeraProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //athena
                        l_Search = "\"athena\":{\"current\":";
                        l_Index = p_Response.IndexOf(l_Search, l_IndexPlayerGods);
                        l_Favor = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.FavorAthena = int.Parse(l_Favor.Split('.')[0]);
                        l_Search = "\"production\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        m_Player.FavorAthenaProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //athena
                        l_Search = "\"hades\":{\"current\":";
                        l_Index = p_Response.IndexOf(l_Search, l_IndexPlayerGods);
                        l_Favor = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.FavorHades = int.Parse(l_Favor.Split('.')[0]);
                        l_Search = "\"production\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        m_Player.FavorHadesProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //artemis
                        l_Search = "\"artemis\":{\"current\":";
                        l_Index = p_Response.IndexOf(l_Search, l_IndexPlayerGods);
                        if (l_Index != -1)
                        {
                            l_Favor = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            m_Player.FavorArtemis = int.Parse(l_Favor.Split('.')[0]);
                            l_Search = "\"production\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            m_Player.FavorArtemisProduction = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        }

                        //CommandsMenuBubble
                        string l_IncomingAttacksTotal = "";
                        l_Search = "{\"model_class_name\":\"CommandsMenuBubble\"";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_Search = "\"incoming_attacks_total\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_IncomingAttacksTotal = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.IncomingAttacks = int.Parse(l_IncomingAttacksTotal);

                        //Towns
                        l_Search = "{\"class_name\":\"Towns\",\"data\":[";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IndexEnd = p_Response.IndexOf("]", l_Index + l_Search.Length);
                        string l_TownID = "";
                        string l_TownName = "";
                        string l_IslandX = "000";
                        string l_IslandY = "000";
                        string l_Plenty = "";
                        string l_Rare = "";
                        //string l_Population = "";
                        string l_PopulationAvailable = "";
                        string l_PopulationExtra = "";
                        string l_God = "";
                        string l_Points = "";
                        string l_EspionageStorage = "";
                        string l_Storage = "";
                        string l_HasConqueror = "";
                        string l_Wood = "";
                        string l_Stone = "";
                        string l_Iron = "";
                        string l_WoodProduction = "";
                        string l_StoneProduction = "";
                        string l_IronProduction = "";
                        string l_AvailableTradeCapacity = "";
                        string l_ResourcesLastUpdate = "";
                        string l_ResponseSub = "";

                        l_Search = "player_id";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        while (l_Index < l_IndexEnd && l_Index != -1)
                        {
                            l_ResponseSub = p_Response.Substring(l_Index, p_Response.IndexOf("}}", l_Index) - l_Index);
                            l_Search = "\"name\":\"";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_TownName = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf("\"", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_TownName = l_Parser.fixSpecialCharacters(l_TownName);
                            l_Search = "\"island_x\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_IslandX = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"island_y\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_IslandY = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"resources_last_update\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_ResourcesLastUpdate = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"resource_rare\":\"";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_Rare = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf("\"", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"resource_plenty\":\"";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_Plenty = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf("\"", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            //l_Search = "\"population\":"; //Example "population":{"max":1457,
                            //l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            //l_Population = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"god\":\"";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            if (l_IndexSub != -1)
                                l_God = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf("\"", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            else
                            {
                                l_Search = "\"god\":";
                                l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                                l_God = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            }
                            l_Search = "\"points\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_Points = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"espionage_storage\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_EspionageStorage = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"population_extra\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_PopulationExtra = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"id\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_TownID = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"available_population\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_PopulationAvailable = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"has_conqueror\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_HasConqueror = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"last_wood\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_Wood = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"last_stone\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_Stone = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"last_iron\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_Iron = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"available_trade_capacity\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_AvailableTradeCapacity = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"storage\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_Storage = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"production\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_Search = "\"wood\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, l_IndexSub);
                            l_WoodProduction = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"stone\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, l_IndexSub);
                            l_StoneProduction = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"iron\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, l_IndexSub);
                            l_IronProduction = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf("}", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));

                            if (m_Player.isUniqueTown(l_TownID))
                            {
                                m_Player.Towns.Add(new Town(l_TownID, l_TownName, l_IslandX, l_IslandY));
                                m_TownsSorted = false;
                            }
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].ResourcesLastUpdate = l_ResourcesLastUpdate;
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].ResourceRare = l_Rare;
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].ResourcePlenty = l_Plenty;
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].PopulationMax = int.Parse(l_Population);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].God = l_God;
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Points = l_Points;
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].EspionageStorage = int.Parse(l_EspionageStorage);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].PopulationExtra = int.Parse(l_PopulationExtra);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].PopulationAvailable = int.Parse(l_PopulationAvailable);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].HasConqueror = l_HasConqueror;
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Wood = int.Parse(l_Wood);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Stone = int.Parse(l_Stone);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Iron = int.Parse(l_Iron);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].FreeTradeCapacity = int.Parse(l_AvailableTradeCapacity);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Storage = int.Parse(l_Storage);
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].WoodProduction = int.Parse(l_WoodProduction);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].WoodProduction = int.Parse(l_WoodProduction.Split('.')[0]);
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].StoneProduction = int.Parse(l_StoneProduction);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].StoneProduction = int.Parse(l_StoneProduction.Split('.')[0]);
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].IronProduction = int.Parse(l_IronProduction);
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].IronProduction = int.Parse(l_IronProduction.Split('.')[0]);

                            //next town
                            l_Search = "player_id";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        }

                        //Collections.RunningPowers
                        string l_CastedPower = "";
                        m_Player.resetCastedPowers();
                        l_Search = "{\"class_name\":\"RunningPowers\"";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IndexEnd = p_Response.IndexOf("]}", l_Index);
                        l_Search = "\"power_id\":\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        while (l_Index < l_IndexEnd && l_Index != -1)
                        {
                            l_CastedPower = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Search = "\"town_id\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_TownID = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].CastedPowers += l_CastedPower + ";";
                            //next power
                            l_Search = "\"power_id\":\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                        }

                        //Collections.BuildingOrders
                        string l_Building = "";
                        l_Search = "{\"class_name\":\"BuildingOrders\",\"data\":[";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IndexEnd = p_Response.IndexOf("]", l_Index + l_Search.Length);
                        l_Search = "\"town_id\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        m_Player.resetBuildingQueue();
                        while (l_Index < l_IndexEnd && l_Index != -1)
                        {
                            l_TownID = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"building_type\":\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Building = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].IngameBuildingQueueParsed += l_Building + ";";
                            //next building order
                            l_Search = "\"town_id\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                        }
                        m_Player.updateBuildingQueue();

                        //Collections.Trades
                        l_Search = "{\"class_name\":\"Trades\",\"data\":[";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IndexEnd = p_Response.IndexOf("]", l_Index + l_Search.Length);
                        l_Search = "\"origin_town_id\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        int l_IndexStart = l_Index;
                        string l_origin_town_id = "";
                        //string l_origin_town_name = "";
                        //string l_origin_ix = "";
                        //string l_origin_iy = "";
                        //string l_origin_town_player_id = "";
                        //string l_origin_farm_town_name = "";
                        string l_destination_town_id = "";
                        //string l_destination_town_name = "";
                        //string l_destination_ix = "";
                        //string l_destination_iy = "";
                        //string l_destination_town_player_id = "";
                        //string l_destination_farm_town_name = "";
                        //string l_wonder_type = "";
                        //string l_wonder_ix = "";
                        //string l_wonder_iy = "";
                        string l_id = "";
                        //string l_started_at = "";
                        //string l_arrival_at = "";
                        //string l_origin_town_type = "";
                        //string l_destination_town_type = "";
                        string l_in_exchange = "";
                        //string l_arrival_seconds_left = "";
                        //string l_cancelable = "";
                        //string l_origin_town_link = "";
                        //string l_destination_town_link = "";
                        m_Player.resetTrades();
                        while (l_Index < l_IndexEnd && l_Index != -1)
                        {
                            l_origin_town_id = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"destination_town_id\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_destination_town_id = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"id\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_id = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"wood\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_Wood = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"stone\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_Stone = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"iron\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_Iron = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"in_exchange\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_in_exchange = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            //add trade
                            m_Player.Trades.Add(new Trade(l_id));
                            m_Player.Trades[m_Player.getTradeById(l_id)].Origin_town_id = l_origin_town_id;
                            m_Player.Trades[m_Player.getTradeById(l_id)].Destination_town_id = l_destination_town_id;
                            m_Player.Trades[m_Player.getTradeById(l_id)].Wood = l_Wood;
                            m_Player.Trades[m_Player.getTradeById(l_id)].Stone = l_Stone;
                            m_Player.Trades[m_Player.getTradeById(l_id)].Iron = l_Iron;
                            m_Player.Trades[m_Player.getTradeById(l_id)].In_exchange = l_in_exchange;
                            //next trade
                            l_Search = "\"origin_town_id\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart + l_Search.Length);
                            l_IndexStart = l_Index;
                        }

                        //Collections.RemainingUnitOrders
                        l_Search = "{\"class_name\":\"RemainingUnitOrders\",\"data\":[";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IndexEnd = p_Response.IndexOf("]", l_Index + l_Search.Length);
                        l_Search = "\"UnitOrder\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_IndexStart = l_Index;
                        string l_UnitType = "";
                        string l_Kind = "";
                        string l_UnitsLeft = "";
                        int l_UnitIndex = -1;
                        m_Player.resetUnitQueue();
                        while (l_Index < l_IndexEnd && l_Index != -1)
                        {
                            l_Search = "\"town_id\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_TownID = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"unit_type\":\"";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_UnitType = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"kind\":\"";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_Kind = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"units_left\":";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart);
                            l_UnitsLeft = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                            l_UnitIndex = m_Player.Towns[m_CurrentTownIntern].getUnitIndex(l_UnitType);
                            if (l_UnitIndex != -1)
                            {
                                m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].ArmyUnits[l_UnitIndex].QueueGame += int.Parse(l_UnitsLeft);
                                if (l_Kind.Equals("ground"))
                                    m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].SizeOfLandUnitQueue += 1;
                                else if (l_Kind.Equals("naval"))
                                    m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].SizeOfNavyUnitQueue += 1;
                            }

                            //next queue order
                            l_Search = "\"UnitOrder\"";
                            l_Index = p_Response.IndexOf(l_Search, l_IndexStart + l_Search.Length);
                            l_IndexStart = l_Index;
                        }

                        //Collections.Units
                        string l_CurrentTownID = "";
                        string l_HomeTownID = "";
                        string l_Unit = "";
                        string l_Count = "";
                        l_Search = "{\"class_name\":\"Units\",\"data\":[";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_Search = "\"home_town_id\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        m_Player.resetUnits();
                        while (l_Index != -1)
                        {
                            l_HomeTownID = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"current_town_id\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_CurrentTownID = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            for (int i = 0; i < 27; i++)
                            {
                                l_Search = ",\"";//,"unit":1,"unit_next".....
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_Unit = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_Search = ":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_Count = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                m_Player.setUnitCount(l_Unit, l_Count, l_HomeTownID, l_CurrentTownID);
                            }
                            //next unit data set
                            l_Search = "\"home_town_id\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        }

                        //Collections.TownResearches
                        l_Search = "{\"class_name\":\"TownResearches\",\"data\":[";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IndexEnd = p_Response.IndexOf("]}", l_Index);
                        l_Search = "Researches";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_ResponseSub = p_Response.Substring(l_Index, p_Response.IndexOf("}}", l_Index) - l_Index);
                        while (l_Index != -1 && l_Index < l_IndexEnd)
                        {
                            l_Search = "\"id\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_TownID = l_ResponseSub.Substring(l_IndexSub + l_Search.Length);

                            //Update research status
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("slinger", l_ResponseSub.Contains("\"slinger\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("archer", l_ResponseSub.Contains("\"archer\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("hoplite", l_ResponseSub.Contains("\"hoplite\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("town_guard", l_ResponseSub.Contains("\"town_guard\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("diplomacy", l_ResponseSub.Contains("\"diplomacy\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("espionage", l_ResponseSub.Contains("\"espionage\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("booty", l_ResponseSub.Contains("\"booty\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("pottery", l_ResponseSub.Contains("\"pottery\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("rider", l_ResponseSub.Contains("\"rider\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("architecture", l_ResponseSub.Contains("\"architecture\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("instructor", l_ResponseSub.Contains("\"instructor\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("bireme", l_ResponseSub.Contains("\"bireme\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("building_crane", l_ResponseSub.Contains("\"building_crane\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("meteorology", l_ResponseSub.Contains("\"meteorology\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("chariot", l_ResponseSub.Contains("\"chariot\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("attack_ship", l_ResponseSub.Contains("\"attack_ship\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("conscription", l_ResponseSub.Contains("\"conscription\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("shipwright", l_ResponseSub.Contains("\"shipwright\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("demolition_ship", l_ResponseSub.Contains("\"demolition_ship\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("catapult", l_ResponseSub.Contains("\"catapult\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("cryptography", l_ResponseSub.Contains("\"cryptography\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("colonize_ship", l_ResponseSub.Contains("\"colonize_ship\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("small_transporter", l_ResponseSub.Contains("\"small_transporter\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("plow", l_ResponseSub.Contains("\"plow\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("berth", l_ResponseSub.Contains("\"berth\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("trireme", l_ResponseSub.Contains("\"trireme\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("phalanx", l_ResponseSub.Contains("\"phalanx\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("breach", l_ResponseSub.Contains("\"breach\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("mathematics", l_ResponseSub.Contains("\"mathematics\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("ram", l_ResponseSub.Contains("\"ram\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("cartography", l_ResponseSub.Contains("\"cartography\":true"));
                            m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("take_over", l_ResponseSub.Contains("\"take_over\":true"));
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("stone_storm", l_ResponseSub.Contains("\"stone_storm\":true"));
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("temple_looting", l_ResponseSub.Contains("\"temple_looting\":true"));
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("divine_selection", l_ResponseSub.Contains("\"divine_selection\":true"));
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("combat_experience", l_ResponseSub.Contains("\"combat_experience\":true"));
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("strong_wine", l_ResponseSub.Contains("\"strong_wine\":true"));
                            //m_Player.Towns[m_Player.getTownIndexByID(l_TownID)].Research.setResearchStatus("set_sail", l_ResponseSub.Contains("\"set_sail\":true"));

                            //Next
                            l_Search = "Researches";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                            l_ResponseSub = p_Response.Substring(l_Index, p_Response.IndexOf("}}", l_Index) - l_Index);
                        }

                        //Militia
                        //Found at {"class_name":"Militias","data":[]}
                        //However, not useful because this method (updateGameData) is only called at startup and reconnects.

                        //Translation data - Buildings
                        string l_LocalName = "";
                        string l_DevName = "";
                        l_Search = "\"buildings\":{\"data\":";
                        l_Index = p_Response.IndexOf(l_Search);
                        l_IndexEnd = p_Response.IndexOf("\"mtime\":", l_Index);
                        l_Search = "{\\\"name\\\":\\\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        while (l_Index != -1 && l_Index < l_IndexEnd)
                        {
                            l_LocalName = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_LocalName = l_Parser.fixSpecialCharacters(l_LocalName);
                            l_Search = "\\\"id\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_DevName = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            if (!l_DevName.Equals("place"))
                                m_Player.setLocalBuildingName(l_DevName, l_LocalName);
                            //Search next
                            l_Search = "{\\\"name\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                        }

                        //Translation data - Units
                        l_LocalName = "";
                        l_DevName = "";
                        l_Search = "\"units\":{\"data\":";
                        l_Index = p_Response.IndexOf(l_Search);
                        l_IndexEnd = p_Response.IndexOf("\"mtime\":", l_Index);
                        l_Search = "\\\"id\\\":\\\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_Search = "\\\"name\\\":\\\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);//Skip militia
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        while (l_Index != -1 && l_Index < l_IndexEnd)
                        {
                            l_LocalName = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_LocalName = l_Parser.fixSpecialCharacters(l_LocalName);
                            l_Search = "\\\"id\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_DevName = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\", l_Index + l_Search.Length) - (l_Index + l_Search.Length));                            
                            m_Player.setLocalUnitName(l_DevName, l_LocalName);
                            //Search next
                            l_Search = "\\\"name\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                        }

                        //Groups information - Groups require the Grepolis premium feature "Administrator"
                        //Collections.TownGroups
                        string l_GroupName = "";
                        string l_GroupID = "";
                        int l_GroupIndex = 0;
                        l_Search = "{\"class_name\":\"TownGroups\",\"data\":[";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IndexEnd = p_Response.IndexOf("]}", l_Index);
                        l_Search = "TownGroup";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_ResponseSub = p_Response.Substring(l_Index, p_Response.IndexOf("}}", l_Index) - l_Index);
                        while (l_Index != -1 && l_Index < l_IndexEnd)
                        {
                            l_Search = "\"name\":\"";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_GroupName = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf("\",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_GroupName = l_Parser.fixSpecialCharacters(l_GroupName);
                            l_Search = "\"id\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_GroupID = l_ResponseSub.Substring(l_IndexSub + l_Search.Length);

                            //Add
                            if (m_Player.isUniqueGroup(l_GroupID))
                            {
                                //Ignore default groups (No group, All)
                                if (!l_GroupID.Equals("-1") && !l_GroupID.Equals("0"))
                                    m_Player.Groups.Add(new Group(l_GroupID, l_GroupName, true));
                            }
                            else
                            {
                                //reset
                                l_GroupIndex = m_Player.getGroupIndexByID(l_GroupID);
                                m_Player.Groups[l_GroupIndex].Towns.Clear();
                            }

                            //Next
                            l_Search = "TownGroup";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                            l_ResponseSub = p_Response.Substring(l_Index, p_Response.IndexOf("}}", l_Index) - l_Index);
                        }

                        //Groups information - Groups require the Grepolis premium feature "Administrator"
                        //Collections.TownGroupTowns
                        l_TownID = "";
                        l_GroupID = "";
                        l_Search = "{\"class_name\":\"TownGroupTowns\",\"data\":[";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IndexEnd = p_Response.IndexOf("]}", l_Index);
                        l_Search = "\"TownGroupTown\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_ResponseSub = p_Response.Substring(l_Index, p_Response.IndexOf("}}", l_Index) - l_Index);
                        while (l_Index != -1 && l_Index < l_IndexEnd)
                        {
                            l_Search = "\"group_id\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_GroupID = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));
                            l_Search = "\"town_id\":";
                            l_IndexSub = l_ResponseSub.IndexOf(l_Search, 0);
                            l_TownID = l_ResponseSub.Substring(l_IndexSub + l_Search.Length, l_ResponseSub.IndexOf(",", l_IndexSub + l_Search.Length) - (l_IndexSub + l_Search.Length));

                            //Add
                            //Ignore default groups (No group, All)
                            if (!l_GroupID.Equals("-1") && !l_GroupID.Equals("0"))
                            {
                                l_GroupIndex = m_Player.getGroupIndexByID(l_GroupID);
                                m_Player.Groups[l_GroupIndex].Towns.Add(l_TownID);
                            }

                            //Next
                            l_Search = "\"TownGroupTown\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                            if (l_Index != -1)
                                l_ResponseSub = p_Response.Substring(l_Index, p_Response.IndexOf("}}", l_Index) - l_Index);
                        }
                        //End
                        //Add here new stuff to extract;
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in updateGameDataResponse(): " + l_Error);

                            if (m_RetryCountServerError < 1)
                            {
                                m_RetryCountServerError++;
                                retryManager();
                            }
                            else
                            {
                                m_RetryCountServerError = 0;
                                startReconnect(1);
                            }
                        }
                    }//end error
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;

                //Server time
                updateServerTime("_srvtime", p_Response);
                for (int i = 0; i < m_Player.Towns.Count; i++)
                {
                    m_Player.Towns[i].ServerTime = m_ServerTime;
                }
                addNotifications(p_Response);

                if (!m_TownListInitialized)//set to false at reconnect
                {
                    m_TownListInitialized = true;
                    //Finish login sequence
                    CustomArgs l_CustomArgs = new CustomArgs("");
                    townListUpdated(this, l_CustomArgs);
                }
                else
                {
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("updateGameDataResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to switch to the next town.
         */ 
        public void switchTown()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            //Get town index
            while (m_CurrentTownIntern < m_Player.Towns.Count)
            {
                if (m_Player.Towns[m_CurrentTownIntern].HasConqueror.Equals("false"))
                    break;
                else
                {
                    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Has conqueror.");
                    m_CurrentTownIntern++;
                }
            }

            if (m_CurrentTownIntern < m_Player.Towns.Count)
            {
                if(l_Settings.FarmerRandomize)
                    m_Player.Towns[m_CurrentTownIntern].randomizeFarmers();

                string l_ServerTime = "0";//Length must be 13
                string l_Action = "switch_town";
                string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
                //string l_Json = "{\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}";
                string l_Json = "{\"town_id\":" + l_Townid + ",\"nl_init\":true}";
                l_Json = Uri.EscapeDataString(l_Json);
                l_ServerTime = m_ServerTime;
                if (l_ServerTime.Length > 0)
                {
                    while (l_ServerTime.Length < 13)
                        l_ServerTime = l_ServerTime + "0";
                }

                try
                {
                    m_State = "switchtown";
                    setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Switching town");

                    string l_Url = "http://" + l_Settings.GenServer + "/game/index?action=" + l_Action + "&town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                    Uri l_Uri = new Uri(l_Url);

                    m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    m_HttpHandler.DownloadStringAsync(l_Uri);
                    m_HttpHandler.Headers.Remove("X-Requested-With");
                }
                catch (Exception e)
                {
                    startReconnect(0);
                    if (l_Settings.AdvDebugMode)
                        l_IOHandler.debug("Exception in switchTown(): " + e.Message);
                }
            }
            else
            {
                final();
            }
        }

        /*
         * Handles the server response of switchTown().
         */
        private void switchTownResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                string l_Search = "";
                string l_Error = "";
                int l_Index = -1;
                int l_Index2 = -1;
                bool l_Add = true;

                int l_ValidCode = validateResponse(p_Response, "switchTownResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        l_Search = "\"incoming_attacks_total\\\":";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        string l_Incoming_attacks_total = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.IncomingAttacks = int.Parse(l_Incoming_attacks_total);

                        //Extract Movement Information
                        string l_MovType = "";
                        string l_MovCancelable = "";
                        string l_MovStarted_at = "";
                        string l_MovArrival_at = "";
                        string l_MovArrival_eta = "";
                        //string l_MovArrival_seconds_left = "";
                        string l_MovArrived_human = "";
                        string l_MovId = "";
                        string l_MovIncoming = "";
                        string l_MovIncoming_attack = "";//Only when l_MovIncoming is true
                        string l_MovCommand_name = "";

                        string l_MovTId = "";
                        string l_MovTName = "";
                        string l_MovTargetTownEncrypted = "";
                        string l_MovTargetTownDecrypted = "";
                        //string l_MovTName_short = "";
                        //string l_MovTPlayer_id = "";
                        //string l_MovTTown_type = "";
                        //string l_MovTTownLink = "";

                        //Delete old movement info
                        m_Player.Towns[m_CurrentTownIntern].Movements.Clear();

                        l_Search = "\\\"unit_movements\\\":[{";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        if (l_Index != -1)
                        {
                            string l_ResponseMov = p_Response.Substring(l_Index + l_Search.Length - 1, p_Response.IndexOf("}]", l_Index) - (l_Index + l_Search.Length - 1));
                            l_Search = "{\\\"type\\\":\\\"";
                            l_Index = l_ResponseMov.IndexOf(l_Search, 0);
                            while (l_Index != -1)
                            {
                                l_MovType = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf("\\\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_Search = "\\\"cancelable\\\":";
                                l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                l_MovCancelable = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_Search = "\\\"started_at\\\":";
                                l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                l_MovStarted_at = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_Search = "\\\"arrival_at\\\":";
                                l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                l_MovArrival_at = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                //l_Search = "\"arrival_eta\":";
                                //l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                //l_MovArrival_eta = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                //l_Search = "\"arrival_seconds_left\":";
                                //l_Index = p_Response.IndexOf(l_Search, l_Index);
                                //l_MovArrival_seconds_left = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_Search = "\\\"arrived_human\\\":\\\"";
                                l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                l_MovArrived_human = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_MovArrived_human = l_Parser.fixSpecialCharacters(l_MovArrived_human);
                                l_Search = "\\\"id\\\":";
                                l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                l_MovId = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_Search = "\\\"incoming\\\":";
                                l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                l_MovIncoming = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                if (l_MovIncoming.Equals("true"))
                                {
                                    l_Search = "\\\"incoming_attack\\\":";
                                    l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                    l_MovIncoming_attack = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                }
                                else
                                {
                                    l_MovIncoming_attack = "";
                                }
                                l_Search = "\\\"command_name\\\":\\\"";
                                l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                l_MovCommand_name = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_MovCommand_name = l_Parser.fixSpecialCharacters(l_MovCommand_name);

                                //Movement target town
                                if (l_MovIncoming_attack.Equals("true"))
                                {
                                    l_Search = "\\\"town\\\":{";
                                    l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                    string l_TownData = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf("}", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                    if (l_TownData.Contains("#"))
                                    {
                                        l_Search = "#";
                                        l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                                        l_MovTargetTownEncrypted = l_ResponseMov.Substring(l_Index + l_Search.Length, l_ResponseMov.IndexOf("\\", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                        l_MovTargetTownDecrypted = l_Parser.decryptLink(l_MovTargetTownEncrypted);
                                        if (!l_MovTargetTownDecrypted.Equals("false"))
                                        {
                                            l_Search = "\"id\":";
                                            l_Index2 = l_MovTargetTownDecrypted.IndexOf(l_Search, 0);
                                            l_MovTId = l_MovTargetTownDecrypted.Substring(l_Index2 + l_Search.Length, l_MovTargetTownDecrypted.IndexOf(",", l_Index2 + l_Search.Length) - (l_Index2 + l_Search.Length));
                                            l_Search = "\"name\":\"";
                                            l_Index2 = l_MovTargetTownDecrypted.IndexOf(l_Search, l_Index2);
                                            l_MovTName = l_MovTargetTownDecrypted.Substring(l_Index2 + l_Search.Length, l_MovTargetTownDecrypted.IndexOf("\"", l_Index2 + l_Search.Length) - (l_Index2 + l_Search.Length));
                                            l_MovTName = l_Parser.fixSpecialCharacters(l_MovTName);
                                        }
                                        else
                                        {
                                            //Decryptlink failed
                                            l_Add = false;
                                        }
                                    }
                                    else
                                    {
                                        //Incoming Quest attack
                                        l_Add = false;
                                    }
                                }

                                //Add
                                //m_Player.Towns[m_CurrentTownIntern].Movements.Add(new Movement(l_MovType, l_MovCancelable.Equals("true"),l_MovStarted_at, l_MovArrival_at, l_MovArrival_eta, l_MovArrival_seconds_left, l_MovArrived_human, l_MovId, l_MovIncoming.Equals(true),l_MovIncoming_attack.Equals("true"),l_MovCommand_name));
                                if (l_Add)
                                {
                                    m_Player.Towns[m_CurrentTownIntern].Movements.Add(new Movement(l_MovType, l_MovCancelable.Equals("true"), l_MovStarted_at, l_MovArrival_at, l_MovArrival_eta, l_MovArrived_human, l_MovId, l_MovIncoming.Equals("true"), l_MovIncoming_attack.Equals("true"), l_MovCommand_name));
                                    m_Player.Towns[m_CurrentTownIntern].Movements[m_Player.Towns[m_CurrentTownIntern].Movements.Count - 1].addTOIInfo(l_MovTId, l_MovTName);
                                }
                                else
                                {
                                    l_Add = true;
                                }

                                //Search next
                                l_Search = "{\\\"type\\\":\\\"";
                                l_Index = l_ResponseMov.IndexOf(l_Search, l_Index);
                            }
                        }

                        //Server time
                        updateServerTime("_srvtime", p_Response);
                        addNotifications(p_Response);
                        
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in switchTownResponse(): " + l_Error);
                        }
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("switchTownResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * This server request is triggered in a real browser when switching towns.
         * It only purpose in this bot --WAS-- to better emulate a real browser.
         * Now that updateGameData can only be called once this method is used to update building information.
         */ 
        private void buildingBuildData()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_Action = "execute";

            try
            {
                m_State = "buildingBuildData";

                //http://***.grepolis.com/game/frontend_bridge?town_id=***&action=execute&h=***
                NameValueCollection l_Content = new NameValueCollection();
                string l_Url = "http://" + l_Settings.GenServer + "/game/frontend_bridge?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H;
                Uri l_Uri = new Uri(l_Url);
                //l_Content.Add("json", "{\"model_url\":\"BuildingBuildData/" + m_Player.Towns[m_CurrentTownIntern].TownID + "\",\"action_name\":\"forceUpdate\",\"arguments\":null,\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nlreq_id\":" + m_Nlreq_id + "}");
                l_Content.Add("json", "{\"model_url\":\"BuildingBuildData/" + m_Player.Towns[m_CurrentTownIntern].TownID + "\",\"action_name\":\"forceUpdate\",\"arguments\":null,\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");

                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                m_HttpHandler.Headers.Remove("X-Requested-With");
            }
            catch(Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in buildingBuildData(): " + e.Message);
            }
        }

        /*
         * Handles the server response of buildingBuildData().
         */
        private void buildingBuildDataResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                int l_Index = -1;
                int l_IndexEnd = -1;
                string l_Search = "";
                string l_Error = "";
                string l_IsBuildingOrderQueueFull = "";

                string l_Upgradable = "";
                //string l_UpgradableReduced = "";//Gold spender to build for 25% less resources
                string l_Teardownable = "";
                string l_DevName = "";
                string l_HasMaxLevel = "";
                int l_DevNameIndex = -1;
                int l_Level = 0;
                string l_LevelString = "";
                int l_NextLevel = 0;
                int l_TearDownLevel = 0;

                int l_ValidCode = validateResponse(p_Response, "buildingBuildDataResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        l_IndexEnd = p_Response.IndexOf("}}}}\"");
                        l_Search = "is_building_order_queue_full\\\":";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_IsBuildingOrderQueueFull = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        m_Player.Towns[m_CurrentTownIntern].IsBuildingOrderQueueFull = l_IsBuildingOrderQueueFull.Equals("true");

                        l_Search = "building_data\\\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index) + l_Search.Length;

                        l_Search = "\\\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        while (l_Index < l_IndexEnd && l_Index != -1)
                        {
                            l_DevName = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "can_upgrade\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Upgradable = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "can_tear_down\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Teardownable = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "has_max_level\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_HasMaxLevel = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\\\"level\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_LevelString = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            if (l_LevelString.Contains("-"))
                                l_Level = 0;
                            else
                                l_Level = int.Parse(l_LevelString);
                            l_Search = "next_level\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_NextLevel = int.Parse(p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length)));
                            l_Search = "tear_down_level\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_TearDownLevel = int.Parse(p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length)));

                            //Update building
                            if (!l_DevName.Equals("place"))
                            {
                                l_DevNameIndex = m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(l_DevName);
                                m_Player.Towns[m_CurrentTownIntern].Buildings[l_DevNameIndex].Level = l_Level;
                                m_Player.Towns[m_CurrentTownIntern].Buildings[l_DevNameIndex].NextLevel = l_NextLevel;
                                m_Player.Towns[m_CurrentTownIntern].Buildings[l_DevNameIndex].TearDownLevel = l_TearDownLevel;
                                m_Player.Towns[m_CurrentTownIntern].Buildings[l_DevNameIndex].IsMaxLevel = l_HasMaxLevel.Equals("true");
                                m_Player.Towns[m_CurrentTownIntern].Buildings[l_DevNameIndex].Upgradable = l_Upgradable.Equals("true");
                                m_Player.Towns[m_CurrentTownIntern].Buildings[l_DevNameIndex].Teardownable = l_Teardownable.Equals("true");
                            }

                            //Search Next
                            //In case of bug/bad performance. Good alternative is: cyle through each building using m_Buildings[i].DevName. By searching the response for the building devname.
                            l_Search = "group_locked\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index) + l_Search.Length;
                            l_Search = "\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                        }

                        //Update storage info
                        //l_DevNameIndex = m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("storage");
                        //l_Level = m_Player.Towns[m_CurrentTownIntern].Buildings[l_DevNameIndex].Level;
                        //double l_HideFactor = 100;
                        //double l_StorageFactor = 200;
                        //double l_StoragePow = 1.35399;
                        //m_Player.Towns[m_CurrentTownIntern].Storage = (int)(l_Level * l_HideFactor + Math.Pow(l_Level, l_StoragePow) * l_StorageFactor);

                        //Server time
                        updateServerTime("_srvtime", p_Response);
                        addNotifications(p_Response);

                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in buildingBuildDataResponse(): " + l_Error);
                        }
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("buildingBuildDataResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to locate and update the farmers of a single town.
         */ 
        public void locateFarmers()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "get_chunks";
            //string l_Json = "{\"chunks\":[{\"x\":" + m_Player.Towns[m_CurrentTownIntern].getChunkX() + ",\"y\":" + m_Player.Towns[m_CurrentTownIntern].getChunkY() + ",\"timestamp\":0}],\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nlreq_id\":" + m_Nlreq_id + "}";
            string l_Json = "{\"chunks\":[{\"x\":" + m_Player.Towns[m_CurrentTownIntern].getChunkX() + ",\"y\":" + m_Player.Towns[m_CurrentTownIntern].getChunkY() + ",\"timestamp\":0}],\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            l_Json = Uri.EscapeDataString(l_Json);
            l_ServerTime = m_ServerTime;
            if (l_ServerTime.Length > 0)
            {
                while (l_ServerTime.Length < 13)
                    l_ServerTime = l_ServerTime + "0";
            }

            try
            {
                m_State = "locatefarmers";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Locating farmers");
                //http://***.grepolis.com/game/map_data?town_id=***&action=get_chunks&h=***&json=***&_=1424448249018
                string l_Url = "http://" + l_Settings.GenServer + "/game/map_data?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.DownloadStringAsync(l_Uri);
                m_HttpHandler.Headers.Remove("X-Requested-With");
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in locateFarmers(): " + e.Message);
            }
        }

        /*
         * Handles the server response of locateFarmers().
         */
        private void locateFarmersResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                //{"id":####,"name":"********","dir":"n","expansion_stage":4,"x":###,"y":###,"ox":###,"oy":###,"offer":"stone","demand":"iron","mood":14,"relation_status":1,"ratio":1.25,"loot":1303078581,"lootable_human":"tomorrow at 12:16 AM ","looted":1303042581}
                int l_Index = -1;
                int l_Index2 = -1;
                string l_Search = "";
                string l_ID = "";
                string l_Name = "";
                string l_IslandX = "";
                string l_IslandY = "";
                string l_Mood = "";
                string l_RelationStatus = "";
                string l_LootTimer = "";
                string l_LootTimerHuman = "";
                string l_ExpansionState = "";

                List<string> l_ProcessedFarmers = new List<string>();

                int l_ValidCode = validateResponse(p_Response, "locateFarmersResponse");
                if (l_ValidCode == 1)
                {
                    //Get island quest data
                    m_Player.Towns[m_CurrentTownIntern].AvailableQuests = new Regex("island_quest_base_name").Matches(p_Response).Count;

                    //Get farmer data
                    l_Search = "\"relation_status\":";
                    l_Index = p_Response.IndexOf(l_Search, 0);

                    while (l_Index != -1)
                    {
                        while (!p_Response[l_Index].Equals('{'))
                            l_Index--;
                        l_Search = "\"id\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        //Get farmers id
                        l_ID = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Search = "\"name\":\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        //Get farmers name
                        l_Name = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Name = l_Parser.fixSpecialCharacters(l_Name);
                        l_Search = "\"expansion_stage\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        //Get expansion state
                        l_ExpansionState = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Search = "\"x\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        //Get farmers island x coord
                        l_IslandX = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Search = "\"y\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        //Get farmers island y coord
                        l_IslandY = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Search = "\"mood\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        //Get farmers mood
                        l_Mood = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Search = "\"relation_status\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        //Get farmers relation status
                        l_RelationStatus = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        if (l_RelationStatus.Equals("1"))
                        {
                            l_Search = "\"loot\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            //Get farmers loot time
                            l_LootTimer = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"lootable_human\":\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            //Get farmers loot time human
                            l_LootTimerHuman = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_LootTimerHuman = l_Parser.fixSpecialCharacters(l_LootTimerHuman);
                        }
                        else
                        {
                            l_LootTimer = "0";
                            l_LootTimerHuman = "Not available";
                        }

                        //Add or update farmer
                        if (m_Player.Towns[m_CurrentTownIntern].IslandX.Equals(l_IslandX) && m_Player.Towns[m_CurrentTownIntern].IslandY.Equals(l_IslandY))
                        {
                            if (m_Player.Towns[m_CurrentTownIntern].isUniqueFarm(l_ID))
                            {
                                //m_Player.Towns[m_CurrentTownIntern].Farmers.Add(new Farmer(l_ID, l_Name + " (" + l_ID + ")", l_ExpansionState, l_IslandX, l_IslandY, l_Mood, l_RelationStatus, l_LootTimer, l_LootTimerHuman));
                                m_Player.Towns[m_CurrentTownIntern].Farmers.Add(new Farmer(l_ID, l_Name, l_ExpansionState, l_IslandX, l_IslandY, l_Mood, l_RelationStatus, l_LootTimer, l_LootTimerHuman));
                            }
                            else
                            {
                                l_Index2 = m_Player.Towns[m_CurrentTownIntern].getFarmersIndex(l_ID);
                                m_Player.Towns[m_CurrentTownIntern].Farmers[l_Index2].ExpansionState = int.Parse(l_ExpansionState);
                                m_Player.Towns[m_CurrentTownIntern].Farmers[l_Index2].Mood = int.Parse(l_Mood);
                                m_Player.Towns[m_CurrentTownIntern].Farmers[l_Index2].RelationStatus = l_RelationStatus.Equals("1");
                                m_Player.Towns[m_CurrentTownIntern].Farmers[l_Index2].LootTimer = l_LootTimer;
                                m_Player.Towns[m_CurrentTownIntern].Farmers[l_Index2].LootTimerHuman = l_LootTimerHuman;
                            }
                        }

                        //Search next farmer
                        l_Search = "\"relation_status\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                    }
                    //Sort farmers
                    //m_Player.Towns[m_CurrentTownIntern].Farmers.Sort((x, y) => string.Compare(x.Name, y.Name));
                    m_Player.Towns[m_CurrentTownIntern].Farmers.Sort((x, y) => string.Compare(x.ID, y.ID));

                    //Server time
                    updateServerTime("_srvtime", p_Response);

                    stateManagerDelay();
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("locateFarmersResponse", e.Message + "\n" + p_Response);
                
                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to update information about the state of cultural festivals.
         */ 
        private void updateCulturalInfo()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "culture";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            //string l_Json = "{\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}";
            string l_Json = "{\"town_id\":" + l_Townid + ",\"nl_init\":true}";
            l_Json = Uri.EscapeDataString(l_Json);
            l_ServerTime = m_ServerTime;
            if (l_ServerTime.Length > 0)
            {
                while (l_ServerTime.Length < 13)
                    l_ServerTime = l_ServerTime + "0";
            }

            try
            {
                m_State = "updateculturalinfo";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Collecting cultural data");
                //http://***.grepolis.com/game/building_place?town_id=***&action=culture&h=***&json=***&_=***
                string l_Url = "http://" + l_Settings.GenServer + "/game/building_place?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.DownloadStringAsync(l_Uri);
                m_HttpHandler.Headers.Remove("X-Requested-With");
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateCulturalInfo(): " + e.Message);
            }
        }

        /*
         * Handles the server response of updateCulturalInfo().
         */
        private void updateCulturalInfoResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                string l_Search = "";
                string l_Error = "";
                string l_CulturalLevelStr = "";
                string l_CulturalCitiesStr = "";
                int l_Index = -1;

                int l_ValidCode = validateResponse(p_Response, "updateCulturalInfoResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        //Update cultural stats
                        //l_Search = "<div id=\"place_culture_level\">"; //2.37
                        l_Search = "<div id=\\\"place_culture_level\\\">";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_CulturalLevelStr = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_CulturalLevelStr = l_Parser.fixSpecialCharacters(l_CulturalLevelStr);

                        m_Player.CulturalLevelStr = l_CulturalLevelStr;
                        //l_Search = "<div id=\"place_culture_towns\">"; //2.37
                        l_Search = "<div id=\\\"place_culture_towns\\\">";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_CulturalCitiesStr = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_CulturalCitiesStr = l_Parser.fixSpecialCharacters(l_CulturalCitiesStr);

                        m_Player.CulturalCitiesStr = l_CulturalCitiesStr;
                        //l_Search = "<div id=\"place_culture_count\">"; //2.37
                        l_Search = "<div id=\\\"place_culture_count\\\">";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_Search = "\\/>";
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        m_Player.CulturalPointsStr = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<\\/div>", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        string l_CulturalPointsCurrent = m_Player.CulturalPointsStr.Split('/')[0];
                        string CulturalPointsMax = m_Player.CulturalPointsStr.Split('/')[1];
                        l_CulturalPointsCurrent = String.Join("", Regex.Split(l_CulturalPointsCurrent, "[^\\d]"));
                        CulturalPointsMax = String.Join("", Regex.Split(CulturalPointsMax, "[^\\d]"));
                        m_Player.CulturalPointsStr = l_CulturalPointsCurrent + "/" + CulturalPointsMax;
                        m_Player.CulturalPointsCurrent = int.Parse(l_CulturalPointsCurrent);
                        m_Player.CulturalPointsMax = int.Parse(CulturalPointsMax);

                        //Update cultural festivals
                        int l_IndexParty = -1;
                        int l_IndexGames = -1;
                        int l_IndexTriumph = -1;
                        int l_IndexTheater = -1;
                        string l_SubString = "";

                        l_IndexParty = p_Response.IndexOf("party.jpg");
                        if (l_IndexParty == -1)
                            l_IndexParty = p_Response.IndexOf("birthday.png");
                        if (l_IndexParty == -1)
                            l_IndexParty = p_Response.IndexOf("birthday.jpg");
                        if (l_IndexParty == -1)
                            l_IndexParty = p_Response.IndexOf("xmas_party_new.jpg");
                        if (l_IndexParty == -1)
                            l_IndexParty = p_Response.IndexOf(".jpg");

                        l_IndexGames = p_Response.IndexOf("bread.jpg");
                        l_IndexTriumph = p_Response.IndexOf("triumph.jpg");
                        l_IndexTheater = p_Response.IndexOf("theater.jpg");

                        //Get local names
                        //l_Search = "<div class=\"game_header bold\">"; //2.37
                        l_Search = "<div class=\\\"game_header bold\\\">";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[0].NameLocal = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //l_Search = "<div class=\"game_header bold\">"; //2.37
                        l_Search = "<div class=\\\"game_header bold\\\">";
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[1].NameLocal = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //l_Search = "<div class=\"game_header bold\">"; //2.37
                        l_Search = "<div class=\\\"game_header bold\\\">";
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[2].NameLocal = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        //l_Search = "<div class=\"game_header bold\">"; //2.37
                        l_Search = "<div class=\\\"game_header bold\\\">";
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[3].NameLocal = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                        //Check requirements
                        l_SubString = p_Response.Substring(l_IndexParty, l_IndexGames - l_IndexParty);
                        //m_Player.Towns[m_CurrentTownIntern].CulturalEvents[0].Ready = l_SubString.Contains("onclick=\"BuildingPlace.startCelebration('"); //2.37
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[0].Ready = l_SubString.Contains("onclick=\\\"BuildingPlace.startCelebration('");
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[0].EnoughResources = !l_SubString.Contains("place_not_enough_resources");
                        /*if (m_Player.Towns[m_CurrentTownIntern].CulturalEvents[0].EnoughResources)
                        {
                            l_Index = l_IndexParty;
                            //l_Search = "<td class=\"\">"; //2.37
                            l_Search = "<td class=\\\"\\\">";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Wood = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            //l_Search = "<td class=\"\">"; //2.37
                            l_Search = "<td class=\\\"\\\">";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                            l_Stone = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            //l_Search = "<td class=\"\">"; //2.37
                            l_Search = "<td class=\\\"\\\">";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                            l_Iron = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Wood = String.Join("", Regex.Split(l_Wood, "[^\\d]"));
                            l_Stone = String.Join("", Regex.Split(l_Stone, "[^\\d]"));
                            l_Iron = String.Join("", Regex.Split(l_Iron, "[^\\d]"));
                            m_Player.Towns[m_CurrentTownIntern].CulturalEvents[0].EnoughResources = int.Parse(l_Wood) <= m_Player.Towns[m_CurrentTownIntern].Wood && int.Parse(l_Stone) <= m_Player.Towns[m_CurrentTownIntern].Stone && int.Parse(l_Iron) <= m_Player.Towns[m_CurrentTownIntern].Iron;
                        }*/
                        l_SubString = p_Response.Substring(l_IndexGames, l_IndexTriumph - l_IndexGames);
                        //m_Player.Towns[m_CurrentTownIntern].CulturalEvents[1].Ready = l_SubString.Contains("onclick=\"BuildingPlace.startCelebration('games'"); //2.37
                        //m_Player.Towns[m_CurrentTownIntern].CulturalEvents[1].Ready = l_SubString.Contains("onclick=\\\"BuildingPlace.startCelebration('games'"); //2.66?
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[1].Ready = l_SubString.Contains("btn_organize_olympic_games");
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[1].EnoughResources = m_Player.Gold >= 50;
                        l_SubString = p_Response.Substring(l_IndexTriumph, l_IndexTheater - l_IndexTriumph);
                        //m_Player.Towns[m_CurrentTownIntern].CulturalEvents[2].Ready = l_SubString.Contains("onclick=\"BuildingPlace.startCelebration('triumph'"); //2.37
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[2].Ready = l_SubString.Contains("onclick=\\\"BuildingPlace.startCelebration('triumph'");
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[2].EnoughResources = !l_SubString.Contains("place_not_enough_resources");
                        l_SubString = p_Response.Substring(l_IndexTheater, p_Response.Length - l_IndexTheater);
                        //m_Player.Towns[m_CurrentTownIntern].CulturalEvents[3].Ready = l_SubString.Contains("onclick=\"BuildingPlace.startCelebration('theater'"); //2.37
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[3].Ready = l_SubString.Contains("onclick=\\\"BuildingPlace.startCelebration('theater'");
                        m_Player.Towns[m_CurrentTownIntern].CulturalEvents[3].EnoughResources = !l_SubString.Contains("place_not_enough_resources");
                        /*if (m_Player.Towns[m_CurrentTownIntern].CulturalEvents[3].EnoughResources)
                        {
                            l_Index = l_IndexTheater;
                            //l_Search = "<td class=\"\">"; //2.37
                            l_Search = "<td class=\\\"\\\">";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Wood = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            //l_Search = "<td class=\"\">"; //2.37
                            l_Search = "<td class=\\\"\\\">";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                            l_Stone = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            //l_Search = "<td class=\"\">"; //2.37
                            l_Search = "<td class=\\\"\\\">";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                            l_Iron = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Wood = String.Join("", Regex.Split(l_Wood, "[^\\d]"));
                            l_Stone = String.Join("", Regex.Split(l_Stone, "[^\\d]"));
                            l_Iron = String.Join("", Regex.Split(l_Iron, "[^\\d]"));
                            m_Player.Towns[m_CurrentTownIntern].CulturalEvents[3].EnoughResources = int.Parse(l_Wood) <= m_Player.Towns[m_CurrentTownIntern].Wood && int.Parse(l_Stone) <= m_Player.Towns[m_CurrentTownIntern].Stone && int.Parse(l_Iron) <= m_Player.Towns[m_CurrentTownIntern].Iron;
                        }*/

                        //Server time
                        updateServerTime("_srvtime", p_Response);
                        addNotifications(p_Response);
                        
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in updateCulturalInfoResponse(): " + l_Error);
                        }
                    }
                    stateManagerDelay();
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("updateCulturalInfoResponse", e.Message + "\n" + p_Response);
                
                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to start a cultural festival
         */ 
        private void checkCultureFestivals()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_CelebrationType = "";

            try
            {
                m_State = "checkculturefestivals";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Checking cultural festivals.");

                if (l_Settings.MasterCulture && m_Player.Towns[m_CurrentTownIntern].CulturalFestivalsEnabled)
                {
                    if (m_Player.Towns[m_CurrentTownIntern].CulturalTheaterEnabled && m_Player.Towns[m_CurrentTownIntern].CulturalEvents[3].Ready && m_Player.Towns[m_CurrentTownIntern].CulturalEvents[3].EnoughResources)
                    {
                        l_CelebrationType = "theater";
                    }
                    else if (m_Player.Towns[m_CurrentTownIntern].CulturalPartyEnabled && m_Player.Towns[m_CurrentTownIntern].CulturalEvents[0].Ready && m_Player.Towns[m_CurrentTownIntern].CulturalEvents[0].EnoughResources)
                    {
                        l_CelebrationType = "party";
                    }
                    else if (m_Player.Towns[m_CurrentTownIntern].CulturalTriumphEnabled && m_Player.Towns[m_CurrentTownIntern].CulturalEvents[2].Ready && m_Player.Towns[m_CurrentTownIntern].CulturalEvents[2].EnoughResources)
                    {
                        l_CelebrationType = "triumph";
                    }
                    else if (m_Player.Towns[m_CurrentTownIntern].CulturalGamesEnabled && m_Player.Towns[m_CurrentTownIntern].CulturalEvents[1].Ready && m_Player.Towns[m_CurrentTownIntern].CulturalEvents[1].EnoughResources && l_Settings.PreUseGold)
                    {
                        l_CelebrationType = "games";
                    }

                    if (!l_CelebrationType.Equals(""))
                    {
                        m_CulturalFestivalStarted = true;
                        //http://###.grepolis.com/game/building_place?town_id=###&action=start_celebration&h=###########
                        NameValueCollection l_Content = new NameValueCollection();
                        string l_Url = "http://" + l_Settings.GenServer + "/game/building_place?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=start_celebration&h=" + m_H;
                        Uri l_Uri = new Uri(l_Url);
                        //{"celebration_type":"l_CelebrationType","town_id":"#####","nl_init":true} l_CelebrationType = party/games/triumph/theater
                        l_Content.Add("json", "{\"celebration_type\":\"" + l_CelebrationType + "\",\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");

                        m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                        m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                        m_HttpHandler.Headers.Remove("X-Requested-With");
                    }
                    else
                    {
                        m_CulturalFestivalStarted = false;
                        stateManagerDelay();
                    }
                }
                else
                {
                    m_CulturalFestivalStarted = false;
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkCultureFestivals(): " + e.Message);
            }
        }

        /*
         * Handles the server response of checkCultureFestivals().
         */
        private void checkCultureFestivalsResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                string l_Search = "";
                int l_Index = -1;
                string l_Error = "";

                int l_ValidCode = validateResponse(p_Response, "checkCultureFestivalsResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        //TODO: Add check for gold too low warning when a festival has been started. (If the warning still exists...)
                        //if (!p_Response.Contains("{\"enough_gold\":false"))//Check doesn't work anymore since 2.48
                        //{
                            updateResourcesFromNotification(p_Response, false);

                            //Server time
                            updateServerTime("_srvtime", p_Response);

                            logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Cultural event started");

                            stateManagerDelay();
                        //}
                        //else
                        //{
                        //    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Not enough gold.");
                        //    l_Settings.PreUseGold = false;
                        //    stateManagerDelay();
                        //}
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkCultureFestivalsResponse(): " + l_Error);
                        }
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkCultureFestivalsResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to check if the militia is ready.
         */ 
        private void updateMilitia()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "index";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            //string l_Json = "{\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}";
            string l_Json = "{\"town_id\":" + l_Townid + ",\"nl_init\":true}";
            l_Json = Uri.EscapeDataString(l_Json);
            l_ServerTime = m_ServerTime;
            if (l_ServerTime.Length > 0)
            {
                while (l_ServerTime.Length < 13)
                    l_ServerTime = l_ServerTime + "0";
            }

            try
            {
                m_State = "updatemilitia";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Collecting militia data");

                string l_Url = "http://" + l_Settings.GenServer + "/game/building_farm?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.DownloadStringAsync(l_Uri);
                m_HttpHandler.Headers.Remove("X-Requested-With");
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in updateMilitia(): " + e.Message);
            }
        }

        /*
         * Handles the server response of updateMilitia().
         */
        private void updateMilitiaResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            int l_Index = 0;
            string l_Search = "onclick=";
            string l_Error = "";
            string l_Substring = "";

            try
            {
                int l_ValidCode = validateResponse(p_Response, "updateMilitiaResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_Substring = p_Response.Substring(l_Index, p_Response.IndexOf(";", l_Index) - l_Index);
                        m_Player.Towns[m_CurrentTownIntern].MilitiaReady = !l_Substring.Contains("none");

                        addNotifications(p_Response);
                        
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in updateMilitiaResponse(): " + l_Error);
                        }
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("updateMilitiaResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to activate the militia when needed.
         */ 
        private void checkMilitia()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                m_State = "checkmilitia";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Checking militia");

                if (Player.Towns[m_CurrentTownIntern].MilitiaTrigger > 0 && Player.Towns[m_CurrentTownIntern].MilitiaReady && Player.Towns[m_CurrentTownIntern].isMilitiaNeeded(ServerTime))
                {
                    //http://***.grepolis.com/game/building_farm?town_id=***&action=request_militia&h=***
                    NameValueCollection l_Content = new NameValueCollection();
                    string l_Url = "http://" + l_Settings.GenServer + "/game/building_farm?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=request_militia&h=" + m_H;
                    Uri l_Uri = new Uri(l_Url);
                    //l_Content.Add("json", "{\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nlreq_id\":" + m_Nlreq_id + "}");//json={"town_id":"#####","nlreq_id":#####}
                    l_Content.Add("json", "{\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");

                    m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                    m_HttpHandler.Headers.Remove("X-Requested-With");
                }
                else
                {
                    //This method is called when the militia is not needed/ready
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkMilitia(): " + e.Message);
            }
        }

        /*
         * Handles the server response of checkMilitia().
         */
        private void checkMilitiaResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            int l_Index = 0;
            string l_Search = "";
            string l_Error = "";

            try
            {
                int l_ValidCode = validateResponse(p_Response, "checkMilitiaResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        if (p_Response.Contains("{\"success\":"))
                            logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Militia enlisted.");

                        updateResourcesFromNotification(p_Response, false);

                        addNotifications(p_Response);
                        
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkMilitiaResponse(): " + l_Error);
                        }
                        stateManagerDelay();
                    }

                    
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkMilitiaResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request that simulates opening loot/demand window of a farmer.
         */ 
        private void attackFarmers1()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            int l_FarmersIndex = -1;
            bool l_attackFarmersComplete = true;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "claim_info";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            string l_Json = "";
            string l_Url = "";
            int[] l_FarmerData = { 0, 0 };

            try
            {
                m_State = "attackfarmers1";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Demanding resources from farmers (1/2)");
                //http://###.grepolis.com/game/farm_town_info?action=claim_info&town_id=#####&h=########### (friendly)
                //http://###.grepolis.com/game/farm_town_info?action=pillage_info&town_id=#####&h=########### (unfriendly)

                if (!m_CanIModifyFarmers)//Check if farmer settings are loaded.
                {
                    //logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Loading farmer settings.");
                    m_Player.loadFarmersSettings(l_Townid);
                }
                if (l_Settings.FarmerScheduler.Split(';')[DateTime.Now.Hour].Equals("True"))//Check schedule
                {
                    if (l_Settings.MasterFarmers && m_Player.Towns[m_CurrentTownIntern].FarmersLootEnabled && m_Player.Towns[m_CurrentTownIntern].Farmers.Count > 0)//Check if current town has farmers on his Island
                    {
                        if (!m_Player.Towns[m_CurrentTownIntern].isStorageFull())//Check if storage is full
                        {
                            l_FarmerData = m_Player.Towns[m_CurrentTownIntern].getLootableFarmer(ServerTime, m_CurrentFarmIntern);
                            l_FarmersIndex = l_FarmerData[0];
                            if (l_FarmersIndex != -1)//Check if there is a farmer that we can loot
                            {
                                m_CurrentFarmIntern = l_FarmerData[1];
                                l_attackFarmersComplete = false;

                                if (m_Player.Towns[m_CurrentTownIntern].Farmers[l_FarmersIndex].Mood >= m_Player.Towns[m_CurrentTownIntern].FarmersMinMood && !m_Player.Towns[m_CurrentTownIntern].FarmersFriendlyDemandsOnly)//Check if the type of the loot is non-friendly
                                {
                                    l_Action = "pillage_info";
                                }
                                else//Type of loot is friendly
                                {
                                    l_Action = "claim_info";
                                }

                                //l_Json = "{\"id\":\"" + m_Player.Towns[m_CurrentTownIntern].Farmers[l_FarmersIndex].ID + "\",\"town_id\":\"" + l_Townid + "\",\"nlreq_id\":" + m_Nlreq_id + "}";
                                l_Json = "{\"id\":\"" + m_Player.Towns[m_CurrentTownIntern].Farmers[l_FarmersIndex].ID + "\",\"town_id\":" + l_Townid + ",\"nl_init\":true}";
                                l_Json = Uri.EscapeDataString(l_Json);
                                l_ServerTime = m_ServerTime;
                                if (l_ServerTime.Length > 0)
                                {
                                    while (l_ServerTime.Length < 13)
                                        l_ServerTime = l_ServerTime + "0";
                                }

                                l_Url = "http://" + l_Settings.GenServer + "/game/farm_town_info?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                                Uri l_Uri = new Uri(l_Url);

                                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                                m_HttpHandler.DownloadStringAsync(l_Uri);
                                m_HttpHandler.Headers.Remove("X-Requested-With");
                            }
                            else
                            {
                                logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": No farmers available at the moment.");
                            }
                        }
                        else
                        {
                            logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are temporarily disabled for this town, your warehouse is full.");
                        }
                    }
                    else
                    {
                        logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are disabled for this town.");
                    }
                }
                else
                {
                    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are disabled for this town by scheduler.");
                }

                if (l_attackFarmersComplete)
                {
                    m_CurrentFarmIntern = 0;
                    m_State = "attackfarmers2";
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in attackFarmers1(): " + e.Message);
            }
        }

        /*
         * Handles the server response of attackFarmers1().
         */
        private void attackFarmers1Response(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            string l_Search = "";
            string l_ID = "";
            string l_Limit = "";
            string l_LimitA = "";
            string l_LimitB = "";
            string l_Error = "";
            int l_Index = 0;
            int l_IndexFarmer = 0;

            try
            {
                int l_ValidCode = validateResponse(p_Response, "attackFarmers1Response");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        //Get id
                        l_Search = "{\\\"id\\\":";
                        l_Index = p_Response.IndexOf(l_Search);
                        l_ID = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("}", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                        //Get limit info
                        l_Search = "<div id=\\\"farmtown_loot\\\">";
                        l_Index = p_Response.IndexOf(l_Search);
                        l_Search = "<div class=";
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        l_Search = ">";
                        l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        l_Limit = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("<", l_Index) - (l_Index + l_Search.Length));
                        l_LimitA = l_Limit.Substring(0, l_Limit.IndexOf("\\"));
                        l_LimitB = l_Limit.Substring(l_Limit.IndexOf("/") + 1, l_Limit.Length - (l_Limit.IndexOf("/") + 1));
                        l_Limit = l_LimitA + "/" + l_LimitB;
                        l_IndexFarmer = m_Player.Towns[m_CurrentTownIntern].getFarmersIndex(l_ID);
                        m_Player.Towns[m_CurrentTownIntern].Farmers[l_IndexFarmer].Limit = l_Limit;

                        //Remove halfs from l_LimitB
                        if (l_LimitB.Contains("."))
                            l_LimitB = l_LimitB.Substring(0, l_LimitB.IndexOf("."));

                        //Set limit
                        if ((int.Parse(l_LimitA) >= int.Parse(l_LimitB)) || (int.Parse(l_LimitA) >= l_Settings.FarmerMaxResources))
                        {
                            m_Player.Towns[m_CurrentTownIntern].Farmers[l_IndexFarmer].FarmersLimitReached = true;
                            m_CurrentFarmIntern++;
                            m_State = "attackfarmersnext";
                        }
                        else
                            m_Player.Towns[m_CurrentTownIntern].Farmers[l_IndexFarmer].FarmersLimitReached = false;

                        addNotifications(p_Response);
                        
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in attackFarmers1Response(): " + l_Error);

                            //Prevent endless loop
                            m_CurrentFarmIntern++;
                        }
                        m_State = "attackfarmersnext";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("attackFarmers1Response", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to loot or demand resources from a farmer.
         */
        private void attackFarmers2()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            bool l_attackFarmersComplete = true;

            try
            {
                m_State = "attackfarmers2";
                int l_CurrentFarmIntern = m_Player.Towns[m_CurrentTownIntern].randomizedFarmer(m_CurrentFarmIntern);
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Demanding resources from farmers (2/2)");
                //http://###.grepolis.com/game/farm_town_info?action=claim_load&town_id=#####&h=###########

                if (l_Settings.FarmerScheduler.Split(';')[DateTime.Now.Hour].Equals("True"))
                {
                    if (m_Player.Towns[m_CurrentTownIntern].Farmers.Count > 0 && m_Player.Towns[m_CurrentTownIntern].FarmersLootEnabled)//Check if current town has farmers on his Island
                    {
                        if (!m_Player.Towns[m_CurrentTownIntern].isStorageFull())//Check if storage is full
                        {
                            l_attackFarmersComplete = false;

                            NameValueCollection l_Content = new NameValueCollection();
                            string l_Url = "http://" + l_Settings.GenServer + "/game/farm_town_info?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=claim_load&h=" + m_H;
                            Uri l_Uri = new Uri(l_Url);

                            if (m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].Mood >= m_Player.Towns[m_CurrentTownIntern].FarmersMinMood && !m_Player.Towns[m_CurrentTownIntern].FarmersFriendlyDemandsOnly)//Check if the type of the loot is non-friendly
                            {
                                //l_Content.Add("json", "{\"target_id\":\"" + m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].ID + "\",\"claim_type\":\"double\",\"time\":" + (int.Parse(m_Player.Towns[m_CurrentTownIntern].FarmersLootInterval) * 60).ToString() + ",\"town_id\":\"" + m_Player.Towns[m_CurrentTownIntern].TownID + "\",\"nlreq_id\":" + Nlreq_id + "}");//json={"target_id":"####","claim_type":"double","time":###,"town_id":"#####","nlreq_id":###} (Hostile)
                                l_Content.Add("json", "{\"target_id\":\"" + m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].ID + "\",\"claim_type\":\"double\",\"time\":" + (int.Parse(m_Player.Towns[m_CurrentTownIntern].FarmersLootInterval) * 60).ToString() + ",\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");
                            }
                            else//Type of loot is friendly
                            {
                                //l_Content.Add("json", "{\"target_id\":\"" + m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].ID + "\",\"claim_type\":\"normal\",\"time\":" + (int.Parse(m_Player.Towns[m_CurrentTownIntern].FarmersLootInterval) * 60).ToString() + ",\"town_id\":\"" + m_Player.Towns[m_CurrentTownIntern].TownID + "\",\"nlreq_id\":" + Nlreq_id + "}");//json={"target_id":"####","claim_type":"normal","time":###,"town_id":"#####","nlreq_id":###} (Friendly)
                                l_Content.Add("json", "{\"target_id\":\"" + m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].ID + "\",\"claim_type\":\"normal\",\"time\":" + (int.Parse(m_Player.Towns[m_CurrentTownIntern].FarmersLootInterval) * 60).ToString() + ",\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");
                            }

                            m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                            m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                            m_HttpHandler.Headers.Remove("X-Requested-With");
                        }
                        else
                        {
                            logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are temporarily disabled for this town, your warehouse is full.");
                        }
                    }
                    else
                    {
                        logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are disabled for this town.");
                    }
                }
                else
                {
                    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are disabled for this town by scheduler.");
                }

                if (l_attackFarmersComplete)
                {
                    m_CurrentFarmIntern = 0;
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in attackFarmers2(): " + e.Message);
            }
        }

        /*
         * Handles the server response of attackFarmers().
         */
        private void attackFarmers2Response(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                string l_Search = "";
                int l_Index = -1;
                string l_RelationStatus = "0";
                string l_LootTimerHuman = "Not available";
                string l_Mood = "0";
                string l_Error = "";
                string l_Looted = "";
                string l_ClaimedResources = "";
                int l_CurrentFarmIntern = m_Player.Towns[m_CurrentTownIntern].randomizedFarmer(m_CurrentFarmIntern);

                int l_ValidCode = validateResponse(p_Response, "attackFarmers2Response");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        l_Search = "{\"relation_status\":";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        l_RelationStatus = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Search = "\"lootable_human\":\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_LootTimerHuman = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_LootTimerHuman = l_Parser.fixSpecialCharacters(l_LootTimerHuman);
                        l_Search = "\"satisfaction\":\"";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_Mood = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        l_Search = "\"claimed_resources_per_resource_type\":";
                        l_Index = p_Response.IndexOf(l_Search, l_Index);
                        l_ClaimedResources = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                        if (p_Response.Contains("FarmTownPlayerRelation"))
                        {
                            l_Search = "\\\"loot\\\":";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Looted = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                        }

                        updateResourcesFromNotification(p_Response, false);

                        //Server time
                        updateServerTime("_srvtime", p_Response);

                        m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].RelationStatus = l_RelationStatus.Equals("1");
                        m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].LootTimerHuman = l_LootTimerHuman;
                        if (l_Mood.Contains("."))
                            l_Mood = l_Mood.Substring(0, l_Mood.IndexOf("."));
                        m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].Mood = int.Parse(l_Mood);
                        m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].LootTimer = (long.Parse(ServerTime) + long.Parse(m_Player.Towns[m_CurrentTownIntern].FarmersLootInterval)).ToString();
                        m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].Looted = l_Looted;

                        logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Successfull looted/demanded " + l_ClaimedResources + " resources from farmer " + m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].Name);

                        addNotifications(p_Response);
                        
                        m_State = "attackfarmersnext";
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in attackFarmers2Response(): " + l_Error);

                            //Prevent endless loop
                            m_Player.Towns[m_CurrentTownIntern].Farmers[l_CurrentFarmIntern].LootTimer = (long.Parse(ServerTime) + long.Parse(m_Player.Towns[m_CurrentTownIntern].FarmersLootInterval)).ToString();
                            m_CurrentFarmIntern++;
                        }
                        m_State = "attackfarmersnext";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("attackFarmers2Response", e.Message + "\n" + p_Response);
                
                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to loot or demand farmers using the premium captain feature
         */ 
        private void attackFarmersAll1()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            bool l_attackFarmersComplete = true;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "index";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            //string l_Json = "{\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}";
            string l_Json = "{\"town_id\":" + l_Townid + ",\"nl_init\":true}";
            l_Json = Uri.EscapeDataString(l_Json);
            l_ServerTime = m_ServerTime;
            if (l_ServerTime.Length > 0)
            {
                while (l_ServerTime.Length < 13)
                    l_ServerTime = l_ServerTime + "0";
            }

            try
            {
                m_State = "attackfarmersall1";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Demanding resources from farmers (1/3)");
                //http://###.grepolis.com/game/farm_town_overviews?action=index&town_id=#####&h=###########&json=%7B%22town_id%22%3A%22#####%22%2C%22nlreq_id%22%3A0%7D&_=1324833362870

                if (!m_CanIModifyFarmers)//Check if farmer settings are loaded.
                {
                    //logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Loading farmer settings.");
                    m_Player.loadFarmersSettings(l_Townid);
                }
                if (l_Settings.FarmerScheduler.Split(';')[DateTime.Now.Hour].Equals("True"))//Check schedule
                {
                    if (l_Settings.MasterFarmers && m_Player.Towns[m_CurrentTownIntern].FarmersLootEnabled && m_Player.Towns[m_CurrentTownIntern].Farmers.Count > 0)//Check if current town has farmers on his Island
                    {
                        if (!m_Player.Towns[m_CurrentTownIntern].isStorageFull())//Check if storage is full
                        {
                            if (m_Player.Towns[m_CurrentTownIntern].isEnabledFarmersReady(ServerTime))
                            {
                                l_attackFarmersComplete = false;

                                string l_Url = "http://" + l_Settings.GenServer + "/game/farm_town_overviews?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                                Uri l_Uri = new Uri(l_Url);

                                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                                m_HttpHandler.DownloadStringAsync(l_Uri);
                                m_HttpHandler.Headers.Remove("X-Requested-With");
                            }
                            else
                            {
                                logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": No farmers available.");
                            }
                        }
                        else
                        {
                            logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are temporarily disabled for this town, your warehouse is full.");
                        }
                    }
                    else
                    {
                        logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are disabled for this town.");
                    }
                }
                else
                {
                    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Farmers are disabled for this town by scheduler.");
                }
                
                if (l_attackFarmersComplete)
                {
                    m_CurrentFarmIntern = 0;
                    m_State = "attackfarmersall3";
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in attackFarmersAll1(): " + e.Message);
            }
        }

        /*
         * Handles the server response of attackFarmersAll1().
         */
        private void attackFarmersAll1Response(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            string l_Search = "";
            string l_Error = "";
            int l_Index = 0;

            try
            {
                int l_ValidCode = validateResponse(p_Response, "attackFarmersAll1Response");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        //Server time
                        updateServerTime("_srvtime", p_Response);
                        addNotifications(p_Response);
                        
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in attackFarmersAll1Response(): " + l_Error);
                        }

                        m_State = "attackfarmersall3";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("attackFarmersAll1Response", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to loot or demand farmers using the premium captain feature
         */
        private void attackFarmersAll2()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "get_farm_towns_for_town";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            string l_IslandX = m_Player.Towns[m_CurrentTownIntern].IslandX;
            string l_IslandY = m_Player.Towns[m_CurrentTownIntern].IslandY;
            string l_BootyResearched = "0";
            string l_TradeOfficeBuild = "0";

            if (m_Player.Towns[m_CurrentTownIntern].Research.isResearched("booty"))
                l_BootyResearched = "1";
            if (m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("trade_office")].Level == 1)
                l_TradeOfficeBuild = "1";

            //string l_Json = "{\"island_x\":" + l_IslandX + ",\"island_y\":" + l_IslandY + ",\"booty_researched\":" + l_BootyResearched + ",\"trade_office\":" + l_TradeOfficeBuild + ",\"town_id\":\"" + l_Townid + "\",\"nlreq_id\":" + m_Nlreq_id + "}";
            string l_Json = "{\"island_x\":" + l_IslandX + ",\"island_y\":" + l_IslandY + ",\"current_town_id\":" + l_Townid + ",\"booty_researched\":" + l_BootyResearched + ",\"trade_office\":" + l_TradeOfficeBuild + ",\"town_id\":" + l_Townid + ",\"nl_init\":true}";
            l_Json = Uri.EscapeDataString(l_Json);
            l_ServerTime = m_ServerTime;
            if (l_ServerTime.Length > 0)
            {
                while (l_ServerTime.Length < 13)
                    l_ServerTime = l_ServerTime + "0";
            }

            try
            {
                m_State = "attackfarmersall2";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Demanding resources from farmers (2/3)");
                //http://###.grepolis.com/game/farm_town_overviews?action=get_farm_towns_for_town&town_id=#####&h=###########&json=%7B%22island_x%22%3A###%2C%22island_y%22%3A###%2C%22booty_researched%22%3A1%2C%22trade_office%22%3A0%2C%22town_id%22%3A%22#####%22%2C%22nlreq_id%22%3A0%7D&_=1324843453689
                string l_Url = "http://" + l_Settings.GenServer + "/game/farm_town_overviews?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.DownloadStringAsync(l_Uri);
                m_HttpHandler.Headers.Remove("X-Requested-With");
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in attackFarmersAll2(): " + e.Message);
            }
        }

        /*
         * Handles the server response of attackFarmersAll2().
         */
        private void attackFarmersAll2Response(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            string l_Search = "";
            string l_Error = "";
            int l_Index = 0;

            try
            {
                int l_ValidCode = validateResponse(p_Response, "attackFarmersAll2Response");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        //Response contains farmers status: mood, relation, lootable timer, location
                        //Server time
                        updateServerTime("_srvtime", p_Response);
                        addNotifications(p_Response);
                        
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in attackFarmersAll2Response(): " + l_Error);
                        }

                        m_State = "attackfarmersall3";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("attackFarmersAll2Response", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to loot or demand farmers using the premium captain feature
         */
        private void attackFarmersAll3()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_Action = "claim_loads";
            string l_FarmTownIds = "";
            string l_TimeOption = "300";
            string l_ClaimFactor = "normal";//normal or double
            string l_TownId = m_Player.Towns[m_CurrentTownIntern].TownID;
            //http://###.grepolis.com/game/farm_town_overviews?action=claim_loads&town_id=#####&h=###########
            //{"farm_town_ids":[####,####,####,####,####,####,####,####],"time_option":300,"claim_factor":"double","current_town_id":#####,"town_id":"#####","nlreq_id":###}
            //{"farm_town_ids":[####,####,####,####,####,####,####,####],"time_option":300,"claim_factor":"normal","current_town_id":#####,"town_id":"#####","nlreq_id":###}

            try
            {
                m_State = "attackfarmersall3";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Demanding resources from farmers (3/3)");

                //Claim factor
                if (m_Player.Towns[m_CurrentTownIntern].getLowestEnabledFarmersMood() >= m_Player.Towns[m_CurrentTownIntern].FarmersMinMood && !m_Player.Towns[m_CurrentTownIntern].FarmersFriendlyDemandsOnly)//Check if the type of the loot is non-friendly
                {
                    l_ClaimFactor = "double";
                }
                else//Type of loot is friendly
                {
                    l_ClaimFactor = "normal";
                }

                //Time
                l_TimeOption = (int.Parse(m_Player.Towns[m_CurrentTownIntern].FarmersLootInterval) * 60).ToString();

                //FarmTownIds
                l_FarmTownIds = m_Player.Towns[m_CurrentTownIntern].getEnabledFarmersID();

                if (!l_FarmTownIds.Equals(""))
                {
                    NameValueCollection l_Content = new NameValueCollection();
                    string l_Url = "http://" + l_Settings.GenServer + "/game/farm_town_overviews?town_id=" + l_TownId + "&action=" + l_Action + "&h=" + m_H;
                    Uri l_Uri = new Uri(l_Url);
                    //l_Content.Add("json", "{\"farm_town_ids\":[" + l_FarmTownIds + "],\"time_option\":" + l_TimeOption + ",\"claim_factor\":\"" + l_ClaimFactor + "\",\"current_town_id\":" + l_TownId + ",\"town_id\":\"" + l_TownId + "\",\"nlreq_id\":" + m_Nlreq_id + "}");
                    l_Content.Add("json", "{\"farm_town_ids\":[" + l_FarmTownIds + "],\"time_option\":" + l_TimeOption + ",\"claim_factor\":\"" + l_ClaimFactor + "\",\"current_town_id\":" + l_TownId + ",\"town_id\":" + l_TownId + ",\"nl_init\":true}");
                    m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                    m_HttpHandler.Headers.Remove("X-Requested-With");
                }
                else
                {
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in attackFarmersAll3(): " + e.Message);
            }
        }

        /*
         * Handles the server response of attackFarmersAll3().
         */
        private void attackFarmersAll3Response(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            string l_Search = "";
            string l_Looted = "";
            string l_Error = "";
            int l_Index = 0;
            int l_FarmerIndex = 0;
            string l_FarmersID = "";

            try
            {
                int l_ValidCode = validateResponse(p_Response, "attackFarmersAll3Response");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        l_Search = "FarmTownPlayerRelation";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        while (l_Index != -1)
                        {
                            l_Search = "\\\"farm_town_id\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_FarmersID = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\\\"loot\\\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Looted = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            
                            //Update data
                            l_FarmerIndex = m_Player.Towns[m_CurrentTownIntern].getFarmersIndex(l_FarmersID);
                            m_Player.Towns[m_CurrentTownIntern].Farmers[l_FarmerIndex].Looted = l_Looted;
                            if ((int.Parse(l_Looted) >= l_Settings.FarmerMaxResources))
                                m_Player.Towns[m_CurrentTownIntern].Farmers[l_FarmerIndex].FarmersLimitReached = true;
                            else
                                m_Player.Towns[m_CurrentTownIntern].Farmers[l_FarmerIndex].FarmersLimitReached = false;
                            
                            //Search next
                            l_Search = "FarmTownPlayerRelation";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Search = "FarmTownPlayerRelation";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        }

                        logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Successfull looted/demanded resources from " + m_Player.Towns[m_CurrentTownIntern].getEnabledFarmersName());

                        updateResourcesFromNotification(p_Response, false);
                        //Server time
                        updateServerTime("_srvtime", p_Response);
                        addNotifications(p_Response);
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in attackFarmersAll3Response(): " + l_Error);
                        }   
                    }
                    stateManagerDelay();
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("attackFarmersAll3Response", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to add buildings to the queue.
         */ 
        private void checkBuildingQueue()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                m_State = "checkbuildingqueue";
                m_AddedExtraBuildingInNonTargetQueueMode = false;
                bool l_TeardownableBuildingFound = false;
                m_BuildingTearDown = "";

                if (l_Settings.MasterBuildingQueue && m_Player.Towns[m_CurrentTownIntern].BuildingQueueEnabled)
                {
                    string l_Building = "";

                    //Check if a Farm is needed
                    if (m_Player.Towns[m_CurrentTownIntern].PopulationAvailable < l_Settings.BuildFarmBelow)
                    {
                        //Check if building queue is empty (Needs different checks)
                        if (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)
                        {
                            if (!m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding().Equals("farm") &&
                                        !m_Player.Towns[m_CurrentTownIntern].getIngameBuildingQueue().Contains("farm") &&
                                        m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("farm")].Upgradable &&
                                        m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("farm")].NextLevel <= m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("farm")].getMaximumLevel(l_Settings.GenIsHeroWorld))
                            {
                                l_Building = "farm";
                            }
                        }
                        else
                        {
                            if (!m_Player.Towns[m_CurrentTownIntern].getIngameBuildingQueue().Contains("farm") &&
                                        m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("farm")].Upgradable &&
                                        m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("farm")].NextLevel <= m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("farm")].getMaximumLevel(l_Settings.GenIsHeroWorld))
                            {
                                l_Building = "farm";
                            }
                        }
                    }

                    if (l_Building.Equals(""))
                    {
                        if (m_Player.Towns[m_CurrentTownIntern].BuildingLevelsTargetEnabled && !(l_Settings.AdvancedQueue && (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)))
                        {
                            //Automatic mode activad
                            int l_LowestLevel = 50;
                            int l_HighestLevel = 0;
                            int l_Level = 0;
                            string l_BuildingTemp = "";
                            for (int i = 0; i < m_Player.Towns[m_CurrentTownIntern].Buildings.Count; i++)
                            {
                                l_BuildingTemp = m_Player.Towns[m_CurrentTownIntern].Buildings[i].DevName;
                                l_Level = m_Player.Towns[m_CurrentTownIntern].Buildings[i].NextLevel;// +m_Player.Towns[m_CurrentTownIntern].countIngameBuildingQueueByName(l_BuildingTemp);
                                if (m_Player.Towns[m_CurrentTownIntern].Buildings[i].Upgradable)
                                {
                                    if (l_Level <= m_Player.Towns[m_CurrentTownIntern].Buildings[i].TargetLevel)//l_Level <= TargetLevel instead of l_Level < TargetLevel
                                    {
                                        if (l_Level < l_LowestLevel)
                                        {
                                            l_Building = l_BuildingTemp;
                                            l_LowestLevel = l_Level;
                                        }
                                    }
                                }
                            }
                            //Search building to demolish when no upgradable building is found
                            //Highest level building is downgraded first
                            //Requirement for demolishing is Senate (main) building lvl 10
                            if (l_Building.Equals("") && m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding("main")].Level >= 10 && m_Player.Towns[m_CurrentTownIntern].BuildingDowngradeEnabled)
                            {
                                for (int i = 0; i < m_Player.Towns[m_CurrentTownIntern].Buildings.Count; i++)
                                {
                                    l_BuildingTemp = m_Player.Towns[m_CurrentTownIntern].Buildings[i].DevName;
                                    //l_Level = m_Player.Towns[m_CurrentTownIntern].Buildings[i].NextLevel -1;//Uses next level to take buildings currently in the ingame queue into account
                                    l_Level = m_Player.Towns[m_CurrentTownIntern].Buildings[i].Level;//Takes buildings in ingame into account since the method buildingBuildData is used
                                    if (m_Player.Towns[m_CurrentTownIntern].Buildings[i].Teardownable)
                                    {
                                        if (l_Level > m_Player.Towns[m_CurrentTownIntern].Buildings[i].TargetLevel)
                                        {
                                            if (l_Level > l_HighestLevel)
                                            {
                                                l_TeardownableBuildingFound = true;
                                                l_Building = l_BuildingTemp;
                                                m_BuildingTearDown = l_Building;
                                                l_HighestLevel = l_Level;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (m_Player.Towns[m_CurrentTownIntern].BuildingLevelsTargetEnabled && (l_Settings.AdvancedQueue && (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)))
                        {
                            //Advanced Queue mode activated
                            if (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)
                            {
                                //Check if max level is already reached
                                int l_BuildingIndex = m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding());
                                while (m_Player.Towns[m_CurrentTownIntern].Buildings[l_BuildingIndex].NextLevel > m_Player.Towns[m_CurrentTownIntern].Buildings[l_BuildingIndex].getMaximumLevel(l_Settings.GenIsHeroWorld))
                                {
                                    m_Player.Towns[m_CurrentTownIntern].dequeueBuilding();
                                    if (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)
                                        l_BuildingIndex = m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding());
                                    else
                                        break;
                                }
                            }

                            if (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)
                            {
                                //Check if max level is already reached
                                int l_BuildingIndex = m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding());
                                if (m_Player.Towns[m_CurrentTownIntern].Buildings[l_BuildingIndex].Upgradable)
                                {
                                    l_Building = m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding();
                                }
                            }
                        }
                        else
                        {
                            //Normal mode activated
                            if (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)
                            {
                                //Check if max level is already reached
                                int l_BuildingIndex = m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding());
                                while (m_Player.Towns[m_CurrentTownIntern].Buildings[l_BuildingIndex].NextLevel > m_Player.Towns[m_CurrentTownIntern].Buildings[l_BuildingIndex].getMaximumLevel(l_Settings.GenIsHeroWorld))
                                {
                                    m_Player.Towns[m_CurrentTownIntern].dequeueBuilding();
                                    if (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)
                                        l_BuildingIndex = m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding());
                                    else
                                        break;
                                }
                            }

                            if (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0)
                            {
                                //Check if max level is already reached
                                int l_BuildingIndex = m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding());
                                if (m_Player.Towns[m_CurrentTownIntern].Buildings[l_BuildingIndex].Upgradable)
                                {
                                    l_Building = m_Player.Towns[m_CurrentTownIntern].peekQueueBuilding();
                                }
                            }
                        }
                    }

                    if (!l_Building.Equals(""))
                    {
                        if (!l_TeardownableBuildingFound)
                        {
                            setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Checking building queue");
                            NameValueCollection l_Content = new NameValueCollection();
                            //http://***.grepolis.com/game/frontend_bridge?town_id=***&action=execute&h=***
                            string l_Url = "http://" + l_Settings.GenServer + "/game/frontend_bridge?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=execute&h=" + m_H;
                            Uri l_Uri = new Uri(l_Url);
                            //l_Content.Add("json", "{\"model_url\":\"BuildingOrder\",\"action_name\":\"buildUp\",\"arguments\":{\"building_id\":\"" + l_Building + "\",\"build_for_gold\":false},\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nlreq_id\":" + m_Nlreq_id + "}");//Building started directly from main interface
                            l_Content.Add("json", "{\"model_url\":\"BuildingOrder\",\"action_name\":\"buildUp\",\"arguments\":{\"building_id\":\"" + l_Building + "\",\"build_for_gold\":false},\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");//Building started directly from main interface

                            logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": " + m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(l_Building)].LocalName + " added to the ingame queue.");

                            m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                            m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                            m_HttpHandler.Headers.Remove("X-Requested-With");
                        }
                        else
                        {
                            checkBuildingQueuePreTearDown();
                        }
                    }
                    else
                    {
                        logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Nothing to build.");

                        setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Nothing to build");
                        m_State = "checkbuildingqueueSKIP";
                        stateManagerDelay();
                    }
                }
                else
                {
                    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Building queue is disabled.");

                    //Building queue is disabled for this town
                    setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Building queue is disabled for this town");
                    m_State = "checkbuildingqueueSKIP";
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkBuildingQueue(): " + e.Message);
            }
        }

        /*
         * Handles the server response of checkBuildingQueue().
         */
        private void checkBuildingQueueResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                string l_Search = "";
                int l_Index = -1;
                string l_BuildingName = "";
                string l_BuildingQueueParsed = "";

                int l_ValidCode = validateResponse(p_Response, "checkBuildingQueueResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Building added to ingame queue");
                        if (!m_Player.Towns[m_CurrentTownIntern].BuildingLevelsTargetEnabled && !m_AddedExtraBuildingInNonTargetQueueMode && (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0))
                        {
                            m_Player.Towns[m_CurrentTownIntern].dequeueBuilding();
                        }
                        else if (m_Player.Towns[m_CurrentTownIntern].BuildingLevelsTargetEnabled && l_Settings.AdvancedQueue && (m_Player.Towns[m_CurrentTownIntern].BuildingQueue.Count > 0))
                        {
                            m_Player.Towns[m_CurrentTownIntern].dequeueBuilding();
                        }

                        updateResourcesFromNotification(p_Response, false);

                        l_Search = "\\\"building_type\\\":\\\"";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        while (l_Index != -1)
                        {
                            l_BuildingName = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_BuildingQueueParsed += l_BuildingName + ";";
                            //Search for next building
                            l_Search = "\\\"building_type\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        }
                        //Update building queue 
                        m_Player.Towns[m_CurrentTownIntern].setIngameBuildingQueue(l_BuildingQueueParsed);
                       
                        //Server time
                        updateServerTime("_srvtime", p_Response);
                        addNotifications(p_Response);

                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            //l_IOHandler.debug("Ingame error while adding building to ingame queue: " + p_Response.Substring(p_Response.IndexOf(":\"", 0) + 2, p_Response.IndexOf("\"}", 0) - (p_Response.IndexOf(":\"", 0)) + 2));
                            l_IOHandler.debug("Ingame error while adding building to ingame queue");
                        }
                        //This method is called when there is nothing to build
                        m_State = "checkbuildingqueueSKIP";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkBuildingQueueResponse", e.Message + "\n" + p_Response);
                
                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to open the main building and the demolish tab to downgrade building
         */
        private void checkBuildingQueuePreTearDown()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "index";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            //string l_Json = "{\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}";
            string l_Json = "{\"town_id\":" + l_Townid + ",\"nl_init\":true}";
            l_Json = Uri.EscapeDataString(l_Json);
            l_ServerTime = m_ServerTime;
            if (l_ServerTime.Length > 0)
            {
                while (l_ServerTime.Length < 13)
                    l_ServerTime = l_ServerTime + "0";
            }

            try
            {
                m_State = "checkbuildingqueuepreteardown";
                setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Open demolish tab");
                //http://***.grepolis.com/game/building_main?town_id=***&action=index&h=***d&json=***&_=***
                string l_Url = "http://" + l_Settings.GenServer + "/game/building_main?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.DownloadStringAsync(l_Uri);
                m_HttpHandler.Headers.Remove("X-Requested-With");
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkBuildingQueuePreTearDown(): " + e.Message);
            }
        }

        /*
         * Handles the server response of updateBuildings().
         */
        private void checkBuildingQueuePreTearDownResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                int l_ValidCode = validateResponse(p_Response, "checkBuildingQueuePreTearDownResponse");
                if (l_ValidCode == 1)
                {
                    m_Player.Towns[m_CurrentTownIntern].BuildingsCanBeTearedDown = p_Response.Contains("tear_down_buildings");

                    //Server time
                    updateServerTime("_srvtime", p_Response);
                    addNotifications(p_Response);
                    

                    if (!m_Player.Towns[m_CurrentTownIntern].BuildingsCanBeTearedDown)
                        m_State = "checkbuildingqueueSKIP";
                    stateManagerDelay();
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkBuildingQueuePreTearDownResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to add buildings to the queue to demolish.
         */ 
        private void checkBuildingQueueTearDown()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                m_State = "checkbuildingqueueteardown";

                if (!m_BuildingTearDown.Equals(""))
                {
                    setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Checking building queue");
                    NameValueCollection l_Content = new NameValueCollection();
                    string l_Action = "execute";
                    //http://***.grepolis.com/game/frontend_bridge?town_id=***&action=execute&h=a7eac0b82cd
                    String l_Url = "http://" + l_Settings.GenServer + "/game/frontend_bridge?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H;
                    Uri l_Uri = new Uri(l_Url);
                    //l_Content.Add("json", "{\"model_url\":\"BuildingOrder\",\"action_name\":\"tearDown\",\"arguments\":{\"building_id\":\"" + m_BuildingTearDown + "\"},\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nlreq_id\":" + m_Nlreq_id + "}");
                    l_Content.Add("json", "{\"model_url\":\"BuildingOrder\",\"action_name\":\"tearDown\",\"arguments\":{\"building_id\":\"" + m_BuildingTearDown + "\"},\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");
                    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": " + m_Player.Towns[m_CurrentTownIntern].Buildings[m_Player.Towns[m_CurrentTownIntern].getIndexBuilding(m_BuildingTearDown)].LocalName + " demolished.");

                    m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                    m_HttpHandler.Headers.Remove("X-Requested-With");
                }
                else
                {
                    m_State = "checkbuildingqueueSKIP";
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkBuildingQueueTearDown(): " + e.Message);
            }
        }

        /*
         * Handles the server response of checkBuildingQueueTearDown().
         */
        private void checkBuildingQueueTearDownResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                string l_Search = "";
                int l_Index = -1;
                string l_BuildingName = "";
                string l_BuildingQueueParsed = "";

                int l_ValidCode = validateResponse(p_Response, "checkBuildingQueueTearDownResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Building demolished");

                        updateResourcesFromNotification(p_Response, false);

                        l_Search = "\\\"building_type\\\":\\\"";
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        while (l_Index != -1)
                        {
                            l_BuildingName = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\\",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_BuildingQueueParsed += l_BuildingName + ";";
                            //Search for next building
                            l_Search = "\\\"building_type\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index + l_Search.Length);
                        }
                        //Update building queue 
                        m_Player.Towns[m_CurrentTownIntern].setIngameBuildingQueue(l_BuildingQueueParsed);

                        addNotifications(p_Response);
                        
                        //Server time
                        updateServerTime("_srvtime", p_Response);

                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_IOHandler.debug("Ingame error while demolishing building");
                        }
                        m_State = "checkbuildingqueueSKIP";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkBuildingQueueTearDownResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to simulate opening barracks and train land units - step 1.
         * Opens barracks window only.
         */ 
        private void checkPreLandArmyQueue()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "index";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            //string l_Json = "{\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}";
            string l_Json = "{\"town_id\":" + l_Townid + ",\"nl_init\":true}";
            l_Json = Uri.EscapeDataString(l_Json);
            l_ServerTime = m_ServerTime;
            if (l_ServerTime.Length > 0)
            {
                while (l_ServerTime.Length < 13)
                    l_ServerTime = l_ServerTime + "0";
            }

            try
            {
                if (m_Player.Towns[m_CurrentTownIntern].Buildings[9].Level > 0)//barrack
                {
                    m_State = "checkprelandarmyqueue";
                    setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Updating land army data");

                    string l_Url = "http://" + l_Settings.GenServer + "/game/building_barracks?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action = " + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                    Uri l_Uri = new Uri(l_Url);

                    m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    m_HttpHandler.DownloadStringAsync(l_Uri);
                    m_HttpHandler.Headers.Remove("X-Requested-With");
                }
                else
                {
                    m_State = "checklandarmyqueueSKIP";
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkPreLandArmyQueue(): " + e.Message);
            }
        }

        /*
         * Handles the server response of checkPreLandArmyQueue().
         */
        private void checkPreLandArmyQueueResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            int l_Index = 0;
            int l_UnitIndex = -1;
            string l_Search = "UnitOrder.init(";
            string l_Name = "";
            string l_Count = "";
            string l_Total = "";
            string l_MaxBuild = "";
            string l_UnitsLeft = "";
            string l_Error = "";

            try
            {
                int l_ValidCode = validateResponse(p_Response, "checkPreLandArmyQueueResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        if (!p_Response.Contains("barracks.png"))
                        {
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Search = "\\\"id\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            while (l_Index != -1)
                            {
                                l_Name = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_UnitIndex = m_Player.Towns[m_CurrentTownIntern].getUnitIndex(l_Name);
                                l_Search = "\\\"count\\\":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_Count = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].CurrentAmount = int.Parse(l_Count);
                                l_Search = "\\\"total\\\":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_Total = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].TotalAmount = int.Parse(l_Total);
                                l_Search = "\\\"max_build\\\":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_MaxBuild = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].MaxBuild = int.Parse(l_MaxBuild);
                                //Search next
                                l_Search = "\\\"id\\\":\\\"";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                            }

                            m_Player.Towns[m_CurrentTownIntern].resetUnitQueueLand();
                            l_Search = "{\\\"unit_id\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            while (l_Index != -1)
                            {
                                l_Name = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_UnitIndex = m_Player.Towns[m_CurrentTownIntern].getUnitIndex(l_Name);
                                l_Search = "\\\"units_left\\\":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_UnitsLeft = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                                if (l_UnitIndex != -1)
                                {
                                    m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].QueueGame += int.Parse(l_UnitsLeft);
                                    m_Player.Towns[m_CurrentTownIntern].SizeOfLandUnitQueue += 1;
                                }

                                //Search next
                                l_Search = "{\\\"unit_id\\\":\\\"";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                            }
                            addNotifications(p_Response);
                            
                        }
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkPreLandArmyQueueResponse(): " + l_Error);
                        }
                        m_State = "checklandarmyqueueSKIP";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkPreLandArmyQueueResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to simulate opening barracks and train land units - step 2.
         * Barracks already openened, adds units to the queue.
         */ 
        private void checkLandArmyQueue()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                m_State = "checklandarmyqueue";
                int l_Amount = 0;
                int l_QueuePop = 0;

                //Check if it's possible to queue an unit at this moment
                if (l_Settings.MasterUnitQueue && m_Player.Towns[m_CurrentTownIntern].UnitQueueEnabled && m_Player.Towns[m_CurrentTownIntern].SizeOfLandUnitQueue < 7 && m_Player.Towns[m_CurrentTownIntern].SizeOfLandUnitQueue < l_Settings.QueueLimit)
                {
                    //Check which unit you need to queue
                    int l_UnitIndex = m_Player.Towns[m_CurrentTownIntern].getMostTrainableLandUnitInQueue(m_Player.getFavorByTownIndexInt(m_CurrentTownIntern));
                    if (l_UnitIndex != -1)
                    {
                        //Check how many you can queue
                        l_Amount = m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].QueueBot - (m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].TotalAmount + m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].QueueGame);
                        if (m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].MaxBuild < l_Amount)
                        {
                            l_Amount = m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].MaxBuild;
                            //check if total queue pop is enough
                            l_QueuePop = l_Amount * m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].Population;
                            if (l_QueuePop < l_Settings.MinUnitQueuePop)
                                l_Amount = 0;//When l_Amount is 0 it will skip the queue
                        }
                        //Final check
                        if (l_Amount > 0)
                        {
                            setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Checking land unit queue");
                            //http://###.grepolis.com/game/building_barracks?action=build&town_id=#####&h=###########
                            NameValueCollection l_Content = new NameValueCollection();
                            String l_Url = "http://" + l_Settings.GenServer + "/game/building_barracks?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=build&h=" + m_H;
                            Uri l_Uri = new Uri(l_Url);
                            //l_Content.Add("json", "{\"unit_id\":\"" + m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].Name + "\",\"amount\":" + l_Amount.ToString() + ",\"town_id\":\"" + m_Player.Towns[m_CurrentTownIntern].TownID + "\",\"nlreq_id\":" + m_Nlreq_id + "}");//json={"unit_id":"archer","amount":#,"town_id":"#####","nlreq_id":##}
                            l_Content.Add("json", "{\"unit_id\":\"" + m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].Name + "\",\"amount\":" + l_Amount.ToString() + ",\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");

                            string l_UnitName = m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].LocalName;

                            logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": " + l_UnitName + "(" + l_Amount.ToString() + ") added to the ingame queue.");

                            m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                            m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                            m_HttpHandler.Headers.Remove("X-Requested-With");
                        }
                        else
                        {
                            //Unit queue skipped. Max trainable units is 0.
                            m_State = "checklandarmyqueueSKIP";
                            stateManagerDelay();
                        }
                    }
                    else
                    {
                        //Unit queue skipped. Cannot find trainable units.
                        m_State = "checklandarmyqueueSKIP";
                        stateManagerDelay();
                    }
                }
                else
                {
                    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Army queue is disabled for this town (or queue is full).");
                    m_State = "checklandarmyqueueSKIP";
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkLandArmyQueue(): " + e.Message);
            }
        }

        /*
         * Handles the server response of checkLandArmyQueue().
         */
        private void checkLandArmyQueueResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                string l_Search = "\"bar\":";
                int l_Index = -1;
                string l_Error = "";

                int l_ValidCode = validateResponse(p_Response, "checkLandArmyQueueResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Land unit(s) added to ingame queue");

                        l_Index = p_Response.IndexOf(l_Search, 0);
                        if (l_Index != -1)
                        {
                            updateResourcesFromNotification(p_Response, false);
                            addNotifications(p_Response);
                        }
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkLandArmyQueueResponse(): " + l_Error);
                            
                        }
                        m_State = "checklandarmyqueueSKIP";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkLandArmyQueueResponse", e.Message + "\n" + p_Response);
                
                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to simulate opening docks and train navy units - step 1.
         * Opens docks window only.
         */ 
        private void checkPreNavyArmyQueue()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            string l_ServerTime = "0";//Length must be 13
            string l_Action = "index";
            string l_Townid = m_Player.Towns[m_CurrentTownIntern].TownID;
            //string l_Json = "{\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}";
            string l_Json = "{\"town_id\":" + l_Townid + ",\"nl_init\":true}";
            l_Json = Uri.EscapeDataString(l_Json);
            l_ServerTime = m_ServerTime;
            if (l_ServerTime.Length > 0)
            {
                while (l_ServerTime.Length < 13)
                    l_ServerTime = l_ServerTime + "0";
            }

            try
            {
                if (m_Player.Towns[m_CurrentTownIntern].Buildings[8].Level > 0)//docks
                {
                    m_State = "checkprenavyarmyqueue";
                    setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Updating navy army data");

                    string l_Url = "http://" + l_Settings.GenServer + "/game/building_docks?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                    Uri l_Uri = new Uri(l_Url);

                    m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    m_HttpHandler.DownloadStringAsync(l_Uri);
                    m_HttpHandler.Headers.Remove("X-Requested-With");
                }
                else
                {
                    m_State = "checknavyarmyqueueSKIP";
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkPreNavyArmyQueue(): " + e.Message);
            }
        }

        /*
         * Handles the server response of checkPreNavyArmyQueue().
         */
        private void checkPreNavyArmyQueueResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            int l_Index = 0;
            int l_UnitIndex = -1;
            string l_Search = "UnitOrder.init(";
            string l_Name = "";
            string l_Total = "";
            string l_Count = "";
            string l_MaxBuild = "";
            string l_UnitsLeft = "";
            string l_Error = "";

            try
            {
                int l_ValidCode = validateResponse(p_Response, "checkPreNavyArmyQueueResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        if (!p_Response.Contains("docks.png"))
                        {
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Search = "\\\"id\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            while (l_Index != -1)
                            {
                                l_Name = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_UnitIndex = m_Player.Towns[m_CurrentTownIntern].getUnitIndex(l_Name);
                                l_Search = "\\\"count\\\":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_Count = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].CurrentAmount = int.Parse(l_Count);
                                l_Search = "\\\"total\\\":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_Total = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].TotalAmount = int.Parse(l_Total);
                                l_Search = "\\\"max_build\\\":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_MaxBuild = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].MaxBuild = int.Parse(l_MaxBuild);
                                //Search next
                                l_Search = "\\\"id\\\":\\\"";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                            }

                            m_Player.Towns[m_CurrentTownIntern].resetUnitQueueNavy();
                            l_Search = "{\\\"unit_id\\\":\\\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            while (l_Index != -1)
                            {
                                l_Name = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\\\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                                l_UnitIndex = m_Player.Towns[m_CurrentTownIntern].getUnitIndex(l_Name);
                                l_Search = "\\\"units_left\\\":";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                                l_UnitsLeft = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                                if (l_UnitIndex != -1)
                                {
                                    m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].QueueGame += int.Parse(l_UnitsLeft);
                                    m_Player.Towns[m_CurrentTownIntern].SizeOfNavyUnitQueue += 1;
                                }

                                //Search next
                                l_Search = "{\\\"unit_id\\\":\\\"";
                                l_Index = p_Response.IndexOf(l_Search, l_Index);
                            }

                            addNotifications(p_Response);                            
                        }
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkPreNavyArmyQueueResponse(): " + l_Error);
                        }
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkPreNavyArmyQueueResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }

        /*
         * Server request to simulate opening docks and train navy units - step 2.
         * Docks already opened, add units to the queue.
         */
        private void checkNavyArmyQueue()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                m_State = "checknavyarmyqueue";
                int l_Amount = 0;
                int l_QueuePop = 0;

                //Check if it's possible to queue an unit at this moment
                if (l_Settings.MasterUnitQueue && m_Player.Towns[m_CurrentTownIntern].UnitQueueEnabled && m_Player.Towns[m_CurrentTownIntern].SizeOfNavyUnitQueue < 7 && m_Player.Towns[m_CurrentTownIntern].SizeOfNavyUnitQueue < l_Settings.QueueLimit)
                {
                    //Check which unit you need to queue
                    int l_UnitIndex = m_Player.Towns[m_CurrentTownIntern].getMostTrainableNavyUnitInQueue(m_Player.getFavorByTownIndexInt(m_CurrentTownIntern));
                    if (l_UnitIndex != -1)
                    {
                        //Check how many you can queue
                        l_Amount = m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].QueueBot - (m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].TotalAmount + m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].QueueGame);
                        if (m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].MaxBuild < l_Amount)
                        {
                            l_Amount = m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].MaxBuild;
                            //check if total queue pop is enough
                            l_QueuePop = l_Amount * m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].Population;
                            if (l_QueuePop < l_Settings.MinUnitQueuePop)
                                l_Amount = 0;//When l_Amount is 0 it will skip the queue
                        }
                        //Final check
                        if (l_Amount > 0)
                        {
                            setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Checking navy unit queue");
                            //http://###.grepolis.com/game/building_docks?action=build&town_id=#####&h=###########
                            NameValueCollection l_Content = new NameValueCollection();
                            String l_Url = "http://" + l_Settings.GenServer + "/game/building_docks?town_id=" + m_Player.Towns[m_CurrentTownIntern].TownID + "&action=build&h=" + m_H;
                            Uri l_Uri = new Uri(l_Url);
                            //l_Content.Add("json", "{\"unit_id\":\"" + m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].Name + "\",\"amount\":" + l_Amount.ToString() + ",\"town_id\":\"" + m_Player.Towns[m_CurrentTownIntern].TownID + "\",\"nlreq_id\":" + m_Nlreq_id + "}");//json={"unit_id":"####","amount":#,"town_id":"###","nlreq_id":###}
                            l_Content.Add("json", "{\"unit_id\":\"" + m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].Name + "\",\"amount\":" + l_Amount.ToString() + ",\"town_id\":" + m_Player.Towns[m_CurrentTownIntern].TownID + ",\"nl_init\":true}");

                            string l_UnitName = m_Player.Towns[m_CurrentTownIntern].ArmyUnits[l_UnitIndex].LocalName;

                            logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": " + l_UnitName + "(" + l_Amount.ToString() + ") added to the ingame queue.");

                            m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                            m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                            m_HttpHandler.Headers.Remove("X-Requested-With");
                        }
                        else
                        {
                            m_State = "checknavyarmyqueueSKIP";
                            stateManagerDelay();
                        }
                    }
                    else
                    {
                        m_State = "checknavyarmyqueueSKIP";
                        stateManagerDelay();
                    }
                }
                else
                {
                    logEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Navy queue is disabled for this town (or queue is full).");
                    m_State = "checknavyarmyqueueSKIP";
                    stateManagerDelay();
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkNavyArmyQueue(): " + e.Message);
            }
        }

        /*
         * Handles the server response of checkNavyArmyQueue().
         */
        private void checkNavyArmyQueueResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                string l_Search = "\"bar\":";
                int l_Index = -1;
                string l_Error = "";

                int l_ValidCode = validateResponse(p_Response, "checkNavyArmyQueueResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        setStatusBarEvent(m_Player.Towns[m_CurrentTownIntern].Name + ": Navy unit(s) added to ingame queue");

                        l_Index = p_Response.IndexOf(l_Search, 0);
                        if (l_Index != -1)
                        {
                            updateResourcesFromNotification(p_Response, false);

                            addNotifications(p_Response);                            
                        }
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkNavyArmyQueueResponse(): " + l_Error);
                        }
                        m_State = "checknavyarmyqueueSKIP";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
                m_RetryCountServerError = 0;
            }
            catch (Exception e)
            {
                setStatusBarEvent("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("(#" + m_RetryCountServerError + ")Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkNavyArmyQueueResponse", e.Message + "\n" + p_Response);

                if (m_RetryCountServerError < 1)
                {
                    m_RetryCountServerError++;
                    retryManager();
                }
                else
                {
                    m_RetryCountServerError = 0;
                    startReconnect(1);
                }
            }
        }
 
        private void final()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            m_TimeoutTimer.Stop();

            if (m_IsBotRunning)
            {
                randomizeStateManagerMode();
                setToTownProcessedStateEvent();
            }

            m_CurrentTownIntern++;
            if (m_CurrentTownIntern < m_Player.Towns.Count)
            {
                m_TimeoutTimer.Start();
                switchTown();
            }
            else
            {
                //Reset m_ReconnectCount when no errors have occurred.
                m_ReconnectCount = 0;
                setGuiToTimeoutProcessedStateEvent();
                l_IOHandler.saveTownsSettings(m_Player);
            }
        }

        /*
         * Server request that simulates opening the trade window.
         */ 
        private void checkTrade()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            m_State = "checktrade";
            string[] l_TownsSortedByDistance = m_TownsSortedByDistance.Split(';');
            int l_TownsSortedByDistanceIndex = 0;
            bool l_TraderFound = false;
            bool l_MarketTooLow = false;

            try
            {
                //Check if trading is enabled
                if (l_Settings.MasterTrade && m_Player.Towns[m_CurrentTownTradeIntern].TradeEnabled)
                {
                    if (m_Player.Towns[m_CurrentTownTradeIntern].HasConqueror.Equals("false"))
                    {
                        setStatusBarEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Trading.");
                        switch (m_Player.Towns[m_CurrentTownTradeIntern].TradeMode)
                        {
                            case "receive":
                                checkTradeNext(true);
                                break;
                            case "spy cave":
                                checkSpy();
                                break;
                            case "send":
                                //TODO: Lvl currently includes buildings still in the ingame queue -> Create method to substract those extra lvls
                                //Check if Marketplace is high enough
                                if (m_Player.Towns[m_CurrentTownTradeIntern].Buildings[m_Player.Towns[m_CurrentTownTradeIntern].getIndexBuilding("market")].Level >= 10)
                                {
                                    //Send resources to nearest town within allowed distance
                                    for (int i = m_CurrentTownTradeIntern2; i < m_Player.Towns.Count; i++)
                                    {
                                        l_TownsSortedByDistanceIndex = int.Parse(l_TownsSortedByDistance[i]);
                                        if (!m_Player.Towns[l_TownsSortedByDistanceIndex].HasEnoughResources &&
                                            m_Player.Towns[l_TownsSortedByDistanceIndex].HasConqueror.Equals("false") &&
                                            m_Player.Towns[l_TownsSortedByDistanceIndex].TradeEnabled &&
                                            (m_Player.Towns[l_TownsSortedByDistanceIndex].TradeMode.Equals("receive") || m_Player.Towns[l_TownsSortedByDistanceIndex].TradeMode.Equals("spy cave")) &&
                                            m_Player.Towns[l_TownsSortedByDistanceIndex].getDistance(m_Player.Towns[m_CurrentTownTradeIntern].IslandX, m_Player.Towns[m_CurrentTownTradeIntern].IslandY) < m_Player.Towns[m_CurrentTownTradeIntern].TradeMaxDistance)
                                        {
                                            l_TraderFound = true;
                                            m_CurrentTownTradeIntern2 = i;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    //Marketplace lvl too low
                                    int l_BuildingIndex = m_Player.Towns[m_CurrentTownTradeIntern].getBuildingIndex("market");
                                    l_MarketTooLow = true;
                                    logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": " + m_Player.Towns[m_CurrentTownTradeIntern].Buildings[l_BuildingIndex].LocalName + " (" + m_Player.Towns[m_CurrentTownTradeIntern].Buildings[l_BuildingIndex].Level + ") too low.");
                                }

                                if (l_TraderFound)
                                {
                                    //Create server request                     
                                    string l_ServerTime = "0";//Length must be 13
                                    string l_Action = "trading";
                                    string l_Townid = m_Player.Towns[m_CurrentTownTradeIntern].TownID;//Town that send the resources
                                    string l_Id = m_Player.Towns[int.Parse(l_TownsSortedByDistance[m_CurrentTownTradeIntern2])].TownID;//Town that receives the resources
                                    //string l_Json = "{\"id\":\"" + l_Id + "\",\" + l_Townid + "\",\"nlreq_id\":" + m_Nlreq_id + "}";
                                    string l_Json = "{\"id\":\"" + l_Id + "\",\"town_id\":" + l_Townid + ",\"nl_init\":true}";
                                    l_Json = Uri.EscapeDataString(l_Json);
                                    l_ServerTime = m_ServerTime;
                                    if (l_ServerTime.Length > 0)
                                    {
                                        while (l_ServerTime.Length < 13)
                                            l_ServerTime = l_ServerTime + "0";
                                    }

                                    //http://###.grepolis.com/game/town_info?action=trading&town_id=###&h=###&json=%7B%22id%22%3A%22###%22%2C%22town_id%22%3A%22###%22%2C%22nlreq_id%22%3A0%7D&_=###
                                    string l_Url = "http://" + l_Settings.GenServer + "/game/town_info?town_id=" + l_Townid + "&action=" + l_Action + "&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                                    Uri l_Uri = new Uri(l_Url);
                                    m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                                    m_HttpHandler.DownloadStringAsync(l_Uri);
                                    m_HttpHandler.Headers.Remove("X-Requested-With");
                                }
                                else
                                {
                                    if (!l_MarketTooLow)
                                        logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": No towns (" + (l_TownsSortedByDistance.Length - 1) + ") within range in receive/spy mode need resources.");

                                    //Marketplace lvl too low, or
                                    //No towns within range of sender are in receive or spy cave mode
                                    checkTradeNext(true);
                                }
                                break;
                        }
                    }
                    else
                    {
                        //Has conqueror
                        logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Has conqueror.");
                        checkTradeNext(true);
                    }
                }
                else
                {
                    //Trading disabled
                    logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Trading disabled.");
                    checkTradeNext(true);
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkTrade(): " + e.Message);
            }
        }

        private void checkTradeNext(bool p_NextSender)
        {
            m_TimeoutTimer.Stop();
            if(p_NextSender)
                m_CurrentTownTradeIntern++;
            if (m_CurrentTownTradeIntern < m_Player.Towns.Count)
            {
                m_CurrentTownTradeIntern2++;
                if(p_NextSender)
                    m_CurrentTownTradeIntern2 = 0;
                m_TownsSortedByDistance = m_Player.getTownsSortedByDistance(m_CurrentTownTradeIntern);
                m_TimeoutTimer.Start();
                checkTrade();
            }
            else
            {
                //Trading done
                logEvent("Trading complete.");
                setToTradeProcessedStateEvent();
            }
        }

        private void checkTradeResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                string[] l_TownsSortedByDistance = m_TownsSortedByDistance.Split(';');
                int l_TownsSortedByDistanceIndex = int.Parse(l_TownsSortedByDistance[m_CurrentTownTradeIntern2]);

                string l_Search = "\"max_capacity\":";
                int l_Index = -1;
                string l_Error = "";
                //Trading
                string l_MaxCapacity = "";
                string l_AvailableCapacity = "";
                string l_Wood = "0";
                string l_Stone = "0";
                string l_Iron = "0";
                string l_StorageVolume = "";
                string l_IncWood = "0";
                string l_IncStone = "0";
                string l_IncIron = "0";
                
                int l_ValidCode = validateResponse(p_Response, "checkTradeResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        l_Index = p_Response.IndexOf(l_Search, 0);
                        if (l_Index != -1)
                        {
                            //Get trade info
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_MaxCapacity = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"available_capacity\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_AvailableCapacity = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "{\"wood\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Wood = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"stone\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Stone = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"iron\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_Iron = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("}", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"storage_volume\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_StorageVolume = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "{\"wood\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_IncWood = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"stone\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_IncStone = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf(",", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Search = "\"iron\":";
                            l_Index = p_Response.IndexOf(l_Search, l_Index);
                            l_IncIron = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("}", l_Index + l_Search.Length) - (l_Index + l_Search.Length));

                            //Server time
                            updateServerTime("_srvtime", p_Response);
                            addNotifications(p_Response);

                            //Update resources
                            m_Player.Towns[l_TownsSortedByDistanceIndex].Wood = int.Parse(l_Wood);
                            m_Player.Towns[l_TownsSortedByDistanceIndex].Stone = int.Parse(l_Stone);
                            m_Player.Towns[l_TownsSortedByDistanceIndex].Iron = int.Parse(l_Iron);
                            m_Player.Towns[l_TownsSortedByDistanceIndex].WoodInc = int.Parse(l_IncWood);
                            m_Player.Towns[l_TownsSortedByDistanceIndex].StoneInc = int.Parse(l_IncStone);
                            m_Player.Towns[l_TownsSortedByDistanceIndex].IronInc = int.Parse(l_IncIron);
                            m_Player.Towns[l_TownsSortedByDistanceIndex].ResourcesLastUpdate = m_ServerTime;
                            //Update capacity
                            m_Player.Towns[m_CurrentTownTradeIntern].MaxTradeCapacity = int.Parse(l_MaxCapacity);
                            m_Player.Towns[m_CurrentTownTradeIntern].FreeTradeCapacity = int.Parse(l_AvailableCapacity);

                            stateManagerDelay();
                        }
                        else
                        {
                            //Trade window not loaded or changed. Goto next sender.
                            m_CurrentTownTradeIntern++;
                            m_State = "checktrade2";
                            stateManagerDelay();
                        }
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkTradeResponse(): " + l_Error);
                        }
                        //Ingame error occured. Goto next sender.
                        m_CurrentTownTradeIntern++;
                        m_State = "checktrade2";
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
            }
            catch (Exception e)
            {
                setStatusBarEvent("Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkTradeResponse", e.Message + "\n" + p_Response);

                //Parse error occured. Goto next sender.
                m_CurrentTownTradeIntern++;
                m_State = "checktrade2";
                stateManagerDelay();
            }
        }

        /*
         * Server request that sends the resources
         */ 
        private void checkTrade2()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            m_State = "checktrade2";

            string[] l_TownsSortedByDistance = m_TownsSortedByDistance.Split(';');
            int l_TownsSortedByDistanceIndex = int.Parse(l_TownsSortedByDistance[m_CurrentTownTradeIntern2]);

            //Get available resources to send
            int l_MaxSendWood = Math.Max(0, m_Player.Towns[m_CurrentTownTradeIntern].Wood - m_Player.Towns[m_CurrentTownTradeIntern].TradeRemainingResources);
            int l_MaxSendStone = Math.Max(0, m_Player.Towns[m_CurrentTownTradeIntern].Stone - m_Player.Towns[m_CurrentTownTradeIntern].TradeRemainingResources);
            int l_MaxSendIron = Math.Max(0, m_Player.Towns[m_CurrentTownTradeIntern].Iron - m_Player.Towns[m_CurrentTownTradeIntern].TradeRemainingResources);

            try
            {
                //Check if enough resources are available
                if ((m_Player.Towns[m_CurrentTownTradeIntern].FreeTradeCapacity > m_Player.Towns[m_CurrentTownTradeIntern].TradeMinSendAmount) &&
                    (l_MaxSendWood > 100 || l_MaxSendStone > 100 || l_MaxSendIron > 100))
                {
                    //Check if current town needs resources
                    int l_MaxNeededWood = Math.Max(0, ((m_Player.Towns[l_TownsSortedByDistanceIndex].Storage * m_Player.Towns[l_TownsSortedByDistanceIndex].TradePercentageWarehouse) / 100) - (m_Player.Towns[l_TownsSortedByDistanceIndex].Wood + m_Player.Towns[l_TownsSortedByDistanceIndex].WoodInc));
                    int l_MaxNeededStone = Math.Max(0, ((m_Player.Towns[l_TownsSortedByDistanceIndex].Storage * m_Player.Towns[l_TownsSortedByDistanceIndex].TradePercentageWarehouse) / 100) - (m_Player.Towns[l_TownsSortedByDistanceIndex].Stone + m_Player.Towns[l_TownsSortedByDistanceIndex].StoneInc));
                    int l_MaxNeededIron = Math.Max(0, ((m_Player.Towns[l_TownsSortedByDistanceIndex].Storage * m_Player.Towns[l_TownsSortedByDistanceIndex].TradePercentageWarehouse) / 100) - (m_Player.Towns[l_TownsSortedByDistanceIndex].Iron + m_Player.Towns[l_TownsSortedByDistanceIndex].IronInc));
                    if ((l_MaxNeededWood + l_MaxNeededStone + l_MaxNeededIron > m_Player.Towns[m_CurrentTownTradeIntern].TradeMinSendAmount) &&
                        (l_MaxNeededWood > 100 || l_MaxNeededStone > 100 || l_MaxNeededIron > 100))
                    {
                        //Calculate amount of resources to send
                        int l_SendWood = Math.Min(l_MaxSendWood, l_MaxNeededWood);
                        int l_SendStone = Math.Min(l_MaxSendStone, l_MaxNeededStone);
                        int l_SendIron = Math.Min(l_MaxSendIron, l_MaxNeededIron);
                        
                        while (true)
                        {
                            if (l_SendWood + l_SendStone + l_SendIron > m_Player.Towns[m_CurrentTownTradeIntern].FreeTradeCapacity)
                            {
                                if (l_SendWood >= l_SendStone && l_SendWood >= l_SendIron)
                                    l_SendWood -= 100;
                                else if (l_SendStone >= l_SendWood && l_SendStone >= l_SendIron)
                                    l_SendStone -= 100;
                                else
                                    l_SendIron -= 100;
                            }
                            else
                            {
                                l_SendWood = Math.Max(0, l_SendWood);
                                l_SendStone = Math.Max(0, l_SendStone);
                                l_SendIron = Math.Max(0, l_SendIron);
                                //Ready to send resources. Exit loop.
                                break;
                            }
                        }

                        if (l_SendWood + l_SendStone + l_SendIron > m_Player.Towns[m_CurrentTownTradeIntern].TradeMinSendAmount)
                        {
                            //Send resources
                            setStatusBarEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Sending resources to " + m_Player.Towns[l_TownsSortedByDistanceIndex].Name + ".");

                            NameValueCollection l_Content = new NameValueCollection();
                            //http://###.grepolis.com/game/town_info?action=trade&town_id=###&h=###
                            String l_Url = "http://" + l_Settings.GenServer + "/game/town_info?town_id=" + m_Player.Towns[m_CurrentTownTradeIntern].TownID + "&action=trade&h=" + m_H;
                            Uri l_Uri = new Uri(l_Url);
                            //l_Content.Add("json", "{\"id\":" + m_Player.Towns[l_TownsSortedByDistanceIndex].TownID + ",\"wood\":" + l_SendWood.ToString() + ",\"stone\":" + l_SendStone.ToString() + ",\"iron\":" + l_SendIron.ToString() + ",\"town_id\":\"" + m_Player.Towns[m_CurrentTownTradeIntern].TownID + "\",\"nlreq_id\":" + m_Nlreq_id + "}");
                            l_Content.Add("json", "{\"id\":" + m_Player.Towns[l_TownsSortedByDistanceIndex].TownID + ",\"wood\":" + l_SendWood.ToString() + ",\"stone\":" + l_SendStone.ToString() + ",\"iron\":" + l_SendIron.ToString() + ",\"town_id\":" + m_Player.Towns[m_CurrentTownTradeIntern].TownID + ",\"nl_init\":true}");

                            logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Sending resources to " + m_Player.Towns[l_TownsSortedByDistanceIndex].Name + " (Wood: " + l_SendWood.ToString() + "/" + (m_Player.Towns[l_TownsSortedByDistanceIndex].Wood + m_Player.Towns[l_TownsSortedByDistanceIndex].WoodInc) + ", Stone: " + l_SendStone.ToString() + "/" + (m_Player.Towns[l_TownsSortedByDistanceIndex].Stone + m_Player.Towns[l_TownsSortedByDistanceIndex].StoneInc) + ", Silver: " + l_SendIron.ToString() + "/" + (m_Player.Towns[l_TownsSortedByDistanceIndex].Iron + m_Player.Towns[l_TownsSortedByDistanceIndex].IronInc) + ").");

                            m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                            m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                            m_HttpHandler.Headers.Remove("X-Requested-With");
                        }
                        else
                        {
                            //Not enough *specific* resources to send. Sender could still have enough of a resource that another town needs.
                            logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Not enough resources for " + m_Player.Towns[l_TownsSortedByDistanceIndex].Name + " / trade capacity. (Trade capacity: " + m_Player.Towns[m_CurrentTownTradeIntern].FreeTradeCapacity + ", Wood: " + m_Player.Towns[m_CurrentTownTradeIntern].Wood + ", Stone: " + m_Player.Towns[m_CurrentTownTradeIntern].Stone + ", Silver: " + m_Player.Towns[m_CurrentTownTradeIntern].Iron + ")");
                            checkTradeNext(false);
                        }
                    }
                    else
                    {
                        //Town has sufficient resources
                        logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": " + m_Player.Towns[l_TownsSortedByDistanceIndex].Name + " has enough resources.");
                        m_Player.Towns[l_TownsSortedByDistanceIndex].HasEnoughResources = true;
                        checkTradeNext(false);
                    }
                }
                else
                {
                    //Not enough resources to send
                    logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Not enough resources / trade capacity. (Trade capacity: " + m_Player.Towns[m_CurrentTownTradeIntern].FreeTradeCapacity + ", Wood: " + m_Player.Towns[m_CurrentTownTradeIntern].Wood + ", Stone: " + m_Player.Towns[m_CurrentTownTradeIntern].Stone + ", Silver: " + m_Player.Towns[m_CurrentTownTradeIntern].Iron + ")");
                    checkTradeNext(true);
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkTrade2(): " + e.Message);
            }
        }

        private void checkTrade2Response(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                string l_Search = "_srvtime";
                int l_Index = -1;
                string l_Error = "";

                int l_ValidCode = validateResponse(p_Response, "checkTrade2Response");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {

                        l_Index = p_Response.IndexOf(l_Search, 0);
                        if (l_Index != -1)
                        {
                            updateResourcesFromNotification(p_Response, true);
                            //Server time
                            updateServerTime("_srvtime", p_Response);
                            addNotifications(p_Response);
                        }
                        m_CurrentTownTradeIntern2++;
                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkTrade2Response(): " + l_Error);
                        }
                        //Ingame error occured. Goto next sender.
                        m_CurrentTownTradeIntern++;
                        stateManagerDelay();
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
            }
            catch (Exception e)
            {
                setStatusBarEvent("Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkTrade2Response", e.Message + "\n" + p_Response);

                //Parse error occured. Goto next sender.
                m_CurrentTownTradeIntern++;
                stateManagerDelay();
            }
        }

        /*
         * Server request that simulates opening the spy cave window
         */ 
        private void checkSpy()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                m_State = "checkspy";
                
                int l_IronToStore = m_Player.Towns[m_CurrentTownTradeIntern].Iron - m_Player.Towns[m_CurrentTownTradeIntern].TradeRemainingResources;
                if (l_IronToStore > m_Player.Towns[m_CurrentTownTradeIntern].TradeMinSendAmount)
                {

                    string l_ServerTime = "0";//Length must be 13
                    string l_Townid = m_Player.Towns[m_CurrentTownTradeIntern].TownID;
                    //string l_Json = "{\"window_type\":\"hide\",\"tab_type\":\"index\",\"known_data\":{\"models\":[],\"collections\":[\"Towns\"],\"templates\":[]},\"town_id\":" + l_Townid + ",\"nlreq_id\":" + m_Nlreq_id + "}";
                    string l_Json = "{\"window_type\":\"hide\",\"tab_type\":\"index\",\"known_data\":{\"models\":[],\"collections\":[\"Towns\"],\"templates\":[\"hide__building_hide\",\"hide__no_building\"]},\"town_id\":" + l_Townid + ",\"nl_init\":true}";
                    l_Json = Uri.EscapeDataString(l_Json);
                    l_ServerTime = m_ServerTime;
                    if (l_ServerTime.Length > 0)
                    {
                        while (l_ServerTime.Length < 13)
                            l_ServerTime = l_ServerTime + "0";
                    }

                    //http://###.grepolis.com/game/frontend_bridge?town_id=###&action=fetch&h=bd1e7e3a580&json=####&_=###
                    string l_Url = "http://" + l_Settings.GenServer + "/game/frontend_bridge?town_id=" + l_Townid + "&action=fetch&h=" + m_H + "&json=" + l_Json + "&_=" + l_ServerTime;
                    Uri l_Uri = new Uri(l_Url);

                    m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    m_HttpHandler.DownloadStringAsync(l_Uri);
                    m_HttpHandler.Headers.Remove("X-Requested-With");
                }
                else
                {
                    //Not enough silver to store. Goto next sender
                    logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Not enough silver to store. (Wood: " + m_Player.Towns[m_CurrentTownTradeIntern].Wood + " (+" + m_Player.Towns[m_CurrentTownTradeIntern].WoodInc + "), Stone: " + m_Player.Towns[m_CurrentTownTradeIntern].Stone + " (+" + m_Player.Towns[m_CurrentTownTradeIntern].StoneInc + "), Silver: " + m_Player.Towns[m_CurrentTownTradeIntern].Iron + " (+" + m_Player.Towns[m_CurrentTownTradeIntern].IronInc + "))");
                    checkTradeNext(true);
                }
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkSpy(): " + e.Message);
            }
        }

        private void checkSpyResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                int l_Index = 0;
                string l_Search = "";
                string l_Error = "";

                int l_ValidCode = validateResponse(p_Response, "checkSpyResponse");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        setStatusBarEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Filling spy cave.");

                        //Server time
                        updateServerTime("_srvtime", p_Response);
                        addNotifications(p_Response);

                        stateManagerDelay();
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkSpyResponse(): " + l_Error);
                        }
                        //Ingame error. Goto next sender.
                        checkTradeNext(true);
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
            }
            catch (Exception e)
            {
                setStatusBarEvent("Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkSpyResponse", e.Message + "\n" + p_Response);
                
                //Parse error. Goto next sender.
                checkTradeNext(true);
            }
        }

        /*
         * Server request that stores silver in cave
         */ 
        private void checkSpy2()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            m_State = "checkspy2";

            try
            {
                string l_TownId = m_Player.Towns[m_CurrentTownTradeIntern].TownID;
                int l_IronToStore = m_Player.Towns[m_CurrentTownTradeIntern].Iron - m_Player.Towns[m_CurrentTownTradeIntern].TradeRemainingResources;

                //http://###.grepolis.com/game/frontend_bridge?town_id=###&action=execute&h=###
                //json={"model_url":"BuildingHide","action_name":"storeIron","arguments":{"iron_to_store":1000},"town_id":117455,"nlreq_id":174854100}
                NameValueCollection l_Content = new NameValueCollection();
                string l_Url = "http://" + l_Settings.GenServer + "/game/frontend_bridge?town_id=" + l_TownId + "&action=execute&h=" + m_H;
                Uri l_Uri = new Uri(l_Url);
                //l_Content.Add("json", "{\"model_url\":\"BuildingHide\",\"action_name\":\"storeIron\",\"arguments\":{\"iron_to_store\":" + l_IronToStore.ToString() + "},\"town_id\":" + l_TownId + ",\"nlreq_id\":" + m_Nlreq_id + "}");
                l_Content.Add("json", "{\"model_url\":\"BuildingHide\",\"action_name\":\"storeIron\",\"arguments\":{\"iron_to_store\":" + l_IronToStore.ToString() + "},\"town_id\":" + l_TownId + ",\"nl_init\":true}");

                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                m_HttpHandler.Headers.Remove("X-Requested-With");
            }
            catch (Exception e)
            {
                startReconnect(0);
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in checkSpy2(): " + e.Message);
            }
        }

        private void checkSpy2Response(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;
            Parser l_Parser = Parser.Instance;

            try
            {
                int l_Index = 0;
                string l_Search = "";
                string l_Error = "";

                int l_ValidCode = validateResponse(p_Response, "checkSpy2Response");
                if (l_ValidCode == 1)
                {
                    if (!p_Response.Contains("{\"error\":\""))
                    {
                        setStatusBarEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Storing silver in cave (" + (m_Player.Towns[m_CurrentTownTradeIntern].Iron - m_Player.Towns[m_CurrentTownTradeIntern].TradeRemainingResources).ToString() + ").");
                        logEvent(m_Player.Towns[m_CurrentTownTradeIntern].Name + ": Storing silver in cave. (Deposit: " + (m_Player.Towns[m_CurrentTownTradeIntern].Iron - m_Player.Towns[m_CurrentTownTradeIntern].TradeRemainingResources).ToString() + ", Wood: " + m_Player.Towns[m_CurrentTownTradeIntern].Wood + " (+" + m_Player.Towns[m_CurrentTownTradeIntern].WoodInc + "), Stone: " + m_Player.Towns[m_CurrentTownTradeIntern].Stone + " (+" + m_Player.Towns[m_CurrentTownTradeIntern].StoneInc + "), Silver: " + m_Player.Towns[m_CurrentTownTradeIntern].Iron + " (+" + m_Player.Towns[m_CurrentTownTradeIntern].IronInc + ")).");

                        updateResourcesFromNotification(p_Response, true);

                        //Server time
                        updateServerTime("_srvtime", p_Response);

                        checkTradeNext(true);
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                        {
                            l_Search = "{\"error\":\"";
                            l_Index = p_Response.IndexOf(l_Search, 0);
                            l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                            l_Error = l_Parser.fixSpecialCharacters(l_Error);

                            l_IOHandler.debug("Ingame error in checkSpy2Response(): " + l_Error);
                        }
                        //Ingame error. Goto next sender.
                        checkTradeNext(true);
                    }
                }
                else
                {
                    processValidatedResponse(l_ValidCode);
                }
            }
            catch (Exception e)
            {
                setStatusBarEvent("Critical error occurred. Server response saved in Response dir.");
                l_IOHandler.debug("Critical error occurred. Server response saved in Response dir.");

                if (l_Settings.AdvDebugMode)
                    l_IOHandler.saveServerResponse("checkSpy2Response", e.Message + "\n" + p_Response);

                //Parse error. Goto next sender.
                checkTradeNext(true);
            }
        }

        /*
         * 9kw Api method
         */ 
        public string captchaBalanceQuerySingle()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            try
            {
                string l_Action = "usercaptchaguthaben";

                string l_Url = "http://www.9kw.eu/index.cgi?action=" + l_Action + "&apikey=" + l_Settings.CaptchaApikey;
                Uri l_Uri = new Uri(l_Url);

                return m_HttpHandlerCaptcha.DownloadString(l_Uri);
            }
            catch (Exception e)
            {
                l_IOHandler.debug("Exception in captchaBalanceQuerySingle(): " + e.Message);
                return e.Message;
            }
        }

        /*
         * 9kw Api method
         */
        private void captcha9kwServiceStatus()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            m_State = "captcha9kwServiceStatus";

            try
            {
                string l_Action = "userservercheck";

                string l_Url = "http://www.9kw.eu/index.cgi?action=" + l_Action;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandlerCaptcha.DownloadStringAsync(l_Uri);
            }
            catch (Exception e)
            {
                l_IOHandler.debug("Exception in captcha9kwServiceStatus(): " + e.Message);
            }
        }

        private void captcha9kwServiceStatusResponse(string p_Response)
        {
            /*
            0001 API key doesn't exist
            0002 API key not found
            0003 Active API key not found
            0004 API key deactivated by owner
            0005 No user found
            0006 No data found
            0007 No ID found
            0008 No captcha found
            0009 No image found
            0010 Image size not allowed
            0011 Balance insufficient
            0012 Already done.
            0013 No answer found.
            0014 Captcha already answered.
            */

            if (p_Response.StartsWith("00"))
            {
                logEvent("Unable to get server status: " + p_Response);
            }
            else
            {
                //worker=7|avg24h=16s|avg1h=20s|avg15m=18s|avg5m=23s|inwork=18|queue=11|workermouse=3|workerconfirm=1|
                m_CaptchaCurrentWorkers = -1;
                m_CaptchaCurrentQueue = 100;
                string l_Search = "worker=";
                string l_Worker = "";
                string l_Queue = "";
                int l_Index = p_Response.IndexOf(l_Search);
                if (l_Index != -1)
                {
                    l_Worker = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("|", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                    l_Search = "queue=";
                    l_Index = p_Response.IndexOf(l_Search,l_Index + l_Search.Length);
                    l_Queue = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("|", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                    m_CaptchaCurrentWorkers = int.Parse(l_Worker);
                    m_CaptchaCurrentQueue = int.Parse(l_Queue);
                }
                logEvent(p_Response);
                stateManagerDelayCaptcha();
            }
        }

        /*
         * 9kw Api method
         */
        private void captchaBalanceQuery()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            m_State = "captchaBalanceQuery";

            try
            {
                string l_Action = "usercaptchaguthaben";

                string l_Url = "http://www.9kw.eu/index.cgi?action=" + l_Action + "&apikey=" + l_Settings.CaptchaApikey;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandlerCaptcha.DownloadStringAsync(l_Uri);
            }
            catch (Exception e)
            {
                l_IOHandler.debug("Exception in captchaBalanceQuery(): " + e.Message);
            }
        }

        private void captchaBalanceQueryResponse(string p_Response)
        {
            /*
            0001 API key doesn't exist
            0002 API key not found
            0003 Active API key not found
            0004 API key deactivated by owner
            0005 No user found
            0006 No data found
            0007 No ID found
            0008 No captcha found
            0009 No image found
            0010 Image size not allowed
            0011 Balance insufficient
            0012 Already done.
            0013 No answer found.
            0014 Captcha already answered.
            */

            if (p_Response.StartsWith("00"))
            {
                logEvent("Unable to get captcha balance: " + p_Response);
            }
            else
            {
                logEvent("Current captcha balance: " + p_Response);
                int l_Balance = 0;
                int.TryParse(p_Response, out l_Balance);
                if (l_Balance < 10)
                {
                    logEvent("Your captcha balance is too low.");
                }
                else
                {
                    stateManagerDelayCaptcha();
                }
            }
        }

        /*
         * 9kw Api method
         */
        private void captchaSubmitCaptcha()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            m_State = "captchaSubmitCaptcha";

            try
            {
                m_State = "captchaSubmitCaptcha";
                string l_Action = "usercaptchaupload";
                Image l_Image = new Bitmap(m_CaptchaImage);

                //Base64
                string l_Base64 = CaptchaHandler.imageToBase64(l_Image, System.Drawing.Imaging.ImageFormat.Png);

                NameValueCollection l_Content = new NameValueCollection();
                string l_Url = "http://www.9kw.eu/index.cgi";
                Uri l_Uri = new Uri(l_Url);
                l_Content.Add("apikey", l_Settings.CaptchaApikey);
                l_Content.Add("action", l_Action);
                l_Content.Add("file-upload-01", l_Base64);
                l_Content.Add("source", "grepolis2bot");
                l_Content.Add("oldsource", "recaptcha");
                l_Content.Add("base64", "1");
                if(l_Settings.CaptchaPriority > 0 && l_Settings.CaptchaPriority <= 10)
                    l_Content.Add("prio", l_Settings.CaptchaPriority.ToString());
                l_Content.Add("maxtimeout", "300");
                l_Content.Add("nomd5", "1");
                if(l_Settings.CaptchaSelfSolve)
                    l_Content.Add("selfsolve", "1");
                if (m_CaptchaType.Equals("math"))
                {
                    l_Content.Add("math", "1");
                    l_Content.Add("numeric", "1");
                    l_Content.Add("min_len", "1");
                    l_Content.Add("max_len", "3");
                }
                else
                {
                    //Removed because reCaptcha can consists of numeric captchas.
                    //l_Content.Add("phrase", "1");
                }

                m_HttpHandlerCaptcha.UploadValuesAsync(l_Uri, l_Content);
            }
            catch (Exception e)
            {
                l_IOHandler.debug("Exception in captchaSubmitCaptcha(): " + e.Message);
            }
        }

        private void captchaSubmitCaptchaResponse(string p_Response)
        {
            /*
            0001 API key doesn't exist
            0002 API key not found
            0003 Active API key not found
            0004 API key deactivated by owner
            0005 No user found
            0006 No data found
            0007 No ID found
            0008 No captcha found
            0009 No image found
            0010 Image size not allowed
            0011 Balance insufficient
            0012 Already done.
            0013 No answer found.
            0014 Captcha already answered.
            0015 Captcha zu schnell eingereicht. /  Captcha submitted too quickly.
            */
            if (p_Response.StartsWith("00"))
            {
                logEvent("Unable to submit captcha: " + p_Response);
                if (p_Response.StartsWith("0015"))
                {
                    m_State = "captchaSubmitCaptchaERROR";
                    stateManagerDelayCaptcha();
                }
            }
            else
            {
                logEvent("Captcha submitted to server with id: " + p_Response);
                m_CaptchaID = p_Response;
                stateManagerDelayCaptcha();
            }
        }

        /*
         * 9kw Api method
         */
        private void captchaCaptchaData()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            m_State = "captchaCaptchaData";

            try
            {
                string l_Action = "usercaptchacorrectdata";

                string l_Url = "http://www.9kw.eu/index.cgi?action=" + l_Action + "&id=" + m_CaptchaID + "&apikey=" + l_Settings.CaptchaApikey;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandlerCaptcha.DownloadStringAsync(l_Uri);
            }
            catch (Exception e)
            {
                l_IOHandler.debug("Exception in captchaCaptchaData(): " + e.Message);
            }
        }

        private void captchaCaptchaDataResponse(string p_Response)
        {
            /*
            0001 API key doesn't exist
            0002 API key not found
            0003 Active API key not found
            0004 API key deactivated by owner
            0005 No user found
            0006 No data found
            0007 No ID found
            0008 No captcha found
            0009 No image found
            0010 Image size not allowed
            0011 Balance insufficient
            0012 Already done.
            0013 No answer found.
            0014 Captcha already answered.
            */
            if (p_Response.StartsWith("00"))
            {
                logEvent("Unable to get captcha data: " + p_Response);
            }
            else if (p_Response.Contains("ERROR NO USER"))
            {
                logEvent("No user available to solve captcha: " + p_Response);
                //Restart Captcha sequence
                //startCaptchaSequence(m_CaptchaImage, m_CaptchaType);
                m_CaptchaAnswer = "";
                m_State = "captchaCaptchaDataERROR";
                stateManagerDelayCaptcha();
            }
            else
            {
                if (p_Response.Equals(""))
                {
                    logEvent("Captcha answer not ready.");
                    stateManagerDelayCaptcha();
                }
                else
                {
                    logEvent("Captcha answer received: " + p_Response);
                    m_CaptchaAnswer = p_Response;
                    CustomArgs l_CustomArgs = new CustomArgs("");
                    captchaAnswerReady(this, l_CustomArgs);
                }
            }
        }

        /*
         * 9kw Api method
         */
        public void captchaCaptchaCorrect()
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            m_State = "captchaCaptchaCorrect";

            try
            {
                string l_Action = "usercaptchacorrectback";
                string l_Correct = "1";
                if (!m_CaptchaAnswerCorrect)
                    l_Correct = "2";

                string l_Url = "http://www.9kw.eu/index.cgi?action=" + l_Action + "&correct=" + l_Correct + "&id=" + m_CaptchaID + "&apikey=" + l_Settings.CaptchaApikey;
                Uri l_Uri = new Uri(l_Url);

                m_HttpHandlerCaptcha.DownloadStringAsync(l_Uri);
            }
            catch (Exception e)
            {
                l_IOHandler.debug("Exception in captchaCaptchaCorrect(): " + e.Message);
            }
        }

        private void captchaCaptchaCorrectResponse(string p_Response)
        {
            CustomArgs l_CustomArgs = new CustomArgs("");
            captchaAnswerModerated(this, l_CustomArgs);
        }

        /*
         * Server request that sends captcha answer
         */ 
        public void captchaSendToGrepolis()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            m_State = "captchaSendToGrepolis";
            int l_CurrentTownIntern = m_CurrentTownIntern;
            if (l_CurrentTownIntern >= m_Player.Towns.Count || l_CurrentTownIntern < 0)
                l_CurrentTownIntern = 0;

            try
            {
                NameValueCollection l_Content = new NameValueCollection();
                String l_Url = "http://" + l_Settings.GenServer + "/game/bot_check?action=confirm&town_id=" + m_Player.Towns[l_CurrentTownIntern].TownID + "&h=" + m_H;

                if (m_CaptchaType.Equals("math"))
                {
                    //l_Content.Add("json", "{\"code\": \"" + m_CaptchaAnswer + "\",\"town_id\":" + m_Player.Towns[l_CurrentTownIntern].TownID + ",\"nlreq_id\":" + m_Nlreq_id + "}");//{"code":"###","town_id":#####,"nlreq_id":######}
                    l_Content.Add("json", "{\"code\": \"" + m_CaptchaAnswer + "\",\"town_id\":" + m_Player.Towns[l_CurrentTownIntern].TownID + ",\"nl_init\":true}");
                }
                else
                {
                    //l_Content.Add("json", "{\"code\":\"{\\\"challenge\\\":\\\"" + m_CaptchaReCaptchaID + "\\\",\\\"response\\\":\\\"" + m_CaptchaAnswer + "\\\"}\",\"town_id\":" + m_Player.Towns[l_CurrentTownIntern].TownID + ",\"nlreq_id\":" + m_Nlreq_id + "}");//{"code":"{\"challenge\":\"###############\",\"response\":\"############\"}","town_id":#####,"nlreq_id":########}
                    l_Content.Add("json", "{\"code\":\"{\\\"challenge\\\":\\\"" + m_CaptchaReCaptchaID + "\\\",\\\"response\\\":\\\"" + m_CaptchaAnswer + "\\\"}\"}");
                }
                
                Uri l_Uri = new Uri(l_Url);
                m_HttpHandler.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_HttpHandler.UploadValuesAsync(l_Uri, l_Content);
                m_HttpHandler.Headers.Remove("X-Requested-With");  
            }
            catch (Exception e)
            {
                l_IOHandler.debug("Exception in captchaSendToGrepolis(): " + e.Message);
            }
        }

        public void captchaSendToGrepolisResponse(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Parser l_Parser = Parser.Instance;

            string l_Search = "";
            string l_Error = "";
            int l_Index = -1;

            if (!p_Response.Contains("{\"error\":\""))
            {
                //Captcha answer correct
                m_CaptchaAnswerCorrect = true;
                m_CaptchaAnswerInCorrectCount = 0;

                CustomArgs l_CustomArgs = new CustomArgs("");
                captchaAnswerSendToGrepolisCorrect(this, l_CustomArgs);
            }
            else
            {
                //Captcha answer incorrect
                m_CaptchaAnswerCorrect = false;
                m_CaptchaAnswerInCorrectCount++;

                CustomArgs l_CustomArgs = new CustomArgs("");
                captchaAnswerSendToGrepolisInCorrect(this, l_CustomArgs);

                l_Search = "{\"error\":\"";
                l_Index = p_Response.IndexOf(l_Search, 0);
                l_Error = p_Response.Substring(l_Index + l_Search.Length, p_Response.IndexOf("\"", l_Index + l_Search.Length) - (l_Index + l_Search.Length));
                l_Error = l_Parser.fixSpecialCharacters(l_Error);

                l_IOHandler.debug("Ingame error in captchaSendToGrepolisResponse(): " + l_Error + " (#" + m_CaptchaAnswerInCorrectCount + ")");
                logEvent(l_Error + " (#"+m_CaptchaAnswerInCorrectCount+")");
            }
        }

        #endregion

        //-->Event handlers

        /*
         * When an exception occurs this method will be called to repeat the latest server request.
         * All requests will only be repeated once.
         */ 
        private void retryManager()
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            //Cancels active request
            if (m_HttpHandler.IsBusy)
            {
                m_HttpHandler.CancelAsync();
                l_IOHandler.debug("Active request canceled. (retryManager)");
            }

            m_RetryCount++;
            m_TimeoutTimer.Stop();
            m_TimeoutTimer.Start();
            switch (m_State)
            {
                case "updateGameData":
                    updateGameData();
                    break;
                case "switchtown":
                    switchTown();
                    break;
                case "buildingBuildData":
                    buildingBuildData();
                    break;
                case "locatefarmers":
                    locateFarmers();
                    break;
                case "checkbuildingqueuepreteardown":
                    checkBuildingQueuePreTearDown();
                    break;
                case "updateculturalinfo":
                    updateCulturalInfo();
                    break;
                case "checkculturefestivals":
                    checkCultureFestivals();
                    break;
                case "updatemilitia":
                    updateMilitia();
                    break;
                case "checkmilitia":
                    checkMilitia();
                    break;
                case "attackfarmers1":
                    attackFarmers1();
                    break;
                case "attackfarmers2":
                    attackFarmers2();
                    break;
                case "attackfarmersall1":
                    attackFarmersAll1();
                    break;
                case "attackfarmersall2":
                    attackFarmersAll2();
                    break;
                case "attackfarmersall3":
                    attackFarmersAll3();
                    break;
                case "checkbuildingqueue":
                    checkBuildingQueue();
                    break;
                case "checkprelandarmyqueue":
                    checkPreLandArmyQueue();
                    break;
                case "checklandarmyqueue":
                    checkLandArmyQueue();
                    break;
                case "checkprenavyarmyqueue":
                    checkPreNavyArmyQueue();
                    break;
                case "checknavyarmyqueue":
                    checkNavyArmyQueue();
                    break;
                //Trading
                case "checktrade":
                    checkTrade();
                    break;
                case "checktrade2":
                    checkTrade();//checkTrade2 not possible, you need to start by part one again.
                    break;
                case "checkspy":
                    checkSpy();
                    break;
                case "checkspy2":
                    checkSpy();//checkSpy2 not possible, you need to start by part one again.
                    break;
            }
        }

        /*
         * This method is called after every server response.
         * It adds a short delay before starting the next server request.
         */ 
        private void stateManagerDelay()
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            //Cancels active request
            if (m_HttpHandler.IsBusy)
            {
                m_HttpHandler.CancelAsync();
                l_IOHandler.debug("Active request canceled. (stateManagerDelay)");
            }

            Random l_Random = new Random();
            int l_DelayFarmers = l_Random.Next(l_Settings.AdvMinDelayFarmers, l_Settings.AdvMaxDelayFarmers);
            int l_Delay = l_Random.Next(l_Settings.AdvMinDelayRequests, l_Settings.AdvMaxDelayRequests);

            if (m_State.Equals("attackfarmers1") || m_State.Equals("attackfarmers2") || m_State.Equals("attackfarmersnext") ||
                m_State.Equals("attackfarmersall1") || m_State.Equals("attackfarmersall2") || m_State.Equals("attackfarmersall3"))
                m_RequestTimer.Interval = l_DelayFarmers;
            else
                m_RequestTimer.Interval = l_Delay;

            CustomArgs l_CustomArgs = new CustomArgs("");
            serverRequestDelayRequested(this, l_CustomArgs);
        }

        /*
         * This method is called after every server response (CAPTCHA related).
         * It adds a short delay before starting the next server request.
         */
        private void stateManagerDelayCaptcha()
        {
            Settings l_Settings = Settings.Instance;

            if (m_State.Equals("captchaSubmitCaptcha") || m_State.Equals("captchaCaptchaData"))
                m_RequestTimerCaptcha.Interval = 5000;
            else
                m_RequestTimerCaptcha.Interval = 1000;

            if (m_State.Equals("captcha9kwServiceStatus"))
            {
                if ((m_CaptchaCurrentWorkers < l_Settings.CaptchaMinWorkers || m_CaptchaCurrentQueue > l_Settings.CaptchaMaxQueue) && m_CaptchaCurrentWorkers < l_Settings.CaptchaMinWorkersALT)
                {
                    m_State = "captcha9kwServiceStatusRetry";
                    m_RequestTimerCaptcha.Interval = l_Settings.CaptchaExtraDelay * 1000;
                }
            }
            else if (m_State.Equals("captchaCaptchaDataERROR"))
            {
                m_RequestTimerCaptcha.Interval = l_Settings.CaptchaExtraDelay * 1000;
            }
            else if (m_State.Equals("captchaSubmitCaptchaERROR"))
            {
                m_RequestTimerCaptcha.Interval = l_Settings.CaptchaExtraDelay * 1000;
            }

            CustomArgs l_CustomArgs = new CustomArgs("");
            serverRequestCaptchaDelayRequested(this, l_CustomArgs);
        }

        /*
         * Switches between the two server request sequences,
         * as set by the method randomizeStateManagerMode()
         */
        private void stateManager()
        {
            if (!m_RequestedToPauseBot)
            {
                //Should be enough randomizing for the time being.
                m_TimeoutTimer.Start();
                if (m_State.Equals("checktrade") || m_State.Equals("checktrade2") || m_State.Equals("checkspy") || m_State.Equals("checkspy2"))
                {
                    stateManagerTrade();
                }
                else
                {
                    switch (m_StateManagerMode)
                    {
                        case 0:
                            stateManager1();
                            break;
                        case 1:
                            stateManager2();
                            break;
                    }
                }
            }
            else
            {
                setGuiToTimeoutProcessedStateEvent();
            }
        }

        /*
         * Manages the sequence of server requests related to trading
         */ 
        private void stateManagerTrade()
        {
            switch (m_State)
            {
                case "checktrade":
                    checkTrade2();
                    break;
                case "checktrade2":
                    checkTrade();
                    break;
                case "checkspy":
                    checkSpy2();
                    break;
            }
        }

        /*
         * Manages the sequence of server requests related to captcha
         */ 
        private void stateManagerCaptcha()
        {
            switch (m_State)
            {
                case "captcha9kwServiceStatusRetry":
                    captcha9kwServiceStatus();
                    break;
                case "captcha9kwServiceStatus":
                    captchaBalanceQuery();
                    break;
                case "captchaBalanceQuery":
                    captchaSubmitCaptcha();
                    break;
                case "captchaSubmitCaptcha":
                    captchaCaptchaData();
                    break;
                case "captchaCaptchaData":
                    captchaCaptchaData();
                    break;
                case "captchaCaptchaDataERROR":
                    startCaptchaSequence(m_CaptchaImage, m_CaptchaType);
                    break;
                case "captchaSubmitCaptchaERROR":
                    startCaptchaSequence(m_CaptchaImage, m_CaptchaType);
                    break;
            }
        }

        /*
         * Manages the sequence of server requests - variant 1
         */ 
        private void stateManager1()
        {
            Settings l_Settings = Settings.Instance;

            switch (m_State)
            {
                case "switchtown":
                    buildingBuildData();
                    break;
                case "buildingBuildData":
                    locateFarmers();
                    break;
                case "locatefarmers":
                    updateCulturalInfo();
                    break;
                case "updateculturalinfo":
                    checkCultureFestivals();
                    break;
                case "checkculturefestivals":
                    updateMilitia();
                    break;
                case "updatemilitia":
                    checkMilitia();
                    break;
                case "checkmilitia":
                    if (l_Settings.PreUseFarmAllFeature && (long.Parse(ServerTime) - long.Parse(m_Player.CaptainActive) < 0))
                    {
                        //This should be the last call when randomizing!!! 
                        //The building/unit queue requests starting after farming are always in the same order.
                        attackFarmersAll1();
                    }
                    else
                    {
                        //This should be the last call when randomizing!!!
                        //The building/unit queue requests starting after farming are always in the same order.
                        attackFarmers1();
                    }
                    break;
                case "attackfarmers1":
                    attackFarmers2();
                    break;
                case "attackfarmersnext":
                    attackFarmers1();
                    break;
                case "attackfarmers2":
                    if (m_TimeoutOnQueueTimer && !m_CulturalFestivalStarted)
                    {
                        checkBuildingQueue();
                    }
                    else
                    {
                        final();
                    }
                    break;
                case "attackfarmersall1":
                    attackFarmersAll2();
                    break;
                case "attackfarmersall2":
                    attackFarmersAll3();
                    break;
                case "attackfarmersall3":
                    if (m_TimeoutOnQueueTimer && !m_CulturalFestivalStarted)
                    {
                        checkBuildingQueue();
                    }
                    else
                    {
                        final();
                    }
                    break;
                case "checkbuildingqueuepreteardown":
                    checkBuildingQueueTearDown();
                    break;
                case "checkbuildingqueueteardown":
                    final();
                    break;
                case "checkbuildingqueue":
                    final();
                    break;
                case "checkbuildingqueueSKIP":
                    checkPreLandArmyQueue();
                    break;
                case "checkprelandarmyqueue":
                    checkLandArmyQueue();
                    break;
                case "checklandarmyqueue":
                    final();
                    break;
                case "checklandarmyqueueSKIP":
                    checkPreNavyArmyQueue();
                    break;
                case "checkprenavyarmyqueue":
                    checkNavyArmyQueue();
                    break;
                case "checknavyarmyqueue":
                    final();
                    break;
                case "checknavyarmyqueueSKIP":
                    final();
                    break;
            }
        }

        /*
         * Manages the sequence of server requests - variant 2
         */ 
        private void stateManager2()
        {
            Settings l_Settings = Settings.Instance;

            switch (m_State)
            {
                case "switchtown":
                    buildingBuildData();
                    break;
                case "buildingBuildData":
                    locateFarmers();
                    break;
                case "locatefarmers":
                    updateMilitia();
                    break;
                case "updatemilitia":
                    checkMilitia();
                    break;
                case "checkmilitia":
                    updateCulturalInfo();
                    break;
                case "updateculturalinfo":
                    checkCultureFestivals();
                    break;
                case "checkculturefestivals":
                    if (l_Settings.PreUseFarmAllFeature && (long.Parse(ServerTime) - long.Parse(m_Player.CaptainActive) < 0))
                    {
                        //This should be the last call when randomizing!!! 
                        //The building/unit queue requests starting after farming are always in the same order.
                        attackFarmersAll1();//This should be the last call when randomizing!!!
                    }
                    else
                    {
                        //This should be the last call when randomizing!!! 
                        //The building/unit queue requests starting after farming are always in the same order.
                        attackFarmers1();//This should be the last call when randomizing!!!
                    }
                    break;
                case "attackfarmers1":
                    attackFarmers2();
                    break;
                case "attackfarmersnext":
                    attackFarmers1();
                    break;
                case "attackfarmers2":
                    if (m_TimeoutOnQueueTimer && !m_CulturalFestivalStarted)
                    {
                        checkBuildingQueue();
                    }
                    else
                    {
                        final();
                    }
                    break;
                case "attackfarmersall1":
                    attackFarmersAll2();
                    break;
                case "attackfarmersall2":
                    attackFarmersAll3();
                    break;
                case "attackfarmersall3":
                    if (m_TimeoutOnQueueTimer && !m_CulturalFestivalStarted)
                    {
                        checkBuildingQueue();
                    }
                    else
                    {
                        final();
                    }
                    break;
                case "checkbuildingqueuepreteardown":
                    checkBuildingQueueTearDown();
                    break;
                case "checkbuildingqueueteardown":
                    final();
                    break;
                case "checkbuildingqueue":
                    final();
                    break;
                case "checkbuildingqueueSKIP":
                    checkPreLandArmyQueue();
                    break;
                case "checkprelandarmyqueue":
                    checkLandArmyQueue();
                    break;
                case "checklandarmyqueue":
                    final();
                    break;
                case "checklandarmyqueueSKIP":
                    checkPreNavyArmyQueue();
                    break;
                case "checkprenavyarmyqueue":
                    checkNavyArmyQueue();
                    break;
                case "checknavyarmyqueue":
                    final();
                    break;
                case "checknavyarmyqueueSKIP":
                    final();
                    break;
            }
        }

        /*
         * Handles which method should parse the server response received from the Grepolis server
         */ 
        private void responseManager(string p_Response)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            //Cancels active request
            if (m_HttpHandler.IsBusy)
            {
                m_HttpHandler.CancelAsync();
                l_IOHandler.debug("Active request canceled. (responseManager)");
            }

            string l_Response = p_Response;
            m_TimeoutTimer.Stop();

            if (p_Response.Contains("”"))
                l_Response = p_Response.Replace("”", "\"");

            switch (m_State)
            {
                case "updateGameData":
                    updateGameDataResponse(l_Response);
                    break;
                case "switchtown":
                    switchTownResponse(l_Response);
                    break;
                case "buildingBuildData":
                    buildingBuildDataResponse(l_Response);
                    break;
                case "locatefarmers":
                    locateFarmersResponse(l_Response);
                    break;
                case "checkbuildingqueuepreteardown":
                    checkBuildingQueuePreTearDownResponse(l_Response);
                    break;
                case "updateculturalinfo":
                    updateCulturalInfoResponse(l_Response);
                    break;
                case "checkculturefestivals":
                    checkCultureFestivalsResponse(l_Response);
                    break;
                case "updatemilitia":
                    updateMilitiaResponse(l_Response);
                    break;
                case "checkmilitia":
                    checkMilitiaResponse(l_Response);
                    break;
                case "attackfarmers1":
                    attackFarmers1Response(l_Response);
                    break;
                case "attackfarmersall1":
                    attackFarmersAll1Response(l_Response);
                    break;
                case "attackfarmers2":
                    attackFarmers2Response(l_Response);
                    break;
                case "attackfarmersall2":
                    attackFarmersAll2Response(l_Response);
                    break;
                case "attackfarmersall3":
                    attackFarmersAll3Response(l_Response);
                    break;
                case "checkbuildingqueue":
                    checkBuildingQueueResponse(l_Response);
                    break;
                case "checkbuildingqueueteardown":
                    checkBuildingQueueTearDownResponse(l_Response);
                    break;
                case "checkprelandarmyqueue":
                    checkPreLandArmyQueueResponse(l_Response);
                    break;
                case "checklandarmyqueue":
                    checkLandArmyQueueResponse(l_Response);
                    break;
                case "checkprenavyarmyqueue":
                    checkPreNavyArmyQueueResponse(l_Response);
                    break;
                case "checknavyarmyqueue":
                    checkNavyArmyQueueResponse(l_Response);
                    break;
                //Trading
                case "checktrade":
                    checkTradeResponse(l_Response);
                    break;
                case "checktrade2":
                    checkTrade2Response(l_Response);
                    break;
                case "checkspy":
                    checkSpyResponse(l_Response);
                    break;
                case "checkspy2":
                    checkSpy2Response(l_Response);
                    break;
                //Captcha
                case "captcha9kwServiceStatus":
                    captcha9kwServiceStatusResponse(l_Response);
                    break;
                case "captchaBalanceQuery":
                    captchaBalanceQueryResponse(l_Response);
                    break;
                case "captchaSubmitCaptcha":
                    captchaSubmitCaptchaResponse(l_Response);
                    break;
                case "captchaCaptchaData":
                    captchaCaptchaDataResponse(l_Response);
                    break;
                case "captchaCaptchaCorrect":
                    captchaCaptchaCorrectResponse(l_Response);
                    break;
                case "captchaSendToGrepolis":
                    captchaSendToGrepolisResponse(l_Response);
                    break;
            }
        }

        /*
         * Handles all the server responses from the Grepolis server requested as POST
         */ 
        private void m_HttpHandler_UploadValuesCompleted(object sender, System.Net.UploadValuesCompletedEventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            if (e.Error == null && !e.Cancelled)
            {
                string l_Response = Encoding.Default.GetString(e.Result);
                if (m_State.Equals("captchaSendToGrepolis"))
                {
                    responseManager(l_Response);
                }
                else
                {
                    m_RetryCount = 0;
                    if (l_Settings.AdvOutputAllMode)
                        l_IOHandler.saveServerResponse2(m_State, l_Response);
                    responseManager(l_Response);
                }
            }
            else if (e.Cancelled)
            {
                if (!m_State.Equals("captchaSendToGrepolis"))
                    retryManager();
            }
            else
            {
                if (!m_State.Equals("captchaSendToGrepolis"))
                {
                    if (m_RetryCount >= 1)
                    {
                        startReconnect(2);
                        if (l_Settings.AdvDebugMode)
                            l_IOHandler.debug("Exception in m_HttpHandler_UploadValuesCompleted(): " + "(" + m_State + "#" + m_RetryCount.ToString() + ") " + e.Error.Message);
                    }
                    else
                    {
                        if (l_Settings.AdvDebugMode)
                            l_IOHandler.debug("Exception in m_HttpHandler_UploadValuesCompleted(): " + "(" + m_State + "#" + m_RetryCount.ToString() + ") " + e.Error.Message);
                        retryManager();
                    }
                }
                else
                {
                    if (l_Settings.AdvDebugMode)
                        l_IOHandler.debug("Exception in m_HttpHandler_UploadValuesCompleted(): " + "(" + m_State + ") " + e.Error.Message);
                }
            }
        }

        /*
         * Handles all the server responses from the Grepolis server requested as GET
         */ 
        private void m_HttpHandler_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            if (e.Error == null && !e.Cancelled)
            {
                m_RetryCount = 0;
                string l_Response = e.Result;
                if(l_Settings.AdvOutputAllMode)
                    l_IOHandler.saveServerResponse2(m_State, l_Response);
                responseManager(l_Response);
            }
            else if (e.Cancelled)
            {
                retryManager();
            }
            else
            {
                if (m_RetryCount >= 1)
                {
                    startReconnect(2);
                    if (l_Settings.AdvDebugMode)
                        l_IOHandler.debug("Exception in m_HttpHandler_DownloadStringCompleted(): " + "(" + m_State + "#" + m_RetryCount.ToString() + ") " + e.Error.Message);
                }
                else
                {
                    if (l_Settings.AdvDebugMode)
                        l_IOHandler.debug("Exception in m_HttpHandler_DownloadStringCompleted(): " + "(" + m_State + "#" + m_RetryCount.ToString() + ") " + e.Error.Message);
                    retryManager();
                }
            }
        }

        /*
         * Handles all the server responses from the 9kw server requested as POST
         */ 
        private void m_HttpHandlerCaptcha_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            if (e.Error == null && !e.Cancelled)
            {
                string l_Response = Encoding.Default.GetString(e.Result);
                m_CaptchaRetryCounter = 0;
                responseManager(l_Response);
                if (l_Settings.AdvOutputAllMode)
                    l_IOHandler.saveServerResponse2(m_State, l_Response);
            }
            else if (e.Cancelled)
            {
                l_IOHandler.debug("Exception in m_HttpHandlerCaptcha_UploadValuesCompleted(): " + "(" + m_State + ") Request canceled.");
            }
            else
            {
                l_IOHandler.debug("Exception in m_HttpHandlerCaptcha_UploadValuesCompleted(): " + "(" + m_State + ") " + e.Error.Message);
                if (m_CaptchaRetryCounter < 5)
                {
                    m_CaptchaRetryCounter++;
                    switch (m_State)
                    {
                        case "captcha9kwServiceStatus":
                            captcha9kwServiceStatus();
                            break;
                        case "captchaBalanceQuery":
                            captchaBalanceQuery();
                            break;
                        case "captchaSubmitCaptcha":
                            captchaSubmitCaptcha();
                            break;
                        case "captchaCaptchaData":
                            captchaCaptchaData();
                            break;
                        case "captchaCaptchaCorrect":
                            captchaCaptchaCorrect();
                            break;
                    }
                }
                else
                {
                    //Retries failed
                    logEvent("Can't connect to 9kw. Bot paused.");
                    CustomArgs l_CustomArgs = new CustomArgs("");
                    captchaSolver9kwDown(this, l_CustomArgs);
                }
            }
        }

        /*
         * Handles all the server responses from the 9kw server requested as GET
         */ 
        private void m_HttpHandlerCaptcha_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            if (e.Error == null && !e.Cancelled)
            {
                string l_Response = e.Result;
                m_CaptchaRetryCounter = 0;
                responseManager(l_Response);
                if (l_Settings.AdvOutputAllMode)
                    l_IOHandler.saveServerResponse2(m_State, l_Response);
            }
            else if (e.Cancelled)
            {
                l_IOHandler.debug("Exception in m_HttpHandlerCaptcha_DownloadStringCompleted(): " + "(" + m_State + ") Request canceled.");
            }
            else
            {
                l_IOHandler.debug("Exception in m_HttpHandlerCaptcha_DownloadStringCompleted(): " + "(" + m_State + ") " + e.Error.Message);
                if (m_CaptchaRetryCounter < 5)
                {
                    m_CaptchaRetryCounter++;
                    switch (m_State)
                    {
                        case "captcha9kwServiceStatus":
                            captcha9kwServiceStatus();
                            break;
                        case "captchaBalanceQuery":
                            captchaBalanceQuery();
                            break;
                        case "captchaSubmitCaptcha":
                            captchaSubmitCaptcha();
                            break;
                        case "captchaCaptchaData":
                            captchaCaptchaData();
                            break;
                        case "captchaCaptchaCorrect":
                            captchaCaptchaCorrect();
                            break;
                    }
                }
                else
                {
                    //Retries failed
                    logEvent("Can't connect to 9kw. Bot paused.");
                    CustomArgs l_CustomArgs = new CustomArgs("");
                    captchaSolver9kwDown(this, l_CustomArgs);
                }
            }
        }

        void m_HttpHandlerCheckVersion_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            IOHandler l_IOHandler = IOHandler.Instance;
            Settings l_Settings = Settings.Instance;

            if (e.Error == null)
            {
                string l_Response = e.Result;
                versionInfoEvent(l_Response);
            }
            else
            {
                if (l_Settings.AdvDebugMode)
                    l_IOHandler.debug("Exception in m_HttpHandlerCheckVersion_DownloadStringCompleted(): " + e.Error.Message);
            }

        }

        private void RefreshTimer_InternalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_IsBotRunning)
            {
                updateRefreshTimerEvent();
            }
            else
            {
                pause();
            }
        }

        private void QueueTimer_InternalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_IsBotRunning)
            {
                updateQueueTimerEvent();
            }
            else
            {
                pause();
            }
        }

        private void TradeTimer_InternalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_IsBotRunning)
            {
                updateTradeTimerEvent();
            }
            else
            {
                pause();
            }
        }

        private void ReconnectTimer_InternalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateReconnectTimerEvent();
        }

        private void ForcedReconnectTimer_InternalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateForcedReconnectTimerEvent();
        }

        private void ConnectedTimer_InternalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateConnectedTimerEvent();
        }

        private void m_RequestTimer_Tick(object sender, EventArgs e)
        {
            m_RequestTimer.Stop();
            stateManager();
        }

        private void m_RequestTimerCaptcha_Tick(object sender, EventArgs e)
        {
            m_RequestTimerCaptcha.Stop();
            stateManagerCaptcha();
        }

        private void m_TimeoutTimer_Tick(object sender, EventArgs e)
        {
            Settings l_Settings = Settings.Instance;
            IOHandler l_IOHandler = IOHandler.Instance;

            m_TimeoutTimer.Stop();
            m_TimeoutTimer.Interval = l_Settings.AdvTimeout * 1000;
            m_HttpHandler.CancelAsync();
            logEvent("Timeout - Server didn't respond for over " + l_Settings.AdvTimeout.ToString() + " seconds. (" + m_State + ")");
            if (l_Settings.AdvDebugMode)
                l_IOHandler.debug("Timeout - Server didn't respond for over " + l_Settings.AdvTimeout.ToString() + " seconds. (" + m_State + ")");
        }
    }
}