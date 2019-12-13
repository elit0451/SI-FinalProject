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

    public static void main(String[] args) throws ParseException {
        try {
            /*SETUP VARS FOR NEW BOOKING*/
            NewBookingDetails newBookingDetails = new NewBookingDetails();
            CarTypeDetails carDetails = new CarTypeDetails("1", "A", 3, 992.10);
            CarDetails car = new CarDetails("5309118446", carDetails);
            DriverDetails driver = new DriverDetails("1", "Gretta Thake", new SimpleDateFormat("yyyy-MM-dd").parse("1966-10-18"), "22-9484786");
            
            newBookingDetails.setPickUpTime(new SimpleDateFormat("yyyy-MM-dd").parse("2019-04-15"));
            newBookingDetails.setDeliveryTime(new SimpleDateFormat("yyyy-MM-dd").parse("2019-04-20"));
            newBookingDetails.setPickUpPlace(new HotelDetails("24", "","",0,null,""));
            newBookingDetails.setDeliveryPlace(new HotelDetails("24", "","",0,null,""));
            newBookingDetails.setDriver(driver);
            newBookingDetails.setCar(car);
            
            
            /*SETUP VARS FOR AVAILABL CARS*/
            AvailabilityDetails availabilityDetails = new AvailabilityDetails(
                        new SimpleDateFormat("yyyy-MM-dd").parse("2019-04-15"), 
                        new SimpleDateFormat("yyyy-MM-dd").parse("2019-04-20"), 
                        new HotelDetails("19", "","",0,null,""));
            
            /*CONNECTING BOOKING CLIENT RMI*/
            IBooking bookingClient = (IBooking) Naming.lookup("//localhost/Booking");
            
            /*CONNECTING CAR CLIENT RMI*/
            ICars carClient = (ICars) Naming.lookup("//localhost/Cars");
            
            /*CREATE ADD BOOKING REQUEST AND PROCESSING*/
            bookingClient.addNewBooking(newBookingDetails);
            
            
            /*CREATE AND PROCESS CAR DATA*/
            List<CarDetails> list = carClient.getAvailableCars(availabilityDetails);
            
            JSONArray jsonArr = new JSONArray();
            
            for(CarDetails carD : list)
            {
                JSONObject jsonCar = new JSONObject();
                jsonCar.put("license", carD.getLicensePlate());
                jsonCar.put("type", carD.getCarType().getName());
                jsonCar.put("seats", carD.getCarType().getNumberOfSeats());
                jsonCar.put("price", carD.getCarType().getPricePerDay());
                jsonArr.put(jsonCar);
            }
                System.out.println(jsonArr.toString());
            
            
        } catch (NotBoundException | MalformedURLException | RemoteException | ParseException ex) {
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
