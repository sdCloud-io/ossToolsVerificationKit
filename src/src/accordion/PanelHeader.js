import { Accordion, Badge, Col, Container, Row } from "react-bootstrap";
import React from "react";
import { ResultBadge } from "./ResultBadge";

export function PanelHeader(props) {

    return (
        <Accordion.Header>
            <Container fluid>
                <Row>
                    <Col className="text-center">{ props.modelName.replace(/^.*[\\\/]/, '') }</Col>
                    {
                        props.instrumentsInformation.map((instrumentResult, i) =>
                            <Col className="text-center">
                                {/*{ instrumentResult.InstrumentName }  <Badge bg="secondary"> { instrumentResult.InstrumentVersion } </Badge>*/}
                                <ResultBadge result={ props.instrumentsResults[i].Result }/>
                            </Col>
                        )
                    }
                </Row>
            </Container>
        </Accordion.Header>
    )
}