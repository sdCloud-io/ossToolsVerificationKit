import { Button, Col, Row } from "react-bootstrap";
import Form from "react-bootstrap/Form";
import React, { useState } from "react";
import { onlyUnique } from "../ArrayHelper";
import { useSnackbar } from 'notistack';

export function InstrumentPicker(props) {

    const defaultInstrument = props.reportService.getDefaultInstrument()

    const { enqueueSnackbar} = useSnackbar();
    const [name, setName] = useState(defaultInstrument.Name)
    const [version, setVersion] = useState(defaultInstrument.Version)
    const [modelType, setModelType] = useState(defaultInstrument.ModelType)

    function GetInstrumentsName() {
        return props.reportService.instruments.map(instrument => instrument.Name).filter(onlyUnique)
    }

    function OnChangeInstrumentName(e) {
        setName(e.target.value)
        const modelType = props.reportService.getModelTypesByName(e.target.value)[0]
        setModelType(modelType)
        const version = props.reportService.getModelTypesByNameAndModelType(e.target.value, modelType)[0]
        setVersion(version)
    }

    function GetInstrumentModelType() {
        return props.reportService.getModelTypesByName(name).filter(onlyUnique)
    }

    function OnChangeInstrumentModelType(e) {
        setModelType(e.target.value)
        const version = props.reportService.getModelTypesByNameAndModelType(name, e.target.value)[0]
        setVersion(version)
    }

    function GetInstrumentVersion() {
        return props.reportService.getModelTypesByNameAndModelType(name, modelType)
    }

    function OnChangeInstrumentVersion(e) {
        setVersion(e.target.value)
    }

    function AddInstrument() {
        if (props.instruments.length > 4) {
            enqueueSnackbar('Can compare only 5 instruments', {
                autoHideDuration: 5000, variant: 'warning', anchorOrigin: {
                    vertical: 'top',
                    horizontal: 'right'
                }
            });
            return
        }
        const instrument = {
            Name: name,
            ModelType: modelType,
            InstrumentVersion: version
        }
        props.setInstruments(props.instruments.concat(instrument))
    }

    return (
        <Row className="m-3">
            <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                <Form.Select onChange={ OnChangeInstrumentName } aria-label="Default select example">
                    { GetInstrumentsName().map(item => <option value={ item }>{ item }</option>) }
                </Form.Select>
            </Col>
            <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                <Form.Select onChange={ OnChangeInstrumentModelType } aria-label="Default select example">
                    { GetInstrumentModelType().map(item => <option value={ item }>{ item }</option>) }
                </Form.Select>
            </Col>
            <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                <Form.Select onChange={ OnChangeInstrumentVersion } aria-label="Default select example">
                    { GetInstrumentVersion().map(item => <option value={ item }>{ item }</option>) }
                </Form.Select>
            </Col>
            <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                <Button onClick={ AddInstrument } variant="outline-dark">
                    Add instrument
                </Button>
            </Col>
        </Row>
    )
}