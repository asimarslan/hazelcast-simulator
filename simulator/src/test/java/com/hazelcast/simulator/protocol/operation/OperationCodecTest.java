package com.hazelcast.simulator.protocol.operation;

import com.hazelcast.simulator.agent.workerjvm.WorkerJvmSettings;
import com.hazelcast.simulator.common.JavaProfiler;
import com.hazelcast.simulator.coordinator.CoordinatorParameters;
import com.hazelcast.simulator.protocol.core.SimulatorMessage;
import com.hazelcast.simulator.worker.WorkerType;
import org.apache.log4j.Logger;
import org.junit.Test;

import java.util.Collections;

import static com.hazelcast.simulator.protocol.core.SimulatorAddress.COORDINATOR;
import static com.hazelcast.simulator.utils.ReflectionUtils.invokePrivateConstructor;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

public class OperationCodecTest {

    private static final Logger LOGGER = Logger.getLogger(OperationCodecTest.class);

    @Test
    public void testConstructor() throws Exception {
        invokePrivateConstructor(OperationCodec.class);
    }

    @Test
    public void testCodec_fromJson() {
        IntegrationTestOperation operation = new IntegrationTestOperation("codecTest_fromJson");
        String json = OperationCodec.toJson(operation);
        assertNotNull(json);

        IntegrationTestOperation decoded = (IntegrationTestOperation) OperationCodec.fromJson(json, IntegrationTestOperation.class);
        assertEquals(operation.getTestData(), decoded.getTestData());
    }

    @Test
    public void testCodec_fromSimulatorMessage() {
        IntegrationTestOperation operation = new IntegrationTestOperation("codecTest_fromSimulatorMessage");
        String json = OperationCodec.toJson(operation);
        assertNotNull(json);

        SimulatorMessage message = new SimulatorMessage(COORDINATOR, COORDINATOR, 0, OperationType.INTEGRATION_TEST, json);
        IntegrationTestOperation decoded = (IntegrationTestOperation) OperationCodec.fromSimulatorMessage(message);
        assertEquals(operation.getTestData(), decoded.getTestData());
    }

    @Test
    public void testCodec_withComplexOperation() {
        CoordinatorParameters parameters = mock(CoordinatorParameters.class);
        when(parameters.getMemberJvmOptions()).thenReturn("-verbose:gc -Xloggc:verbosegc.log");
        when(parameters.getMemberHzConfig()).thenReturn("<hazelcast xsi:schemaLocation=\"http://www.hazelcast.com/schema/config\n"
                + "  http://www.hazelcast.com/schema/config/hazelcast-config-3.6.xsd\"\n"
                + "  xmlns=\"http://www.hazelcast.com/schema/config\"\n"
                + "  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" />");
        when(parameters.isAutoCreateHzInstance()).thenReturn(true);
        when(parameters.getWorkerStartupTimeout()).thenReturn(12345);
        when(parameters.getProfiler()).thenReturn(JavaProfiler.FLIGHTRECORDER);

        WorkerJvmSettings workerJvmSettings = new WorkerJvmSettings(1, WorkerType.MEMBER, parameters);

        CreateWorkerOperation operation = new CreateWorkerOperation(Collections.singletonList(workerJvmSettings));
        String json = OperationCodec.toJson(operation);
        assertNotNull(json);
        LOGGER.info(json);

        CreateWorkerOperation decoded = (CreateWorkerOperation) OperationCodec.fromJson(json, CreateWorkerOperation.class);
        WorkerJvmSettings decodedSettings = decoded.getWorkerJvmSettings().get(0);
        assertEquals(workerJvmSettings.getWorkerIndex(), decodedSettings.getWorkerIndex());
        assertEquals(workerJvmSettings.getWorkerType(), decodedSettings.getWorkerType());
        assertEquals(workerJvmSettings.getHazelcastConfig(), decodedSettings.getHazelcastConfig());
        assertEquals(workerJvmSettings.getJvmOptions(), decodedSettings.getJvmOptions());
        assertEquals(workerJvmSettings.isAutoCreateHzInstance(), decodedSettings.isAutoCreateHzInstance());
        assertEquals(workerJvmSettings.getWorkerStartupTimeout(), decodedSettings.getWorkerStartupTimeout());
        assertEquals(workerJvmSettings.getProfiler(), decodedSettings.getProfiler());
        assertEquals(workerJvmSettings.getProfilerSettings(), decodedSettings.getProfilerSettings());
        assertEquals(workerJvmSettings.getNumaCtl(), decodedSettings.getNumaCtl());
    }
}
