import React from "react";
import { Col, Container, Row } from "react-bootstrap";


export function CompareRaws(props) {
    function compareParams(value1, value2) {
        const float1 = parseFloat(value1)
        const float2 = parseFloat(value2)

        if (float1 === float2) return true
        if (float1 < float2)
            return (((float2 - float1) / float1) * 100) <= props.confidenceInterval;
        else
            return (((float1 - float2) / float1) * 100) <= props.confidenceInterval;
    }

    function compareInstruments() {
        const paramsFirstInstrument = Object.getOwnPropertyNames(props.value[0].ResultDictionary)
        const paramsSecondInstrument = Object.getOwnPropertyNames(props.value[1].ResultDictionary)
        const commonParams = paramsFirstInstrument.filter(elem => paramsSecondInstrument.indexOf(elem) !== -1)
        return commonParams.map(elem => [elem, compareParams(props.value[0].ResultDictionary[elem], props.value[1].ResultDictionary[elem]), props.value[0].ResultDictionary[elem], props.value[1].ResultDictionary[elem]])
    }

    function isInConfidenceRender(value) {
        if (value) {
            return <Col className="text-center">In the confidence interval</Col>
        } else {
            return <Col className="text-center">Not in the confidence interval</Col>
        }
    }

    if (props.value[0].ResultDictionary === null || props.value[1].ResultDictionary === null) return <div/>
    const properties = compareInstruments()
    return (
        <Container fluid>
            <hr/>
            { properties.map(elem =>
                <Row>
                    <Col className="text-center">{ elem[0] }</Col>
                    <Col className="text-center">{ elem[2] }</Col>
                    <Col className="text-center">{ elem[3] }</Col>
                    { isInConfidenceRender(elem[1]) }
                </Row>) }
        </Container>
    )

}