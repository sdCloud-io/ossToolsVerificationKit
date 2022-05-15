import { Button, Col, Row } from "react-bootstrap";
import Form from "react-bootstrap/Form";
import React, { useState } from "react";

export function InstrumentPicker(props) {
    function GetInstrumentsName() {
        return [...new Set(props.reports.map(item => item.Name))]
    }

    function OnChangeInstrumentName(e) {
        const name = e.target.value

        const modelType = [...new Set(props.reports
            .filter(item => item.Name === props.state.Name)
            .map(item => item.ModelType))];

        const versions = [...new Set(props.reports
            .filter(item => item.Name === name && item.ModelType == modelType[0])
            .map(item => item.InstrumentVersion))];

        props.updateState(prev => ({
            ...prev,
            Name : e.target.value,
            ModelType: modelType[0],
            Version: versions[0]
        }))
    }

    function GetInstrumentModelType() {
        return [...new Set(props.reports.filter(item => item.Name === props.state.Name).map(item => item.ModelType))]
    }

    function OnChangeInstrumentModelType(e) {
        const modelType = e.target.value

        const versions = [...new Set(props.reports
            .filter(item => item.Name === props.state.Name && item.ModelType == modelType)
            .map(item => item.InstrumentVersion))];

        props.updateState(prev => ({
            ...prev,
            ModelType: modelType,
            Version: versions[0]
        }))
    }

    function GetInstrumentVersion() {
        return [...new Set(props.reports
            .filter(item => item.Name === props.state.Name && item.ModelType == props.state.ModelType)
            .map(item => item.InstrumentVersion))];
    }

    function OnChangeInstrumentVersion(e) {
        const version = e.target.value;
        props.updateState(prev => ({
            ...prev,
            Version: version
        }))
    }

    return (
        <Row className="m-3">
            <Col lg={ 3 } md={ 3 } sm={ 3 }
                 xl={ 3 } xs={ 3 }
                 xxl={ 3 }>
                <Form.Select onChange={ OnChangeInstrumentName } aria-label="Default select example">
                    { GetInstrumentsName().map(item => <option value={ item }>{ item }</option>) }
                </Form.Select>
            </Col>
            <Col lg={ 3 } md={ 3 } sm={ 3 }
                 xl={ 3 } xs={ 3 }
                 xxl={ 3 }>
                <Form.Select onChange={ OnChangeInstrumentModelType } aria-label="Default select example">
                    { GetInstrumentModelType().map(item => <option value={ item }>{ item }</option>) }
                </Form.Select>
            </Col>
            <Col lg={ 3 } md={ 3 } sm={ 3 }
                 xl={ 3 } xs={ 3 }
                 xxl={ 3 }>
                <Form.Select onChange={ OnChangeInstrumentVersion } aria-label="Default select example">
                    { GetInstrumentVersion().map(item => <option value={ item }>{ item }</option>) }
                </Form.Select>
            </Col>
            {
                props.showButton &&
                <Col>
                    <div className="d-grid gap-2">
                        <Button onClick={ props.buttonEvent } variant="outline-dark" className="align-middle">
                            { props.buttonText }
                        </Button>
                    </div>
                </Col>
            }
        </Row>
    )
}