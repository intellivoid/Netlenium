namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Holds the information about all commands specified by the JSON wire protocol.
    /// This class cannot be inherited, as it is intended to be a singleton, and
    /// allowing subclasses introduces the possibility of multiple instances.
    /// </summary>
    public sealed class WebDriverWireProtocolCommandInfoRepository : CommandInfoRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverWireProtocolCommandInfoRepository"/> class.
        /// </summary>
        public WebDriverWireProtocolCommandInfoRepository()
            : base()
        {
            InitializeCommandDictionary();
        }

        /// <summary>
        /// Gets the level of the W3C WebDriver specification that this repository supports.
        /// </summary>
        public override int SpecificationLevel
        {
            get { return 0; }
        }

        /// <summary>
        /// Initializes the dictionary of commands for the CommandInfoRepository
        /// </summary>
        protected override void InitializeCommandDictionary()
        {
            TryAddCommand(DriverCommand.DefineDriverMapping, new CommandInfo(CommandInfo.PostCommand, "/config/drivers"));
            TryAddCommand(DriverCommand.Status, new CommandInfo(CommandInfo.GetCommand, "/status"));
            TryAddCommand(DriverCommand.NewSession, new CommandInfo(CommandInfo.PostCommand, "/session"));
            TryAddCommand(DriverCommand.GetSessionList, new CommandInfo(CommandInfo.GetCommand, "/sessions"));
            TryAddCommand(DriverCommand.GetSessionCapabilities, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}"));
            TryAddCommand(DriverCommand.Quit, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}"));
            TryAddCommand(DriverCommand.GetCurrentWindowHandle, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window_handle"));
            TryAddCommand(DriverCommand.GetWindowHandles, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window_handles"));
            TryAddCommand(DriverCommand.GetCurrentUrl, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/url"));
            TryAddCommand(DriverCommand.Get, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/url"));
            TryAddCommand(DriverCommand.GoForward, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/forward"));
            TryAddCommand(DriverCommand.GoBack, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/back"));
            TryAddCommand(DriverCommand.Refresh, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/refresh"));
            TryAddCommand(DriverCommand.ExecuteScript, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/execute"));
            TryAddCommand(DriverCommand.ExecuteAsyncScript, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/execute_async"));
            TryAddCommand(DriverCommand.Screenshot, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/screenshot"));
            TryAddCommand(DriverCommand.ElementScreenshot, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/screenshot/{id}"));
            TryAddCommand(DriverCommand.SwitchToFrame, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/frame"));
            TryAddCommand(DriverCommand.SwitchToParentFrame, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/frame/parent"));
            TryAddCommand(DriverCommand.SwitchToWindow, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window"));
            TryAddCommand(DriverCommand.GetAllCookies, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/cookie"));
            TryAddCommand(DriverCommand.AddCookie, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/cookie"));
            TryAddCommand(DriverCommand.DeleteAllCookies, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/cookie"));
            TryAddCommand(DriverCommand.DeleteCookie, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/cookie/{name}"));
            TryAddCommand(DriverCommand.GetPageSource, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/source"));
            TryAddCommand(DriverCommand.GetTitle, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/title"));
            TryAddCommand(DriverCommand.FindElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element"));
            TryAddCommand(DriverCommand.FindElements, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/elements"));
            TryAddCommand(DriverCommand.GetActiveElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/active"));
            TryAddCommand(DriverCommand.FindChildElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/element"));
            TryAddCommand(DriverCommand.FindChildElements, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/elements"));
            TryAddCommand(DriverCommand.DescribeElement, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}"));
            TryAddCommand(DriverCommand.ClickElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/click"));
            TryAddCommand(DriverCommand.GetElementText, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/text"));
            TryAddCommand(DriverCommand.SubmitElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/submit"));
            TryAddCommand(DriverCommand.SendKeysToElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/value"));
            TryAddCommand(DriverCommand.GetElementTagName, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/name"));
            TryAddCommand(DriverCommand.ClearElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/clear"));
            TryAddCommand(DriverCommand.IsElementSelected, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/selected"));
            TryAddCommand(DriverCommand.IsElementEnabled, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/enabled"));
            TryAddCommand(DriverCommand.IsElementDisplayed, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/displayed"));
            TryAddCommand(DriverCommand.GetElementLocation, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/location"));
            TryAddCommand(DriverCommand.GetElementLocationOnceScrolledIntoView, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/location_in_view"));
            TryAddCommand(DriverCommand.GetElementSize, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/size"));
            TryAddCommand(DriverCommand.GetElementValueOfCssProperty, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/css/{propertyName}"));
            TryAddCommand(DriverCommand.GetElementAttribute, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/attribute/{name}"));
            TryAddCommand(DriverCommand.ElementEquals, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/equals/{other}"));
            TryAddCommand(DriverCommand.Close, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/window"));
            TryAddCommand(DriverCommand.GetWindowSize, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window/{windowHandle}/size"));
            TryAddCommand(DriverCommand.SetWindowSize, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/{windowHandle}/size"));
            TryAddCommand(DriverCommand.GetWindowPosition, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window/{windowHandle}/position"));
            TryAddCommand(DriverCommand.SetWindowPosition, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/{windowHandle}/position"));
            TryAddCommand(DriverCommand.MaximizeWindow, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/{windowHandle}/maximize"));
            TryAddCommand(DriverCommand.MinimizeWindow, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/minimize"));
            TryAddCommand(DriverCommand.FullScreenWindow, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/fullscreen"));
            TryAddCommand(DriverCommand.GetOrientation, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/orientation"));
            TryAddCommand(DriverCommand.SetOrientation, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/orientation"));
            TryAddCommand(DriverCommand.DismissAlert, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/dismiss_alert"));
            TryAddCommand(DriverCommand.AcceptAlert, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/accept_alert"));
            TryAddCommand(DriverCommand.GetAlertText, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/alert_text"));
            TryAddCommand(DriverCommand.SetAlertValue, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/alert_text"));
            TryAddCommand(DriverCommand.SetAlertCredentials, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/alert/credentials"));
            TryAddCommand(DriverCommand.SetTimeouts, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/timeouts"));
            TryAddCommand(DriverCommand.ImplicitlyWait, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/timeouts/implicit_wait"));
            TryAddCommand(DriverCommand.SetAsyncScriptTimeout, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/timeouts/async_script"));

            // Advanced interactions commands
            TryAddCommand(DriverCommand.MouseClick, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/click"));
            TryAddCommand(DriverCommand.MouseDoubleClick, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/doubleclick"));
            TryAddCommand(DriverCommand.MouseDown, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/buttondown"));
            TryAddCommand(DriverCommand.MouseUp, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/buttonup"));
            TryAddCommand(DriverCommand.MouseMoveTo, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/moveto"));
            TryAddCommand(DriverCommand.SendKeysToActiveElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/keys"));

            // Touch screen interactions commands
            TryAddCommand(DriverCommand.TouchSingleTap, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/touch/click"));
            TryAddCommand(DriverCommand.TouchPress, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/touch/down"));
            TryAddCommand(DriverCommand.TouchRelease, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/touch/up"));
            TryAddCommand(DriverCommand.TouchMove, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/touch/move"));
            TryAddCommand(DriverCommand.TouchScroll, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/touch/scroll"));
            TryAddCommand(DriverCommand.TouchDoubleTap, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/touch/doubleclick"));
            TryAddCommand(DriverCommand.TouchLongPress, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/touch/longclick"));
            TryAddCommand(DriverCommand.TouchFlick, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/touch/flick"));

            TryAddCommand(DriverCommand.UploadFile, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/file"));

            TryAddCommand(DriverCommand.GetAvailableLogTypes, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/log/types"));
            TryAddCommand(DriverCommand.GetLog, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/log"));

            // HTML5 commands
            TryAddCommand(DriverCommand.GetLocation, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/location"));
            TryAddCommand(DriverCommand.SetLocation, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/location"));
            TryAddCommand(DriverCommand.GetAppCache, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/application_cache"));
            TryAddCommand(DriverCommand.GetAppCacheStatus, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/application_cache/status"));
            TryAddCommand(DriverCommand.ClearAppCache, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/application_cache/clear"));
            TryAddCommand(DriverCommand.GetLocalStorageKeys, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/local_storage"));
            TryAddCommand(DriverCommand.SetLocalStorageItem, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/local_storage"));
            TryAddCommand(DriverCommand.ClearLocalStorage, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/local_storage"));
            TryAddCommand(DriverCommand.GetLocalStorageItem, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/local_storage/key/{key}"));
            TryAddCommand(DriverCommand.RemoveLocalStorageItem, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/local_storage/key/{key}"));
            TryAddCommand(DriverCommand.GetLocalStorageSize, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/local_storage/size"));
            TryAddCommand(DriverCommand.GetSessionStorageKeys, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/session_storage"));
            TryAddCommand(DriverCommand.SetSessionStorageItem, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/session_storage"));
            TryAddCommand(DriverCommand.ClearSessionStorage, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/session_storage"));
            TryAddCommand(DriverCommand.GetSessionStorageItem, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/session_storage/key/{key}"));
            TryAddCommand(DriverCommand.RemoveSessionStorageItem, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/session_storage/key/{key}"));
            TryAddCommand(DriverCommand.GetSessionStorageSize, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/session_storage/size"));
        }
    }
}
