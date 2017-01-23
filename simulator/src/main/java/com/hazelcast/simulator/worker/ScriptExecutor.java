/*
 * Copyright (c) 2008-2016, Hazelcast, Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.hazelcast.simulator.worker;

import com.hazelcast.core.HazelcastInstance;
import com.hazelcast.simulator.protocol.core.ResponseType;
import com.hazelcast.simulator.protocol.operation.ExecuteScriptOperation;
import com.hazelcast.simulator.utils.BashCommand;
import com.hazelcast.simulator.utils.EmbeddedScriptCommand;
import org.apache.log4j.Logger;

import java.io.File;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.Callable;

import static com.hazelcast.simulator.utils.FileUtils.fileAsText;
import static com.hazelcast.simulator.utils.FileUtils.getUserDir;
import static java.lang.String.format;

public class ScriptExecutor {
    private static final Logger LOGGER = Logger.getLogger(ScriptExecutor.class);

    private final HazelcastInstance hazelcastInstance;

    public ScriptExecutor(HazelcastInstance hazelcastInstance) {
        this.hazelcastInstance = hazelcastInstance;
    }

    public void execute(final ExecuteScriptOperation operation, final Promise promise) {
        if (operation.isFireAndForget()) {
            promise.answer(ResponseType.SUCCESS);
        }

        String fullCommand = operation.getCommand();
        int indexColon = fullCommand.indexOf(":");
        String extension = fullCommand.substring(0, indexColon);
        final String command = fullCommand.substring(indexColon + 1);

        final Callable<String> task;
        if (extension.equals("bash")) {
            task = newBashScriptCallable(command);
        } else {
            task = newGenericScriptCallable(extension, command);
        }

        new Thread() {
            public void run() {
                try {
                    String result = task.call();
                    promise.answer(ResponseType.SUCCESS, result);
                } catch (Exception e) {
                    LOGGER.warn("Failed to execute script: " + command, e);
                    promise.answer(ResponseType.EXCEPTION_DURING_OPERATION_EXECUTION, e.getMessage());
                }
            }
        }.start();
    }

    private Callable<String> newBashScriptCallable(final String command) {
        Callable<String> task;
        task = new Callable<String>() {
            @Override
            public String call() throws Exception {
                Map<String, Object> environment = new HashMap<String, Object>();
                File pidFile = new File(getUserDir(), "worker.pid");
                if (pidFile.exists()) {
                    environment.put("PID", fileAsText(pidFile));
                }

                return new BashCommand(command)
                        .setDirectory(getUserDir())
                        .addEnvironment(environment)
                        .setThrowsException(true)
                        .execute();
            }
        };
        return task;
    }

    private Callable<String> newGenericScriptCallable(final String extension, final String command) {
        Callable<String> task;
        task = new Callable<String>() {
            @Override
            public String call() throws Exception {
                Object result = new EmbeddedScriptCommand(command)
                        .addEnvironment("hazelcastInstance", hazelcastInstance)
                        .addEnvironment("simulator", new SimulatorScope())
                        .setEngineName(extension)
                        .execute();
                LOGGER.info(format("Script [%s] with [%s]", command, result));
                return result == null ? "null" : result.toString();
            }
        };
        return task;
    }

    public class SimulatorScope {

        public void exit(int exitCode) {
            System.exit(exitCode);
        }
    }
}
