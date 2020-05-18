import React, { useState } from "react";
import {
    Container,
    Row,
    Col,
    Form,
    FormGroup,
    Button,
    Alert,
} from "reactstrap";
import { Input } from "../../components/form";

const Account = () => {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [snapchat, setSnapchat] = useState("");
    const [instagram, setInstagram] = useState("");
    const [error, setError] = useState(null);

    const handleUpdateText = (e) => {
        const { name, value } = e.target;

        switch (name) {
            case "username":
                setUsername(value);
                break;
            case "snapchat":
                setSnapchat(value);
                break;
            case "instagram":
                setInstagram(value);
                break;
            case "email":
                setEmail(value);
                break;
        }
    };

    useState(() => {
        if (error !== null) {
            setTimeout(() => setError(null), 4000);
        }
    }, [error]);

    return (
        <Container className="sections">
            <Row>
                <Col md="6">
                    <div className="section p-4">
                        <h1 className="display-4">Account</h1>
                        <p className="text-muted small info">
                            Use this page to edit your account information.
                        </p>
                        <hr />
                        <Alert
                            color="danger"
                            isOpen={error !== null}
                            toggle={() => setError(null)}
                            fade
                        >
                            {error}
                        </Alert>
                        <Form
                            autoCapitalize="off"
                            autoComplete="off"
                            autoCorrect="off"
                        >
                            <Input
                                type="text"
                                name="username"
                                value={username}
                                label="Username"
                                onChange={handleUpdateText}
                            />
                            <Input
                                type="text"
                                name="email"
                                value={email}
                                label="Email"
                                onChange={handleUpdateText}
                            />
                        </Form>
                    </div>
                </Col>
                <Col md="6" className="mt-4 mt-md-0">
                    <div className="section p-4">
                        <h2 className="display-4 text-left">Socials</h2>
                        <p className="text-muted small">
                            Here is your social media handles.
                        </p>
                        <Form
                            autoComplete="off"
                            autoCapitalize="off"
                            autoCorrect="off"
                        >
                            <Input
                                type="text"
                                name="snapchat"
                                label="Snapchat"
                                value={snapchat}
                                onChange={handleUpdateText}
                            />
                            <Input
                                type="text"
                                name="instagram"
                                label="Instagram"
                                value={instagram}
                                onChange={handleUpdateText}
                            />
                            <FormGroup className="float-right">
                                <Button type="submit" color="primary">
                                    SAVE
                                </Button>
                            </FormGroup>
                        </Form>
                    </div>
                </Col>
            </Row>
        </Container>
    );
};

export default Account;
