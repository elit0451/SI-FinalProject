package com.lonelytravelers.rmiconnector;

import com.rabbitmq.client.AMQP.BasicProperties;
import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;
import com.rabbitmq.client.DeliverCallback;
import java.io.IOException;
import java.util.concurrent.TimeoutException;
import java.util.logging.Level;
import java.util.logging.Logger;

public class MessageGateway {
    private static ConnectionFactory factory;
    private static Connection requestConnection;
    private static Channel requestChannel;

    static {
        try {
            factory = new ConnectionFactory();
            factory.setHost("rabbitmq");
            requestConnection = factory.newConnection();
            requestChannel = requestConnection.createChannel();
        } catch (IOException | TimeoutException ex) {
            Logger.getLogger(MessageGateway.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    public static void ReceiveRequests() throws IOException {
        requestChannel.exchangeDeclare("connector", "Direct");

        String queueName = requestChannel.queueDeclare().getQueue();
        requestChannel.queueBind(queueName, "connector", "requests");

        DeliverCallback deliverCallback = (consumerTag, delivery) -> {
            var body = delivery.getBody();
            var message = new String(body, "UTF-8");
            CommandRouter.Route(message);
        };

        requestChannel.basicConsume(queueName, true, deliverCallback, consumerTag -> {
        });
    }

    public static void SendAvailableCars(String message) throws IOException {
        requestChannel.exchangeDeclare("cars", "Direct");

        var props = new BasicProperties.Builder().build();

        requestChannel.basicPublish("cars", "available", props, message.getBytes());
    }

}
