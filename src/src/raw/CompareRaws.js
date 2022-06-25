import React from "react";
import { Badge, Col, Container, Row } from "react-bootstrap";


export function CompareRaws(props) {
    function mapValuesIntoCol(values) {
        return values.map(value => <Col className="text-center">{ value }</Col>)
    }

    return (
        <Container fluid>
            <hr/>
            { props.value.map(elem =>
                <Row>
                    <Col className="text-center">{ elem.ParameterName } { !elem.IsInConfidenceInterval &&
                    <Badge pill bg="warning" text="dark">Not in interval</Badge> }
                    </Col>
                    { mapValuesIntoCol(elem.Values) }
                </Row>)
            }
        </Container>
    )
}