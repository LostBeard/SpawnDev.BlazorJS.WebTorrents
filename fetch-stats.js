/** 
 * FetchStats overrides the global fetch method to enable http to https upgrades, stats logging, rate limiting, and request blocking
 * WebTorrent can cause millions of error messages in minutes in the devtools if some torrents use http:// webSeed addresses and
 * the WebTorrent client app was loaded over https://
 * Error:
 * Mixed Content: The page at '<URL>' as loaded over HTTPS, but requested an insecure resource '<URL>'. This request has been blocked; the content must be served over HTTPS.
 * - upgrading fetch requests from http:// to https:// when on a secure context is (likely) the best/easiest fix
 * - This not a problem with WebTorrent but an incompatiblity between the the torrents and a browser environment. 
 * - Upgrading to https:// when on a secure context may not fix the issue if the server does not actually provide the same resoruce over https://
 * **/

(() => {
    var fetchStats = {};
    globalThis.fetchStats = fetchStats;
    var upgradeInsecureRequests = true;
    var rateLimit = true;
    var rateLimitDelayBase = 10000;
    var isSecureContext = new URL(document.baseURI).protocol === 'https:';  // might also be able to use 'window.isSecureContext'
    var fetchOrig = fetch;
    var sleep = (ms) => new Promise(r => setTimeout(r, ms));
    var hostFails = {};
    var hostPings = {};
    fetchStats.hostPings = hostPings;
    var blockedHosts = [];
    fetchStats.blockHost = function (hostname) {
        if (hostname.indexOf('://') !== -1) hostname = new URL(hostname).hostname;
        var i = blockedHosts.indexOf(hostname);
        if (i !== -1) return false;
        blockedHosts.push(hostname);
        return true;
    };
    fetchStats.unblockHost = function (hostname) {
        if (hostname.indexOf('://') !== -1) hostname = new URL(hostname).hostname;
        var i = blockedHosts.indexOf(hostname);
        if (i === -1) return false;
        blockedHosts.splice(i, 1);
        return true;
    };
    fetchStats.unblockAll = function () {
        var toRemove = [...blockedHosts];
        for (var k of toRemove) {
            fetchStats.unblockHost(k);
        }
        // blockedHosts = [];
        return true;
    };
    fetchStats.isHostBlocked = function (hostname) {
        if (hostname.indexOf('://') !== -1) hostname = new URL(hostname).hostname;
        return blockedHosts.indexOf(hostname) !== -1;
    };
    globalThis.fetch = async (resource, options) => {
        if (typeof resource === 'string') {
            var url = new URL(resource, document.baseURI);
            resource = url.toString();
            var insecureRequst = resource.indexOf('http://') === 0;
            if (isSecureContext && insecureRequst && upgradeInsecureRequests) {
                resource = `https://${resource.substring('http://'.length)}`;
            }
            var hostname = url.hostname;
            var now = new Date().getTime();
            var hostFail = hostFails[hostname];
            var hostPing = hostPings[hostname];
            if (hostPing == null) {
                hostPing = {
                    hostName: hostname,
                    bestPing: -1,
                    failCount: 0,
                    successCount: 0,
                    failTotal: 0,
                    successTotal: 0,
                    pinged: null,
                    lastSuccess: null,
                    lastFail: null,
                    ping: -1,
                    blockCount: 0,
                };
                hostPings[hostname] = hostPing;
            }
            var isBlocked = fetchStats.isHostBlocked(hostname);
            if (isBlocked) {
                //console.log('Blocked host', hostname);
                hostPing.blockCount++;
                await sleep(5);
                throw new DOMException('Blocked it');
            }
            if (hostFail && rateLimit) {
                var elapsedSinceLastFail = now - hostFail.lastFail;
                if (elapsedSinceLastFail < rateLimitDelayBase && hostFail.count > 2) {
                    await sleep(5);
                    throw new DOMException('Rate limited');
                }
            }
            try {
                now = new Date().getTime();
                var nowBefore = now;
                hostPing.pinged = now;
                var ret = await fetchOrig(resource, options);
                now = new Date().getTime();
                hostPing.pinged = now;
                hostPing.lastSuccess = now;
                hostPing.ping = now - nowBefore;
                hostPing.failCount = 0;
                hostPing.successCount++;
                hostPing.successTotal++;
                if (hostPing.bestPing < 0 || hostPing.ping < hostPing.bestPing) {
                    hostPing.bestPing = hostPing.ping;
                }
                if (hostFail) {
                    delete hostFails[hostname];
                }
                return ret;
            } catch (e) {
                now = new Date().getTime();
                hostPing.pinged = now;
                hostPing.lastFail = now;
                hostPing.ping = -1;
                hostPing.failCount++;
                hostPing.failTotal++;
                hostPing.successCount = 0;
                if (!hostFail) {
                    hostFail = {
                        hostName: hostname,
                        count: 0,
                        lastFail: 0,
                    };
                    hostFails[hostname] = hostFail;
                }
                hostFail.lastFail = now;
                hostFail.count++;
                throw e;
            }
        } else {
            // resource is a Request object
            try {
                return await fetchOrig(resource, options);
            } catch (e) {
                await sleep(1000);
                throw e;
            }
        }
    };
    class WebSocketExtEvent extends Event {
        constructor(type, extSocket) {
            super(type);
            this.socket = extSocket;
        }
    }
    class WebSocketExtNewEvent extends WebSocketExtEvent {
        constructor(extSocket) {
            super('newsocket', extSocket);
        }
    }
    class WebSocketExtSendEvent extends WebSocketExtEvent {
        constructor(extSocket, data) {
            super('send', extSocket);
            this.data = data;
            this.cancelSend = false;
        }
    }
    var webSocketExtId = 0;
    class WebSocketExt extends WebSocket {
        constructor(wsUrl, protocols) {
            var url = new URL(wsUrl);
            var hostname = url.hostname;
            var isBlocked = fetchStats.isHostBlocked(hostname);
            if (isBlocked) {
                wsUrl = WebSocketExt.blockedHostTarget;
            }
            super(wsUrl, protocols);
            this.extId = webSocketExtId++;
            globalThis.dispatchEvent(new WebSocketExtNewEvent(this));
        }
        send(data) {
            var e = new WebSocketExtSendEvent(this, data);
            this.dispatchEvent(e);
            if (e.cancelSend) return;
            super.send(e.data);
        }
        static blockedHostTarget = 'wss://localhost:65535/host_blocked';
    }
    globalThis.WebSocket = WebSocketExt;
    // change the class name to match WebSocket
    Object.defineProperty(globalThis.WebSocket, 'name', { value: 'WebSocket' })
})();
