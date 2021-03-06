package com.hazelcast.simulator.worker.metronome;

import org.junit.Test;

import static java.lang.String.format;
import static org.junit.Assert.assertTrue;

public abstract class AbstractMetronomeTest {

    private Metronome metronome;

    abstract MetronomeType getMetronomeType();

    @Test
    public void testWithFixedIntervalMs() {
        int intervalMs = 50;
        metronome = getFixedIntervalMsMetronome(intervalMs);

        testMetronome(intervalMs);
    }

    @Test
    public void testWithFixedFrequency_25() {
        float frequency = 25;
        int intervalMs = 40;
        metronome = getFixedFrequencyMetronome(frequency);

        testMetronome(intervalMs);
    }

    @Test
    public void testWithFixedFrequency_100() {
        float frequency = 100;
        int intervalMs = 10;
        metronome = getFixedFrequencyMetronome(frequency);

        testMetronome(intervalMs);
    }

    @Test
    public void testWithFixedFrequency_1000() {
        float frequency = 1000;
        int intervalMs = 1;
        metronome = getFixedFrequencyMetronome(frequency);

        testMetronome(intervalMs);
    }

    private Metronome getFixedIntervalMsMetronome(int intervalMs) {
        return new MetronomeBuilder()
                .withIntervalMillis(intervalMs)
                .withMetronomeType(getMetronomeType())
                .build();
    }

    private Metronome getFixedFrequencyMetronome(float frequency) {
        return new MetronomeBuilder()
                .withRatePerSecond(frequency)
                .withMetronomeType(getMetronomeType())
                .build();
    }

    private void testMetronome(int intervalMs) {
        // we don't want to measure the first invocation, since it has a random delay
        metronome.waitForNext();

        long lastTimestamp = 0;
        for (int i = 0; i < 10; i++) {
            long startTimestamp = System.currentTimeMillis();
            metronome.waitForNext();

            if (lastTimestamp != 0) {
                long currentTimestamp = System.currentTimeMillis();
                long actual = lastTimestamp + intervalMs;
                assertTrue(format("Expected currentTimestamp >= lastTimestamp + interval, but was %d >= %d (diff: %d ms)",
                        currentTimestamp, actual, actual - currentTimestamp), currentTimestamp >= actual);
            }
            lastTimestamp = startTimestamp;
        }
    }
}
