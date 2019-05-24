var proxy_enabled = %PROXY_ENABLED%;

var proxy_config = {
    mode: "fixed_servers",
    rules: {
      singleProxy: {
        scheme: "%PROXY_SCHEME%",
        host: "%PROXY_HOST%",
        port: parseInt(%PROXY_PORT%)
      }
    }
};

function callbackFn(details) {
    return {
        authCredentials: {
            username: "%PROXY_USERNAME%",
            password: "%PROXY_PASSWORD%"
        }
    };
}

if (proxy_enabled === true) {
    chrome.proxy.settings.set({ value: proxy_config, scope: "regular" }, function () { });
    chrome.webRequest.onAuthRequired.addListener(
        callbackFn,
        { urls: ["<all_urls>"] },
        ['blocking']
    );
}