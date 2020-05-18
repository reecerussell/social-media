import React, { Suspense } from "react";
import {
    BrowserRouter as Router,
    Route,
    Redirect,
    Switch,
} from "react-router-dom";
import "./scss/styles.scss";

import routes from "./routes";

const basePageTitle = "Social Media";

const renderView = (props, route) => {
    document.title = route.name + " | " + basePageTitle;

    return <route.component {...props} />;
};

const App = () => (
    <Router>
        <Suspense fallback={null}>
            <Switch>
                {routes.map((route, idx) => {
                    return route.component ? (
                        <Route
                            key={idx}
                            path={route.path}
                            exact={route.exact}
                            name={route.name}
                            render={(props) => renderView(props, route)}
                        />
                    ) : null;
                })}
                <Redirect from="/" to="/register" />
            </Switch>
        </Suspense>
    </Router>
);

export default App;
