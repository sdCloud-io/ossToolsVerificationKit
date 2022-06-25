import React, { useState } from "react";
import { Button, Col, Container, Row } from "react-bootstrap";
import { InstrumentPicker } from "./InstrumentPicker";
import { IntervalPicker } from "./IntervalPicker";
import { Header } from "./Header";
import { InstrumentList } from "./InstrumentList";

export function PickerComponent(props) {
    const [instruments, setInstruments] = useState([])
    const [interval, setInterval] = useState(5)

    function showInstruments() {
        const getParameters = () => {
            return { instruments, interval }
        }
        props.showReports(getParameters)
    }

    return (
        <Container className="text-center mt-5" fluid>
            <div className="picker">
                <Header/>
                <InstrumentPicker setInstruments={ setInstruments }
                                  instruments={ instruments }
                                  reportService={ props.reportService }/>
                <InstrumentList instruments={ instruments } setInstruments={ setInstruments }/>
                {
                    instruments.length > 1 &&
                    <IntervalPicker setInterval={ setInterval } interval={ interval }/>
                }

                <Row className="mt-4 m-3">
                    <Col>
                        <Button onClick={ showInstruments } variant="primary">
                            { instruments.length > 1 ? "Show instruments reports" : "Show instrument report" }
                        </Button>
                    </Col>
                </Row>
            </div>
        </Container>
    )
}