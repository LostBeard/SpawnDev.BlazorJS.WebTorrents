(() => {
    var fetchStats = {};
    globalThis.fetchStats = fetchStats;
    var upgradeHttp = new URL(document.baseURI).protocol === 'https:';  // might also be able to use 'window.isSecureContext'
    var fetchOrig = fetch;
    var sleep = (ms) => new Promise(r => setTimeout(r, ms));
    var fails = {};
    var originFails = {};
    var originPings = {};
    fetchStats.originPings = originPings;
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
    fetchStats.isBlockedHost = function (hostname) {
        if (hostname.indexOf('://') !== -1) hostname = new URL(hostname).hostname;
        return blockedHosts.indexOf(hostname) !== -1;
    };
    globalThis.fetch = async (resource, options) => {
        var orig = resource;
        if (typeof resource === 'string') {
            var url = new URL(resource, document.baseURI);
            resource = url.toString();
            // resource is a string
            if (upgradeHttp && resource.indexOf('http://') === 0) {
                resource = `https://${resource.substring('http://'.length)}`;
            }
            var origin = new URL(resource).origin;
            var hostname = url.hostname;
            var isBlocked = fetchStats.isBlockedHost(hostname);
            if (isBlocked) {
                //console.log('Blocked host', hostname);
                await sleep(5);
                throw new DOMException('Failed it');
            }
            var now = new Date().getTime();
            var originFail = originFails[origin];
            var fail = fails[resource];
            var originPing = originPings[origin];
            if (originPing == null) {
                originPing = {
                    origin: origin,
                    bestPing: -1,
                    failCount: 0,
                    successCount: 0,
                    failTotal: 0,
                    successTotal: 0,
                    pinged: null,
                    lastSuccess: null,
                    lastFail: null,
                    ping: -1,
                };
                originPings[origin] = originPing;
            }
            if (originFail) {
                var elapsedSinceLastFail = now - originFail.lastFail;
                if (elapsedSinceLastFail < 5000 && fail.count > 1) {
                    await sleep(5);
                    throw new DOMException('Failed it');
                }
            }
            if (fail) {
                var elapsedSinceLastFail = now - fail.lastFail;
                var sleep = 0;
                var retryDelay = Math.min(fail.count + 2, 30) * 1000;
                var left = retryDelay - elapsedSinceLastFail;
                if (elapsedSinceLastFail < 5000 && fail.count > 1) {
                    await sleep(5);
                    throw new DOMException('Failed it');
                }
            }
            try {
                now = new Date().getTime();
                var nowBefore = now;
                originPing.pinged = now;
                var ret = await fetchOrig(resource, options);
                now = new Date().getTime();
                originPing.pinged = now;
                originPing.lastSuccess = now;
                originPing.ping = now - nowBefore;
                originPing.failCount = 0;
                originPing.successCount++;
                originPing.successTotal++;
                if (originPing.bestPing < 0 || originPing.ping < originPing.bestPing) {
                    originPing.bestPing = originPing.ping;
                }
                if (fail) {
                    delete fails[resource];
                }
                if (originFail) {
                    delete originFails[origin];
                }
                return ret;
            } catch (e) {
                now = new Date().getTime();
                originPing.pinged = now;
                originPing.lastFail = now;
                originPing.ping = -1;
                originPing.failCount++;
                originPing.failTotal++;
                originPing.successCount = 0;
                if (!fail) {
                    fail = {
                        origin: origin,
                        url: resource,
                        count: 0,
                        passStart: 0,
                        lastFail: 0,
                    };
                    fails[resource] = fail;
                }
                fail.lastFail = now;
                fail.count++;
                if (!originFail) {
                    originFail = {
                        origin: origin,
                        count: 0,
                        passStart: 0,
                        lastFail: 0,
                    };
                    originFail[origin] = originFail;
                }
                originFail.lastFail = now;
                originFail.count++;
                throw e;
            }
        } else {
            // resource is a Request object
            // currently not modified. could cause issues if a relative path was used to create the Request object.
            try {
                return await fetchOrig(resource, options);
            } catch (e) {
                await sleep(1000);
                throw e;
            }
        }
    };
})();