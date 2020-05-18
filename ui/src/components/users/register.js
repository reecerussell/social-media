import React, { useState, useEffect } from "react";
import * as Api from "../../utils/api";
import { Redirect } from "react-router-dom";
import { Form, FormGroup, Button, Spinner } from "reactstrap";
import { Input } from "../form";

const Register = () => {
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
            return setError("Please enter a username.");
        }

        if (username.length < 3) {
            return setError(
                "Your username cannot be shorter than 3 characters!"
            );
        }

        if (username.length > 25) {
            return setError(
                "Your username cannot be greater than 25 characters!"
            );
        }

        if (password === "") {
            return setError("Please enter a password.");
        }

        if (password.length < 6) {
            return setError(
                "Your password must be at least 6 characters long!"
            );
        }

        if (password.length > 256) {
            return setError(
                "Your password cannt be greater than 256 characters!"
            );
        }

        return true;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (loading || !validateInput()) {
            return;
        }

        setLoading(true);

        await Api.Account.Register(
            {
                username,
                password,
            },
            async () => setRedirect("/login"),
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
                placeholder="Enter a username"
                label="Username"
            />
            <Input
                type="password"
                name="password"
                value={password}
                onChange={handleChange}
                placeholder="Enter a password"
                label="Password"
            />
            <FormGroup>
                <Button
                    color="primary"
                    type="submit"
                    block
                    className="float-right"
                >
                    {loading ? (
                        <Spinner size="sm" color="default" />
                    ) : (
                        "REGISTER"
                    )}
                </Button>
            </FormGroup>
        </Form>
    );
};

export default Register;
