import { GetAccessToken, Logout } from "../user";

const Send = async (url, options) => {
    const ac = GetAccessToken();

    if (!options) {
        options = {
            headers: {
                "Content-Type": "application/json",
            },
        };
    } else if (!options.headers) {
        options.headers = {
            "Content-Type": "application/json",
        };
    }

    if (ac) {
        options.headers["Authorization"] = "Bearer " + ac;
    }

    return await fetch(url, options);
};

const BaseUrl = document.getElementById("base-api-url").value;

const Fetch = async (url, options, onSuccess, onFail) => {
    try {
        const res = await Send(BaseUrl + url, options);

        switch (res.status) {
            case 200:
            case 201:
                if (onSuccess) {
                    await onSuccess(res);
                }
                break;
            case 401:
                Logout();
                break;
            default:
                try {
                    const { message } = await res.json();
                    onFail(message);
                } catch {
                    onFail("An error occured while reading the response.");
                }
                break;
        }
    } catch {
        onFail("An error occured while communicating with the server.");
    }
};

export default Fetch;
export { Fetch, BaseUrl };
