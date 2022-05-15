import { Accordion, Col, Container, Row } from "react-bootstrap";
import React from "react";
import { ResultBadge } from "./ResultBadge";

export function PanelHeader(props) {
    return (
        <Accordion.Header>
            <Container fluid>
                <Row>
                    <Col className="text-center">{ props.value[0].ModelPath.replace(/^.*[\\\/]/, '') }</Col>
                    {
                        props.instrumentsNames.map( (instrumentResult, i) =>
                            <Col className="text-center">
                                { instrumentResult }
                                <ResultBadge result={ props.value[i].Result }/>
                            </Col>
                        )
                    }
                </Row>
            </Container>
        </Accordion.Header>
    )
}