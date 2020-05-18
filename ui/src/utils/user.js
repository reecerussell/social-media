const AccessTokenKey = "sm_ac";
const ExpiryKey = "sm_ac_ed";

const GetAccessToken = () => localStorage.getItem(AccessTokenKey);

const Login = (token, expires) => {
    expires *= 1000;
    const expiryDate = new Date(new Date(expires).toUTCString()).getTime();

    localStorage.setItem(AccessTokenKey, token);
    localStorage.setItem(ExpiryKey, expiryDate);

    triggerListeners();
};

const Logout = () => {
    localStorage.removeItem(AccessTokenKey);
    localStorage.removeItem(ExpiryKey);

    triggerListeners();
};

const IsAuthenticated = () => {
    const token = GetAccessToken();
    if (!token) {
        return false;
    }

    const time = localStorage.getItem(ExpiryKey);
    const expiryDate = new Date(parseInt(time));
    if (expiryDate < new Date()) {
        return false;
    }

    return true;
};

const listeners = new Map();

const Listen = (name, callback) => listeners.set(name, callback);
const Unlisten = (name) => listeners.delete(name);

const triggerListeners = () => listeners.forEach((c) => c());

const GetId = () => getCurrentPayload()["user_id"];

const getCurrentPayload = () => {
    const token = GetAccessToken();
    if (!token) {
        return null;
    }

    const parts = token.split(".");
    if (parts.length < 2) {
        return null;
    }

    const payloadData = atob(parts[1]);

    return JSON.parse(payloadData);
};

export {
    GetAccessToken,
    Login,
    Logout,
    IsAuthenticated,
    Listen,
    Unlisten,
    GetId,
};
