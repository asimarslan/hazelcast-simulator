#!/bin/bash
#
# Script to start up a Simulator Worker. To customize the behavior of the worker, including Java configuration,
# copy this file into the 'work dir' of simulator. See the end of this file for examples for different profilers.
#

# Automatic exit on script failure.
set -e

# Printing the command being executed (useful for debugging)
set -x


# redirecting output/error to the right logfiles.
exec > worker.out
exec 2>worker.err


echo ".Net client worker is starting ..."
echo $SIMULATOR_HOME
echo $WORKER_TYPE
echo $WORKER_INDEX
echo $HAZELCAST_CONFIG

SIMULATOR_DOTNET_LIB="/cygdrive/c/Users/asim/git/hazelcast-simulator/workers/dotnet/SimulatorWorker/bin/Debug"

#DOTNET_PATH="$SIMULATOR_HOME"

#hazelcast.logging.type="log4j"
#log4j.configuration="file:log4j.xml"
SIMULATOR_HOME=$SIMULATOR_HOME
workerHome="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo $(eval "echo \"$LOG4j_CONFIG\"")>log4net.xml
echo $HAZELCAST_CONFIG>hazelcast-client.xml

WORKER_ARGS="publicAddress=$PUBLIC_ADDRESS \
            agentIndex=$AGENT_INDEX \
            workerType=$WORKER_TYPE \
            workerId=$WORKER_ID \
            workerIndex=$WORKER_INDEX \
            workerPort=$WORKER_PORT \
            workerPerformanceMonitorIntervalSeconds=$WORKER_PERFORMANCE_MONITOR_INTERVAL_SECONDS \
            autoCreateHzInstance=$AUTOCREATE_HAZELCAST_INSTANCE \
            hzConfigFile=hazelcast-client.xml \
            workerHome=$workerHome"

"$SIMULATOR_DOTNET_LIB/SimulatorWorker.exe" $WORKER_ARGS
