import React from "react";
import { DragDropContext, Droppable } from "react-beautiful-dnd";
import { InstrumentCard } from "./InstrumentCard";

export function InstrumentList(props) {
    function onDragEnd(result) {
        const { destination, source, reason } = result;

        if (!destination || reason === 'CANCEL') {
            return;
        }

        if (destination.droppableId === source.droppableId && destination.index === source.index) {
            return;
        }

        const instrumentsCopy = Object.assign([], props.instruments)
        const droppedInstruments = props.instruments[source.index]

        instrumentsCopy.splice(source.index, 1);
        instrumentsCopy.splice(destination.index, 0, droppedInstruments)

        props.setInstruments(instrumentsCopy)
    }

    function deleteInstrument(index) {
        const instrumentsCopy = Object.assign([], props.instruments)
        instrumentsCopy.splice(index, 1);
        props.setInstruments(instrumentsCopy)
    }

    return (
        <DragDropContext onDragEnd={ onDragEnd }>
            <Droppable droppableId='dp1'>
                {
                    (provided) => (
                        <div ref={ provided.innerRef } { ...provided.droppableProps }>
                            { props.instruments.map((instrument, index) =>
                                <InstrumentCard instrument={ instrument } index={ index }
                                                onDelete={ deleteInstrument }/>) }
                            { provided.placeholder }
                        </div>
                    )
                }
            </Droppable>
        </DragDropContext>
    )
}