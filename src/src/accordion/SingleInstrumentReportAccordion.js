import { Accordion } from "react-bootstrap";
import { PanelHeader } from "./PanelHeader";
import { LogRaw } from "../raw/LogRaw";
import { TimeRaws } from "../raw/TimeRaws";
import { ParametersRaw } from "../raw/ParametersRaw";
import React from "react";
import { CompareService } from "../domain/CompareService";
import { Header } from "./Header";

export function SingleInstrumentReportAccordion(props) {
    const compareService = new CompareService();
    const instrumentsInfo = compareService.getInstrumentInformation(props.reports)

    return (<Accordion defaultActiveKey="0">
        <Header instrumentsInformation={ instrumentsInfo }/>
        { props.reports[0].ModelResults.map((resultItem, i) =>
            <Accordion.Item className="m-3 panel" eventKey={ i }>
                <PanelHeader modelName={ resultItem.ModelPath }
                             instrumentsInformation={ instrumentsInfo }
                             instrumentsResults={ [resultItem] }/>
                <Accordion.Body>
                    <LogRaw values={ [resultItem] }/>
                    <TimeRaws value={ [resultItem] }/>
                    <ParametersRaw parameterDictionary={ resultItem.ResultDictionary } isCompare={ false }/>
                </Accordion.Body>
            </Accordion.Item>) }
    </Accordion>)
}