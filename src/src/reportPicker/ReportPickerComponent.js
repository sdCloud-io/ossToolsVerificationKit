import React from "react";
import { Button, Col, Container, Row } from "react-bootstrap";
import { InstrumentPicker } from "./InstrumentPicker";
import Form from 'react-bootstrap/Form'
import RangeSlider from 'react-bootstrap-range-slider';

export function ReportPickerComponent(props) {
    function AddInstrument() {
        props.updateShowExtra(true)
    }

    function RemoveInstrument() {
        props.updateShowExtra(false)
    }

    return (
        <Container className="text-center mt-5" fluid>
            <div className="panel">
                <InstrumentPicker state={ props.instrument } updateState={ props.setInstrument } reports={ props.reports }
                                  buttonEvent={ AddInstrument }
                                  showButton={ !props.showExtraSelect } buttonText="Add instrument"/>
                {
                    props.showExtraSelect &&
                    <div>
                        <InstrumentPicker state={ props.extraInstrument } updateState={ props.setExtraInstrument }
                                          reports={ props.reports }
                                          buttonEvent={ RemoveInstrument }
                                          showButton={ props.showExtraSelect } buttonText="Remove instrument"/>

                        <Row className="m-3">
                            <Form>
                                <Form.Group as={ Row }>
                                    <Col xs="3">Confidence interval</Col>
                                    <Col xs="6">
                                        <RangeSlider
                                            value={ props.confidenceInterval }
                                            onChange={ e => props.setConfidenceInterval(e.target.value) }
                                        />
                                    </Col>
                                    <Col xs="3">
                                        <Form.Control value={ props.confidenceInterval }
                                                      onChange={ e => props.setConfidenceInterval(e.target.value) }
                                                      type="number"/>
                                    </Col>
                                </Form.Group>
                            </Form>
                        </Row>
                    </div>
                }

                <Row className="mt-4 m-3">
                    <Col>
                        <Button onClick={ props.showResult }
                                variant="primary">{ props.showExtraSelect ? "Show instruments reports" : "Show instrument report" }</Button>
                    </Col>
                </Row>
            </div>
        </Container>
    )
}