import { Accordion } from "react-bootstrap";
import { PanelHeader } from "./PanelHeader";
import { LogRaw } from "../raw/LogRaw";
import { TimeRaws } from "../raw/TimeRaws";
import { CompareRaws } from "../raw/CompareRaws";
import React from "react";
import { ParametersRaw } from "../raw/ParametersRaw";

export function ReportAccordion(props) {

    function containModel(ModelPath) {
        return props.reports[1].ModelResults.findIndex(elem => elem.ModelPath === ModelPath) !== -1;
    }


    if (props.reports.length === 0) return <div/>
    const nameOfInstrument = props.reports[0].InstrumentName;

    if (props.reports.length === 1) {
        return (<Accordion defaultActiveKey="0">
            { props.reports[0].ModelResults.map((resultItem, i) =>
                <Accordion.Item className="m-3 panel" eventKey={ i }>
                    <PanelHeader value={ [resultItem] } instrumentsNames={ [nameOfInstrument] }/>
                    <Accordion.Body>
                        <LogRaw values={ [resultItem] }/>
                        <TimeRaws value={ [resultItem] }/>
                        <ParametersRaw parameterDictionary={ resultItem.ResultDictionary } isCompare={ false }/>
                    </Accordion.Body>
                </Accordion.Item>) }
        </Accordion>)
    } else {
        const nameOfExtraInstrument = props.reports[1].InstrumentName;
        const compareReports = props.reports[0].ModelResults.filter(item => containModel(item.ModelPath))
            .map(elem => [elem, props.reports[1].ModelResults.find(modelResult => modelResult.ModelPath === elem.ModelPath)])

        return (
            <Accordion defaultActiveKey="0">
                { compareReports.map((resultItem, i) =>
                    <Accordion.Item className="m-3 panel" eventKey={ i }>
                        <PanelHeader value={ resultItem }
                                     instrumentsNames={ [nameOfInstrument, nameOfExtraInstrument] }/>
                        <Accordion.Body>
                            <LogRaw values={ resultItem }/>
                            <TimeRaws value={ resultItem }/>
                            <CompareRaws confidenceInterval={ props.confidenceInterval } value={ resultItem }/>
                        </Accordion.Body>
                    </Accordion.Item>) }
            </Accordion>
        )
    }
}