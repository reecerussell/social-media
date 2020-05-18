import React, { useState, useEffect } from "react";
import * as Api from "../../utils/api";
import { Redirect } from "react-router-dom";
import { Form, FormGroup, Button, Spinner } from "reactstrap";
import { Input } from "../form";
import { Login as SetLogin } from "../../utils/user";

const Login = () => {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [redirect, setRedirect] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (error !== null) {
            setTimeout(() => setError(null), 8000);
        }
    }, [error]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        switch (name) {
            case "username":
                setUsername(value);
                break;
            case "password":
                setPassword(value);
                break;
        }
    };

    const validateInput = () => {
        if (username === "") {
            return setError("Please enter your username!");
        }

        if (password === "") {
            return setError("Please enter your password!");
        }

        return true;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (loading || !validateInput()) {
            return;
        }

        setLoading(true);

        await Api.Account.Login(
            {
                username,
                password,
            },
            async (res) => {
                const { token, expires } = await res.json();
                SetLogin(token, expires);
                const params = new URLSearchParams(window.location.search);
                const returnUrl = params.get("returnUrl");
                if (returnUrl) {
                    setRedirect(returnUrl);
                } else {
                    setRedirect("/account");
                }
            },
            setError
        );

        setLoading(false);
    };

    if (!loading && redirect) {
        return <Redirect to={redirect} />;
    }

    const errorAlert = error !== null ? <p className="error">{error}</p> : null;

    return (
        <Form autoComplete="off" onSubmit={handleSubmit}>
            {errorAlert}
            <Input
                type="text"
                name="username"
                value={username}
                onChange={handleChange}
                placeholder="Enter your username"
                label="Username"
            />
            <Input
                type="password"
                name="password"
                value={password}
                onChange={handleChange}
                placeholder="Enter your password"
                label="Password"
            />
            <FormGroup>
                <Button
                    color="primary"
                    type="submit"
                    block
                    className="float-right"
                >
                    {loading ? <Spinner size="sm" color="default" /> : "LOGIN"}
                </Button>
            </FormGroup>
        </Form>
    );
};

export default Register;
