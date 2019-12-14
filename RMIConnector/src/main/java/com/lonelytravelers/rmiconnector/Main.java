/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.lonelytravelers.rmiconnector;

import DTOs.AvailabilityDetails;
import DTOs.CarDetails;
import DTOs.CarTypeDetails;
import DTOs.DriverDetails;
import DTOs.HotelDetails;
import DTOs.Identifiers.StationIdentifier;
import DTOs.NewBookingDetails;
import interfaces.IBooking;
import interfaces.ICars;
import java.io.IOException;
import java.net.MalformedURLException;
import java.rmi.Naming;
import java.rmi.NotBoundException;
import java.rmi.RemoteException;
import java.rmi.registry.Registry;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.json.JSONArray;
import org.json.JSONObject;

/**
 *
 * @author Kast
 */
public class Main {
    public static Registry registry;

    public Main() throws RemoteException {
    }

    public static void main(String[] args) {
        try {
            MessageGateway.ReceiveRequests();
        } catch (IOException ex) {
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
        }
        
        while(true){}
    }
}
