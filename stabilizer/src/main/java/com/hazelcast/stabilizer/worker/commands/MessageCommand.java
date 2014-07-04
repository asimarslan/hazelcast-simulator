package com.hazelcast.stabilizer.worker.commands;

import com.hazelcast.stabilizer.common.messaging.Message;

public class MessageCommand extends Command {
    private final Message message;

    public MessageCommand(Message message) {
        this.message = message;
    }

    public Message getMessage() {
        return message;
    }

    @Override
    public boolean awaitReply() {
        return false;
    }
}
