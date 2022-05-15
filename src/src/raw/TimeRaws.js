import React from "react";
import { Col, Container, Row } from "react-bootstrap";

export function TimeRaws(props) {

    function getGenerationTimeFromItem() {
        return props.value.map(instrumentResult =>
            instrumentResult.CodeGenerationTime)
    }

    function getExecutionTimeFromItem() {
        return props.value.map(instrumentResult =>
            instrumentResult.CodeExecutionTime)
    }

    function getCompilationTimeFromItem() {
        return props.value.map(instrumentResult =>
            instrumentResult.CodeExecutionTime)
    }


    return (
        <Container fluid className="text-center">
            <Row>
                <Col>Generation Time</Col>
                { getGenerationTimeFromItem().map(value => <Col>{ value } </Col>) }
            </Row>

            <Row>
                <Col>Execution Time</Col>
                { getExecutionTimeFromItem().map(value => <Col>{ value } </Col>) }
            </Row>

            <Row>
                <Col>Compilation Time</Col>
                { getCompilationTimeFromItem().map(value => <Col>{ value } </Col>) }
            </Row>
        </Container>
    )
}