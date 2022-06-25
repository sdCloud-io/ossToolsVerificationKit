import { Col, Row } from "react-bootstrap";
import React from "react";

export function Header() {
    return (
        <Row className="m-3 text-center">
            <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                Instrument Name
            </Col>
            <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                Model Types
            </Col>
            <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                Instrument Version
            </Col>

        </Row>)
}