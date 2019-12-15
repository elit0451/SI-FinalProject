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
        private static  ConnectionFactory factory;
        private static Connection requestConnection;
        private static Channel requestChannel;

        static{
            try {
                factory = new ConnectionFactory();
                factory.setHost("rabbitmq");
                requestConnection = factory.newConnection();
                requestChannel = requestConnection.createChannel();
            } catch (IOException | TimeoutException ex) {
                Logger.getLogger(MessageGateway.class.getName()).log(Level.SEVERE, null, ex);
            }
        }

        public static void ReceiveRequests() throws IOException
        {
            requestChannel.queueDeclare("connector.requests",false, false, false, null);

            DeliverCallback deliverCallback = (consumerTag, delivery) -> {
                var body = delivery.getBody();
                var message = new String(body, "UTF-8");
                CommandRouter.Route(message);
            };
            
            requestChannel.basicConsume("connector.requests", true, deliverCallback, consumerTag -> { });
        }

        public static void SendAvailableCars(String message) throws IOException
        {
            requestChannel.queueDeclare("cars.available", false, false, false, null);

            var props = new BasicProperties.Builder().replyTo("cars.available").build();

            requestChannel.basicPublish("", "cars.available", props, message.getBytes());
        }

}
