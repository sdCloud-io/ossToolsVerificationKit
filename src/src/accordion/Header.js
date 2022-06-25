import { Badge, Col, Container, Row } from "react-bootstrap";
import React from "react";

export function Header(props) {
    return (
        <div className="m-3">
            <Container fluid>
                <Row>
                    <Col className="text-center">Model name</Col>
                    {
                        props.instrumentsInformation.map(instrumentResult =>
                            <Col className="text-center">
                                { instrumentResult.InstrumentName } <Badge
                                bg="secondary"> { instrumentResult.InstrumentVersion } </Badge>
                            </Col>
                        )
                    }
                </Row>
            </Container>
        </div>
    )
}