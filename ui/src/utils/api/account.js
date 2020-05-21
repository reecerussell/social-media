import Fetch from "./fetch";

const Register = async (data, onSuccess, onError) =>
    await Fetch(
        "register",
        { method: "POST", body: JSON.stringify(data) },
        onSuccess,
        onError
    );

const Login = async (creds, onSuccess, onError) =>
    await Fetch(
        "login",
        {
            method: "POST",
            body: JSON.stringify(creds),
        },
        onSuccess,
        onError
    );

export { Register, Login };
