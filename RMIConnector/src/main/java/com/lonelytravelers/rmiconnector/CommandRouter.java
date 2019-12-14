/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.lonelytravelers.rmiconnector;

import DTOs.AvailabilityDetails;
import java.io.IOException;
import org.json.JSONObject;

/**
 *
 * @author Kast
 */
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
