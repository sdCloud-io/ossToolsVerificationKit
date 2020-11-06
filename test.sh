#!/bin/sh

SCRIPT_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"

source $SCRIPT_HOME/log.sh
source $SCRIPT_HOME/time.sh
source $SCRIPT_HOME/config.sh
source $SCRIPT_HOME/operations.sh
source $SCRIPT_HOME/file.sh
source $SCRIPT_HOME/util.sh
source $SCRIPT_HOME/json.sh
source $SCRIPT_HOME/report.sh

log_printScriptHeader


log_printSectionHeader "Script initial variables:"
log_printVariable "CONST_TEST_MODELS_REPO"
log_printVariable "CONST_SDE_REPO"
log_printVariable "CONST_PYSD_REPO"

log_printInfoLine ""
log_printInfoLine "Setting up build directory"
BUILD_DIR="build"
if [ -d "$BUILD_DIR" ]; then rm -Rf $BUILD_DIR; fi
mkdir $BUILD_DIR
cd $BUILD_DIR

report_openReport "$SCRIPT_HOME/build/testReport.json"
report_writeEnvironment


log_printInfoLine ""

log_printInfoLine "Checking out test models repository from $CONST_TEST_MODELS_REPO"
git clone $CONST_TEST_MODELS_REPO ./$CONST_TEST_MODELS_HOME 2>&1 | log_wrapExternalLogOutput

log_printInfoLine "Checking out SDEverywhere repository from $CONST_SDE_ENGINE_HOME"
git clone $CONST_SDE_REPO ./$CONST_SDE_ENGINE_HOME 2>&1 | log_wrapExternalLogOutput

log_printInfoLine "Checking out PySD repository from $CONST_PYSD_REPO"
git clone $CONST_PYSD_REPO ./$CONST_PYSD_ENGINE_HOME 2>&1 | log_wrapExternalLogOutput



log_printSectionHeader "Building SDEverywhere with npm:"
pushd $CONST_SDE_ENGINE_HOME 2>&1 > /dev/null
npm install 2>&1 | log_wrapExternalLogOutput
SDE_CMD="node ./$CONST_SDE_ENGINE_HOME/src/sde.js"
log_printVariable "SDE_CMD"
popd 2>&1 > /dev/null

log_printSectionHeader "Preparing PySD for tests"
ln -s $CONST_PYSD_ENGINE_HOME/pysd ./pysd
cp ../PySDHelper.py ./
PySD_CMD="./PySDHelper.py"
chmod +x PySDHelper.py


report_startResultsArray "sdeResults"
time_rememberMoment "startedProcessingAllModels"

log_printSectionHeader "Starting test Vensim models execution with SDEverywhere"

modelsCount=0
successCount=0
failuresCount=0

for MODEL_PATH in $(file_listAllVensimModels)
do
modelsCount=`expr $modelsCount + 1`
#MODEL_PATH="/extra/repos/sdCertTool-repo/build/testModels/tests/trig/test_trig.mdl"
#MODEL_PATH="/extra/repos/sdCertTool-repo/build/testModels/tests/subscript_1d_arrays/test_subscript_1d_arrays.mdl"
    ops_executeModelWithSDE "$SDE_CMD" "$MODEL_PATH"

    if util_lastOperationFailed $?
    then
        failuresCount=`expr $failuresCount + 1`
        safeLog=`cat $LOG_PATH | sed 's/"/_/g'`
        report_writeFailedExecution \
                                    "$MODEL_PATH" \
                                    "$safeLog"    \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_GENERATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_COMPILATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_EXECUTION]} \
                                    $MODEL_PATH
    else
        successCount=`expr $successCount + 1`
        safeLog=`cat $LOG_PATH | sed 's/"/_/g'`
        report_writeSuccessfulExecution \
                                    "$MODEL_PATH" \
                                    "$safeLog"    \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_GENERATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_COMPILATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_EXECUTION]} \
                                    $MODEL_PATH
    fi
done

timeDelta=`time_timeSinceRememberedMoment "startedProcessingAllModels"`

report_endResultsArray




report_startResultsArray "pysdXmileResults"
time_rememberMoment "startedProcessingAllModels"

modelsCount=0
successCount=0
failuresCount=0

log_printSectionHeader "Starting test XMILE models execution with PySD"

for MODEL_PATH in $(file_listAllXMILEModels)
do
modelsCount=`expr $modelsCount + 1`
#MODEL_PATH="/extra/repos/sdCertTool-repo/build/testModels/tests/trig/test_trig.mdl"
#MODEL_PATH="/extra/repos/sdCertTool-repo/build/testModels/tests/subscript_1d_arrays/test_subscript_1d_arrays.mdl"

    ops_executeXmileModelWithPySD "$PySD_CMD" "$MODEL_PATH"

    if util_lastOperationFailed $?
    then
        failuresCount=`expr $failuresCount + 1`
        safeLog=`cat $LOG_PATH | sed 's/"/_/g'`
        report_writeFailedExecution \
                                    "$MODEL_PATH" \
                                    "$safeLog"    \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_GENERATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_COMPILATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_EXECUTION]} \
                                    $MODEL_PATH
    else
        successCount=`expr $successCount + 1`
        safeLog=`cat $LOG_PATH | sed 's/"/_/g'`
        report_writeSuccessfulExecution \
                                    "$MODEL_PATH" \
                                    "$safeLog"    \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_GENERATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_COMPILATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_EXECUTION]} \
                                    $MODEL_PATH
    fi
done

timeDelta=`time_timeSinceRememberedMoment "startedProcessingAllModels"`

report_endResultsArray






report_startResultsArray "pysdResults"
time_rememberMoment "startedProcessingAllModels"

modelsCount=0
successCount=0
failuresCount=0

log_printSectionHeader "Starting test Vensim models execution with PySD"

for MODEL_PATH in $(file_listAllVensimModels)
do
modelsCount=`expr $modelsCount + 1`
#MODEL_PATH="/extra/repos/sdCertTool-repo/build/testModels/tests/trig/test_trig.mdl"
#MODEL_PATH="/extra/repos/sdCertTool-repo/build/testModels/tests/subscript_1d_arrays/test_subscript_1d_arrays.mdl"

    ops_executeModelWithPySD "$PySD_CMD" "$MODEL_PATH"

    if util_lastOperationFailed $?
    then
        failuresCount=`expr $failuresCount + 1`
        safeLog=`cat $LOG_PATH | sed 's/"/_/g'`
        report_writeFailedExecution \
                                    "$MODEL_PATH" \
                                    "$safeLog"    \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_GENERATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_COMPILATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_EXECUTION]} \
                                    $MODEL_PATH
    else
        successCount=`expr $successCount + 1`
        safeLog=`cat $LOG_PATH | sed 's/"/_/g'`
        report_writeSuccessfulExecution \
                                    "$MODEL_PATH" \
                                    "$safeLog"    \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_GENERATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_COMPILATION]} \
                                    ${LAST_OPERATION_RESULT[$OPS_MOMENT_MODEL_CODE_EXECUTION]} \
                                    $MODEL_PATH
    fi
done

timeDelta=`time_timeSinceRememberedMoment "startedProcessingAllModels"`

report_endResultsArray



report_writeOverallExecutionMetrics \
                                    $timeDelta \
                                    $modelsCount \
                                    $successCount \
                                    $failuresCount

log_printInfoLine "*************************************************"
log_printInfoLine " All Vensim models in test repo were"
log_printInfoLine " processed by SDEverywhere in $timeDelta ms"
log_printInfoLine "*************************************************"
report_closeReport