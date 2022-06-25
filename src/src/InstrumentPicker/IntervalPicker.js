import Form from "react-bootstrap/Form";
import { Col, Row } from "react-bootstrap";
import RangeSlider from "react-bootstrap-range-slider";
import React from "react";

export function IntervalPicker(props) {
    return (
        <Row className="m-3">
            <Form>
                <Form.Group as={ Row }>
                    <Col xs="3">Confidence interval</Col>
                    <Col xs="6">
                        <RangeSlider
                            value={ props.interval }
                            onChange={ e => props.setInterval(e.target.value) }
                        />
                    </Col>
                    <Col xs="3">
                        <Form.Control value={ props.interval }
                                      onChange={ e => props.setInterval(e.target.value) }
                                      type="number"/>
                    </Col>
                </Form.Group>
            </Form>
        </Row>)
}