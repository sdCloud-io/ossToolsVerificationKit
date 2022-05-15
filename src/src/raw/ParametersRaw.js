import React from "react";
import { Col, Container, Row } from "react-bootstrap";


export function ParametersRaw(props) {
    const colSpan = props.isCompare ? 8 : 6;

    if (props.parameterDictionary === null) return <div/>
    return (
        <Container fluid>
            <hr/>
            { Object
                .getOwnPropertyNames(props.parameterDictionary)
                .map(x =>
                    <Row>
                        <Col className="text-center">{ x }</Col>
                        <Col lg={ colSpan } md={ colSpan } sm={ colSpan }
                             xl={ colSpan } xs={ colSpan }
                             xxl={ colSpan } className="text-center">{ props.parameterDictionary[x] }</Col>
                    </Row>) }
        </Container>
    )
}