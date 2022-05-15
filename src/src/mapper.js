export function ExtractDescription(report){
    return {
        Name : report.InstrumentName,
        ModelType : report.ModelTypes,
        InstrumentVersion : report.InstrumentVersion
    }
}