package com.hazelcast.simulator.protocol.core;

import java.util.HashMap;
import java.util.Map;
import java.util.Set;

/**
 * Response which is sent back to the sender {@link SimulatorAddress} of a {@link SimulatorMessage}.
 *
 * Returns a {@link ResponseType} per destination {@link SimulatorAddress},
 * e.g. if multiple Simulator components have been addressed by a single {@link SimulatorMessage}.
 */
public class Response {

    private final Map<SimulatorAddress, ResponseType> responseTypes = new HashMap<SimulatorAddress, ResponseType>();

    private final long messageId;
    private final SimulatorAddress destination;

    public Response(SimulatorMessage message) {
        this(message.getMessageId(), message.getSource());
    }

    public Response(long messageId, SimulatorAddress destination, SimulatorAddress source, ResponseType responseType) {
        this(messageId, destination);
        responseTypes.put(source, responseType);
    }

    public Response(long messageId, SimulatorAddress destination) {
        this.messageId = messageId;
        this.destination = destination;
    }

    public void addResponse(SimulatorAddress address, ResponseType responseType) {
        responseTypes.put(address, responseType);
    }

    public void addResponse(Response response) {
        responseTypes.putAll(response.responseTypes);
    }

    public long getMessageId() {
        return messageId;
    }

    public SimulatorAddress getDestination() {
        return destination;
    }

    public int size() {
        return responseTypes.size();
    }

    public Set<Map.Entry<SimulatorAddress, ResponseType>> entrySet() {
        return responseTypes.entrySet();
    }

    public ResponseType getFirstErrorResponseType() {
        for (ResponseType responseType : responseTypes.values()) {
            if (responseType != ResponseType.SUCCESS) {
                return responseType;
            }
        }
        return ResponseType.SUCCESS;
    }

    @Override
    public String toString() {
        return "Response{"
                + "messageId=" + messageId
                + ", destination=" + destination
                + ", responseTypes=" + responseTypes
                + '}';
    }
}
