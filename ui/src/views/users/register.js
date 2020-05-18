import React from "react";
import { Container, Row, Col } from "reactstrap";
import { Link } from "react-router-dom";
import { Register as RegisterForm } from "../../components/users";

const Register = () => (
    <Container className="sections">
        <Row>
            <Col md="8" lg="6" xl="5">
                <div className="section p-4">
                    <h1 className="display-4">Social Media</h1>
                    <p className="text-muted small pt-1 info">
                        The brand new, inivitive social media platform, leading
                        the industry in how a sociel media platform should be!
                    </p>
                    <hr />
                    <h2 className="display-3">Fancy joining?</h2>
                    <p className="text-center">
                        Just enter a few details below to get started!
                    </p>
                    <RegisterForm />
                    <p className="text-center">
                        <Link to="/login">Already have an account?</Link>
                    </p>
                </div>
            </Col>
        </Row>
    </Container>
);

export default Register;
