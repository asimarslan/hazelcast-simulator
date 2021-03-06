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
package com.hazelcast.simulator.probes.impl;

import com.hazelcast.simulator.probes.Probe;
import org.HdrHistogram.Histogram;
import org.HdrHistogram.Recorder;

import static java.util.concurrent.TimeUnit.HOURS;
import static java.util.concurrent.TimeUnit.MICROSECONDS;

/**
 * Measures the latency distribution of a test.
 */
public class HdrProbe implements Probe {

    // we care only about microsecond accuracy.
    public static final long LOWEST_DISCERNIBLE_VALUE = MICROSECONDS.toNanos(1);

    // we want to track up to an hour.
    public static final long HIGHEST_TRACKABLE_VALUE = HOURS.toNanos(1);

    // since we care about us, the value should be 1000 according to the javadoc of Recorder.
    public static final int NUMBER_OF_SIGNIFICANT_VALUE_DIGITS = 3;

    // these settings come the website; just above the following link
    //https://github.com/HdrHistogram/HdrHistogram#histogram-variants-and-internal-representation
    private final Recorder recorder = new Recorder(
            LOWEST_DISCERNIBLE_VALUE,
            HIGHEST_TRACKABLE_VALUE,
            NUMBER_OF_SIGNIFICANT_VALUE_DIGITS);

    private final boolean partOfTotalThroughput;

    public HdrProbe(boolean partOfTotalThroughput) {
        this.partOfTotalThroughput = partOfTotalThroughput;
    }

    @Override
    public boolean isPartOfTotalThroughput() {
        return partOfTotalThroughput;
    }

    @Override
    public void done(long startNanos) {
        if (startNanos <= 0) {
            throw new IllegalArgumentException("startedNanos has to be a positive number");
        }

        long nowNanos = System.nanoTime();
        recordValue(nowNanos - startNanos);
    }

    @Override
    public void recordValue(long latencyNanos) {
        if (latencyNanos > HIGHEST_TRACKABLE_VALUE) {
            latencyNanos = HIGHEST_TRACKABLE_VALUE;
        }
        recorder.recordValue(latencyNanos);
    }

    public Histogram getIntervalHistogram() {
        return recorder.getIntervalHistogram();
    }
}
