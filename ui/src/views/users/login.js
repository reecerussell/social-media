import React from "react";
import { Container, Row, Col, Form, FormGroup, Button } from "reactstrap";
import { Input } from "../../components/form";
import { Link } from "react-router-dom";

const Login = () => (
    <Container className="sections">
        <Row>
            <Col md="6">
                <div className="section p-4">
                    <h1 className="display-4">Pow</h1>
                    <p className="text-muted small">
                        Want to test your friends knowlegde by creating your own
                        quizes and questions? Now you can, with POW!
                    </p>
                    <hr />
                    <h2 className="display-3">Login</h2>
                    <p>Enter your account details to login!</p>
                    <Form autoComplete="off">
                        <FormGroup>
                            <Input
                                type="text"
                                placeholder="Enter your username..."
                                label="Username"
                                autoCapitalize="off"
                                autoComplete="off"
                            />
                        </FormGroup>
                        <FormGroup>
                            <Input
                                type="password"
                                placeholder="Enter your password..."
                                label="Password"
                                autoCapitalize="off"
                                autoComplete="off"
                            />
                        </FormGroup>
                        <FormGroup>
                            <Button color="primary" block type="submit">
                                Login
                            </Button>
                        </FormGroup>
                    </Form>
                    <p>
                        <Link to="/register">Don't have an account?</Link>
                    </p>
                </div>
            </Col>
        </Row>
    </Container>
);

export default Login;
