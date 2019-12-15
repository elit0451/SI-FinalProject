package com.lonelytravelers.rmiconnector;

import java.io.IOException;
import org.json.JSONObject;

public class CommandRouter {
    public static void Route(String message) throws IOException
        {
            JSONObject receivedObj = new JSONObject(message);
            String command = receivedObj.getString("command");
            switch (command)
            {
                case "getAvailableCars":
                    String dateFrom = receivedObj.getString("from");
                    String dateTo = receivedObj.getString("to");
                    String stationId = receivedObj.getString("stationId");
                    
                    String availableJSON = Connector.getAvailableCars(dateFrom, dateTo, stationId);
                    MessageGateway.SendAvailableCars(availableJSON);
                    break;
                case "addBooking":
                    String pickUpTime = receivedObj.getString("pickUpTime");
                    String deliveryTime = receivedObj.getString("deliveryTime");
                    String pickUpPlace = receivedObj.getString("pickUpPlace");
                    String deliveryPlace = receivedObj.getString("deliveryPlace");
                    String driver = receivedObj.getString("driver");
                    String car = receivedObj.getString("car");
                    
                    Connector.addNewBooking(pickUpTime, deliveryTime, pickUpPlace, deliveryPlace, driver, car);
                    break;
            }
        }
}
