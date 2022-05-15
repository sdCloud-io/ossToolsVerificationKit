import React, { useState } from "react";
import { ReportPickerComponent } from "./reportPicker/ReportPickerComponent";
import { ExtractDescription } from "./mapper";
import { ReportAccordion } from "./accordion/ReportAccordion";


export function AppComponent(props) {
    const [showExtraSelect, updateExtraSelect] = useState(false)
    const [instrument, setInstrument] = useState(getDefaultState)
    const [extraInstrument, setExtraInstrument] = useState(getDefaultState)
    const [confidenceInterval, setConfidenceInterval] = React.useState(5);
    const [reports, setReport] = useState([])

    function getDefaultState() {
        return {
            Name: props.reports.map(ExtractDescription)[0].Name,
            ModelType: props.reports.map(ExtractDescription)[0].ModelType,
            Version: props.reports.map(ExtractDescription)[0].InstrumentVersion
        }
    }

    function showResult() {
        setReport(getReports())
    }

    function getReports() {
        const report = props.reports.find(item => item.InstrumentName === instrument.Name && item.ModelTypes == instrument.ModelType && item.InstrumentVersion === instrument.Version);
        if (showExtraSelect) {
            const extraReport = props.reports.find(item => item.InstrumentName === extraInstrument.Name && item.ModelTypes == extraInstrument.ModelType && item.InstrumentVersion === extraInstrument.Version);
            return [report, extraReport];
        }
        return [report]
    }

    return (
        <div className="mt-5">
            <ReportPickerComponent
                reports={ props.reports.map(ExtractDescription) } showExtraSelect={ showExtraSelect }
                updateShowExtra={ updateExtraSelect } showResult={ showResult } instrument={ instrument }
                setInstrument={ setInstrument } extraInstrument={ extraInstrument }
                setExtraInstrument={ setExtraInstrument } confidenceInterval={ confidenceInterval }
                setConfidenceInterval={ setConfidenceInterval }/>
            <ReportAccordion reports={ reports } confidenceInterval={ confidenceInterval }/>
        </div>)
}