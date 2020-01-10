package com.lonelytravelers.rmiconnector;

import java.io.IOException;
import org.json.JSONObject;

public class CommandRouter {
    public static void Route(String message) throws IOException
        {
            JSONObject receivedObj = new JSONObject(message);
            String command = receivedObj.getString("Command");
            switch (command)
            {
                case "getAvailableCars":
                    String dateFrom = receivedObj.getString("From");
                    String dateTo = receivedObj.getString("To");
                    String stationId = receivedObj.getString("StationId");
                    
                    String availableJSON = Connector.getAvailableCars(dateFrom, dateTo, stationId);
                    MessageGateway.SendAvailableCars(availableJSON);
                    break;
                case "addBooking":
                    String pickUpTime = receivedObj.getString("PickUpTime");
                    String deliveryTime = receivedObj.getString("DeliveryTime");
                    String pickUpPlace = receivedObj.getString("PickUpPlace");
                    String deliveryPlace = receivedObj.getString("DeliveryPlace");
                    String driver = receivedObj.getString("Driver");
                    String car = receivedObj.getString("Car");
                    
                    Connector.addNewBooking(pickUpTime, deliveryTime, pickUpPlace, deliveryPlace, driver, car);
                    break;
            }
        }
}
