import React from "react";
import { Draggable } from "react-beautiful-dnd";
import { Button, Col, Row } from "react-bootstrap";

export function InstrumentCard(props) {
    return (
        <Draggable
            key={ props.index }
                   draggableId={ props.index + '' }
                   index={ props.index }
        >
            { (provided) =>
                <div ref={ provided.innerRef }
                     { ...provided.draggableProps }
                     { ...provided.dragHandleProps }>
                    <Row className="m-3 align-items-center instrument-card border-card p-2">
                        <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                            <h6>{ props.index + 1 }) { props.instrument.Name }</h6>
                        </Col>
                        <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                            <h6>{ props.instrument.ModelType }</h6>
                        </Col>
                        <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                            <h6>{ props.instrument.InstrumentVersion }</h6>
                        </Col>
                        <Col lg={ 3 } md={ 3 } sm={ 3 } xl={ 3 } xs={ 3 } xxl={ 3 }>
                            <Button onClick={ () => props.onDelete(props.index) } variant="outline-danger">
                                Delete
                            </Button>
                        </Col>
                    </Row>
                </div>
            }
        </Draggable>
    )
}