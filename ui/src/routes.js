import React from "react";

const Login = React.lazy(() => import("./views/users/login"));
const Register = React.lazy(() => import("./views/users/register"));

const Account = React.lazy(() => import("./views/account"));

export default [
    {
        path: "/register",
        name: "Register",
        exact: true,
        component: Register,
    },
    {
        path: "/login",
        name: "Login",
        exact: true,
        component: Login,
    },
    {
        path: "/account",
        name: "Account",
        exact: true,
        component: Account,
    },
];
