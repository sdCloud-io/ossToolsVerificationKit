import { Col, Container, Row } from "react-bootstrap";
import React from "react";
import { LogPanel } from "./LogPanel";

export function LogRaw(props) {
    const colSpan = props.values.length > 1 ? 4 : 6;

    return (
        <Container fluid className="text-center">
            <Row>
                <Col className="text-center">Log</Col>
                {
                    props.values.map(instrumentResult =>
                        instrumentResult.Log.length > 0 ?
                            <Col lg={ colSpan } xxl={ colSpan } xs={ colSpan } xl={ colSpan } md={ colSpan }
                                 sm={ colSpan }><LogPanel log={ instrumentResult.Log }/></Col>:
                            <Col className="text-center">no logs</Col>)
                }
            </Row>
        </Container>
    )
}