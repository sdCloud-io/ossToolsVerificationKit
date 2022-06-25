import { Accordion } from "react-bootstrap";
import { PanelHeader } from "./PanelHeader";
import { LogRaw } from "../raw/LogRaw";
import { TimeRaws } from "../raw/TimeRaws";
import { CompareRaws } from "../raw/CompareRaws";
import React from "react";
import { CompareService } from "../domain/CompareService";
import { Header } from "./Header";

export function MultipleInstrumentReportAccordion(props) {
    const compareService = new CompareService()
    const reports = compareService.filterByCommonModelResult(props.reports)
    const instrumentsInfo = compareService.getInstrumentInformation(props.reports)

    return (
        <Accordion defaultActiveKey="0">
            <Header instrumentsInformation={ instrumentsInfo }/>
            { reports.map((item, i) =>
                <Accordion.Item className="m-3 panel" eventKey={ i }>
                    <PanelHeader modelName={ item.ModelName }
                                 instrumentsInformation={ instrumentsInfo } instrumentsResults={ item.Values }/>
                    <Accordion.Body>
                        <LogRaw values={ item.Values }/>
                        <TimeRaws value={ item.Values }/>
                        <CompareRaws
                            value={ compareService.compareInstruments(props.confidenceInterval, item.Values) }/>
                    </Accordion.Body>
                </Accordion.Item>) }
        </Accordion>
    )
}