(function () {

    window.initRefreshTokenService = (dotNetCallBack, refreshUrl, storageMode, secondaryKeys) => {
        window.refreshTokenServiceConfig = {
            dotNetCallBack: dotNetCallBack,
            refreshUrl: refreshUrl,
            storageMode: storageMode,
            secondaryKeys: secondaryKeys,
            storageKey: '__parsedToken',
            timerId: -1
        };
        console.log(`Refresh token service, refreshUrl: ${refreshUrl}, storageMode: ${storageMode}`);
        console.log('Refresh token service, initialized');
    }

    window.startRefreshTokenService = (dueTimeInMilliseconds) => {
        //console.log(`Refresh token service, begin waiting for ${dueTimeInMilliseconds} ms`);
        window.stopRefreshTokenService();

        let refreshTokenAsync = async () => {
            //console.log(`Refresh token service, begin fetch refresh token...`);

            let parsedToken = { accessToken: '', refreshTokenString: '', secondaryKeys: [] };
            let parsedTokenStr = "";
            if (window.refreshTokenServiceConfig.storageMode === 0) {
                parsedTokenStr = localStorage.getItem(window.refreshTokenServiceConfig.storageKey);
            } else {
                parsedTokenStr = sessionStorage.getItem(window.refreshTokenServiceConfig.storageKey);
            }
            //console.log(`Refresh token service, storage data: ${parsedTokenStr}`);
            parsedToken = JSON.parse(parsedTokenStr);

            const data = {
                refreshTokenString: parsedToken.refreshToken,
                secondaryKeys: window.refreshTokenServiceConfig.secondaryKeys
            }

            //console.log(`Refresh token service, before fetch, accessToken: ${parsedToken.accessToken}, data: ${data}`);

            const response = await fetch(window.refreshTokenServiceConfig.refreshUrl, {
                method: 'POST',
                mode: 'cors',
                cache: 'no-cache',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${parsedToken.accessToken}`
                },
                redirect: 'follow',
                referrerPolicy: 'no-referrer',
                body: JSON.stringify(data)
            });

            //console.log(`Refresh token service, after fetch.`);
            
            return response;
        };

        window.refreshTokenServiceConfig.timerId = setTimeout(() => {
            window.refreshTokenServiceConfig.timerId = -1;
            refreshTokenAsync()
                .then(async (response) => {

                    if (response.status === 401) {
                        // not authorized
                        const parsedTokenDataStr = JSON.stringify({ accessToken: '', refreshToken: '' });
                        if (window.refreshTokenServiceConfig.storageMode === 0) {
                            localStorage.setItem(window.refreshTokenServiceConfig.storageKey, parsedTokenDataStr);
                        } else {
                            sessionStorage.setItem(window.refreshTokenServiceConfig.storageKey, parsedTokenDataStr);
                        }
                    } else if (response.status === 200) {
                        const authenticationResponse = await response.json();
                        const authResponseStr = JSON.stringify(authenticationResponse);

                        //console.log(`Refresh token service, authenticationResponse: ${authResponseStr}`);

                        window.refreshTokenServiceConfig.dotNetCallBack.invokeMethodAsync('CallbackReceiveAuthenticationResponse', authResponseStr)
                            .then((receiveAuthenticationResultStr) => {

                                //console.log(`Refresh token service, receiveAuthenticationResultStr: ${receiveAuthenticationResultStr}`);

                                const receiveAuthenticationResult = JSON.parse(receiveAuthenticationResultStr);
                                const parsedTokenDataStr = JSON.stringify(receiveAuthenticationResult.parsedTokenData);
                                if (window.refreshTokenServiceConfig.storageMode === 0) {
                                    localStorage.setItem(window.refreshTokenServiceConfig.storageKey, parsedTokenDataStr);
                                } else {
                                    sessionStorage.setItem(window.refreshTokenServiceConfig.storageKey, parsedTokenDataStr);
                                }

                                if (receiveAuthenticationResult.startService) {
                                    window.startRefreshTokenService(receiveAuthenticationResult.serviceDueTime);
                                }

                            });
                    } else {
                        // server error, try again a bit later
                        window.startRefreshTokenService(1000);
                    }

                });
        }, dueTimeInMilliseconds);
    }

    window.stopRefreshTokenService = () => {
        //console.log(`stopRefreshTokenService, window.refreshTokenServiceConfig.timerId: ${window.refreshTokenServiceConfig.timerId}`);
        if (window.refreshTokenServiceConfig.timerId !== -1) {
            clearTimeout(window.refreshTokenServiceConfig.timerId);
        }
    }

})();

console.log("Refresh token service script inserted");
