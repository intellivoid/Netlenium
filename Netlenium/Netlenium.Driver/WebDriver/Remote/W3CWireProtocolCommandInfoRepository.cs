namespace Netlenium.Driver.WebDriver.Remote
{
    /// <summary>
    /// Holds the information about all commands specified by the JSON wire protocol.
    /// This class cannot be inherited, as it is intended to be a singleton, and
    /// allowing subclasses introduces the possibility of multiple instances.
    /// </summary>
    public sealed class W3CWireProtocolCommandInfoRepository : CommandInfoRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="W3CWireProtocolCommandInfoRepository"/> class.
        /// </summary>
        public W3CWireProtocolCommandInfoRepository()
            : base()
        {
            InitializeCommandDictionary();
        }

        /// <summary>
        /// Gets the level of the W3C WebDriver specification that this repository supports.
        /// </summary>
        public override int SpecificationLevel
        {
            get { return 1; }
        }

        /// <summary>
        /// Initializes the dictionary of commands for the CommandInfoRepository
        /// </summary>
        protected override void InitializeCommandDictionary()
        {
            TryAddCommand(DriverCommand.Status, new CommandInfo(CommandInfo.GetCommand, "/status"));
            TryAddCommand(DriverCommand.NewSession, new CommandInfo(CommandInfo.PostCommand, "/session"));
            TryAddCommand(DriverCommand.Quit, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}"));
            TryAddCommand(DriverCommand.GetTimeouts, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/timeouts"));
            TryAddCommand(DriverCommand.SetTimeouts, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/timeouts"));
            TryAddCommand(DriverCommand.Get, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/url"));
            TryAddCommand(DriverCommand.GetCurrentUrl, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/url"));
            TryAddCommand(DriverCommand.GoBack, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/back"));
            TryAddCommand(DriverCommand.GoForward, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/forward"));
            TryAddCommand(DriverCommand.Refresh, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/refresh"));
            TryAddCommand(DriverCommand.GetTitle, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/title"));
            TryAddCommand(DriverCommand.GetCurrentWindowHandle, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window"));
            TryAddCommand(DriverCommand.Close, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/window"));
            TryAddCommand(DriverCommand.SwitchToWindow, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window"));
            TryAddCommand(DriverCommand.GetWindowHandles, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window/handles"));
            TryAddCommand(DriverCommand.SwitchToFrame, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/frame"));
            TryAddCommand(DriverCommand.SwitchToParentFrame, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/frame/parent"));

            // TODO: Remove window size/position end points when spec-compliant remote ends have
            // migrated to the window rect commands.
            TryAddCommand(DriverCommand.GetWindowSize, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window/size"));
            TryAddCommand(DriverCommand.SetWindowSize, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/size"));
            TryAddCommand(DriverCommand.GetWindowPosition, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window/position"));
            TryAddCommand(DriverCommand.SetWindowPosition, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/position"));
            TryAddCommand(DriverCommand.GetWindowRect, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/window/rect"));
            TryAddCommand(DriverCommand.SetWindowRect, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/rect"));

            TryAddCommand(DriverCommand.MaximizeWindow, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/maximize"));
            TryAddCommand(DriverCommand.MinimizeWindow, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/minimize"));
            TryAddCommand(DriverCommand.FullScreenWindow, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/window/fullscreen"));
            TryAddCommand(DriverCommand.GetActiveElement, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/active"));
            TryAddCommand(DriverCommand.FindElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element"));
            TryAddCommand(DriverCommand.FindElements, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/elements"));
            TryAddCommand(DriverCommand.FindChildElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/element"));
            TryAddCommand(DriverCommand.FindChildElements, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/elements"));
            TryAddCommand(DriverCommand.IsElementSelected, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/selected"));
            TryAddCommand(DriverCommand.ClickElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/click"));
            TryAddCommand(DriverCommand.ClearElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/clear"));
            TryAddCommand(DriverCommand.SendKeysToElement, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/element/{id}/value"));
            TryAddCommand(DriverCommand.GetElementAttribute, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/attribute/{name}"));
            TryAddCommand(DriverCommand.GetElementProperty, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/property/{name}"));
            TryAddCommand(DriverCommand.GetElementValueOfCssProperty, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/css/{name}"));
            TryAddCommand(DriverCommand.GetElementText, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/text"));
            TryAddCommand(DriverCommand.GetElementTagName, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/name"));
            TryAddCommand(DriverCommand.GetElementRect, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/rect"));
            TryAddCommand(DriverCommand.IsElementEnabled, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/enabled"));
            TryAddCommand(DriverCommand.GetPageSource, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/source"));
            TryAddCommand(DriverCommand.ExecuteScript, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/execute/sync"));
            TryAddCommand(DriverCommand.ExecuteAsyncScript, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/execute/async"));
            TryAddCommand(DriverCommand.GetAllCookies, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/cookie"));
            TryAddCommand(DriverCommand.GetCookie, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/cookie/{name}"));
            TryAddCommand(DriverCommand.AddCookie, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/cookie"));
            TryAddCommand(DriverCommand.DeleteCookie, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/cookie/{name}"));
            TryAddCommand(DriverCommand.DeleteAllCookies, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/cookie"));
            TryAddCommand(DriverCommand.Actions, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/actions"));
            TryAddCommand(DriverCommand.CancelActions, new CommandInfo(CommandInfo.DeleteCommand, "/session/{sessionId}/actions"));
            TryAddCommand(DriverCommand.DismissAlert, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/alert/dismiss"));
            TryAddCommand(DriverCommand.AcceptAlert, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/alert/accept"));
            TryAddCommand(DriverCommand.GetAlertText, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/alert/text"));
            TryAddCommand(DriverCommand.SetAlertValue, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/alert/text"));
            TryAddCommand(DriverCommand.Screenshot, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/screenshot"));
            TryAddCommand(DriverCommand.ElementScreenshot, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/screenshot"));

            // Commands below here are not included in the W3C specification,
            // but are required for full fidelity of execution with Selenium's
            // local-end implementation of WebDriver.
            TryAddCommand(DriverCommand.GetSessionCapabilities, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}"));
            TryAddCommand(DriverCommand.IsElementDisplayed, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/displayed"));
            TryAddCommand(DriverCommand.ElementEquals, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/equals/{other}"));
            TryAddCommand(DriverCommand.DefineDriverMapping, new CommandInfo(CommandInfo.PostCommand, "/config/drivers"));
            TryAddCommand(DriverCommand.UploadFile, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/file"));
            TryAddCommand(DriverCommand.SetAlertCredentials, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/alert/credentials"));
            TryAddCommand(DriverCommand.GetSessionList, new CommandInfo(CommandInfo.GetCommand, "/sessions"));
            TryAddCommand(DriverCommand.GetElementLocationOnceScrolledIntoView, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}/location_in_view"));
            TryAddCommand(DriverCommand.DescribeElement, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/element/{id}"));
            TryAddCommand(DriverCommand.GetOrientation, new CommandInfo(CommandInfo.GetCommand, "/session/{sessionId}/orientation"));
            TryAddCommand(DriverCommand.SetOrientation, new CommandInfo(CommandInfo.PostCommand, "/session/{sessionId}/orientation"));

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
